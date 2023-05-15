using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Diagnostics;
using System.Threading;
using System.Text.Json.Nodes;
using PuppeteerSharp;

namespace ChatRoomRecorder
{
    public enum ChatRoomWebsite
    {
        BongaCams,
        Chaturbate
     }

    public enum ChatRoomStatus
    {
        Record,
        Public,
        Private,
        Hidden,
        Away,
        Offline,
        Error,
        Unknown
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
            MatchCollection matches = null;
            if ((matches = Regex.Matches(aUrl, "^https://chaturbate.com/([^/]*)/?$", RegexOptions.IgnoreCase)).Count > 0)
                _website = ChatRoomWebsite.Chaturbate;
            else if ((matches = Regex.Matches(aUrl, "^https://[^/.]*.bongacams[0-9]*.com/([^/]*)/?$", RegexOptions.IgnoreCase)).Count > 0)
                _website = ChatRoomWebsite.BongaCams;
            else
                throw new ArgumentException();

            _browser = null;
            _name = matches[0].Groups[1].Value;
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
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _disposing_started = false;
            _disposing_finished = false;
            _task = Task.Run(async () =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds((double)(_random.Next(60) + 30)), _cancellationToken);
                    await UpdateAsync();
                }
            }, _cancellationToken);
        }

        public void Dispose()
        {
            if (_disposing_started) return;

            _disposing_started = true;
            _cancellationTokenSource.Cancel();
            _task.ContinueWith(_task =>
            {
                if (_ffmpegProcess != null)
                {
                    if (_ffmpegProcess.HasExited == false)
                    {
                        _ffmpegProcess.Kill();
                        _ffmpegProcess.WaitForExit();
                    }
                    _ffmpegProcess.Close();
                }
                _cancellationTokenSource.Dispose();
                _disposing_finished = true;
            });
        }

        private async Task UpdateAsync()
        {
            //we're recording and that's what we want, so just return

            if (_action == ChatRoomAction.Record && _status == ChatRoomStatus.Record && _ffmpegProcess != null && _ffmpegProcess.HasExited == false)
            {
                try
                {
                    FileInfo fi = new FileInfo(_fileName);
                    if (fi.Length > _fileSize)
                    {
                        _fileSize = fi.Length;
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
                return;
            }

            //update room's info

            Tuple<ChatRoomStatus, string, List<string>> statusTuple = null;
            switch (_website)
            {
                case ChatRoomWebsite.Chaturbate:
                    statusTuple = await GetStatusChaturbateAsync();
                    break;
                case ChatRoomWebsite.BongaCams:
                    statusTuple = await GetStatusBongaCamsAsync();
                    break;
            }
            ChatRoomStatus status = statusTuple.Item1;
            string playlistUrl = statusTuple.Item2;
            List<string> availableResolutions = statusTuple.Item3;

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

            //start recording, if we need it

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

        private async Task<Tuple<ChatRoomStatus, string, List<string>>> GetStatusChaturbateAsync()
        {
            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = null;
            List<string> availableResolutions = null;

            JsonDocument doc = null;

            if (_browser != null)
            {
                try
                {
                    IPage page = await _browser.NewPageAsync();
                    await page.SetRequestInterceptionAsync(true);
                    page.Request += async (sender, e) =>
                    {
                        Payload payload = new Payload()
                        {
                            Method = HttpMethod.Post,
                            Headers = new Dictionary<string, string>
                            {
                            { "X-Requested-With", "XMLHttpRequest" },
                            { "Content-Type", "application/x-www-form-urlencoded" }
                            },
                            PostData = "room_slug=" + _name
                        };
                        await e.Request.ContinueAsync(payload);
                    };
                    IResponse response = await page.GoToAsync("https://chaturbate.com/get_edge_hls_url_ajax/");
                    string respStr = await response.TextAsync();
                    await page.CloseAsync();

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

            return Tuple.Create(status, playlistUrl, availableResolutions);
        }

        private async Task<Tuple<ChatRoomStatus, string, List<string>>> GetStatusBongaCamsAsync()
        {
            try
            {
                HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Post, "https://bongacams.com/tools/listing_v3.php");
                reqMsg.Headers.Add("X-Requested-With", "XMLHttpRequest");
                Dictionary<string, string> reqCont = new Dictionary<string, string>();
                reqCont.Add("model_search[display_name]", _name);
                reqMsg.Content = new FormUrlEncodedContent(reqCont);
                HttpResponseMessage respMsg = await _httpClient.SendAsync(reqMsg, _cancellationToken);
                string respStr = await respMsg.Content.ReadAsStringAsync();

                JsonNode rootNode = JsonNode.Parse(respStr);
                
                if ((int)rootNode["total_count"] != 1)
                    return Tuple.Create(ChatRoomStatus.Error, (string)null, (List<string>)null);

                if ((int)rootNode["online_count"] != 1)
                    return Tuple.Create(ChatRoomStatus.Offline, (string)null, (List<string>)null);

                JsonNode modelNode = ((JsonArray)rootNode["models"])[0];

                string username = (string)modelNode["username"];
                if (username == null)
                    return Tuple.Create(ChatRoomStatus.Error, (string)null, (List<string>)null);

                string esid = (string)modelNode["esid"];
                if (esid == null) 
                    return Tuple.Create(ChatRoomStatus.Error, (string)null, (List<string>)null);

                string playlistUrl = String.Format("https://{0}.bcvcdn.com/hls/stream_{1}/playlist.m3u8", esid, username);
                reqMsg = new HttpRequestMessage(HttpMethod.Get, playlistUrl);
                respMsg = await _httpClient.SendAsync(reqMsg, _cancellationToken);
                if (!respMsg.IsSuccessStatusCode) 
                    return Tuple.Create(ChatRoomStatus.Private, (string)null, (List<string>)null);

                string[] playlist = (await respMsg.Content.ReadAsStringAsync()).Split('\n');
                List<string> availableResolutions = new List<string>();
                for (int i = 0; i < playlist.Length; i++)
                {
                    MatchCollection matches = Regex.Matches(playlist[i], "^.*RESOLUTION=([0-9]*x[0-9]*).*$", RegexOptions.IgnoreCase);
                    if (matches.Count > 0)
                        availableResolutions.Add(matches[0].Groups[1].Value);
                }
                if (availableResolutions.Count > 0)
                    return Tuple.Create(ChatRoomStatus.Public, playlistUrl, availableResolutions);
                else
                    return Tuple.Create(ChatRoomStatus.Error, (string)null, (List<string>)null);
            }
            catch (Exception)
            {
                return Tuple.Create(ChatRoomStatus.Error, (string)null, (List<string>)null);
            }
        }

        public IBrowser Browser
        {
            set
            {
                if (_disposing_started) throw new InvalidOperationException();
                _browser = value;
            }
            get
            {
                if (_disposing_started) throw new InvalidOperationException();
                return _browser;
            }
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

        private IBrowser _browser;
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
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private Task _task;
        private bool _disposing_started;
        private bool _disposing_finished;

        private static Random _random = new Random();
        private static HttpClient _httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });
    }
}
