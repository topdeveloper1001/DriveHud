//-----------------------------------------------------------------------
// <copyright file="ChangeEquityCalculatorPopupViewEventArgs.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.EquityCalculator.Views;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
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
            PopupViewType = viewType;
        }
    }

    public class ChangeEquityCalculatorPopupViewEvent : PubSubEvent<ChangeEquityCalculatorPopupViewEventArgs>
    {
    }

    public class SelectorMainContainerViewModel : BaseViewModel, IInteractionRequestAware
    {
        private UserControl _selectedView;

        public ICommand OnViewLoadedCommand { get; set; }

        public Action FinishInteraction { get; set; }

        public INotification Notification { get; set; }

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
            if (Notification != null && Notification is CardSelectorNotification)
            {
                var v = Notification as CardSelectorNotification;

                if (v.SelectorType != CardSelectorType.BoardSelector)
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