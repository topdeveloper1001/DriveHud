//-----------------------------------------------------------------------
// <copyright file="PopupViewModelBase.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Application.ViewModels
{
    public abstract class PopupViewModelBase : ViewModelBase
    {
        public PopupViewModelBase()
        {
        }

        protected virtual void InitializeCommands()
        {
            ClosePopupCommand = ReactiveCommand.Create();
            ClosePopupCommand.Subscribe(x => ClosePopup());
        }

        public ReactiveCommand<object> ClosePopupCommand { get; private set; }

        protected bool isPopupOpened;

        public bool IsPopupOpened
        {
            get
            {
                return isPopupOpened;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isPopupOpened, value);
            }
        }

        protected object popupViewModel;

        public object PopupViewModel
        {
            get
            {
                return popupViewModel;
            }
            protected set
            {
                this.RaiseAndSetIfChanged(ref popupViewModel, value);
            }
        }

        protected virtual void ClosePopup()
        {            
            IsPopupOpened = false;
            PopupViewModel = null;
        }

        protected virtual void OpenPopup(object popupViewModel)
        {
            PopupViewModel = popupViewModel;
            IsPopupOpened = true;
        }
    }
}