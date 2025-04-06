using System;
using System.Collections.Generic;

namespace ChatRoomRecorder
{
    public class Category
    {
        public Category(int id, string name)
        {
            _id = id;
            _name = name;
            _filter = string.Empty;
            _parent = null;
            _allCategories = new List<Category>();
            _allChatRooms = new List<ChatRoom>();
            _filteredCategories = new List<Category>();
            _filteredChatRooms = new List<ChatRoom>();
        }

        public void Add(Category category)
        {
            _allCategories.Add(category);
            category._filter = _filter;
            category._parent = this;

            ApplyFilter();
        }

        public void Add(ChatRoom chatRoom)
        {
            _allChatRooms.Add(chatRoom);

            ApplyFilter();
        }

        public void Remove(Category category)
        {
            _allCategories.Remove(category);
            category._parent = null;

            ApplyFilter();
        }

        public void Remove(ChatRoom chatRoom)
        {
            _allChatRooms.Remove(chatRoom);

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Category root = this;

            while (root._parent != null)
            {
                root = root.Parent;
            }

            ApplyFilterRecursive(root, ChatRoom.ParseUrl(_filter));
        }

        private void ApplyFilterRecursive(Category c, Tuple<ChatRoomWebsite, string, string> t)
        {
            c._filteredCategories.Clear();

            foreach (Category category in c.AllCategories)
            {
                ApplyFilterRecursive(category, t);

                if (_filter == string.Empty || category._filteredCategories.Count > 0 || category._filteredChatRooms.Count > 0)
                {
                    c._filteredCategories.Add(category);
                }
            }

            c._filteredChatRooms.Clear();

            foreach (ChatRoom chatRoom in c.AllChatRooms)
            {
                if (_filter == string.Empty ||
                    t == null && chatRoom.ChatRoomUrl.Contains(_filter, StringComparison.CurrentCultureIgnoreCase) ||
                    t != null && chatRoom.Website == t.Item1 && chatRoom.Name.Contains(t.Item2, StringComparison.CurrentCultureIgnoreCase))
                {
                    c._filteredChatRooms.Add(chatRoom);
                }
            }
        }

        public int ID
        {
            set
            {
                _id = value;
            }
            get
            {
                return _id;
            }
        }

        public string Name
        {
            set
            {
                _name = value != null ? value : string.Empty;
            }
            get
            {
                return _name;
            }
        }

        public string Filter
        {
            set
            {
                _filter = value != null ? value : string.Empty;

                foreach (Category category in _allCategories)
                {
                    category.Filter = _filter;
                }

                ApplyFilter();
            }
            get
            {
                return _filter;
            }
        }

        public Category Parent
        {
            get
            {
                return _parent;
            }
        }

        public Category[] AllCategories
        {
            get
            {
                return _allCategories.ToArray();
            }
        }

        public ChatRoom[] AllChatRooms
        {
            get
            {
                return _allChatRooms.ToArray();
            }
        }

        public Category[] FilteredCategories
        {
            get
            {
                return _filteredCategories.ToArray();
            }
        }

        public ChatRoom[] FilteredChatRooms
        {
            get
            {
                return _filteredChatRooms.ToArray();
            }
        }

        private int _id;
        private string _name;
        private string _filter;
        private Category _parent;
        private List<Category> _allCategories;
        private List<ChatRoom> _allChatRooms;
        private List<Category> _filteredCategories;
        private List<ChatRoom> _filteredChatRooms;
    }
}
