namespace ChatRoomRecorder
{
    partial class ThumbnailForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThumbnailForm));
            TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            WindowsMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            OKButton = new System.Windows.Forms.Button();
            TableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)WindowsMediaPlayer).BeginInit();
            TableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.ColumnCount = 1;
            TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel1.Controls.Add(WindowsMediaPlayer, 0, 0);
            TableLayoutPanel1.Controls.Add(TableLayoutPanel2, 0, 1);
            TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            TableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.Padding = new System.Windows.Forms.Padding(4);
            TableLayoutPanel1.RowCount = 2;
            TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            TableLayoutPanel1.Size = new System.Drawing.Size(484, 361);
            TableLayoutPanel1.TabIndex = 0;
            // 
            // WindowsMediaPlayer
            // 
            WindowsMediaPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            WindowsMediaPlayer.Enabled = true;
            WindowsMediaPlayer.Location = new System.Drawing.Point(4, 4);
            WindowsMediaPlayer.Margin = new System.Windows.Forms.Padding(0);
            WindowsMediaPlayer.Name = "WindowsMediaPlayer";
            WindowsMediaPlayer.OcxState = (System.Windows.Forms.AxHost.State)resources.GetObject("WindowsMediaPlayer.OcxState");
            WindowsMediaPlayer.Size = new System.Drawing.Size(476, 320);
            WindowsMediaPlayer.TabIndex = 1;
            WindowsMediaPlayer.OpenStateChange += WindowsMediaPlayer_OpenStateChange;
            // 
            // TableLayoutPanel2
            // 
            TableLayoutPanel2.ColumnCount = 2;
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            TableLayoutPanel2.Controls.Add(OKButton, 1, 0);
            TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            TableLayoutPanel2.Location = new System.Drawing.Point(4, 324);
            TableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            TableLayoutPanel2.Name = "TableLayoutPanel2";
            TableLayoutPanel2.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            TableLayoutPanel2.RowCount = 1;
            TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel2.Size = new System.Drawing.Size(476, 33);
            TableLayoutPanel2.TabIndex = 2;
            // 
            // OKButton
            // 
            OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            OKButton.Dock = System.Windows.Forms.DockStyle.Fill;
            OKButton.Enabled = false;
            OKButton.Location = new System.Drawing.Point(378, 4);
            OKButton.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            OKButton.Name = "OKButton";
            OKButton.Size = new System.Drawing.Size(98, 29);
            OKButton.TabIndex = 2;
            OKButton.Text = "OK";
            OKButton.UseVisualStyleBackColor = true;
            OKButton.Click += OKButton_Click;
            // 
            // ThumbnailForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(484, 361);
            Controls.Add(TableLayoutPanel1);
            Name = "ThumbnailForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Create thumbnail";
            FormClosed += ThumbnailForm_FormClosed;
            TableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)WindowsMediaPlayer).EndInit();
            TableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private AxWMPLib.AxWindowsMediaPlayer WindowsMediaPlayer;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel2;
        private System.Windows.Forms.Button OKButton;
    }
}