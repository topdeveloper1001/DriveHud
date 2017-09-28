//-----------------------------------------------------------------------
// <copyright file="AppContainerViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using Model.AppStore;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public class AppContainerViewModel : ViewModelBase, INotification, IInteractionRequestAware
    {
        private readonly AppStoreProduct appStoreProduct;

        public AppContainerViewModel(AppStoreProduct appStoreProduct)
        {
            this.appStoreProduct = appStoreProduct;

            title = appStoreProduct.ProductName;
        }

        private object containerViewModel;

        public object ContainerViewModel
        {
            get
            {
                return containerViewModel;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref containerViewModel, value);
            }
        }

        #region Implementation of INotification

        private object content;

        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref content, value);
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
                this.RaiseAndSetIfChanged(ref title, value);
            }
        }

        #endregion

        #region Implementation of IInteractionRequestAware

        private Action finishInteraction;

        public Action FinishInteraction
        {
            get
            {
                return finishInteraction;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref finishInteraction, value);
            }
        }

        private INotification notification;

        public INotification Notification
        {
            get
            {
                return notification;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref notification, value);
            }
        }

        #endregion
    }
}