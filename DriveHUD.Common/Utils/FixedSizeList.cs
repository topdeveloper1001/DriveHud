using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Common.Utils
{
    /// <summary>
    /// List extension that will remove the very first element in the collection after its size exceeds specified capacity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixedSizeList<T> : List<T>
    {
        private int _capacity = -1;

        public FixedSizeList(int capacity = 10)
        {
            this.Capacity = capacity;
        }

        public new void Add(T elem)
        {
            base.Add(elem);

            while (this.Count > Capacity)
            {
                this.RemoveAt(0);
            }
        }

        // <summary>
        /// Move item at oldIndex to newIndex.
        /// </summary>
        public virtual void Move(int oldIndex, int newIndex)
        {
            T removedItem = this[oldIndex];

            base.RemoveAt(oldIndex);
            base.Insert(newIndex, removedItem);
        }

        public new int Capacity
        {
            get
            {
                return _capacity;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("capacity", "Capacity must be higher than 0");
                }
                _capacity = value;
            }
        }

    }
}
