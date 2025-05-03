namespace ChatRoomRecorder
{
    partial class TaskForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ProgressBar = new System.Windows.Forms.ProgressBar();
            Timer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // ProgressBar
            // 
            ProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            ProgressBar.Location = new System.Drawing.Point(0, 0);
            ProgressBar.Margin = new System.Windows.Forms.Padding(4);
            ProgressBar.Name = "ProgressBar";
            ProgressBar.Size = new System.Drawing.Size(484, 31);
            ProgressBar.Step = 20;
            ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            ProgressBar.TabIndex = 0;
            // 
            // Timer
            // 
            Timer.Interval = 500;
            Timer.Tick += Timer_Tick;
            // 
            // TaskForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(484, 31);
            Controls.Add(ProgressBar);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TaskForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            FormClosing += TaskForm_FormClosing;
            FormClosed += TaskForm_FormClosed;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Timer Timer;
    }
}