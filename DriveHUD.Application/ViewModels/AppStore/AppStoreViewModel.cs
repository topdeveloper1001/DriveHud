//-----------------------------------------------------------------------
// <copyright file="AppStoreViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Utils;
using Model.AppStore;
using ReactiveUI;
using System;
using System.Diagnostics;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public abstract class AppStoreViewModel<TModel> : AppStoreBaseViewModel<TModel>
        where TModel : IAppStoreModel
    {
        #region Commands

        public ReactiveCommand<object> LearnMoreCommand { get; private set; }

        public ReactiveCommand<object> AddToCartCommand { get; private set; }

        #endregion

        protected override void InitializeCommands()
        {
            LearnMoreCommand = ReactiveCommand.Create();
            LearnMoreCommand.Subscribe(x => OnLearnMore(x));

            AddToCartCommand = ReactiveCommand.Create();
            AddToCartCommand.Subscribe(x => OnAddToCart(x));
        }

        protected abstract void OnLearnMore(object item);

        protected abstract void OnAddToCart(object item);

        protected virtual void OpenLink(string link)
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
    }
}