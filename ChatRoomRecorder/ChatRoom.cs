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
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChatRoomRecorder
{
    public class ChatRoom : IDisposable
    {
        public static Tuple<ChatRoomWebsite, string, string> ParseUrl(string url)
        {
            if (url == null || url.Length == 0)
            {
                return null;
            }

            url = url.Trim().ToLower();
            if (url[url.Length - 1] != '/')
            {
                url += '/';
            }

            MatchCollection matches;
            
            RegexOptions regexOptions = RegexOptions.Compiled & RegexOptions.Singleline & RegexOptions.CultureInvariant & RegexOptions.IgnoreCase;
            
            if ((matches = Regex.Matches(url, @"^https://chaturbate.com/([^/]+)/.*$", regexOptions)).Count > 0 ||
                (matches = Regex.Matches(url, @"^chaturbate[ ]+([^ ]+).*/$", regexOptions)).Count > 0)

            {
                return Tuple.Create(ChatRoomWebsite.Chaturbate, matches[0].Groups[1].Value, string.Format("https://chaturbate.com/{0}/", matches[0].Groups[1].Value));
            }

            if ((matches = Regex.Matches(url, @"^https://(?:[a-z0-9-.]+.)?bongacams[0-9]*.(?:com|cam)/([^/]+(?=#!/.*$)|[^/]+(?=/.*$))", regexOptions)).Count > 0 ||
                (matches = Regex.Matches(url, @"^bongacams[ ]+([^ ]+).*/$", regexOptions)).Count > 0)
            {
                return Tuple.Create(ChatRoomWebsite.BongaCams, matches[0].Groups[1].Value, string.Format("https://bongacams.com/{0}/", matches[0].Groups[1].Value));
            }

            if ((matches = Regex.Matches(url, @"^https://(?:[a-z0-9-.]+.)?stripchat.com/([^/]+)/.*$", regexOptions)).Count > 0 ||
                (matches = Regex.Matches(url, @"^stripchat[ ]+([^ ]+).*/$", regexOptions)).Count > 0)
            {
                return Tuple.Create(ChatRoomWebsite.Stripchat, matches[0].Groups[1].Value, string.Format("https://stripchat.com/{0}/", matches[0].Groups[1].Value));
            }

            if ((matches = Regex.Matches(url, @"^https://www.flirt4free.com/\?model=([^/]+)/.*$", regexOptions)).Count > 0 ||
                (matches = Regex.Matches(url, @"^flirt4free[ ]+([^ ]+).*/$", regexOptions)).Count > 0)
            {
                return Tuple.Create(ChatRoomWebsite.Flirt4Free, matches[0].Groups[1].Value, string.Format("https://www.flirt4free.com/?model={0}/", matches[0].Groups[1].Value));
            }

            if ((matches = Regex.Matches(url, @"^https://www.camsoda.com/([^/]+)/.*$", regexOptions)).Count > 0 ||
                (matches = Regex.Matches(url, @"^camsoda[ ]+([^ ]+).*/$", regexOptions)).Count > 0)
            {
                return Tuple.Create(ChatRoomWebsite.CamSoda, matches[0].Groups[1].Value, string.Format("https://www.camsoda.com/{0}/", matches[0].Groups[1].Value));
            }

            if ((matches = Regex.Matches(url, @"^https://(?:[a-z0-9-.]+.)?cam4.com/([^/]+)/.*$", regexOptions)).Count > 0 ||
                (matches = Regex.Matches(url, @"^cam4[ ]+([^ ]+).*/$", regexOptions)).Count > 0)
            {
                return Tuple.Create(ChatRoomWebsite.Cam4, matches[0].Groups[1].Value, string.Format("https://cam4.com/{0}/", matches[0].Groups[1].Value));
            }

            return null;
        }

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
                _delay = 0;
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
                _uri = string.Empty;
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
                        case ChatRoomWebsite.Flirt4Free:
                            SendRequestFlirt4Free();
                            break;
                        case ChatRoomWebsite.CamSoda:
                            SendRequestCamSoda();
                            break;
                        case ChatRoomWebsite.Cam4:
                            SendRequestCam4();
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

        private async void _timer_Tick(object sender, EventArgs e)
        {
            if (_semaphore.CurrentCount > 0)
            {
                await _semaphore.WaitAsync();
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

        private async void _browser_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            await _semaphore.WaitAsync();
            try
            {
                _browser.CoreWebView2.WebResourceResponseReceived += _browser_WebResourceResponseReceived;
                _browser.CoreWebView2.NavigationCompleted += _browser_NavigationCompleted;
                _browserInitialized = true;
                _isUpdating = false;
            }
            finally
            {
                _semaphore.Release();
            }

            UpdateCompleted?.Invoke(this, EventArgs.Empty);
        }

        private async void _browser_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            string template = @"https://[^/]+/(.*)";
            MatchCollection matches1;
            MatchCollection matches2;

            if (_isUpdating &&
                e.Response.StatusCode == 200 && 
                (matches1 = Regex.Matches(_uri, template, RegexOptions.IgnoreCase)).Count > 0 &&
                (matches2 = Regex.Matches(e.Request.Uri, template, RegexOptions.IgnoreCase)).Count > 0 &&
                matches1[0].Groups[1].Value == matches2[0].Groups[1].Value)
            {
                try
                {
                    string response = await (new StreamReader(await e.Response.GetContentAsync())).ReadToEndAsync();

                    await Task.Factory.StartNew(async () =>
                    {
                        await _semaphore.WaitAsync();
                        try
                        {
                            switch (_website)
                            {
                                case ChatRoomWebsite.Chaturbate:
                                    ProcessResponseChaturbate(response);
                                    break;
                                case ChatRoomWebsite.BongaCams:
                                    ProcessResponseBongaCams(response);
                                    break;
                                case ChatRoomWebsite.Stripchat:
                                    ProcessResponseStripchat(response);
                                    break;
                                case ChatRoomWebsite.Flirt4Free:
                                    ProcessResponseFlirt4Free(response);
                                    break;
                                case ChatRoomWebsite.CamSoda:
                                    ProcessResponseCamSoda(response);
                                    break;
                                case ChatRoomWebsite.Cam4:
                                    ProcessResponseCam4(response);
                                    break;
                            }

                            _isUpdating = false;
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }, TaskCreationOptions.LongRunning).WaitAsync(_cancellationToken);

                    UpdateCompleted?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    AddLogEntry(string.Format("{0} - {1}", c_errorOccuredLogMessage, exception.Message));
                }
            }
        }

        private async void _browser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_isUpdating && !e.IsSuccess)
                {
                    _status = ChatRoomStatus.Error;
                    _playlistUrl = String.Empty;
                    _availableResolutions.Clear();

                    _browser.Dispose();
                    _browser = null;
                    _browserInitialized = false;

                    _isUpdating = false;

                    AddLogEntry(string.Format("{0} - {1}", c_errorOccuredLogMessage, e.WebErrorStatus.ToString()));
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async void SendRequestChaturbate()
        {
            try
            {
                await Task.Delay(s_random.Next(0, _delay * 1000), _cancellationToken);

                if (_browser.Source == new Uri("about:blank"))
                {
                    _uri = string.Format("https://chaturbate.com/api/chatvideocontext/{0}/", _name);

                    CoreWebView2WebResourceRequest request = _browser.CoreWebView2.Environment.CreateWebResourceRequest(
                        _uri,
                        "GET",
                        null,
                        string.Empty);
                    _browser.CoreWebView2.NavigateWithWebResourceRequest(request);
                }
                else
                {
                    _browser.Reload();
                }
            }
            catch
            {
                //do nothing
            }
        }

        private async void SendRequestBongaCams()
        {
            try
            {
                await Task.Delay(s_random.Next(0, _delay * 1000), _cancellationToken);

                if (_browser.Source == new Uri("about:blank"))
                {
                    _uri = "https://bongacams.com/tools/listing_v3.php";

                    string postData = "model_search[display_name]=" + _name;
                    CoreWebView2WebResourceRequest request = _browser.CoreWebView2.Environment.CreateWebResourceRequest(
                        _uri,
                        "POST",
                        new MemoryStream(Encoding.UTF8.GetBytes(postData)),
                        "Content-Type: application/x-www-form-urlencoded\r\nX-Requested-With: XMLHttpRequest");
                    _browser.CoreWebView2.NavigateWithWebResourceRequest(request);
                }
                else
                {
                    _browser.Reload();
                }
            }
            catch
            {
                //do nothing
            }
        }

        private async void SendRequestStripchat()
        {
            try
            {
                await Task.Delay(s_random.Next(0, _delay * 1000), _cancellationToken);

                if (_browser.Source == new Uri("about:blank"))
                {
                    _uri = string.Format("https://stripchat.com/api/front/v2/models/username/{0}/cam", _name);

                    CoreWebView2WebResourceRequest request = _browser.CoreWebView2.Environment.CreateWebResourceRequest(
                        _uri,
                        "GET",
                        null,
                        "Content-Type: application/x-www-form-urlencoded\r\nX-Requested-With: XMLHttpRequest");
                    _browser.CoreWebView2.NavigateWithWebResourceRequest(request);
                }
                else
                {
                    _browser.Reload();
                }
            }
            catch
            {
                //do nothing
            }
        }

        private async void SendRequestFlirt4Free()
        {
            try
            {
                await Task.Delay(s_random.Next(0, _delay * 1000), _cancellationToken);

                if (_browser.Source == new Uri("about:blank"))
                {
                    _uri = "https://www.flirt4free.com/?tpl=index2&model=json";

                    CoreWebView2WebResourceRequest request = _browser.CoreWebView2.Environment.CreateWebResourceRequest(
                        _uri,
                        "GET",
                        null,
                        "Content-Type: application/x-www-form-urlencoded\r\nX-Requested-With: XMLHttpRequest");
                    _browser.CoreWebView2.NavigateWithWebResourceRequest(request);
                }
                else
                {
                    _browser.Reload();
                }
            }
            catch
            {
                //do nothing
            }
        }

        private async void SendRequestCamSoda()
        {
            try
            {
                await Task.Delay(s_random.Next(0, _delay * 1000), _cancellationToken);

                if (_browser.Source == new Uri("about:blank"))
                {
                    _uri = string.Format("https://www.camsoda.com/api/v1/video/vtoken/{0}", _name);

                    CoreWebView2WebResourceRequest request = _browser.CoreWebView2.Environment.CreateWebResourceRequest(
                        _uri,
                        "GET",
                        null,
                        "Content-Type: application/x-www-form-urlencoded\r\nX-Requested-With: XMLHttpRequest");
                    _browser.CoreWebView2.NavigateWithWebResourceRequest(request);
                }
                else
                {
                    _browser.Reload();
                }
            }
            catch
            {
                //do nothing
            }
        }

        private async void SendRequestCam4()
        {
            try
            {
                await Task.Delay(s_random.Next(0, _delay * 1000), _cancellationToken);

                if (_browser.Source == new Uri("about:blank"))
                {
                    _uri = string.Format("https://cam4.com/directoryCams?directoryJson=true&online=true&username={0}", _name);

                    CoreWebView2WebResourceRequest request = _browser.CoreWebView2.Environment.CreateWebResourceRequest(
                        _uri,
                        "GET",
                        null,
                        "Content-Type: application/x-www-form-urlencoded\r\nX-Requested-With: XMLHttpRequest");
                    _browser.CoreWebView2.NavigateWithWebResourceRequest(request);
                }
                else
                {
                    _browser.Reload();
                }
            }
            catch
            {
                //do nothing
            }
        }

        private void ProcessResponseChaturbate(string response)
        {
            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = string.Empty;
            List<ChatRoomResolution> availableResolutions = new List<ChatRoomResolution>();

            try
            {
                JsonNode chatRoomNode = JsonNode.Parse(response);

                switch ((string)chatRoomNode["room_status"])
                {
                    case "public":
                        status = ChatRoomStatus.Public;
                        break;
                    case "private":
                        status = ChatRoomStatus.Private;
                        break;
                    case "password protected":
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

                AddLogEntry(string.Format("{0} - {1} ('{2}')", c_statusObtainedLogMessage, status, (string)chatRoomNode["room_status"]));

                if (status == ChatRoomStatus.Public)
                {
                    playlistUrl = (string)chatRoomNode["hls_source"];

                    AddLogEntry(string.Format("{0} - {1}", c_playlistObtainedLogMessage, playlistUrl));

                    HttpRequestMessage reqMsg = new(HttpMethod.Get, playlistUrl);
                    reqMsg.Headers.Add("Accept-Encoding", "gzip");
                    HttpResponseMessage respMsg = s_httpClient.Send(reqMsg, _cancellationToken);
                    string[] playlist = new StreamReader(respMsg.Content.ReadAsStream()).ReadToEnd().Split('\n');
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
            }
            catch (Exception exception)
            {
                status = ChatRoomStatus.Error;
                playlistUrl = string.Empty;
                availableResolutions.Clear();

                AddLogEntry(string.Format("{0} - {1}", c_errorOccuredLogMessage, exception.Message));
            }
            finally
            {
                _status = status;
                _playlistUrl = playlistUrl;
                _availableResolutions = availableResolutions;
            }

            if (_status != ChatRoomStatus.Offline && _status != ChatRoomStatus.Error && _status != ChatRoomStatus.Unknown)
            {
                _lastSeen = DateTime.Now;
            }
        }

        private void ProcessResponseBongaCams(string response)
        {
            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = string.Empty;
            List<ChatRoomResolution> availableResolutions = new List<ChatRoomResolution>();

            try
            {
                JsonNode rootNode = JsonNode.Parse(response);
                JsonNode chatRoomNode = null;
                
                for (int i = 0; i < (int)rootNode["total_count"] && chatRoomNode == null; i++)
                {
                    JsonNode node = ((JsonArray)rootNode["models"])[i];
                    string name = (string)node["username"];
                    if (name != null && name.ToLower() == _name)
                    {
                        chatRoomNode = node;
                    }
                }

                if (chatRoomNode == null)
                {
                    throw new ArgumentException();
                }

                string esid = (string)chatRoomNode["esid"];

                if (esid == null)
                {
                    throw new ArgumentException();
                }

                playlistUrl = string.Format("https://{0}.bcvcdn.com/hls/stream_{1}/playlist.m3u8", esid, (string)chatRoomNode["username"]);

                HttpRequestMessage reqMsg = new(HttpMethod.Get, playlistUrl);
                HttpResponseMessage respMsg = s_httpClient.Send(reqMsg, _cancellationToken);
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

                    string[] playlist = new StreamReader(respMsg.Content.ReadAsStream()).ReadToEnd().Split('\n');
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
            }
            catch (Exception exception)
            {
                status = ChatRoomStatus.Error;
                playlistUrl = String.Empty;
                availableResolutions.Clear();

                AddLogEntry(string.Format("{0} - {1}", c_errorOccuredLogMessage, exception.Message));
            }
            finally
            {
                _status = status;
                _playlistUrl = playlistUrl;
                _availableResolutions = availableResolutions;
            }

            if (_status != ChatRoomStatus.Offline && _status != ChatRoomStatus.Error && _status != ChatRoomStatus.Unknown)
            {
                _lastSeen = DateTime.Now;
            }
        }

        private void ProcessResponseStripchat(string response)
        {
            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = string.Empty;
            List<ChatRoomResolution> availableResolutions = new List<ChatRoomResolution>();

            try
            {
                JsonNode rootNode = JsonNode.Parse(response);
                
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
                    case "":
                        status = ChatRoomStatus.Hidden;
                        break;
                }

                AddLogEntry(string.Format("{0} - {1} ('{2}')", c_statusObtainedLogMessage, status, rootNode["user"]["user"]["status"]));

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

                        Record(playlistUrl, null);
                    }
                }
            }
            catch (Exception exception)
            {
                status = ChatRoomStatus.Error;
                playlistUrl = String.Empty;
                availableResolutions.Clear();

                AddLogEntry(string.Format("{0} - {1}", c_errorOccuredLogMessage, exception.Message));
            }
            finally
            {
                _status = status;
                _playlistUrl = playlistUrl;
                _availableResolutions = availableResolutions;
            }

            if (_status != ChatRoomStatus.Offline && _status != ChatRoomStatus.Error && _status != ChatRoomStatus.Unknown)
            {
                _lastSeen = DateTime.Now;
            }
        }

        private void ProcessResponseFlirt4Free(string response)
        {
            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = string.Empty;
            List<ChatRoomResolution> availableResolutions = new List<ChatRoomResolution>();

            try
            {
                string name = string.Format("\"{0}\"", _name);
                int pos = response.IndexOf(name);
                int first = -1;
                int last = -1;
                int k = 0;
                for (int i = pos - 1; i >= 0 && first == -1; i--)
                {
                    switch (response[i])
                    {
                        case '{': if (k == 0) first = i; else k--; break;
                        case '}': k++; break;
                    }
                }
                for (int i = pos + name.Length; i < response.Length && first != -1 && last == -1; i++)
                {
                    switch (response[i])
                    {
                        case '}': if (k == 0) last = i; else k--; break;
                        case '{': k++; break;
                    }
                }
                JsonNode chatRoomNode = JsonNode.Parse((first != -1 && last != -1) ? response.Substring(first, last - first + 1) : "{\"room_status_char\":\"\"}");

                switch ((string)chatRoomNode["room_status_char"])
                {
                    case "O":
                        status = ChatRoomStatus.Public;
                        break;
                    case "P":
                        status = ChatRoomStatus.Private;
                        break;
                    case "F":
                        status = ChatRoomStatus.Private;
                        break;
                    case "":
                        status = ChatRoomStatus.Offline;
                        break;
                }

                AddLogEntry(string.Format("{0} - {1} ('{2}')", c_statusObtainedLogMessage, status, chatRoomNode["room_status_char"]));

                if (status == ChatRoomStatus.Public)
                {
                    int id = Convert.ToInt32((string)chatRoomNode["model_id"]);

                    string streamInfoUrl = string.Format("https://www.flirt4free.com/ws/chat/get-stream-urls.php?model_id={0}", id);
                    HttpRequestMessage reqMsg = new(HttpMethod.Get, streamInfoUrl);
                    HttpResponseMessage respMsg = s_httpClient.Send(reqMsg, _cancellationToken);
                    JsonNode streamInfoNode = JsonNode.Parse(new StreamReader(respMsg.Content.ReadAsStream()).ReadToEnd());
                    playlistUrl = string.Format("https:{0}", (string)streamInfoNode["data"]["hls"][0]["url"]);

                    AddLogEntry(string.Format("{0} - {1}", c_playlistObtainedLogMessage, playlistUrl));

                    reqMsg = new(HttpMethod.Get, playlistUrl);
                    respMsg = s_httpClient.Send(reqMsg, _cancellationToken);
                    string[] playlist = new StreamReader(respMsg.Content.ReadAsStream()).ReadToEnd().Split('\n');
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
            }
            catch (Exception exception)
            {
                status = ChatRoomStatus.Error;
                playlistUrl = String.Empty;
                availableResolutions.Clear();

                AddLogEntry(string.Format("{0} - {1}", c_errorOccuredLogMessage, exception.Message));
            }
            finally
            {
                _status = status;
                _playlistUrl = playlistUrl;
                _availableResolutions = availableResolutions;
            }

            if (_status != ChatRoomStatus.Offline && _status != ChatRoomStatus.Error && _status != ChatRoomStatus.Unknown)
            {
                _lastSeen = DateTime.Now;
            }
        }

        private void ProcessResponseCamSoda(string response)
        {
            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = string.Empty;
            List<ChatRoomResolution> availableResolutions = new List<ChatRoomResolution>();

            try
            {
                JsonNode chatRoomNode = JsonNode.Parse(response);
                JsonArray serversNode = (JsonArray)chatRoomNode["edge_servers"];
                string streamName = (string)chatRoomNode["stream_name"];

                if (streamName == string.Empty)
                {
                    status = ChatRoomStatus.Offline;
                } 
                else if (serversNode.Count == 0)
                {
                    status = ChatRoomStatus.Private;
                } 
                else
                {
                    status = ChatRoomStatus.Public;
                }

                AddLogEntry(string.Format("{0} - {1}", c_statusObtainedLogMessage, status));

                if (status == ChatRoomStatus.Public)
                {
                    playlistUrl = string.Format("https://{0}/{1}_v1/playlist.m3u8", (string)serversNode[0], streamName);

                    AddLogEntry(string.Format("{0} - {1}", c_playlistObtainedLogMessage, playlistUrl));

                    HttpRequestMessage reqMsg = new(HttpMethod.Get, playlistUrl);
                    HttpResponseMessage respMsg = s_httpClient.Send(reqMsg, _cancellationToken);
                    string[] playlist = new StreamReader(respMsg.Content.ReadAsStream()).ReadToEnd().Split('\n');
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
                        Record(playlistUrl, string.Format("{0}p", availableResolutions[streamIndex].Height.ToString()));
                    }
                }
            }
            catch (Exception exception)
            {
                status = ChatRoomStatus.Error;
                playlistUrl = String.Empty;
                availableResolutions.Clear();

                AddLogEntry(string.Format("{0} - {1}", c_errorOccuredLogMessage, exception.Message));
            }
            finally
            {
                _status = status;
                _playlistUrl = playlistUrl;
                _availableResolutions = availableResolutions;
            }

            if (_status != ChatRoomStatus.Offline && _status != ChatRoomStatus.Error && _status != ChatRoomStatus.Unknown)
            {
                _lastSeen = DateTime.Now;
            }
        }

        private void ProcessResponseCam4(string response)
        {
            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = string.Empty;
            List<ChatRoomResolution> availableResolutions = new List<ChatRoomResolution>();

            try
            {
                JsonNode chatRoomNode = null;

                JsonArray allNodes = (JsonArray)JsonNode.Parse(response);
                if (allNodes.Count > 0)
                {
                    chatRoomNode = allNodes[0];

                    switch ((string)chatRoomNode["showType"])
                    {
                        case "NORMAL":
                            status = ChatRoomStatus.Public;
                            break;
                        case "PRIVATE_SHOW":
                            status = ChatRoomStatus.Private;
                            break;
                        case "GROUP_SHOW":
                            status = ChatRoomStatus.Group;
                            break;
                    }
                }
                else
                {
                    status = ChatRoomStatus.Offline;
                }

                if (status == ChatRoomStatus.Public && (string)chatRoomNode["hlsPreviewUrl"] == string.Empty)
                {
                    status = ChatRoomStatus.Private;
                }

                AddLogEntry(string.Format("{0} - {1}", c_statusObtainedLogMessage, status));

                if (status == ChatRoomStatus.Public)
                {
                    playlistUrl = (string)chatRoomNode["hlsPreviewUrl"];

                    AddLogEntry(string.Format("{0} - {1}", c_playlistObtainedLogMessage, playlistUrl));

                    HttpRequestMessage reqMsg = new(HttpMethod.Get, playlistUrl);
                    HttpResponseMessage respMsg = s_httpClient.Send(reqMsg, _cancellationToken);
                    string[] playlist = new StreamReader(respMsg.Content.ReadAsStream()).ReadToEnd().Split('\n');
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
            }
            catch (Exception exception)
            {
                status = ChatRoomStatus.Error;
                playlistUrl = String.Empty;
                availableResolutions.Clear();

                AddLogEntry(string.Format("{0} - {1}", c_errorOccuredLogMessage, exception.Message));
            }
            finally
            {
                _status = status;
                _playlistUrl = playlistUrl;
                _availableResolutions = availableResolutions;
            }

            if (_status != ChatRoomStatus.Offline && _status != ChatRoomStatus.Error && _status != ChatRoomStatus.Unknown)
            {
                _lastSeen = DateTime.Now;
            }
        }

        private void Record(string playlistUrl, object stream)
        {
            _fileName = string.Format("{0}\\{1} {2} {3}.ts", _outputDirectory, _website, _name, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")).ToLower();
            _fileSize = 0;
            
            int port = s_random.Next(16384, 65536);

            List<ProcessStartInfo> psis = new List<ProcessStartInfo>();

            switch (_website)
            {
                case ChatRoomWebsite.Chaturbate:
                    psis.Add(new ProcessStartInfo()
                    {
                        FileName = _ffmpegPath,
                        Arguments = string.Format("-analyzeduration 16M -i \"{0}\" -map 0:p:{1} -c copy \"{2}\"", playlistUrl, Convert.ToInt32(stream), _fileName)
                    });
                    break;
                case ChatRoomWebsite.BongaCams:
                    psis.Add(new ProcessStartInfo()
                    {
                        FileName = _ffmpegPath,
                        Arguments = string.Format("-analyzeduration 16M -i \"{0}\" -map 0:p:{1} -c copy \"{2}\"", playlistUrl, Convert.ToInt32(stream), _fileName)
                    });
                    break;
                case ChatRoomWebsite.Stripchat:
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
                case ChatRoomWebsite.Flirt4Free:
                    psis.Add(new ProcessStartInfo()
                    {
                        FileName = _ffmpegPath,
                        Arguments = string.Format("-analyzeduration 16M -i \"{0}\" -map 0:p:{1} -c copy \"{2}\"", playlistUrl, Convert.ToInt32(stream), _fileName)
                    });
                    break;
                case ChatRoomWebsite.CamSoda:
                    psis.Add(new ProcessStartInfo()
                    {
                        FileName = _streamlinkPath,
                        Arguments = string.Format("--player-external-http --player-external-http-continuous false --player-external-http-interface 127.0.0.1 --player-external-http-port {0} " +
                            "--default-stream {1} --stream-segment-timeout 60 --stream-timeout 60 --http-timeout 60 --hls-segment-queue-threshold 99 {2}", port, stream, playlistUrl)
                    });
                    psis.Add(new ProcessStartInfo()
                    {
                        FileName = _ffmpegPath,
                        Arguments = string.Format("-reconnect 1 -reconnect_at_eof 1 -reconnect_on_network_error 1 -reconnect_on_http_error 1 -reconnect_streamed 1 -i http://127.0.0.1:{0} -c copy \"{1}\"", port, _fileName)
                    });
                    break;
                case ChatRoomWebsite.Cam4:
                    psis.Add(new ProcessStartInfo()
                    {
                        FileName = _ffmpegPath,
                        Arguments = string.Format("-analyzeduration 16M -i \"{0}\" -map 0:p:{1} -c copy \"{2}\"", playlistUrl, Convert.ToInt32(stream), _fileName)
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

        public event EventHandler UpdateCompleted;

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

        public int Delay
        {
            set
            {
                _delay = (value < 0) ? 0 : value;
            }
            get
            { 
                return _delay; 
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
        private int _delay;
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
        private string _uri;
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