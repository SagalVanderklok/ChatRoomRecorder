using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ChatRoomRecorder
{
    public partial class TaskForm : Form
    {
        public TaskForm(string name, ProcessStartInfo psi)
        {
            InitializeComponent();

            Text = name;

            psi.UseShellExecute = false;
            psi.LoadUserProfile = false;
            psi.CreateNoWindow = true;

            _process = Process.Start(psi);
            _semaphore = new SemaphoreSlim(1, 1);

            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_semaphore.CurrentCount > 0)
            {
                _semaphore.Wait();
                try
                {
                    if (Timer.Enabled && _process.HasExited && DialogResult == DialogResult.None)
                    {
                        DialogResult = DialogResult.OK;
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        private void TaskForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _semaphore.Wait();
            try
            {
                if (DialogResult == DialogResult.Cancel && MessageBox.Show(c_TaskAbortingWarningMessage, string.Empty, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void TaskForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _semaphore.Wait();
            try
            {
                Timer.Stop();

                _process.Kill();
                _process.WaitForExit();
                _process.Close();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private Process _process;
        private SemaphoreSlim _semaphore;

        private const string c_TaskAbortingWarningMessage = "The task will be aborted!";
    }
}
