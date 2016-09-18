using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.EquityCalculator.Views;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Enums;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public class ChangeEquityCalculatorPopupViewEventArgs : EventArgs
    {
        public enum ViewType
        {
            CardsSelectorView,
            PreflopSelectorView
        }

        public ViewType PopupViewType { get; }

        public ChangeEquityCalculatorPopupViewEventArgs(ViewType viewType)
        {
            this.PopupViewType = viewType;
        }
    }

    public class ChangeEquityCalculatorPopupViewEvent : PubSubEvent<ChangeEquityCalculatorPopupViewEventArgs>
    {

    }

    public class SelectorMainContainerViewModel : BaseViewModel, IInteractionRequestAware
    {
        private INotification _notification;
        private Action _finishInteraction;
        private UserControl _selectedView;

        public ICommand OnViewLoadedCommand { get; set; }

        public Action FinishInteraction
        {
            get
            {
                return _finishInteraction;
            }

            set
            {
                _finishInteraction = value;
            }
        }

        public INotification Notification
        {
            get
            {
                return _notification;
            }

            set
            {
                _notification = value;
            }
        }

        public UserControl SelectedView
        {
            get
            {
                return _selectedView;
            }

            set
            {
                if (value != null)
                {
                    ((IInteractionRequestAware)(value.DataContext)).Notification = this.Notification;
                    ((IInteractionRequestAware)(value.DataContext)).FinishInteraction = this.FinishInteraction;
                }
                SetProperty(ref _selectedView, value);
            }
        }

        public SelectorMainContainerViewModel()
        {
            OnViewLoadedCommand = new RelayCommand(ViewLoaded);

            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<ChangeEquityCalculatorPopupViewEvent>().Subscribe(ChangeView);
        }

        private void ViewLoaded(object obj)
        {
            if (_notification != null && _notification is CardSelectorNotification)
            {
                var v = _notification as CardSelectorNotification;

                if ((v.SelectorType != CardSelectorType.BoardSelector)
                    && ((v.CardsContainer.Ranges != null && v.CardsContainer.Ranges.Any())
                    || v.CardsContainer.Cards.All(x => x.Rank == RangeCardRank.None && x.Suit == RangeCardSuit.None)))
                {
                    SelectedView = new PreflopSelectorView();
                }
                else
                {
                    SelectedView = new CardSelectorView();
                }
            }
        }

        private void ChangeView(ChangeEquityCalculatorPopupViewEventArgs obj)
        {
            if (obj != null)
            {
                if (obj.PopupViewType.Equals(ChangeEquityCalculatorPopupViewEventArgs.ViewType.CardsSelectorView))
                {
                    SelectedView = new CardSelectorView();
                }
                else if (obj.PopupViewType.Equals(ChangeEquityCalculatorPopupViewEventArgs.ViewType.PreflopSelectorView))
                {
                    SelectedView = new PreflopSelectorView();
                }
            }
        }
    }
}
