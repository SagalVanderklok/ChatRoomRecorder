using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace ChatRoomRecorder
{
    public partial class MergeForm : Form
    {
        public MergeForm(List<FileInfo> files)
        {
            InitializeComponent();

            _filesList = new();
            _filesList.Append(files);

            FilesDataGridView.DataSource = _filesList;
        }

        private void MergeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            WindowsMediaPlayer.close();
        }

        private void FilesDataGridView_DoubleClick(object sender, System.EventArgs e)
        {
            if (FilesDataGridView.SelectedRows.Count == 1)
            {
                WindowsMediaPlayer.URL = _filesList[FilesDataGridView.SelectedRows[0].Index].FullName;
            }
        }

        private void FilesDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1 && FilesDataGridView.SelectedRows.Count == 1 && FilesDataGridView.SelectedRows[0].Index == e.RowIndex)
            {
                FilesDataGridView.DoDragDrop(FilesDataGridView.Rows[e.RowIndex], DragDropEffects.Move);
            }
        }

        private void FilesDataGridView_DragOver(object sender, DragEventArgs e)
        {
            Point newPoint = FilesDataGridView.PointToClient(new Point(e.X, e.Y));
            int newIndex = FilesDataGridView.HitTest(newPoint.X, newPoint.Y).RowIndex;
            int oldIndex = ((DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow))).Index;

            if (newIndex != -1 && newIndex != oldIndex)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void FilesDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            Point newPoint = FilesDataGridView.PointToClient(new Point(e.X, e.Y));
            int newIndex = FilesDataGridView.HitTest(newPoint.X, newPoint.Y).RowIndex;
            int oldIndex = ((DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow))).Index;

            if (newIndex > oldIndex)
            {
                _filesList.Insert(newIndex + 1, _filesList[oldIndex]);
                _filesList.RemoveAt(oldIndex);
            }
            else
            {
                _filesList.Insert(newIndex, _filesList[oldIndex]);
                _filesList.RemoveAt(oldIndex + 1);
            }
        }

        public List<FileInfo> Files
        {
            get
            {
                return _filesList.Cast<FileInfo>().ToList();
            }
        }

        private BindingListFiles _filesList;
    }
}
