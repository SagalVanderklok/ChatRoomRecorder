using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing;

namespace ChatRoomRecorder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.Text = Assembly.GetEntryAssembly().GetName().Name + " " + Assembly.GetEntryAssembly().GetName().Version.ToString(3);
            Assembly asm = Assembly.GetEntryAssembly();
            LicenseTextBox.Text =
                "\r\n" +
                ((AssemblyCopyrightAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(asm, typeof(AssemblyCopyrightAttribute))).Copyright + " | " +
                ((AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(asm, typeof(AssemblyDescriptionAttribute))).Description +
                "\r\n\r\n" +
                LicenseTextBox.Text;

            ChatRoomsDataGridView.Columns[c_indexColumnIndex].ValueType = typeof(int);
            ChatRoomsDataGridView.Columns[c_websiteColumnIndex].ValueType = typeof(ChatRoomWebsite);
            ChatRoomsDataGridView.Columns[c_nameColumnIndex].ValueType = typeof(string);
            ChatRoomsDataGridView.Columns[c_actionColumnIndex].ValueType = typeof(ChatRoomAction);
            ChatRoomsDataGridView.Columns[c_statusColumnIndex].ValueType = typeof(ChatRoomStatus);
            ChatRoomsDataGridView.Columns[c_resolutionColumnIndex].ValueType = typeof(string);
            ChatRoomsDataGridView.Columns[c_lastUpdateColumnIndex].ValueType = typeof(string);

            ReadConfig();

            ChangeCellEventHandling(true);

            ChatRoomsUpdateTimer.Start();
            DataGridViewUpdateTimer.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (!_isExiting)
                {
                    _isExiting = true;

                    WriteConfig();

                    foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                    {
                        ((ChatRoom)row.Tag).Dispose();
                    }

                    ChatRoomsUpdateTimer.Stop();
                    DataGridViewUpdateTimer.Stop();
                    FormCloseTimer.Start();

                    TabControl.Enabled = false;
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

        private void ChatRoomsUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (Monitor.TryEnter(_chatRoomsDataGridViewLock))
            {
                if (ChatRoomsUpdateTimer.Enabled)
                {
                    bool atLeastOneUpdated = false;
                    foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                    {
                        ChatRoom chatRoom = (ChatRoom)row.Tag;
                        if (chatRoom.Action != ChatRoomAction.None)
                        {
                            if (!atLeastOneUpdated && chatRoom.LastUpdate < _lastUpdate)
                            {
                                chatRoom.Update();
                                atLeastOneUpdated = true;
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
                    if (!atLeastOneUpdated)
                    {
                        _lastUpdate = DateTime.Now;
                    }
                }

                Monitor.Exit(_chatRoomsDataGridViewLock);
            }
        }

        private void DataGridViewUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (Monitor.TryEnter(_chatRoomsDataGridViewLock))
            {
                ChangeCellEventHandling(false);

                if (DataGridViewUpdateTimer.Enabled)
                {
                    foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                    {
                        if (ChatRoomsDataGridView.IsCurrentCellInEditMode && ChatRoomsDataGridView.SelectedRows.Count > 0 && ChatRoomsDataGridView.SelectedRows[0] == row) continue;

                        ChatRoom chatRoom = (ChatRoom)row.Tag;
                        row.Cells[c_statusColumnIndex].Value = chatRoom.Status;
                        DataGridViewComboBoxCell resolutionCell = (DataGridViewComboBoxCell)row.Cells[c_resolutionColumnIndex];
                        if (chatRoom.AvailableResolutions.Length > 0)
                        {
                            resolutionCell.DataSource = (new string[] { string.Empty }).Concat(chatRoom.AvailableResolutions).ToArray();
                            resolutionCell.Value = chatRoom.AvailableResolutions.Contains(chatRoom.PreferredResolution) ? chatRoom.PreferredResolution : string.Empty;
                        }
                        else if (chatRoom.PreferredResolution != string.Empty)
                        {
                            resolutionCell.DataSource = new string[] { string.Empty, chatRoom.PreferredResolution };
                            resolutionCell.Value = chatRoom.PreferredResolution;
                        }
                        else
                        {
                            resolutionCell.DataSource = new string[] { string.Empty };
                            resolutionCell.Value = string.Empty;
                        }
                        row.Cells[c_lastUpdateColumnIndex].Value = chatRoom.LastUpdate.ToString();
                    }
                }

                ChangeCellEventHandling(true);

                Monitor.Exit(_chatRoomsDataGridViewLock);
            }
        }

        private void FormCloseTimer_Tick(object sender, EventArgs e)
        {
            if (Monitor.TryEnter(_chatRoomsDataGridViewLock))
            {
                if (FormCloseTimer.Enabled)
                {
                    this.Close();
                }

                Monitor.Exit(_chatRoomsDataGridViewLock);
            }
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            if (Regex.Matches(AddressTextBox.Text, "^(http[s]?:.*)$", RegexOptions.IgnoreCase).Count > 0)
            {
                WebView.CoreWebView2.Navigate(AddressTextBox.Text);
            }
            else
            {
                MessageBox.Show(c_unsupportedUrlMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            WebView.CoreWebView2.GoBack();
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            WebView.CoreWebView2.GoForward();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            WebView.CoreWebView2.Stop();
        }

        private void URLTextBox_TextChanged(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                ChatRoomsDataGridView.CurrentCell = null;

                DataGridViewRow firstVisibleRow = null;
                Tuple<ChatRoomWebsite, string, string> parsedUrl = ChatRoom.ParseUrl(URLTextBox.Text);
                foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                {
                    ChatRoom chatRoom = (ChatRoom)row.Tag;
                    row.Visible =
                        chatRoom.RoomUrl.Contains(URLTextBox.Text.ToLower()) ||
                        parsedUrl != null && chatRoom.Website == parsedUrl.Item1 && chatRoom.Name.Contains(parsedUrl.Item2);
                    if (row.Visible && firstVisibleRow == null)
                    {
                        firstVisibleRow = row;
                    }
                }

                if (firstVisibleRow != null)
                {
                    ChatRoomsDataGridView.CurrentCell = firstVisibleRow.Cells[c_indexColumnIndex];
                }
            }
        }

        private void PlusButton_Click(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                ChangeCellEventHandling(false);

                bool error = false;
                Tuple<ChatRoomWebsite, string, string> parsedUrl = ChatRoom.ParseUrl(URLTextBox.Text);

                if (parsedUrl == null)
                {
                    error = true;
                    MessageBox.Show(c_unsupportedWebSiteMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                for (int i = 0; i < ChatRoomsDataGridView.Rows.Count && !error; i++)
                {
                    ChatRoom chatRoom = (ChatRoom)ChatRoomsDataGridView.Rows[i].Tag;
                    if (chatRoom.Website == parsedUrl.Item1 && chatRoom.Name == parsedUrl.Item2)
                    {
                        error = true;
                        MessageBox.Show(c_duplicateChatRoomMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (!error)
                {
                    ChatRoom chatRoom = new ChatRoom(URLTextBox.Text) { OutputDirectory = OutputDirectoryTextBox.Text, FFmpegPath = FFmpegPathTextBox.Text };

                    URLTextBox.Clear();

                    AddDataGridViewRow(chatRoom, ChatRoomsDataGridView.Rows.Count + 1, true);

                    WriteConfig();
                }

                ChangeCellEventHandling(true);
            }
        }

        private void MinusButton_Click(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                ChangeCellEventHandling(false);

                if (ChatRoomsDataGridView.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = ChatRoomsDataGridView.SelectedRows[0];

                    for (int i = 0; i < ChatRoomsDataGridView.Rows.Count; i++)
                    {
                        DataGridViewCell curCell = ChatRoomsDataGridView.Rows[i].Cells[c_indexColumnIndex];
                        DataGridViewCell delCell = row.Cells[c_indexColumnIndex];
                        if ((int)curCell.Value > (int)delCell.Value)
                        {
                            curCell.Value = (int)curCell.Value - 1;
                        }
                    }

                    ((ChatRoom)row.Tag).Dispose();
                    ChatRoomsDataGridView.Rows.Remove(row);

                    WriteConfig();
                }

                ChangeCellEventHandling(true);
            }
        }

        private void ChatRoomsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                ChangeCellEventHandling(false);

                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    DataGridViewRow row = ChatRoomsDataGridView.Rows[e.RowIndex];
                    ChatRoom chatRoom = (ChatRoom)row.Tag;
                    switch (e.ColumnIndex)
                    {
                        case 0:  //Index
                            int newIndex;
                            int oldIndex = (int)row.Cells[c_indexColumnIndex].Value;
                            if (int.TryParse((string)e.FormattedValue, out newIndex) && (newIndex >= 1) && (newIndex <= ChatRoomsDataGridView.Rows.Count))
                            {
                                if (newIndex > oldIndex)
                                {
                                    foreach (DataGridViewRow curRow in ChatRoomsDataGridView.Rows)
                                    {
                                        DataGridViewCell curCell = curRow.Cells[c_indexColumnIndex];
                                        if ((int)curCell.Value > oldIndex && (int)curCell.Value <= newIndex)
                                        {
                                            curCell.Value = (int)curCell.Value - 1;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (DataGridViewRow curRow in ChatRoomsDataGridView.Rows)
                                    {
                                        DataGridViewCell curCell = curRow.Cells[c_indexColumnIndex];
                                        if ((int)curCell.Value >= newIndex && (int)curCell.Value < oldIndex)
                                        {
                                            curCell.Value = (int)curCell.Value + 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                e.Cancel = true;
                            }
                            break;
                        case 3:  //Action
                            chatRoom.Action = (ChatRoomAction)Enum.Parse(typeof(ChatRoomAction), (string)e.FormattedValue);
                            break;
                        case 5:  //Resolution
                            chatRoom.PreferredResolution = e.FormattedValue == null ? string.Empty : (string)e.FormattedValue;
                            break;
                    }
                }

                ChangeCellEventHandling(true);
            }
        }

        private void ChatRoomsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    WriteConfig();
                }
            }
        }

        private void ChatRoomsDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                DataGridViewColumn newColumn = ChatRoomsDataGridView.Columns[e.ColumnIndex];
                DataGridViewColumn oldColumn = ChatRoomsDataGridView.SortedColumn;

                if (newColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                {
                    newColumn.HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    ChatRoomsDataGridView.Sort(newColumn, ListSortDirection.Descending);
                }
                else
                {
                    newColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    ChatRoomsDataGridView.Sort(newColumn, ListSortDirection.Ascending);
                }

                if (newColumn != oldColumn)
                {
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }

                if (ChatRoomsDataGridView.CurrentCell != null && ChatRoomsDataGridView.CurrentCell.Visible && !ChatRoomsDataGridView.CurrentCell.Displayed)
                {
                    ChatRoomsDataGridView.FirstDisplayedCell = ChatRoomsDataGridView.CurrentCell;
                }

                WriteConfig();
            }
        }

        private void ChatRoomsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (e.RowIndex >= 0)
                {
                    ChatRoomsDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(ChatRoomsDataGridView.Font, FontStyle.Bold);
                }
            }
        }

        private void ChatRoomsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (e.RowIndex >= 0)
                {
                    ChatRoomsDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(ChatRoomsDataGridView.Font, FontStyle.Regular);
                }
            }
        }

        private void ChatRoomsDataGridView_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (e.RowIndex >= 0)
                {
                    ChatRoomsDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(ChatRoomsDataGridView.Font, FontStyle.Regular);
                }
            }
        }

        private void OutputDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                lock (_chatRoomsDataGridViewLock)
                {
                    foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                    {
                        ((ChatRoom)row.Tag).OutputDirectory = fbd.SelectedPath;
                    }
                    OutputDirectoryTextBox.Text = fbd.SelectedPath;

                    WriteConfig();
                }
            }
        }

        private void FFmpegPathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, Filter = string.Format("{0}|{0}", c_ffmpegBinaryName) };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lock (_chatRoomsDataGridViewLock)
                {
                    foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                    {
                        ((ChatRoom)row.Tag).FFmpegPath = ofd.FileName;
                    }
                    FFmpegPathTextBox.Text = ofd.FileName;

                    WriteConfig();
                }
            }
        }

        private void ChangeCellEventHandling(bool state)
        {
            if (state)
            {
                ChatRoomsDataGridView.CellValidating += ChatRoomsDataGridView_CellValidating;
                ChatRoomsDataGridView.CellValueChanged += ChatRoomsDataGridView_CellValueChanged;
            }
            else
            {
                ChatRoomsDataGridView.CellValidating -= ChatRoomsDataGridView_CellValidating;
                ChatRoomsDataGridView.CellValueChanged -= ChatRoomsDataGridView_CellValueChanged;
            }
        }

        private void AddDataGridViewRow(ChatRoom chatRoom, int index, bool current)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(ChatRoomsDataGridView);
            row.Tag = chatRoom;
            row.Cells[c_indexColumnIndex].Value = index;
            row.Cells[c_websiteColumnIndex].Value = chatRoom.Website;
            row.Cells[c_nameColumnIndex].Value = chatRoom.Name;
            DataGridViewComboBoxCell actionCell = (DataGridViewComboBoxCell)row.Cells[c_actionColumnIndex];
            actionCell.DataSource = new ChatRoomAction[] { ChatRoomAction.None, ChatRoomAction.Monitor, ChatRoomAction.Record };
            actionCell.Value = chatRoom.Action;
            row.Cells[c_statusColumnIndex].Value = chatRoom.Status;
            DataGridViewComboBoxCell resolutionCell = (DataGridViewComboBoxCell)row.Cells[c_resolutionColumnIndex];
            resolutionCell.DataSource = new string[] { string.Empty };
            resolutionCell.Value = string.Empty;
            row.Cells[c_lastUpdateColumnIndex].Value = chatRoom.LastUpdate.ToString();
            ChatRoomsDataGridView.Rows.Add(row);

            if (current)
            {
                ChatRoomsDataGridView.CurrentCell = row.Cells[c_indexColumnIndex];
            }
        }

        private void ReadConfig()
        {
            try
            {
                JsonNode rootNode = JsonNode.Parse(File.ReadAllText(_configDir + Path.DirectorySeparatorChar + c_configFileName));

                if (rootNode[c_outputDirectoryJsonItemName] == null ||
                    rootNode[c_ffmpegPathJsonItemName] == null ||
                    rootNode[c_sortColumnJsonItemName] == null ||
                    rootNode[c_sortDirectionJsonItemName] == null ||
                    rootNode[c_chatRoomsJsonItemName] ==null)
                {
                    throw new ArgumentException();
                }

                OutputDirectoryTextBox.Text = (string)rootNode[c_outputDirectoryJsonItemName];
                FFmpegPathTextBox.Text = (string)rootNode[c_ffmpegPathJsonItemName];
                DataGridViewColumn sortColumn = ChatRoomsDataGridView.Columns[(string)rootNode[c_sortColumnJsonItemName]];
                sortColumn.HeaderCell.SortGlyphDirection = (SortOrder)Enum.Parse(typeof(SortOrder), (string)rootNode[c_sortDirectionJsonItemName]);

                JsonArray arrayNode = (JsonArray)rootNode[c_chatRoomsJsonItemName];
                for (int i = 0; i < arrayNode.Count; i++)
                {
                    JsonNode chatRoomNode = arrayNode[i];
                    
                    if (chatRoomNode[c_roomUrlJsonItemName] == null ||
                        chatRoomNode[c_actionJsonItemName] == null ||
                        chatRoomNode[c_preferredResolutionJsonItemName] == null)
                    {
                        throw new ArgumentException();
                    }
                    
                    ChatRoom chatRoom = new ChatRoom((string)chatRoomNode[c_roomUrlJsonItemName]);
                    chatRoom.Action = (ChatRoomAction)Enum.Parse(typeof(ChatRoomAction), (string)chatRoomNode[c_actionJsonItemName]);
                    chatRoom.PreferredResolution = (string)chatRoomNode[c_preferredResolutionJsonItemName];
                    chatRoom.OutputDirectory = (string)rootNode[c_outputDirectoryJsonItemName];
                    chatRoom.FFmpegPath = (string)rootNode[c_ffmpegPathJsonItemName];
                    
                    AddDataGridViewRow(chatRoom, i + 1, false);
                }

                ChatRoomsDataGridView.Sort(sortColumn, (ListSortDirection)Enum.Parse(typeof(ListSortDirection), (string)rootNode[c_sortDirectionJsonItemName]));
            }
            catch (Exception)
            {
                OutputDirectoryTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                FFmpegPathTextBox.Text = (new FileInfo(Process.GetCurrentProcess().MainModule.FileName)).Directory.FullName + Path.DirectorySeparatorChar + c_ffmpegBinaryName;
                DataGridViewColumn sortColumn = ChatRoomsDataGridView.Columns[c_indexColumnIndex];
                sortColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending;

                foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                {
                    if (row.Tag != null)
                    {
                        ((ChatRoom)row.Tag).Dispose();
                    }
                }
                ChatRoomsDataGridView.Rows.Clear();

                ChatRoomsDataGridView.Sort(sortColumn, ListSortDirection.Ascending);
            }
        }

        private void WriteConfig()
        {
            try
            {
                Directory.CreateDirectory(_configDir);
                using (FileStream fs = new FileStream(_configDir + Path.DirectorySeparatorChar + c_configFileName, FileMode.Create))
                {
                    Utf8JsonWriter writer = new Utf8JsonWriter(fs);
                    writer.WriteStartObject();
                    writer.WriteString(c_outputDirectoryJsonItemName, OutputDirectoryTextBox.Text);
                    writer.WriteString(c_ffmpegPathJsonItemName, FFmpegPathTextBox.Text);
                    writer.WriteString(c_sortColumnJsonItemName, ChatRoomsDataGridView.SortedColumn.Name);
                    writer.WriteString(c_sortDirectionJsonItemName, ChatRoomsDataGridView.SortOrder.ToString());
                    writer.WriteStartArray(c_chatRoomsJsonItemName);
                    foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                    {
                        ChatRoom chatRoom = (ChatRoom)row.Tag;
                        writer.WriteStartObject();
                        writer.WriteString(c_roomUrlJsonItemName, chatRoom.RoomUrl);
                        writer.WriteString(c_actionJsonItemName, chatRoom.Action.ToString());
                        writer.WriteString(c_preferredResolutionJsonItemName, chatRoom.PreferredResolution);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    writer.Flush();
                }
            }
            catch (Exception)
            {
            }
        }

        private string _configDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + Assembly.GetEntryAssembly().GetName().Name;
        private Object _chatRoomsDataGridViewLock = new object();
        private DateTime _lastUpdate = DateTime.Now;
        private bool _isExiting = false;

        private const string c_configFileName = "config.json";
        private const string c_ffmpegBinaryName = "ffmpeg.exe";

        private const string c_outputDirectoryJsonItemName = "OutputDirectory";
        private const string c_ffmpegPathJsonItemName = "FFmpegPath";
        private const string c_sortColumnJsonItemName = "SortColumn";
        private const string c_sortDirectionJsonItemName = "SortDirection";
        private const string c_chatRoomsJsonItemName = "ChatRooms";
        private const string c_roomUrlJsonItemName = "RoomUrl";
        private const string c_actionJsonItemName = "Action";
        private const string c_preferredResolutionJsonItemName = "PreferredResolution";

        private const int c_indexColumnIndex = 0;
        private const int c_websiteColumnIndex = 1;
        private const int c_nameColumnIndex = 2;
        private const int c_actionColumnIndex = 3;
        private const int c_statusColumnIndex = 4;
        private const int c_resolutionColumnIndex = 5;
        private const int c_lastUpdateColumnIndex = 6;

        private const string c_unsupportedUrlMessage = "The URL is incorrect! Only http://* and https://* URLs are supported!";
        private const string c_unsupportedWebSiteMessage = "The URL is incorrect! Supported websites are Chaturbate and BongaCams!";
        private const string c_duplicateChatRoomMessage = "The chat room is already in the list!";
    }
}