using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace ChatRoomRecorder
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            while (true)
            {
                Process[] chrome_instances = Process.GetProcessesByName("chrome");
                if (chrome_instances.Length > 0)
                {
                    if (MessageBox.Show("Close all instances of Chrome Chrome and click \"Retry\" to start the application or click \"Cancel\" to exit",
                        Assembly.GetEntryAssembly().GetName().Name + " " + Assembly.GetEntryAssembly().GetName().Version.ToString(3),
                        MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                else
                {
                    break;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
