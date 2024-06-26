﻿using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
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
    public enum ChatRoomWebsite
    {
        BongaCams,
        Chaturbate,
        Stripchat
    }

    public enum ChatRoomAction
    {
        None,
        Monitor,
        Record
    }

    public enum ChatRoomStatus
    {
        Unknown,
        Error,
        Offline,
        Away,
        Hidden,
        Group,
        Private,
        Public,
        Record
    }

    public record struct ChatRoomResolution : IComparable
    {
        public ChatRoomResolution(short width, short height)
        {
            if (width >= 0 && height >= 0)
            {
                _width = width;
                _height = height;
            }
            else
            {
                _width = 0;
                _height = 0;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", _width, _height);
        }

        public int ToInt32()
        {
            return (int)_width << 16 | _height;
        }

        public int CompareTo(object resolution)
        {
            return TotalPixels.CompareTo(((ChatRoomResolution)resolution).TotalPixels);
        }

        public static ChatRoomResolution Parse(string resolution)
        {
            try
            {
                MatchCollection matches = Regex.Matches(resolution, "^([0-9]*)x([0-9]*)$", RegexOptions.IgnoreCase);
                return new ChatRoomResolution(short.Parse(matches[0].Groups[1].Value), short.Parse(matches[0].Groups[2].Value));
            }
            catch (Exception)
            {
                return ChatRoomResolution.MinValue;
            }
        }

        public static ChatRoomResolution Parse(int resolution)
        {
            return new ChatRoomResolution(Convert.ToInt16(resolution >> 16), Convert.ToInt16(resolution << 16 >> 16));
        }

        public static int FindClosest(ChatRoomResolution desiredResolution, ChatRoomResolution[] allResolutions)
        {
            int index = -1;
            int difference = -1;

            if (allResolutions != null)
            {
                for (int i = 0; i < allResolutions.Length; i++)
                {
                    if (allResolutions[i] != ChatRoomResolution.MinValue)
                    {
                        int current_difference = Math.Abs(desiredResolution.TotalPixels - allResolutions[i].TotalPixels);
                        if (index == -1 || current_difference < difference)
                        {
                            index = i;
                            difference = current_difference;
                        }
                    }
                }
            }

            return index;
        }

        public static bool operator <(ChatRoomResolution left, ChatRoomResolution right)
        {
            return left.TotalPixels < right.TotalPixels;
        }

        public static bool operator >(ChatRoomResolution left, ChatRoomResolution right)
        {
            return left.TotalPixels > right.TotalPixels;
        }

        public static bool operator <=(ChatRoomResolution left, ChatRoomResolution right)
        {
            return left.TotalPixels <= right.TotalPixels;
        }

        public static bool operator >=(ChatRoomResolution left, ChatRoomResolution right)
        {
            return left.TotalPixels >= right.TotalPixels;
        }

        public int TotalPixels
        {
            get
            {
                return (int)_width * (int)_height;
            }
        }

        public short Width
        {
            get
            {
                return _width;
            }
        }

        public short Height
        {
            get
            {
                return _height;
            }
        }

        public static ChatRoomResolution MinValue
        {
            get
            {
                return new ChatRoomResolution(0, 0);
            }
        }

        public static ChatRoomResolution MaxValue
        {
            get
            {
                return new ChatRoomResolution(short.MaxValue, short.MaxValue);
            }
        }

        public static ChatRoomResolution[] CommonResolutions
        {
            get
            {
                return new ChatRoomResolution[]
                {
                    new ChatRoomResolution(640, 360),
                    new ChatRoomResolution(960, 540),
                    new ChatRoomResolution(1280, 720),
                    new ChatRoomResolution(1600, 900),
                    new ChatRoomResolution(1920, 1080),
                    new ChatRoomResolution(2560, 1440),
                    new ChatRoomResolution(3840, 2160)
                };
            }
        }

        private short _width;
        private short _height;
    }

    public class ChatRoom : IDisposable
    {
        public event EventHandler UpdateCompleted;

        public ChatRoom(string url)
        {
            Tuple<ChatRoomWebsite, string, string> parsedUrl = ParseUrl(url);
            if (parsedUrl != null)
            {
                _website = parsedUrl.Item1;
                _name = parsedUrl.Item2;
                _status = ChatRoomStatus.Unknown;
                _action = ChatRoomAction.None;
                _chatRoomUrl = parsedUrl.Item3;
                _playlistUrl = String.Empty;
                _availableResolutions = new List<ChatRoomResolution>();
                _preferredResolution = ChatRoomResolution.MaxValue;
                _outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                _ffmpegPath = string.Format("{0}{1}ffmpeg.exe", new FileInfo(Environment.ProcessPath).Directory.FullName, Path.DirectorySeparatorChar);
                _ffmpegProcess = null;
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

                        if (_ffmpegProcess != null)
                        {
                            if (_ffmpegProcess.HasExited == false)
                            {
                                _ffmpegProcess.Kill();
                                _ffmpegProcess.WaitForExit();
                            }
                            _ffmpegProcess.Close();
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

                if (_action == ChatRoomAction.Record && _status == ChatRoomStatus.Record && _ffmpegProcess != null && _ffmpegProcess.HasExited == false)
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

                //stop any recording (if we need to restart it, we'll do it later)

                if (_ffmpegProcess != null)
                {
                    if (_ffmpegProcess.HasExited == false)
                    {
                        _ffmpegProcess.Kill();
                        _ffmpegProcess.WaitForExit();
                    }
                    _ffmpegProcess.Close();
                    _ffmpegProcess = null;
                }
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

                if (status == ChatRoomStatus.Public)
                {
                    playlistUrl = doc.RootElement.GetProperty("url").GetString();
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
                        Record(string.Format("-analyzeduration 15M -i \"{0}\" -map 0:p:{1} -c copy", playlistUrl, streamIndex));
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

                HttpRequestMessage reqMsg = new(HttpMethod.Get, String.Format("https://{0}.bcvcdn.com/hls/stream_{1}/playlist.m3u8", esid, (string)modelNode["username"]));
                HttpResponseMessage respMsg = await s_httpClient.SendAsync(reqMsg, _cancellationToken);

                if (respMsg.IsSuccessStatusCode)
                {
                    playlistUrl = reqMsg.RequestUri.OriginalString;
                    string[] playlist = (await respMsg.Content.ReadAsStringAsync()).Split('\n');
                    for (int i = 0; i < playlist.Length; i++)
                    {
                        MatchCollection matches = Regex.Matches(playlist[i], "^.*RESOLUTION=([0-9]*x[0-9]*).*$", RegexOptions.IgnoreCase);
                        if (matches.Count > 0)
                        {
                            availableResolutions.Add(ChatRoomResolution.Parse(matches[0].Groups[1].Value));
                        }
                    }

                    if (_action == ChatRoomAction.Record)
                    {
                        status = ChatRoomStatus.Record;
                    }
                    else
                    {
                        status = ChatRoomStatus.Public;
                    }
                }
                else
                {
                    status = ChatRoomStatus.Offline;
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
                        Record(string.Format("-analyzeduration 15M -i \"{0}\" -map 0:p:{1} -c copy", playlistUrl, streamIndex));
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
                    case "groupShow":
                        status = ChatRoomStatus.Group;
                        break;
                    case "off":
                        status = ChatRoomStatus.Offline;
                        break;
                }

                if (status == ChatRoomStatus.Public)
                {
                    playlistUrl = string.Format("https://b-{0}.doppiocdn.com/hls/{1}/{1}.m3u8", rootNode["cam"]["viewServers"]["flashphoner-hls"], rootNode["cam"]["streamName"]);
                    availableResolutions.Add(new ChatRoomResolution((short)rootNode["cam"]["broadcastSettings"]["width"], (short)rootNode["cam"]["broadcastSettings"]["height"]));
                    JsonArray presetsNode = (JsonArray)rootNode["cam"]["broadcastSettings"]["presets"]["default"];
                    for (int i = 0; i < presetsNode.Count; i++)
                    {
                        ChatRoomResolution resolution = ChatRoomResolution.MinValue;
                        switch ((string)presetsNode[i]["name"])
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
                        string streamUrl = playlistUrl;
                        if (streamIndex > 0)
                        {
                            streamUrl = string.Format("{0}_{1}p{2}",
                                streamUrl.Substring(0, streamUrl.LastIndexOf(".")),
                                availableResolutions[streamIndex].Height.ToString(),
                                streamUrl.Substring(streamUrl.LastIndexOf(".")));
                        }
                        Record(string.Format("-re -i \"{0}\" -c copy", streamUrl));
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

        private void Record(string ffmpegArgs)
        {
            _fileName = string.Format("{0}{1}{2} {3} {4}.ts", _outputDirectory, Path.DirectorySeparatorChar, _website, _name, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")).ToLower();
            _fileSize = 0;
            ProcessStartInfo psi = new() { FileName = _ffmpegPath, Arguments = string.Format("{0} \"{1}\"", ffmpegArgs, _fileName), UseShellExecute = false, LoadUserProfile = false, CreateNoWindow = true };
            _ffmpegProcess = Process.Start(psi);
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

            if ((matches = Regex.Matches(url, @"^https://chaturbate.com/([^/]+)/$", RegexOptions.IgnoreCase)).Count > 0 ||
                (matches = Regex.Matches(url, @"^chaturbate[ ]+([^ ]+).*/$", RegexOptions.IgnoreCase)).Count > 0)

            {
                return Tuple.Create(ChatRoomWebsite.Chaturbate, matches[0].Groups[1].Value, string.Format("https://chaturbate.com/{0}/", matches[0].Groups[1].Value));
            }

            if ((matches = Regex.Matches(url, @"^https://(?:[^/.]+.)?bongacams[0-9]*.com/([^/]+(?=#!/$)|[^/]+(?=/$))", RegexOptions.IgnoreCase)).Count > 0 ||
                (matches = Regex.Matches(url, @"^bongacams[ ]+([^ ]+).*/$", RegexOptions.IgnoreCase)).Count > 0)
            {
                return Tuple.Create(ChatRoomWebsite.BongaCams, matches[0].Groups[1].Value, string.Format("https://bongacams.com/{0}/", matches[0].Groups[1].Value));
            }

            if ((matches = Regex.Matches(url, @"^https://(?:[^.]+.)?stripchat.com/([^/]+)/$", RegexOptions.IgnoreCase)).Count > 0 ||
                (matches = Regex.Matches(url, @"^stripchat[ ]+([^ ]+).*/$", RegexOptions.IgnoreCase)).Count > 0)
            {
                return Tuple.Create(ChatRoomWebsite.Stripchat, matches[0].Groups[1].Value, string.Format("https://stripchat.com/{0}/", matches[0].Groups[1].Value));
            }

            return null;
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

        public static int TotalCount
        {
            get
            {
                return s_totalCount;
            }
        }

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
        private Process _ffmpegProcess;
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

        private const int c_updateLimit = 60;

        private static HttpClient s_httpClient = new(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });
        private static int s_totalCount = 0;
    }
}
