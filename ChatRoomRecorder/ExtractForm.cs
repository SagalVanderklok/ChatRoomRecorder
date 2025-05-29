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

            StartDateTimePicker.MinDate = StartDateTimePicker.MaxDate = EndDateTimePicker.MinDate = EndDateTimePicker.MaxDate = new DateTime(2000, 1, 1);
        }

        private void ExtractForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            WindowsMediaPlayer.close();
        }

        private void WindowsMediaPlayer_OpenStateChange(object sender, AxWMPLib._WMPOCXEvents_OpenStateChangeEvent e)
        {
            if (e.newState == 13)
            {
                EndDateTimePicker.Value = StartDateTimePicker.MaxDate = EndDateTimePicker.MaxDate = new DateTime(2000, 1, 1).AddSeconds(Math.Ceiling(WindowsMediaPlayer.currentMedia.duration));
            }
        }

        private void DateTimePicker_ValueChanged(object sender, System.EventArgs e)
        {
            OKButton.Enabled = StartDateTimePicker.Value < EndDateTimePicker.Value;
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
    }
}