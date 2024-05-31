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

            SettingsBindingSource.DataSource = new Settings()
            {
                OutputDirectory = c_defaultSettings.OutputDirectory,
                FFmpegPath = c_defaultSettings.FFmpegPath,
                ChaturbateConcurrentUpdates = c_defaultSettings.ChaturbateConcurrentUpdates,
                BongaCamsConcurrentUpdates = c_defaultSettings.BongaCamsConcurrentUpdates,
                UpdateInterval = c_defaultSettings.UpdateInterval
            };
            OutputDirectoryTextBox.DataBindings.Add("Text", SettingsBindingSource, c_outputDirectorySettingName);
            FFmpegPathTextBox.DataBindings.Add("Text", SettingsBindingSource, c_ffmpegPathSettingName);
            ChaturbateConcurrentUpdatesNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_chaturbateConcurrentUpdatesSettingName);
            BongaCamsConcurrentUpdatesNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_bongaCamsConcurrentUpdatesSettingName);
            UpdateIntervalNumericUpDown.DataBindings.Add("Value", SettingsBindingSource, c_updateIntervalSettingName);

            ChatRoomsBindingSource.DataSource = new SortableBindingList<ChatRoom>();
            ActionColumn.DataSource = new ChatRoomAction[] { ChatRoomAction.None, ChatRoomAction.Monitor, ChatRoomAction.Record };
            ResolutionColumn.DataSource = ChatRoomResolution.CommonResolutions;

            FilesBindingSource.DataSource = new SortableBindingList<FileInfo>();

            ReadData();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (_lock)
            {
                if (!_isExiting)
                {
                    TabControl.Enabled = false;

                    _isExiting = true;

                    for (int i = 0; i < ChatRoomsBindingSource.Count; i++)
                    {
                        ((ChatRoom)ChatRoomsBindingSource[i]).Dispose();
                    }

                    if (_connection != null)
                    {
                        _connection.Close();
                    }

                    ChatRoomsUpdateTimer.Stop();
                    FormCloseTimer.Start();

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
            if (e.IsSuccess)
            {
                ChatRoomsUpdateTimer.Start();

                TabControl.Enabled = true;
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
                AddChatRoom();
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

        private void UrlTextBox_TextChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                ChatRoomsDataGridView.Enabled = false;

                FilterChatRoomsDataGridView();

                ChatRoomsDataGridView.Enabled = true;
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
                        { ChatRoomWebsite.Chaturbate, ((Settings)SettingsBindingSource.DataSource).ChaturbateConcurrentUpdates },
                        { ChatRoomWebsite.BongaCams, ((Settings)SettingsBindingSource.DataSource).BongaCamsConcurrentUpdates }
                    };

                    Dictionary<ChatRoomWebsite, int> curUpdateCounts = new()
                    {
                        { ChatRoomWebsite.Chaturbate, 0 },
                        { ChatRoomWebsite.BongaCams, 0 }
                    };

                    for (int i = 0; i < ChatRoomsBindingSource.Count; i++)
                    {
                        ChatRoom chatRoom = (ChatRoom)ChatRoomsBindingSource[i];
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
                if (ChatRoomsDataGridView.Rows[ChatRoomsBindingSource.IndexOf(sender)].Visible)
                {
                    ChatRoomsDataGridView.InvalidateRow(ChatRoomsBindingSource.IndexOf(sender));
                }

                WriteData(c_chatRoomsTableName, (ChatRoom)sender, 0);
            }
        }

        private void ChatRoomsDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            UrlTextBox.Clear();
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
                    WriteData(c_chatRoomsTableName, ChatRoomsBindingSource[e.RowIndex], 0);
                }
            }
        }

        private void ChatRoomsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                FilesBindingSource.Clear();

                foreach (DataGridViewRow selectedRow in ChatRoomsDataGridView.SelectedRows)
                {
                    try
                    {
                        ChatRoom chatRoom = (ChatRoom)ChatRoomsBindingSource[selectedRow.Index];
                        DirectoryInfo directory = new(((Settings)SettingsBindingSource.DataSource).OutputDirectory);
                        FileInfo[] files = directory.GetFiles(string.Format("{0} {1} *", chatRoom.Website, chatRoom.Name));
                        foreach (FileInfo file in files)
                        {
                            FilesBindingSource.Add(file);
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
                    if (MessageBox.Show(c_confirmChatRoomsRemovalMessage, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        ChatRoomsDataGridView.Enabled = false;

                        List<ChatRoom> chatRooms = new List<ChatRoom>();
                        foreach (DataGridViewRow row in ChatRoomsDataGridView.SelectedRows)
                        {
                            chatRooms.Add((ChatRoom)ChatRoomsBindingSource[row.Index]);
                        }
                        foreach (ChatRoom chatRoom in chatRooms)
                        {
                            ChatRoomsBindingSource.Remove(chatRoom);
                            WriteData(c_chatRoomsTableName, chatRoom, -1);
                            chatRoom.Dispose();
                        }

                        FilterChatRoomsDataGridView();

                        ChatRoomsDataGridView.Enabled = true;
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
                            List<FileInfo> files = new List<FileInfo>();

                            foreach (DataGridViewRow row in FilesDataGridView.SelectedRows)
                            {
                                files.Add((FileInfo)FilesBindingSource[row.Index]);
                            }

                            foreach (FileInfo file in files)
                            {
                                FilesBindingSource.Remove(file);
                                file.Delete();
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
                            FileName = ((FileInfo)FilesBindingSource[e.RowIndex]).FullName
                        });
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(c_fileOpenErrorMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OutputDirectoryTextBox_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                ((Settings)SettingsBindingSource.DataSource).OutputDirectory = fbd.SelectedPath;
            }
        }

        private void FFmpegPathTextBox_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new() { Multiselect = false, Filter = string.Format("{0}|{0}", c_ffmpegBinaryName) };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ((Settings)SettingsBindingSource.DataSource).FFmpegPath = ofd.FileName;
            }
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            lock (_lock)
            {
                Settings settings = (Settings)SettingsBindingSource.DataSource;

                switch (e.PropertyName)
                {
                    case c_outputDirectorySettingName:
                        for (int i = 0; i < ChatRoomsBindingSource.Count; i++)
                        {
                            ((ChatRoom)ChatRoomsBindingSource[i]).OutputDirectory = settings.OutputDirectory;
                        }
                        break;

                    case c_ffmpegPathSettingName:
                        for (int i = 0; i < ChatRoomsBindingSource.Count; i++)
                        {
                            ((ChatRoom)ChatRoomsBindingSource[i]).FFmpegPath = settings.FFmpegPath;
                        }
                        break;

                    case c_chaturbateConcurrentUpdatesSettingName:
                        break;

                    case c_bongaCamsConcurrentUpdatesSettingName:
                        break;

                    case c_updateIntervalSettingName:
                        ChatRoomsUpdateTimer.Interval = settings.UpdateInterval * 1000;
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

        private void AddChatRoom()
        {
            Tuple<ChatRoomWebsite, string, string> parsedUrl = ChatRoom.ParseUrl(UrlTextBox.Text);

            if (parsedUrl != null)
            {
                bool duplicate = false;

                for (int i = 0; i < ChatRoomsBindingSource.Count && !duplicate; i++)
                {
                    ChatRoom chatRoom = (ChatRoom)ChatRoomsBindingSource[i];
                    if (chatRoom.Website == parsedUrl.Item1 && chatRoom.Name == parsedUrl.Item2)
                    {
                        duplicate = true;
                    }
                }

                if (!duplicate)
                {
                    ChatRoomsDataGridView.Enabled = false;

                    ChatRoom chatRoom = new(parsedUrl.Item3);
                    chatRoom.PreferredResolution = ChatRoomResolution.CommonResolutions[ChatRoomResolution.FindClosest(c_defaultResolution, ChatRoomResolution.CommonResolutions)];
                    chatRoom.OutputDirectory = ((Settings)SettingsBindingSource.DataSource).OutputDirectory;
                    chatRoom.FFmpegPath = ((Settings)SettingsBindingSource.DataSource).FFmpegPath;
                    chatRoom.UpdateCompleted += ChatRoom_UpdateCompleted;
                    ChatRoomsBindingSource.Add(chatRoom);
                    WriteData(c_chatRoomsTableName, chatRoom, 1);
                    
                    FilterChatRoomsDataGridView();

                    ChatRoomsDataGridView.Enabled = true;
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

        private void FilterChatRoomsDataGridView()
        {
            DataGridViewColumn currentColumn = null;
            DataGridViewRow currentRow = null;

            if (ChatRoomsDataGridView.CurrentCell != null)
            {
                currentColumn = ChatRoomsDataGridView.CurrentCell.OwningColumn;
                ChatRoomsDataGridView.CurrentCell = null;
            }

            Tuple<ChatRoomWebsite, string, string> parsedUrl = ChatRoom.ParseUrl(UrlTextBox.Text);
            for (int i = ChatRoomsBindingSource.Count - 1; i >= 0; i--)
            {
                ChatRoom chatRoom = (ChatRoom)ChatRoomsBindingSource[i];
                DataGridViewRow row = ChatRoomsDataGridView.Rows[i];
                row.Visible =
                    parsedUrl == null && chatRoom.ChatRoomUrl.Contains(UrlTextBox.Text, StringComparison.CurrentCultureIgnoreCase) ||
                    parsedUrl != null && chatRoom.Website == parsedUrl.Item1 && chatRoom.Name.Contains(parsedUrl.Item2, StringComparison.CurrentCultureIgnoreCase);
                if (row.Visible)
                {
                    currentRow = row;
                }
            }

            if (currentRow != null)
            {
                if (currentColumn != null)
                {
                    ChatRoomsDataGridView.CurrentCell = currentRow.Cells[ChatRoomsDataGridView.Columns.IndexOf(currentColumn)];
                }
                else
                {
                    ChatRoomsDataGridView.CurrentCell = currentRow.Cells[0];
                }
            }
        }

        private void ReadData()
        {
            Settings settings = (Settings)SettingsBindingSource.DataSource;

            try
            {
                _connection = new SqliteConnection(string.Format("data source={0}", c_dbFileName));
                _connection.Open();

                SqliteCommand command = _connection.CreateCommand();
                command.CommandText = string.Format("create table if not exists {0} ({1} text,{2} text,{3} int,{4} int,{5} int);",
                    c_settingsTableName,
                    c_outputDirectorySettingName,
                    c_ffmpegPathSettingName,
                    c_chaturbateConcurrentUpdatesSettingName,
                    c_bongaCamsConcurrentUpdatesSettingName,
                    c_updateIntervalSettingName);
                command.CommandText += string.Format("create table if not exists {0} ({1} text,{2} int,{3} int,{4} int,{5} int);",
                    c_chatRoomsTableName,
                    c_urlPropertyName,
                    c_actionPropertyName,
                    c_resolutionPropertyName,
                    c_updatedPropertyName,
                    c_seenPropertyName);
                command.ExecuteNonQuery();
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
                        settings.OutputDirectory = reader.GetString(0);
                        settings.FFmpegPath = reader.GetString(1);
                        settings.ChaturbateConcurrentUpdates = reader.GetInt32(2);
                        settings.BongaCamsConcurrentUpdates = reader.GetInt32(3);
                        settings.UpdateInterval = reader.GetInt32(4);
                    }
                }
            }
            catch (Exception)
            {
                settings.OutputDirectory = c_defaultSettings.OutputDirectory;
                settings.FFmpegPath = c_defaultSettings.FFmpegPath;
                settings.ChaturbateConcurrentUpdates = c_defaultSettings.ChaturbateConcurrentUpdates;
                settings.BongaCamsConcurrentUpdates = c_defaultSettings.BongaCamsConcurrentUpdates;
                settings.UpdateInterval = c_defaultSettings.UpdateInterval;
            }
            finally
            {
                settings.PropertyChanged += Settings_PropertyChanged;
            }

            try
            {
                SqliteCommand command = _connection.CreateCommand();
                command.CommandText = string.Format("select * from {0}", c_chatRoomsTableName);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string url = reader.GetString(0);
                        ChatRoomAction action = (ChatRoomAction)reader.GetInt32(1);
                        ChatRoomResolution resolution = ChatRoomResolution.Parse(reader.GetInt32(2));
                        DateTime updated = new(reader.GetInt64(3));
                        DateTime seen = new(reader.GetInt64(4));

                        ChatRoom chatRoom = new(url);
                        chatRoom.Action = action;
                        chatRoom.PreferredResolution = resolution;
                        chatRoom.LastUpdated = updated;
                        chatRoom.LastSeen = seen;
                        chatRoom.OutputDirectory = settings.OutputDirectory;
                        chatRoom.FFmpegPath = settings.FFmpegPath;
                        chatRoom.UpdateCompleted += ChatRoom_UpdateCompleted;
                        ChatRoomsBindingSource.Add(chatRoom);
                    }
                }
            }
            catch (Exception)
            {
                for (int i = 0; i < ChatRoomsBindingSource.Count; i++)
                {
                    ((ChatRoom)ChatRoomsBindingSource[i]).Dispose();
                }
                ChatRoomsBindingSource.Clear();
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
                        Settings settings = (Settings)SettingsBindingSource.DataSource;
                        command.CommandText = string.Format("delete from {0}; insert into {0} values ('{1}','{2}','{3}','{4}','{5}');",
                            c_settingsTableName,
                            settings.OutputDirectory,
                            settings.FFmpegPath,
                            settings.ChaturbateConcurrentUpdates,
                            settings.BongaCamsConcurrentUpdates,
                            settings.UpdateInterval);
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
                                command.CommandText = string.Format("insert into {0} values ('{1}','{2}','{3}','{4}','{5}');",
                                    c_chatRoomsTableName,
                                    chatRoom.ChatRoomUrl,
                                    (int)chatRoom.Action,
                                    chatRoom.PreferredResolution.ToInt32(),
                                    chatRoom.LastUpdated.Ticks,
                                    chatRoom.LastSeen.Ticks);
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

        private SqliteConnection _connection = null;
        private object _lock = new();
        private Dictionary<ChatRoomWebsite, DateTime> _lastUpdates = new()
        {
            { ChatRoomWebsite.Chaturbate, DateTime.Now },
            { ChatRoomWebsite.BongaCams, DateTime.Now }
        };
        private bool _isExiting = false;

        private readonly Settings c_defaultSettings = new Settings()
        {
            OutputDirectory = new FileInfo(Environment.ProcessPath).Directory.FullName,
            FFmpegPath = new FileInfo(Environment.ProcessPath).Directory.FullName + Path.DirectorySeparatorChar + c_ffmpegBinaryName,
            ChaturbateConcurrentUpdates = 1,
            BongaCamsConcurrentUpdates = 1,
            UpdateInterval = 5
        };
        private readonly ChatRoomResolution c_defaultResolution = ChatRoomResolution.Parse("1920x1080");

        private const string c_dbFileName = "ChatRoomRecorder.db";
        private const string c_ffmpegBinaryName = "ffmpeg.exe";

        private const string c_settingsTableName = "Settings";
        private const string c_chatRoomsTableName = "ChatRooms";

        private const string c_outputDirectorySettingName = "OutputDirectory";
        private const string c_ffmpegPathSettingName = "FFmpegPath";
        private const string c_chaturbateConcurrentUpdatesSettingName = "ChaturbateConcurrentUpdates";
        private const string c_bongaCamsConcurrentUpdatesSettingName = "BongaCamsConcurrentUpdates";
        private const string c_updateIntervalSettingName = "UpdateInterval";

        private const string c_urlPropertyName = "URL";
        private const string c_actionPropertyName = "Action";
        private const string c_resolutionPropertyName = "Resolution";
        private const string c_updatedPropertyName = "Updated";
        private const string c_seenPropertyName = "Seen";

        private const string c_unsupportedUrlMessage = "The URL is incorrect! Only http://* and https://* URLs are supported!";
        private const string c_unsupportedWebSiteMessage = "The URL is incorrect! Supported websites are Chaturbate and BongaCams!";
        private const string c_duplicateChatRoomMessage = "The chat room is already in the list!";
        private const string c_fileOpenErrorMessage = "The file can't be opened!";
        private const string c_confirmFilesRemovalMessage = "Do you really want to remove selected files?";
        private const string c_fileRemoveErrorMessage = "The file can't be removed!";
        private const string c_confirmChatRoomsRemovalMessage = "Do you really want to remove selected chat rooms?";
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

    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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

        private string _outputDirectory;
        private string _ffmpegPath;
        private int _chaturbateConcurrentUpdates;
        private int _bongaCamsConcurrentUpdates;
        private int _updateInterval;
    }
}
