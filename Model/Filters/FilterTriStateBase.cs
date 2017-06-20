using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Model.Filters
{
    [Serializable]
    public class FilterTriStateBase : FilterBaseEntity
    {
        public FilterTriStateBase() : this(EnumTriState.Any) { }

        public FilterTriStateBase(EnumTriState param = EnumTriState.Any)
        {
            this.TriStateCollection = new ObservableCollection<TriStateItem>
            (
                new List<TriStateItem>()
                {
                    new TriStateItem() { TriState = EnumTriState.Any, Color = Colors.Black, },
                    new TriStateItem() { TriState = EnumTriState.On, Color = Colors.Green, },
                    new TriStateItem() { TriState = EnumTriState.Off, Color = Colors.Red, },
                }
            );
            this.TriStateSelectedItem = this.TriStateCollection.FirstOrDefault(x => x.TriState == param);
        }

        private ObservableCollection<TriStateItem> _triStateCollection;
        private TriStateItem _triStateSelectedItem;

        public ObservableCollection<TriStateItem> TriStateCollection
        {
            get { return _triStateCollection; }
            set
            {
                if (value == _triStateCollection) return;
                _triStateCollection = value;
                OnPropertyChanged();
            }
        }

        public virtual TriStateItem TriStateSelectedItem
        {
            get { return _triStateSelectedItem; }
            set
            {
                if (value == _triStateSelectedItem) return;
                _triStateSelectedItem = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Swap to the next defined TriState
        /// </summary>
        public void TriStateSwap()
        {
            int index = this.TriStateCollection.IndexOf(this.TriStateCollection.FirstOrDefault(x => x.TriState == this.TriStateSelectedItem.TriState));
            this.TriStateSelectedItem = index == this.TriStateCollection.Count() - 1 ? this.TriStateCollection.ElementAt(0) : this.TriStateCollection.ElementAt(index + 1);
        }

        /// <summary>
        /// Set to a certain defined TriState
        /// </summary>
        public void TriStateSet(EnumTriState _param)
        {
            if (this.TriStateCollection.Where(x => x.TriState == _param).Any())
            {
                this.TriStateSelectedItem = this.TriStateCollection.FirstOrDefault(x => x.TriState == _param);
            }
        }
    }
}
