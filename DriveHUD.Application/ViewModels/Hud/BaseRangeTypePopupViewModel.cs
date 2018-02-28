//-----------------------------------------------------------------------
// <copyright file="BaseRangeTypePopupViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using ReactiveUI;
using System;

namespace DriveHUD.Application.ViewModels.Hud
{
    public abstract class BaseRangeTypePopupViewModel : ViewModelBase
    {
        public BaseRangeTypePopupViewModel()
        {
            InitializeCommands();
        }

        protected virtual void InitializeCommands()
        {
            SaveCommand = ReactiveCommand.Create(() => Save(), CanSave());
            CreateCommand = ReactiveCommand.Create(() => Create());
        }

        protected abstract IObservable<bool> CanSave();

        protected abstract void Save();

        protected abstract void Create();

        #region Commands

        public ReactiveCommand SaveCommand { get; private set; }

        public ReactiveCommand CreateCommand { get; private set; }

        #endregion
    }
}