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
            this.URLTextBox = new System.Windows.Forms.TextBox();
            this.PlusButton = new System.Windows.Forms.Button();
            this.MinusButton = new System.Windows.Forms.Button();
            this.ChatRoomsDataGridView = new System.Windows.Forms.DataGridView();
            this.IndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WebsiteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActionColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResolutionColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.DataGridViewUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.OutputDirectoryLabel = new System.Windows.Forms.Label();
            this.FFmpegPathLabel = new System.Windows.Forms.Label();
            this.OutputDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.FFmpegPathTextBox = new System.Windows.Forms.TextBox();
            this.UpButton = new System.Windows.Forms.Button();
            this.DownButton = new System.Windows.Forms.Button();
            this.ChromeExecutablePathLabel = new System.Windows.Forms.Label();
            this.ChromeDataDirectoryLabel = new System.Windows.Forms.Label();
            this.ChromeExecutablePathTextBox = new System.Windows.Forms.TextBox();
            this.ChromeDataDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ChromeExecutablePathButton = new System.Windows.Forms.Button();
            this.ChromeDataDirectoryButton = new System.Windows.Forms.Button();
            this.FFmpegPathButton = new System.Windows.Forms.Button();
            this.OutputDirectoryButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ChatRoomsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // URLTextBox
            // 
            this.URLTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.URLTextBox.Location = new System.Drawing.Point(10, 10);
            this.URLTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.URLTextBox.Name = "URLTextBox";
            this.URLTextBox.Size = new System.Drawing.Size(423, 20);
            this.URLTextBox.TabIndex = 0;
            // 
            // PlusButton
            // 
            this.PlusButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PlusButton.Location = new System.Drawing.Point(440, 9);
            this.PlusButton.Name = "PlusButton";
            this.PlusButton.Size = new System.Drawing.Size(39, 22);
            this.PlusButton.TabIndex = 1;
            this.PlusButton.Text = "+";
            this.PlusButton.UseVisualStyleBackColor = true;
            this.PlusButton.Click += new System.EventHandler(this.PlusButton_Click);
            // 
            // MinusButton
            // 
            this.MinusButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MinusButton.Location = new System.Drawing.Point(485, 9);
            this.MinusButton.Name = "MinusButton";
            this.MinusButton.Size = new System.Drawing.Size(39, 22);
            this.MinusButton.TabIndex = 2;
            this.MinusButton.Text = "-";
            this.MinusButton.UseVisualStyleBackColor = true;
            this.MinusButton.Click += new System.EventHandler(this.MinusButton_Click);
            // 
            // ChatRoomsDataGridView
            // 
            this.ChatRoomsDataGridView.AllowUserToAddRows = false;
            this.ChatRoomsDataGridView.AllowUserToDeleteRows = false;
            this.ChatRoomsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.ChatRoomsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ChatRoomsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IndexColumn,
            this.WebsiteColumn,
            this.NameColumn,
            this.ActionColumn,
            this.StatusColumn,
            this.ResolutionColumn});
            this.ChatRoomsDataGridView.Location = new System.Drawing.Point(10, 40);
            this.ChatRoomsDataGridView.MultiSelect = false;
            this.ChatRoomsDataGridView.Name = "ChatRoomsDataGridView";
            this.ChatRoomsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ChatRoomsDataGridView.Size = new System.Drawing.Size(605, 271);
            this.ChatRoomsDataGridView.TabIndex = 5;
            this.ChatRoomsDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ChatRoomsDataGridView_CellValueChanged);
            this.ChatRoomsDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ChatRoomsDataGridView_ColumnHeaderMouseClick);
            // 
            // IndexColumn
            // 
            this.IndexColumn.HeaderText = "#";
            this.IndexColumn.Name = "IndexColumn";
            this.IndexColumn.ReadOnly = true;
            this.IndexColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.IndexColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.IndexColumn.Width = 40;
            // 
            // WebsiteColumn
            // 
            this.WebsiteColumn.HeaderText = "Website";
            this.WebsiteColumn.Name = "WebsiteColumn";
            this.WebsiteColumn.ReadOnly = true;
            this.WebsiteColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.WebsiteColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.WebsiteColumn.Width = 80;
            // 
            // NameColumn
            // 
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.NameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.NameColumn.Width = 178;
            // 
            // ActionColumn
            // 
            this.ActionColumn.HeaderText = "Action";
            this.ActionColumn.Name = "ActionColumn";
            this.ActionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ActionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.ActionColumn.Width = 82;
            // 
            // StatusColumn
            // 
            this.StatusColumn.HeaderText = "Status";
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.ReadOnly = true;
            this.StatusColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.StatusColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.StatusColumn.Width = 82;
            // 
            // ResolutionColumn
            // 
            this.ResolutionColumn.HeaderText = "Resolution";
            this.ResolutionColumn.Name = "ResolutionColumn";
            this.ResolutionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ResolutionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.ResolutionColumn.Width = 82;
            // 
            // DataGridViewUpdateTimer
            // 
            this.DataGridViewUpdateTimer.Interval = 5000;
            this.DataGridViewUpdateTimer.Tick += new System.EventHandler(this.DataGridViewUpdateTimer_Tick);
            // 
            // OutputDirectoryLabel
            // 
            this.OutputDirectoryLabel.AutoSize = true;
            this.OutputDirectoryLabel.Location = new System.Drawing.Point(7, 413);
            this.OutputDirectoryLabel.Name = "OutputDirectoryLabel";
            this.OutputDirectoryLabel.Size = new System.Drawing.Size(82, 13);
            this.OutputDirectoryLabel.TabIndex = 7;
            this.OutputDirectoryLabel.Text = "Output directory";
            // 
            // FFmpegPathLabel
            // 
            this.FFmpegPathLabel.AutoSize = true;
            this.FFmpegPathLabel.Location = new System.Drawing.Point(7, 383);
            this.FFmpegPathLabel.Name = "FFmpegPathLabel";
            this.FFmpegPathLabel.Size = new System.Drawing.Size(69, 13);
            this.FFmpegPathLabel.TabIndex = 8;
            this.FFmpegPathLabel.Text = "FFmpeg path";
            // 
            // OutputDirectoryTextBox
            // 
            this.OutputDirectoryTextBox.Location = new System.Drawing.Point(137, 410);
            this.OutputDirectoryTextBox.Name = "OutputDirectoryTextBox";
            this.OutputDirectoryTextBox.ReadOnly = true;
            this.OutputDirectoryTextBox.Size = new System.Drawing.Size(432, 20);
            this.OutputDirectoryTextBox.TabIndex = 9;
            // 
            // FFmpegPathTextBox
            // 
            this.FFmpegPathTextBox.Location = new System.Drawing.Point(137, 380);
            this.FFmpegPathTextBox.Name = "FFmpegPathTextBox";
            this.FFmpegPathTextBox.ReadOnly = true;
            this.FFmpegPathTextBox.Size = new System.Drawing.Size(432, 20);
            this.FFmpegPathTextBox.TabIndex = 8;
            // 
            // UpButton
            // 
            this.UpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UpButton.Location = new System.Drawing.Point(530, 9);
            this.UpButton.Name = "UpButton";
            this.UpButton.Size = new System.Drawing.Size(39, 22);
            this.UpButton.TabIndex = 3;
            this.UpButton.Text = "↑";
            this.UpButton.UseVisualStyleBackColor = true;
            this.UpButton.Click += new System.EventHandler(this.UpButton_Click);
            // 
            // DownButton
            // 
            this.DownButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DownButton.Location = new System.Drawing.Point(575, 9);
            this.DownButton.Name = "DownButton";
            this.DownButton.Size = new System.Drawing.Size(39, 22);
            this.DownButton.TabIndex = 4;
            this.DownButton.Text = "↓";
            this.DownButton.UseVisualStyleBackColor = true;
            this.DownButton.Click += new System.EventHandler(this.DownButton_Click);
            // 
            // ChromeExecutablePathLabel
            // 
            this.ChromeExecutablePathLabel.AutoSize = true;
            this.ChromeExecutablePathLabel.Location = new System.Drawing.Point(7, 323);
            this.ChromeExecutablePathLabel.Name = "ChromeExecutablePathLabel";
            this.ChromeExecutablePathLabel.Size = new System.Drawing.Size(122, 13);
            this.ChromeExecutablePathLabel.TabIndex = 13;
            this.ChromeExecutablePathLabel.Text = "Chrome executable path";
            // 
            // ChromeDataDirectoryLabel
            // 
            this.ChromeDataDirectoryLabel.AutoSize = true;
            this.ChromeDataDirectoryLabel.Location = new System.Drawing.Point(7, 353);
            this.ChromeDataDirectoryLabel.Name = "ChromeDataDirectoryLabel";
            this.ChromeDataDirectoryLabel.Size = new System.Drawing.Size(110, 13);
            this.ChromeDataDirectoryLabel.TabIndex = 14;
            this.ChromeDataDirectoryLabel.Text = "Chrome data directory";
            // 
            // ChromeExecutablePathTextBox
            // 
            this.ChromeExecutablePathTextBox.Location = new System.Drawing.Point(137, 320);
            this.ChromeExecutablePathTextBox.Name = "ChromeExecutablePathTextBox";
            this.ChromeExecutablePathTextBox.ReadOnly = true;
            this.ChromeExecutablePathTextBox.Size = new System.Drawing.Size(432, 20);
            this.ChromeExecutablePathTextBox.TabIndex = 6;
            // 
            // ChromeDataDirectoryTextBox
            // 
            this.ChromeDataDirectoryTextBox.Location = new System.Drawing.Point(137, 350);
            this.ChromeDataDirectoryTextBox.Name = "ChromeDataDirectoryTextBox";
            this.ChromeDataDirectoryTextBox.ReadOnly = true;
            this.ChromeDataDirectoryTextBox.Size = new System.Drawing.Size(432, 20);
            this.ChromeDataDirectoryTextBox.TabIndex = 7;
            // 
            // ChromeExecutablePathButton
            // 
            this.ChromeExecutablePathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ChromeExecutablePathButton.Location = new System.Drawing.Point(576, 319);
            this.ChromeExecutablePathButton.Name = "ChromeExecutablePathButton";
            this.ChromeExecutablePathButton.Size = new System.Drawing.Size(39, 22);
            this.ChromeExecutablePathButton.TabIndex = 15;
            this.ChromeExecutablePathButton.Text = "...";
            this.ChromeExecutablePathButton.UseVisualStyleBackColor = true;
            this.ChromeExecutablePathButton.Click += new System.EventHandler(this.ChromeExecutablePathButton_Click);
            // 
            // ChromeDataDirectoryButton
            // 
            this.ChromeDataDirectoryButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ChromeDataDirectoryButton.Location = new System.Drawing.Point(576, 349);
            this.ChromeDataDirectoryButton.Name = "ChromeDataDirectoryButton";
            this.ChromeDataDirectoryButton.Size = new System.Drawing.Size(39, 22);
            this.ChromeDataDirectoryButton.TabIndex = 16;
            this.ChromeDataDirectoryButton.Text = "...";
            this.ChromeDataDirectoryButton.UseVisualStyleBackColor = true;
            this.ChromeDataDirectoryButton.Click += new System.EventHandler(this.ChromeDataDirectoryButton_Click);
            // 
            // FFmpegPathButton
            // 
            this.FFmpegPathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FFmpegPathButton.Location = new System.Drawing.Point(576, 379);
            this.FFmpegPathButton.Name = "FFmpegPathButton";
            this.FFmpegPathButton.Size = new System.Drawing.Size(39, 22);
            this.FFmpegPathButton.TabIndex = 17;
            this.FFmpegPathButton.Text = "...";
            this.FFmpegPathButton.UseVisualStyleBackColor = true;
            this.FFmpegPathButton.Click += new System.EventHandler(this.FFmpegPathButton_Click);
            // 
            // OutputDirectoryButton
            // 
            this.OutputDirectoryButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OutputDirectoryButton.Location = new System.Drawing.Point(576, 409);
            this.OutputDirectoryButton.Name = "OutputDirectoryButton";
            this.OutputDirectoryButton.Size = new System.Drawing.Size(39, 22);
            this.OutputDirectoryButton.TabIndex = 18;
            this.OutputDirectoryButton.Text = "...";
            this.OutputDirectoryButton.UseVisualStyleBackColor = true;
            this.OutputDirectoryButton.Click += new System.EventHandler(this.OutputDirectoryButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.OutputDirectoryButton);
            this.Controls.Add(this.FFmpegPathButton);
            this.Controls.Add(this.ChromeDataDirectoryButton);
            this.Controls.Add(this.ChromeExecutablePathButton);
            this.Controls.Add(this.ChromeDataDirectoryTextBox);
            this.Controls.Add(this.ChromeExecutablePathTextBox);
            this.Controls.Add(this.ChromeDataDirectoryLabel);
            this.Controls.Add(this.ChromeExecutablePathLabel);
            this.Controls.Add(this.DownButton);
            this.Controls.Add(this.UpButton);
            this.Controls.Add(this.FFmpegPathTextBox);
            this.Controls.Add(this.OutputDirectoryTextBox);
            this.Controls.Add(this.FFmpegPathLabel);
            this.Controls.Add(this.OutputDirectoryLabel);
            this.Controls.Add(this.ChatRoomsDataGridView);
            this.Controls.Add(this.MinusButton);
            this.Controls.Add(this.PlusButton);
            this.Controls.Add(this.URLTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.MainForm_HelpButtonClicked);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.ChatRoomsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox URLTextBox;
        private System.Windows.Forms.Button PlusButton;
        private System.Windows.Forms.Button MinusButton;
        private System.Windows.Forms.DataGridView ChatRoomsDataGridView;
        private System.Windows.Forms.Timer DataGridViewUpdateTimer;
        private System.Windows.Forms.Label OutputDirectoryLabel;
        private System.Windows.Forms.Label FFmpegPathLabel;
        private System.Windows.Forms.TextBox OutputDirectoryTextBox;
        private System.Windows.Forms.TextBox FFmpegPathTextBox;
        private System.Windows.Forms.Button UpButton;
        private System.Windows.Forms.Button DownButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn IndexColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn WebsiteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ActionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ResolutionColumn;
        private System.Windows.Forms.Label ChromeExecutablePathLabel;
        private System.Windows.Forms.Label ChromeDataDirectoryLabel;
        private System.Windows.Forms.TextBox ChromeExecutablePathTextBox;
        private System.Windows.Forms.TextBox ChromeDataDirectoryTextBox;
        private System.Windows.Forms.Button ChromeExecutablePathButton;
        private System.Windows.Forms.Button ChromeDataDirectoryButton;
        private System.Windows.Forms.Button FFmpegPathButton;
        private System.Windows.Forms.Button OutputDirectoryButton;
    }
}

