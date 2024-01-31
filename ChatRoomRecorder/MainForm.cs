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
using System.Collections.Generic;

namespace ChatRoomRecorder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();  

            Text = Assembly.GetEntryAssembly().GetName().Name + " " + Assembly.GetEntryAssembly().GetName().Version.ToString(3);
            Assembly asm = Assembly.GetEntryAssembly();

            LicenseTextBox.Text =
                ((AssemblyCopyrightAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(asm, typeof(AssemblyCopyrightAttribute))).Copyright +
                "\r\n\r\n" +
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

            ChatRoomsDataGridView.Columns[c_indexColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
            ChatRoomsDataGridView.Sort(ChatRoomsDataGridView.Columns[c_indexColumnIndex], ListSortDirection.Ascending);

            FilesDataGridView.Columns[c_fileNameColumnIndex].ValueType = typeof(string);
            FilesDataGridView.Columns[c_fileSizeColumnIndex].ValueType = typeof(string);

            FilesDataGridView.Columns[c_fileNameColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
            FilesDataGridView.Sort(FilesDataGridView.Columns[c_fileNameColumnIndex], ListSortDirection.Ascending);
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            lock (_lock)
            {
                if (e.IsSuccess)
                {
                    ToggleCellEventHandling(false);

                    ReadConfig();

                    ChatRoomsUpdateTimer.Start();

                    TabControl.Enabled = true;

                    ToggleCellEventHandling(true);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (_lock)
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

        private void GoButton_Click(object sender, EventArgs e)
        {
            if (WebView.CoreWebView2 != null)
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
            lock (_lock)
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
            lock (_lock)
            {
                Tuple<ChatRoomWebsite, string, string> parsedUrl = ChatRoom.ParseUrl(URLTextBox.Text);
                if (parsedUrl != null)
                {
                    bool duplicate = false;
                    
                    for (int i = 0; i < ChatRoomsDataGridView.Rows.Count && !duplicate; i++)
                    {
                        ChatRoom chatRoom = (ChatRoom)ChatRoomsDataGridView.Rows[i].Tag;
                        if (chatRoom.Website == parsedUrl.Item1 && chatRoom.Name == parsedUrl.Item2)
                        {
                            duplicate = true;
                        }
                    }

                    if (!duplicate)
                    {
                        ToggleCellEventHandling(false);

                        ChatRoom chatRoom = new(URLTextBox.Text) { OutputDirectory = OutputDirectoryTextBox.Text, FFmpegPath = FFmpegPathTextBox.Text };

                        URLTextBox.Clear();

                        AddChatRoom(chatRoom, ChatRoomsDataGridView.Rows.Count + 1, true);

                        WriteConfig();

                        ToggleCellEventHandling(true);
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
        }

        private void ChatRoomsUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (Monitor.TryEnter(_lock))
            {
                if (ChatRoomsUpdateTimer.Enabled)
                {
                    ToggleCellEventHandling(false);

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

                    foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                    {
                        if (ChatRoomsDataGridView.CurrentCell == null || !ChatRoomsDataGridView.IsCurrentCellInEditMode || ChatRoomsDataGridView.CurrentCell.OwningRow != row)
                        {
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
                            row.Cells[c_lastUpdateColumnIndex].Value = chatRoom.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }

                    ToggleCellEventHandling(true);
                }

                Monitor.Exit(_lock);
            }
        }

        private void ChatRoomsDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (_lock)
            {
                SortDataGridView(ChatRoomsDataGridView, ChatRoomsDataGridView.Columns[e.ColumnIndex]);
            }
        }

        private void ChatRoomsDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (_lock)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (MessageBox.Show(c_confirmChatRoomsRemovalMessage, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        ToggleCellEventHandling(false);

                        foreach (DataGridViewRow delRow in ChatRoomsDataGridView.SelectedRows)
                        {
                            foreach (DataGridViewRow curRow in ChatRoomsDataGridView.Rows)
                            {
                                DataGridViewCell curCell = curRow.Cells[c_indexColumnIndex];
                                DataGridViewCell delCell = delRow.Cells[c_indexColumnIndex];
                                if ((int)curCell.Value > (int)delCell.Value)
                                {
                                    curCell.Value = (int)curCell.Value - 1;
                                }
                            }

                            ((ChatRoom)delRow.Tag).Dispose();
                            
                            ChatRoomsDataGridView.Rows.Remove(delRow);
                        }

                        WriteConfig();

                        ToggleCellEventHandling(true);
                    }
                }
            }
        }

        private void ChatRoomsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            lock (_lock)
            {
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
                                ToggleCellEventHandling(false);

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

                                ToggleCellEventHandling(true);
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
            }
        }

        private void ChatRoomsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            lock (_lock)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    WriteConfig();
                }
            }
        }

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

        private void ChatRoomsDataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                FilesDataGridView.Rows.Clear();

                if (ChatRoomsDataGridView.CurrentCell != null)
                {
                    try
                    {
                        ChatRoom chatRoom = (ChatRoom)ChatRoomsDataGridView.CurrentCell.OwningRow.Tag;
                        DirectoryInfo dir = new(OutputDirectoryTextBox.Text);
                        FileInfo[] files = dir.GetFiles(string.Format("{0} {1} *", chatRoom.Website, chatRoom.Name));
                        foreach (FileInfo file in files)
                        {
                            DataGridViewRow row = new();
                            row.CreateCells(FilesDataGridView);
                            row.Cells[c_fileNameColumnIndex].Value = file.Name;
                            row.Cells[c_fileSizeColumnIndex].Value = file.Length / 1024;
                            FilesDataGridView.Rows.Add(row);
                        }

                        SortDataGridView(FilesDataGridView, null);
                    }
                    catch (Exception)
                    {
                        FilesDataGridView.Rows.Clear();
                    }
                }
            }
        }

        private void FilesDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (_lock)
            {
                SortDataGridView(FilesDataGridView, FilesDataGridView.Columns[e.ColumnIndex]);
            }
        }

        private void FilesDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (_lock)
            {
                if (e.RowIndex >= 0)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo()
                        {
                            UseShellExecute = true,
                            FileName = OutputDirectoryTextBox.Text + Path.DirectorySeparatorChar + FilesDataGridView.Rows[e.RowIndex].Cells[c_fileNameColumnIndex].Value.ToString()
                        });
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(c_fileOpenErrorMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void FilesDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (_lock)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (MessageBox.Show(c_confirmFilesRemovalMessage, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        try
                        {
                            foreach (DataGridViewRow delRow in FilesDataGridView.SelectedRows)
                            {
                                File.Delete(OutputDirectoryTextBox.Text + Path.DirectorySeparatorChar + delRow.Cells[c_fileNameColumnIndex].Value.ToString());
                                FilesDataGridView.Rows.Remove(delRow);
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show(c_fileRemoveErrorMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void OutputDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                lock (_lock)
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
            OpenFileDialog ofd = new() { Multiselect = false, Filter = string.Format("{0}|{0}", c_ffmpegBinaryName) };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lock (_lock)
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

        private void AddChatRoom(ChatRoom chatRoom, int index, bool current)
        {
            DataGridViewRow row = new();
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

        private void ToggleCellEventHandling(bool state)
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

        private void SortDataGridView(DataGridView dataGridView, DataGridViewColumn column)
        {
            if (column == null)
            {
                if (dataGridView.SortedColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                {
                    dataGridView.Sort(dataGridView.SortedColumn, ListSortDirection.Ascending);
                }
                else
                {
                    dataGridView.Sort(dataGridView.SortedColumn, ListSortDirection.Descending);
                }
            }
            else
            {
                if (dataGridView.SortedColumn != column)
                {
                    dataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }

                if (column.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                {
                    column.HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    dataGridView.Sort(column, ListSortDirection.Descending);
                }
                else
                {
                    column.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    dataGridView.Sort(column, ListSortDirection.Ascending);
                }
            }

            if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.Visible && !dataGridView.CurrentCell.Displayed)
            {
                dataGridView.FirstDisplayedCell = dataGridView.CurrentCell;
            }
        }

        private void ReadConfig()
        {
            ChatRoom chatRoom = null;
            try
            {
                JsonNode rootNode = JsonNode.Parse(File.ReadAllText(_configDir + Path.DirectorySeparatorChar + c_configFileName));

                OutputDirectoryTextBox.Text = (string)rootNode[c_outputDirectoryJsonItemName];
                FFmpegPathTextBox.Text = (string)rootNode[c_ffmpegPathJsonItemName];

                JsonArray arrayNode = (JsonArray)rootNode[c_chatRoomsJsonItemName];
                for (int i = 0; i < arrayNode.Count; i++)
                {
                    JsonNode chatRoomNode = arrayNode[i];

                    chatRoom = new((string)chatRoomNode[c_roomUrlJsonItemName])
                    {
                        Action = (ChatRoomAction)Enum.Parse(typeof(ChatRoomAction), (string)chatRoomNode[c_actionJsonItemName]),
                        PreferredResolution = (string)chatRoomNode[c_preferredResolutionJsonItemName],
                        OutputDirectory = (string)rootNode[c_outputDirectoryJsonItemName],
                        FFmpegPath = (string)rootNode[c_ffmpegPathJsonItemName]
                    };

                    AddChatRoom(chatRoom, i + 1, false);
                }
            }
            catch (Exception)
            {
                OutputDirectoryTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                FFmpegPathTextBox.Text = (new FileInfo(Environment.ProcessPath)).Directory.FullName + Path.DirectorySeparatorChar + c_ffmpegBinaryName;

                foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                {
                    if (row.Tag != null)
                    {
                        ((ChatRoom)row.Tag).Dispose();
                    }
                }
                if (chatRoom != null)
                {
                    chatRoom.Dispose();
                }

                ChatRoomsDataGridView.Rows.Clear();
            }
        }

        private void WriteConfig()
        {
            try
            {
                SortedList<int, ChatRoom> chatRooms = new();
                foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                {
                    chatRooms.Add((int)row.Cells[c_indexColumnIndex].Value, (ChatRoom)row.Tag);
                }

                Directory.CreateDirectory(_configDir);
                using (FileStream fs = new(_configDir + Path.DirectorySeparatorChar + c_configFileName, FileMode.Create))
                {
                    Utf8JsonWriter writer = new(fs);
                    writer.WriteStartObject();
                    writer.WriteString(c_outputDirectoryJsonItemName, OutputDirectoryTextBox.Text);
                    writer.WriteString(c_ffmpegPathJsonItemName, FFmpegPathTextBox.Text);
                    writer.WriteStartArray(c_chatRoomsJsonItemName);
                    foreach (ChatRoom chatRoom in chatRooms.Values)
                    {
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
                //do nothing
            }
        }

        private string _configDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + Assembly.GetEntryAssembly().GetName().Name;
        private Object _lock = new();
        private DateTime _lastUpdate = DateTime.Now;
        private bool _isExiting = false;

        private const string c_configFileName = "config.json";
        private const string c_ffmpegBinaryName = "ffmpeg.exe";

        private const string c_outputDirectoryJsonItemName = "OutputDirectory";
        private const string c_ffmpegPathJsonItemName = "FFmpegPath";
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

        private const int c_fileNameColumnIndex = 0;
        private const int c_fileSizeColumnIndex = 1;

        private const string c_unsupportedUrlMessage = "The URL is incorrect! Only http://* and https://* URLs are supported!";
        private const string c_unsupportedWebSiteMessage = "The URL is incorrect! Supported websites are Chaturbate and BongaCams!";
        private const string c_duplicateChatRoomMessage = "The chat room is already in the list!";
        private const string c_fileOpenErrorMessage = "The file can't be opened!";
        private const string c_confirmFilesRemovalMessage = "Do you really want to remove selected files?";
        private const string c_fileRemoveErrorMessage = "The file can't be removed!";
        private const string c_confirmChatRoomsRemovalMessage = "Do you really want to remove selected chat rooms?";
    }
}