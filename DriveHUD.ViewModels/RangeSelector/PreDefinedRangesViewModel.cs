using DriveHUD.Common.Infrastructure.Base;
using Model;
using Model.Notifications;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.ViewModels
{
    public class PreDefinedRangesViewModel : BaseViewModel, IInteractionRequestAware
    {
        #region Fields
        private PreDefinedRangesNotifcation _notification;
        private PreDefinedRangeModel _openRanges = PreDefinedRangeModel.GetDefaultHandsToOpenRaiseWith();
        private PreDefinedRangeModel _callingRangeOpen = PreDefinedRangeModel.GetDefaultHandsToCallWithWhenBlindsOpenRaised();
        private PreDefinedRangeModel _callingRangeClose = PreDefinedRangeModel.GetDefaultHandsToCallWithWhenNonBlindsOpenRaised();
        private PreDefinedRangeModel _bet3Ranges = PreDefinedRangeModel.GetDefaultHandsTo3BetWith();
        private PreDefinedRangeModel _bet4Ranges = PreDefinedRangeModel.GetDefaultHandsTo4BetWith();
        private PreDefinedRangeModel _limpedPotRanges = PreDefinedRangeModel.GetDefaultHandsCallUnraisedPotWith();
        #endregion

        #region Properties
        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get
            {
                return _notification;
            }

            set
            {
                if (value is PreDefinedRangesNotifcation)
                {
                    this._notification = value as PreDefinedRangesNotifcation;
                }
            }
        }

        public PreDefinedRangeModel OpenRanges
        {
            get
            {
                return _openRanges;
            }

            set
            {
                SetProperty(ref _openRanges, value);
            }
        }

        public PreDefinedRangeModel CallingRangeOpen
        {
            get
            {
                return _callingRangeOpen;
            }

            set
            {
                _callingRangeOpen = value;
            }
        }

        public PreDefinedRangeModel CallingRangeClose
        {
            get
            {
                return _callingRangeClose;
            }

            set
            {
                _callingRangeClose = value;
            }
        }

        public PreDefinedRangeModel Bet4Ranges
        {
            get
            {
                return _bet4Ranges;
            }

            set
            {
                _bet4Ranges = value;
            }
        }

        public PreDefinedRangeModel Bet3Ranges
        {
            get
            {
                return _bet3Ranges;
            }

            set
            {
                _bet3Ranges = value;
            }
        }

        public PreDefinedRangeModel LimpedPotRanges
        {
            get
            {
                return _limpedPotRanges;
            }

            set
            {
                _limpedPotRanges = value;
            }
        }
        #endregion

        #region ICommand
        public ICommand SelectRangeCommand { get; set; }
        #endregion

        public PreDefinedRangesViewModel()
        {
            Init();
        }

        private void Init()
        {
            SelectRangeCommand = new RelayCommand(SelectRange);
        }

        public void SelectRange(object obj)
        {
            if (obj != null && obj.GetType().IsGenericType)
            {
                var type = obj.GetType();
                var value = type.GetProperty("Value");
                var valueObj = value.GetValue(obj, null);
                if (valueObj != null && valueObj.GetType().IsGenericType)
                {
                    var type2 = valueObj.GetType();
                    var key = type2.GetProperty("Key");
                    var keyObj = key.GetValue(valueObj, null) as List<String>;

                    this._notification.ItemsList = new List<String>(keyObj);
                    this.FinishInteraction();
                }
            }
        }

    }
}
