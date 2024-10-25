using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;

namespace ChatRoomRecorder
{
    public class ChatRoom : IDisposable
    {
        public event EventHandler UpdateCompleted;

        public ChatRoom(string url)
        {
            Tuple<ChatRoomWebsite, string, string> parsedUrl = ParseUrl(url);
            if (parsedUrl != null)
            {
                _id = 0;
                _website = parsedUrl.Item1;
                _name = parsedUrl.Item2;
                _status = ChatRoomStatus.Unknown;
                _action = ChatRoomAction.None;
                _chatRoomUrl = parsedUrl.Item3;
                _playlistUrl = String.Empty;
                _availableResolutions = new List<ChatRoomResolution>();
                _preferredResolution = ChatRoomResolution.MaxValue;
                _outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                _ffmpegPath = string.Format("{0}\\Streamlink\\ffmpeg\\ffmpeg.exe", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
                _streamlinkPath = string.Format("{0}\\Streamlink\\bin\\streamlink.exe", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
                _processes = new List<Process>();
                _fileName = string.Empty;
                _fileSize = -1;
                _lastUpdated = DateTime.MinValue;
                _lastSeen = DateTime.MinValue;
                _isUpdating = false;
                _disposingStarted = false;
                _disposingFinished = false;
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
                _browser = null;
                _browserInitialized = false;
                _timer = new System.Windows.Forms.Timer();
                _timer.Interval = 1000;
                _timer.Tick += _timer_Tick;
                _semaphore = new SemaphoreSlim(1, 1);
                _log = new ConcurrentQueue<Tuple<DateTime, string>>();
                Interlocked.Increment(ref s_totalCount);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void Dispose()
        {
            _semaphore.Wait();
            try
            {
                if (!_disposingStarted)
                {
                    _disposingStarted = true;
                    _cancellationTokenSource.Cancel();
                    _timer.Start();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_semaphore.CurrentCount > 0)
            {
                _semaphore.Wait();
                try
                {
                    if (_timer.Enabled && !_disposingFinished && (!_isUpdating || _isUpdating && (DateTime.Now - _lastUpdated).TotalSeconds >= c_updateLimit))
                    {
                        _timer.Stop();

                        foreach (Process process in _processes)
                        {
                            if (process.HasExited == false)
                            {
                                process.Kill();
                                process.WaitForExit();
                            }
                            process.Close();
                        }

                        if (_browser != null)
                        {
                            _browser.Dispose();
                        }

                        _disposingFinished = true;
                        Interlocked.Decrement(ref s_totalCount);
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        public void Update()
        {
            _semaphore.Wait();
            try
            {
                if (_disposingStarted || (DateTime.Now - _lastUpdated).TotalSeconds < c_updateLimit)
                {
                    return;
                }

                _isUpdating = true;
                _lastUpdated = DateTime.Now;

                //we're recording and that's what we want - return, otherwise - continue

                if (_action == ChatRoomAction.Record && _status == ChatRoomStatus.Record)
                {
                    if (_processes.Count > 0)
                    {
                        bool processesAreRunning = true;

                        foreach (Process process in _processes)
                        {
                            processesAreRunning = processesAreRunning && !process.HasExited;
                        }

                        if (processesAreRunning)
                        {
                            try
                            {
                                FileInfo fi = new(_fileName);
                                if (fi.Length > _fileSize)
                                {
                                    _fileSize = fi.Length;
                                    _isUpdating = false;

                                    return;
                                }
                            }
                            catch (Exception)
                            {
                                //do nothing
                            }
                        }
                    }
                }

                //stop any recording (if we need to restart it, we'll do it later)

                foreach (Process process in _processes)
                {
                    if (process.HasExited == false)
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    process.Close();
                }
                _processes.Clear();
                _fileName = string.Empty;
                _fileSize = -1;

                //we don't want to monitor or record, so clear data and return

                if (_action == ChatRoomAction.None)
                {
                    _status = ChatRoomStatus.Unknown;
                    _playlistUrl = String.Empty;
                    _availableResolutions.Clear();
                    if (_browser != null)
                    {
                        _browser.Dispose();
                        _browser = null;
                        _browserInitialized = false;
                    }
                    _isUpdating = false;

                    return;
                }

                //initialize the browser, if it's not ready

                if (_browser == null)
                {
                    _browser = new WebView2();
                    _browser.CoreWebView2InitializationCompleted += _browser_CoreWebView2InitializationCompleted;
                    _browser.Source = new Uri("about:blank", UriKind.Absolute);

                    return;
                }

                //update room's info

                if (_browserInitialized)
                {
                    switch (_website)
                    {
                        case ChatRoomWebsite.Chaturbate:
                            SendRequestChaturbate();
                            break;
                        case ChatRoomWebsite.BongaCams:
                            SendRequestBongaCams();
                            break;
                        case ChatRoomWebsite.Stripchat:
                            SendRequestStripchat();
                            break;
                    }
                }
            }
            finally
            {
                _semaphore.Release();

                if (!_isUpdating)
                {
                    UpdateCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void _browser_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            _semaphore.Wait();
            try
            {
                switch (_website)
                {
                    case ChatRoomWebsite.Chaturbate:
                        _browser.CoreWebView2.WebResourceResponseReceived += _browser_WebResourceResponseReceivedChaturbate;
                        break;
                    case ChatRoomWebsite.BongaCams:
                        _browser.CoreWebView2.WebResourceResponseReceived += _browser_WebResourceResponseReceivedBongaCams;
                        break;
                    case ChatRoomWebsite.Stripchat:
                        _browser.CoreWebView2.WebResourceResponseReceived += _browser_WebResourceResponseReceivedStripchat;
                        break;
                }
                _browser.CoreWebView2.NavigationCompleted += _browser_NavigationCompleted;
                _browserInitialized = true;
                _isUpdating = false;
            }
            finally
            {
                _semaphore.Release();

                UpdateCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        private void _browser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            _semaphore.Wait();
            try
            {
                if (_isUpdating && !e.IsSuccess)
                {
                    _status = ChatRoomStatus.Error;
                    _playlistUrl = String.Empty;
                    _availableResolutions.Clear();
                    _isUpdating = false;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void SendRequestChaturbate()
        {
            string postData = "room_slug=" + _name;
            CoreWebView2WebResourceRequest request = _browser.CoreWebView2.Environment.CreateWebResourceRequest(
                "https://chaturbate.com/get_edge_hls_url_ajax/",
                "POST",
                new MemoryStream(Encoding.UTF8.GetBytes(postData)),
                "Content-Type: application/x-www-form-urlencoded\r\nX-Requested-With: XMLHttpRequest");
            _browser.CoreWebView2.NavigateWithWebResourceRequest(request);
        }

        private void SendRequestBongaCams()
        {
            string postData = "model_search[display_name]=" + _name;
            CoreWebView2WebResourceRequest request = _browser.CoreWebView2.Environment.CreateWebResourceRequest(
                "https://bongacams.com/tools/listing_v3.php",
                "POST",
                new MemoryStream(Encoding.UTF8.GetBytes(postData)),
                "Content-Type: application/x-www-form-urlencoded\r\nX-Requested-With: XMLHttpRequest");
            _browser.CoreWebView2.NavigateWithWebResourceRequest(request);
        }

        private void SendRequestStripchat()
        {
            CoreWebView2WebResourceRequest request = _browser.CoreWebView2.Environment.CreateWebResourceRequest(
                string.Format("https://stripchat.com/api/front/v2/models/username/{0}/cam", _name),
                "GET",
                null,
                "Content-Type: application/x-www-form-urlencoded\r\nX-Requested-With: XMLHttpRequest");
            _browser.CoreWebView2.NavigateWithWebResourceRequest(request);
        }

        private async void _browser_WebResourceResponseReceivedChaturbate(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_isUpdating)
                {
                    return;
                }
            }
            finally
            {
                _semaphore.Release();
            }

            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = string.Empty;
            List<ChatRoomResolution> availableResolutions = new List<ChatRoomResolution>();

            try
            {
                string respStr = new StreamReader(await e.Response.GetContentAsync()).ReadToEnd();
                JsonDocument doc = JsonDocument.Parse(respStr);

                switch (doc.RootElement.GetProperty("room_status").GetString())
                {
                    case "public":
                        status = ChatRoomStatus.Public;
                        break;
                    case "private":
                        status = ChatRoomStatus.Private;
                        break;
                    case "hidden":
                        status = ChatRoomStatus.Hidden;
                        break;
                    case "away":
                        status = ChatRoomStatus.Away;
                        break;
                    case "offline":
                        status = ChatRoomStatus.Offline;
                        break;
                }

                AddLogEntry(string.Format("{0} - {1}", c_statusObtainedLogMessage, doc.RootElement.GetProperty("room_status").GetString()));

                if (status == ChatRoomStatus.Public)
                {
                    playlistUrl = doc.RootElement.GetProperty("url").GetString();

                    AddLogEntry(string.Format("{0} - {1}", c_playlistObtainedLogMessage, playlistUrl));

                    HttpRequestMessage reqMsg = new(HttpMethod.Get, playlistUrl);
                    reqMsg.Headers.Add("Accept-Encoding", "gzip");
                    HttpResponseMessage respMsg = await s_httpClient.SendAsync(reqMsg, _cancellationToken);
                    string[] playlist = (await respMsg.Content.ReadAsStringAsync()).Split('\n');
                    for (int i = 0; i < playlist.Length; i++)
                    {
                        MatchCollection matches = Regex.Matches(playlist[i], "^.*RESOLUTION=([0-9]*x[0-9]*).*$", RegexOptions.IgnoreCase);
                        if (matches.Count > 0)
                        {
                            availableResolutions.Add(ChatRoomResolution.Parse(matches[0].Groups[1].Value));
                        }
                    }

                    AddLogEntry(string.Format("{0} - {1}", c_resolutionsObtainedLogMessage, string.Join(", ", availableResolutions)));
                }

                if (status == ChatRoomStatus.Public && _action == ChatRoomAction.Record)
                {
                    status = ChatRoomStatus.Record;
                }

                if (status == ChatRoomStatus.Public || status == ChatRoomStatus.Record)
                {
                    int streamIndex = ChatRoomResolution.FindClosest(_preferredResolution, availableResolutions.ToArray());

                    if (streamIndex == -1)
                    {
                        throw new ArgumentException();
                    }

                    if (status == ChatRoomStatus.Record)
                    {
                        Record(playlistUrl, streamIndex);
                    }
                }

                if (status == ChatRoomStatus.Record || status == ChatRoomStatus.Public || status == ChatRoomStatus.Private || status == ChatRoomStatus.Hidden || status == ChatRoomStatus.Away)
                {
                    _lastSeen = DateTime.Now;
                }
            }
            catch (Exception)
            {
                status = ChatRoomStatus.Error;
                playlistUrl = string.Empty;
                availableResolutions.Clear();

                AddLogEntry(c_errorOccuredLogMessage);
            }

            await _semaphore.WaitAsync();
            try
            {
                _status = status;
                _playlistUrl = playlistUrl;
                _availableResolutions = availableResolutions;
                _isUpdating = false;
            }
            finally
            {
                _semaphore.Release();
            }

            UpdateCompleted?.Invoke(this, EventArgs.Empty);
        }

        private async void _browser_WebResourceResponseReceivedBongaCams(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_isUpdating)
                {
                    return;
                }
            }
            finally
            {
                _semaphore.Release();
            }

            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = string.Empty;
            List<ChatRoomResolution> availableResolutions = new List<ChatRoomResolution>();

            try
            {
                string respStr = new StreamReader(await e.Response.GetContentAsync()).ReadToEnd();
                JsonNode rootNode = JsonNode.Parse(respStr);
                JsonNode modelNode = null;
                
                for (int i = 0; i < (int)rootNode["total_count"] && modelNode == null; i++)
                {
                    JsonNode node = ((JsonArray)rootNode["models"])[i];
                    string name = (string)node["username"];
                    if (name != null && name.ToLower() == _name)
                    {
                        modelNode = node;
                    }
                }

                if (modelNode == null)
                {
                    throw new ArgumentException();
                }

                string esid = (string)modelNode["esid"];
                if (esid == null)
                {
                    throw new ArgumentException();
                }

                playlistUrl = string.Format("https://{0}.bcvcdn.com/hls/stream_{1}/playlist.m3u8", esid, (string)modelNode["username"]);

                HttpRequestMessage reqMsg = new(HttpMethod.Get, playlistUrl);
                HttpResponseMessage respMsg = await s_httpClient.SendAsync(reqMsg, _cancellationToken);
                if (respMsg.IsSuccessStatusCode)
                {
                    status = ChatRoomStatus.Public;
                }
                else
                {
                    status = ChatRoomStatus.Offline;
                }

                AddLogEntry(string.Format("{0} - {1}", c_statusObtainedLogMessage, status));

                if (status == ChatRoomStatus.Public)
                {
                    AddLogEntry(string.Format("{0} - {1}", c_playlistObtainedLogMessage, playlistUrl));

                    string[] playlist = (await respMsg.Content.ReadAsStringAsync()).Split('\n');
                    for (int i = 0; i < playlist.Length; i++)
                    {
                        MatchCollection matches = Regex.Matches(playlist[i], "^.*RESOLUTION=([0-9]*x[0-9]*).*$", RegexOptions.IgnoreCase);
                        if (matches.Count > 0)
                        {
                            availableResolutions.Add(ChatRoomResolution.Parse(matches[0].Groups[1].Value));
                        }
                    }

                    AddLogEntry(string.Format("{0} - {1}", c_resolutionsObtainedLogMessage, string.Join(", ", availableResolutions)));
                }

                if (status == ChatRoomStatus.Public && _action == ChatRoomAction.Record)
                {
                    status = ChatRoomStatus.Record;
                }

                if (status == ChatRoomStatus.Public || status == ChatRoomStatus.Record)
                {
                    int streamIndex = ChatRoomResolution.FindClosest(_preferredResolution, availableResolutions.ToArray());

                    if (streamIndex == -1)
                    {
                        throw new ArgumentException();
                    }

                    if (status == ChatRoomStatus.Record)
                    {
                        Record(playlistUrl, streamIndex);
                    }
                }

                if (status == ChatRoomStatus.Record || status == ChatRoomStatus.Public)
                {
                    _lastSeen = DateTime.Now;
                }
            }
            catch (Exception)
            {
                status = ChatRoomStatus.Error;
                playlistUrl = String.Empty;
                availableResolutions.Clear();

                AddLogEntry(c_errorOccuredLogMessage);
            }

            await _semaphore.WaitAsync();
            try
            {
                _status = status;
                _playlistUrl = playlistUrl;
                _availableResolutions = availableResolutions;
                _isUpdating = false;
            }
            finally
            {
                _semaphore.Release();
            }

            UpdateCompleted?.Invoke(this, EventArgs.Empty);
        }

        private async void _browser_WebResourceResponseReceivedStripchat(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_isUpdating)
                {
                    return;
                }
            }
            finally
            {
                _semaphore.Release();
            }

            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = string.Empty;
            List<ChatRoomResolution> availableResolutions = new List<ChatRoomResolution>();

            try
            {
                string respStr = new StreamReader(await e.Response.GetContentAsync()).ReadToEnd();
                JsonNode rootNode = JsonNode.Parse(respStr);
                
                switch ((string)rootNode["user"]["user"]["status"])
                {
                    case "public":
                        status = ChatRoomStatus.Public;
                        break;
                    case "virtualPrivate":
                        status = ChatRoomStatus.Private;
                        break;
                    case "private":
                        status = ChatRoomStatus.Private;
                        break;
                    case "p2p":
                        status = ChatRoomStatus.Private;
                        break;
                    case "p2pVoice":
                        status = ChatRoomStatus.Private;
                        break;
                    case "groupShow":
                        status = ChatRoomStatus.Group;
                        break;
                    case "idle":
                        status = ChatRoomStatus.Idle;
                        break;
                    case "off":
                        status = ChatRoomStatus.Offline;
                        break;
                }

                AddLogEntry(string.Format("{0} - {1}", c_statusObtainedLogMessage, rootNode["user"]["user"]["status"]));

                if (status == ChatRoomStatus.Public)
                {
                    playlistUrl = string.Format("https://b-hls-{0:00}.doppiocdn.com/hls/{1}/{1}.m3u8", s_random.Next(1, 26), rootNode["cam"]["streamName"]);

                    AddLogEntry(string.Format("{0} - {1}", c_playlistObtainedLogMessage, playlistUrl));

                    availableResolutions.Add(new ChatRoomResolution((short)rootNode["cam"]["broadcastSettings"]["width"], (short)rootNode["cam"]["broadcastSettings"]["height"]));
                    JsonArray presetsNode = (JsonArray)rootNode["cam"]["broadcastSettings"]["presets"]["default"];
                    for (int i = 0; i < presetsNode.Count; i++)
                    {
                        ChatRoomResolution resolution = ChatRoomResolution.MinValue;
                        switch ((string)presetsNode[i])
                        {
                            case "1080p":
                                resolution = new ChatRoomResolution(1920, 1080);
                                break;
                            case "720p":
                                resolution = new ChatRoomResolution(1280, 720);
                                break;
                            case "480p":
                                resolution = new ChatRoomResolution(854, 480);
                                break;
                            case "240p":
                                resolution = new ChatRoomResolution(426, 240);
                                break;
                        }
                        if (resolution != ChatRoomResolution.MinValue && !availableResolutions.Contains(resolution))
                        {
                            availableResolutions.Add(resolution);
                        }
                    }

                    AddLogEntry(string.Format("{0} - {1}", c_resolutionsObtainedLogMessage, string.Join(", ", availableResolutions)));
                }

                if (status == ChatRoomStatus.Public && _action == ChatRoomAction.Record)
                {
                    status = ChatRoomStatus.Record;
                }

                if (status == ChatRoomStatus.Public || status == ChatRoomStatus.Record)
                {
                    int streamIndex = ChatRoomResolution.FindClosest(_preferredResolution, availableResolutions.ToArray());

                    if (streamIndex == -1)
                    {
                        throw new ArgumentException();
                    }

                    if (status == ChatRoomStatus.Record)
                    {
                        if (streamIndex > 0)
                        {
                            playlistUrl = string.Format("{0}_{1}p{2}",
                                playlistUrl.Substring(0, playlistUrl.LastIndexOf(".")),
                                availableResolutions[streamIndex].Height.ToString(),
                                playlistUrl.Substring(playlistUrl.LastIndexOf(".")));
                        }

                        Record(playlistUrl, -1);
                    }
                }

                if (status == ChatRoomStatus.Record || status == ChatRoomStatus.Public || status == ChatRoomStatus.Private || status == ChatRoomStatus.Group)
                {
                    _lastSeen = DateTime.Now;
                }
            }
            catch (Exception)
            {
                status = ChatRoomStatus.Error;
                playlistUrl = string.Empty;
                availableResolutions.Clear();

                AddLogEntry(c_errorOccuredLogMessage);
            }

            await _semaphore.WaitAsync();
            try
            {
                _status = status;
                _playlistUrl = playlistUrl;
                _availableResolutions = availableResolutions;
                _isUpdating = false;
            }
            finally
            {
                _semaphore.Release();
            }

            UpdateCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void Record(string playlistUrl, int streamIndex)
        {
            _fileName = string.Format("{0}\\{1} {2} {3}.ts", _outputDirectory, _website, _name, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")).ToLower();
            _fileSize = 0;

            List<ProcessStartInfo> psis = new List<ProcessStartInfo>();

            switch (_website)
            {
                case ChatRoomWebsite.Chaturbate:
                    psis.Add(new ProcessStartInfo()
                    {
                        FileName = _ffmpegPath,
                        Arguments = string.Format("-analyzeduration 16M -i \"{0}\" -map 0:p:{1} -c copy \"{2}\"", playlistUrl, streamIndex, _fileName)
                    });
                    break;
                case ChatRoomWebsite.BongaCams:
                    psis.Add(new ProcessStartInfo()
                    {
                        FileName = _ffmpegPath,
                        Arguments = string.Format("-analyzeduration 16M -i \"{0}\" -map 0:p:{1} -c copy \"{2}\"", playlistUrl, streamIndex, _fileName)
                    });
                    break;
                case ChatRoomWebsite.Stripchat:
                    int port = s_random.Next(16384, 65536);
                    psis.Add(new ProcessStartInfo()
                    {
                        FileName = _streamlinkPath,
                        Arguments = string.Format("--player-external-http --player-external-http-continuous false --player-external-http-interface 127.0.0.1 --player-external-http-port {0} " +
                            "--default-stream best --stream-segment-timeout 60 --stream-timeout 60 --http-timeout 60 --hls-segment-queue-threshold 99 {1}", port, playlistUrl)
                    });
                    psis.Add(new ProcessStartInfo()
                    {
                        FileName = _ffmpegPath,
                        Arguments = string.Format("-reconnect 1 -reconnect_at_eof 1 -reconnect_on_network_error 1 -reconnect_on_http_error 1 -reconnect_streamed 1 -i http://127.0.0.1:{0} -c copy \"{1}\"", port, _fileName)
                    });
                    break;
            }

            foreach (ProcessStartInfo psi in psis)
            {
                psi.UseShellExecute = false;
                psi.LoadUserProfile = false;
                psi.CreateNoWindow = true;
                _processes.Add(Process.Start(psi));

                AddLogEntry(string.Format("{0} - {1} {2}", c_recordingStartedLogMessage, psi.FileName, psi.Arguments));
            }
        }

        private void AddLogEntry(string info)
        {
            _log.Enqueue(new Tuple<DateTime, string>(DateTime.Now, info));
        }

        public static Tuple<ChatRoomWebsite, string, string> ParseUrl(string url)
        {
            if (url == null || url.Length == 0)
            {
                return null;
            }

            url = url.ToLower();
            if (url[url.Length - 1] != '/')
            {
                url += '/';
            }

            MatchCollection matches;

            if ((matches = Regex.Matches(url, @"^https://chaturbate.com/([^/]+)/.*$", RegexOptions.IgnoreCase)).Count > 0 ||
                (matches = Regex.Matches(url, @"^chaturbate[ ]+([^ ]+).*/$", RegexOptions.IgnoreCase)).Count > 0)

            {
                return Tuple.Create(ChatRoomWebsite.Chaturbate, matches[0].Groups[1].Value, string.Format("https://chaturbate.com/{0}/", matches[0].Groups[1].Value));
            }

            if ((matches = Regex.Matches(url, @"^https://(?:[a-z0-9-.]+.)?bongacams[0-9]*.com/([^/]+(?=#!/.*$)|[^/]+(?=/.*$))", RegexOptions.IgnoreCase)).Count > 0 ||
                (matches = Regex.Matches(url, @"^bongacams[ ]+([^ ]+).*/$", RegexOptions.IgnoreCase)).Count > 0)
            {
                return Tuple.Create(ChatRoomWebsite.BongaCams, matches[0].Groups[1].Value, string.Format("https://bongacams.com/{0}/", matches[0].Groups[1].Value));
            }

            if ((matches = Regex.Matches(url, @"^https://(?:[a-z0-9-.]+.)?stripchat.com/([^/]+)/.*$", RegexOptions.IgnoreCase)).Count > 0 ||
                (matches = Regex.Matches(url, @"^stripchat[ ]+([^ ]+).*/$", RegexOptions.IgnoreCase)).Count > 0)
            {
                return Tuple.Create(ChatRoomWebsite.Stripchat, matches[0].Groups[1].Value, string.Format("https://stripchat.com/{0}/", matches[0].Groups[1].Value));
            }

            return null;
        }

        public int ID
        {
            set
            {
                _id = value;
            }
            get
            {
                return _id;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public ChatRoomWebsite Website
        {
            get
            {
                return _website;
            }
        }

        public ChatRoomStatus Status
        {
            get
            {
                return _status;
            }
        }

        public ChatRoomAction Action
        {
            set
            {
                _action = value;
            }
            get
            {
                return _action;
            }
        }

        public string ChatRoomUrl
        {
            get
            {
                return _chatRoomUrl;
            }
        }

        public string PlaylistUrl
        {
            get
            {
                return _playlistUrl;
            }
        }

        public ChatRoomResolution[] AvailableResolutions
        {
            get
            {
                return _availableResolutions.ToArray();
            }
        }

        public ChatRoomResolution PreferredResolution
        {
            set
            {
                _preferredResolution = value;
            }
            get
            {
                return _preferredResolution;
            }
        }

        public string OutputDirectory
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    _outputDirectory = value;
                }
            }
            get
            {
                return _outputDirectory;
            }
        }

        public string FFmpegPath
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    _ffmpegPath = value;
                }
            }
            get
            {
                return _ffmpegPath;
            }
        }

        public string StreamlinkPath
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    _streamlinkPath = value;
                }
            }
            get
            {
                return _streamlinkPath;
            }
        }

        public DateTime LastUpdated
        {
            set
            {
                _lastUpdated = value;
            }
            get
            {
                return _lastUpdated;
            }
        }

        public DateTime LastSeen
        {
            set
            {
                _lastSeen = value;
            }
            get
            {
                return _lastSeen;
            }
        }

        public bool DisposingStarted
        {
            get
            {
                return _disposingStarted;
            }
        }

        public bool DisposingFinished
        {
            get
            {
                return _disposingFinished;
            }
        }

        public ConcurrentQueue<Tuple<DateTime, string>> Log
        {
            get
            {
                return _log;
            }
        }

        public static int TotalCount
        {
            get
            {
                return s_totalCount;
            }
        }

        private int _id;
        private string _name;
        private ChatRoomWebsite _website;
        private ChatRoomStatus _status;
        private ChatRoomAction _action;
        private string _chatRoomUrl;
        private string _playlistUrl;
        private List<ChatRoomResolution> _availableResolutions;
        private ChatRoomResolution _preferredResolution;
        private string _outputDirectory;
        private string _ffmpegPath;
        private string _streamlinkPath;
        private List<Process> _processes;
        private string _fileName;
        private long _fileSize;
        private DateTime _lastUpdated;
        private DateTime _lastSeen;
        private bool _isUpdating;
        private bool _disposingStarted;
        private bool _disposingFinished;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private WebView2 _browser;
        private bool _browserInitialized;
        private System.Windows.Forms.Timer _timer;
        private SemaphoreSlim _semaphore;
        private ConcurrentQueue<Tuple<DateTime, string>> _log;

        private const int c_updateLimit = 60;

        private static HttpClient s_httpClient = new(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });
        private static Random s_random = new Random();
        private static int s_totalCount = 0;

        private const string c_statusObtainedLogMessage = "status";
        private const string c_playlistObtainedLogMessage = "playlist";
        private const string c_resolutionsObtainedLogMessage = "resolutions";
        private const string c_recordingStartedLogMessage = "recording";
        private const string c_errorOccuredLogMessage = "error occured";
    }
}
