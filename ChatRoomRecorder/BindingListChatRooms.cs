using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ChatRoomRecorder
{
    public class BindingListChatRooms : BindingListSortable<ChatRoom>, IBindingListView
    {
        public BindingListChatRooms()
        {
            ListChanged += BindingListChatRooms_ListChanged;
        }

        private void BindingListChatRooms_ListChanged(object sender, ListChangedEventArgs e)
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

        public void Add(List<ChatRoom> chatRooms)
        {
            RaiseListChangedEvents = false;

            foreach (ChatRoom chatRoom in chatRooms)
            {
                _unfilteredItems.Add(chatRoom);
            }

            FilterItems();

            RaiseListChangedEvents = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
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

                    FilterItems();

                    RaiseListChangedEvents = true;

                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                }
            }
        }

        private void FilterItems()
        {
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
}
