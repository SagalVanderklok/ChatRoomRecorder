using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ChatRoomRecorder
{
    public partial class MainForm : Form
    {
        #region MainForm

        public MainForm()
        {
            lock (_lock)
            {
                InitializeComponent();

                Text = string.Format("{0} {1}", Assembly.GetEntryAssembly().GetName().Name, Assembly.GetEntryAssembly().GetName().Version.ToString(3));

                ChatRoomsBindingSource.DataSource = _chatRoomsList;
                ActionColumn.DataSource = new ChatRoomAction[] { ChatRoomAction.None, ChatRoomAction.Monitor, ChatRoomAction.Record };
                ResolutionColumn.DataSource = ChatRoomResolution.CommonResolutions;

                foreach (ChatRoomAction action in Enum.GetValues(typeof(ChatRoomAction)))
                {
                    ToolStripMenuItem item = new();
                    item.Text = action.ToString();
                    item.Tag = action;
                    item.Click += SetActionChatRoomToolStripMenuItem_Click;
                    SetActionToolStripMenuItem.DropDownItems.Add(item);
                }

                foreach (ChatRoomResolution resolution in ChatRoomResolution.CommonResolutions)
                {
                    ToolStripMenuItem item = new();
                    item.Text = resolution.ToString();
                    item.Tag = resolution;
                    item.Click += SetResolutionChatRoomToolStripMenuItem_Click;
                    SetResolutionToolStripMenuItem.DropDownItems.Add(item);
                }

                FilesBindingSource.DataSource = _filesList;

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
                Flirt4FreeConcurrentUpdatesNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_flirt4FreeConcurrentUpdatesSettingName);
                CamSodaConcurrentUpdatesNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_camSodaConcurrentUpdatesSettingName);
                Cam4ConcurrentUpdatesNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_cam4ConcurrentUpdatesSettingName);
                UpdateDelayNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_updateDelaySettingName);
                UpdateIntervalNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_updateIntervalSettingName);
                DefaultActionComboBox.DataBindings.Add("SelectedItem", SettingsBindingSource, c_defaultActionSettingName);
                DefaultResolutionComboBox.DataBindings.Add("SelectedItem", SettingsBindingSource, c_defaultResolutionSettingName);
                LogSizeNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_logSizeSettingName);
                _settings.PropertyChanged += Settings_PropertyChanged;

                LicenseTextBox.Text = string.Format("{0}\r\n\r\n{1}\r\n\r\n{2}",
                    LicenseTextBox.Text,
                    ((AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyDescriptionAttribute))).Description,
                    ((AssemblyCopyrightAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyCopyrightAttribute))).Copyright);

                foreach (ChatRoomWebsite website in Enum.GetValues(typeof(ChatRoomWebsite)))
                {
                    _lastUpdates.Add(website, DateTime.Now);
                }

                ReadData();

                ShowCategories();

                MoveFiles();
            }
        }

        private void WebView2_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            lock (_lock)
            {
                if (e.IsSuccess && !_isExiting)
                {
                    ChatRoomsUpdateTimer.Start();

                    TabControl.Enabled = true;
                }
            }
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
                        foreach (Category category in parent.AllCategories)
                        {
                            cs.Push(category);
                        }
                        foreach (ChatRoom chatRoom in parent.AllChatRooms)
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
            lock (_lock)
            {
                if (FormCloseTimer.Enabled)
                {
                    Close();
                }
            }
        }

        #endregion

        #region Updating

        private void ChatRoomsUpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateChatRooms();
        }

        private void ChatRoom_UpdateCompleted(object sender, EventArgs e)
        {
            SaveChatRoom((ChatRoom)sender, true);

            UpdateLog((ChatRoom)sender);
        }

        private void UpdateChatRooms()
        {
            lock (_lock)
            {
                if (ChatRoomsUpdateTimer.Enabled)
                {
                    Dictionary<ChatRoomWebsite, int> maxUpdateCounts = new()
                    {
                        { ChatRoomWebsite.Chaturbate, _settings.ChaturbateConcurrentUpdates },
                        { ChatRoomWebsite.BongaCams, _settings.BongaCamsConcurrentUpdates },
                        { ChatRoomWebsite.Stripchat, _settings.StripchatConcurrentUpdates },
                        { ChatRoomWebsite.Flirt4Free, _settings.Flirt4FreeConcurrentUpdates },
                        { ChatRoomWebsite.CamSoda, _settings.CamSodaConcurrentUpdates },
                        { ChatRoomWebsite.Cam4, _settings.Cam4ConcurrentUpdates }
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
                        foreach (Category category in parent.AllCategories)
                        {
                            cs.Push(category);
                        }
                        foreach (ChatRoom chatRoom in parent.AllChatRooms)
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
            }
        }

        private void SaveChatRoom(ChatRoom chatRoom, bool resetItem)
        {
            lock (_lock)
            {
                WriteData(c_chatRoomsTableName, 0, chatRoom, null, null);

                if (resetItem)
                {
                    int index = _chatRoomsList.IndexOf(chatRoom);
                    if (index != -1)
                    {
                        _chatRoomsList.ResetItem(index);
                    }
                }
            }
        }

        private void UpdateLog(ChatRoom chatRoom)
        {
            lock (_lock)
            {
                LogListBox.BeginUpdate();

                if (_settings.LogSize > 0)
                {
                    if (LogListBox.Items.Count > 1.1 * _settings.LogSize)
                    {
                        for (int i = 0; i < _settings.LogSize; i++)
                        {
                            LogListBox.Items[i] = LogListBox.Items[LogListBox.Items.Count - _settings.LogSize + i];
                        }
                        while (LogListBox.Items.Count > _settings.LogSize)
                        {
                            LogListBox.Items.RemoveAt(LogListBox.Items.Count - 1);
                        }
                    }

                    Tuple<DateTime, string> entry = null;
                    while (chatRoom.Log.TryDequeue(out entry))
                    {
                        LogListBox.Items.Add(string.Format("{0:yyyy/MM/dd HH:mm:ss:ffffff} - {1} - {2} - {3}", entry.Item1, chatRoom.Website, chatRoom.Name, entry.Item2).ToLower());
                    }
                }
                else
                {
                    chatRoom.Log.Clear();
                    LogListBox.Items.Clear();
                }

                LogListBox.EndUpdate();
            }
        }

        #endregion

        #region Web browser

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

        #region Filtering

        private void UrlTextBox_TextChanged(object sender, EventArgs e)
        {
            FilterCategoriesAndChatRooms();
        }

        private void FilterCategoriesAndChatRooms()
        {
            lock (_lock)
            {
                _root.Filter = FilterTextBox.Text;

                //filtering categories

                TreeNode root = CategoriesTreeView.Nodes[0];

                Stack<TreeNode> ns = new();
                ns.Push(root);
                while (ns.Count > 0)
                {
                    TreeNode currentNode = ns.Pop();
                    Category currentCategory = (Category)currentNode.Tag;

                    if (currentCategory.Filter == string.Empty)
                    {
                        currentNode.NodeFont = new Font(CategoriesTreeView.Font, FontStyle.Regular);
                    }
                    else if (currentCategory.FilteredChatRooms.Length > 0)
                    {
                        currentNode.NodeFont = new Font(CategoriesTreeView.Font, FontStyle.Underline);
                    }
                    else if (currentCategory.FilteredCategories.Length > 0)
                    {
                        currentNode.NodeFont = new Font(CategoriesTreeView.Font, FontStyle.Regular);
                    }
                    else
                    {
                        currentNode.NodeFont = new Font(CategoriesTreeView.Font, FontStyle.Italic);
                    }

                    foreach (TreeNode subNode in currentNode.Nodes)
                    {
                        ns.Push(subNode);
                    }
                }

                //filtering chat rooms

                List<ChatRoom> selectedBefore = new();

                foreach (DataGridViewRow row in ChatRoomsDataGridView.SelectedRows)
                {
                    selectedBefore.Add(_chatRoomsList[row.Index]);
                }

                _chatRoomsList.Clear();
                _chatRoomsList.Append(((Category)CategoriesTreeView.SelectedNode.Tag).FilteredChatRooms.ToList<ChatRoom>());

                List<int> selectedAfter = new();

                foreach (ChatRoom chatRoom in selectedBefore)
                {
                    int index = _chatRoomsList.IndexOf(chatRoom);
                    if (index != -1)
                    {
                        selectedAfter.Add(index);
                    }
                }

                if (selectedAfter.Count > 0)
                {
                    ChatRoomsDataGridView.ClearSelection();

                    foreach (int index in selectedAfter)
                    {
                        ChatRoomsDataGridView.Rows[index].Selected = true;
                    }

                    ChatRoomsDataGridView.FirstDisplayedScrollingRowIndex = selectedAfter.Min();
                }
            }
        }

        #endregion

        #region Categories

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
            if (e.Label != null)
            {
                RenameCategory(e.Label);
            }
        }

        private void CategoriesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowChatRooms();
        }

        private void AddCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddCategory();
        }

        private void RemoveCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveCategory();
        }

        private void RenameCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CategoriesTreeView.SelectedNode.BeginEdit();
        }

        private void CategoriesTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F2:
                    CategoriesTreeView.SelectedNode.BeginEdit();
                    break;
            }
        }

        private void CategoriesContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            AddCategoryToolStripMenuItem.Enabled = CategoriesTreeView.SelectedNode != null;
            RemoveCategoryToolStripMenuItem.Enabled = CategoriesTreeView.SelectedNode != null && CategoriesTreeView.SelectedNode.Tag != _root;
            RenameCategoryToolStripMenuItem.Enabled = CategoriesTreeView.SelectedNode != null && CategoriesTreeView.SelectedNode.Tag != _root;
        }

        private void ShowCategories()
        {
            TreeNode root = new() { Text = _root.Name, Tag = _root };

            Stack<TreeNode> ns = new();
            ns.Push(root);
            while (ns.Count > 0)
            {
                TreeNode currentNode = ns.Pop();
                Category currentCategory = (Category)currentNode.Tag;

                foreach (Category subCategory in currentCategory.AllCategories)
                {
                    TreeNode subNode = new TreeNode() { Text = subCategory.Name, Tag = subCategory };
                    currentNode.Nodes.Add(subNode);
                    ns.Push(subNode);
                }
            }

            CategoriesTreeView.Nodes.Clear();
            CategoriesTreeView.Nodes.Add(root);
            CategoriesTreeView.Sort();
            CategoriesTreeView.ExpandAll();
            CategoriesTreeView.SelectedNode = root;
        }

        private void AddCategory()
        {
            lock (_lock)
            {
                Category parent = (Category)CategoriesTreeView.SelectedNode.Tag;
                Category category = new Category(0, "New category");
                parent.Add(category);

                TreeNode node = new TreeNode() { Text = category.Name, Tag = category };
                CategoriesTreeView.SelectedNode.Nodes.Add(node);
                CategoriesTreeView.SelectedNode = node;

                WriteData(c_categoriesTableName, 1, null, category, parent);

                FilterCategoriesAndChatRooms();
            }
        }

        private void RemoveCategory()
        {
            if (MessageBox.Show(c_confirmCategoryRemovingMessage, string.Empty, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                TreeNode currentNode = CategoriesTreeView.SelectedNode;
                TreeNode parentNode = currentNode.Parent;

                Category currentCategory = (Category)currentNode.Tag;
                Category parentCategory = (Category)parentNode.Tag;

                if (currentCategory.AllCategories.Length == 0 && currentCategory.AllChatRooms.Length == 0)
                {
                    lock (_lock)
                    {
                        parentCategory.Remove(currentCategory);

                        parentNode.Nodes.Remove(currentNode);
                        if (parentNode.Nodes.Count == 0)
                        {
                            parentNode.Collapse();
                        }

                        WriteData(c_categoriesTableName, -1, null, currentCategory, null);
                    }
                }
                else
                {
                    MessageBox.Show(c_categoryNotEmptyMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RenameCategory(string newName)
        {
            lock (_lock)
            {
                Category category = (Category)CategoriesTreeView.SelectedNode.Tag;
                category.Name = newName;

                CategoriesTreeView.BeginInvoke(new Action(() =>
                {
                    TreeNode selectedNode = CategoriesTreeView.SelectedNode;
                    CategoriesTreeView.Sort();
                    CategoriesTreeView.SelectedNode = selectedNode;
                }));

                WriteData(c_categoriesTableName, 0, null, category, null);
            }
        }

        #endregion

        #region Chat rooms

        private void AddChatRoomFromUrlButton_Click(object sender, EventArgs e)
        {
            AddChatRoomFromUrl();
        }

        private void FilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddChatRoomFromUrl();
            }
        }

        private void ChatRoomsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ChatRoomsDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(ChatRoomsDataGridView.DefaultCellStyle.Font, FontStyle.Bold);
            }
        }

        private void ChatRoomsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ChatRoomsDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(ChatRoomsDataGridView.DefaultCellStyle.Font, FontStyle.Regular);
            }
        }

        private void ChatRoomsDataGridView_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ChatRoomsDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(ChatRoomsDataGridView.DefaultCellStyle.Font, FontStyle.Regular);
            }
        }

        private void ChatRoomsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SaveChatRoom(_chatRoomsList[e.RowIndex], false);
            }
        }

        private void ChatRoomsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            ShowFiles();

            ShowThumbnail();
        }

        private void AddChatRoomFromURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddChatRoomFromUrl();
        }

        private void AddChatRoomFromListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddChatRoomFromList();
        }

        private void AddChatRoomFromFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddChatRoomsFromFolder();
        }

        private void RemoveChatRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveChatRoom();
        }

        private void CopyUrlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyUrl();
        }

        private void SetActionChatRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetAction((ChatRoomAction)((ToolStripMenuItem)sender).Tag);
        }

        private void SetResolutionChatRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetResolution((ChatRoomResolution)((ToolStripMenuItem)sender).Tag);
        }

        private void ChatRoomsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            AddChatRoomFromListToolStripMenuItem.Enabled = true;
            AddChatRoomFromFolderToolStripMenuItem.Enabled = true;
            RemoveChatRoomToolStripMenuItem.Enabled = ChatRoomsDataGridView.SelectedRows.Count > 0;
            CopyUrlToolStripMenuItem.Enabled = ChatRoomsDataGridView.SelectedRows.Count > 0;
            SetActionToolStripMenuItem.Enabled = ChatRoomsDataGridView.SelectedRows.Count > 0;
            SetResolutionToolStripMenuItem.Enabled = ChatRoomsDataGridView.SelectedRows.Count > 0;
        }

        private void ShowChatRooms()
        {
            lock (_lock)
            {
                _chatRoomsList.Clear();
                _chatRoomsList.Append(((Category)CategoriesTreeView.SelectedNode.Tag).FilteredChatRooms.ToList<ChatRoom>());
            }
        }

        private void AddChatRoomFromUrl()
        {
            Tuple<ChatRoomWebsite, string, string> parsedUrl = ChatRoom.ParseUrl(FilterTextBox.Text);

            if (parsedUrl != null)
            {
                if (!ChatRoomExists(parsedUrl))
                {
                    ChatRoom newChatRoom = AddChatRoom(parsedUrl);

                    FilterCategoriesAndChatRooms();

                    FilterTextBox.Text = string.Empty;
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

        private void AddChatRoomFromList()
        {
            UrlsForm urlsForm = new UrlsForm();
            if (urlsForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<Tuple<ChatRoomWebsite, string, string>> parsedUrls = new();

                    foreach (string url in urlsForm.Urls)
                    {
                        parsedUrls.Add(ChatRoom.ParseUrl(url));
                    }

                    List<ChatRoom> newChatRooms = new();

                    foreach (Tuple<ChatRoomWebsite, string, string> parsedUrl in parsedUrls)
                    {
                        if (parsedUrl != null && !ChatRoomExists(parsedUrl))
                        {
                            newChatRooms.Add(AddChatRoom(parsedUrl));
                        }
                    }

                    MessageBox.Show(c_chatRoomsAddedMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show(c_chatRoomsAddingErrorMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                FilterCategoriesAndChatRooms();
            }
        }

        private void AddChatRoomsFromFolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<Tuple<ChatRoomWebsite, string, string>> parsedUrls = new();

                    foreach (DirectoryInfo directory in new DirectoryInfo(fbd.SelectedPath).GetDirectories())
                    {
                        parsedUrls.Add(ChatRoom.ParseUrl(directory.Name));
                    }

                    List<ChatRoom> newChatRooms = new();

                    foreach (Tuple<ChatRoomWebsite, string, string> parsedUrl in parsedUrls)
                    {
                        if (parsedUrl != null && !ChatRoomExists(parsedUrl))
                        {
                            newChatRooms.Add(AddChatRoom(parsedUrl));
                        }
                    }

                    MessageBox.Show(c_chatRoomsAddedMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show(c_chatRoomsAddingErrorMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                FilterCategoriesAndChatRooms();
            }
        }

        private bool ChatRoomExists(Tuple<ChatRoomWebsite, string, string> parsedUrl)
        {
            Stack<Category> cs = new Stack<Category>();
            cs.Push(_root);
            while (cs.Count > 0)
            {
                Category parent = cs.Pop();
                foreach (ChatRoom chatRoom in parent.AllChatRooms)
                {
                    if (chatRoom.Website == parsedUrl.Item1 && chatRoom.Name == parsedUrl.Item2)
                    {
                        return true;
                    }
                }
                foreach (Category category in parent.AllCategories)
                {
                    cs.Push(category);
                }
            }
            return false;
        }

        private ChatRoom AddChatRoom(Tuple<ChatRoomWebsite, string, string> parsedUrl)
        {
            lock (_lock)
            {
                ChatRoom chatRoom = new(parsedUrl.Item3);
                chatRoom.OutputDirectory = _settings.OutputDirectory;
                chatRoom.FFmpegPath = _settings.FFmpegPath;
                chatRoom.StreamlinkPath = _settings.StreamlinkPath;
                chatRoom.Action = _settings.DefaultAction;
                chatRoom.PreferredResolution = _settings.DefaultResolution;
                chatRoom.UpdateCompleted += ChatRoom_UpdateCompleted;

                Category category = (Category)CategoriesTreeView.SelectedNode.Tag;
                category.Add(chatRoom);

                WriteData(c_chatRoomsTableName, 1, chatRoom, category, null);

                return chatRoom;
            }
        }

        private void RemoveChatRoom()
        {
            if (MessageBox.Show(c_confirmChatRoomsRemovingMessage, string.Empty, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                lock (_lock)
                {
                    List<ChatRoom> chatRooms = new List<ChatRoom>();

                    foreach (DataGridViewRow row in ChatRoomsDataGridView.SelectedRows)
                    {
                        chatRooms.Add(_chatRoomsList[row.Index]);
                    }

                    _chatRoomsList.Delete(chatRooms);

                    foreach (ChatRoom chatRoom in chatRooms)
                    {
                        ((Category)CategoriesTreeView.SelectedNode.Tag).Remove(chatRoom);

                        WriteData(c_chatRoomsTableName, -1, chatRoom, null, null);

                        chatRoom.Dispose();
                    }

                    FilterCategoriesAndChatRooms();
                }
            }
        }

        private void CopyUrl()
        {
            List<string> urls = new();
            foreach (DataGridViewRow selectedRow in ChatRoomsDataGridView.SelectedRows)
            {
                urls.Add(_chatRoomsList[selectedRow.Index].ChatRoomUrl);
            }
            Clipboard.SetText(string.Join("\r\n", urls));
        }

        private void SetAction(ChatRoomAction action)
        {
            foreach (DataGridViewRow selectedRow in ChatRoomsDataGridView.SelectedRows)
            {
                _chatRoomsList[selectedRow.Index].Action = action;

                SaveChatRoom(_chatRoomsList[selectedRow.Index], true);
            }
        }

        private void SetResolution(ChatRoomResolution resolution)
        {
            foreach (DataGridViewRow selectedRow in ChatRoomsDataGridView.SelectedRows)
            {
                _chatRoomsList[selectedRow.Index].PreferredResolution = resolution;

                SaveChatRoom(_chatRoomsList[selectedRow.Index], true);
            }
        }

        #endregion

        #region Files

        private void FilesDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    OpenFile();
                    e.Handled = true;
                    break;
                case Keys.Delete:
                    RemoveFile();
                    e.Handled = true;
                    break;
            }
        }

        private void FilesDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            OpenFile();
        }

        private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void RemoveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveFile();
        }

        private void ShowFileInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFileInExplorer();
        }

        private void FilesContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            OpenFileToolStripMenuItem.Enabled = FilesDataGridView.SelectedRows.Count == 1;
            RemoveFileToolStripMenuItem.Enabled = FilesDataGridView.SelectedRows.Count > 0;
            ShowFileInExplorerToolStripMenuItem.Enabled = FilesDataGridView.SelectedRows.Count == 1;
        }

        private void ShowFiles()
        {
            try
            {
                _filesList.Clear();

                List<FileInfo> files = new();

                foreach (DataGridViewRow selectedRow in ChatRoomsDataGridView.SelectedRows)
                {
                    ChatRoom chatRoom = _chatRoomsList[selectedRow.Index];
                    DirectoryInfo dir = new(string.Format("{0}\\{1} {2}", _settings.OutputDirectory, chatRoom.Website, chatRoom.Name).ToLower());
                    if (dir.Exists)
                    {
                        files.AddRange(dir.GetFiles(string.Format("{0} {1} *.ts", chatRoom.Website, chatRoom.Name).ToLower()));
                    }
                }

                _filesList.Append(files);
            }
            catch (Exception)
            {
                //do nothing
            }
        }

        private void OpenFile()
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

        private void RemoveFile()
        {
            if (MessageBox.Show(c_confirmFilesRemovingMessage, string.Empty, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                try
                {
                    List<FileInfo> files = new List<FileInfo>();

                    foreach (DataGridViewRow row in FilesDataGridView.SelectedRows)
                    {
                        files.Add(_filesList[row.Index]);
                    }

                    _filesList.Delete(files);

                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(c_fileRemovingErrorMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowFileInExplorer()
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = "explorer",
                    Arguments = string.Format("/select,\"{0}\"", _filesList[FilesDataGridView.SelectedRows[0].Index].FullName)
                });
            }
            catch (Exception)
            {
                MessageBox.Show(c_fileShowingErrorMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Thumbnail

        private void RemoveThumbnailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveThumbnail();
        }

        private void ThumbnailContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            RemoveThumbnailToolStripMenuItem.Enabled = ThumbnailPictureBox.Image != null;
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

        private void RemoveThumbnail()
        {
            if (MessageBox.Show(c_confirmThumbnailRemovingMessage, string.Empty, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
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

        #endregion

        #region Settings

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
            SaveSettings(e.PropertyName);
        }

        private void SaveSettings(string propertyName)
        {
            lock (_lock)
            {
                switch (propertyName)
                {
                    case c_outputDirectorySettingName:
                        if (_root != null)
                        {
                            Stack<Category> cs = new Stack<Category>();
                            cs.Push(_root);
                            while (cs.Count > 0)
                            {
                                Category parent = cs.Pop();
                                foreach (Category category in parent.AllCategories)
                                {
                                    cs.Push(category);
                                }
                                foreach (ChatRoom chatRoom in parent.AllChatRooms)
                                {
                                    chatRoom.OutputDirectory = _settings.OutputDirectory;
                                }
                            }
                        }
                        break;

                    case c_ffmpegPathSettingName:
                        if (_root != null)
                        {
                            Stack<Category> cs = new Stack<Category>();
                            cs.Push(_root);
                            while (cs.Count > 0)
                            {
                                Category parent = cs.Pop();
                                foreach (Category category in parent.AllCategories)
                                {
                                    cs.Push(category);
                                }
                                foreach (ChatRoom chatRoom in parent.AllChatRooms)
                                {
                                    chatRoom.FFmpegPath = _settings.FFmpegPath;
                                }
                            }
                        }
                        break;

                    case c_streamlinkPathSettingName:
                        if (_root != null)
                        {
                            Stack<Category> cs = new Stack<Category>();
                            cs.Push(_root);
                            while (cs.Count > 0)
                            {
                                Category parent = cs.Pop();
                                foreach (Category category in parent.AllCategories)
                                {
                                    cs.Push(category);
                                }
                                foreach (ChatRoom chatRoom in parent.AllChatRooms)
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

                    case c_flirt4FreeConcurrentUpdatesSettingName:
                        //do nothing
                        break;

                    case c_camSodaConcurrentUpdatesSettingName:
                        //do nothing
                        break;

                    case c_cam4ConcurrentUpdatesSettingName:
                        //do nothing
                        break;

                    case c_updateDelaySettingName:
                        if (_root != null)
                        {
                            Stack<Category> cs = new Stack<Category>();
                            cs.Push(_root);
                            while (cs.Count > 0)
                            {
                                Category parent = cs.Pop();
                                foreach (Category category in parent.AllCategories)
                                {
                                    cs.Push(category);
                                }
                                foreach (ChatRoom chatRoom in parent.AllChatRooms)
                                {
                                    chatRoom.Delay = _settings.UpdateDelay;
                                }
                            }
                        }
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

                    case c_logSizeSettingName:
                        //do nothing
                        break;
                }

                WriteData(c_settingsTableName, 0, null, null, null);
            }
        }

        #endregion

        #region Logging

        private void CopyLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyLog();
        }

        private void LogContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            LogContextMenuStrip.Enabled = LogListBox.SelectedItems.Count > 0;
        }

        private void CopyLog()
        {
            Clipboard.SetText(string.Join("\r\n", LogListBox.SelectedItems.Cast<string>().ToArray()));
        }

        #endregion

        #region Drag and drop

        private void ChatRoomsDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex != -1 && e.ColumnIndex != 2 && e.ColumnIndex != 3 && ChatRoomsDataGridView.SelectedRows.Contains(ChatRoomsDataGridView.Rows[e.RowIndex]))
                {
                    ChatRoomsDataGridView.DoDragDrop(ChatRoomsDataGridView.SelectedRows, DragDropEffects.Move);
                }
            }
        }

        private void CategoriesTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                CategoriesTreeView.Tag = new object();
            }
        }

        private void CategoriesTreeView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                CategoriesTreeView.Tag = null;
            }
        }

        private void CategoriesTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CategoriesTreeView.Tag != null && CategoriesTreeView.GetNodeAt(new Point(e.X, e.Y)) != CategoriesTreeView.SelectedNode)
            {
                CategoriesTreeView.DoDragDrop(CategoriesTreeView.SelectedNode, DragDropEffects.Move);
            }
        }

        private void CategoriesTreeView_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel || e.Action == DragAction.Drop)
            {
                CategoriesTreeView.Tag = null;
            }
        }

        private void CategoriesTreeView_DragOver(object sender, DragEventArgs e)
        {
            TreeNode dstNode = CategoriesTreeView.GetNodeAt(CategoriesTreeView.PointToClient(new Point(e.X, e.Y)));

            if (dstNode != null)
            {
                if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
                {
                    e.Effect = DragDropEffects.Move;
                }
                else if (e.Data.GetDataPresent(typeof(TreeNode)))
                {
                    bool cancel = false;

                    Stack<TreeNode> ns = new Stack<TreeNode>();
                    ns.Push(CategoriesTreeView.SelectedNode);
                    while (ns.Count > 0 && !cancel)
                    {
                        TreeNodeCollection nodes = ns.Pop().Nodes;
                        if (!nodes.Contains(dstNode))
                        {
                            foreach (TreeNode node in nodes)
                            {
                                ns.Push(node);
                            }
                        }
                        else
                        {
                            cancel = true;
                        }
                    };

                    if (!cancel)
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void CategoriesTreeView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
            {
                MoveChatRooms(new Point(e.X, e.Y));
            }
            else if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                MoveCategories(new Point(e.X, e.Y));
            }
        }

        private void MoveChatRooms(Point point)
        {
            lock (_lock)
            {
                TreeNode oldNode = CategoriesTreeView.SelectedNode;
                TreeNode newNode = CategoriesTreeView.GetNodeAt(CategoriesTreeView.PointToClient(point));

                if (newNode != oldNode)
                {
                    List<ChatRoom> chatRooms = new List<ChatRoom>();

                    foreach (DataGridViewRow row in ChatRoomsDataGridView.SelectedRows)
                    {
                        chatRooms.Add(_chatRoomsList[row.Index]);
                    }

                    _chatRoomsList.Delete(chatRooms);

                    foreach (ChatRoom chatRoom in chatRooms)
                    {
                        ((Category)oldNode.Tag).Remove(chatRoom);
                        ((Category)newNode.Tag).Add(chatRoom);

                        WriteData(c_chatRoomsTableName, 0, chatRoom, null, (Category)newNode.Tag);
                    }

                    FilterCategoriesAndChatRooms();
                }
            }
        }

        private void MoveCategories(Point point)
        {
            lock (_lock)
            {
                TreeNode curNode = CategoriesTreeView.SelectedNode;
                TreeNode dstNode = CategoriesTreeView.GetNodeAt(CategoriesTreeView.PointToClient(point));
                TreeNode parentNode = curNode.Parent;

                if (dstNode != curNode && dstNode != parentNode)
                {
                    Category curCategory = (Category)curNode.Tag;
                    Category dstCategory = (Category)dstNode.Tag;
                    Category parentCategory = (Category)parentNode.Tag;

                    parentCategory.Remove(curCategory);
                    dstCategory.Add(curCategory);

                    if (parentNode.Nodes.Count == 1)
                    {
                        parentNode.Collapse();
                    }
                    dstNode.Expand();

                    parentNode.Nodes.Remove(curNode);
                    dstNode.Nodes.Add(curNode);

                    CategoriesTreeView.SelectedNode = null;
                    CategoriesTreeView.SelectedNode = curNode;

                    WriteData(c_categoriesTableName, 0, null, curCategory, dstCategory);

                    FilterCategoriesAndChatRooms();
                }
            }
        }

        #endregion

        #region Database

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
                        case 3:
                            _command.CommandText =
                                string.Format("alter table {0} add {1} integer;",
                                    c_settingsTableName,
                                    c_settingsTableFlirt4FreeConcurrentUpdatesColumnName) +
                                string.Format("alter table {0} add {1} integer;",
                                    c_settingsTableName,
                                    c_settingsTableUpdateDelayColumnName) +
                                "pragma user_version = 4;";
                            _command.ExecuteNonQuery();
                            break;
                        case 4:
                            _command.CommandText =
                                string.Format("alter table {0} add {1} integer;",
                                    c_settingsTableName,
                                    c_settingsTableCamSodaConcurrentUpdatesColumnName) +
                                string.Format("alter table {0} add {1} integer;",
                                    c_settingsTableName,
                                    c_settingsTableLogSizeColumnName) +
                                "pragma user_version = 5;";
                            _command.ExecuteNonQuery();
                            break;
                        case 5:
                            _command.CommandText =
                                string.Format("alter table {0} add {1} integer;",
                                    c_settingsTableName,
                                    c_settingsTableCam4ConcurrentUpdatesColumnName) +
                                "pragma user_version = 6;";
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
                        _settings.Flirt4FreeConcurrentUpdates = reader.GetInt32(c_settingsTableFlirt4FreeConcurrentUpdatesColumnName);
                        _settings.CamSodaConcurrentUpdates = reader.GetInt32(c_settingsTableCamSodaConcurrentUpdatesColumnName);
                        _settings.Cam4ConcurrentUpdates = reader.GetInt32(c_settingsTableCam4ConcurrentUpdatesColumnName);
                        _settings.UpdateDelay = reader.GetInt32(c_settingsTableUpdateDelayColumnName);
                        _settings.UpdateInterval = reader.GetInt32(c_settingsTableUpdateIntervalColumnName);
                        _settings.DefaultAction = (ChatRoomAction)reader.GetInt32(c_settingsTableDefaultActionColumnName);
                        _settings.DefaultResolution = ChatRoomResolution.Parse(reader.GetInt32(c_settingsTableDefaultResolutionColumnName));
                        _settings.LogSize = reader.GetInt32(c_settingsTableLogSizeColumnName);
                    }
                }
            }
            catch (Exception)
            {
                c_defaultSettings.CopyTo(_settings);
            }

            //read categories and chat rooms

            try
            {
                _root = new Category(1, c_rootCategoryName);

                Stack<Category> st = new();
                st.Push(_root);
                while (st.Count > 0)
                {
                    Category curCategory = st.Pop();

                    _command.CommandText = string.Format("select * from {0} where {1}={2};", c_categoriesTableName, c_categoriesTableCategoryColumnName, curCategory.ID);
                    using (SqliteDataReader reader = _command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Category newCategory = new Category(reader.GetInt32(c_categoriesTableIdColumnName), reader.GetString(c_categoriesTableNameColumnName));
                            curCategory.Add(newCategory);

                            st.Push(newCategory);
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
                            chatRoom.Delay = _settings.UpdateDelay;
                            chatRoom.LastUpdated = updated;
                            chatRoom.LastSeen = seen;
                            chatRoom.UpdateCompleted += ChatRoom_UpdateCompleted;

                            curCategory.Add(chatRoom);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Stack<Category> cs = new();
                cs.Push(_root);
                while (cs.Count > 0)
                {
                    Category parent = cs.Pop();
                    foreach (Category category in parent.AllCategories)
                    {
                        cs.Push(category);
                    }
                    foreach (ChatRoom chatRoom in parent.AllChatRooms)
                    {
                        chatRoom.Dispose();
                    }
                }

                _root = new Category(1, c_rootCategoryName);
            }
        }

        private void WriteData(string table, int operation, ChatRoom chatRoom, Category category, Category parent)
        {
            try
            {
                switch (table)
                {
                    case c_settingsTableName:

                        _command.CommandText = string.Format("delete from {0}; insert into {0} ({1},{3},{5},{7},{9},{11},{13},{15},{17},{19},{21},{23},{25},{27}) values ('{2}','{4}','{6}','{8}','{10}','{12}','{14}','{16}','{18}','{20}','{22}','{24}','{26}','{28}');",
                            c_settingsTableName,
                            c_settingsTableOutputDirectoryColumnName, _settings.OutputDirectory,
                            c_settingsTableFFmpegPathColumnName, _settings.FFmpegPath,
                            c_settingsTableStreamlinkPathColumnName, _settings.StreamlinkPath,
                            c_settingsTableChaturbateConcurrentUpdatesColumnName, _settings.ChaturbateConcurrentUpdates,
                            c_settingsTableBongaCamsConcurrentUpdatesColumnName, _settings.BongaCamsConcurrentUpdates,
                            c_settingsTableStripchatConcurrentUpdatesColumnName, _settings.StripchatConcurrentUpdates,
                            c_settingsTableFlirt4FreeConcurrentUpdatesColumnName, _settings.Flirt4FreeConcurrentUpdates,
                            c_settingsTableCamSodaConcurrentUpdatesColumnName, _settings.CamSodaConcurrentUpdates,
                            c_settingsTableCam4ConcurrentUpdatesColumnName, _settings.Cam4ConcurrentUpdates,
                            c_settingsTableUpdateDelayColumnName, _settings.UpdateDelay,
                            c_settingsTableUpdateIntervalColumnName, _settings.UpdateInterval,
                            c_settingsTableDefaultActionColumnName, (int)_settings.DefaultAction,
                            c_settingsTableDefaultResolutionColumnName, _settings.DefaultResolution.ToInt32(),
                            c_settingsTableLogSizeColumnName, _settings.LogSize);
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

        #region Supplemental

        private void MoveFiles()
        {
            foreach (FileInfo file in new DirectoryInfo(_settings.OutputDirectory).GetFiles())
            {
                MatchCollection matches = Regex.Matches(file.Name, "^([^ ]+ [^ ]+) (.*.ts)$");
                if (matches.Count > 0)
                {
                    string dir = string.Format("{0}\\{1}", _settings.OutputDirectory, matches[0].Groups[1].Value);
                    Directory.CreateDirectory(dir);
                    File.Move(file.FullName, string.Format("{0}\\{1}", dir, file.Name));
                }
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
            Flirt4FreeConcurrentUpdates = 1,
            CamSodaConcurrentUpdates = 1,
            Cam4ConcurrentUpdates = 1,
            UpdateDelay = 15,
            UpdateInterval = 60,
            DefaultAction = ChatRoomAction.None,
            DefaultResolution = ChatRoomResolution.CommonResolutions[ChatRoomResolution.FindClosest(new ChatRoomResolution(1920, 1080), ChatRoomResolution.CommonResolutions)],
            LogSize = 5000
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
        private const string c_settingsTableFlirt4FreeConcurrentUpdatesColumnName = "Flirt4FreeConcurrentUpdates";
        private const string c_settingsTableCamSodaConcurrentUpdatesColumnName = "CamSodaConcurrentUpdates";
        private const string c_settingsTableCam4ConcurrentUpdatesColumnName = "Cam4ConcurrentUpdates";
        private const string c_settingsTableUpdateDelayColumnName = "UpdateDelay";
        private const string c_settingsTableUpdateIntervalColumnName = "UpdateInterval";
        private const string c_settingsTableDefaultActionColumnName = "DefaultAction";
        private const string c_settingsTableDefaultResolutionColumnName = "DefaultResolution";
        private const string c_settingsTableLogSizeColumnName = "LogSize";

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
        private const string c_flirt4FreeConcurrentUpdatesSettingName = "Flirt4FreeConcurrentUpdates";
        private const string c_camSodaConcurrentUpdatesSettingName = "CamSodaConcurrentUpdates";
        private const string c_cam4ConcurrentUpdatesSettingName = "Cam4ConcurrentUpdates";
        private const string c_updateDelaySettingName = "UpdateDelay";
        private const string c_updateIntervalSettingName = "UpdateInterval";
        private const string c_defaultActionSettingName = "DefaultAction";
        private const string c_defaultResolutionSettingName = "DefaultResolution";
        private const string c_logSizeSettingName = "LogSize";

        private const string c_unsupportedUrlMessage = "The URL is incorrect! Only http://* and https://* URLs are supported!";
        private const string c_unsupportedWebSiteMessage = "The URL is incorrect! Supported websites are Chaturbate, BongaCams, Stripchat, Flirt4Free, CamSoda and Cam4!";
        private const string c_duplicateChatRoomMessage = "The chat room is already existed!";
        private const string c_confirmCategoryRemovingMessage = "The category will be removed!";
        private const string c_categoryNotEmptyMessage = "The category is not empty!";
        private const string c_confirmChatRoomsRemovingMessage = "The selected chat rooms will be removed!";
        private const string c_fileOpeningErrorMessage = "The file can't be opened!";
        private const string c_confirmFilesRemovingMessage = "The selected files will be removed!";
        private const string c_fileRemovingErrorMessage = "The file can't be removed!";
        private const string c_fileShowingErrorMessage = "The file can't be shown!";
        private const string c_confirmThumbnailRemovingMessage = "The thumbnail will be removed!";
        private const string c_thumbnailRemovingErrorMessage = "The thumbnail can't be removed!";
        private const string c_chatRoomsAddingErrorMessage = "The chat rooms can't be added!";
        private const string c_chatRoomsAddedMessage = "The chat rooms have been added!";

        #endregion
    }
}