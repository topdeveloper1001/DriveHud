//-----------------------------------------------------------------------
// <copyright file="AppsAppStoreViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.PlayerXRay;
using Model.AppStore;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public class AppsAppStoreViewModel : AppStoreBaseViewModel<IAppsAppStoreModel>
    {
        public InteractionRequest<INotification> OpenAppRequest { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            OpenAppRequest = new InteractionRequest<INotification>();

            GridColumns = 2;
            GridRows = 3;

            InitializeModelAsync(() =>
            {
                Model.Load();
            });
        }

        #region Commands

        public ReactiveCommand<object> LaunchCommand { get; private set; }

        #endregion

        protected override void InitializeCommands()
        {
            LaunchCommand = ReactiveCommand.Create();
            LaunchCommand.Subscribe(x => Launch(x));
        }

        private void Launch(object item)
        {
            var appStoreProduct = item as AppStoreProduct;

            if (appStoreProduct == null)
            {
                return;
            }

            var appContainerViewModel = new AppContainerViewModel(appStoreProduct)
            {
                ContainerViewModel = new PlayerXRayMainViewModel()
            };

            OpenAppRequest?.Raise(appContainerViewModel);
        }
    }
}