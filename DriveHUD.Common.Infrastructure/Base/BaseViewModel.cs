//-----------------------------------------------------------------------
// <copyright file="BaseViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Enums;
using Prism.Mvvm;
using System;

namespace DriveHUD.Common.Infrastructure.Base
{
    public abstract class BaseViewModel : BindableBase
    {
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
                RaisePropertyChanged();
            }
        }

        public EnumViewModelType Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;
                _type = value;
                RaisePropertyChanged();
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged();
            }
        }

    }
}