using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ChatRoomRecorder
{
    public class BindingListSortable<T> : BindingList<T>
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
}
