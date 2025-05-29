using System;
using System.Windows.Forms;

namespace ChatRoomRecorder
{
    public partial class UrlsForm : Form
    {
        public UrlsForm()
        {
            InitializeComponent();
        }

        public string[] Urls
        {
            get
            {
                return UrlsTextBox.Lines;
            }
        }
    }
}