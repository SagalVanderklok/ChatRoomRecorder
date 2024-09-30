using System.Collections.Generic;

namespace ChatRoomRecorder
{
    public class Category
    {
        public Category(int id, string name)
        {
            _id = id;
            _name = name;
            _categories = new List<Category>();
            _chatRooms = new List<ChatRoom>();
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
                if (value != null)
                {
                    _name = value;
                }
                else
                {
                    _name = string.Empty;
                }
            }
            get
            {
                return _name;
            }
        }

        public List<Category> Categories
        {
            get
            {
                return _categories;
            }
        }

        public List<ChatRoom> ChatRooms
        {
            get
            {
                return _chatRooms;
            }
        }

        private int _id;
        private string _name;
        private List<Category> _categories;
        private List<ChatRoom> _chatRooms;
    }
}
