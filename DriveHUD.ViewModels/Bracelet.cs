using DriveHUD.Common.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.ViewModels
{
    public class Bracelet : INotifyPropertyChanged
    {
        private int _place;
        private int _numberOfWins;
        private ObservableCollection<BraceletItem> _braceletItems = new ObservableCollection<BraceletItem>();

        public int PlaceNumber
        {
            get
            {
                return _place;
            }

            set
            {
                if (value == _place) return;
                _place = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfWins
        {
            get
            {
                return _numberOfWins;
            }

            set
            {
                if (value == _numberOfWins) return;
                _numberOfWins = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BraceletItem> BraceletItems
        {
            get
            {
                return _braceletItems;
            }

            set
            {
                if (_braceletItems == value) return;
                _braceletItems = value;
                OnPropertyChanged();
            }
        }

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
