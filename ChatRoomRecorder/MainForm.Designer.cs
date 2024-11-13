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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            FilesBindingSource = new System.Windows.Forms.BindingSource(components);
            TabControl = new System.Windows.Forms.TabControl();
            WebBrowserTabPage = new System.Windows.Forms.TabPage();
            WebBrowserTableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            WebBrowserTableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            StopButton = new System.Windows.Forms.Button();
            ForwardButton = new System.Windows.Forms.Button();
            BackButton = new System.Windows.Forms.Button();
            NavigateButton = new System.Windows.Forms.Button();
            AddressBarTextBox = new System.Windows.Forms.TextBox();
            WebView2Panel = new System.Windows.Forms.Panel();
            WebView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ChatRoomsTabPage = new System.Windows.Forms.TabPage();
            ChatRoomsTableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ChatRoomsTableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            UrlTextBox = new System.Windows.Forms.TextBox();
            AddChatRoomFromUrlButton = new System.Windows.Forms.Button();
            AddChatRoomsFromFileButton = new System.Windows.Forms.Button();
            AddChatRoomsFromFolderButton = new System.Windows.Forms.Button();
            ChatRoomsSplitContainer1 = new System.Windows.Forms.SplitContainer();
            ChatRoomsSplitContainer3 = new System.Windows.Forms.SplitContainer();
            CategoriesTreeView = new System.Windows.Forms.TreeView();
            CategoriesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            AddCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            RemoveCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            RenameCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            CategoriesImageList = new System.Windows.Forms.ImageList(components);
            ChatRoomsDataGridView = new System.Windows.Forms.DataGridView();
            WebsiteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ActionColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ResolutionColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            UpdatedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            SeenColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            UrlColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ChatRoomsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            CopyUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            SetActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            SetResolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            RemoveChatRoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ChatRoomsBindingSource = new System.Windows.Forms.BindingSource(components);
            ChatRoomsSplitContainer2 = new System.Windows.Forms.SplitContainer();
            ThumbnailPictureBox = new System.Windows.Forms.PictureBox();
            ThumbnailContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            RemoveThumbnailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            FilesDataGridView = new System.Windows.Forms.DataGridView();
            FileNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            FileSizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            FilesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            OpenFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            RemoveFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            SettingsTabPage = new System.Windows.Forms.TabPage();
            SettingsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            OutputDirectoryTextBox = new System.Windows.Forms.TextBox();
            FFmpegPathTextBox = new System.Windows.Forms.TextBox();
            ChaturbateConcurrentUpdatesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            BongaCamsConcurrentUpdatesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            UpdateIntervalNumericUpDown = new System.Windows.Forms.NumericUpDown();
            OutputDirectoryLabel = new System.Windows.Forms.Label();
            FFmpegPathLabel = new System.Windows.Forms.Label();
            ChaturbateConcurrentUpdatesLabel = new System.Windows.Forms.Label();
            BongaCamsConcurrentUpdatesLabel = new System.Windows.Forms.Label();
            UpdateIntervalLabel = new System.Windows.Forms.Label();
            StripchatConcurrentUpdatesLabel = new System.Windows.Forms.Label();
            StripchatConcurrentUpdatesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            DefaultActionLabel = new System.Windows.Forms.Label();
            DefaultResolutionLabel = new System.Windows.Forms.Label();
            DefaultActionComboBox = new System.Windows.Forms.ComboBox();
            DefaultResolutionComboBox = new System.Windows.Forms.ComboBox();
            StreamlinkPathLabel = new System.Windows.Forms.Label();
            StreamlinkPathTextBox = new System.Windows.Forms.TextBox();
            LogTabPage = new System.Windows.Forms.TabPage();
            LogListBox = new System.Windows.Forms.ListBox();
            LogContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            CopyLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            AboutTabPage = new System.Windows.Forms.TabPage();
            LicenseTextBox = new System.Windows.Forms.TextBox();
            ChatRoomsUpdateTimer = new System.Windows.Forms.Timer(components);
            FormCloseTimer = new System.Windows.Forms.Timer(components);
            SettingsBindingSource = new System.Windows.Forms.BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)FilesBindingSource).BeginInit();
            TabControl.SuspendLayout();
            WebBrowserTabPage.SuspendLayout();
            WebBrowserTableLayoutPanel1.SuspendLayout();
            WebBrowserTableLayoutPanel2.SuspendLayout();
            WebView2Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)WebView2).BeginInit();
            ChatRoomsTabPage.SuspendLayout();
            ChatRoomsTableLayoutPanel1.SuspendLayout();
            ChatRoomsTableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ChatRoomsSplitContainer1).BeginInit();
            ChatRoomsSplitContainer1.Panel1.SuspendLayout();
            ChatRoomsSplitContainer1.Panel2.SuspendLayout();
            ChatRoomsSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ChatRoomsSplitContainer3).BeginInit();
            ChatRoomsSplitContainer3.Panel1.SuspendLayout();
            ChatRoomsSplitContainer3.Panel2.SuspendLayout();
            ChatRoomsSplitContainer3.SuspendLayout();
            CategoriesContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ChatRoomsDataGridView).BeginInit();
            ChatRoomsContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ChatRoomsBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ChatRoomsSplitContainer2).BeginInit();
            ChatRoomsSplitContainer2.Panel1.SuspendLayout();
            ChatRoomsSplitContainer2.Panel2.SuspendLayout();
            ChatRoomsSplitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ThumbnailPictureBox).BeginInit();
            ThumbnailContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FilesDataGridView).BeginInit();
            FilesContextMenuStrip.SuspendLayout();
            SettingsTabPage.SuspendLayout();
            SettingsTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ChaturbateConcurrentUpdatesNumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BongaCamsConcurrentUpdatesNumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)UpdateIntervalNumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StripchatConcurrentUpdatesNumericUpDown).BeginInit();
            LogTabPage.SuspendLayout();
            LogContextMenuStrip.SuspendLayout();
            AboutTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SettingsBindingSource).BeginInit();
            SuspendLayout();
            // 
            // TabControl
            // 
            TabControl.Controls.Add(WebBrowserTabPage);
            TabControl.Controls.Add(ChatRoomsTabPage);
            TabControl.Controls.Add(SettingsTabPage);
            TabControl.Controls.Add(LogTabPage);
            TabControl.Controls.Add(AboutTabPage);
            TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            TabControl.Enabled = false;
            TabControl.Location = new System.Drawing.Point(0, 0);
            TabControl.Margin = new System.Windows.Forms.Padding(0);
            TabControl.Name = "TabControl";
            TabControl.Padding = new System.Drawing.Point(5, 5);
            TabControl.SelectedIndex = 0;
            TabControl.Size = new System.Drawing.Size(784, 561);
            TabControl.TabIndex = 0;
            // 
            // WebBrowserTabPage
            // 
            WebBrowserTabPage.Controls.Add(WebBrowserTableLayoutPanel1);
            WebBrowserTabPage.Location = new System.Drawing.Point(4, 28);
            WebBrowserTabPage.Margin = new System.Windows.Forms.Padding(0);
            WebBrowserTabPage.Name = "WebBrowserTabPage";
            WebBrowserTabPage.Size = new System.Drawing.Size(776, 529);
            WebBrowserTabPage.TabIndex = 0;
            WebBrowserTabPage.Text = "Web browser";
            WebBrowserTabPage.UseVisualStyleBackColor = true;
            // 
            // WebBrowserTableLayoutPanel1
            // 
            WebBrowserTableLayoutPanel1.ColumnCount = 1;
            WebBrowserTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            WebBrowserTableLayoutPanel1.Controls.Add(WebBrowserTableLayoutPanel2, 0, 0);
            WebBrowserTableLayoutPanel1.Controls.Add(WebView2Panel, 0, 1);
            WebBrowserTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            WebBrowserTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            WebBrowserTableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            WebBrowserTableLayoutPanel1.Name = "WebBrowserTableLayoutPanel1";
            WebBrowserTableLayoutPanel1.RowCount = 2;
            WebBrowserTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            WebBrowserTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            WebBrowserTableLayoutPanel1.Size = new System.Drawing.Size(776, 529);
            WebBrowserTableLayoutPanel1.TabIndex = 0;
            // 
            // WebBrowserTableLayoutPanel2
            // 
            WebBrowserTableLayoutPanel2.ColumnCount = 5;
            WebBrowserTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            WebBrowserTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            WebBrowserTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            WebBrowserTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            WebBrowserTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            WebBrowserTableLayoutPanel2.Controls.Add(StopButton, 0, 0);
            WebBrowserTableLayoutPanel2.Controls.Add(ForwardButton, 0, 0);
            WebBrowserTableLayoutPanel2.Controls.Add(BackButton, 0, 0);
            WebBrowserTableLayoutPanel2.Controls.Add(NavigateButton, 3, 0);
            WebBrowserTableLayoutPanel2.Controls.Add(AddressBarTextBox, 4, 0);
            WebBrowserTableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            WebBrowserTableLayoutPanel2.Location = new System.Drawing.Point(0, 5);
            WebBrowserTableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            WebBrowserTableLayoutPanel2.Name = "WebBrowserTableLayoutPanel2";
            WebBrowserTableLayoutPanel2.RowCount = 1;
            WebBrowserTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            WebBrowserTableLayoutPanel2.Size = new System.Drawing.Size(776, 23);
            WebBrowserTableLayoutPanel2.TabIndex = 0;
            // 
            // StopButton
            // 
            StopButton.Dock = System.Windows.Forms.DockStyle.Fill;
            StopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StopButton.Image = Properties.Resources.stop;
            StopButton.Location = new System.Drawing.Point(102, 0);
            StopButton.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            StopButton.Name = "StopButton";
            StopButton.Size = new System.Drawing.Size(46, 23);
            StopButton.TabIndex = 3;
            StopButton.UseVisualStyleBackColor = true;
            StopButton.Click += StopButton_Click;
            // 
            // ForwardButton
            // 
            ForwardButton.Dock = System.Windows.Forms.DockStyle.Fill;
            ForwardButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ForwardButton.Image = Properties.Resources.forward;
            ForwardButton.Location = new System.Drawing.Point(52, 0);
            ForwardButton.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            ForwardButton.Name = "ForwardButton";
            ForwardButton.Size = new System.Drawing.Size(46, 23);
            ForwardButton.TabIndex = 2;
            ForwardButton.UseVisualStyleBackColor = true;
            ForwardButton.Click += ForwardButton_Click;
            // 
            // BackButton
            // 
            BackButton.Dock = System.Windows.Forms.DockStyle.Fill;
            BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BackButton.Image = Properties.Resources.back;
            BackButton.Location = new System.Drawing.Point(0, 0);
            BackButton.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            BackButton.Name = "BackButton";
            BackButton.Size = new System.Drawing.Size(48, 23);
            BackButton.TabIndex = 1;
            BackButton.UseVisualStyleBackColor = true;
            BackButton.Click += BackButton_Click;
            // 
            // NavigateButton
            // 
            NavigateButton.Dock = System.Windows.Forms.DockStyle.Fill;
            NavigateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NavigateButton.Image = Properties.Resources.enter;
            NavigateButton.Location = new System.Drawing.Point(152, 0);
            NavigateButton.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            NavigateButton.Name = "NavigateButton";
            NavigateButton.Size = new System.Drawing.Size(46, 23);
            NavigateButton.TabIndex = 4;
            NavigateButton.UseVisualStyleBackColor = true;
            NavigateButton.Click += NavigateButton_Click;
            // 
            // AddressBarTextBox
            // 
            AddressBarTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            AddressBarTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            AddressBarTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            AddressBarTextBox.Location = new System.Drawing.Point(202, 0);
            AddressBarTextBox.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            AddressBarTextBox.Name = "AddressBarTextBox";
            AddressBarTextBox.Size = new System.Drawing.Size(574, 23);
            AddressBarTextBox.TabIndex = 5;
            AddressBarTextBox.KeyDown += AddressBarTextBox_KeyDown;
            // 
            // WebView2Panel
            // 
            WebView2Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            WebView2Panel.Controls.Add(WebView2);
            WebView2Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            WebView2Panel.Location = new System.Drawing.Point(0, 33);
            WebView2Panel.Margin = new System.Windows.Forms.Padding(0);
            WebView2Panel.Name = "WebView2Panel";
            WebView2Panel.Size = new System.Drawing.Size(776, 496);
            WebView2Panel.TabIndex = 1;
            // 
            // WebView2
            // 
            WebView2.AllowExternalDrop = true;
            WebView2.CreationProperties = null;
            WebView2.DefaultBackgroundColor = System.Drawing.Color.White;
            WebView2.Dock = System.Windows.Forms.DockStyle.Fill;
            WebView2.Location = new System.Drawing.Point(0, 0);
            WebView2.Margin = new System.Windows.Forms.Padding(0);
            WebView2.Name = "WebView2";
            WebView2.Size = new System.Drawing.Size(774, 494);
            WebView2.Source = new System.Uri("about:blank", System.UriKind.Absolute);
            WebView2.TabIndex = 6;
            WebView2.ZoomFactor = 1D;
            WebView2.CoreWebView2InitializationCompleted += WebView2_CoreWebView2InitializationCompleted;
            WebView2.ContentLoading += WebView2_ContentLoading;
            // 
            // ChatRoomsTabPage
            // 
            ChatRoomsTabPage.Controls.Add(ChatRoomsTableLayoutPanel1);
            ChatRoomsTabPage.Location = new System.Drawing.Point(4, 28);
            ChatRoomsTabPage.Margin = new System.Windows.Forms.Padding(0);
            ChatRoomsTabPage.Name = "ChatRoomsTabPage";
            ChatRoomsTabPage.Size = new System.Drawing.Size(776, 529);
            ChatRoomsTabPage.TabIndex = 1;
            ChatRoomsTabPage.Text = "Chat rooms";
            ChatRoomsTabPage.UseVisualStyleBackColor = true;
            // 
            // ChatRoomsTableLayoutPanel1
            // 
            ChatRoomsTableLayoutPanel1.ColumnCount = 1;
            ChatRoomsTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            ChatRoomsTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            ChatRoomsTableLayoutPanel1.Controls.Add(ChatRoomsTableLayoutPanel2, 0, 0);
            ChatRoomsTableLayoutPanel1.Controls.Add(ChatRoomsSplitContainer1, 0, 1);
            ChatRoomsTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            ChatRoomsTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            ChatRoomsTableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            ChatRoomsTableLayoutPanel1.Name = "ChatRoomsTableLayoutPanel1";
            ChatRoomsTableLayoutPanel1.RowCount = 2;
            ChatRoomsTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            ChatRoomsTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            ChatRoomsTableLayoutPanel1.Size = new System.Drawing.Size(776, 529);
            ChatRoomsTableLayoutPanel1.TabIndex = 13;
            // 
            // ChatRoomsTableLayoutPanel2
            // 
            ChatRoomsTableLayoutPanel2.ColumnCount = 4;
            ChatRoomsTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            ChatRoomsTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            ChatRoomsTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            ChatRoomsTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            ChatRoomsTableLayoutPanel2.Controls.Add(UrlTextBox, 3, 0);
            ChatRoomsTableLayoutPanel2.Controls.Add(AddChatRoomFromUrlButton, 0, 0);
            ChatRoomsTableLayoutPanel2.Controls.Add(AddChatRoomsFromFileButton, 1, 0);
            ChatRoomsTableLayoutPanel2.Controls.Add(AddChatRoomsFromFolderButton, 2, 0);
            ChatRoomsTableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            ChatRoomsTableLayoutPanel2.Location = new System.Drawing.Point(0, 5);
            ChatRoomsTableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            ChatRoomsTableLayoutPanel2.Name = "ChatRoomsTableLayoutPanel2";
            ChatRoomsTableLayoutPanel2.RowCount = 1;
            ChatRoomsTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            ChatRoomsTableLayoutPanel2.Size = new System.Drawing.Size(776, 23);
            ChatRoomsTableLayoutPanel2.TabIndex = 0;
            // 
            // UrlTextBox
            // 
            UrlTextBox.BackColor = System.Drawing.SystemColors.Window;
            UrlTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            UrlTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            UrlTextBox.Location = new System.Drawing.Point(152, 0);
            UrlTextBox.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            UrlTextBox.Name = "UrlTextBox";
            UrlTextBox.Size = new System.Drawing.Size(624, 23);
            UrlTextBox.TabIndex = 4;
            UrlTextBox.WordWrap = false;
            UrlTextBox.TextChanged += UrlTextBox_TextChanged;
            UrlTextBox.KeyDown += UrlTextBox_KeyDown;
            // 
            // AddChatRoomFromUrlButton
            // 
            AddChatRoomFromUrlButton.Dock = System.Windows.Forms.DockStyle.Fill;
            AddChatRoomFromUrlButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            AddChatRoomFromUrlButton.Image = Properties.Resources.url;
            AddChatRoomFromUrlButton.Location = new System.Drawing.Point(0, 0);
            AddChatRoomFromUrlButton.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            AddChatRoomFromUrlButton.Name = "AddChatRoomFromUrlButton";
            AddChatRoomFromUrlButton.Size = new System.Drawing.Size(48, 23);
            AddChatRoomFromUrlButton.TabIndex = 1;
            AddChatRoomFromUrlButton.UseVisualStyleBackColor = true;
            AddChatRoomFromUrlButton.Click += AddChatRoomFromUrlButton_Click;
            // 
            // AddChatRoomsFromFileButton
            // 
            AddChatRoomsFromFileButton.Dock = System.Windows.Forms.DockStyle.Fill;
            AddChatRoomsFromFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            AddChatRoomsFromFileButton.Image = Properties.Resources.file;
            AddChatRoomsFromFileButton.Location = new System.Drawing.Point(52, 0);
            AddChatRoomsFromFileButton.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            AddChatRoomsFromFileButton.Name = "AddChatRoomsFromFileButton";
            AddChatRoomsFromFileButton.Size = new System.Drawing.Size(46, 23);
            AddChatRoomsFromFileButton.TabIndex = 2;
            AddChatRoomsFromFileButton.UseVisualStyleBackColor = true;
            AddChatRoomsFromFileButton.Click += AddChatRoomsFromFileButton_Click;
            // 
            // AddChatRoomsFromFolderButton
            // 
            AddChatRoomsFromFolderButton.Dock = System.Windows.Forms.DockStyle.Fill;
            AddChatRoomsFromFolderButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            AddChatRoomsFromFolderButton.Image = Properties.Resources.folder;
            AddChatRoomsFromFolderButton.Location = new System.Drawing.Point(102, 0);
            AddChatRoomsFromFolderButton.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            AddChatRoomsFromFolderButton.Name = "AddChatRoomsFromFolderButton";
            AddChatRoomsFromFolderButton.Size = new System.Drawing.Size(46, 23);
            AddChatRoomsFromFolderButton.TabIndex = 3;
            AddChatRoomsFromFolderButton.UseVisualStyleBackColor = true;
            AddChatRoomsFromFolderButton.Click += AddChatRoomsFromFolderButton_Click;
            // 
            // ChatRoomsSplitContainer1
            // 
            ChatRoomsSplitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ChatRoomsSplitContainer1.BackColor = System.Drawing.Color.Transparent;
            ChatRoomsSplitContainer1.Location = new System.Drawing.Point(0, 33);
            ChatRoomsSplitContainer1.Margin = new System.Windows.Forms.Padding(0);
            ChatRoomsSplitContainer1.Name = "ChatRoomsSplitContainer1";
            ChatRoomsSplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ChatRoomsSplitContainer1.Panel1
            // 
            ChatRoomsSplitContainer1.Panel1.BackColor = System.Drawing.Color.Transparent;
            ChatRoomsSplitContainer1.Panel1.Controls.Add(ChatRoomsSplitContainer3);
            // 
            // ChatRoomsSplitContainer1.Panel2
            // 
            ChatRoomsSplitContainer1.Panel2.BackColor = System.Drawing.Color.Transparent;
            ChatRoomsSplitContainer1.Panel2.Controls.Add(ChatRoomsSplitContainer2);
            ChatRoomsSplitContainer1.Size = new System.Drawing.Size(776, 496);
            ChatRoomsSplitContainer1.SplitterDistance = 351;
            ChatRoomsSplitContainer1.TabIndex = 12;
            ChatRoomsSplitContainer1.TabStop = false;
            // 
            // ChatRoomsSplitContainer3
            // 
            ChatRoomsSplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            ChatRoomsSplitContainer3.Location = new System.Drawing.Point(0, 0);
            ChatRoomsSplitContainer3.Margin = new System.Windows.Forms.Padding(0);
            ChatRoomsSplitContainer3.Name = "ChatRoomsSplitContainer3";
            // 
            // ChatRoomsSplitContainer3.Panel1
            // 
            ChatRoomsSplitContainer3.Panel1.Controls.Add(CategoriesTreeView);
            // 
            // ChatRoomsSplitContainer3.Panel2
            // 
            ChatRoomsSplitContainer3.Panel2.Controls.Add(ChatRoomsDataGridView);
            ChatRoomsSplitContainer3.Size = new System.Drawing.Size(776, 351);
            ChatRoomsSplitContainer3.SplitterDistance = 125;
            ChatRoomsSplitContainer3.TabIndex = 12;
            ChatRoomsSplitContainer3.TabStop = false;
            // 
            // CategoriesTreeView
            // 
            CategoriesTreeView.AllowDrop = true;
            CategoriesTreeView.ContextMenuStrip = CategoriesContextMenuStrip;
            CategoriesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            CategoriesTreeView.ForeColor = System.Drawing.SystemColors.WindowText;
            CategoriesTreeView.FullRowSelect = true;
            CategoriesTreeView.HideSelection = false;
            CategoriesTreeView.HotTracking = true;
            CategoriesTreeView.ImageIndex = 0;
            CategoriesTreeView.ImageList = CategoriesImageList;
            CategoriesTreeView.LabelEdit = true;
            CategoriesTreeView.Location = new System.Drawing.Point(0, 0);
            CategoriesTreeView.Margin = new System.Windows.Forms.Padding(0);
            CategoriesTreeView.Name = "CategoriesTreeView";
            CategoriesTreeView.SelectedImageIndex = 1;
            CategoriesTreeView.Size = new System.Drawing.Size(125, 351);
            CategoriesTreeView.TabIndex = 5;
            CategoriesTreeView.BeforeLabelEdit += CategoriesTreeView_BeforeLabelEdit;
            CategoriesTreeView.AfterLabelEdit += CategoriesTreeView_AfterLabelEdit;
            CategoriesTreeView.AfterCollapse += CategoriesTreeView_AfterCollapse;
            CategoriesTreeView.AfterExpand += CategoriesTreeView_AfterExpand;
            CategoriesTreeView.AfterSelect += CategoriesTreeView_AfterSelect;
            CategoriesTreeView.DragDrop += CategoriesTreeView_DragDrop;
            CategoriesTreeView.DragOver += CategoriesTreeView_DragOver;
            CategoriesTreeView.QueryContinueDrag += CategoriesTreeView_QueryContinueDrag;
            CategoriesTreeView.KeyDown += CategoriesTreeView_KeyDown;
            CategoriesTreeView.MouseDown += CategoriesTreeView_MouseDown;
            CategoriesTreeView.MouseMove += CategoriesTreeView_MouseMove;
            CategoriesTreeView.MouseUp += CategoriesTreeView_MouseUp;
            // 
            // CategoriesContextMenuStrip
            // 
            CategoriesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { AddCategoryToolStripMenuItem, RemoveCategoryToolStripMenuItem, RenameCategoryToolStripMenuItem });
            CategoriesContextMenuStrip.Name = "CategoriesContextMenuStrip";
            CategoriesContextMenuStrip.Size = new System.Drawing.Size(118, 70);
            CategoriesContextMenuStrip.Opening += CategoriesContextMenuStrip_Opening;
            // 
            // AddCategoryToolStripMenuItem
            // 
            AddCategoryToolStripMenuItem.Name = "AddCategoryToolStripMenuItem";
            AddCategoryToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            AddCategoryToolStripMenuItem.Text = "Add";
            AddCategoryToolStripMenuItem.Click += AddCategoryToolStripMenuItem_Click;
            // 
            // RemoveCategoryToolStripMenuItem
            // 
            RemoveCategoryToolStripMenuItem.Name = "RemoveCategoryToolStripMenuItem";
            RemoveCategoryToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            RemoveCategoryToolStripMenuItem.Text = "Remove";
            RemoveCategoryToolStripMenuItem.Click += RemoveCategoryToolStripMenuItem_Click;
            // 
            // RenameCategoryToolStripMenuItem
            // 
            RenameCategoryToolStripMenuItem.Name = "RenameCategoryToolStripMenuItem";
            RenameCategoryToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            RenameCategoryToolStripMenuItem.Text = "Rename";
            RenameCategoryToolStripMenuItem.Click += RenameCategoryToolStripMenuItem_Click;
            // 
            // CategoriesImageList
            // 
            CategoriesImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            CategoriesImageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("CategoriesImageList.ImageStream");
            CategoriesImageList.TransparentColor = System.Drawing.Color.Transparent;
            CategoriesImageList.Images.SetKeyName(0, "collapsed");
            CategoriesImageList.Images.SetKeyName(1, "expanded");
            // 
            // ChatRoomsDataGridView
            // 
            ChatRoomsDataGridView.AllowUserToAddRows = false;
            ChatRoomsDataGridView.AllowUserToDeleteRows = false;
            ChatRoomsDataGridView.AutoGenerateColumns = false;
            ChatRoomsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            ChatRoomsDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            ChatRoomsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            ChatRoomsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            ChatRoomsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            ChatRoomsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { WebsiteColumn, NameColumn, ActionColumn, ResolutionColumn, StatusColumn, UpdatedColumn, SeenColumn, UrlColumn });
            ChatRoomsDataGridView.ContextMenuStrip = ChatRoomsContextMenuStrip;
            ChatRoomsDataGridView.DataSource = ChatRoomsBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            ChatRoomsDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            ChatRoomsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            ChatRoomsDataGridView.Location = new System.Drawing.Point(0, 0);
            ChatRoomsDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ChatRoomsDataGridView.Name = "ChatRoomsDataGridView";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            ChatRoomsDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            ChatRoomsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            ChatRoomsDataGridView.Size = new System.Drawing.Size(647, 351);
            ChatRoomsDataGridView.TabIndex = 6;
            ChatRoomsDataGridView.CellBeginEdit += ChatRoomsDataGridView_CellBeginEdit;
            ChatRoomsDataGridView.CellEndEdit += ChatRoomsDataGridView_CellEndEdit;
            ChatRoomsDataGridView.CellLeave += ChatRoomsDataGridView_CellLeave;
            ChatRoomsDataGridView.CellMouseDown += ChatRoomsDataGridView_CellMouseDown;
            ChatRoomsDataGridView.CellValueChanged += ChatRoomsDataGridView_CellValueChanged;
            ChatRoomsDataGridView.SelectionChanged += ChatRoomsDataGridView_SelectionChanged;
            // 
            // WebsiteColumn
            // 
            WebsiteColumn.DataPropertyName = "Website";
            WebsiteColumn.HeaderText = "Website";
            WebsiteColumn.Name = "WebsiteColumn";
            WebsiteColumn.ReadOnly = true;
            // 
            // NameColumn
            // 
            NameColumn.DataPropertyName = "Name";
            NameColumn.HeaderText = "Name";
            NameColumn.Name = "NameColumn";
            NameColumn.ReadOnly = true;
            // 
            // ActionColumn
            // 
            ActionColumn.DataPropertyName = "Action";
            ActionColumn.HeaderText = "Action";
            ActionColumn.Name = "ActionColumn";
            ActionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ResolutionColumn
            // 
            ResolutionColumn.DataPropertyName = "PreferredResolution";
            ResolutionColumn.HeaderText = "Resolution";
            ResolutionColumn.Name = "ResolutionColumn";
            ResolutionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // StatusColumn
            // 
            StatusColumn.DataPropertyName = "Status";
            StatusColumn.HeaderText = "Status";
            StatusColumn.Name = "StatusColumn";
            StatusColumn.ReadOnly = true;
            // 
            // UpdatedColumn
            // 
            UpdatedColumn.DataPropertyName = "LastUpdated";
            UpdatedColumn.HeaderText = "Updated";
            UpdatedColumn.Name = "UpdatedColumn";
            UpdatedColumn.ReadOnly = true;
            // 
            // SeenColumn
            // 
            SeenColumn.DataPropertyName = "LastSeen";
            SeenColumn.HeaderText = "Seen";
            SeenColumn.Name = "SeenColumn";
            SeenColumn.ReadOnly = true;
            // 
            // UrlColumn
            // 
            UrlColumn.DataPropertyName = "ChatRoomUrl";
            UrlColumn.HeaderText = "Url";
            UrlColumn.Name = "UrlColumn";
            UrlColumn.ReadOnly = true;
            // 
            // ChatRoomsContextMenuStrip
            // 
            ChatRoomsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { CopyUrlToolStripMenuItem, SetActionToolStripMenuItem, SetResolutionToolStripMenuItem, RemoveChatRoomToolStripMenuItem });
            ChatRoomsContextMenuStrip.Name = "ChatRoomsContextMenuStrip";
            ChatRoomsContextMenuStrip.Size = new System.Drawing.Size(147, 92);
            ChatRoomsContextMenuStrip.Opening += ChatRoomsContextMenuStrip_Opening;
            // 
            // CopyUrlToolStripMenuItem
            // 
            CopyUrlToolStripMenuItem.Name = "CopyUrlToolStripMenuItem";
            CopyUrlToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            CopyUrlToolStripMenuItem.Text = "Copy URL";
            CopyUrlToolStripMenuItem.Click += CopyUrlToolStripMenuItem_Click;
            // 
            // SetActionToolStripMenuItem
            // 
            SetActionToolStripMenuItem.Name = "SetActionToolStripMenuItem";
            SetActionToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            SetActionToolStripMenuItem.Text = "Set action";
            // 
            // SetResolutionToolStripMenuItem
            // 
            SetResolutionToolStripMenuItem.Name = "SetResolutionToolStripMenuItem";
            SetResolutionToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            SetResolutionToolStripMenuItem.Text = "Set resolution";
            // 
            // RemoveChatRoomToolStripMenuItem
            // 
            RemoveChatRoomToolStripMenuItem.Name = "RemoveChatRoomToolStripMenuItem";
            RemoveChatRoomToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            RemoveChatRoomToolStripMenuItem.Text = "Remove";
            RemoveChatRoomToolStripMenuItem.Click += RemoveChatRoomToolStripMenuItem_Click;
            // 
            // ChatRoomsSplitContainer2
            // 
            ChatRoomsSplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            ChatRoomsSplitContainer2.Location = new System.Drawing.Point(0, 0);
            ChatRoomsSplitContainer2.Margin = new System.Windows.Forms.Padding(0);
            ChatRoomsSplitContainer2.Name = "ChatRoomsSplitContainer2";
            // 
            // ChatRoomsSplitContainer2.Panel1
            // 
            ChatRoomsSplitContainer2.Panel1.Controls.Add(ThumbnailPictureBox);
            // 
            // ChatRoomsSplitContainer2.Panel2
            // 
            ChatRoomsSplitContainer2.Panel2.Controls.Add(FilesDataGridView);
            ChatRoomsSplitContainer2.Size = new System.Drawing.Size(776, 141);
            ChatRoomsSplitContainer2.SplitterDistance = 125;
            ChatRoomsSplitContainer2.TabIndex = 1;
            ChatRoomsSplitContainer2.TabStop = false;
            // 
            // ThumbnailPictureBox
            // 
            ThumbnailPictureBox.BackColor = System.Drawing.SystemColors.Window;
            ThumbnailPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ThumbnailPictureBox.ContextMenuStrip = ThumbnailContextMenuStrip;
            ThumbnailPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            ThumbnailPictureBox.Location = new System.Drawing.Point(0, 0);
            ThumbnailPictureBox.Margin = new System.Windows.Forms.Padding(0);
            ThumbnailPictureBox.Name = "ThumbnailPictureBox";
            ThumbnailPictureBox.Size = new System.Drawing.Size(125, 141);
            ThumbnailPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            ThumbnailPictureBox.TabIndex = 0;
            ThumbnailPictureBox.TabStop = false;
            // 
            // ThumbnailContextMenuStrip
            // 
            ThumbnailContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { RemoveThumbnailToolStripMenuItem });
            ThumbnailContextMenuStrip.Name = "ThumbnailContextMenuStrip";
            ThumbnailContextMenuStrip.Size = new System.Drawing.Size(118, 26);
            ThumbnailContextMenuStrip.Opening += ThumbnailContextMenuStrip_Opening;
            // 
            // RemoveThumbnailToolStripMenuItem
            // 
            RemoveThumbnailToolStripMenuItem.Name = "RemoveThumbnailToolStripMenuItem";
            RemoveThumbnailToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            RemoveThumbnailToolStripMenuItem.Text = "Remove";
            RemoveThumbnailToolStripMenuItem.Click += RemoveThumbnailToolStripMenuItem_Click;
            // 
            // FilesDataGridView
            // 
            FilesDataGridView.AllowUserToAddRows = false;
            FilesDataGridView.AllowUserToDeleteRows = false;
            FilesDataGridView.AutoGenerateColumns = false;
            FilesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            FilesDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            FilesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            FilesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            FilesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { FileNameColumn, FileSizeColumn });
            FilesDataGridView.ContextMenuStrip = FilesContextMenuStrip;
            FilesDataGridView.DataSource = FilesBindingSource;
            FilesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            FilesDataGridView.Location = new System.Drawing.Point(0, 0);
            FilesDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FilesDataGridView.Name = "FilesDataGridView";
            FilesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            FilesDataGridView.Size = new System.Drawing.Size(647, 141);
            FilesDataGridView.TabIndex = 7;
            FilesDataGridView.CellMouseDoubleClick += FilesDataGridView_CellMouseDoubleClick;
            FilesDataGridView.KeyDown += FilesDataGridView_KeyDown;
            // 
            // FileNameColumn
            // 
            FileNameColumn.DataPropertyName = "Name";
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FileNameColumn.DefaultCellStyle = dataGridViewCellStyle4;
            FileNameColumn.FillWeight = 199.8681F;
            FileNameColumn.HeaderText = "File name";
            FileNameColumn.Name = "FileNameColumn";
            // 
            // FileSizeColumn
            // 
            FileSizeColumn.DataPropertyName = "Length";
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.Format = "N0";
            dataGridViewCellStyle5.NullValue = null;
            FileSizeColumn.DefaultCellStyle = dataGridViewCellStyle5;
            FileSizeColumn.FillWeight = 50.25381F;
            FileSizeColumn.HeaderText = "File size";
            FileSizeColumn.Name = "FileSizeColumn";
            // 
            // FilesContextMenuStrip
            // 
            FilesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { OpenFileToolStripMenuItem, RemoveFileToolStripMenuItem });
            FilesContextMenuStrip.Name = "FilesContextMenuStrip";
            FilesContextMenuStrip.Size = new System.Drawing.Size(118, 48);
            FilesContextMenuStrip.Opening += FilesContextMenuStrip_Opening;
            // 
            // OpenFileToolStripMenuItem
            // 
            OpenFileToolStripMenuItem.Name = "OpenFileToolStripMenuItem";
            OpenFileToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            OpenFileToolStripMenuItem.Text = "Open";
            OpenFileToolStripMenuItem.Click += OpenFileToolStripMenuItem_Click;
            // 
            // RemoveFileToolStripMenuItem
            // 
            RemoveFileToolStripMenuItem.Name = "RemoveFileToolStripMenuItem";
            RemoveFileToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            RemoveFileToolStripMenuItem.Text = "Remove";
            RemoveFileToolStripMenuItem.Click += RemoveFileToolStripMenuItem_Click;
            // 
            // SettingsTabPage
            // 
            SettingsTabPage.Controls.Add(SettingsTableLayoutPanel);
            SettingsTabPage.Location = new System.Drawing.Point(4, 28);
            SettingsTabPage.Margin = new System.Windows.Forms.Padding(0);
            SettingsTabPage.Name = "SettingsTabPage";
            SettingsTabPage.Size = new System.Drawing.Size(776, 529);
            SettingsTabPage.TabIndex = 2;
            SettingsTabPage.Text = "Settings";
            SettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // SettingsTableLayoutPanel
            // 
            SettingsTableLayoutPanel.ColumnCount = 4;
            SettingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            SettingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            SettingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            SettingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            SettingsTableLayoutPanel.Controls.Add(OutputDirectoryTextBox, 2, 0);
            SettingsTableLayoutPanel.Controls.Add(FFmpegPathTextBox, 2, 1);
            SettingsTableLayoutPanel.Controls.Add(ChaturbateConcurrentUpdatesNumericUpDown, 2, 3);
            SettingsTableLayoutPanel.Controls.Add(BongaCamsConcurrentUpdatesNumericUpDown, 2, 4);
            SettingsTableLayoutPanel.Controls.Add(UpdateIntervalNumericUpDown, 2, 6);
            SettingsTableLayoutPanel.Controls.Add(OutputDirectoryLabel, 1, 0);
            SettingsTableLayoutPanel.Controls.Add(FFmpegPathLabel, 1, 1);
            SettingsTableLayoutPanel.Controls.Add(ChaturbateConcurrentUpdatesLabel, 1, 3);
            SettingsTableLayoutPanel.Controls.Add(BongaCamsConcurrentUpdatesLabel, 1, 4);
            SettingsTableLayoutPanel.Controls.Add(UpdateIntervalLabel, 1, 6);
            SettingsTableLayoutPanel.Controls.Add(StripchatConcurrentUpdatesLabel, 1, 5);
            SettingsTableLayoutPanel.Controls.Add(StripchatConcurrentUpdatesNumericUpDown, 2, 5);
            SettingsTableLayoutPanel.Controls.Add(DefaultActionLabel, 1, 7);
            SettingsTableLayoutPanel.Controls.Add(DefaultResolutionLabel, 1, 8);
            SettingsTableLayoutPanel.Controls.Add(DefaultActionComboBox, 2, 7);
            SettingsTableLayoutPanel.Controls.Add(DefaultResolutionComboBox, 2, 8);
            SettingsTableLayoutPanel.Controls.Add(StreamlinkPathLabel, 1, 2);
            SettingsTableLayoutPanel.Controls.Add(StreamlinkPathTextBox, 2, 2);
            SettingsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            SettingsTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            SettingsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            SettingsTableLayoutPanel.Name = "SettingsTableLayoutPanel";
            SettingsTableLayoutPanel.RowCount = 10;
            SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            SettingsTableLayoutPanel.Size = new System.Drawing.Size(776, 529);
            SettingsTableLayoutPanel.TabIndex = 39;
            // 
            // OutputDirectoryTextBox
            // 
            OutputDirectoryTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            OutputDirectoryTextBox.BackColor = System.Drawing.SystemColors.Window;
            OutputDirectoryTextBox.Location = new System.Drawing.Point(388, 5);
            OutputDirectoryTextBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            OutputDirectoryTextBox.Name = "OutputDirectoryTextBox";
            OutputDirectoryTextBox.ReadOnly = true;
            OutputDirectoryTextBox.Size = new System.Drawing.Size(350, 23);
            OutputDirectoryTextBox.TabIndex = 1;
            OutputDirectoryTextBox.Click += OutputDirectoryTextBox_Click;
            // 
            // FFmpegPathTextBox
            // 
            FFmpegPathTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FFmpegPathTextBox.BackColor = System.Drawing.SystemColors.Window;
            FFmpegPathTextBox.Location = new System.Drawing.Point(388, 34);
            FFmpegPathTextBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            FFmpegPathTextBox.Name = "FFmpegPathTextBox";
            FFmpegPathTextBox.ReadOnly = true;
            FFmpegPathTextBox.Size = new System.Drawing.Size(350, 23);
            FFmpegPathTextBox.TabIndex = 2;
            FFmpegPathTextBox.Click += FFmpegPathTextBox_Click;
            // 
            // ChaturbateConcurrentUpdatesNumericUpDown
            // 
            ChaturbateConcurrentUpdatesNumericUpDown.BackColor = System.Drawing.SystemColors.Window;
            ChaturbateConcurrentUpdatesNumericUpDown.Dock = System.Windows.Forms.DockStyle.Top;
            ChaturbateConcurrentUpdatesNumericUpDown.Location = new System.Drawing.Point(388, 92);
            ChaturbateConcurrentUpdatesNumericUpDown.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            ChaturbateConcurrentUpdatesNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            ChaturbateConcurrentUpdatesNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            ChaturbateConcurrentUpdatesNumericUpDown.Name = "ChaturbateConcurrentUpdatesNumericUpDown";
            ChaturbateConcurrentUpdatesNumericUpDown.Size = new System.Drawing.Size(350, 23);
            ChaturbateConcurrentUpdatesNumericUpDown.TabIndex = 4;
            ChaturbateConcurrentUpdatesNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            ChaturbateConcurrentUpdatesNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // BongaCamsConcurrentUpdatesNumericUpDown
            // 
            BongaCamsConcurrentUpdatesNumericUpDown.BackColor = System.Drawing.SystemColors.Window;
            BongaCamsConcurrentUpdatesNumericUpDown.Dock = System.Windows.Forms.DockStyle.Top;
            BongaCamsConcurrentUpdatesNumericUpDown.Location = new System.Drawing.Point(388, 121);
            BongaCamsConcurrentUpdatesNumericUpDown.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            BongaCamsConcurrentUpdatesNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            BongaCamsConcurrentUpdatesNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            BongaCamsConcurrentUpdatesNumericUpDown.Name = "BongaCamsConcurrentUpdatesNumericUpDown";
            BongaCamsConcurrentUpdatesNumericUpDown.Size = new System.Drawing.Size(350, 23);
            BongaCamsConcurrentUpdatesNumericUpDown.TabIndex = 5;
            BongaCamsConcurrentUpdatesNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            BongaCamsConcurrentUpdatesNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // UpdateIntervalNumericUpDown
            // 
            UpdateIntervalNumericUpDown.BackColor = System.Drawing.SystemColors.Window;
            UpdateIntervalNumericUpDown.Dock = System.Windows.Forms.DockStyle.Top;
            UpdateIntervalNumericUpDown.Location = new System.Drawing.Point(388, 179);
            UpdateIntervalNumericUpDown.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            UpdateIntervalNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            UpdateIntervalNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            UpdateIntervalNumericUpDown.Name = "UpdateIntervalNumericUpDown";
            UpdateIntervalNumericUpDown.Size = new System.Drawing.Size(350, 23);
            UpdateIntervalNumericUpDown.TabIndex = 7;
            UpdateIntervalNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            UpdateIntervalNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // OutputDirectoryLabel
            // 
            OutputDirectoryLabel.AutoSize = true;
            OutputDirectoryLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            OutputDirectoryLabel.Location = new System.Drawing.Point(38, 5);
            OutputDirectoryLabel.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            OutputDirectoryLabel.Name = "OutputDirectoryLabel";
            OutputDirectoryLabel.Size = new System.Drawing.Size(350, 24);
            OutputDirectoryLabel.TabIndex = 21;
            OutputDirectoryLabel.Text = "Output directory";
            // 
            // FFmpegPathLabel
            // 
            FFmpegPathLabel.AutoSize = true;
            FFmpegPathLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            FFmpegPathLabel.Location = new System.Drawing.Point(38, 34);
            FFmpegPathLabel.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            FFmpegPathLabel.Name = "FFmpegPathLabel";
            FFmpegPathLabel.Size = new System.Drawing.Size(350, 24);
            FFmpegPathLabel.TabIndex = 32;
            FFmpegPathLabel.Text = "FFmpeg path";
            // 
            // ChaturbateConcurrentUpdatesLabel
            // 
            ChaturbateConcurrentUpdatesLabel.AutoSize = true;
            ChaturbateConcurrentUpdatesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            ChaturbateConcurrentUpdatesLabel.Location = new System.Drawing.Point(38, 92);
            ChaturbateConcurrentUpdatesLabel.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            ChaturbateConcurrentUpdatesLabel.Name = "ChaturbateConcurrentUpdatesLabel";
            ChaturbateConcurrentUpdatesLabel.Size = new System.Drawing.Size(350, 24);
            ChaturbateConcurrentUpdatesLabel.TabIndex = 33;
            ChaturbateConcurrentUpdatesLabel.Text = "Chaturbate concurrent updates";
            // 
            // BongaCamsConcurrentUpdatesLabel
            // 
            BongaCamsConcurrentUpdatesLabel.AutoSize = true;
            BongaCamsConcurrentUpdatesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            BongaCamsConcurrentUpdatesLabel.Location = new System.Drawing.Point(38, 121);
            BongaCamsConcurrentUpdatesLabel.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            BongaCamsConcurrentUpdatesLabel.Name = "BongaCamsConcurrentUpdatesLabel";
            BongaCamsConcurrentUpdatesLabel.Size = new System.Drawing.Size(350, 24);
            BongaCamsConcurrentUpdatesLabel.TabIndex = 34;
            BongaCamsConcurrentUpdatesLabel.Text = "BongaCams concurrent updates";
            // 
            // UpdateIntervalLabel
            // 
            UpdateIntervalLabel.AutoSize = true;
            UpdateIntervalLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            UpdateIntervalLabel.Location = new System.Drawing.Point(38, 179);
            UpdateIntervalLabel.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            UpdateIntervalLabel.Name = "UpdateIntervalLabel";
            UpdateIntervalLabel.Size = new System.Drawing.Size(350, 24);
            UpdateIntervalLabel.TabIndex = 37;
            UpdateIntervalLabel.Text = "Update interval";
            // 
            // StripchatConcurrentUpdatesLabel
            // 
            StripchatConcurrentUpdatesLabel.AutoSize = true;
            StripchatConcurrentUpdatesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            StripchatConcurrentUpdatesLabel.Location = new System.Drawing.Point(38, 150);
            StripchatConcurrentUpdatesLabel.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            StripchatConcurrentUpdatesLabel.Name = "StripchatConcurrentUpdatesLabel";
            StripchatConcurrentUpdatesLabel.Size = new System.Drawing.Size(350, 24);
            StripchatConcurrentUpdatesLabel.TabIndex = 39;
            StripchatConcurrentUpdatesLabel.Text = "Stripchat concurrent updates";
            // 
            // StripchatConcurrentUpdatesNumericUpDown
            // 
            StripchatConcurrentUpdatesNumericUpDown.BackColor = System.Drawing.SystemColors.Window;
            StripchatConcurrentUpdatesNumericUpDown.Dock = System.Windows.Forms.DockStyle.Top;
            StripchatConcurrentUpdatesNumericUpDown.Location = new System.Drawing.Point(388, 150);
            StripchatConcurrentUpdatesNumericUpDown.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            StripchatConcurrentUpdatesNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            StripchatConcurrentUpdatesNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            StripchatConcurrentUpdatesNumericUpDown.Name = "StripchatConcurrentUpdatesNumericUpDown";
            StripchatConcurrentUpdatesNumericUpDown.Size = new System.Drawing.Size(350, 23);
            StripchatConcurrentUpdatesNumericUpDown.TabIndex = 6;
            StripchatConcurrentUpdatesNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            StripchatConcurrentUpdatesNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // DefaultActionLabel
            // 
            DefaultActionLabel.AutoSize = true;
            DefaultActionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            DefaultActionLabel.Location = new System.Drawing.Point(38, 208);
            DefaultActionLabel.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            DefaultActionLabel.Name = "DefaultActionLabel";
            DefaultActionLabel.Size = new System.Drawing.Size(350, 24);
            DefaultActionLabel.TabIndex = 41;
            DefaultActionLabel.Text = "Default action";
            // 
            // DefaultResolutionLabel
            // 
            DefaultResolutionLabel.AutoSize = true;
            DefaultResolutionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            DefaultResolutionLabel.Location = new System.Drawing.Point(38, 237);
            DefaultResolutionLabel.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            DefaultResolutionLabel.Name = "DefaultResolutionLabel";
            DefaultResolutionLabel.Size = new System.Drawing.Size(350, 24);
            DefaultResolutionLabel.TabIndex = 42;
            DefaultResolutionLabel.Text = "Default resolution";
            // 
            // DefaultActionComboBox
            // 
            DefaultActionComboBox.BackColor = System.Drawing.SystemColors.Window;
            DefaultActionComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            DefaultActionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            DefaultActionComboBox.FormattingEnabled = true;
            DefaultActionComboBox.Location = new System.Drawing.Point(388, 208);
            DefaultActionComboBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            DefaultActionComboBox.Name = "DefaultActionComboBox";
            DefaultActionComboBox.Size = new System.Drawing.Size(350, 23);
            DefaultActionComboBox.TabIndex = 8;
            // 
            // DefaultResolutionComboBox
            // 
            DefaultResolutionComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            DefaultResolutionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            DefaultResolutionComboBox.FormattingEnabled = true;
            DefaultResolutionComboBox.Location = new System.Drawing.Point(388, 237);
            DefaultResolutionComboBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            DefaultResolutionComboBox.Name = "DefaultResolutionComboBox";
            DefaultResolutionComboBox.Size = new System.Drawing.Size(350, 23);
            DefaultResolutionComboBox.TabIndex = 9;
            // 
            // StreamlinkPathLabel
            // 
            StreamlinkPathLabel.AutoSize = true;
            StreamlinkPathLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            StreamlinkPathLabel.Location = new System.Drawing.Point(38, 63);
            StreamlinkPathLabel.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            StreamlinkPathLabel.Name = "StreamlinkPathLabel";
            StreamlinkPathLabel.Size = new System.Drawing.Size(350, 24);
            StreamlinkPathLabel.TabIndex = 45;
            StreamlinkPathLabel.Text = "Streamlink path";
            // 
            // StreamlinkPathTextBox
            // 
            StreamlinkPathTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            StreamlinkPathTextBox.BackColor = System.Drawing.SystemColors.Window;
            StreamlinkPathTextBox.Location = new System.Drawing.Point(388, 63);
            StreamlinkPathTextBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            StreamlinkPathTextBox.Name = "StreamlinkPathTextBox";
            StreamlinkPathTextBox.ReadOnly = true;
            StreamlinkPathTextBox.Size = new System.Drawing.Size(350, 23);
            StreamlinkPathTextBox.TabIndex = 3;
            StreamlinkPathTextBox.Click += StreamlinkPathTextBox_Click;
            // 
            // LogTabPage
            // 
            LogTabPage.Controls.Add(LogListBox);
            LogTabPage.Location = new System.Drawing.Point(4, 28);
            LogTabPage.Margin = new System.Windows.Forms.Padding(0);
            LogTabPage.Name = "LogTabPage";
            LogTabPage.Size = new System.Drawing.Size(776, 529);
            LogTabPage.TabIndex = 4;
            LogTabPage.Text = "Log";
            LogTabPage.UseVisualStyleBackColor = true;
            // 
            // LogListBox
            // 
            LogListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            LogListBox.ContextMenuStrip = LogContextMenuStrip;
            LogListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            LogListBox.FormattingEnabled = true;
            LogListBox.HorizontalScrollbar = true;
            LogListBox.IntegralHeight = false;
            LogListBox.ItemHeight = 15;
            LogListBox.Location = new System.Drawing.Point(0, 0);
            LogListBox.Name = "LogListBox";
            LogListBox.ScrollAlwaysVisible = true;
            LogListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            LogListBox.Size = new System.Drawing.Size(776, 529);
            LogListBox.TabIndex = 0;
            // 
            // LogContextMenuStrip
            // 
            LogContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { CopyLogToolStripMenuItem });
            LogContextMenuStrip.Name = "LogContextMenuStrip";
            LogContextMenuStrip.Size = new System.Drawing.Size(103, 26);
            LogContextMenuStrip.Opening += LogContextMenuStrip_Opening;
            // 
            // CopyLogToolStripMenuItem
            // 
            CopyLogToolStripMenuItem.Name = "CopyLogToolStripMenuItem";
            CopyLogToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            CopyLogToolStripMenuItem.Text = "Copy";
            CopyLogToolStripMenuItem.Click += CopyLogToolStripMenuItem_Click;
            // 
            // AboutTabPage
            // 
            AboutTabPage.Controls.Add(LicenseTextBox);
            AboutTabPage.Location = new System.Drawing.Point(4, 28);
            AboutTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            AboutTabPage.Name = "AboutTabPage";
            AboutTabPage.Size = new System.Drawing.Size(776, 529);
            AboutTabPage.TabIndex = 3;
            AboutTabPage.Text = "About";
            AboutTabPage.UseVisualStyleBackColor = true;
            // 
            // LicenseTextBox
            // 
            LicenseTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LicenseTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            LicenseTextBox.Location = new System.Drawing.Point(0, 0);
            LicenseTextBox.Margin = new System.Windows.Forms.Padding(0);
            LicenseTextBox.Multiline = true;
            LicenseTextBox.Name = "LicenseTextBox";
            LicenseTextBox.Size = new System.Drawing.Size(776, 529);
            LicenseTextBox.TabIndex = 2;
            LicenseTextBox.Text = resources.GetString("LicenseTextBox.Text");
            LicenseTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ChatRoomsUpdateTimer
            // 
            ChatRoomsUpdateTimer.Interval = 5000;
            ChatRoomsUpdateTimer.Tick += ChatRoomsUpdateTimer_Tick;
            // 
            // FormCloseTimer
            // 
            FormCloseTimer.Interval = 1000;
            FormCloseTimer.Tick += FormCloseTimer_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Window;
            ClientSize = new System.Drawing.Size(784, 561);
            Controls.Add(TabControl);
            Margin = new System.Windows.Forms.Padding(2);
            MinimumSize = new System.Drawing.Size(800, 600);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            FormClosing += MainForm_FormClosing;
            ((System.ComponentModel.ISupportInitialize)FilesBindingSource).EndInit();
            TabControl.ResumeLayout(false);
            WebBrowserTabPage.ResumeLayout(false);
            WebBrowserTableLayoutPanel1.ResumeLayout(false);
            WebBrowserTableLayoutPanel2.ResumeLayout(false);
            WebBrowserTableLayoutPanel2.PerformLayout();
            WebView2Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)WebView2).EndInit();
            ChatRoomsTabPage.ResumeLayout(false);
            ChatRoomsTableLayoutPanel1.ResumeLayout(false);
            ChatRoomsTableLayoutPanel2.ResumeLayout(false);
            ChatRoomsTableLayoutPanel2.PerformLayout();
            ChatRoomsSplitContainer1.Panel1.ResumeLayout(false);
            ChatRoomsSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ChatRoomsSplitContainer1).EndInit();
            ChatRoomsSplitContainer1.ResumeLayout(false);
            ChatRoomsSplitContainer3.Panel1.ResumeLayout(false);
            ChatRoomsSplitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ChatRoomsSplitContainer3).EndInit();
            ChatRoomsSplitContainer3.ResumeLayout(false);
            CategoriesContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ChatRoomsDataGridView).EndInit();
            ChatRoomsContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ChatRoomsBindingSource).EndInit();
            ChatRoomsSplitContainer2.Panel1.ResumeLayout(false);
            ChatRoomsSplitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ChatRoomsSplitContainer2).EndInit();
            ChatRoomsSplitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ThumbnailPictureBox).EndInit();
            ThumbnailContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)FilesDataGridView).EndInit();
            FilesContextMenuStrip.ResumeLayout(false);
            SettingsTabPage.ResumeLayout(false);
            SettingsTableLayoutPanel.ResumeLayout(false);
            SettingsTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ChaturbateConcurrentUpdatesNumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)BongaCamsConcurrentUpdatesNumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)UpdateIntervalNumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)StripchatConcurrentUpdatesNumericUpDown).EndInit();
            LogTabPage.ResumeLayout(false);
            LogContextMenuStrip.ResumeLayout(false);
            AboutTabPage.ResumeLayout(false);
            AboutTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SettingsBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage ChatRoomsTabPage;
        private System.Windows.Forms.TabPage SettingsTabPage;
        private System.Windows.Forms.DataGridView ChatRoomsDataGridView;
        private System.Windows.Forms.TextBox UrlTextBox;
        private System.Windows.Forms.TextBox FFmpegPathTextBox;
        private System.Windows.Forms.TextBox OutputDirectoryTextBox;
        private System.Windows.Forms.Label OutputDirectoryLabel;
        private System.Windows.Forms.TabPage AboutTabPage;
        private System.Windows.Forms.TextBox LicenseTextBox;
        private System.Windows.Forms.Timer ChatRoomsUpdateTimer;
        private System.Windows.Forms.Label FFmpegPathLabel;
        private System.Windows.Forms.Timer FormCloseTimer;
        private System.Windows.Forms.SplitContainer ChatRoomsSplitContainer1;
        private System.Windows.Forms.DataGridView FilesDataGridView;
        private System.Windows.Forms.Label ChaturbateConcurrentUpdatesLabel;
        private System.Windows.Forms.Label BongaCamsConcurrentUpdatesLabel;
        private System.Windows.Forms.NumericUpDown BongaCamsConcurrentUpdatesNumericUpDown;
        private System.Windows.Forms.NumericUpDown ChaturbateConcurrentUpdatesNumericUpDown;
        private System.Windows.Forms.Label UpdateIntervalLabel;
        private System.Windows.Forms.NumericUpDown UpdateIntervalNumericUpDown;
        private System.Windows.Forms.BindingSource ChatRoomsBindingSource;
        private System.Windows.Forms.BindingSource FilesBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileSizeColumn;
        private System.Windows.Forms.BindingSource SettingsBindingSource;
        private System.Windows.Forms.TabPage WebBrowserTabPage;
        private System.Windows.Forms.TableLayoutPanel WebBrowserTableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel WebBrowserTableLayoutPanel2;
        private System.Windows.Forms.Button NavigateButton;
        private System.Windows.Forms.TextBox AddressBarTextBox;
        private System.Windows.Forms.Panel WebView2Panel;
        private Microsoft.Web.WebView2.WinForms.WebView2 WebView2;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button ForwardButton;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.TableLayoutPanel ChatRoomsTableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel ChatRoomsTableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel SettingsTableLayoutPanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn WebsiteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ActionColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ResolutionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdatedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SeenColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UrlColumn;
        private System.Windows.Forms.SplitContainer ChatRoomsSplitContainer2;
        private System.Windows.Forms.PictureBox ThumbnailPictureBox;
        private System.Windows.Forms.Label StripchatConcurrentUpdatesLabel;
        private System.Windows.Forms.NumericUpDown StripchatConcurrentUpdatesNumericUpDown;
        private System.Windows.Forms.Label DefaultActionLabel;
        private System.Windows.Forms.Label DefaultResolutionLabel;
        private System.Windows.Forms.ComboBox DefaultActionComboBox;
        private System.Windows.Forms.ComboBox DefaultResolutionComboBox;
        private System.Windows.Forms.Label StreamlinkPathLabel;
        private System.Windows.Forms.TextBox StreamlinkPathTextBox;
        private System.Windows.Forms.SplitContainer ChatRoomsSplitContainer3;
        private System.Windows.Forms.TreeView CategoriesTreeView;
        private System.Windows.Forms.ContextMenuStrip ThumbnailContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem RemoveThumbnailToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip FilesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem OpenFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveFileToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ChatRoomsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem RemoveChatRoomToolStripMenuItem;
        private System.Windows.Forms.ImageList CategoriesImageList;
        private System.Windows.Forms.ContextMenuStrip CategoriesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem AddCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RenameCategoryToolStripMenuItem;
        private System.Windows.Forms.Button AddChatRoomFromUrlButton;
        private System.Windows.Forms.Button AddChatRoomsFromFileButton;
        private System.Windows.Forms.Button AddChatRoomsFromFolderButton;
        private System.Windows.Forms.TabPage LogTabPage;
        private System.Windows.Forms.ListBox LogListBox;
        private System.Windows.Forms.ContextMenuStrip LogContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem CopyLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetActionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetResolutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CopyUrlToolStripMenuItem;
    }
}

