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
            this.DataGridViewUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.OutputDirectoryLabel = new System.Windows.Forms.Label();
            this.FFmpegPathLabel = new System.Windows.Forms.Label();
            this.OutputDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.FFmpegPathTextBox = new System.Windows.Forms.TextBox();
            this.UpButton = new System.Windows.Forms.Button();
            this.DownButton = new System.Windows.Forms.Button();
            this.IndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActionColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResolutionColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ChatRoomsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // URLTextBox
            // 
            this.URLTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.URLTextBox.Location = new System.Drawing.Point(10, 10);
            this.URLTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.URLTextBox.Name = "URLTextBox";
            this.URLTextBox.Size = new System.Drawing.Size(406, 20);
            this.URLTextBox.TabIndex = 0;
            // 
            // PlusButton
            // 
            this.PlusButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PlusButton.Location = new System.Drawing.Point(421, 9);
            this.PlusButton.Name = "PlusButton";
            this.PlusButton.Size = new System.Drawing.Size(44, 22);
            this.PlusButton.TabIndex = 1;
            this.PlusButton.Text = "+";
            this.PlusButton.UseVisualStyleBackColor = true;
            this.PlusButton.Click += new System.EventHandler(this.PlusButton_Click);
            // 
            // MinusButton
            // 
            this.MinusButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MinusButton.Location = new System.Drawing.Point(471, 9);
            this.MinusButton.Name = "MinusButton";
            this.MinusButton.Size = new System.Drawing.Size(44, 22);
            this.MinusButton.TabIndex = 3;
            this.MinusButton.Text = "-";
            this.MinusButton.UseVisualStyleBackColor = true;
            this.MinusButton.Click += new System.EventHandler(this.MinusButton_Click);
            // 
            // ChatRoomsDataGridView
            // 
            this.ChatRoomsDataGridView.AllowUserToAddRows = false;
            this.ChatRoomsDataGridView.AllowUserToDeleteRows = false;
            this.ChatRoomsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ChatRoomsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IndexColumn,
            this.NameColumn,
            this.ActionColumn,
            this.StatusColumn,
            this.ResolutionColumn});
            this.ChatRoomsDataGridView.Location = new System.Drawing.Point(10, 40);
            this.ChatRoomsDataGridView.MultiSelect = false;
            this.ChatRoomsDataGridView.Name = "ChatRoomsDataGridView";
            this.ChatRoomsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ChatRoomsDataGridView.Size = new System.Drawing.Size(605, 330);
            this.ChatRoomsDataGridView.TabIndex = 6;
            this.ChatRoomsDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ChatRoomsDataGridView_CellValueChanged);
            this.ChatRoomsDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ChatRoomsDataGridView_ColumnHeaderMouseClick);
            // 
            // DataGridViewUpdateTimer
            // 
            this.DataGridViewUpdateTimer.Interval = 5000;
            this.DataGridViewUpdateTimer.Tick += new System.EventHandler(this.DataGridViewUpdateTimer_Tick);
            // 
            // OutputDirectoryLabel
            // 
            this.OutputDirectoryLabel.AutoSize = true;
            this.OutputDirectoryLabel.Location = new System.Drawing.Point(7, 383);
            this.OutputDirectoryLabel.Name = "OutputDirectoryLabel";
            this.OutputDirectoryLabel.Size = new System.Drawing.Size(82, 13);
            this.OutputDirectoryLabel.TabIndex = 7;
            this.OutputDirectoryLabel.Text = "Output directory";
            // 
            // FFmpegPathLabel
            // 
            this.FFmpegPathLabel.AutoSize = true;
            this.FFmpegPathLabel.Location = new System.Drawing.Point(7, 413);
            this.FFmpegPathLabel.Name = "FFmpegPathLabel";
            this.FFmpegPathLabel.Size = new System.Drawing.Size(69, 13);
            this.FFmpegPathLabel.TabIndex = 8;
            this.FFmpegPathLabel.Text = "FFmpeg path";
            // 
            // OutputDirectoryTextBox
            // 
            this.OutputDirectoryTextBox.Location = new System.Drawing.Point(95, 380);
            this.OutputDirectoryTextBox.Name = "OutputDirectoryTextBox";
            this.OutputDirectoryTextBox.ReadOnly = true;
            this.OutputDirectoryTextBox.Size = new System.Drawing.Size(520, 20);
            this.OutputDirectoryTextBox.TabIndex = 9;
            this.OutputDirectoryTextBox.Enter += new System.EventHandler(this.OutputDirectoryTextBox_Enter);
            // 
            // FFmpegPathTextBox
            // 
            this.FFmpegPathTextBox.Location = new System.Drawing.Point(95, 410);
            this.FFmpegPathTextBox.Name = "FFmpegPathTextBox";
            this.FFmpegPathTextBox.ReadOnly = true;
            this.FFmpegPathTextBox.Size = new System.Drawing.Size(520, 20);
            this.FFmpegPathTextBox.TabIndex = 10;
            this.FFmpegPathTextBox.Enter += new System.EventHandler(this.FFmpegPathTextBox_Enter);
            // 
            // UpButton
            // 
            this.UpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UpButton.Location = new System.Drawing.Point(521, 9);
            this.UpButton.Name = "UpButton";
            this.UpButton.Size = new System.Drawing.Size(44, 22);
            this.UpButton.TabIndex = 11;
            this.UpButton.Text = "↑";
            this.UpButton.UseVisualStyleBackColor = true;
            this.UpButton.Click += new System.EventHandler(this.UpButton_Click);
            // 
            // DownButton
            // 
            this.DownButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DownButton.Location = new System.Drawing.Point(571, 9);
            this.DownButton.Name = "DownButton";
            this.DownButton.Size = new System.Drawing.Size(44, 22);
            this.DownButton.TabIndex = 12;
            this.DownButton.Text = "↓";
            this.DownButton.UseVisualStyleBackColor = true;
            this.DownButton.Click += new System.EventHandler(this.DownButton_Click);
            // 
            // IndexColumn
            // 
            this.IndexColumn.HeaderText = "#";
            this.IndexColumn.Name = "IndexColumn";
            this.IndexColumn.ReadOnly = true;
            this.IndexColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.IndexColumn.Width = 40;
            // 
            // NameColumn
            // 
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.NameColumn.Width = 260;
            // 
            // ActionColumn
            // 
            this.ActionColumn.HeaderText = "Action";
            this.ActionColumn.Name = "ActionColumn";
            this.ActionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.ActionColumn.Width = 80;
            // 
            // StatusColumn
            // 
            this.StatusColumn.HeaderText = "Status";
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.ReadOnly = true;
            this.StatusColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.StatusColumn.Width = 80;
            // 
            // ResolutionColumn
            // 
            this.ResolutionColumn.HeaderText = "Resolution";
            this.ResolutionColumn.Name = "ResolutionColumn";
            this.ResolutionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.ResolutionColumn.Width = 80;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ActionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ResolutionColumn;
    }
}

