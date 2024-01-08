namespace ChatRoomRecorder
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.DataGridViewUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.TabControl = new System.Windows.Forms.TabControl();
            this.WebBrowserTabPage = new System.Windows.Forms.TabPage();
            this.StopButton = new System.Windows.Forms.Button();
            this.ForwardButton = new System.Windows.Forms.Button();
            this.BackButton = new System.Windows.Forms.Button();
            this.WebViewPanel = new System.Windows.Forms.Panel();
            this.WebView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.GoButton = new System.Windows.Forms.Button();
            this.AddressTextBox = new System.Windows.Forms.TextBox();
            this.ChatRoomsTabPage = new System.Windows.Forms.TabPage();
            this.ChatRoomsDataGridView = new System.Windows.Forms.DataGridView();
            this.IndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WebsiteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActionColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResolutionColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.LastUpdateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinusButton = new System.Windows.Forms.Button();
            this.PlusButton = new System.Windows.Forms.Button();
            this.URLTextBox = new System.Windows.Forms.TextBox();
            this.SettingsTabPage = new System.Windows.Forms.TabPage();
            this.FFmpegPathLabel = new System.Windows.Forms.Label();
            this.OutputDirectoryButton = new System.Windows.Forms.Button();
            this.FFmpegPathButton = new System.Windows.Forms.Button();
            this.FFmpegPathTextBox = new System.Windows.Forms.TextBox();
            this.OutputDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.OutputDirectoryLabel = new System.Windows.Forms.Label();
            this.AboutTabPage = new System.Windows.Forms.TabPage();
            this.LicenseTextBox = new System.Windows.Forms.TextBox();
            this.ChatRoomsUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.FormCloseTimer = new System.Windows.Forms.Timer(this.components);
            this.TabControl.SuspendLayout();
            this.WebBrowserTabPage.SuspendLayout();
            this.WebViewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WebView)).BeginInit();
            this.ChatRoomsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChatRoomsDataGridView)).BeginInit();
            this.SettingsTabPage.SuspendLayout();
            this.AboutTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataGridViewUpdateTimer
            // 
            this.DataGridViewUpdateTimer.Interval = 5000;
            this.DataGridViewUpdateTimer.Tick += new System.EventHandler(this.DataGridViewUpdateTimer_Tick);
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.WebBrowserTabPage);
            this.TabControl.Controls.Add(this.ChatRoomsTabPage);
            this.TabControl.Controls.Add(this.SettingsTabPage);
            this.TabControl.Controls.Add(this.AboutTabPage);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(784, 561);
            this.TabControl.TabIndex = 19;
            // 
            // WebBrowserTabPage
            // 
            this.WebBrowserTabPage.Controls.Add(this.StopButton);
            this.WebBrowserTabPage.Controls.Add(this.ForwardButton);
            this.WebBrowserTabPage.Controls.Add(this.BackButton);
            this.WebBrowserTabPage.Controls.Add(this.WebViewPanel);
            this.WebBrowserTabPage.Controls.Add(this.GoButton);
            this.WebBrowserTabPage.Controls.Add(this.AddressTextBox);
            this.WebBrowserTabPage.Location = new System.Drawing.Point(4, 22);
            this.WebBrowserTabPage.Name = "WebBrowserTabPage";
            this.WebBrowserTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.WebBrowserTabPage.Size = new System.Drawing.Size(776, 535);
            this.WebBrowserTabPage.TabIndex = 0;
            this.WebBrowserTabPage.Text = "Web browser";
            this.WebBrowserTabPage.UseVisualStyleBackColor = true;
            // 
            // StopButton
            // 
            this.StopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StopButton.Location = new System.Drawing.Point(730, 3);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(44, 22);
            this.StopButton.TabIndex = 5;
            this.StopButton.Text = "⤬";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // ForwardButton
            // 
            this.ForwardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ForwardButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForwardButton.Location = new System.Drawing.Point(684, 3);
            this.ForwardButton.Name = "ForwardButton";
            this.ForwardButton.Size = new System.Drawing.Size(44, 22);
            this.ForwardButton.TabIndex = 4;
            this.ForwardButton.Text = "→";
            this.ForwardButton.UseVisualStyleBackColor = true;
            this.ForwardButton.Click += new System.EventHandler(this.ForwardButton_Click);
            // 
            // BackButton
            // 
            this.BackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BackButton.Location = new System.Drawing.Point(638, 3);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(44, 22);
            this.BackButton.TabIndex = 3;
            this.BackButton.Text = "←";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // WebViewPanel
            // 
            this.WebViewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WebViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.WebViewPanel.Controls.Add(this.WebView);
            this.WebViewPanel.Location = new System.Drawing.Point(0, 28);
            this.WebViewPanel.Name = "WebViewPanel";
            this.WebViewPanel.Size = new System.Drawing.Size(774, 507);
            this.WebViewPanel.TabIndex = 2;
            // 
            // WebView
            // 
            this.WebView.AllowExternalDrop = true;
            this.WebView.CreationProperties = null;
            this.WebView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.WebView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebView.Location = new System.Drawing.Point(0, 0);
            this.WebView.Name = "WebView";
            this.WebView.Size = new System.Drawing.Size(772, 505);
            this.WebView.Source = new System.Uri("about:blank", System.UriKind.Absolute);
            this.WebView.TabIndex = 0;
            this.WebView.ZoomFactor = 1D;
            // 
            // GoButton
            // 
            this.GoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GoButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GoButton.Location = new System.Drawing.Point(592, 3);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(44, 22);
            this.GoButton.TabIndex = 1;
            this.GoButton.Text = "GO";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // AddressTextBox
            // 
            this.AddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddressTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AddressTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.AddressTextBox.Location = new System.Drawing.Point(0, 4);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new System.Drawing.Size(589, 20);
            this.AddressTextBox.TabIndex = 0;
            // 
            // ChatRoomsTabPage
            // 
            this.ChatRoomsTabPage.Controls.Add(this.ChatRoomsDataGridView);
            this.ChatRoomsTabPage.Controls.Add(this.MinusButton);
            this.ChatRoomsTabPage.Controls.Add(this.PlusButton);
            this.ChatRoomsTabPage.Controls.Add(this.URLTextBox);
            this.ChatRoomsTabPage.Location = new System.Drawing.Point(4, 22);
            this.ChatRoomsTabPage.Name = "ChatRoomsTabPage";
            this.ChatRoomsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ChatRoomsTabPage.Size = new System.Drawing.Size(776, 535);
            this.ChatRoomsTabPage.TabIndex = 1;
            this.ChatRoomsTabPage.Text = "Chat rooms";
            this.ChatRoomsTabPage.UseVisualStyleBackColor = true;
            // 
            // ChatRoomsDataGridView
            // 
            this.ChatRoomsDataGridView.AllowUserToAddRows = false;
            this.ChatRoomsDataGridView.AllowUserToDeleteRows = false;
            this.ChatRoomsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChatRoomsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ChatRoomsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.ChatRoomsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ChatRoomsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IndexColumn,
            this.WebsiteColumn,
            this.NameColumn,
            this.ActionColumn,
            this.StatusColumn,
            this.ResolutionColumn,
            this.LastUpdateColumn});
            this.ChatRoomsDataGridView.Location = new System.Drawing.Point(0, 28);
            this.ChatRoomsDataGridView.MultiSelect = false;
            this.ChatRoomsDataGridView.Name = "ChatRoomsDataGridView";
            this.ChatRoomsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ChatRoomsDataGridView.Size = new System.Drawing.Size(774, 506);
            this.ChatRoomsDataGridView.TabIndex = 11;
            this.ChatRoomsDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.ChatRoomsDataGridView_CellValidating);
            this.ChatRoomsDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ChatRoomsDataGridView_CellValueChanged);
            this.ChatRoomsDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ChatRoomsDataGridView_ColumnHeaderMouseClick);
            // 
            // IndexColumn
            // 
            this.IndexColumn.FillWeight = 57.46263F;
            this.IndexColumn.HeaderText = "#";
            this.IndexColumn.Name = "IndexColumn";
            this.IndexColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // WebsiteColumn
            // 
            this.WebsiteColumn.FillWeight = 110.095F;
            this.WebsiteColumn.HeaderText = "Website";
            this.WebsiteColumn.Name = "WebsiteColumn";
            this.WebsiteColumn.ReadOnly = true;
            this.WebsiteColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // NameColumn
            // 
            this.NameColumn.FillWeight = 106.1234F;
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // ActionColumn
            // 
            this.ActionColumn.FillWeight = 107.7252F;
            this.ActionColumn.HeaderText = "Action";
            this.ActionColumn.MaxDropDownItems = 16;
            this.ActionColumn.Name = "ActionColumn";
            this.ActionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // StatusColumn
            // 
            this.StatusColumn.FillWeight = 106.692F;
            this.StatusColumn.HeaderText = "Status";
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.ReadOnly = true;
            this.StatusColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // ResolutionColumn
            // 
            this.ResolutionColumn.FillWeight = 105.3025F;
            this.ResolutionColumn.HeaderText = "Resolution";
            this.ResolutionColumn.MaxDropDownItems = 16;
            this.ResolutionColumn.Name = "ResolutionColumn";
            this.ResolutionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // LastUpdateColumn
            // 
            this.LastUpdateColumn.FillWeight = 106.5989F;
            this.LastUpdateColumn.HeaderText = "Last update";
            this.LastUpdateColumn.Name = "LastUpdateColumn";
            this.LastUpdateColumn.ReadOnly = true;
            this.LastUpdateColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // MinusButton
            // 
            this.MinusButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MinusButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MinusButton.Location = new System.Drawing.Point(730, 3);
            this.MinusButton.Name = "MinusButton";
            this.MinusButton.Size = new System.Drawing.Size(44, 22);
            this.MinusButton.TabIndex = 8;
            this.MinusButton.Text = "-";
            this.MinusButton.UseVisualStyleBackColor = true;
            this.MinusButton.Click += new System.EventHandler(this.MinusButton_Click);
            // 
            // PlusButton
            // 
            this.PlusButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PlusButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PlusButton.Location = new System.Drawing.Point(684, 3);
            this.PlusButton.Name = "PlusButton";
            this.PlusButton.Size = new System.Drawing.Size(44, 22);
            this.PlusButton.TabIndex = 7;
            this.PlusButton.Text = "+";
            this.PlusButton.UseVisualStyleBackColor = true;
            this.PlusButton.Click += new System.EventHandler(this.PlusButton_Click);
            // 
            // URLTextBox
            // 
            this.URLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.URLTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.URLTextBox.Location = new System.Drawing.Point(0, 4);
            this.URLTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.URLTextBox.Name = "URLTextBox";
            this.URLTextBox.Size = new System.Drawing.Size(681, 20);
            this.URLTextBox.TabIndex = 6;
            // 
            // SettingsTabPage
            // 
            this.SettingsTabPage.Controls.Add(this.FFmpegPathLabel);
            this.SettingsTabPage.Controls.Add(this.OutputDirectoryButton);
            this.SettingsTabPage.Controls.Add(this.FFmpegPathButton);
            this.SettingsTabPage.Controls.Add(this.FFmpegPathTextBox);
            this.SettingsTabPage.Controls.Add(this.OutputDirectoryTextBox);
            this.SettingsTabPage.Controls.Add(this.OutputDirectoryLabel);
            this.SettingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.SettingsTabPage.Name = "SettingsTabPage";
            this.SettingsTabPage.Size = new System.Drawing.Size(776, 535);
            this.SettingsTabPage.TabIndex = 2;
            this.SettingsTabPage.Text = "Settings";
            this.SettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // FFmpegPathLabel
            // 
            this.FFmpegPathLabel.AutoSize = true;
            this.FFmpegPathLabel.Location = new System.Drawing.Point(-3, 8);
            this.FFmpegPathLabel.Name = "FFmpegPathLabel";
            this.FFmpegPathLabel.Size = new System.Drawing.Size(69, 13);
            this.FFmpegPathLabel.TabIndex = 32;
            this.FFmpegPathLabel.Text = "FFmpeg path";
            // 
            // OutputDirectoryButton
            // 
            this.OutputDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputDirectoryButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OutputDirectoryButton.Location = new System.Drawing.Point(730, 28);
            this.OutputDirectoryButton.Name = "OutputDirectoryButton";
            this.OutputDirectoryButton.Size = new System.Drawing.Size(44, 22);
            this.OutputDirectoryButton.TabIndex = 30;
            this.OutputDirectoryButton.Text = "...";
            this.OutputDirectoryButton.UseVisualStyleBackColor = true;
            this.OutputDirectoryButton.Click += new System.EventHandler(this.OutputDirectoryButton_Click);
            // 
            // FFmpegPathButton
            // 
            this.FFmpegPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FFmpegPathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FFmpegPathButton.Location = new System.Drawing.Point(730, 3);
            this.FFmpegPathButton.Name = "FFmpegPathButton";
            this.FFmpegPathButton.Size = new System.Drawing.Size(44, 22);
            this.FFmpegPathButton.TabIndex = 29;
            this.FFmpegPathButton.Text = "...";
            this.FFmpegPathButton.UseVisualStyleBackColor = true;
            this.FFmpegPathButton.Click += new System.EventHandler(this.FFmpegPathButton_Click);
            // 
            // FFmpegPathTextBox
            // 
            this.FFmpegPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FFmpegPathTextBox.Location = new System.Drawing.Point(78, 4);
            this.FFmpegPathTextBox.Name = "FFmpegPathTextBox";
            this.FFmpegPathTextBox.ReadOnly = true;
            this.FFmpegPathTextBox.Size = new System.Drawing.Size(649, 20);
            this.FFmpegPathTextBox.TabIndex = 22;
            // 
            // OutputDirectoryTextBox
            // 
            this.OutputDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputDirectoryTextBox.Location = new System.Drawing.Point(78, 29);
            this.OutputDirectoryTextBox.Name = "OutputDirectoryTextBox";
            this.OutputDirectoryTextBox.ReadOnly = true;
            this.OutputDirectoryTextBox.Size = new System.Drawing.Size(649, 20);
            this.OutputDirectoryTextBox.TabIndex = 24;
            // 
            // OutputDirectoryLabel
            // 
            this.OutputDirectoryLabel.AutoSize = true;
            this.OutputDirectoryLabel.Location = new System.Drawing.Point(-3, 31);
            this.OutputDirectoryLabel.Name = "OutputDirectoryLabel";
            this.OutputDirectoryLabel.Size = new System.Drawing.Size(82, 13);
            this.OutputDirectoryLabel.TabIndex = 21;
            this.OutputDirectoryLabel.Text = "Output directory";
            // 
            // AboutTabPage
            // 
            this.AboutTabPage.Controls.Add(this.LicenseTextBox);
            this.AboutTabPage.Location = new System.Drawing.Point(4, 22);
            this.AboutTabPage.Name = "AboutTabPage";
            this.AboutTabPage.Size = new System.Drawing.Size(776, 535);
            this.AboutTabPage.TabIndex = 3;
            this.AboutTabPage.Text = "About";
            this.AboutTabPage.UseVisualStyleBackColor = true;
            // 
            // LicenseTextBox
            // 
            this.LicenseTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LicenseTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LicenseTextBox.Location = new System.Drawing.Point(0, 0);
            this.LicenseTextBox.Multiline = true;
            this.LicenseTextBox.Name = "LicenseTextBox";
            this.LicenseTextBox.Size = new System.Drawing.Size(776, 535);
            this.LicenseTextBox.TabIndex = 2;
            this.LicenseTextBox.Text = resources.GetString("LicenseTextBox.Text");
            // 
            // ChatRoomsUpdateTimer
            // 
            this.ChatRoomsUpdateTimer.Interval = 5000;
            this.ChatRoomsUpdateTimer.Tick += new System.EventHandler(this.ChatRoomsUpdateTimer_Tick);
            // 
            // FormCloseTimer
            // 
            this.FormCloseTimer.Interval = 1000;
            this.FormCloseTimer.Tick += new System.EventHandler(this.FormCloseTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.TabControl);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.TabControl.ResumeLayout(false);
            this.WebBrowserTabPage.ResumeLayout(false);
            this.WebBrowserTabPage.PerformLayout();
            this.WebViewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.WebView)).EndInit();
            this.ChatRoomsTabPage.ResumeLayout(false);
            this.ChatRoomsTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChatRoomsDataGridView)).EndInit();
            this.SettingsTabPage.ResumeLayout(false);
            this.SettingsTabPage.PerformLayout();
            this.AboutTabPage.ResumeLayout(false);
            this.AboutTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer DataGridViewUpdateTimer;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage WebBrowserTabPage;
        private System.Windows.Forms.TabPage ChatRoomsTabPage;
        private System.Windows.Forms.TabPage SettingsTabPage;
        private System.Windows.Forms.DataGridView ChatRoomsDataGridView;
        private System.Windows.Forms.Button MinusButton;
        private System.Windows.Forms.Button PlusButton;
        private System.Windows.Forms.TextBox URLTextBox;
        private System.Windows.Forms.Button OutputDirectoryButton;
        private System.Windows.Forms.Button FFmpegPathButton;
        private System.Windows.Forms.TextBox FFmpegPathTextBox;
        private System.Windows.Forms.TextBox OutputDirectoryTextBox;
        private System.Windows.Forms.Label OutputDirectoryLabel;
        private System.Windows.Forms.TabPage AboutTabPage;
        private System.Windows.Forms.TextBox LicenseTextBox;
        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.TextBox AddressTextBox;
        private System.Windows.Forms.Timer ChatRoomsUpdateTimer;
        private System.Windows.Forms.Panel WebViewPanel;
        private Microsoft.Web.WebView2.WinForms.WebView2 WebView;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button ForwardButton;
        private System.Windows.Forms.Label FFmpegPathLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn IndexColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn WebsiteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ActionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ResolutionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdateColumn;
        private System.Windows.Forms.Timer FormCloseTimer;
    }
}

