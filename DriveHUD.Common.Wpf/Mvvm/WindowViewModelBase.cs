//-----------------------------------------------------------------------
// <copyright file="IPopupWindowViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.Common.Wpf.Mvvm
{
    public abstract class WindowViewModelBase<TViewModel> : WpfViewModel<TViewModel>, IWindowViewModelBase
         where TViewModel : WindowViewModelBase<TViewModel>
    {
        public event EventHandler Initialized;

        protected virtual void OnInitialized()
        {
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Closed;

        protected virtual void OnClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public virtual bool OnClosing()
        {
            return true;
        }

        public abstract void Configure(object viewModelInfo);
    }
}