using DriveHUD.Common.Infrastructure.Base;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Settings;

namespace DriveHUD.Application.ViewModels.Settings
{
    public abstract class SettingsViewModel<T> : BaseViewModel, ISettingsViewModel where T : ISettingsBase
    {
        internal SettingsViewModel(string name)
        {
            _name = name;
        }

        #region Properties
        private string _name;
        private Type _viewModelType;

        protected T SettingsModel { get; private set; }

        internal Type ViewModelType
        {
            get { return _viewModelType; }
            set { _viewModelType = value; }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public virtual void SetSettingsModel(ISettingsBase model)
        {
            SetSettingsModelInternal(model);
        }

        private void SetSettingsModelInternal(ISettingsBase model)
        {
            if (model != null & model is T)
            {
                SettingsModel = (T)model;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
