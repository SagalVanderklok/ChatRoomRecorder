namespace ChatRoomRecorder
{
    partial class UrlsForm
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
            TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            OkButton = new System.Windows.Forms.Button();
            UrlsTextBox = new System.Windows.Forms.TextBox();
            TableLayoutPanel1.SuspendLayout();
            TableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.ColumnCount = 1;
            TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel1.Controls.Add(TableLayoutPanel2, 0, 1);
            TableLayoutPanel1.Controls.Add(UrlsTextBox, 0, 0);
            TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            TableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 2;
            TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            TableLayoutPanel1.Size = new System.Drawing.Size(484, 361);
            TableLayoutPanel1.TabIndex = 0;
            // 
            // TableLayoutPanel2
            // 
            TableLayoutPanel2.ColumnCount = 2;
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TableLayoutPanel2.Controls.Add(OkButton, 1, 0);
            TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            TableLayoutPanel2.Location = new System.Drawing.Point(0, 328);
            TableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            TableLayoutPanel2.Name = "TableLayoutPanel2";
            TableLayoutPanel2.RowCount = 1;
            TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanel2.Size = new System.Drawing.Size(484, 33);
            TableLayoutPanel2.TabIndex = 0;
            // 
            // OkButton
            // 
            OkButton.Dock = System.Windows.Forms.DockStyle.Fill;
            OkButton.Location = new System.Drawing.Point(388, 2);
            OkButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 4);
            OkButton.Name = "OkButton";
            OkButton.Size = new System.Drawing.Size(92, 27);
            OkButton.TabIndex = 2;
            OkButton.Text = "OK";
            OkButton.UseVisualStyleBackColor = true;
            OkButton.Click += OkButton_Click;
            // 
            // UrlsTextBox
            // 
            UrlsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            UrlsTextBox.Location = new System.Drawing.Point(4, 4);
            UrlsTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 2);
            UrlsTextBox.Multiline = true;
            UrlsTextBox.Name = "UrlsTextBox";
            UrlsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            UrlsTextBox.Size = new System.Drawing.Size(476, 322);
            UrlsTextBox.TabIndex = 1;
            UrlsTextBox.WordWrap = false;
            // 
            // UrlsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(484, 361);
            Controls.Add(TableLayoutPanel1);
            Name = "UrlsForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "URLs";
            TableLayoutPanel1.ResumeLayout(false);
            TableLayoutPanel1.PerformLayout();
            TableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel2;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.TextBox UrlsTextBox;
    }
}