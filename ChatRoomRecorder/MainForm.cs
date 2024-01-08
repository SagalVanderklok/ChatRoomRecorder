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

            ChatRoomsUpdateTimer.Start();
            DataGridViewUpdateTimer.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (!FormCloseTimer.Enabled)
                {
                    WriteConfig();

                    foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                    {
                        ((ChatRoom)row.Tag).Dispose();
                    }

                    ChatRoomsUpdateTimer.Stop();
                    DataGridViewUpdateTimer.Stop();
                    FormCloseTimer.Start();

                    this.Visible = false;
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
            lock (_chatRoomsDataGridViewLock)
            {
                if (FormCloseTimer.Enabled)
                {
                    this.Close();
                }
            }
        }

        private void ChatRoomsUpdateTimer_Tick(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (!ChatRoomsUpdateTimer.Enabled || ChatRoomsDataGridView.IsCurrentCellInEditMode) return;

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
        }

        private void DataGridViewUpdateTimer_Tick(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (!DataGridViewUpdateTimer.Enabled || ChatRoomsDataGridView.IsCurrentCellInEditMode) return;

                foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                {
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

                SortDataGridView();
            }
        }

        private void PlusButton_Click(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                Tuple<ChatRoomWebsite, string> parsedUrl = ChatRoom.ParseUrl(URLTextBox.Text);

                if (parsedUrl == null)
                {
                    MessageBox.Show("The URL is incorrect! Supported websites are Chaturbate and BongaCams!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                {
                    ChatRoom chatRoom = (ChatRoom)row.Tag;
                    if (chatRoom.Website == parsedUrl.Item1 && chatRoom.Name == parsedUrl.Item2)
                    {
                        MessageBox.Show("The chat room is already in the list!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                AddDataGridViewRow(new ChatRoom(URLTextBox.Text) { OutputDirectory = OutputDirectoryTextBox.Text, FFmpegPath = FFmpegPathTextBox.Text }, ChatRoomsDataGridView.Rows.Count);

                URLTextBox.Clear();

                SortDataGridView();

                WriteConfig();
            }
        }

        private void MinusButton_Click(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (ChatRoomsDataGridView.SelectedRows.Count > 0)
                {
                    ChatRoomsDataGridView.CellValidating -= ChatRoomsDataGridView_CellValidating;
                    ChatRoomsDataGridView.CellValueChanged -= ChatRoomsDataGridView_CellValueChanged;

                    DataGridViewRow row = ChatRoomsDataGridView.SelectedRows[0];

                    for (int i = 0; i < ChatRoomsDataGridView.Rows.Count; i++)
                    {
                        DataGridViewCell curCell = ChatRoomsDataGridView.Rows[i].Cells[_indexColumnIndex];
                        DataGridViewCell delCell = row.Cells[_indexColumnIndex];
                        if ((int)curCell.Value > (int)delCell.Value)
                            curCell.Value = (int)curCell.Value - 1;
                    }

                    ((ChatRoom)row.Tag).Dispose();
                    ChatRoomsDataGridView.Rows.Remove(row);

                    ChatRoomsDataGridView.CellValidating += ChatRoomsDataGridView_CellValidating;
                    ChatRoomsDataGridView.CellValueChanged += ChatRoomsDataGridView_CellValueChanged;

                    WriteConfig();
                }
            }
        }

        private void ChatRoomsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    DataGridViewRow row = ChatRoomsDataGridView.Rows[e.RowIndex];
                    ChatRoom chatRoom = (ChatRoom)row.Tag;
                    switch (e.ColumnIndex)
                    {
                        case 0:  //Index
                            ChatRoomsDataGridView.CellValidating -= ChatRoomsDataGridView_CellValidating;
                            ChatRoomsDataGridView.CellValueChanged -= ChatRoomsDataGridView_CellValueChanged;

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

                            ChatRoomsDataGridView.CellValidating += ChatRoomsDataGridView_CellValidating;
                            ChatRoomsDataGridView.CellValueChanged += ChatRoomsDataGridView_CellValueChanged;
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
            lock (_chatRoomsDataGridViewLock)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    SortDataGridView();

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

        private void AddDataGridViewRow(ChatRoom chatRoom, int index)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.Tag = chatRoom;
            row.CreateCells(ChatRoomsDataGridView);
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
        }

        private void SortDataGridView()
        {
            if (ChatRoomsDataGridView.SortOrder == SortOrder.Ascending)
                ChatRoomsDataGridView.Sort(ChatRoomsDataGridView.SortedColumn, ListSortDirection.Ascending);
            else
                ChatRoomsDataGridView.Sort(ChatRoomsDataGridView.SortedColumn, ListSortDirection.Descending);
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
                    AddDataGridViewRow(chatRoom, i + 1);
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
                    if (row.Tag != null) ((ChatRoom)row.Tag).Dispose();
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
                if (fs != null) fs.Close();
                if (!config_saved) File.Delete(_configDir + Path.DirectorySeparatorChar + _configFile);
            }
        }

        private string _configDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + Assembly.GetEntryAssembly().GetName().Name;
        private string _configFile = "config.json";
        private Object _chatRoomsDataGridViewLock = new object();
        private DateTime _lastUpdate = DateTime.Now;

        private const int _indexColumnIndex = 0;
        private const int _websiteColumnIndex = 1;
        private const int _nameColumnIndex = 2;
        private const int _actionColumnIndex = 3;
        private const int _statusColumnIndex = 4;
        private const int _resolutionColumnIndex = 5;
        private const int _lastUpdateColumnIndex = 6;
    }
}
