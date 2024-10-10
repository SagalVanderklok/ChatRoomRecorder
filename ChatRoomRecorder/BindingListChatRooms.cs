using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ChatRoomRecorder
{
    public class BindingListChatRooms : BindingListSortable<ChatRoom>, IBindingListView
    {
        public void Append(List<ChatRoom> chatRooms)
        {
            _unfilteredItems.AddRange(chatRooms);

            FilterItems();
        }

        public void Flush()
        {
            _unfilteredItems.Clear();

            FilterItems();
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

                    FilterItems();
                }
            }
        }

        private void FilterItems()
        {
            RaiseListChangedEvents = false;

            Items.Clear();
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

            if (_isSorted)
            {
                ApplySortCore(_propertyDescriptor, _sortDirection);
            }

            RaiseListChangedEvents = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private string _filter = null;
        private List<ChatRoom> _unfilteredItems = new();
    }
}
