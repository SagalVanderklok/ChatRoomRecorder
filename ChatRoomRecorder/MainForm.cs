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

namespace ChatRoomRecorder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            Assembly asm = Assembly.GetEntryAssembly();
            this.Text = asm.GetName().Name + " " + asm.GetName().Version.ToString(3);

            ChatRoomsDataGridView.Columns["IndexColumn"].ValueType = typeof(int);
            ChatRoomsDataGridView.Columns["NameColumn"].ValueType = typeof(string);
            ChatRoomsDataGridView.Columns["ActionColumn"].ValueType = typeof(ChatRoomAction);
            ChatRoomsDataGridView.Columns["StatusColumn"].ValueType = typeof(ChatRoomStatus);
            ChatRoomsDataGridView.Columns["ResolutionColumn"].ValueType = typeof(string);

            try
            {
                JsonNode rootNode = JsonNode.Parse(File.ReadAllText(_configDir + Path.DirectorySeparatorChar + _configFile));
                string outputDirectory = (string)rootNode["OutputDirectory"];
                string ffmpegPath = (string)rootNode["FFmpegPath"];
                JsonArray arrayNode = (JsonArray)rootNode["ChatRooms"];
                foreach (JsonNode chatRoomNode in arrayNode)
                {
                    ChatRoom chatRoom = new ChatRoom((string)chatRoomNode["RoomUrl"]);
                    chatRoom.Action = (ChatRoomAction)(int)chatRoomNode["Action"];
                    chatRoom.PreferredResolution = (string)chatRoomNode["PreferredResolution"];
                    chatRoom.OutputDirectory = outputDirectory;
                    chatRoom.FFmpegPath = ffmpegPath;
                    _chatRooms.Add(chatRoom);
                }

                OutputDirectoryTextBox.Text = outputDirectory;
                FFmpegPathTextBox.Text = ffmpegPath;
                for (int i = 0; i < _chatRooms.Count; i++)
                {
                    AddDataGridViewRow(_chatRooms[i], i + 1);
                }
            }
            catch (Exception)
            {
                foreach (ChatRoom chatRoom in _chatRooms)
                {
                    chatRoom.Dispose();
                }
                _chatRooms.Clear();

                OutputDirectoryTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                FFmpegPathTextBox.Text = (new FileInfo(Process.GetCurrentProcess().MainModule.FileName)).Directory.FullName + Path.DirectorySeparatorChar + "ffmpeg.exe";
                ChatRoomsDataGridView.Rows.Clear();
            }

            DataGridViewUpdateTimer.Enabled = true;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            bool config_saved = false;
            FileStream fs = null;
            try
            {
                Directory.CreateDirectory(_configDir);
                fs = new FileStream(_configDir + Path.DirectorySeparatorChar + _configFile, FileMode.Create);
                Utf8JsonWriter writer = new Utf8JsonWriter(fs);
                writer.WriteStartObject();
                writer.WriteString("OutputDirectory", OutputDirectoryTextBox.Text);
                writer.WriteString("FFmpegPath", FFmpegPathTextBox.Text);
                writer.WriteStartArray("ChatRooms");
                for (int i = 0; i < _chatRooms.Count; i++)
                {
                    writer.WriteStartObject();
                    writer.WriteString("RoomUrl", _chatRooms[i].RoomUrl);
                    writer.WriteNumber("Action", (int)_chatRooms[i].Action);
                    writer.WriteString("PreferredResolution", _chatRooms[i].PreferredResolution);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
                writer.Flush();
                config_saved = true;
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            if (!config_saved) File.Delete(_configDir + Path.DirectorySeparatorChar + _configFile);

            foreach (ChatRoom chatRoom in _chatRooms)
            {
                chatRoom.Dispose();
            }
        }

        private void MainForm_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Assembly asm = Assembly.GetEntryAssembly();
            string msg =
                asm.GetName().Name + " " + asm.GetName().Version.ToString(3) + "\n\n" +
                ((AssemblyCopyrightAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(asm, typeof(AssemblyCopyrightAttribute))).Copyright + "\n\n" +
                ((AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(asm, typeof(AssemblyDescriptionAttribute))).Description + "\n\n" +
                "This program is free software: you can redistribute it and/or modify\n" +
                "it under the terms of the GNU General Public License as published by\n" +
                "the Free Software Foundation, either version 3 of the License, or\n" +
                "(at your option) any later version.\n\n" +
                "This program is distributed in the hope that it will be useful,\n" +
                "but WITHOUT ANY WARRANTY; without even the implied warranty of\n" +
                "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the\n" +
                "GNU General Public License for more details.\n\n" +
                "You should have received a copy of the GNU General Public License\n" +
                "along with this program.  If not, see <https://www.gnu.org/licenses/>.\n";
            MessageBox.Show(msg);
            e.Cancel = true;
        }

        private void PlusButton_Click(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                try
                {
                    ChatRoom chatRoom = new ChatRoom(URLTextBox.Text);
                    chatRoom.OutputDirectory = OutputDirectoryTextBox.Text;
                    chatRoom.FFmpegPath = FFmpegPathTextBox.Text;
                    _chatRooms.Add(chatRoom);

                    AddDataGridViewRow(chatRoom, _chatRooms.Count);

                    URLTextBox.Clear();
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to add a chat room! Make sure the link looks like https://chaturbate.com/*", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MinusButton_Click(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (ChatRoomsDataGridView.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = ChatRoomsDataGridView.SelectedRows[0];
                    int columnIndex = ChatRoomsDataGridView.Columns["IndexColumn"].Index;
                    int chatRoomIndex = (int)row.Cells[columnIndex].Value;

                    ChatRoomsDataGridView.Rows.Remove(row);
                    for (int i = 0; i < ChatRoomsDataGridView.Rows.Count; i++)
                    {
                        if ((int)ChatRoomsDataGridView.Rows[i].Cells[columnIndex].Value > chatRoomIndex)
                        {
                            ChatRoomsDataGridView.Rows[i].Cells[columnIndex].Value = (int)ChatRoomsDataGridView.Rows[i].Cells[columnIndex].Value - 1;
                        }
                    }

                    ChatRoom chatRoom = _chatRooms[chatRoomIndex - 1];
                    _chatRooms.Remove(chatRoom);
                    chatRoom.Dispose();
                }
            }
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (ChatRoomsDataGridView.SelectedRows.Count > 0)
                {
                    int columnIndex = ChatRoomsDataGridView.Columns["IndexColumn"].Index;
                    DataGridViewRow selRow = ChatRoomsDataGridView.SelectedRows[0];
                    DataGridViewRow prevRow = null;
                    foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                    {
                        if ((int)selRow.Cells[columnIndex].Value == (int)row.Cells[columnIndex].Value + 1)
                        {
                            prevRow = row;
                            break;
                        }
                    }

                    if (prevRow == null) return;

                    ChatRoom tmp = _chatRooms[(int)prevRow.Cells[columnIndex].Value - 1];
                    _chatRooms[(int)prevRow.Cells[columnIndex].Value - 1] = _chatRooms[(int)selRow.Cells[columnIndex].Value - 1];
                    _chatRooms[(int)selRow.Cells[columnIndex].Value - 1] = tmp;

                    selRow.Cells[columnIndex].Value = (int)selRow.Cells[columnIndex].Value - 1;
                    prevRow.Cells[columnIndex].Value = (int)prevRow.Cells[columnIndex].Value + 1;

                    if (ChatRoomsDataGridView.SortedColumn != null)
                    {
                        if (ChatRoomsDataGridView.SortOrder == SortOrder.Ascending)
                            ChatRoomsDataGridView.Sort(ChatRoomsDataGridView.SortedColumn, ListSortDirection.Ascending);
                        else if (ChatRoomsDataGridView.SortOrder == SortOrder.Descending)
                            ChatRoomsDataGridView.Sort(ChatRoomsDataGridView.SortedColumn, ListSortDirection.Descending);
                    }
                }
            }
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
            {
                if (ChatRoomsDataGridView.SelectedRows.Count > 0)
                {
                    int columnIndex = ChatRoomsDataGridView.Columns["IndexColumn"].Index;
                    DataGridViewRow selRow = ChatRoomsDataGridView.SelectedRows[0];
                    DataGridViewRow nextRow = null;
                    foreach (DataGridViewRow row in ChatRoomsDataGridView.Rows)
                    {
                        if ((int)selRow.Cells[columnIndex].Value == (int)row.Cells[columnIndex].Value - 1)
                        {
                            nextRow = row;
                            break;
                        }
                    }

                    if (nextRow == null) return;

                    ChatRoom tmp = _chatRooms[(int)nextRow.Cells[columnIndex].Value - 1];
                    _chatRooms[(int)nextRow.Cells[columnIndex].Value - 1] = _chatRooms[(int)selRow.Cells[columnIndex].Value - 1];
                    _chatRooms[(int)selRow.Cells[columnIndex].Value - 1] = tmp;

                    selRow.Cells[columnIndex].Value = (int)selRow.Cells[columnIndex].Value + 1;
                    nextRow.Cells[columnIndex].Value = (int)nextRow.Cells[columnIndex].Value - 1;

                    if (ChatRoomsDataGridView.SortedColumn != null)
                    {
                        if (ChatRoomsDataGridView.SortOrder == SortOrder.Ascending)
                            ChatRoomsDataGridView.Sort(ChatRoomsDataGridView.SortedColumn, ListSortDirection.Ascending);
                        else if (ChatRoomsDataGridView.SortOrder == SortOrder.Descending)
                            ChatRoomsDataGridView.Sort(ChatRoomsDataGridView.SortedColumn, ListSortDirection.Descending);
                    }
                }
            }
        }

        private void DataGridViewUpdateTimer_Tick(object sender, EventArgs e)
        {
            lock (_chatRoomsDataGridViewLock)
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
                }
            }
        }

        private void ChatRoomsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = ChatRoomsDataGridView.Rows[e.RowIndex];
                DataGridViewCell cell = row.Cells[e.ColumnIndex];
                ChatRoom chatRoom = _chatRooms[(int)row.Cells[ChatRoomsDataGridView.Columns["IndexColumn"].Index].Value - 1];
                switch (ChatRoomsDataGridView.Columns[e.ColumnIndex].Name)
                {
                    case "ActionColumn":
                        chatRoom.Action = (ChatRoomAction)cell.Value;
                        break;
                    case "ResolutionColumn":
                        chatRoom.PreferredResolution = cell.Value == null ? string.Empty : (string)cell.Value;
                        break;
                }
            }
        }

        private void OutputDirectoryTextBox_Enter(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                OutputDirectoryTextBox.Text = fbd.SelectedPath;
                foreach (ChatRoom chatRoom in _chatRooms)
                {
                    chatRoom.OutputDirectory = fbd.SelectedPath;
                }
            }
        }

        private void FFmpegPathTextBox_Enter(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FFmpegPathTextBox.Text = ofd.FileName;
                foreach (ChatRoom chatRoom in _chatRooms)
                {
                    chatRoom.FFmpegPath = ofd.FileName;
                }
            }
        }

        private void AddDataGridViewRow(ChatRoom chatRoom, int index)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(ChatRoomsDataGridView);
            row.Cells[ChatRoomsDataGridView.Columns["IndexColumn"].Index].Value = index;
            row.Cells[ChatRoomsDataGridView.Columns["NameColumn"].Index].Value = chatRoom.Name;
            DataGridViewComboBoxCell actionCell = (DataGridViewComboBoxCell)row.Cells[ChatRoomsDataGridView.Columns["ActionColumn"].Index];
            actionCell.DataSource = new ChatRoomAction[] { ChatRoomAction.None, ChatRoomAction.Monitor, ChatRoomAction.Record };
            actionCell.Value = chatRoom.Action;
            row.Cells[ChatRoomsDataGridView.Columns["StatusColumn"].Index].Value = chatRoom.Status;
            DataGridViewComboBoxCell resolutionCell = (DataGridViewComboBoxCell)row.Cells[ChatRoomsDataGridView.Columns["ResolutionColumn"].Index];
            resolutionCell.DataSource = new string[] { string.Empty };
            resolutionCell.Value = string.Empty;
            ChatRoomsDataGridView.Rows.Add(row);
        }

        private List<ChatRoom> _chatRooms = new List<ChatRoom>();
        private Object _chatRoomsDataGridViewLock = new object();
        private string _configDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + Assembly.GetEntryAssembly().GetName().Name;
        private string _configFile = "config.json";
    }
}
