using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DriveHUD.Common.Utils
{
    /// <summary>
    /// Extended <see cref="ObservableCollection{T}"/> with methods that will fire single event for range method 
    /// instead of event per collection item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private const string IndexerName = "Item[]";

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
     
        public virtual void Reset(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentException(nameof(collection));
            }

            int count = Count;

            Items.Clear();

            foreach (T item in collection)
            {
                Items.Add(item);
            }

            if (Count != count)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            }

            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}