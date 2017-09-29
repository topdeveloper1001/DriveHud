//-----------------------------------------------------------------------
// <copyright file="AppStoreBaseViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Events;
using DriveHUD.Common.Wpf.Mvvm;
using Microsoft.Practices.ServiceLocation;
using Model.AppStore;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public abstract class AppStoreBaseViewModel<TModel> : WindowViewModelBase, IAppStoreViewModel
        where TModel : IAppStoreModel
    {
        public event EventHandler Updated;

        public AppStoreBaseViewModel()
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

        private int gridColumns;

        public virtual int GridColumns
        {
            get
            {
                return gridColumns;
            }
            protected set
            {
                this.RaiseAndSetIfChanged(ref gridColumns, value);
            }
        }

        private int gridRows;

        public virtual int GridRows
        {
            get
            {
                return gridRows;
            }
            protected set
            {
                this.RaiseAndSetIfChanged(ref gridRows, value);
            }
        }

        public int ItemsCount
        {
            get
            {
                return Model != null ? Model.ItemsCount : default(int);
            }
        }

        public int ProductsPerPage
        {
            get
            {
                return GridColumns * GridRows;
            }
        }

        private InteractionRequest<INotification> viewRequest;

        public InteractionRequest<INotification> ViewRequest
        {
            get
            {
                return viewRequest;
            }
            protected set
            {
                this.RaiseAndSetIfChanged(ref viewRequest, value);
            }
        }

        #endregion

        #region IAppStoreViewModel implementation

        public virtual void Initialize()
        {
            InitializeCommands();
        }

        public virtual void Refresh(int pageNumber)
        {
            var start = ProductsPerPage * (pageNumber - 1);
            Model.Refresh(start, ProductsPerPage);
        }

        public virtual void Search(string searchText)
        {
            Model.Search(searchText);
            OnUpdated();
        }

        #endregion

        protected virtual void ModelInitialized()
        {
            OnUpdated();
        }

        protected ReactiveOperation InitializeModelAsync(Action initializeModel)
        {
            var operation = StartAsyncOperation(initializeModel, ModelInitialized);
            return operation;
        }

        protected abstract void InitializeCommands();

        protected virtual void OnUpdated()
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
}