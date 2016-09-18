using DriveHUD.Common.Annotations;
using HandHistories.Objects.GameDescription;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DriveHUD.ViewModels
{
    public class BraceletItem : INotifyPropertyChanged
    {
        private string _id;
        private string _amountString;

        public string Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public string AmountString
        {
            get
            {
                return _amountString;
            }

            set
            {
                if (value == _amountString) return;
                _amountString = value;
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