//-----------------------------------------------------------------------
// <copyright file="FilterViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using Model.Enums;
using Model.Filters;
using System.Linq;

namespace DriveHUD.Application.ViewModels
{
    public abstract class FilterViewModel<T> : BaseViewModel, IFilterViewModel where T : FilterBaseEntity, IFilterModel
    {
        protected IFilterModelManagerService FilterModelManager { get; private set; }

        internal FilterViewModel(EnumViewModelType viewModelType, IFilterModelManagerService service)
        {
            Type = viewModelType;
            FilterModelManager = service;

            InitializeFilterModel();
        }

        public virtual void InitializeFilterModel()
        {
            FilterModel = (T)FilterModelManager.FilterModelCollection.FirstOrDefault(x => x.GetType().Equals(typeof(T)));
            FilterModelClone = (T)FilterModel.Clone();
        }

        public virtual void RestoreDefaultState()
        {
            FilterModel.LoadFilter(FilterModelClone);
        }

        public virtual void UpdateDefaultState()
        {
            FilterModelClone = (T)FilterModel.Clone();
        }

        public virtual object GetDefaultStateModel()
        {
            return FilterModelClone;
        }

        public void UpdateDefaultStateModel(object model)
        {
            if (!(model is T newModel))
            {
                return;
            }

            FilterModelClone = (T)newModel.Clone();
        }

        #region Properties

        private T _filterModel;
        private T _filterModelClone;

        public virtual T FilterModel
        {
            get
            {
                return _filterModel;
            }
            set
            {
                SetProperty(ref _filterModel, value);
            }
        }

        public virtual T FilterModelClone
        {
            get
            {
                return _filterModelClone;
            }
            set
            {
                SetProperty(ref _filterModelClone, value);
            }
        }

        #endregion
    }
}