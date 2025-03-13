using System.Collections.Generic;

namespace ChatRoomRecorder
{
    public class Category
    {
        public Category(int id, string name)
        {
            _parent = null;
            _id = id;
            _name = name;
            _filter = string.Empty;
            _allCategories = new List<Category>();
            _allChatRooms = new List<ChatRoom>();
            _filteredCategories = new List<Category>();
            _filteredChatRooms = new List<ChatRoom>();
        }

        public void Add(Category category)
        {
            _allCategories.Add(category);
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

            ApplyFilterRecursive(root);
        }

        private void ApplyFilterRecursive(Category parent)
        {
            foreach (Category category in parent.AllCategories)
            {

            }
        }

        public Category Parent
        {
            get
            {
                return _parent;
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

                ApplyFilter();
            }
            get
            {
                return _filter;
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

        private Category _parent;
        private int _id;
        private string _name;
        private string _filter;
        private List<Category> _allCategories;
        private List<ChatRoom> _allChatRooms;
        private List<Category> _filteredCategories;
        private List<ChatRoom> _filteredChatRooms;
    }
}
