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
using System.Threading;
using System.Windows.Forms;

namespace ChatRoomRecorder
{
    public partial class MainForm : Form
    {
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

                    foreach (ChatRoom chatRoom in _chatRoomsList.UnfilteredItems)
                    {
                        chatRoom.Dispose();
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

        private void PlusButton_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                AddChatRoom(UrlTextBox.Text);
            }
        }

        private void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            lock (_lock)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    AddChatRoom(UrlTextBox.Text);
                }
            }
        }

        private void UrlTextBox_TextChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                _chatRoomsList.Filter = UrlTextBox.Text;
            }
        }

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

                    foreach (ChatRoom chatRoom in _chatRoomsList.UnfilteredItems)
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

                WriteData(c_chatRoomsTableName, (ChatRoom)sender, 0);
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

        private void ChatRoomsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            lock (_lock)
            {
                if (e.RowIndex >= 0)
                {
                    WriteData(c_chatRoomsTableName, _chatRoomsList[e.RowIndex], 0);
                }
            }
        }

        private void ChatRoomsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                _filesList.Clear();

                foreach (DataGridViewRow selectedRow in ChatRoomsDataGridView.SelectedRows)
                {
                    try
                    {
                        ChatRoom chatRoom = _chatRoomsList[selectedRow.Index];
                        DirectoryInfo directory = new(_settings.OutputDirectory);
                        FileInfo[] files = directory.GetFiles(string.Format("{0} {1} *", chatRoom.Website, chatRoom.Name));
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
        }

        private void ChatRoomsDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (_lock)
            {
                if (e.Button == MouseButtons.Right)
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
                            _chatRoomsList.Remove(chatRoom);
                            WriteData(c_chatRoomsTableName, chatRoom, -1);
                            chatRoom.Dispose();
                        }
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
                            FileName = (_filesList[e.RowIndex]).FullName
                        });
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(c_fileOpeningErrorMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ThumbnailPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            lock (_lock)
            {
                if (e.Button == MouseButtons.Right && ThumbnailPictureBox.Image != null)
                {
                    if (MessageBox.Show(c_confirmThumbnailRemovingMessage, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
        }

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
                        foreach (ChatRoom chatRoom in _chatRoomsList.UnfilteredItems)
                        {
                            chatRoom.OutputDirectory = _settings.OutputDirectory;
                        }
                        break;

                    case c_ffmpegPathSettingName:
                        foreach (ChatRoom chatRoom in _chatRoomsList.UnfilteredItems)
                        {
                            chatRoom.FFmpegPath = _settings.FFmpegPath;
                        }
                        break;

                    case c_streamlinkPathSettingName:
                        foreach (ChatRoom chatRoom in _chatRoomsList.UnfilteredItems)
                        {
                            chatRoom.StreamlinkPath = _settings.StreamlinkPath;
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

                WriteData(c_settingsTableName);
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

        private void AddChatRoom(string url)
        {
            Tuple<ChatRoomWebsite, string, string> parsedUrl = ChatRoom.ParseUrl(url);

            if (parsedUrl != null)
            {
                bool duplicate = false;

                foreach (ChatRoom chatRoom in _chatRoomsList.UnfilteredItems)
                {
                    if (chatRoom.Website == parsedUrl.Item1 && chatRoom.Name == parsedUrl.Item2)
                    {
                        duplicate = true;
                        break;
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
                    _chatRoomsList.Add(chatRoom);
                    WriteData(c_chatRoomsTableName, chatRoom, 1);
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

        private void ReadData()
        {
            try
            {
                _connection = new SqliteConnection(string.Format("data source={0}", c_databaseFileName));
                _connection.Open();

                SqliteCommand command = _connection.CreateCommand();

                command.CommandText = string.Format("create table if not exists {0} ({1} text,{2} text,{3} int,{4} int,{5} int);",
                    c_settingsTableName,
                    c_outputDirectorySettingName,
                    c_ffmpegPathSettingName,
                    c_chaturbateConcurrentUpdatesSettingName,
                    c_bongaCamsConcurrentUpdatesSettingName,
                    c_updateIntervalSettingName);
                command.ExecuteNonQuery();

                command.CommandText = string.Format("create table if not exists {0} ({1} text,{2} int,{3} int,{4} int,{5} int);",
                    c_chatRoomsTableName,
                    c_urlPropertyName,
                    c_actionPropertyName,
                    c_resolutionPropertyName,
                    c_updatedPropertyName,
                    c_seenPropertyName);
                command.ExecuteNonQuery();

                bool schemaIsUpdated = false;
                do
                {
                    command.CommandText = "select * from pragma_user_version;";
                    int version = Convert.ToInt32(command.ExecuteScalar());
                    switch (version)
                    {
                        case 0:
                            command.CommandText =
                                string.Format("alter table {0} add {1} int;", c_settingsTableName, c_stripchatConcurrentUpdatesSettingName) +
                                string.Format("alter table {0} add {1} int;", c_settingsTableName, c_defaultActionSettingName) +
                                string.Format("alter table {0} add {1} int;", c_settingsTableName, c_defaultResolutionSettingName) +
                                "pragma user_version = 1;";
                            command.ExecuteNonQuery();
                            break;
                        case 1:
                            command.CommandText =
                                string.Format("alter table {0} add {1} text;", c_settingsTableName, c_streamlinkPathSettingName) +
                                "pragma user_version = 2;";
                            command.ExecuteNonQuery();
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

            try
            {
                SqliteCommand command = _connection.CreateCommand();
                command.CommandText = string.Format("select * from {0}", c_settingsTableName);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _settings.OutputDirectory = reader.GetString(c_outputDirectorySettingName);
                        _settings.FFmpegPath = reader.GetString(c_ffmpegPathSettingName);
                        _settings.StreamlinkPath = reader.GetString(c_streamlinkPathSettingName);
                        _settings.ChaturbateConcurrentUpdates = reader.GetInt32(c_chaturbateConcurrentUpdatesSettingName);
                        _settings.BongaCamsConcurrentUpdates = reader.GetInt32(c_bongaCamsConcurrentUpdatesSettingName);
                        _settings.StripchatConcurrentUpdates = reader.GetInt32(c_stripchatConcurrentUpdatesSettingName);
                        _settings.UpdateInterval = reader.GetInt32(c_updateIntervalSettingName);
                        _settings.DefaultAction = (ChatRoomAction)reader.GetInt32(c_defaultActionSettingName);
                        _settings.DefaultResolution = ChatRoomResolution.Parse(reader.GetInt32(c_defaultResolutionSettingName));
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

            try
            {
                SqliteCommand command = _connection.CreateCommand();
                command.CommandText = string.Format("select * from {0}", c_chatRoomsTableName);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string url = reader.GetString(c_urlPropertyName);
                        ChatRoomAction action = (ChatRoomAction)reader.GetInt32(c_actionPropertyName);
                        ChatRoomResolution resolution = ChatRoomResolution.Parse(reader.GetInt32(c_resolutionPropertyName));
                        DateTime updated = new(reader.GetInt64(c_updatedPropertyName));
                        DateTime seen = new(reader.GetInt64(c_seenPropertyName));

                        ChatRoom chatRoom = new(url);
                        chatRoom.OutputDirectory = _settings.OutputDirectory;
                        chatRoom.FFmpegPath = _settings.FFmpegPath;
                        chatRoom.StreamlinkPath = _settings.StreamlinkPath;
                        chatRoom.Action = action;
                        chatRoom.PreferredResolution = resolution;
                        chatRoom.LastUpdated = updated;
                        chatRoom.LastSeen = seen;
                        chatRoom.UpdateCompleted += ChatRoom_UpdateCompleted;
                        _chatRoomsList.Add(chatRoom);
                    }
                }
            }
            catch (Exception)
            {
                foreach (ChatRoom chatRoom in _chatRoomsList.UnfilteredItems)
                {
                    chatRoom.Dispose();
                }
                _chatRoomsList.RemoveAll();
            }
        }

        private void WriteData(string table, object item = null, int operation = 0)
        {
            try
            {
                SqliteCommand command = _connection.CreateCommand();

                switch (table)
                {
                    case c_settingsTableName:

                        command.CommandText = string.Format("delete from {0}; insert into {0} ({1},{3},{5},{7},{9},{11},{13},{15},{17}) values ('{2}','{4}','{6}','{8}','{10}','{12}','{14}','{16}','{18}');",
                            c_settingsTableName,
                            c_outputDirectorySettingName, _settings.OutputDirectory,
                            c_ffmpegPathSettingName, _settings.FFmpegPath,
                            c_streamlinkPathSettingName, _settings.StreamlinkPath,
                            c_chaturbateConcurrentUpdatesSettingName, _settings.ChaturbateConcurrentUpdates,
                            c_bongaCamsConcurrentUpdatesSettingName, _settings.BongaCamsConcurrentUpdates,
                            c_stripchatConcurrentUpdatesSettingName, _settings.StripchatConcurrentUpdates,
                            c_updateIntervalSettingName, _settings.UpdateInterval,
                            c_defaultActionSettingName, (int)_settings.DefaultAction,
                            c_defaultResolutionSettingName, _settings.DefaultResolution.ToInt32());
                        break;

                    case c_chatRoomsTableName:

                        ChatRoom chatRoom = (ChatRoom)item;
                        switch (operation)
                        {
                            case -1:
                                command.CommandText = string.Format("delete from {0} where {1}='{2}';",
                                    c_chatRoomsTableName,
                                    c_urlPropertyName,
                                    chatRoom.ChatRoomUrl);
                                break;

                            case 0:
                                command.CommandText = string.Format("update {0} set {1}='{2}', {3}='{4}', {5}='{6}', {7}='{8}' where {9}='{10}';",
                                    c_chatRoomsTableName,
                                    c_actionPropertyName, (int)chatRoom.Action,
                                    c_resolutionPropertyName, chatRoom.PreferredResolution.ToInt32(),
                                    c_updatedPropertyName, chatRoom.LastUpdated.Ticks,
                                    c_seenPropertyName, chatRoom.LastSeen.Ticks,
                                    c_urlPropertyName, chatRoom.ChatRoomUrl);
                                break;

                            case 1:
                                command.CommandText = string.Format("insert into {0} ({1},{3},{5},{7},{9}) values ('{2}','{4}','{6}','{8}','{10}');",
                                    c_chatRoomsTableName,
                                    c_actionPropertyName, (int)chatRoom.Action,
                                    c_resolutionPropertyName, chatRoom.PreferredResolution.ToInt32(),
                                    c_updatedPropertyName, chatRoom.LastUpdated.Ticks,
                                    c_seenPropertyName, chatRoom.LastSeen.Ticks,
                                    c_urlPropertyName, chatRoom.ChatRoomUrl);
                                break;
                        }
                        break;
                }

                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //do nothing
            }
        }

        private Settings _settings = new Settings();
        private ChatRoomsBindingList _chatRoomsList = new();
        private FilesBindingList _filesList = new();
        private SqliteConnection _connection = null;
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
        private const string c_ffmpegBinaryName = "ffmpeg.exe";
        private const string c_streamlinkBinaryName = "streamlink.exe";
        private const string c_thumbnailsDirectoryName = "Thumbnails";

        private const string c_settingsTableName = "Settings";
        private const string c_chatRoomsTableName = "ChatRooms";

        private const string c_outputDirectorySettingName = "OutputDirectory";
        private const string c_ffmpegPathSettingName = "FFmpegPath";
        private const string c_streamlinkPathSettingName = "StreamlinkPath";
        private const string c_chaturbateConcurrentUpdatesSettingName = "ChaturbateConcurrentUpdates";
        private const string c_bongaCamsConcurrentUpdatesSettingName = "BongaCamsConcurrentUpdates";
        private const string c_stripchatConcurrentUpdatesSettingName = "StripchatConcurrentUpdates";
        private const string c_updateIntervalSettingName = "UpdateInterval";
        private const string c_defaultActionSettingName = "DefaultAction";
        private const string c_defaultResolutionSettingName = "DefaultResolution";

        private const string c_urlPropertyName = "URL";
        private const string c_actionPropertyName = "Action";
        private const string c_resolutionPropertyName = "Resolution";
        private const string c_updatedPropertyName = "Updated";
        private const string c_seenPropertyName = "Seen";

        private const string c_unsupportedUrlMessage = "The URL is incorrect! Only http://* and https://* URLs are supported!";
        private const string c_unsupportedWebSiteMessage = "The URL is incorrect! Supported websites are Chaturbate, BongaCams and Stripchat!";
        private const string c_duplicateChatRoomMessage = "The chat room is already in the list!";
        private const string c_confirmChatRoomsRemovingMessage = "Do you really want to remove selected chat rooms?";
        private const string c_fileOpeningErrorMessage = "The file can't be opened!";
        private const string c_confirmFilesRemovingMessage = "Do you really want to remove selected files?";
        private const string c_fileRemovingErrorMessage = "The file can't be removed!";
        private const string c_confirmThumbnailRemovingMessage = "Do you really want to remove this thumbnail?";
        private const string c_thumbnailRemovingErrorMessage = "The thumbnail can't be removed!";
    }

    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void CopyTo(Settings settings)
        {
            settings._outputDirectory = _outputDirectory;
            settings._ffmpegPath = _ffmpegPath;
            settings._streamlinkPath = _streamlinkPath;
            settings._chaturbateConcurrentUpdates = _chaturbateConcurrentUpdates;
            settings._bongaCamsConcurrentUpdates = _bongaCamsConcurrentUpdates;
            settings._stripchatConcurrentUpdates = _stripchatConcurrentUpdates;
            settings._updateInterval = _updateInterval;
            settings._defaultAction = _defaultAction;
            settings._defaultResolution = _defaultResolution;
        }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string OutputDirectory
        {
            get
            {
                return _outputDirectory;
            }
            set
            {
                _outputDirectory = value;
                NotifyPropertyChanged(nameof(OutputDirectory));
            }
        }

        public string FFmpegPath
        {
            get
            {
                return _ffmpegPath;
            }
            set
            {
                _ffmpegPath = value;
                NotifyPropertyChanged(nameof(FFmpegPath));
            }
        }

        public string StreamlinkPath
        {
            get
            {
                return _streamlinkPath;
            }
            set
            {
                _streamlinkPath = value;
                NotifyPropertyChanged(nameof(StreamlinkPath));
            }
        }

        public int ChaturbateConcurrentUpdates
        {
            get
            {
                return _chaturbateConcurrentUpdates;
            }
            set
            {
                _chaturbateConcurrentUpdates = value >= 1 ? value : 1;
                NotifyPropertyChanged(nameof(ChaturbateConcurrentUpdates));
            }
        }

        public int BongaCamsConcurrentUpdates
        {
            get
            {
                return _bongaCamsConcurrentUpdates;
            }
            set
            {
                _bongaCamsConcurrentUpdates = value >= 1 ? value : 1;
                NotifyPropertyChanged(nameof(BongaCamsConcurrentUpdates));
            }
        }

        public int StripchatConcurrentUpdates
        {
            get
            {
                return _stripchatConcurrentUpdates;
            }
            set
            {
                _stripchatConcurrentUpdates = value >= 1 ? value : 1;
                NotifyPropertyChanged(nameof(StripchatConcurrentUpdates));
            }
        }

        public int UpdateInterval
        {
            get
            {
                return _updateInterval;
            }
            set
            {
                _updateInterval = value >= 1 ? value : 1;
                NotifyPropertyChanged(nameof(UpdateInterval));
            }
        }

        public ChatRoomAction DefaultAction
        {
            get
            {
                return _defaultAction;
            }
            set
            {
                _defaultAction = value;
                NotifyPropertyChanged(nameof(DefaultAction));
            }
        }

        public ChatRoomResolution DefaultResolution
        {
            get
            {
                return _defaultResolution;
            }
            set
            {
                _defaultResolution = value;
                NotifyPropertyChanged(nameof(DefaultResolution));
            }
        }

        private string _outputDirectory;
        private string _ffmpegPath;
        private string _streamlinkPath;
        private int _chaturbateConcurrentUpdates;
        private int _bongaCamsConcurrentUpdates;
        private int _stripchatConcurrentUpdates;
        private int _updateInterval;
        private ChatRoomAction _defaultAction;
        private ChatRoomResolution _defaultResolution;
    }

    public class SortableBindingList<T> : BindingList<T>
    {
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            IEnumerable<T> query = null;

            if (direction == ListSortDirection.Ascending)
            {
                query = Items.OrderBy(i => prop.GetValue(i));
            }
            else
            {
                query = Items.OrderByDescending(i => prop.GetValue(i));
            }

            int index = 0;
            foreach (object item in query)
            {
                Items[index++] = (T)item;
            }

            _isSorted = true;
            _propertyDescriptor = prop;
            _sortDirection = direction;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
        }

        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }

        protected override bool IsSortedCore
        {
            get
            {
                return _isSorted;
            }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return _propertyDescriptor;
            }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return _sortDirection;
            }
        }

        private bool _isSorted;
        private PropertyDescriptor _propertyDescriptor;
        private ListSortDirection _sortDirection;
    }

    public class ChatRoomsBindingList: SortableBindingList<ChatRoom>, IBindingListView
    {
        public ChatRoomsBindingList()
        {
            ListChanged += ChatRoomsBindingList_ListChanged;
        }

        private void ChatRoomsBindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    _unfilteredItems.Add(Items[e.NewIndex]);
                    _filteredItems.Add(Items[e.NewIndex]);
                    break;
                case ListChangedType.ItemDeleted:
                    _unfilteredItems.Remove(_filteredItems[e.NewIndex]);
                    _filteredItems.RemoveAt(e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    _filteredItems = Items.ToList();
                    break;
            }
        }

        public void RemoveAll()
        {
            RaiseListChangedEvents = false;

            Items.Clear();
            _unfilteredItems.Clear();

            RaiseListChangedEvents = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public void ApplySort(ListSortDescriptionCollection sorts)
        {
            //do nothing
        }

        public void RemoveFilter()
        {
            Filter = null;
        }

        public bool SupportsAdvancedSorting
        {
            get
            {
                return false;
            }
        }

        public ListSortDescriptionCollection SortDescriptions
        {
            get
            {
                return null;

            }
        }

        public bool SupportsFiltering
        {
            get
            {
                return true;
            }
        }

        public string Filter
        {
            get
            {
                return _filter;
            }

            set
            {
                if (value == string.Empty)
                {
                    value = null;
                }

                if (_filter != value)
                {
                    _filter = value;

                    RaiseListChangedEvents = false;

                    ClearItems();
                    Tuple<ChatRoomWebsite, string, string> parsedUrl = ChatRoom.ParseUrl(_filter);
                    foreach (ChatRoom chatRoom in _unfilteredItems)
                    {
                        if (_filter == null ||
                            parsedUrl == null && chatRoom.ChatRoomUrl.Contains(_filter, StringComparison.CurrentCultureIgnoreCase) ||
                            parsedUrl != null && chatRoom.Website == parsedUrl.Item1 && chatRoom.Name.Contains(parsedUrl.Item2, StringComparison.CurrentCultureIgnoreCase))
                        {
                            Items.Add(chatRoom);
                        }
                    }

                    RaiseListChangedEvents = true;

                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                }
            }
        }

        public List<ChatRoom> UnfilteredItems
        {
            get
            {
                return _unfilteredItems;
            }
        }

        private string _filter = null;
        private List<ChatRoom> _unfilteredItems = new();
        private List<ChatRoom> _filteredItems = new();
    }

    public class FilesBindingList : SortableBindingList<FileInfo>
    {
        //empty
    }
}
