using System;
using System.Windows.Forms;

namespace ChatRoomRecorder
{
    public partial class ExtractForm : Form
    {
        public ExtractForm(string Url)
        {
            InitializeComponent();

            WindowsMediaPlayer.URL = Url;
        }

        public string StartTime
        {
            get
            {
                return TimeOnly.FromDateTime(StartDateTimePicker.Value).ToString("HH:mm:ss");
            }
        }

        public string EndTime
        {
            get
            {
                return TimeOnly.FromDateTime(EndDateTimePicker.Value).ToString("HH:mm:ss");
            }
        }

        private void DateTimePicker_ValueChanged(object sender, System.EventArgs e)
        {
            OKButton.Enabled =
                TimeOnly.FromDateTime(StartDateTimePicker.Value) < TimeOnly.FromDateTime(EndDateTimePicker.Value) &&
                TimeOnly.FromDateTime(EndDateTimePicker.Value) <= TimeOnly.FromTimeSpan(TimeSpan.FromSeconds((int)Math.Ceiling(WindowsMediaPlayer.currentMedia.duration)));
        }

        private void WindowsMediaPlayer_OpenStateChange(object sender, AxWMPLib._WMPOCXEvents_OpenStateChangeEvent e)
        {
            if (e.newState == 13)
            {
                EndDateTimePicker.Value = EndDateTimePicker.Value.AddSeconds(WindowsMediaPlayer.currentMedia.duration);
            }
        }
    }
}