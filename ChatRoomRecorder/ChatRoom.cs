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

namespace ChatRoomRecorder
{
    public enum ChatRoomStatus
    {
        Public,
        Private,
        Hidden,
        Away,
        Offline,
        Record,
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
            string pattern = "^https://chaturbate.com/([^/]*)/?$";
            MatchCollection matches = Regex.Matches(aUrl, pattern, RegexOptions.IgnoreCase);
            if (matches.Count > 0)
            {
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
                _disposed = false;
                _task = Task.Run(async () =>
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds((double)(_random.Next(60) + 30)), _cancellationToken);
                        await UpdateAsync();
                    }
                }, _cancellationToken);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
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
            });
        }

        private async Task UpdateAsync()
        {
            //we're recording and that's what we want, so just return

            if (_action == ChatRoomAction.Record && _status == ChatRoomStatus.Record && _ffmpegProcess != null && _ffmpegProcess.HasExited == false)
            {
                bool skip = true;
                try
                {
                    FileInfo fi = new FileInfo(_fileName);
                    if (fi.Length > _fileSize)
                    {
                        _fileSize = fi.Length;
                    }
                    else
                    {
                        skip = false;
                    }
                }
                catch (Exception) 
                {
                    skip = false;
                }
                if (skip) return;
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

            ChatRoomStatus status = ChatRoomStatus.Unknown;
            string playlistUrl = null;
            List<string> availableResolutions = null;
            JsonDocument doc = null;

            try
            {
                HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Post, "https://chaturbate.com/get_edge_hls_url_ajax/");
                reqMsg.Headers.Add("X-Requested-With", "XMLHttpRequest");
                Dictionary<string, string> reqCont = new Dictionary<string, string> { { "room_slug", _name } };
                reqMsg.Content = new FormUrlEncodedContent(reqCont);
                HttpResponseMessage respMsg = await _httpClient.SendAsync(reqMsg, _cancellationToken);
                string respStr = await respMsg.Content.ReadAsStringAsync();
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
                    default:
                        status = ChatRoomStatus.Unknown;
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

            //start recording, if we need it

            if (_status == ChatRoomStatus.Record)
            {
                _fileName = _outputDirectory + Path.DirectorySeparatorChar + _name + " " + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".ts";
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

        public string Name
        {
            get
            {
                if (_disposed) throw new InvalidOperationException();
                return _name;
            }
        }

        public ChatRoomStatus Status
        {
            get
            {
                if (_disposed) throw new InvalidOperationException();
                return _status;
            }
        }

        public ChatRoomAction Action
        {
            set
            {
                if (_disposed) throw new InvalidOperationException();
                _action = value;
            }
            get
            {
                if (_disposed) throw new InvalidOperationException();
                return _action;
            }
        }

        public string RoomUrl
        {
            get
            {
                if (_disposed) throw new InvalidOperationException();
                return _roomUrl;
            }
        }

        public string PlaylistUrl
        {
            get
            {
                if (_disposed) throw new InvalidOperationException();
                return _playlistUrl;
            }
        }

        public string[] AvailableResolutions
        {
            get
            {
                if (_disposed) throw new InvalidOperationException();
                return _availableResolutions.ToArray();
            }
        }

        public string PreferredResolution
        {
            set
            {
                if (_disposed) throw new InvalidOperationException();
                if (value == null) throw new ArgumentException();
                _preferredResolution = value;
            }
            get
            {
                if (_disposed) throw new InvalidOperationException();
                return _preferredResolution;
            }
        }

        public string OutputDirectory
        {
            set
            {
                if (_disposed) throw new InvalidOperationException();
                if (value == null) throw new ArgumentException();
                _outputDirectory = value;
            }
            get
            {
                if (_disposed) throw new InvalidOperationException();
                return _outputDirectory;
            }
        }

        public string FFmpegPath
        {
            set
            {
                if (_disposed) throw new InvalidOperationException();
                if (value == null) throw new ArgumentException();
                _ffmpegPath = value;
            }
            get
            {
                if (_disposed) throw new InvalidOperationException();
                return _ffmpegPath;
            }
        }

        private string _name;
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
        private bool _disposed;

        private static Random _random = new Random();
        private static HttpClient _httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });
    }
}
