namespace ChatRoomRecorder
{
    partial class MergeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            SplitContainer = new System.Windows.Forms.SplitContainer();
            WindowsMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            FilesDataGridView = new System.Windows.Forms.DataGridView();
            FileNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            FilesSizeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            FilesBindingSource = new System.Windows.Forms.BindingSource(components);
            TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            OKButton = new System.Windows.Forms.Button();
            TableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SplitContainer).BeginInit();
            SplitContainer.Panel1.SuspendLayout();
            SplitContainer.Panel2.SuspendLayout();
            SplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)WindowsMediaPlayer).BeginInit();
            ((System.ComponentModel.ISupportInitialize)FilesDataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)FilesBindingSource).BeginInit();
            TableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.ColumnCount = 1;
            TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TableLayoutPanel1.Controls.Add(SplitContainer, 0, 0);
            TableLayoutPanel1.Controls.Add(TableLayoutPanel2, 0, 1);
            TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            TableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.Padding = new System.Windows.Forms.Padding(4);
            TableLayoutPanel1.RowCount = 2;
            TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            TableLayoutPanel1.Size = new System.Drawing.Size(484, 461);
            TableLayoutPanel1.TabIndex = 0;
            // 
            // SplitContainer
            // 
            SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            SplitContainer.Location = new System.Drawing.Point(4, 4);
            SplitContainer.Margin = new System.Windows.Forms.Padding(0);
            SplitContainer.Name = "SplitContainer";
            SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer.Panel1
            // 
            SplitContainer.Panel1.Controls.Add(WindowsMediaPlayer);
            // 
            // SplitContainer.Panel2
            // 
            SplitContainer.Panel2.Controls.Add(FilesDataGridView);
            SplitContainer.Size = new System.Drawing.Size(476, 420);
            SplitContainer.SplitterDistance = 287;
            SplitContainer.TabIndex = 0;
            // 
            // WindowsMediaPlayer
            // 
            WindowsMediaPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            WindowsMediaPlayer.Enabled = true;
            WindowsMediaPlayer.Location = new System.Drawing.Point(0, 0);
            WindowsMediaPlayer.Margin = new System.Windows.Forms.Padding(0);
            WindowsMediaPlayer.Name = "WindowsMediaPlayer";
            WindowsMediaPlayer.OcxState = (System.Windows.Forms.AxHost.State)resources.GetObject("WindowsMediaPlayer.OcxState");
            WindowsMediaPlayer.Size = new System.Drawing.Size(476, 287);
            WindowsMediaPlayer.TabIndex = 1;
            // 
            // FilesDataGridView
            // 
            FilesDataGridView.AllowDrop = true;
            FilesDataGridView.AllowUserToAddRows = false;
            FilesDataGridView.AllowUserToDeleteRows = false;
            FilesDataGridView.AllowUserToResizeRows = false;
            FilesDataGridView.AutoGenerateColumns = false;
            FilesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            FilesDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            FilesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            FilesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            FilesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { FileNameDataGridViewTextBoxColumn, FilesSizeDataGridViewTextBoxColumn });
            FilesDataGridView.DataSource = FilesBindingSource;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            FilesDataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            FilesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            FilesDataGridView.Location = new System.Drawing.Point(0, 0);
            FilesDataGridView.Margin = new System.Windows.Forms.Padding(0);
            FilesDataGridView.MultiSelect = false;
            FilesDataGridView.Name = "FilesDataGridView";
            FilesDataGridView.RowHeadersVisible = false;
            FilesDataGridView.RowTemplate.Height = 25;
            FilesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            FilesDataGridView.Size = new System.Drawing.Size(476, 129);
            FilesDataGridView.TabIndex = 2;
            FilesDataGridView.CellMouseDown += FilesDataGridView_CellMouseDown;
            FilesDataGridView.DragDrop += FilesDataGridView_DragDrop;
            FilesDataGridView.DragOver += FilesDataGridView_DragOver;
            FilesDataGridView.DoubleClick += FilesDataGridView_DoubleClick;
            // 
            // FileNameDataGridViewTextBoxColumn
            // 
            FileNameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FileNameDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            FileNameDataGridViewTextBoxColumn.FillWeight = 139.0863F;
            FileNameDataGridViewTextBoxColumn.HeaderText = "File name";
            FileNameDataGridViewTextBoxColumn.Name = "FileNameDataGridViewTextBoxColumn";
            FileNameDataGridViewTextBoxColumn.ReadOnly = true;
            FileNameDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FilesSizeDataGridViewTextBoxColumn
            // 
            FilesSizeDataGridViewTextBoxColumn.DataPropertyName = "Length";
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.Format = "N0";
            FilesSizeDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            FilesSizeDataGridViewTextBoxColumn.FillWeight = 60.9137039F;
            FilesSizeDataGridViewTextBoxColumn.HeaderText = "File size";
            FilesSizeDataGridViewTextBoxColumn.Name = "FilesSizeDataGridViewTextBoxColumn";
            FilesSizeDataGridViewTextBoxColumn.ReadOnly = true;
            FilesSizeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FilesBindingSource
            // 
            FilesBindingSource.AllowNew = true;
            FilesBindingSource.DataSource = typeof(BindingListFiles);
            // 
            // TableLayoutPanel2
            // 
            TableLayoutPanel2.ColumnCount = 2;
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            TableLayoutPanel2.Controls.Add(OKButton, 1, 0);
            TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            TableLayoutPanel2.Location = new System.Drawing.Point(4, 424);
            TableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            TableLayoutPanel2.Name = "TableLayoutPanel2";
            TableLayoutPanel2.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            TableLayoutPanel2.RowCount = 1;
            TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TableLayoutPanel2.Size = new System.Drawing.Size(476, 33);
            TableLayoutPanel2.TabIndex = 1;
            // 
            // OKButton
            // 
            OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            OKButton.Dock = System.Windows.Forms.DockStyle.Fill;
            OKButton.Location = new System.Drawing.Point(378, 4);
            OKButton.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            OKButton.Name = "OKButton";
            OKButton.Size = new System.Drawing.Size(98, 29);
            OKButton.TabIndex = 3;
            OKButton.Text = "OK";
            OKButton.UseVisualStyleBackColor = true;
            // 
            // MergeForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(484, 461);
            Controls.Add(TableLayoutPanel1);
            Name = "MergeForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Merge";
            FormClosed += MergeForm_FormClosed;
            TableLayoutPanel1.ResumeLayout(false);
            SplitContainer.Panel1.ResumeLayout(false);
            SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SplitContainer).EndInit();
            SplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)WindowsMediaPlayer).EndInit();
            ((System.ComponentModel.ISupportInitialize)FilesDataGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)FilesBindingSource).EndInit();
            TableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel2;
        private AxWMPLib.AxWindowsMediaPlayer WindowsMediaPlayer;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.DataGridView FilesDataGridView;
        private System.Windows.Forms.BindingSource FilesBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FilesSizeDataGridViewTextBoxColumn;
    }
}