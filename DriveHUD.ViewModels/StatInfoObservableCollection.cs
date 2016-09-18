using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DriveHUD.ViewModels
{
    public class StatInfoObservableCollection<T> : ObservableCollection<StatInfo>, INotifyPropertyChanged
    {
        public StatInfoObservableCollection() : base()
        {
        }

        private int _seriesCount = 0;

        private int _breakCount = 0;

        private bool _allowBreak = false;

        public int SeriesCount
        {
            get { return _seriesCount; }
            set { _seriesCount = value; }
        }

        public int BreakCount
        {
            get { return _breakCount; }
            set { _breakCount = value; }
        }

        public bool AllowBreak
        {
            get { return _allowBreak; }
            set { _allowBreak = value; }
        }

        protected override void InsertItem(int index, StatInfo item)
        {
            base.InsertItem(index, item);

            item.PropertyChanged += Item_PropertyChanged;
            
            // Show OptionButtons for StatInfo,
            // Hide OptionButtons for StatInfoBreak
            foreach (var v in Items.Where(x => x.GetType() == typeof(StatInfo)))
            {
                v.Settings_IsAvailable = true;
            }

            CountUpdate();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            CountUpdate();
        }

        private void CountUpdate()
        {
            SeriesCount = Items == null ? 0 : Items.Where(x => x.GetType() == typeof(StatInfo)).Count();
            BreakCount = Items == null ? 0 : Items.Where(x => x.GetType() == typeof(StatInfoBreak)).Count();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }
}
