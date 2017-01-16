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

using DriveHUD.Common.Infrastructure.Base;
using Microsoft.Practices.ServiceLocation;
using Model.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Common.Utils;
using System.Diagnostics;
using DriveHUD.Common.Log;

namespace DriveHUD.Application.ViewModels
{
    public class AppsViewModel : WindowViewModelBase
    {
        public AppsViewModel()
        {
            model = ServiceLocator.Current.GetInstance<IShopModel>();

            InitializeObservables();
            InitializeCommands();
            Refresh();
        }

        #region Properties

        private IShopModel model;

        public IShopModel Model
        {
            get
            {
                return model;
            }
        }

        private ShopType shopType;

        public ShopType ShopType
        {
            get
            {
                return shopType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref shopType, value);
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<object> LearnMoreCommand { get; private set; }

        public ReactiveCommand<object> AddToCartCommand { get; private set; }

        #endregion

        #region Infrastructure

        private void InitializeCommands()
        {
            LearnMoreCommand = ReactiveCommand.Create();
            LearnMoreCommand.Subscribe(x =>
            {
                var product = x as ShopProduct;

                if (product != null)
                {
                    OpenLink(product.LearnMoreLink);
                }
            });

            AddToCartCommand = ReactiveCommand.Create();
            AddToCartCommand.Subscribe(x =>
            {
                var product = x as ShopProduct;

                if (product != null)
                {
                    OpenLink(product.CartLink);
                }
            });
        }

        private void InitializeObservables()
        {
            this.ObservableForProperty(x => x.ShopType).Subscribe(x => Refresh());
        }

        private void Refresh()
        {
            StartAsyncOperation(() => Model.Refresh(ShopType, 0, 0), () => RefreshUI());
        }

        private void RefreshUI()
        {
        }

        private void OpenLink(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return;
            }

            var browserPath = BrowserHelper.GetDefaultBrowserPath();

            try
            {
                Process.Start(browserPath, link);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Link {link} couldn't be opened", e);
            }
        }

        #endregion
    }   
}