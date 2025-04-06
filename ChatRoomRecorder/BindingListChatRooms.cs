using System.Collections.Generic;
using System.ComponentModel;

namespace ChatRoomRecorder
{
    public class BindingListChatRooms : BindingListSortable<ChatRoom>
    {
        public void Append(List<ChatRoom> chatRooms)
        {
            RaiseListChangedEvents = false;

            foreach (ChatRoom chatRoom in chatRooms)
            {
                Items.Add(chatRoom);
            }

            if (_isSorted)
            {
                ApplySortCore(_propertyDescriptor, _sortDirection);
            }

            RaiseListChangedEvents = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public void Delete(List<ChatRoom> chatRooms)
        {
            RaiseListChangedEvents = false;

            foreach (ChatRoom chatRoom in chatRooms)
            {
                Items.Remove(chatRoom);
            }

            RaiseListChangedEvents = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }
}
