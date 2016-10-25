using System;
using System.Threading;
using Prism.Events;
using Prism.Mvvm;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model;
using Newtonsoft.Json;

namespace DriveHUD.Common.Infrastructure.Base
{
    public abstract class BaseViewModel : BindableBase
    {
        public SynchronizationContext synchronizationContext;

        private Guid _id = Guid.NewGuid();
        private EnumViewModelType _type = EnumViewModelType.NotDefined;
        private bool _isActive;
      
        public SingletonStorageModel StorageModel
        {
            get { return ServiceLocator.Current.TryResolve<SingletonStorageModel>(); }
        }

        public Guid Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public EnumViewModelType Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

    }
}