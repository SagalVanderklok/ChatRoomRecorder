namespace ChatRoomRecorder
{
    partial class ExtractForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtractForm));
            WindowsMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            StartDateTimePicker = new System.Windows.Forms.DateTimePicker();
            EndDateTimePicker = new System.Windows.Forms.DateTimePicker();
            DashLabel = new System.Windows.Forms.Label();
            TableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            OKButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)WindowsMediaPlayer).BeginInit();
            TableLayoutPanel1.SuspendLayout();
            TableLayoutPanel2.SuspendLayout();
            TableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // WindowsMediaPlayer
            // 
            WindowsMediaPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            WindowsMediaPlayer.Enabled = true;
            WindowsMediaPlayer.Location = new System.Drawing.Point(4, 4);
            WindowsMediaPlayer.Margin = new System.Windows.Forms.Padding(0);
            WindowsMediaPlayer.Name = "WindowsMediaPlayer";
            WindowsMediaPlayer.OcxState = (System.Windows.Forms.AxHost.State)resources.GetObject("WindowsMediaPlayer.OcxState");
            WindowsMediaPlayer.Size = new System.Drawing.Size(476, 287);
            WindowsMediaPlayer.TabIndex = 1;
            WindowsMediaPlayer.OpenStateChange += WindowsMediaPlayer_OpenStateChange;
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.ColumnCount = 1;
            TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel1.Controls.Add(WindowsMediaPlayer, 0, 0);
            TableLayoutPanel1.Controls.Add(TableLayoutPanel2, 0, 1);
            TableLayoutPanel1.Controls.Add(TableLayoutPanel3, 0, 2);
            TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            TableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.Padding = new System.Windows.Forms.Padding(4);
            TableLayoutPanel1.RowCount = 3;
            TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            TableLayoutPanel1.Size = new System.Drawing.Size(484, 361);
            TableLayoutPanel1.TabIndex = 1;
            // 
            // TableLayoutPanel2
            // 
            TableLayoutPanel2.ColumnCount = 5;
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TableLayoutPanel2.Controls.Add(StartDateTimePicker, 1, 0);
            TableLayoutPanel2.Controls.Add(EndDateTimePicker, 3, 0);
            TableLayoutPanel2.Controls.Add(DashLabel, 2, 0);
            TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            TableLayoutPanel2.Location = new System.Drawing.Point(4, 291);
            TableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            TableLayoutPanel2.Name = "TableLayoutPanel2";
            TableLayoutPanel2.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            TableLayoutPanel2.RowCount = 1;
            TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel2.Size = new System.Drawing.Size(476, 33);
            TableLayoutPanel2.TabIndex = 1;
            // 
            // StartDateTimePicker
            // 
            StartDateTimePicker.CustomFormat = "HH:mm:ss";
            StartDateTimePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            StartDateTimePicker.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StartDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            StartDateTimePicker.Location = new System.Drawing.Point(130, 4);
            StartDateTimePicker.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            StartDateTimePicker.Name = "StartDateTimePicker";
            StartDateTimePicker.ShowUpDown = true;
            StartDateTimePicker.Size = new System.Drawing.Size(96, 24);
            StartDateTimePicker.TabIndex = 2;
            StartDateTimePicker.Value = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            StartDateTimePicker.ValueChanged += DateTimePicker_ValueChanged;
            // 
            // EndDateTimePicker
            // 
            EndDateTimePicker.CustomFormat = "HH:mm:ss";
            EndDateTimePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            EndDateTimePicker.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            EndDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            EndDateTimePicker.Location = new System.Drawing.Point(250, 4);
            EndDateTimePicker.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            EndDateTimePicker.Name = "EndDateTimePicker";
            EndDateTimePicker.ShowUpDown = true;
            EndDateTimePicker.Size = new System.Drawing.Size(96, 24);
            EndDateTimePicker.TabIndex = 3;
            EndDateTimePicker.Value = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            EndDateTimePicker.ValueChanged += DateTimePicker_ValueChanged;
            // 
            // DashLabel
            // 
            DashLabel.AutoSize = true;
            DashLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            DashLabel.Location = new System.Drawing.Point(230, 4);
            DashLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            DashLabel.Name = "DashLabel";
            DashLabel.Size = new System.Drawing.Size(16, 29);
            DashLabel.TabIndex = 4;
            DashLabel.Text = "-";
            DashLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TableLayoutPanel3
            // 
            TableLayoutPanel3.ColumnCount = 2;
            TableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            TableLayoutPanel3.Controls.Add(OKButton, 1, 0);
            TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            TableLayoutPanel3.Location = new System.Drawing.Point(4, 324);
            TableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            TableLayoutPanel3.Name = "TableLayoutPanel3";
            TableLayoutPanel3.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            TableLayoutPanel3.RowCount = 1;
            TableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TableLayoutPanel3.Size = new System.Drawing.Size(476, 33);
            TableLayoutPanel3.TabIndex = 2;
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
            OKButton.TabIndex = 4;
            OKButton.Text = "OK";
            OKButton.UseVisualStyleBackColor = true;
            // 
            // ExtractForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(484, 361);
            Controls.Add(TableLayoutPanel1);
            Name = "ExtractForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Extract";
            FormClosed += ExtractForm_FormClosed;
            ((System.ComponentModel.ISupportInitialize)WindowsMediaPlayer).EndInit();
            TableLayoutPanel1.ResumeLayout(false);
            TableLayoutPanel2.ResumeLayout(false);
            TableLayoutPanel2.PerformLayout();
            TableLayoutPanel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer WindowsMediaPlayer;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel3;
        private System.Windows.Forms.DateTimePicker StartDateTimePicker;
        private System.Windows.Forms.DateTimePicker EndDateTimePicker;
        private System.Windows.Forms.Label DashLabel;
        private System.Windows.Forms.Button OKButton;
    }
}