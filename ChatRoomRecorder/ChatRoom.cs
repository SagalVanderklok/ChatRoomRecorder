using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Text;
using System.Threading;

namespace ChatRoomRecorder
{
    public enum ChatRoomWebsite
    {
        BongaCams,
        Chaturbate
     }

    public enum ChatRoomStatus
    {
        Unknown,
        Error,
        Offline,
        Away,
        Hidden,
        Private,
        Public,
        Record
    }

    public enum ChatRoomAction
    {
        None,
        Monitor,
        Record
    }

    public class ChatRoom : IDisposable
    {
        public ChatRoom(string url)
        {
            Tuple<ChatRoomWebsite, string, string> parsedUrl = ParseUrl(url);
            if (parsedUrl == null) throw new ArgumentException();

            _website = parsedUrl.Item1;
            _name = parsedUrl.Item2;
            _status = ChatRoomStatus.Unknown;
            _action = ChatRoomAction.None;
            _roomUrl = parsedUrl.Item3;
            _playlistUrl = String.Empty;
            _availableResolutions = new List<string>();
            _preferredResolution = String.Empty;
            _outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            _ffmpegPath = (new FileInfo(Process.GetCurrentProcess().MainModule.FileName)).Directory.FullName + Path.DirectorySeparatorChar + "ffmpeg.exe";
            _ffmpegProcess = null;
            _fileName = string.Empty;
            _fileSize = -1;
            _lastUpdate = DateTime.MinValue;
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
            Interlocked.Increment(ref _totalCount);
        }

        public void Dispose()
        {
            try
            {
                _semaphore.Wait();

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
            if (_semaphore.CurrentCount == 0) return;

            try
            {
                _semaphore.Wait();

                if (!_isUpdating && !_disposingFinished && _timer.Enabled)
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

                    if (_browserInitialized)
                    {
                        _browser.Dispose();
                    }

                    _disposingFinished = true;
                    Interlocked.Decrement(ref _totalCount);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Update()
        {
            try
            {
                _semaphore.Wait();

                if (_disposingStarted || _isUpdating || (DateTime.Now - _lastUpdate).TotalSeconds < 60) return;

                _isUpdating = true;
                _lastUpdate = DateTime.Now;

                //we're recording and that's what we want - return, otherwise - continue

                if (_action == ChatRoomAction.Record && _status == ChatRoomStatus.Record && _ffmpegProcess != null && _ffmpegProcess.HasExited == false)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(_fileName);
                        if (fi.Length > _fileSize)
                        {
                            _fileSize = fi.Length;
                            _isUpdating = false;
                            return;
                        }
                    }
                    catch (Exception) { }
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
                    if (_browserInitialized)
                    {
                        _browser.Dispose(); 
                        _browser = null;
                        _browserInitialized = false;
                    }
                    _isUpdating = false;
                    return;
                }

                //initialize the browser, if it's not ready

                if (!_browserInitialized)
                {
                    _browser = new WebView2();
                    _browser.CoreWebView2InitializationCompleted += _browser_CoreWebView2InitializationCompleted;
                    _browser.Source = new Uri("about:blank", UriKind.Absolute);
                    return;
                }

                //update room's info

                switch (_website)
                {
                    case ChatRoomWebsite.Chaturbate:
                        UpdateStatusChaturbate();
                        break;
                    case ChatRoomWebsite.BongaCams:
                        UpdateStatusBongaCams();
                        break;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void _browser_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            try
            {
                _semaphore.Wait();

                switch (_website)
                {
                    case ChatRoomWebsite.Chaturbate:
                        _browser.CoreWebView2.WebResourceResponseReceived += Chaturbate_WebResourceResponseReceived;
                        break;
                    case ChatRoomWebsite.BongaCams:
                        _browser.CoreWebView2.WebResourceResponseReceived += BongaCams_WebResourceResponseReceived;
                        break;
                }
                _browser.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                _browserInitialized = true;
                _isUpdating = false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void UpdateStatusChaturbate()
        {
            string postData = "room_slug=" + _name;
            CoreWebView2WebResourceRequest request = _browser.CoreWebView2.Environment.CreateWebResourceRequest(
                "https://chaturbate.com/get_edge_hls_url_ajax/",
                "POST",
                new MemoryStream(Encoding.UTF8.GetBytes(postData)),
                "Content-Type: application/x-www-form-urlencoded\r\nX-Requested-With: XMLHttpRequest");
            _browser.CoreWebView2.NavigateWithWebResourceRequest(request);
        }

        private void UpdateStatusBongaCams()
        {
            string postData = "model_search[display_name]=" + _name;
            CoreWebView2WebResourceRequest request = _browser.CoreWebView2.Environment.CreateWebResourceRequest(
                "https://bongacams.com/tools/listing_v3.php",
                "POST",
                new MemoryStream(Encoding.UTF8.GetBytes(postData)),
                "Content-Type: application/x-www-form-urlencoded\r\nX-Requested-With: XMLHttpRequest");
            _browser.CoreWebView2.NavigateWithWebResourceRequest(request);
        }

        private async void Chaturbate_WebResourceResponseReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            try
            {
                await _semaphore.WaitAsync();
                if (!_isUpdating) return;
            }
            finally
            {
                _semaphore.Release();
            }

            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = null;
            List<string> availableResolutions = null;
            JsonDocument doc = null;

            try
            {
                StreamReader reader = new StreamReader(await e.Response.GetContentAsync());
                string respStr = reader.ReadToEnd();

                doc = JsonDocument.Parse(respStr);
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
            }
            catch (Exception)
            {
                status = ChatRoomStatus.Error;
            }

            if (status == ChatRoomStatus.Public)
            {
                try
                {
                    playlistUrl = doc.RootElement.GetProperty("url").GetString();
                    HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Get, playlistUrl);
                    reqMsg.Headers.Add("Accept-Encoding", "gzip");
                    HttpResponseMessage respMsg = await _httpClient.SendAsync(reqMsg, _cancellationToken);
                    string[] playlist = (await respMsg.Content.ReadAsStringAsync()).Split('\n');
                    availableResolutions = new List<string>();
                    for (int i = 0; i < playlist.Length; i++)
                    {
                        string pattern = "^.*RESOLUTION=([0-9]*x[0-9]*).*$";
                        MatchCollection matches = Regex.Matches(playlist[i], pattern, RegexOptions.IgnoreCase);
                        if (matches.Count > 0)
                        {
                            availableResolutions.Add(matches[0].Groups[1].Value);
                        }
                    }
                    if (availableResolutions.Count == 0)
                    {
                        status = ChatRoomStatus.Error;
                    }
                }
                catch (Exception)
                {
                    status = ChatRoomStatus.Error;
                }
            }

            if (status == ChatRoomStatus.Public && _action == ChatRoomAction.Record)
            {
                _status = ChatRoomStatus.Record;
            }
            else
            {
                _status = status;
            }
            if (_status == ChatRoomStatus.Public || _status == ChatRoomStatus.Record)
            {
                _playlistUrl = playlistUrl;
                _availableResolutions = availableResolutions;
            }
            else
            {
                _playlistUrl = String.Empty;
                _availableResolutions.Clear();
            }

            Record();

            try
            {
                await _semaphore.WaitAsync();
                _isUpdating = false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async void BongaCams_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            try
            {
                await _semaphore.WaitAsync();
                if (!_isUpdating) return;
            }
            finally
            {
                _semaphore.Release();
            }

            try
            {
                StreamReader reader = new StreamReader(await e.Response.GetContentAsync());
                string respStr = reader.ReadToEnd();

                JsonNode rootNode = JsonNode.Parse(respStr);

                JsonNode modelNode = null;
                string username = string.Empty;
                for (int i = 0; i < (int)rootNode["total_count"]; i++)
                {
                    JsonNode node = ((JsonArray)rootNode["models"])[i];
                    string name = (string)node["username"];
                    if (name != null && name.ToLower() == _name)
                    {
                        modelNode = node;
                        username = name;
                        break;
                    }
                }
                if (modelNode == null) throw new ArgumentException();

                string esid = (string)modelNode["esid"];
                if (esid == null) throw new ArgumentException();

                string playlistUrl = String.Format("https://{0}.bcvcdn.com/hls/stream_{1}/playlist.m3u8", esid, username);
                HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Get, playlistUrl);
                HttpResponseMessage respMsg = await _httpClient.SendAsync(reqMsg, _cancellationToken);
                if (!respMsg.IsSuccessStatusCode)
                {
                    _status = ChatRoomStatus.Offline;
                    _playlistUrl = String.Empty;
                    _availableResolutions.Clear();
                }
                else
                {
                    string[] playlist = (await respMsg.Content.ReadAsStringAsync()).Split('\n');
                    List<string> availableResolutions = new List<string>();
                    for (int i = 0; i < playlist.Length; i++)
                    {
                        MatchCollection matches = Regex.Matches(playlist[i], "^.*RESOLUTION=([0-9]*x[0-9]*).*$", RegexOptions.IgnoreCase);
                        if (matches.Count > 0)
                            availableResolutions.Add(matches[0].Groups[1].Value);
                    }
                    if (availableResolutions.Count > 0)
                    {
                        if (_action == ChatRoomAction.Record)
                        {
                            _status = ChatRoomStatus.Record;
                        }
                        else
                        {
                            _status = ChatRoomStatus.Public;
                        }
                        _playlistUrl = playlistUrl;
                        _availableResolutions = availableResolutions;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
            catch (Exception)
            {
                _status = ChatRoomStatus.Error;
                _playlistUrl = String.Empty;
                _availableResolutions.Clear();
            }

            Record();

            try
            {
                await _semaphore.WaitAsync();
                _isUpdating = false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void Record()
        {
            if (_status == ChatRoomStatus.Record)
            {
                try
                {
                    _fileName = (_outputDirectory + Path.DirectorySeparatorChar + _website + " " + _name + " " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".ts").ToLower();
                    _fileSize = 0;
                    int streamIndex = _availableResolutions.Contains(_preferredResolution) ? _availableResolutions.IndexOf(_preferredResolution) : _availableResolutions.Count - 1;
                    string ffmpegArgs = String.Format("-analyzeduration 15M -i \"{0}\" -map 0:p:{1} -c copy \"{2}\"", _playlistUrl, streamIndex, _fileName);
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = _ffmpegPath;
                    psi.Arguments = ffmpegArgs;
                    psi.UseShellExecute = false;
                    psi.LoadUserProfile = false;
                    psi.CreateNoWindow = true;
                    _ffmpegProcess = Process.Start(psi);
                }
                catch (Exception)
                {
                    _status = ChatRoomStatus.Error;
                    _playlistUrl = String.Empty;
                    _availableResolutions.Clear();
                }
            }
        }

        private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                _semaphore.Wait();

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

        public static Tuple<ChatRoomWebsite, string, string> ParseUrl(string url)
        {
            if (url == null || url.Length == 0) return null;

            url = url.ToLower();
            if (url[url.Length - 1] != '/')
            {
                url = url + '/';
            }
            
            MatchCollection matches = null;
            if ((matches = Regex.Matches(url, @"^https://chaturbate.com/([^/]+)/$", RegexOptions.IgnoreCase)).Count > 0 ||
                (matches = Regex.Matches(url, @"^chaturbate[ ]+([^ /]+).*$", RegexOptions.IgnoreCase)).Count > 0)

            {
                return Tuple.Create(ChatRoomWebsite.Chaturbate, matches[0].Groups[1].Value, string.Format("https://chaturbate.com/{0}/", matches[0].Groups[1].Value));
            }
            else if ((matches = Regex.Matches(url, @"^https://(?:[^/.]+.)?bongacams[0-9]*.com/([^/]+(?=#!/$)|[^/]+(?=/$))", RegexOptions.IgnoreCase)).Count > 0 ||
                (matches = Regex.Matches(url, @"^bongacams[ ]+([^ /]+).*$", RegexOptions.IgnoreCase)).Count > 0)
            {
                return Tuple.Create(ChatRoomWebsite.BongaCams, matches[0].Groups[1].Value, string.Format("https://bongacams.com/{0}/", matches[0].Groups[1].Value));
            }
            else
            {
                return null;
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

        public string RoomUrl
        {
            get
            {
                return _roomUrl;
            }
        }

        public string PlaylistUrl
        {
            get
            {
                return _playlistUrl;
            }
        }

        public string[] AvailableResolutions
        {
            get
            {
                return _availableResolutions.ToArray();
            }
        }

        public string PreferredResolution
        {
            set
            {
                if (value == null)
                    throw new ArgumentException();
                else
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
                    throw new ArgumentException();
                else
                    _outputDirectory = value;
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
                    throw new ArgumentException();
                else
                    _ffmpegPath = value;
            }
            get
            {
                return _ffmpegPath;
            }
        }

        public DateTime LastUpdate
        {
            get
            {
                return _lastUpdate;
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
                return _totalCount;
            }
        }

        private string _name;
        private ChatRoomWebsite _website;
        private ChatRoomStatus _status;
        private ChatRoomAction _action;
        private string _roomUrl;
        private string _playlistUrl;
        private List<string> _availableResolutions;
        private string _preferredResolution;
        private string _outputDirectory;
        private string _ffmpegPath;
        private Process _ffmpegProcess;
        private string _fileName;
        private long _fileSize;
        private DateTime _lastUpdate;
        private bool _isUpdating;
        private bool _disposingStarted;
        private bool _disposingFinished;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private WebView2 _browser;
        private bool _browserInitialized;
        private System.Windows.Forms.Timer _timer;
        private SemaphoreSlim _semaphore;

        private static HttpClient _httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });
        private static int _totalCount = 0;
    }
}
