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
using DriveHUD.Common.Wpf.Events;
using DriveHUD.Common.Wpf.Mvvm;
using Microsoft.Practices.ServiceLocation;
using ReactiveUI;
using System;
using System.Diagnostics;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public abstract class AppStoreViewModel<TModel> : WindowViewModelBase, IAppStoreViewModel
        where TModel : class
    {
        public AppStoreViewModel()
        {
            Model = ServiceLocator.Current.GetInstance<TModel>();
        }

        #region Properties

        private TModel model;

        public TModel Model
        {
            get
            {
                return model;
            }
            protected set
            {
                this.RaiseAndSetIfChanged(ref model, value);
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<object> LearnMoreCommand { get; private set; }

        public ReactiveCommand<object> AddToCartCommand { get; private set; }

        #endregion

        #region IAppStoreViewModel implementation

        public abstract void Initialize();
                
        #endregion

        protected abstract void ModelInitialized();

        protected ReactiveOperation InitializeModelAsync(Action initializeModel)
        {
            var operation = StartAsyncOperation(initializeModel, ModelInitialized);
            return operation;
        }

        protected virtual void InitializeCommands()
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