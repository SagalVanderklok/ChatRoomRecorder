using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading;
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

            ChatRoomsDataGridView.Columns["IndexColumn"].ValueType = typeof(int);
            ChatRoomsDataGridView.Columns["WebsiteColumn"].ValueType = typeof(ChatRoomWebsite);
            ChatRoomsDataGridView.Columns["NameColumn"].ValueType = typeof(string);
            ChatRoomsDataGridView.Columns["ActionColumn"].ValueType = typeof(ChatRoomAction);
            ChatRoomsDataGridView.Columns["StatusColumn"].ValueType = typeof(ChatRoomStatus);
            ChatRoomsDataGridView.Columns["ResolutionColumn"].ValueType = typeof(string);
            ChatRoomsDataGridView.Columns["LastUpdateColumn"].ValueType = typeof(string);

            ReadConfig();

            ChatRoomsTimer.Start();
            DataGridViewUpdateTimer.Start();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            lock (_chatRoomsLock)
            {
                WriteConfig();

                foreach (ChatRoom chatRoom in _chatRooms)
                {
                    chatRoom.Dispose();
                }

                bool all_disposed = false;
                while (!all_disposed)
                {
                    all_disposed = true;
                    for (int j = 0; j < _chatRooms.Count && !all_disposed; j++)
                    {
                        all_disposed = all_disposed && _chatRooms[j].IsDisposed;
                    }
                    if (!all_disposed)
                    {
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void ChatRoomsTimer_Tick(object sender, EventArgs e)
        {
            lock (_chatRoomsLock)
            {
                if (ChatRoomsDataGridView.IsCurrentCellInEditMode) return;

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
                        chatRoom.Update();
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
            lock (_chatRoomsLock)
            {
                if (ChatRoomsDataGridView.IsCurrentCellInEditMode) return;

                foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                {
                    ChatRoom chatRoom = _chatRooms[(int)row.Cells[ChatRoomsDataGridView.Columns["IndexColumn"].Index].Value - 1];
                    DataGridViewTextBoxCell statusCell = (DataGridViewTextBoxCell)row.Cells[ChatRoomsDataGridView.Columns["StatusColumn"].Index];
                    statusCell.Value = chatRoom.Status;
                    DataGridViewComboBoxCell resolutionCell = (DataGridViewComboBoxCell)row.Cells[ChatRoomsDataGridView.Columns["ResolutionColumn"].Index];
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
                    DataGridViewTextBoxCell lastUpdateCell = (DataGridViewTextBoxCell)row.Cells[ChatRoomsDataGridView.Columns["LastUpdateColumn"].Index];
                    lastUpdateCell.Value = chatRoom.LastUpdate.ToString();
                }

                SortDataGridView();
            }
        }

        private void PlusButton_Click(object sender, EventArgs e)
        {
            lock (_chatRoomsLock)
            {
                Tuple<ChatRoomWebsite, string> parsed_url = ChatRoom.ParseUrl(URLTextBox.Text);

                if (parsed_url == null)
                {
                    MessageBox.Show("The URL is incorrect! Supported websites are Chaturbate and BongaCams!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ChatRoomWebsite website = parsed_url.Item1;
                string name = parsed_url.Item2;
                for (int i = 0; i < _chatRooms.Count; i++)
                {
                    if (_chatRooms[i].Website == website && _chatRooms[i].Name == name)
                    {
                        MessageBox.Show("The chat room is already in the list!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                ChatRoom chatRoom = new ChatRoom(URLTextBox.Text);
                chatRoom.OutputDirectory = OutputDirectoryTextBox.Text;
                chatRoom.FFmpegPath = FFmpegPathTextBox.Text;
                _chatRooms.Add(chatRoom);

                URLTextBox.Clear();

                AddDataGridViewRow(chatRoom, _chatRooms.Count);

                SortDataGridView();

                WriteConfig();
            }
        }

        private void MinusButton_Click(object sender, EventArgs e)
        {
            lock (_chatRoomsLock)
            {
                if (ChatRoomsDataGridView.SelectedRows.Count > 0)
                {
                    ChatRoomsDataGridView.CellValidating -= ChatRoomsDataGridView_CellValidating;
                    ChatRoomsDataGridView.CellValueChanged -= ChatRoomsDataGridView_CellValueChanged;

                    DataGridViewRow row = ChatRoomsDataGridView.SelectedRows[0];
                    int columnIndex = ChatRoomsDataGridView.Columns["IndexColumn"].Index;
                    int chatRoomIndex = (int)row.Cells[columnIndex].Value;

                    ChatRoom chatRoom = _chatRooms[chatRoomIndex - 1];
                    _chatRooms.Remove(chatRoom);
                    chatRoom.Dispose();

                    ChatRoomsDataGridView.Rows.Remove(row);
                    for (int i = 0; i < ChatRoomsDataGridView.Rows.Count; i++)
                    {
                        if ((int)ChatRoomsDataGridView.Rows[i].Cells[columnIndex].Value > chatRoomIndex)
                            ChatRoomsDataGridView.Rows[i].Cells[columnIndex].Value = (int)ChatRoomsDataGridView.Rows[i].Cells[columnIndex].Value - 1;
                    }

                    ChatRoomsDataGridView.CellValidating += ChatRoomsDataGridView_CellValidating;
                    ChatRoomsDataGridView.CellValueChanged += ChatRoomsDataGridView_CellValueChanged;

                    WriteConfig();
                }
            }
        }

        private void ChatRoomsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            lock (_chatRoomsLock)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && ChatRoomsDataGridView.Columns[e.ColumnIndex].Name == "IndexColumn")
                {
                    int index;
                    if (!int.TryParse((string)e.FormattedValue, out index) || (index < 1) || (index > _chatRooms.Count))
                        e.Cancel = true;
                }

            }
        }

        private void ChatRoomsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            lock (_chatRoomsLock)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    DataGridViewRow row = ChatRoomsDataGridView.Rows[e.RowIndex];
                    DataGridViewCell cell = row.Cells[e.ColumnIndex];
                    ChatRoom chatRoom = (ChatRoom)row.Tag;
                    switch (ChatRoomsDataGridView.Columns[e.ColumnIndex].Name)
                    {
                        case "ActionColumn":
                            chatRoom.Action = (ChatRoomAction)cell.Value;
                            break;
                        case "ResolutionColumn":
                            chatRoom.PreferredResolution = cell.Value == null ? string.Empty : (string)cell.Value;
                            break;
                        case "IndexColumn":
                            ChatRoomsDataGridView.CellValidating -= ChatRoomsDataGridView_CellValidating;
                            ChatRoomsDataGridView.CellValueChanged -= ChatRoomsDataGridView_CellValueChanged;

                            int colIndex = ChatRoomsDataGridView.Columns["IndexColumn"].Index;
                            int oldIndex = _chatRooms.IndexOf(chatRoom) + 1;
                            int newIndex = (int)cell.Value;

                            if (newIndex > oldIndex)
                            {
                                for (int i = oldIndex; i < newIndex; i++)
                                {
                                    _chatRooms[i - 1] = _chatRooms[i];
                                }
                                foreach (DataGridViewRow curRow in ChatRoomsDataGridView.Rows)
                                {
                                    DataGridViewCell curCell = curRow.Cells[colIndex];
                                    if ((int)curCell.Value > oldIndex && (int)curCell.Value <= newIndex)
                                    {
                                        curCell.Value = (int)curCell.Value - 1;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = oldIndex - 1; i >= newIndex; i--)
                                {
                                    _chatRooms[i] = _chatRooms[i - 1];
                                }
                                foreach (DataGridViewRow curRow in ChatRoomsDataGridView.Rows)
                                {
                                    DataGridViewCell curCell = curRow.Cells[colIndex];
                                    if ((int)curCell.Value >= newIndex && (int)curCell.Value < oldIndex)
                                    {
                                        curCell.Value = (int)curCell.Value + 1;
                                    }
                                }
                            }
                            _chatRooms[newIndex - 1] = chatRoom;
                            cell.Value = newIndex;

                            ChatRoomsDataGridView.CellValidating += ChatRoomsDataGridView_CellValidating;
                            ChatRoomsDataGridView.CellValueChanged += ChatRoomsDataGridView_CellValueChanged;
                            break;
                    }

                    SortDataGridView();

                    WriteConfig();
                }
            }
        }

        private void ChatRoomsDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (_chatRoomsLock)
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
                lock (_chatRoomsLock)
                {
                    OutputDirectoryTextBox.Text = fbd.SelectedPath;
                    foreach (ChatRoom chatRoom in _chatRooms)
                    {
                        chatRoom.OutputDirectory = fbd.SelectedPath;
                    }

                    WriteConfig();
                }
            }
        }

        private void FFmpegPathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, Filter = "ffmpeg.exe | ffmpeg.exe" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lock (_chatRoomsLock)
                {
                    FFmpegPathTextBox.Text = ofd.FileName;
                    foreach (ChatRoom chatRoom in _chatRooms)
                    {
                        chatRoom.FFmpegPath = ofd.FileName;
                    }

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
            row.Cells[ChatRoomsDataGridView.Columns["IndexColumn"].Index].Value = index;
            row.Cells[ChatRoomsDataGridView.Columns["WebsiteColumn"].Index].Value = chatRoom.Website;
            row.Cells[ChatRoomsDataGridView.Columns["NameColumn"].Index].Value = chatRoom.Name;
            DataGridViewComboBoxCell actionCell = (DataGridViewComboBoxCell)row.Cells[ChatRoomsDataGridView.Columns["ActionColumn"].Index];
            actionCell.DataSource = new ChatRoomAction[] { ChatRoomAction.None, ChatRoomAction.Monitor, ChatRoomAction.Record };
            actionCell.Value = chatRoom.Action;
            row.Cells[ChatRoomsDataGridView.Columns["StatusColumn"].Index].Value = chatRoom.Status;
            DataGridViewComboBoxCell resolutionCell = (DataGridViewComboBoxCell)row.Cells[ChatRoomsDataGridView.Columns["ResolutionColumn"].Index];
            resolutionCell.DataSource = new string[] { string.Empty };
            resolutionCell.Value = string.Empty;
            row.Cells[ChatRoomsDataGridView.Columns["LastUpdateColumn"].Index].Value = chatRoom.LastUpdate.ToString();
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

                string outputDirectory = (string)rootNode["OutputDirectory"];
                string ffmpegPath = (string)rootNode["FFmpegPath"];
                JsonArray arrayNode = (JsonArray)rootNode["ChatRooms"];

                foreach (JsonNode chatRoomNode in arrayNode)
                {
                    ChatRoom chatRoom = new ChatRoom((string)chatRoomNode["RoomUrl"]);
                    chatRoom.Action = (ChatRoomAction)Enum.Parse(typeof(ChatRoomAction), (string)chatRoomNode["Action"]);
                    chatRoom.PreferredResolution = (string)chatRoomNode["PreferredResolution"];
                    chatRoom.OutputDirectory = outputDirectory;
                    chatRoom.FFmpegPath = ffmpegPath;
                    _chatRooms.Add(chatRoom);
                }

                for (int i = 0; i < _chatRooms.Count; i++)
                {
                    AddDataGridViewRow(_chatRooms[i], i + 1);
                }
                DataGridViewColumn sortColumn = ChatRoomsDataGridView.Columns[(string)rootNode["SortColumn"]];
                string sortDirection = (string)rootNode["SortDirection"];
                ChatRoomsDataGridView.Sort(sortColumn, (ListSortDirection)Enum.Parse(typeof(ListSortDirection), sortDirection));
                sortColumn.HeaderCell.SortGlyphDirection = (SortOrder)Enum.Parse(typeof(SortOrder), sortDirection);

                OutputDirectoryTextBox.Text = outputDirectory;
                FFmpegPathTextBox.Text = ffmpegPath;
            }
            catch (Exception)
            {
                foreach (ChatRoom chatRoom in _chatRooms)
                {
                    chatRoom.Dispose();
                }
                _chatRooms.Clear();

                ChatRoomsDataGridView.Rows.Clear();
                DataGridViewColumn sortColumn = ChatRoomsDataGridView.Columns["IndexColumn"];
                ChatRoomsDataGridView.Sort(sortColumn, ListSortDirection.Ascending);
                sortColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending;

                OutputDirectoryTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                FFmpegPathTextBox.Text = (new FileInfo(Process.GetCurrentProcess().MainModule.FileName)).Directory.FullName + Path.DirectorySeparatorChar + "ffmpeg.exe";
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
                for (int i = 0; i < _chatRooms.Count; i++)
                {
                    writer.WriteStartObject();
                    writer.WriteString("RoomUrl", _chatRooms[i].RoomUrl);
                    writer.WriteString("Action", _chatRooms[i].Action.ToString());
                    writer.WriteString("PreferredResolution", _chatRooms[i].PreferredResolution);
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
        private List<ChatRoom> _chatRooms = new List<ChatRoom>();
        private DateTime _lastUpdate = DateTime.Now;
        private Object _chatRoomsLock = new object();
    }
}
