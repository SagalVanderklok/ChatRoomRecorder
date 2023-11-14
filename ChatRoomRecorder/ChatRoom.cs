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
using System.Threading.Tasks;
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
        public ChatRoom(string aUrl)
        {
            aUrl = aUrl.ToLower();
            Tuple<ChatRoomWebsite, string> parsed_url = ParseUrl(aUrl);
            if (parsed_url == null) throw new ArgumentException();

            _website = parsed_url.Item1;
            _name = parsed_url.Item2;
            _status = ChatRoomStatus.Unknown;
            _action = ChatRoomAction.None;
            _roomUrl = aUrl;
            _playlistUrl = String.Empty;
            _availableResolutions = new List<string>();
            _preferredResolution = String.Empty;
            _outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            _ffmpegPath = (new FileInfo(Process.GetCurrentProcess().MainModule.FileName)).Directory.FullName + Path.DirectorySeparatorChar + "ffmpeg.exe";
            _ffmpegProcess = null;
            _fileName = string.Empty;
            _fileSize = -1;
            _pause = 0;
            _isUpdating = false;
            _disposing_started = false;
            _disposing_finished = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _browser = new WebView2();
            switch (_website)
            {
                case ChatRoomWebsite.Chaturbate:
                    _browser.CoreWebView2InitializationCompleted += Chaturbate_CoreWebView2InitializationCompleted;
                    break;
                case ChatRoomWebsite.BongaCams:
                    _browser.CoreWebView2InitializationCompleted += BongaCams_CoreWebView2InitializationCompleted;
                    break;
            }
            _browser.Source = new Uri("about:blank", UriKind.Absolute);
        }

        public void Dispose()
        {
            if (_disposing_started) return;

            _disposing_started = true;
            _cancellationTokenSource.Cancel();

            Task.Run(() =>
            {
                while (_isUpdating)
                {
                    Thread.Sleep(1000);
                }
                if (_ffmpegProcess != null)
                {
                    if (_ffmpegProcess.HasExited == false)
                    {
                        _ffmpegProcess.Kill();
                        _ffmpegProcess.WaitForExit();
                    }
                    _ffmpegProcess.Close();
                }
                _disposing_finished = true;
            });
        }

        public static Tuple<ChatRoomWebsite, string> ParseUrl(string aUrl)
        {
            aUrl = aUrl.ToLower();
            MatchCollection matches = null;
            if ((matches = Regex.Matches(aUrl, "^https://chaturbate.com/([^/]+)/?$", RegexOptions.IgnoreCase)).Count > 0 ||
                (matches = Regex.Matches(aUrl, "^chaturbate[ ]([^ ]+)[ ]?.*$", RegexOptions.IgnoreCase)).Count > 0)

            {
                return Tuple.Create(ChatRoomWebsite.Chaturbate, matches[0].Groups[1].Value);
            }
            else if ((matches = Regex.Matches(aUrl, "^https://[^/.]+.bongacams[0-9]*.com/([^/]+)/?$", RegexOptions.IgnoreCase)).Count > 0 ||
                (matches = Regex.Matches(aUrl, "^bongacams[ ]([^ ]+)[ ]?.*$", RegexOptions.IgnoreCase)).Count > 0)
            {
                return Tuple.Create(ChatRoomWebsite.BongaCams, matches[0].Groups[1].Value);
            }
            else
            {
                return null;
            }
        }

        public void Update(int aPause)
        {
            if (_disposing_started) throw new InvalidOperationException();

            if (_isUpdating) return;

            _isUpdating= true;
            _pause = (double)aPause;

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
                _isUpdating = false;
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

        private void Record()
        {
            if (_status == ChatRoomStatus.Record)
            {
                _fileName = (_outputDirectory + Path.DirectorySeparatorChar + _website + " " + _name + " " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".ts").ToLower();
                _fileSize = 0;
                int streamIndex = _availableResolutions.Contains(_preferredResolution) ? _availableResolutions.IndexOf(_preferredResolution) : _availableResolutions.Count - 1;
                string ffmpegArgs = String.Format("-i \"{0}\" -map 0:p:{1} -c copy \"{2}\"", _playlistUrl, streamIndex, _fileName);
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = _ffmpegPath;
                psi.Arguments = ffmpegArgs;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                _ffmpegProcess = Process.Start(psi);
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

        private async void Chaturbate_WebResourceResponseReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = null;
            List<string> availableResolutions = null;
            JsonDocument doc = null;

            try
            {
                StreamReader reader = new StreamReader(await e.Response.GetContentAsync());
                string respStr = reader.ReadToEnd();

                await Task.Delay(TimeSpan.FromSeconds(_pause), _cancellationToken);

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

            _isUpdating = false;
        }

        private void Chaturbate_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            _browser.CoreWebView2.WebResourceResponseReceived += Chaturbate_WebResourceResponseReceived;
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

        private async void BongaCams_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            try
            {
                StreamReader reader = new StreamReader(await e.Response.GetContentAsync());
                string respStr = reader.ReadToEnd();

                await Task.Delay(TimeSpan.FromSeconds(_pause), _cancellationToken);

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

            _isUpdating = false;
        }

        private void BongaCams_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            _browser.CoreWebView2.WebResourceResponseReceived += BongaCams_WebResourceResponseReceived;
        }

        public string Name
        {
            get
            {
                if (_disposing_started) throw new InvalidOperationException();
                return _name;
            }
        }

        public ChatRoomWebsite Website
        {
            get
            {
                if (_disposing_started) throw new InvalidOperationException();
                return _website;
            }
        }

        public ChatRoomStatus Status
        {
            get
            {
                if (_disposing_started) throw new InvalidOperationException();
                return _status;
            }
        }

        public ChatRoomAction Action
        {
            set
            {
                if (_disposing_started) throw new InvalidOperationException();
                _action = value;
            }
            get
            {
                if (_disposing_started) throw new InvalidOperationException();
                return _action;
            }
        }

        public string RoomUrl
        {
            get
            {
                if (_disposing_started) throw new InvalidOperationException();
                return _roomUrl;
            }
        }

        public string PlaylistUrl
        {
            get
            {
                if (_disposing_started) throw new InvalidOperationException();
                return _playlistUrl;
            }
        }

        public string[] AvailableResolutions
        {
            get
            {
                if (_disposing_started) throw new InvalidOperationException();
                return _availableResolutions.ToArray();
            }
        }

        public string PreferredResolution
        {
            set
            {
                if (_disposing_started) throw new InvalidOperationException();
                if (value == null) throw new ArgumentException();
                _preferredResolution = value;
            }
            get
            {
                if (_disposing_started) throw new InvalidOperationException();
                return _preferredResolution;
            }
        }

        public string OutputDirectory
        {
            set
            {
                if (_disposing_started) throw new InvalidOperationException();
                if (value == null) throw new ArgumentException();
                _outputDirectory = value;
            }
            get
            {
                if (_disposing_started) throw new InvalidOperationException();
                return _outputDirectory;
            }
        }

        public string FFmpegPath
        {
            set
            {
                if (_disposing_started) throw new InvalidOperationException();
                if (value == null) throw new ArgumentException();
                _ffmpegPath = value;
            }
            get
            {
                if (_disposing_started) throw new InvalidOperationException();
                return _ffmpegPath;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return _disposing_finished;
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
        private double _pause;
        private bool _isUpdating;
        private bool _disposing_started;
        private bool _disposing_finished;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private WebView2 _browser;

        private static HttpClient _httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });
    }
}
