using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace ChatRoomRecorder
{
    public partial class MainForm : Form
    {
        #region MainForm creating and closing

        public MainForm()
        {
            InitializeComponent();

            Text = string.Format("{0} {1}", Assembly.GetEntryAssembly().GetName().Name, Assembly.GetEntryAssembly().GetName().Version.ToString(3));

            LicenseTextBox.Text = string.Format("{0}\r\n\r\n{1}\r\n\r\n{2}",
                LicenseTextBox.Text,
                ((AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyDescriptionAttribute))).Description,
                ((AssemblyCopyrightAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyCopyrightAttribute))).Copyright);

            DefaultActionComboBox.DataSource = new ChatRoomAction[] { ChatRoomAction.None, ChatRoomAction.Monitor, ChatRoomAction.Record };
            DefaultResolutionComboBox.DataSource = ChatRoomResolution.CommonResolutions;

            c_defaultSettings.CopyTo(_settings);
            SettingsBindingSource.DataSource = _settings;
            OutputDirectoryTextBox.DataBindings.Add("Text", SettingsBindingSource, c_outputDirectorySettingName);
            FFmpegPathTextBox.DataBindings.Add("Text", SettingsBindingSource, c_ffmpegPathSettingName);
            StreamlinkPathTextBox.DataBindings.Add("Text", SettingsBindingSource, c_streamlinkPathSettingName);
            ChaturbateConcurrentUpdatesNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_chaturbateConcurrentUpdatesSettingName);
            BongaCamsConcurrentUpdatesNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_bongaCamsConcurrentUpdatesSettingName);
            StripchatConcurrentUpdatesNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_stripchatConcurrentUpdatesSettingName);
            UpdateIntervalNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_updateIntervalSettingName);
            DefaultActionComboBox.DataBindings.Add("SelectedItem", SettingsBindingSource, c_defaultActionSettingName);
            DefaultResolutionComboBox.DataBindings.Add("SelectedItem", SettingsBindingSource, c_defaultResolutionSettingName);

            ChatRoomsBindingSource.DataSource = _chatRoomsList;
            ActionColumn.DataSource = new ChatRoomAction[] { ChatRoomAction.None, ChatRoomAction.Monitor, ChatRoomAction.Record };
            ResolutionColumn.DataSource = ChatRoomResolution.CommonResolutions;

            FilesBindingSource.DataSource = _filesList;

            foreach (ChatRoomWebsite website in Enum.GetValues(typeof(ChatRoomWebsite)))
            {
                _lastUpdates.Add(website, DateTime.Now);
            }

            ReadData();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (_lock)
            {
                if (!_isExiting)
                {
                    TabControl.Enabled = false;

                    ChatRoomsUpdateTimer.Stop();
                    FormCloseTimer.Start();

                    Stack<Category> cs = new Stack<Category>();
                    cs.Push(_root);
                    while (cs.Count > 0)
                    {
                        Category parent = cs.Pop();
                        foreach (Category category in parent.Categories)
                        {
                            cs.Push(category);
                        }
                        foreach (ChatRoom chatRoom in parent.ChatRooms)
                        {
                            chatRoom.Dispose();
                        }
                    }

                    if (_connection != null)
                    {
                        _connection.Close();
                    }

                    _isExiting = true;

                    e.Cancel = true;
                }
                else
                {
                    if (ChatRoom.TotalCount > 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        FormCloseTimer.Stop();
                    }
                }
            }
        }

        private void FormCloseTimer_Tick(object sender, EventArgs e)
        {
            if (Monitor.TryEnter(_lock))
            {
                if (FormCloseTimer.Enabled)
                {
                    Close();
                }

                Monitor.Exit(_lock);
            }
        }

        #endregion

        #region Chat rooms updating

        private void ChatRoomsUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (Monitor.TryEnter(_lock))
            {
                if (ChatRoomsUpdateTimer.Enabled)
                {
                    Dictionary<ChatRoomWebsite, int> maxUpdateCounts = new()
                    {
                        { ChatRoomWebsite.Chaturbate, _settings.ChaturbateConcurrentUpdates },
                        { ChatRoomWebsite.BongaCams, _settings.BongaCamsConcurrentUpdates },
                        { ChatRoomWebsite.Stripchat, _settings.StripchatConcurrentUpdates }
                    };

                    Dictionary<ChatRoomWebsite, int> curUpdateCounts = new();
                    foreach (ChatRoomWebsite website in Enum.GetValues(typeof(ChatRoomWebsite)))
                    {
                        curUpdateCounts.Add(website, 0);
                    }

                    Stack<Category> cs = new Stack<Category>();
                    cs.Push(_root);
                    while (cs.Count > 0)
                    {
                        Category parent = cs.Pop();
                        foreach (Category category in parent.Categories)
                        {
                            cs.Push(category);
                        }
                        foreach (ChatRoom chatRoom in parent.ChatRooms)
                        {
                            if (chatRoom.Action != ChatRoomAction.None)
                            {
                                if (curUpdateCounts[chatRoom.Website] < maxUpdateCounts[chatRoom.Website] && chatRoom.LastUpdated < _lastUpdates[chatRoom.Website])
                                {
                                    chatRoom.Update();
                                    curUpdateCounts[chatRoom.Website]++;
                                }
                            }
                            else
                            {
                                if (chatRoom.Status != ChatRoomStatus.Unknown)
                                {
                                    chatRoom.Update();
                                }
                            }
                        }
                    }

                    foreach (ChatRoomWebsite website in Enum.GetValues(typeof(ChatRoomWebsite)))
                    {
                        if (curUpdateCounts[website] < maxUpdateCounts[website])
                        {
                            _lastUpdates[website] = DateTime.Now;
                        }
                    }
                }

                Monitor.Exit(_lock);
            }
        }

        private void ChatRoom_UpdateCompleted(object sender, EventArgs e)
        {
            lock (_lock)
            {
                int index = _chatRoomsList.IndexOf((ChatRoom)sender);
                if (index != -1)
                {
                    ChatRoomsDataGridView.InvalidateRow(index);
                }

                WriteData(c_chatRoomsTableName, 0, (ChatRoom)sender, null, null);
            }
        }

        #endregion

        #region Web browser functioning

        private void WebView2_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            lock (_lock)
            {
                if (e.IsSuccess)
                {
                    ChatRoomsUpdateTimer.Start();

                    TabControl.Enabled = true;
                }
            }
        }

        private void WebView2_ContentLoading(object sender, Microsoft.Web.WebView2.Core.CoreWebView2ContentLoadingEventArgs e)
        {
            AddressBarTextBox.Text = WebView2.Source.ToString();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            WebView2.CoreWebView2.GoBack();
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            WebView2.CoreWebView2.GoForward();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            WebView2.CoreWebView2.Stop();
        }

        private void NavigateButton_Click(object sender, EventArgs e)
        {
            NavigateToUrl();
        }

        private void AddressBarTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NavigateToUrl();
            }
        }

        private void NavigateToUrl()
        {
            if (Regex.Matches(AddressBarTextBox.Text, "^(http[s]?:.*)$", RegexOptions.IgnoreCase).Count > 0)
            {
                WebView2.CoreWebView2.Navigate(AddressBarTextBox.Text);
            }
            else
            {
                MessageBox.Show(c_unsupportedUrlMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Chat rooms adding and filtering

        private void UrlTextBox_TextChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                _chatRoomsList.Filter = UrlTextBox.Text;
            }
        }

        private void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            lock (_lock)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    AddChatRoom();
                }
            }
        }

        private void PlusButton_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                AddChatRoom();
            }
        }

        private void AddChatRoom()
        {
            Tuple<ChatRoomWebsite, string, string> parsedUrl = ChatRoom.ParseUrl(UrlTextBox.Text);

            if (parsedUrl != null)
            {
                bool duplicate = false;

                Stack<Category> cs = new Stack<Category>();
                cs.Push(_root);
                while (cs.Count > 0 && !duplicate)
                {
                    Category parent = cs.Pop();
                    foreach (Category category in parent.Categories)
                    {
                        cs.Push(category);
                    }
                    foreach (ChatRoom chatRoom in parent.ChatRooms)
                    {
                        if (chatRoom.Website == parsedUrl.Item1 && chatRoom.Name == parsedUrl.Item2)
                        {
                            duplicate = true;
                            break;
                        }
                    }
                }

                if (!duplicate)
                {
                    ChatRoom chatRoom = new(parsedUrl.Item3);
                    chatRoom.OutputDirectory = _settings.OutputDirectory;
                    chatRoom.FFmpegPath = _settings.FFmpegPath;
                    chatRoom.StreamlinkPath = _settings.StreamlinkPath;
                    chatRoom.Action = _settings.DefaultAction;
                    chatRoom.PreferredResolution = _settings.DefaultResolution;
                    chatRoom.UpdateCompleted += ChatRoom_UpdateCompleted;

                    Category category = (Category)CategoriesTreeView.SelectedNode.Tag;
                    category.ChatRooms.Add(chatRoom);

                    WriteData(c_chatRoomsTableName, 1, chatRoom, category, null);

                    TreeNode node = CategoriesTreeView.SelectedNode;
                    CategoriesTreeView.SelectedNode = null;
                    CategoriesTreeView.SelectedNode = node;
                }
                else
                {
                    MessageBox.Show(c_duplicateChatRoomMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(c_unsupportedWebSiteMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region CategoriesTreeView functioning

        private void CategoriesTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 0;
        }

        private void CategoriesTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 1;
        }

        private void CategoriesTreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag == _root)
            {
                e.CancelEdit = true;
            }
        }

        private void CategoriesTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            lock (_lock)
            {
                if (e.Label != null)
                {
                    e.Node.Text = e.Label;

                    RenameCategory();
                }
            }
        }

        private void CategoriesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            lock (_lock)
            {
                ShowChatRoom();
            }
        }

        private void AddCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                AddCategory();
            }
        }

        private void RemoveCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                RemoveCategory();
            }
        }

        private void RenameCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CategoriesTreeView.SelectedNode.BeginEdit();
        }

        private void CategoriesTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                CategoriesTreeView.SelectedNode.BeginEdit();
            }
        }

        private void CategoriesContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            AddCategoryToolStripMenuItem.Enabled = CategoriesTreeView.SelectedNode != null;
            RemoveCategoryToolStripMenuItem.Enabled = CategoriesTreeView.SelectedNode != null && CategoriesTreeView.SelectedNode.Tag != _root;
            RenameCategoryToolStripMenuItem.Enabled = CategoriesTreeView.SelectedNode != null && CategoriesTreeView.SelectedNode.Tag != _root;
        }

        private void ShowChatRoom()
        {
            _chatRoomsList.RemoveAll();
            _chatRoomsList.Add(((Category)CategoriesTreeView.SelectedNode.Tag).ChatRooms);
        }

        private void AddCategory()
        {
            Category parent = (Category)CategoriesTreeView.SelectedNode.Tag;
            Category category = new Category(0, "New category");
            parent.Categories.Add(category);

            WriteData(c_categoriesTableName, 1, null, category, parent);

            TreeNode node = new TreeNode() { Text = category.Name, Tag = category };
            CategoriesTreeView.SelectedNode.Nodes.Add(node);
            CategoriesTreeView.SelectedNode = node;
        }

        private void RemoveCategory()
        {
            if (MessageBox.Show(c_confirmCategoryRemovingMessage, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                TreeNode currentNode = CategoriesTreeView.SelectedNode;
                TreeNode parentNode = currentNode.Parent;

                Category currentCategory = (Category)currentNode.Tag;
                Category parentCategory = (Category)parentNode.Tag;

                if (currentCategory.Categories.Count == 0 && currentCategory.ChatRooms.Count == 0)
                {
                    WriteData(c_categoriesTableName, -1, null, currentCategory, null);

                    parentCategory.Categories.Remove(currentCategory);

                    parentNode.Nodes.Remove(currentNode);
                    if (parentNode.Nodes.Count == 0)
                    {
                        parentNode.Collapse();
                    }
                }
                else
                {
                    MessageBox.Show(c_categoryNotEmptyMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RenameCategory()
        {
            Category category = (Category)CategoriesTreeView.SelectedNode.Tag;

            category.Name = CategoriesTreeView.SelectedNode.Text;

            WriteData(c_categoriesTableName, 0, null, category, null);
        }

        #endregion

        #region ChatRoomsDataGridView functioning

        private void ChatRoomsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            lock (_lock)
            {
                if (e.RowIndex >= 0)
                {
                    ChatRoomsDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(ChatRoomsDataGridView.DefaultCellStyle.Font, FontStyle.Bold);
                }
            }
        }

        private void ChatRoomsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            lock (_lock)
            {
                if (e.RowIndex >= 0)
                {
                    ChatRoomsDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(ChatRoomsDataGridView.DefaultCellStyle.Font, FontStyle.Regular);
                }
            }
        }

        private void ChatRoomsDataGridView_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            lock (_lock)
            {
                if (e.RowIndex >= 0)
                {
                    ChatRoomsDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(ChatRoomsDataGridView.DefaultCellStyle.Font, FontStyle.Regular);
                }
            }
        }

        private void ChatRoomsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            lock (_lock)
            {
                if (e.RowIndex >= 0)
                {
                    WriteData(c_chatRoomsTableName, 0, _chatRoomsList[e.RowIndex], null, null);
                }
            }
        }

        private void ChatRoomsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                ShowFile();

                ShowThumbnail();
            }
        }

        private void ShowFile()
        {
            _filesList.Clear();

            foreach (DataGridViewRow selectedRow in ChatRoomsDataGridView.SelectedRows)
            {
                try
                {
                    ChatRoom chatRoom = _chatRoomsList[selectedRow.Index];
                    DirectoryInfo directory = new(_settings.OutputDirectory);
                    FileInfo[] files = directory.GetFiles(string.Format("{0} {1} *.ts", chatRoom.Website, chatRoom.Name));
                    foreach (FileInfo file in files)
                    {
                        file.Refresh();
                        _filesList.Add(file);
                    }
                }
                catch (Exception)
                {
                    //do nothing
                }
            }
        }

        private void ShowThumbnail()
        {
            ThumbnailPictureBox.Image = null;

            if (ChatRoomsDataGridView.SelectedRows.Count == 1)
            {
                try
                {
                    ChatRoom chatRoom = _chatRoomsList[ChatRoomsDataGridView.SelectedRows[0].Index];
                    string thumbnailDirectory = string.Format("{0}\\{1}", new FileInfo(Environment.ProcessPath).Directory.FullName, c_thumbnailsDirectoryName);
                    string thumbnailPath = string.Format("{0}\\{1} {2}.jpg", thumbnailDirectory, chatRoom.Website, chatRoom.Name);

                    if (!Directory.Exists(thumbnailDirectory))
                    {
                        Directory.CreateDirectory(thumbnailDirectory);
                    }

                    if (!File.Exists(thumbnailPath))
                    {
                        if (_filesList.Count > 0)
                        {
                            FileInfo file = _filesList[_random.Next(_filesList.Count)];
                            DateTime time = DateTime.MinValue.AddSeconds((double)_random.Next((int)(file.Length / 1000000 % 86400)));
                            string ffmpegPath = _settings.FFmpegPath;
                            string ffmpegArgs = string.Format("-ss \"{0}\" -i \"{1}\" -frames:v 1 -update 1 -q:v 1 -vf scale=320:-1 -y \"{2}\"",
                                time.ToString("HH:mm:ss"), file.FullName, thumbnailPath);
                            Process.Start(new ProcessStartInfo()
                            {
                                FileName = ffmpegPath,
                                Arguments = ffmpegArgs,
                                UseShellExecute = false,
                                LoadUserProfile = false,
                                CreateNoWindow = true
                            });
                        }
                    }

                    if (File.Exists(thumbnailPath))
                    {
                        using (FileStream fs = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read))
                        {
                            ThumbnailPictureBox.Image = new Bitmap(fs);
                        }
                    }
                }
                catch (Exception)
                {
                    //do nothing
                }
            }
        }

        private void RemoveChatRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                RemoveChatRoom();
            }
        }

        private void ChatRoomsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            RemoveChatRoomToolStripMenuItem.Enabled = ChatRoomsDataGridView.SelectedRows.Count > 0;
        }

        private void RemoveChatRoom()
        {
            if (MessageBox.Show(c_confirmChatRoomsRemovingMessage, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                List<ChatRoom> chatRooms = new List<ChatRoom>();

                foreach (DataGridViewRow row in ChatRoomsDataGridView.SelectedRows)
                {
                    chatRooms.Add(_chatRoomsList[row.Index]);
                }

                foreach (ChatRoom chatRoom in chatRooms)
                {
                    WriteData(c_chatRoomsTableName, -1, chatRoom, null, null);

                    Stack<Category> cs = new Stack<Category>();
                    cs.Push(_root);
                    while (cs.Count > 0)
                    {
                        if (!cs.Peek().ChatRooms.Remove(chatRoom))
                        {
                            foreach (Category category in cs.Pop().Categories)
                            {
                                cs.Push(category);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    chatRoom.Dispose();
                }

                TreeNode node = CategoriesTreeView.SelectedNode;
                CategoriesTreeView.SelectedNode = null;
                CategoriesTreeView.SelectedNode = node;
            }
        }

        #endregion

        #region FilesDataGridView functioning

        private void FilesDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (_lock)
            {
                OpenFile();
            }
        }

        private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                OpenFile();
            }
        }

        private void RemoveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                RemoveFile();
            }
        }

        private void FilesContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            OpenFileToolStripMenuItem.Enabled = FilesDataGridView.SelectedRows.Count == 1;
            RemoveFileToolStripMenuItem.Enabled = FilesDataGridView.SelectedRows.Count > 0;
        }

        private void OpenFile()
        {
            if (FilesDataGridView.SelectedRows.Count == 1)
            {
                try
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        UseShellExecute = true,
                        FileName = _filesList[FilesDataGridView.SelectedRows[0].Index].FullName
                    });
                }
                catch (Exception)
                {
                    MessageBox.Show(c_fileOpeningErrorMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RemoveFile()
        {
            if (MessageBox.Show(c_confirmFilesRemovingMessage, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    List<FileInfo> files = new List<FileInfo>();

                    foreach (DataGridViewRow row in FilesDataGridView.SelectedRows)
                    {
                        files.Add(_filesList[row.Index]);
                    }

                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                        _filesList.Remove(file);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(c_fileRemovingErrorMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region ThumbnailPictureBox functioning

        private void RemoveThumbnailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                RemoveThumbnail();
            }
        }

        private void ThumbnailContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            RemoveThumbnailToolStripMenuItem.Enabled = ThumbnailPictureBox.Image != null;
        }

        private void RemoveThumbnail()
        {
            if (MessageBox.Show(c_confirmThumbnailRemovingMessage, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (ChatRoomsDataGridView.SelectedRows.Count > 0)
                {
                    try
                    {
                        ChatRoom chatRoom = _chatRoomsList[ChatRoomsDataGridView.SelectedRows[0].Index];
                        string thumbnailDirectory = string.Format("{0}\\{1}", new FileInfo(Environment.ProcessPath).Directory.FullName, c_thumbnailsDirectoryName);
                        string thumbnailPath = string.Format("{0}\\{1} {2}.jpg", thumbnailDirectory, chatRoom.Website, chatRoom.Name);

                        File.Delete(thumbnailPath);

                        ThumbnailPictureBox.Image = null;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(c_thumbnailRemovingErrorMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region Drag and drop

        private void ChatRoomsDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex != -1 && e.ColumnIndex != 2 && e.ColumnIndex != 3)
                {
                    List<DataGridViewRow> selectedRows = new();
                    foreach (DataGridViewRow selectedRow in ChatRoomsDataGridView.SelectedRows)
                    {
                        selectedRows.Add(selectedRow);
                    }
                    ChatRoomsDataGridView.DoDragDrop(selectedRows, DragDropEffects.Move);
                }
            }
        }

        private void CategoriesTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (CategoriesTreeView.GetNodeAt(new Point(e.X, e.Y)) == CategoriesTreeView.SelectedNode && CategoriesTreeView.SelectedNode != CategoriesTreeView.Nodes[0])
                {
                    CategoriesTreeView.DoDragDrop(CategoriesTreeView.SelectedNode, DragDropEffects.Move);
                }
            }
        }

        private void CategoriesTreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(List<DataGridViewRow>)) || e.Data.GetDataPresent(typeof(TreeNode)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void CategoriesTreeView_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                TreeNode curNode = CategoriesTreeView.SelectedNode;
                TreeNode dstNode = CategoriesTreeView.GetNodeAt(CategoriesTreeView.PointToClient(new Point(e.X, e.Y)));

                Stack<TreeNode> ns = new Stack<TreeNode>();
                ns.Push(curNode);
                while (ns.Count > 0)
                {
                    TreeNodeCollection nodes = ns.Pop().Nodes;

                    if (nodes.Contains(dstNode))
                    {
                        e.Effect = DragDropEffects.None;
                        break;
                    }

                    foreach (TreeNode node in nodes)
                    {
                        ns.Push(node);
                    }
                }
            }
        }

        private void CategoriesTreeView_DragDrop(object sender, DragEventArgs e)
        {
            lock (_lock)
            {
                TreeNode oldNode = CategoriesTreeView.SelectedNode;
                TreeNode newNode = CategoriesTreeView.GetNodeAt(CategoriesTreeView.PointToClient(new Point(e.X, e.Y)));

                if (newNode != oldNode && newNode != null)
                {
                    Category oldCategory = (Category)oldNode.Tag;
                    Category newCategory = (Category)newNode.Tag;

                    if (e.Data.GetDataPresent(typeof(List<DataGridViewRow>)))
                    {
                        List<DataGridViewRow> selectedRows = (List<DataGridViewRow>)e.Data.GetData(typeof(List<DataGridViewRow>));
                        foreach (DataGridViewRow selectedRow in selectedRows)
                        {
                            ChatRoom chatRoom = _chatRoomsList[selectedRow.Index];

                            WriteData(c_chatRoomsTableName, 0, chatRoom, null, newCategory);

                            oldCategory.ChatRooms.Remove(chatRoom);
                            newCategory.ChatRooms.Add(chatRoom);
                        }

                        CategoriesTreeView.SelectedNode = null;
                        CategoriesTreeView.SelectedNode = oldNode;
                    }
                    else if (e.Data.GetDataPresent(typeof(TreeNode)))
                    {
                        WriteData(c_categoriesTableName, 0, null, oldCategory, newCategory);

                        TreeNode parentNode = oldNode.Parent;
                        Category parentCategory = (Category)parentNode.Tag;

                        parentCategory.Categories.Remove(oldCategory);
                        newCategory.Categories.Add(oldCategory);

                        if (parentNode.Nodes.Count == 1)
                        {
                            parentNode.Collapse();
                        }
                        newNode.Expand();

                        parentNode.Nodes.Remove(oldNode);
                        newNode.Nodes.Add(oldNode);

                        CategoriesTreeView.SelectedNode = null;
                        CategoriesTreeView.SelectedNode = oldNode;
                    }
                }
            }
        }

        #endregion

        #region Settings managing

        private void OutputDirectoryTextBox_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _settings.OutputDirectory = fbd.SelectedPath;
            }
        }

        private void FFmpegPathTextBox_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new() { Multiselect = false, Filter = string.Format("{0}|{0}", c_ffmpegBinaryName) };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _settings.FFmpegPath = ofd.FileName;
            }
        }

        private void StreamlinkPathTextBox_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new() { Multiselect = false, Filter = string.Format("{0}|{0}", c_streamlinkBinaryName) };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _settings.StreamlinkPath = ofd.FileName;
            }
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            lock (_lock)
            {

                switch (e.PropertyName)
                {
                    case c_outputDirectorySettingName:
                        {
                            Stack<Category> cs = new Stack<Category>();
                            cs.Push(_root);
                            while (cs.Count > 0)
                            {
                                Category parent = cs.Pop();
                                foreach (Category category in parent.Categories)
                                {
                                    cs.Push(category);
                                }
                                foreach (ChatRoom chatRoom in parent.ChatRooms)
                                {
                                    chatRoom.OutputDirectory = _settings.OutputDirectory;
                                }
                            }
                        }
                        break;

                    case c_ffmpegPathSettingName:
                        {
                            Stack<Category> cs = new Stack<Category>();
                            cs.Push(_root);
                            while (cs.Count > 0)
                            {
                                Category parent = cs.Pop();
                                foreach (Category category in parent.Categories)
                                {
                                    cs.Push(category);
                                }
                                foreach (ChatRoom chatRoom in parent.ChatRooms)
                                {
                                    chatRoom.FFmpegPath = _settings.FFmpegPath;
                                }
                            }
                        }
                        break;

                    case c_streamlinkPathSettingName:
                        {
                            Stack<Category> cs = new Stack<Category>();
                            cs.Push(_root);
                            while (cs.Count > 0)
                            {
                                Category parent = cs.Pop();
                                foreach (Category category in parent.Categories)
                                {
                                    cs.Push(category);
                                }
                                foreach (ChatRoom chatRoom in parent.ChatRooms)
                                {
                                    chatRoom.StreamlinkPath = _settings.StreamlinkPath;
                                }
                            }
                        }
                        break;

                    case c_chaturbateConcurrentUpdatesSettingName:
                        //do nothing
                        break;

                    case c_bongaCamsConcurrentUpdatesSettingName:
                        //do nothing
                        break;

                    case c_stripchatConcurrentUpdatesSettingName:
                        //do nothing
                        break;

                    case c_updateIntervalSettingName:
                        ChatRoomsUpdateTimer.Interval = _settings.UpdateInterval * 1000;
                        break;

                    case c_defaultActionSettingName:
                        //do nothing
                        break;

                    case c_defaultResolutionSettingName:
                        //do nothing
                        break;
                }

                WriteData(c_settingsTableName, 0, null, null, null);
            }
        }

        #endregion

        #region Database interacting

        private void ReadData()
        {
            //update schema

            try
            {
                _connection = new SqliteConnection(string.Format("data source={0}", c_databaseFileName));
                _connection.Open();

                _command = _connection.CreateCommand();
                _command.CommandText = "pragma foreign_keys = 1;";
                _command.ExecuteNonQuery();

                bool schemaIsUpdated = false;
                do
                {
                    _command.CommandText = "select * from pragma_user_version;";
                    int version = Convert.ToInt32(_command.ExecuteScalar());
                    switch (version)
                    {
                        case 0:
                            _command.CommandText =
                                string.Format("create table {0} ({1} text, {2} text, {3} integer, {4} integer, {5} integer, {6} integer, {7} integer, {8} integer);",
                                    c_settingsTableName,
                                    c_settingsTableOutputDirectoryColumnName,
                                    c_settingsTableFFmpegPathColumnName,
                                    c_settingsTableChaturbateConcurrentUpdatesColumnName,
                                    c_settingsTableBongaCamsConcurrentUpdatesColumnName,
                                    c_settingsTableStripchatConcurrentUpdatesColumnName,
                                    c_settingsTableUpdateIntervalColumnName,
                                    c_settingsTableDefaultActionColumnName,
                                    c_settingsTableDefaultResolutionColumnName) +
                                string.Format("create table {0} ({1} text primary key, {2} integer, {3} integer, {4} integer, {5} integer);",
                                    c_chatRoomsTableName,
                                    c_chatRoomsTableUrlColumnName,
                                    c_chatRoomsTableActionColumnName,
                                    c_chatRoomsTableResolutionColumnName,
                                    c_chatRoomsTableUpdatedColumnName,
                                    c_chatRoomsTableSeenColumnName) +
                                "pragma user_version = 1;";
                            _command.ExecuteNonQuery();
                            break;
                        case 1:
                            _command.CommandText =
                                string.Format("alter table {0} add {1} text;",
                                    c_settingsTableName,
                                    c_settingsTableStreamlinkPathColumnName) +
                                "pragma user_version = 2;";
                            _command.ExecuteNonQuery();
                            break;
                        case 2:
                            _command.CommandText =
                                string.Format("create table {0} ({1} integer primary key autoincrement, {2} text, {3} integer, foreign key ({3}) references {0} ({1}));",
                                    c_categoriesTableName,
                                    c_categoriesTableIdColumnName,
                                    c_categoriesTableNameColumnName,
                                    c_categoriesTableCategoryColumnName) +
                                string.Format("insert into {0} ({1}) values ('{2}');",
                                    c_categoriesTableName,
                                    c_categoriesTableNameColumnName, c_rootCategoryName) +
                                string.Format("create temporary table temp as select {0}, {1}, {2}, {3}, {4} from {5};",
                                    c_chatRoomsTableUrlColumnName,
                                    c_chatRoomsTableActionColumnName,
                                    c_chatRoomsTableResolutionColumnName,
                                    c_chatRoomsTableUpdatedColumnName,
                                    c_chatRoomsTableSeenColumnName,
                                    c_chatRoomsTableName) +
                                string.Format("drop table {0};",
                                    c_chatRoomsTableName) +
                                string.Format("create table {0} ({1} integer primary key autoincrement, {2} text, {3} integer, {4} integer, {5} integer, {6} integer, {7} integer, foreign key ({7}) references {8} ({9}));",
                                    c_chatRoomsTableName,
                                    c_chatRoomsTableIdColumnName,
                                    c_chatRoomsTableUrlColumnName,
                                    c_chatRoomsTableActionColumnName,
                                    c_chatRoomsTableResolutionColumnName,
                                    c_chatRoomsTableUpdatedColumnName,
                                    c_chatRoomsTableSeenColumnName,
                                    c_chatRoomsTableCategoryColumnName,
                                    c_categoriesTableName,
                                    c_categoriesTableIdColumnName
                                    ) +
                                string.Format("insert into {0} ({1}, {2}, {3}, {4}, {5}) select {1}, {2}, {3}, {4}, {5} from temp;",
                                    c_chatRoomsTableName,
                                    c_chatRoomsTableUrlColumnName,
                                    c_chatRoomsTableActionColumnName,
                                    c_chatRoomsTableResolutionColumnName,
                                    c_chatRoomsTableUpdatedColumnName,
                                    c_chatRoomsTableSeenColumnName) +
                                string.Format("update {0} set {1} = 1;",
                                    c_chatRoomsTableName,
                                    c_chatRoomsTableCategoryColumnName
                                    ) +
                                "drop table temp;" +
                                "pragma user_version = 3;";
                            _command.ExecuteNonQuery();
                            break;
                        default:
                            schemaIsUpdated = true;
                            break;
                    }
                } while (!schemaIsUpdated);
            }
            catch (Exception)
            {
                //do nothing
            }

            //read settings

            try
            {
                _command.CommandText = string.Format("select * from {0}", c_settingsTableName);
                using (SqliteDataReader reader = _command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _settings.OutputDirectory = reader.GetString(c_settingsTableOutputDirectoryColumnName);
                        _settings.FFmpegPath = reader.GetString(c_settingsTableFFmpegPathColumnName);
                        _settings.StreamlinkPath = reader.GetString(c_settingsTableStreamlinkPathColumnName);
                        _settings.ChaturbateConcurrentUpdates = reader.GetInt32(c_settingsTableChaturbateConcurrentUpdatesColumnName);
                        _settings.BongaCamsConcurrentUpdates = reader.GetInt32(c_settingsTableBongaCamsConcurrentUpdatesColumnName);
                        _settings.StripchatConcurrentUpdates = reader.GetInt32(c_settingsTableStripchatConcurrentUpdatesColumnName);
                        _settings.UpdateInterval = reader.GetInt32(c_settingsTableUpdateIntervalColumnName);
                        _settings.DefaultAction = (ChatRoomAction)reader.GetInt32(c_settingsTableDefaultActionColumnName);
                        _settings.DefaultResolution = ChatRoomResolution.Parse(reader.GetInt32(c_settingsTableDefaultResolutionColumnName));
                    }
                }
            }
            catch (Exception)
            {
                c_defaultSettings.CopyTo(_settings);
            }
            finally
            {
                _settings.PropertyChanged += Settings_PropertyChanged;
            }

            //read categories and chat rooms

            try
            {
                Stack<TreeNode> ns = new Stack<TreeNode>();
                _root = new Category(1, c_rootCategoryName);
                TreeNode root = new TreeNode() { Text = _root.Name, Tag = _root };
                CategoriesTreeView.Nodes.Add(root);
                ns.Push(root);
                while (ns.Count > 0)
                {
                    TreeNode curNode = ns.Pop();
                    Category curCategory = (Category)curNode.Tag;

                    _command.CommandText = string.Format("select * from {0} where {1}={2};", c_categoriesTableName, c_categoriesTableCategoryColumnName, curCategory.ID);
                    using (SqliteDataReader reader = _command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Category newCategory = new Category(reader.GetInt32(c_categoriesTableIdColumnName), reader.GetString(c_categoriesTableNameColumnName));
                            TreeNode newNode = new TreeNode() { Text = newCategory.Name, Tag = newCategory };
                            curNode.Nodes.Add(newNode);
                            ns.Push(newNode);

                            curCategory.Categories.Add(newCategory);
                        }
                    }

                    _command.CommandText = string.Format("select * from {0} where {1}={2};", c_chatRoomsTableName, c_chatRoomsTableCategoryColumnName, curCategory.ID);
                    using (SqliteDataReader reader = _command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(c_chatRoomsTableIdColumnName);
                            string url = reader.GetString(c_chatRoomsTableUrlColumnName);
                            ChatRoomAction action = (ChatRoomAction)reader.GetInt32(c_chatRoomsTableActionColumnName);
                            ChatRoomResolution resolution = ChatRoomResolution.Parse(reader.GetInt32(c_chatRoomsTableResolutionColumnName));
                            DateTime updated = new(reader.GetInt64(c_chatRoomsTableUpdatedColumnName));
                            DateTime seen = new(reader.GetInt64(c_chatRoomsTableSeenColumnName));

                            ChatRoom chatRoom = new(url);
                            chatRoom.OutputDirectory = _settings.OutputDirectory;
                            chatRoom.FFmpegPath = _settings.FFmpegPath;
                            chatRoom.StreamlinkPath = _settings.StreamlinkPath;
                            chatRoom.ID = id;
                            chatRoom.Action = action;
                            chatRoom.PreferredResolution = resolution;
                            chatRoom.LastUpdated = updated;
                            chatRoom.LastSeen = seen;
                            chatRoom.UpdateCompleted += ChatRoom_UpdateCompleted;

                            curCategory.ChatRooms.Add(chatRoom);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Stack<Category> cs = new Stack<Category>();
                cs.Push(_root);
                while (cs.Count > 0)
                {
                    Category parent = cs.Pop();
                    foreach (Category category in parent.Categories)
                    {
                        cs.Push(category);
                    }
                    foreach (ChatRoom chatRoom in parent.ChatRooms)
                    {
                        chatRoom.Dispose();
                    }
                }

                _root = new Category(1, c_rootCategoryName);

                CategoriesTreeView.Nodes.Clear();
                CategoriesTreeView.Nodes.Add(new TreeNode() { Text = _root.Name, Tag = _root });
            }

            CategoriesTreeView.SelectedNode = CategoriesTreeView.Nodes[0];
        }

        private void WriteData(string table, int operation, ChatRoom chatRoom, Category category, Category parent)
        {
            try
            {
                switch (table)
                {
                    case c_settingsTableName:

                        _command.CommandText = string.Format("delete from {0}; insert into {0} ({1},{3},{5},{7},{9},{11},{13},{15},{17}) values ('{2}','{4}','{6}','{8}','{10}','{12}','{14}','{16}','{18}');",
                            c_settingsTableName,
                            c_settingsTableOutputDirectoryColumnName, _settings.OutputDirectory,
                            c_settingsTableFFmpegPathColumnName, _settings.FFmpegPath,
                            c_settingsTableStreamlinkPathColumnName, _settings.StreamlinkPath,
                            c_settingsTableChaturbateConcurrentUpdatesColumnName, _settings.ChaturbateConcurrentUpdates,
                            c_settingsTableBongaCamsConcurrentUpdatesColumnName, _settings.BongaCamsConcurrentUpdates,
                            c_settingsTableStripchatConcurrentUpdatesColumnName, _settings.StripchatConcurrentUpdates,
                            c_settingsTableUpdateIntervalColumnName, _settings.UpdateInterval,
                            c_settingsTableDefaultActionColumnName, (int)_settings.DefaultAction,
                            c_settingsTableDefaultResolutionColumnName, _settings.DefaultResolution.ToInt32());
                        _command.ExecuteNonQuery();
                        break;

                    case c_chatRoomsTableName:

                        switch (operation)
                        {
                            case -1:
                                _command.CommandText = string.Format("delete from {0} where {1}='{2}';",
                                    c_chatRoomsTableName,
                                    c_chatRoomsTableIdColumnName, chatRoom.ID);
                                _command.ExecuteNonQuery();
                                break;

                            case 0:
                                if (parent == null)
                                {
                                    _command.CommandText = string.Format("update {0} set {1}='{2}', {3}='{4}', {5}='{6}', {7}='{8}' where {9}='{10}';",
                                        c_chatRoomsTableName,
                                        c_chatRoomsTableActionColumnName, (int)chatRoom.Action,
                                        c_chatRoomsTableResolutionColumnName, chatRoom.PreferredResolution.ToInt32(),
                                        c_chatRoomsTableUpdatedColumnName, chatRoom.LastUpdated.Ticks,
                                        c_chatRoomsTableSeenColumnName, chatRoom.LastSeen.Ticks,
                                        c_chatRoomsTableIdColumnName, chatRoom.ID);
                                }
                                else
                                {
                                    _command.CommandText = string.Format("update {0} set {1}='{2}' where {3}='{4}';",
                                        c_chatRoomsTableName,
                                        c_chatRoomsTableCategoryColumnName, parent.ID,
                                        c_chatRoomsTableIdColumnName, chatRoom.ID);
                                }
                                _command.ExecuteNonQuery();
                                break;

                            case 1:
                                _command.CommandText = string.Format("insert into {0} ({1},{3},{5},{7},{9},{11}) values ('{2}','{4}','{6}','{8}','{10}','{12}');",
                                    c_chatRoomsTableName,
                                    c_chatRoomsTableActionColumnName, (int)chatRoom.Action,
                                    c_chatRoomsTableResolutionColumnName, chatRoom.PreferredResolution.ToInt32(),
                                    c_chatRoomsTableUpdatedColumnName, chatRoom.LastUpdated.Ticks,
                                    c_chatRoomsTableSeenColumnName, chatRoom.LastSeen.Ticks,
                                    c_chatRoomsTableUrlColumnName, chatRoom.ChatRoomUrl,
                                    c_chatRoomsTableCategoryColumnName, category.ID);
                                _command.ExecuteNonQuery();

                                _command.CommandText = "select last_insert_rowid();";
                                chatRoom.ID = Convert.ToInt32(_command.ExecuteScalar());
                                break;
                        }
                        break;

                    case c_categoriesTableName:

                        switch (operation)
                        {
                            case -1:
                                _command.CommandText = string.Format("delete from {0} where {1}='{2}';",
                                    c_categoriesTableName,
                                    c_categoriesTableIdColumnName, category.ID);
                                _command.ExecuteNonQuery();
                                break;

                            case 0:
                                if (parent == null)
                                {
                                    _command.CommandText = string.Format("update {0} set {1}='{2}' where {3}='{4}';",
                                        c_categoriesTableName,
                                        c_categoriesTableNameColumnName, category.Name,
                                        c_categoriesTableIdColumnName, category.ID);
                                }
                                else
                                {
                                    _command.CommandText = string.Format("update {0} set {1}='{2}' where {3}='{4}';",
                                        c_categoriesTableName,
                                        c_categoriesTableCategoryColumnName, parent.ID,
                                        c_categoriesTableIdColumnName, category.ID);
                                }
                                _command.ExecuteNonQuery();
                                break;

                            case 1:
                                _command.CommandText = string.Format("insert into {0} ({1},{3}) values ('{2}','{4}');",
                                    c_categoriesTableName,
                                    c_categoriesTableNameColumnName, category.Name,
                                    c_categoriesTableCategoryColumnName, parent.ID);
                                _command.ExecuteNonQuery();

                                _command.CommandText = "select last_insert_rowid();";
                                category.ID = Convert.ToInt32(_command.ExecuteScalar());
                                break;
                        }
                        break;
                }
            }
            catch (Exception)
            {
                //do nothing
            }
        }

        #endregion

        #region Fields

        private Settings _settings = new Settings();
        private Category _root = null;
        private BindingListChatRooms _chatRoomsList = new();
        private BindingListFiles _filesList = new();
        private SqliteConnection _connection = null;
        private SqliteCommand _command = null;
        private object _lock = new();
        private Random _random = new Random();
        private Dictionary<ChatRoomWebsite, DateTime> _lastUpdates = new();
        private bool _isExiting = false;

        private readonly Settings c_defaultSettings = new Settings()
        {
            OutputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
            FFmpegPath = string.Format("{0}\\Streamlink\\ffmpeg\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), c_ffmpegBinaryName),
            StreamlinkPath = string.Format("{0}\\Streamlink\\bin\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), c_streamlinkBinaryName),
            ChaturbateConcurrentUpdates = 1,
            BongaCamsConcurrentUpdates = 1,
            StripchatConcurrentUpdates = 1,
            UpdateInterval = 5,
            DefaultAction = ChatRoomAction.None,
            DefaultResolution = ChatRoomResolution.CommonResolutions[ChatRoomResolution.FindClosest(new ChatRoomResolution(1920, 1080), ChatRoomResolution.CommonResolutions)]
        };

        private const string c_databaseFileName = "ChatRoomRecorder.db";
        private const string c_rootCategoryName = "All chat rooms";
        private const string c_ffmpegBinaryName = "ffmpeg.exe";
        private const string c_streamlinkBinaryName = "streamlink.exe";
        private const string c_thumbnailsDirectoryName = "Thumbnails";

        private const string c_settingsTableName = "Settings";
        private const string c_settingsTableOutputDirectoryColumnName = "OutputDirectory";
        private const string c_settingsTableFFmpegPathColumnName = "FFmpegPath";
        private const string c_settingsTableStreamlinkPathColumnName = "StreamlinkPath";
        private const string c_settingsTableChaturbateConcurrentUpdatesColumnName = "ChaturbateConcurrentUpdates";
        private const string c_settingsTableBongaCamsConcurrentUpdatesColumnName = "BongaCamsConcurrentUpdates";
        private const string c_settingsTableStripchatConcurrentUpdatesColumnName = "StripchatConcurrentUpdates";
        private const string c_settingsTableUpdateIntervalColumnName = "UpdateInterval";
        private const string c_settingsTableDefaultActionColumnName = "DefaultAction";
        private const string c_settingsTableDefaultResolutionColumnName = "DefaultResolution";

        private const string c_chatRoomsTableName = "ChatRooms";
        private const string c_chatRoomsTableIdColumnName = "ID";
        private const string c_chatRoomsTableUrlColumnName = "URL";
        private const string c_chatRoomsTableActionColumnName = "Action";
        private const string c_chatRoomsTableResolutionColumnName = "Resolution";
        private const string c_chatRoomsTableUpdatedColumnName = "Updated";
        private const string c_chatRoomsTableSeenColumnName = "Seen";
        private const string c_chatRoomsTableCategoryColumnName = "Category";

        private const string c_categoriesTableName = "Categories";
        private const string c_categoriesTableIdColumnName = "ID";
        private const string c_categoriesTableNameColumnName = "Name";
        private const string c_categoriesTableCategoryColumnName = "Category";

        private const string c_outputDirectorySettingName = "OutputDirectory";
        private const string c_ffmpegPathSettingName = "FFmpegPath";
        private const string c_streamlinkPathSettingName = "StreamlinkPath";
        private const string c_chaturbateConcurrentUpdatesSettingName = "ChaturbateConcurrentUpdates";
        private const string c_bongaCamsConcurrentUpdatesSettingName = "BongaCamsConcurrentUpdates";
        private const string c_stripchatConcurrentUpdatesSettingName = "StripchatConcurrentUpdates";
        private const string c_updateIntervalSettingName = "UpdateInterval";
        private const string c_defaultActionSettingName = "DefaultAction";
        private const string c_defaultResolutionSettingName = "DefaultResolution";

        private const string c_unsupportedUrlMessage = "The URL is incorrect! Only http://* and https://* URLs are supported!";
        private const string c_unsupportedWebSiteMessage = "The URL is incorrect! Supported websites are Chaturbate, BongaCams and Stripchat!";
        private const string c_duplicateChatRoomMessage = "The chat room is already in the list!";
        private const string c_confirmCategoryRemovingMessage = "Do you really want to remove selected category?";
        private const string c_categoryNotEmptyMessage = "The category is not empty!";
        private const string c_confirmChatRoomsRemovingMessage = "Do you really want to remove selected chat rooms?";
        private const string c_fileOpeningErrorMessage = "The file can't be opened!";
        private const string c_confirmFilesRemovingMessage = "Do you really want to remove selected files?";
        private const string c_fileRemovingErrorMessage = "The file can't be removed!";
        private const string c_confirmThumbnailRemovingMessage = "Do you really want to remove this thumbnail?";
        private const string c_thumbnailRemovingErrorMessage = "The thumbnail can't be removed!";

        #endregion
    }
}
