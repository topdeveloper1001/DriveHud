﻿//-----------------------------------------------------------------------
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

using DriveHUD.Common.Log;
using DriveHUD.Common.Wpf.Interactivity;
using Model.AppStore;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public class AppsAppStoreViewModel : AppStoreBaseViewModel<IAppsAppStoreModel>
    {
        public override void Initialize()
        {
            base.Initialize();

            ViewRequest = new InteractionRequest<INotification>();

            GridColumns = 2;
            GridRows = 3;

            InitializeModelAsync(() =>
            {
                Model.Load(null);
            });
        }

        #region Commands

        public ReactiveCommand LaunchCommand { get; private set; }

        #endregion

        protected override void InitializeCommands()
        {
            LaunchCommand = ReactiveCommand.Create<AppStoreModule>(x => Launch(x));
        }

        private void Launch(AppStoreModule appStoreModule)
        {
            if (appStoreModule == null || string.IsNullOrEmpty(appStoreModule.ModuleName))
            {
                return;
            }

            try
            {
                var moduleViewRequest = new ViewRequestInfo
                {
                    Title = appStoreModule.ProductName
                };

                ViewName = appStoreModule.ModuleName;
                ViewIconSource = appStoreModule.WindowIconSource;
                ViewWidth = appStoreModule.WindowWidth;
                ViewHeight = appStoreModule.WindowHeight;
                ViewRequest?.Raise(moduleViewRequest);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not launch module '{appStoreModule.ModuleName}'", e);
            }
        }
    }
}