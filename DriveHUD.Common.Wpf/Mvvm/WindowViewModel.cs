//-----------------------------------------------------------------------
// <copyright file="WindowViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Events;
using Microsoft.Practices.ServiceLocation;
using ReactiveUI;
using System;

namespace DriveHUD.Common.Wpf.Mvvm
{
    public abstract class WindowViewModel<TViewModel, TModel> : WindowViewModelBase<TViewModel>, IWindowViewModel<TModel>
        where TViewModel : WindowViewModel<TViewModel, TModel>
    {
        public WindowViewModel() : base()
        {
            Model = ServiceLocator.Current.GetInstance<TModel>();
        }

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

        protected virtual void Initialize()
        {
            InitializeCommands();
            OnInitialized();
        }

        protected ReactiveOperation InitializeModelAsync(Action initializeModel)
        {
            var operation = StartAsyncOperation(initializeModel, ModelInitialized);
            return operation;
        }

        protected abstract void InitializeCommands();

        protected virtual void ModelInitialized(Exception e)
        {
            ApplyRules();
        }
    }
}