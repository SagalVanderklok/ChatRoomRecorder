using System.ComponentModel;

namespace ChatRoomRecorder
{
    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void CopyTo(Settings settings)
        {
            settings._outputDirectory = _outputDirectory;
            settings._ffmpegPath = _ffmpegPath;
            settings._streamlinkPath = _streamlinkPath;
            settings._chaturbateConcurrentUpdates = _chaturbateConcurrentUpdates;
            settings._bongaCamsConcurrentUpdates = _bongaCamsConcurrentUpdates;
            settings._stripchatConcurrentUpdates = _stripchatConcurrentUpdates;
            settings._updateInterval = _updateInterval;
            settings._defaultAction = _defaultAction;
            settings._defaultResolution = _defaultResolution;
        }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string OutputDirectory
        {
            get
            {
                return _outputDirectory;
            }
            set
            {
                _outputDirectory = value;
                NotifyPropertyChanged(nameof(OutputDirectory));
            }
        }

        public string FFmpegPath
        {
            get
            {
                return _ffmpegPath;
            }
            set
            {
                _ffmpegPath = value;
                NotifyPropertyChanged(nameof(FFmpegPath));
            }
        }

        public string StreamlinkPath
        {
            get
            {
                return _streamlinkPath;
            }
            set
            {
                _streamlinkPath = value;
                NotifyPropertyChanged(nameof(StreamlinkPath));
            }
        }

        public int ChaturbateConcurrentUpdates
        {
            get
            {
                return _chaturbateConcurrentUpdates;
            }
            set
            {
                _chaturbateConcurrentUpdates = value >= 1 ? value : 1;
                NotifyPropertyChanged(nameof(ChaturbateConcurrentUpdates));
            }
        }

        public int BongaCamsConcurrentUpdates
        {
            get
            {
                return _bongaCamsConcurrentUpdates;
            }
            set
            {
                _bongaCamsConcurrentUpdates = value >= 1 ? value : 1;
                NotifyPropertyChanged(nameof(BongaCamsConcurrentUpdates));
            }
        }

        public int StripchatConcurrentUpdates
        {
            get
            {
                return _stripchatConcurrentUpdates;
            }
            set
            {
                _stripchatConcurrentUpdates = value >= 1 ? value : 1;
                NotifyPropertyChanged(nameof(StripchatConcurrentUpdates));
            }
        }

        public int UpdateInterval
        {
            get
            {
                return _updateInterval;
            }
            set
            {
                _updateInterval = value >= 1 ? value : 1;
                NotifyPropertyChanged(nameof(UpdateInterval));
            }
        }

        public ChatRoomAction DefaultAction
        {
            get
            {
                return _defaultAction;
            }
            set
            {
                _defaultAction = value;
                NotifyPropertyChanged(nameof(DefaultAction));
            }
        }

        public ChatRoomResolution DefaultResolution
        {
            get
            {
                return _defaultResolution;
            }
            set
            {
                _defaultResolution = value;
                NotifyPropertyChanged(nameof(DefaultResolution));
            }
        }

        private string _outputDirectory;
        private string _ffmpegPath;
        private string _streamlinkPath;
        private int _chaturbateConcurrentUpdates;
        private int _bongaCamsConcurrentUpdates;
        private int _stripchatConcurrentUpdates;
        private int _updateInterval;
        private ChatRoomAction _defaultAction;
        private ChatRoomResolution _defaultResolution;
    }
}
