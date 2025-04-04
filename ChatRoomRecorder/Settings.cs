﻿using System.ComponentModel;

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
            settings._flirt4FreeConcurrentUpdates = _flirt4FreeConcurrentUpdates;
            settings._camSodaConcurrentUpdates = _camSodaConcurrentUpdates;
            settings._cam4ConcurrentUpdates = _cam4ConcurrentUpdates;
            settings._updateDelay = _updateDelay;
            settings._updateInterval = _updateInterval;
            settings._defaultAction = _defaultAction;
            settings._defaultResolution = _defaultResolution;
            settings._logSize = _logSize;
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
                _chaturbateConcurrentUpdates = value > 1 ? value : 1;
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
                _bongaCamsConcurrentUpdates = value > 1 ? value : 1;
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
                _stripchatConcurrentUpdates = value > 1 ? value : 1;
                NotifyPropertyChanged(nameof(StripchatConcurrentUpdates));
            }
        }

        public int Flirt4FreeConcurrentUpdates
        {
            get
            {
                return _flirt4FreeConcurrentUpdates;
            }
            set
            {
                _flirt4FreeConcurrentUpdates = value > 1 ? value : 1;
                NotifyPropertyChanged(nameof(Flirt4FreeConcurrentUpdates));
            }
        }

        public int CamSodaConcurrentUpdates
        {
            get
            {
                return _camSodaConcurrentUpdates;
            }
            set
            {
                _camSodaConcurrentUpdates = value > 1 ? value : 1;
                NotifyPropertyChanged(nameof(CamSodaConcurrentUpdates));
            }
        }

        public int Cam4ConcurrentUpdates
        {
            get
            {
                return _cam4ConcurrentUpdates;
            }
            set
            {
                _cam4ConcurrentUpdates = value > 1 ? value : 1;
                NotifyPropertyChanged(nameof(Cam4ConcurrentUpdates));
            }
        }

        public int UpdateDelay
        {
            get
            {
                return _updateDelay;
            }
            set
            {
                _updateDelay = value > 0 ? value : 0;
                NotifyPropertyChanged(nameof(UpdateDelay));
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
                _updateInterval = value > 1 ? value : 1;
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

        public int LogSize
        {
            get
            {
                return _logSize;
            }
            set
            {
                _logSize = value > 0 ? value : 0;
                NotifyPropertyChanged(nameof(LogSize));
            }
        }

        private string _outputDirectory;
        private string _ffmpegPath;
        private string _streamlinkPath;
        private int _chaturbateConcurrentUpdates;
        private int _bongaCamsConcurrentUpdates;
        private int _stripchatConcurrentUpdates;
        private int _flirt4FreeConcurrentUpdates;
        private int _camSodaConcurrentUpdates;
        private int _cam4ConcurrentUpdates;
        private int _updateDelay;
        private int _updateInterval;
        private ChatRoomAction _defaultAction;
        private ChatRoomResolution _defaultResolution;
        private int _logSize;
    }
}
