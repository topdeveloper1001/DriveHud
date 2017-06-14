using DriveHUD.Common.Infrastructure.Base;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels
{
    public abstract class FilterViewModel<T> : BaseViewModel, IFilterViewModel where T : FilterBaseEntity, IFilterModel
    {
        protected IFilterModelManagerService FilterModelManager { get; private set; }

        internal FilterViewModel(EnumViewModelType viewModelType, IFilterModelManagerService service)
        {
            this.Type = viewModelType;
            this.FilterModelManager = service;

            InitializeFilterModel();
        }

        public virtual void InitializeFilterModel()
        {
            this.FilterModel = (T)FilterModelManager.FilterModelCollection.FirstOrDefault(x => x.GetType().Equals(typeof(T)));
            this.FilterModelClone = (T)this.FilterModel.Clone();
        }

        public virtual void RestoreDefaultState()
        {
            this.FilterModel.LoadFilter(FilterModelClone);
        }

        public virtual void UpdateDefaultState()
        {
            this.FilterModelClone = (T)this.FilterModel.Clone();
        }

        public virtual object GetDefaultStateModel()
        {
            return this.FilterModelClone;
        }

        public void UpdateDefaultStateModel(object model)
        {
            var newModel = model as T;
            if (newModel == null)
                return;

            this.FilterModelClone = (T)newModel.Clone();
        }

        #region Properties

        private T _filterModel;
        private T _filterModelClone;

        public virtual T FilterModel
        {
            get { return _filterModel; }
            set
            {
                if (value.Equals(_filterModel)) return;
                _filterModel = value;
                OnPropertyChanged();
            }
        }

        public virtual T FilterModelClone
        {
            get { return _filterModelClone; }
            set
            {
                if (value.Equals(_filterModelClone)) return;
                _filterModelClone = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
