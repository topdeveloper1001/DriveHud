//-----------------------------------------------------------------------
// <copyright file="AppsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.AppStore;
using DriveHUD.Common.Wpf.Mvvm;
using Model.Shop;
using ReactiveUI;
using System;

namespace DriveHUD.Application.ViewModels
{
    public class AppsViewModel : WindowViewModelBase
    {
        public AppsViewModel()
        {
            Initialize();
        }

        #region Properties     

        private AppStoreType appStoreType;

        public AppStoreType AppStoreType
        {
            get
            {
                return appStoreType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref appStoreType, value);
            }
        }

        private IAppStoreViewModel appStoreViewModel;

        public IAppStoreViewModel AppStoreViewModel
        {
            get
            {
                return appStoreViewModel;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref appStoreViewModel, value);
            }
        }

        #endregion

        #region Infrastructure

        private void Initialize()
        {
            InitializeObservables();
            Load();
        }

        private void InitializeObservables()
        {
            this.ObservableForProperty(x => x.AppStoreType).Subscribe(x => Load());
        }

        // to do: replace with service locator
        private void Load()
        {
            switch (appStoreType)
            {
                case AppStoreType.Recommended:
                    AppStoreViewModel = new ProductAppStoreViewModel();
                    AppStoreViewModel.Initialize();
                    break;

                default:
                    AppStoreViewModel = new EmptyAppStoreViewModel();
                    break;
            }
        }

        #endregion
    }
}