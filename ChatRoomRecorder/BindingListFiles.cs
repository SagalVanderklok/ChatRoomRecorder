using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ChatRoomRecorder
{
    public class BindingListFiles : BindingListSortable<FileInfo>
    {
        public void Append(List<FileInfo> files)
        {
            RaiseListChangedEvents = false;

            foreach (FileInfo file in files)
            {
                Items.Add(file);
            }

            if (_isSorted)
            {
                ApplySortCore(_propertyDescriptor, _sortDirection);
            }

            RaiseListChangedEvents = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public void Delete(List<FileInfo> files)
        {
            RaiseListChangedEvents = false;

            foreach (FileInfo file in files)
            {
                Items.Remove(file);
            }

            RaiseListChangedEvents = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public int IndexOf(string path)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].FullName == path)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
