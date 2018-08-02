//-----------------------------------------------------------------------
// <copyright file="PopupContainerBaseNotificationViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Common.Infrastructure.Base;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.PopupContainers
{
    public class PopupContainerBaseNotificationViewModel : BindableBase, IInteractionRequestAware
    {
        public PopupContainerBaseNotificationViewModel()
        {
            ConfirmCommand = new RelayCommand(DoConfirm);
            CancelCommand = new RelayCommand(DoCancel);
        }

        #region Properties

        private bool isDisplayH1Text;

        public bool IsDisplayH1Text
        {
            get
            {
                return isDisplayH1Text;
            }
            set
            {
                SetProperty(ref isDisplayH1Text, value);
            }
        }

        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                SetProperty(ref title, value);
            }
        }

        private string content;

        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                SetProperty(ref content, value);
            }
        }

        private string confirmButtonCaption;

        public string ConfirmButtonCaption
        {
            get
            {
                return confirmButtonCaption;
            }
            set
            {
                SetProperty(ref confirmButtonCaption, value);
            }
        }

        private string cancelButtonCaption;

        public string CancelButtonCaption
        {
            get
            {
                return cancelButtonCaption;
            }
            set
            {
                SetProperty(ref cancelButtonCaption, value);
            }
        }

        #endregion

        #region ICommand

        public ICommand ConfirmCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        private void DoConfirm(object obj)
        {
            notification.Confirmed = true;
            FinishInteraction.Invoke();
        }

        private void DoCancel(object obj)
        {
            notification.Confirmed = false;
            FinishInteraction.Invoke();
        }

        #endregion

        #region IInteractionRequestAware

        public Action FinishInteraction { get; set; }

        private PopupBaseNotification notification;

        public INotification Notification
        {
            get
            {
                return notification;
            }

            set
            {
                notification = value as PopupBaseNotification;

                if (notification != null)
                {
                    Content = notification.Content.ToString();
                    ConfirmButtonCaption = notification.ConfirmButtonCaption;
                    CancelButtonCaption = notification.CancelButtonCaption;
                    IsDisplayH1Text = notification.IsDisplayH1Text;

                    notification.Confirmed = false;
                }
            }
        }

        #endregion
    }
}