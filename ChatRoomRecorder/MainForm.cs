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

            ChatRoomsDataGridView.Columns[_indexColumnIndex].ValueType = typeof(int);
            ChatRoomsDataGridView.Columns[_websiteColumnIndex].ValueType = typeof(ChatRoomWebsite);
            ChatRoomsDataGridView.Columns[_nameColumnIndex].ValueType = typeof(string);
            ChatRoomsDataGridView.Columns[_actionColumnIndex].ValueType = typeof(ChatRoomAction);
            ChatRoomsDataGridView.Columns[_statusColumnIndex].ValueType = typeof(ChatRoomStatus);
            ChatRoomsDataGridView.Columns[_resolutionColumnIndex].ValueType = typeof(string);
            ChatRoomsDataGridView.Columns[_lastUpdateColumnIndex].ValueType = typeof(string);

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

                    this.Enabled = false;
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
                        row.Cells[_statusColumnIndex].Value = chatRoom.Status;
                        DataGridViewComboBoxCell resolutionCell = (DataGridViewComboBoxCell)row.Cells[_resolutionColumnIndex];
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
                        row.Cells[_lastUpdateColumnIndex].Value = chatRoom.LastUpdate.ToString();
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
                MessageBox.Show("The URL is incorrect! Only http://* and https://* URLs are supported!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    ChatRoomsDataGridView.CurrentCell = firstVisibleRow.Cells[_indexColumnIndex];
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
                    MessageBox.Show("The URL is incorrect! Supported websites are Chaturbate and BongaCams!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                for (int i = 0; i < ChatRoomsDataGridView.Rows.Count && !error; i++)
                {
                    ChatRoom chatRoom = (ChatRoom)ChatRoomsDataGridView.Rows[i].Tag;
                    if (chatRoom.Website == parsedUrl.Item1 && chatRoom.Name == parsedUrl.Item2)
                    {
                        error = true;
                        MessageBox.Show("The chat room is already in the list!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        DataGridViewCell curCell = ChatRoomsDataGridView.Rows[i].Cells[_indexColumnIndex];
                        DataGridViewCell delCell = row.Cells[_indexColumnIndex];
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
                            int oldIndex = (int)row.Cells[_indexColumnIndex].Value;
                            if (int.TryParse((string)e.FormattedValue, out newIndex) && (newIndex >= 1) && (newIndex <= ChatRoomsDataGridView.Rows.Count))
                            {
                                if (newIndex > oldIndex)
                                {
                                    foreach (DataGridViewRow curRow in ChatRoomsDataGridView.Rows)
                                    {
                                        DataGridViewCell curCell = curRow.Cells[_indexColumnIndex];
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
                                        DataGridViewCell curCell = curRow.Cells[_indexColumnIndex];
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
                if (newColumn == oldColumn)
                {
                    if (ChatRoomsDataGridView.SortOrder == SortOrder.Descending)
                    {
                        ChatRoomsDataGridView.Sort(oldColumn, ListSortDirection.Ascending);
                        oldColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    }
                    else
                    {
                        ChatRoomsDataGridView.Sort(oldColumn, ListSortDirection.Descending);
                        oldColumn.HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    }
                }
                else
                {
                    ChatRoomsDataGridView.Sort(newColumn, ListSortDirection.Ascending);
                    newColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }

                SortDataGridView();

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
            OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, Filter = "ffmpeg.exe | ffmpeg.exe" };
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
            row.Cells[_indexColumnIndex].Value = index;
            row.Cells[_websiteColumnIndex].Value = chatRoom.Website;
            row.Cells[_nameColumnIndex].Value = chatRoom.Name;
            DataGridViewComboBoxCell actionCell = (DataGridViewComboBoxCell)row.Cells[_actionColumnIndex];
            actionCell.DataSource = new ChatRoomAction[] { ChatRoomAction.None, ChatRoomAction.Monitor, ChatRoomAction.Record };
            actionCell.Value = chatRoom.Action;
            row.Cells[_statusColumnIndex].Value = chatRoom.Status;
            DataGridViewComboBoxCell resolutionCell = (DataGridViewComboBoxCell)row.Cells[_resolutionColumnIndex];
            resolutionCell.DataSource = new string[] { string.Empty };
            resolutionCell.Value = string.Empty;
            row.Cells[_lastUpdateColumnIndex].Value = chatRoom.LastUpdate.ToString();
            ChatRoomsDataGridView.Rows.Add(row);

            if (current)
            {
                ChatRoomsDataGridView.CurrentCell = row.Cells[_indexColumnIndex];
            }
        }

        private void SortDataGridView()
        {
            if (ChatRoomsDataGridView.SortOrder == SortOrder.Ascending)
            {
                ChatRoomsDataGridView.Sort(ChatRoomsDataGridView.SortedColumn, ListSortDirection.Ascending);
            }
            else
            {
                ChatRoomsDataGridView.Sort(ChatRoomsDataGridView.SortedColumn, ListSortDirection.Descending);
            }

            if (ChatRoomsDataGridView.CurrentCell != null && ChatRoomsDataGridView.CurrentCell.Visible && !ChatRoomsDataGridView.CurrentCell.Displayed)
            {
                ChatRoomsDataGridView.FirstDisplayedCell = ChatRoomsDataGridView.CurrentCell;
            }
        }

        private void ReadConfig()
        {
            try
            {
                JsonNode rootNode = JsonNode.Parse(File.ReadAllText(_configDir + Path.DirectorySeparatorChar + _configFile));

                OutputDirectoryTextBox.Text = (string)rootNode["OutputDirectory"];
                FFmpegPathTextBox.Text = (string)rootNode["FFmpegPath"];

                JsonArray arrayNode = (JsonArray)rootNode["ChatRooms"];
                for (int i = 0; i < arrayNode.Count; i++)
                {
                    JsonNode chatRoomNode = arrayNode[i];
                    ChatRoom chatRoom = new ChatRoom((string)chatRoomNode["RoomUrl"]);
                    chatRoom.Action = (ChatRoomAction)Enum.Parse(typeof(ChatRoomAction), (string)chatRoomNode["Action"]);
                    chatRoom.PreferredResolution = (string)chatRoomNode["PreferredResolution"];
                    chatRoom.OutputDirectory = (string)rootNode["OutputDirectory"];
                    chatRoom.FFmpegPath = (string)rootNode["FFmpegPath"];
                    AddDataGridViewRow(chatRoom, i + 1, false);
                }

                DataGridViewColumn sortColumn = ChatRoomsDataGridView.Columns[(string)rootNode["SortColumn"]];
                ChatRoomsDataGridView.Sort(sortColumn, (ListSortDirection)Enum.Parse(typeof(ListSortDirection), (string)rootNode["SortDirection"]));
                sortColumn.HeaderCell.SortGlyphDirection = (SortOrder)Enum.Parse(typeof(SortOrder), (string)rootNode["SortDirection"]);
            }
            catch (Exception)
            {
                OutputDirectoryTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                FFmpegPathTextBox.Text = (new FileInfo(Process.GetCurrentProcess().MainModule.FileName)).Directory.FullName + Path.DirectorySeparatorChar + "ffmpeg.exe";

                foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                {
                    if (row.Tag != null)
                    {
                        ((ChatRoom)row.Tag).Dispose();
                    }
                }
                ChatRoomsDataGridView.Rows.Clear();

                DataGridViewColumn sortColumn = ChatRoomsDataGridView.Columns[_indexColumnIndex];
                ChatRoomsDataGridView.Sort(sortColumn, ListSortDirection.Ascending);
                sortColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
            }
        }

        private void WriteConfig()
        {
            bool config_saved = true;
            FileStream fs = null;
            try
            {
                Directory.CreateDirectory(_configDir);
                fs = new FileStream(_configDir + Path.DirectorySeparatorChar + _configFile, FileMode.Create);
                Utf8JsonWriter writer = new Utf8JsonWriter(fs);
                writer.WriteStartObject();
                writer.WriteString("SortColumn", ChatRoomsDataGridView.SortedColumn.Name);
                writer.WriteString("SortDirection", ChatRoomsDataGridView.SortOrder.ToString());
                writer.WriteString("OutputDirectory", OutputDirectoryTextBox.Text);
                writer.WriteString("FFmpegPath", FFmpegPathTextBox.Text);
                writer.WriteStartArray("ChatRooms");
                foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                {
                    ChatRoom chatRoom = (ChatRoom)row.Tag;
                    writer.WriteStartObject();
                    writer.WriteString("RoomUrl", chatRoom.RoomUrl);
                    writer.WriteString("Action", chatRoom.Action.ToString());
                    writer.WriteString("PreferredResolution", chatRoom.PreferredResolution);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
                writer.Flush();
            }
            catch (Exception)
            {
                config_saved = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
                if (!config_saved)
                {
                    File.Delete(_configDir + Path.DirectorySeparatorChar + _configFile);
                }
            }
        }

        private string _configDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + Assembly.GetEntryAssembly().GetName().Name;
        private string _configFile = "config.json";
        private Object _chatRoomsDataGridViewLock = new object();
        private DateTime _lastUpdate = DateTime.Now;
        private bool _isExiting = false;

        private const int _indexColumnIndex = 0;
        private const int _websiteColumnIndex = 1;
        private const int _nameColumnIndex = 2;
        private const int _actionColumnIndex = 3;
        private const int _statusColumnIndex = 4;
        private const int _resolutionColumnIndex = 5;
        private const int _lastUpdateColumnIndex = 6;
    }
}
