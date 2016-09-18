using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Common.Utils
{
    /// <summary>
    /// Extended <see cref="ObservableCollection{T}"/> with methods that will fire single event for range method 
    /// instead of event per collection item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        public RangeObservableCollection()
            : base()
        {
        }

        public RangeObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public RangeObservableCollection(List<T> list)
            : base(list)
        {
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (collection.Count() == 0)
                return;

            foreach (var item in collection)
            {
                Items.Add(item);
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public virtual void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            bool removed = false;
            foreach (T item in collection)
            {
                if (this.Items.Remove(item))
                    removed = true;
            }

            if (removed)
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public virtual void Reset(T item)
        {
            this.Reset(new List<T>() { item });
        }

        public virtual void Reset(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentException("collection");

            int count = this.Count;

            this.Items.Clear();

            foreach (T item in collection)
            {
                this.Items.Add(item);
            }

            if (this.Count != count)
                this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
