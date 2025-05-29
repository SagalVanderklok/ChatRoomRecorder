using System;
using System.Windows.Forms;

namespace ChatRoomRecorder
{
    public partial class ThumbnailForm : Form
    {
        public ThumbnailForm(string Url)
        {
            InitializeComponent();

            WindowsMediaPlayer.URL = Url;
        }

        private void ThumbnailForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            WindowsMediaPlayer.close();
        }

        private void WindowsMediaPlayer_OpenStateChange(object sender, AxWMPLib._WMPOCXEvents_OpenStateChangeEvent e)
        {
            if (e.newState == 13)
            {
                OKButton.Enabled = true;
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (WindowsMediaPlayer.Ctlcontrols.currentPositionString != string.Empty)
            {
                _time = TimeOnly.FromTimeSpan(DateTime.MinValue.AddSeconds(WindowsMediaPlayer.Ctlcontrols.currentPosition) - DateTime.MinValue).ToString("HH:mm:ss.fff");
            }
        }

        public string Time
        {
            get
            {
                return _time;
            }
        }

        private string _time = "00:00:00";
    }
}