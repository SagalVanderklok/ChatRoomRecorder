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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
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
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.URLTextBox = new System.Windows.Forms.TextBox();
            this.ChatRoomsDataGridView = new System.Windows.Forms.DataGridView();
            this.IndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WebsiteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActionColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResolutionColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.LastUpdateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlusButton = new System.Windows.Forms.Button();
            this.FilesDataGridView = new System.Windows.Forms.DataGridView();
            this.FileNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileSizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.Panel2.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChatRoomsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FilesDataGridView)).BeginInit();
            this.SettingsTabPage.SuspendLayout();
            this.AboutTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.WebBrowserTabPage);
            this.TabControl.Controls.Add(this.ChatRoomsTabPage);
            this.TabControl.Controls.Add(this.SettingsTabPage);
            this.TabControl.Controls.Add(this.AboutTabPage);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Enabled = false;
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
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
            this.WebBrowserTabPage.Location = new System.Drawing.Point(4, 24);
            this.WebBrowserTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.WebBrowserTabPage.Name = "WebBrowserTabPage";
            this.WebBrowserTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.WebBrowserTabPage.Size = new System.Drawing.Size(776, 533);
            this.WebBrowserTabPage.TabIndex = 0;
            this.WebBrowserTabPage.Text = "Web browser";
            this.WebBrowserTabPage.UseVisualStyleBackColor = true;
            // 
            // StopButton
            // 
            this.StopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StopButton.Location = new System.Drawing.Point(723, 4);
            this.StopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(51, 25);
            this.StopButton.TabIndex = 5;
            this.StopButton.Text = "×";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // ForwardButton
            // 
            this.ForwardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ForwardButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForwardButton.Location = new System.Drawing.Point(668, 4);
            this.ForwardButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ForwardButton.Name = "ForwardButton";
            this.ForwardButton.Size = new System.Drawing.Size(51, 25);
            this.ForwardButton.TabIndex = 4;
            this.ForwardButton.Text = "→";
            this.ForwardButton.UseVisualStyleBackColor = true;
            this.ForwardButton.Click += new System.EventHandler(this.ForwardButton_Click);
            // 
            // BackButton
            // 
            this.BackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BackButton.Location = new System.Drawing.Point(613, 4);
            this.BackButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(51, 25);
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
            this.WebViewPanel.Location = new System.Drawing.Point(0, 33);
            this.WebViewPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.WebViewPanel.Name = "WebViewPanel";
            this.WebViewPanel.Size = new System.Drawing.Size(773, 499);
            this.WebViewPanel.TabIndex = 2;
            // 
            // WebView
            // 
            this.WebView.AllowExternalDrop = true;
            this.WebView.CreationProperties = null;
            this.WebView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.WebView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebView.Location = new System.Drawing.Point(0, 0);
            this.WebView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.WebView.Name = "WebView";
            this.WebView.Size = new System.Drawing.Size(771, 497);
            this.WebView.Source = new System.Uri("about:blank", System.UriKind.Absolute);
            this.WebView.TabIndex = 0;
            this.WebView.ZoomFactor = 1D;
            this.WebView.CoreWebView2InitializationCompleted += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs>(this.WebView_CoreWebView2InitializationCompleted);
            // 
            // GoButton
            // 
            this.GoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GoButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GoButton.Location = new System.Drawing.Point(558, 4);
            this.GoButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(51, 25);
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
            this.AddressTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.AddressTextBox.Location = new System.Drawing.Point(0, 5);
            this.AddressTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new System.Drawing.Size(553, 23);
            this.AddressTextBox.TabIndex = 0;
            // 
            // ChatRoomsTabPage
            // 
            this.ChatRoomsTabPage.Controls.Add(this.SplitContainer);
            this.ChatRoomsTabPage.Location = new System.Drawing.Point(4, 24);
            this.ChatRoomsTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ChatRoomsTabPage.Name = "ChatRoomsTabPage";
            this.ChatRoomsTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ChatRoomsTabPage.Size = new System.Drawing.Size(776, 533);
            this.ChatRoomsTabPage.TabIndex = 1;
            this.ChatRoomsTabPage.Text = "Chat rooms";
            this.ChatRoomsTabPage.UseVisualStyleBackColor = true;
            // 
            // SplitContainer
            // 
            this.SplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitContainer.BackColor = System.Drawing.Color.Transparent;
            this.SplitContainer.Location = new System.Drawing.Point(0, 3);
            this.SplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.SplitContainer.Name = "SplitContainer";
            this.SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.SplitContainer.Panel1.Controls.Add(this.URLTextBox);
            this.SplitContainer.Panel1.Controls.Add(this.ChatRoomsDataGridView);
            this.SplitContainer.Panel1.Controls.Add(this.PlusButton);
            // 
            // SplitContainer.Panel2
            // 
            this.SplitContainer.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.SplitContainer.Panel2.Controls.Add(this.FilesDataGridView);
            this.SplitContainer.Size = new System.Drawing.Size(773, 529);
            this.SplitContainer.SplitterDistance = 375;
            this.SplitContainer.TabIndex = 12;
            // 
            // URLTextBox
            // 
            this.URLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.URLTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.URLTextBox.Location = new System.Drawing.Point(0, 2);
            this.URLTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.URLTextBox.Name = "URLTextBox";
            this.URLTextBox.Size = new System.Drawing.Size(718, 23);
            this.URLTextBox.TabIndex = 6;
            this.URLTextBox.TextChanged += new System.EventHandler(this.URLTextBox_TextChanged);
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ChatRoomsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ChatRoomsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ChatRoomsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IndexColumn,
            this.WebsiteColumn,
            this.NameColumn,
            this.ActionColumn,
            this.StatusColumn,
            this.ResolutionColumn,
            this.LastUpdateColumn});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ChatRoomsDataGridView.DefaultCellStyle = dataGridViewCellStyle9;
            this.ChatRoomsDataGridView.Location = new System.Drawing.Point(0, 30);
            this.ChatRoomsDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ChatRoomsDataGridView.Name = "ChatRoomsDataGridView";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ChatRoomsDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.ChatRoomsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ChatRoomsDataGridView.Size = new System.Drawing.Size(773, 344);
            this.ChatRoomsDataGridView.TabIndex = 11;
            this.ChatRoomsDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.ChatRoomsDataGridView_CellBeginEdit);
            this.ChatRoomsDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.ChatRoomsDataGridView_CellEndEdit);
            this.ChatRoomsDataGridView.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.ChatRoomsDataGridView_CellLeave);
            this.ChatRoomsDataGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ChatRoomsDataGridView_CellMouseClick);
            this.ChatRoomsDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.ChatRoomsDataGridView_CellValidating);
            this.ChatRoomsDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ChatRoomsDataGridView_CellValueChanged);
            this.ChatRoomsDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ChatRoomsDataGridView_ColumnHeaderMouseClick);
            this.ChatRoomsDataGridView.CurrentCellChanged += new System.EventHandler(this.ChatRoomsDataGridView_CurrentCellChanged);
            // 
            // IndexColumn
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IndexColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.IndexColumn.FillWeight = 57.46263F;
            this.IndexColumn.HeaderText = "#";
            this.IndexColumn.Name = "IndexColumn";
            this.IndexColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // WebsiteColumn
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.WebsiteColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.WebsiteColumn.FillWeight = 110.095F;
            this.WebsiteColumn.HeaderText = "Website";
            this.WebsiteColumn.Name = "WebsiteColumn";
            this.WebsiteColumn.ReadOnly = true;
            this.WebsiteColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // NameColumn
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.NameColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.NameColumn.FillWeight = 106.1234F;
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // ActionColumn
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ActionColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.ActionColumn.FillWeight = 107.7252F;
            this.ActionColumn.HeaderText = "Action";
            this.ActionColumn.MaxDropDownItems = 16;
            this.ActionColumn.Name = "ActionColumn";
            this.ActionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // StatusColumn
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StatusColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.StatusColumn.FillWeight = 106.692F;
            this.StatusColumn.HeaderText = "Status";
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.ReadOnly = true;
            this.StatusColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // ResolutionColumn
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ResolutionColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.ResolutionColumn.FillWeight = 105.3025F;
            this.ResolutionColumn.HeaderText = "Resolution";
            this.ResolutionColumn.MaxDropDownItems = 16;
            this.ResolutionColumn.Name = "ResolutionColumn";
            this.ResolutionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // LastUpdateColumn
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LastUpdateColumn.DefaultCellStyle = dataGridViewCellStyle8;
            this.LastUpdateColumn.FillWeight = 106.5989F;
            this.LastUpdateColumn.HeaderText = "Last update";
            this.LastUpdateColumn.Name = "LastUpdateColumn";
            this.LastUpdateColumn.ReadOnly = true;
            this.LastUpdateColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // PlusButton
            // 
            this.PlusButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PlusButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PlusButton.Location = new System.Drawing.Point(723, 1);
            this.PlusButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.PlusButton.Name = "PlusButton";
            this.PlusButton.Size = new System.Drawing.Size(51, 25);
            this.PlusButton.TabIndex = 7;
            this.PlusButton.Text = "+";
            this.PlusButton.UseVisualStyleBackColor = true;
            this.PlusButton.Click += new System.EventHandler(this.PlusButton_Click);
            // 
            // FilesDataGridView
            // 
            this.FilesDataGridView.AllowUserToAddRows = false;
            this.FilesDataGridView.AllowUserToDeleteRows = false;
            this.FilesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.FilesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.FilesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FilesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileNameColumn,
            this.FileSizeColumn});
            this.FilesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilesDataGridView.Location = new System.Drawing.Point(0, 0);
            this.FilesDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FilesDataGridView.Name = "FilesDataGridView";
            this.FilesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.FilesDataGridView.Size = new System.Drawing.Size(773, 150);
            this.FilesDataGridView.TabIndex = 0;
            this.FilesDataGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.FilesDataGridView_CellMouseClick);
            this.FilesDataGridView.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.FilesDataGridView_CellMouseDoubleClick);
            this.FilesDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.FilesDataGridView_ColumnHeaderMouseClick);
            // 
            // FileNameColumn
            // 
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FileNameColumn.DefaultCellStyle = dataGridViewCellStyle11;
            this.FileNameColumn.FillWeight = 199.8681F;
            this.FileNameColumn.HeaderText = "File name";
            this.FileNameColumn.Name = "FileNameColumn";
            this.FileNameColumn.ReadOnly = true;
            this.FileNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // FileSizeColumn
            // 
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle12.Format = "N0";
            dataGridViewCellStyle12.NullValue = null;
            this.FileSizeColumn.DefaultCellStyle = dataGridViewCellStyle12;
            this.FileSizeColumn.FillWeight = 50.25381F;
            this.FileSizeColumn.HeaderText = "File size";
            this.FileSizeColumn.Name = "FileSizeColumn";
            this.FileSizeColumn.ReadOnly = true;
            this.FileSizeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // SettingsTabPage
            // 
            this.SettingsTabPage.Controls.Add(this.FFmpegPathLabel);
            this.SettingsTabPage.Controls.Add(this.OutputDirectoryButton);
            this.SettingsTabPage.Controls.Add(this.FFmpegPathButton);
            this.SettingsTabPage.Controls.Add(this.FFmpegPathTextBox);
            this.SettingsTabPage.Controls.Add(this.OutputDirectoryTextBox);
            this.SettingsTabPage.Controls.Add(this.OutputDirectoryLabel);
            this.SettingsTabPage.Location = new System.Drawing.Point(4, 24);
            this.SettingsTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SettingsTabPage.Name = "SettingsTabPage";
            this.SettingsTabPage.Size = new System.Drawing.Size(776, 533);
            this.SettingsTabPage.TabIndex = 2;
            this.SettingsTabPage.Text = "Settings";
            this.SettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // FFmpegPathLabel
            // 
            this.FFmpegPathLabel.AutoSize = true;
            this.FFmpegPathLabel.Location = new System.Drawing.Point(-4, 36);
            this.FFmpegPathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FFmpegPathLabel.Name = "FFmpegPathLabel";
            this.FFmpegPathLabel.Size = new System.Drawing.Size(77, 15);
            this.FFmpegPathLabel.TabIndex = 32;
            this.FFmpegPathLabel.Text = "FFmpeg path";
            // 
            // OutputDirectoryButton
            // 
            this.OutputDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputDirectoryButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.OutputDirectoryButton.Location = new System.Drawing.Point(723, 4);
            this.OutputDirectoryButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.OutputDirectoryButton.Name = "OutputDirectoryButton";
            this.OutputDirectoryButton.Size = new System.Drawing.Size(51, 25);
            this.OutputDirectoryButton.TabIndex = 30;
            this.OutputDirectoryButton.Text = "...";
            this.OutputDirectoryButton.UseVisualStyleBackColor = true;
            this.OutputDirectoryButton.Click += new System.EventHandler(this.OutputDirectoryButton_Click);
            // 
            // FFmpegPathButton
            // 
            this.FFmpegPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FFmpegPathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FFmpegPathButton.Location = new System.Drawing.Point(723, 32);
            this.FFmpegPathButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FFmpegPathButton.Name = "FFmpegPathButton";
            this.FFmpegPathButton.Size = new System.Drawing.Size(51, 25);
            this.FFmpegPathButton.TabIndex = 29;
            this.FFmpegPathButton.Text = "...";
            this.FFmpegPathButton.UseVisualStyleBackColor = true;
            this.FFmpegPathButton.Click += new System.EventHandler(this.FFmpegPathButton_Click);
            // 
            // FFmpegPathTextBox
            // 
            this.FFmpegPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FFmpegPathTextBox.Location = new System.Drawing.Point(91, 33);
            this.FFmpegPathTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FFmpegPathTextBox.Name = "FFmpegPathTextBox";
            this.FFmpegPathTextBox.ReadOnly = true;
            this.FFmpegPathTextBox.Size = new System.Drawing.Size(627, 23);
            this.FFmpegPathTextBox.TabIndex = 22;
            // 
            // OutputDirectoryTextBox
            // 
            this.OutputDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputDirectoryTextBox.Location = new System.Drawing.Point(91, 5);
            this.OutputDirectoryTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.OutputDirectoryTextBox.Name = "OutputDirectoryTextBox";
            this.OutputDirectoryTextBox.ReadOnly = true;
            this.OutputDirectoryTextBox.Size = new System.Drawing.Size(627, 23);
            this.OutputDirectoryTextBox.TabIndex = 24;
            // 
            // OutputDirectoryLabel
            // 
            this.OutputDirectoryLabel.AutoSize = true;
            this.OutputDirectoryLabel.Location = new System.Drawing.Point(-4, 9);
            this.OutputDirectoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.OutputDirectoryLabel.Name = "OutputDirectoryLabel";
            this.OutputDirectoryLabel.Size = new System.Drawing.Size(95, 15);
            this.OutputDirectoryLabel.TabIndex = 21;
            this.OutputDirectoryLabel.Text = "Output directory";
            // 
            // AboutTabPage
            // 
            this.AboutTabPage.Controls.Add(this.LicenseTextBox);
            this.AboutTabPage.Location = new System.Drawing.Point(4, 24);
            this.AboutTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AboutTabPage.Name = "AboutTabPage";
            this.AboutTabPage.Size = new System.Drawing.Size(776, 533);
            this.AboutTabPage.TabIndex = 3;
            this.AboutTabPage.Text = "About";
            this.AboutTabPage.UseVisualStyleBackColor = true;
            // 
            // LicenseTextBox
            // 
            this.LicenseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LicenseTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LicenseTextBox.Location = new System.Drawing.Point(-4, 9);
            this.LicenseTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.LicenseTextBox.Multiline = true;
            this.LicenseTextBox.Name = "LicenseTextBox";
            this.LicenseTextBox.Size = new System.Drawing.Size(778, 522);
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
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
            this.SplitContainer.Panel1.ResumeLayout(false);
            this.SplitContainer.Panel1.PerformLayout();
            this.SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
            this.SplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ChatRoomsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FilesDataGridView)).EndInit();
            this.SettingsTabPage.ResumeLayout(false);
            this.SettingsTabPage.PerformLayout();
            this.AboutTabPage.ResumeLayout(false);
            this.AboutTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage WebBrowserTabPage;
        private System.Windows.Forms.TabPage ChatRoomsTabPage;
        private System.Windows.Forms.TabPage SettingsTabPage;
        private System.Windows.Forms.DataGridView ChatRoomsDataGridView;
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
        private System.Windows.Forms.Timer FormCloseTimer;
        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.DataGridView FilesDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn IndexColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn WebsiteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ActionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ResolutionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileSizeColumn;
    }
}

