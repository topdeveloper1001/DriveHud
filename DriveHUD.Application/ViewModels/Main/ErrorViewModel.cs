//-----------------------------------------------------------------------
// <copyright file="ErrorViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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
using System.Windows;

namespace DriveHUD.Application.ViewModels.Main
{
    public class ErrorViewModel : PopupWindowViewModel
    {
        private ErrorViewModelInfo errorViewModelInfo;

        public override void Configure(object viewModelInfo)
        {
            errorViewModelInfo = viewModelInfo as ErrorViewModelInfo;

            if (errorViewModelInfo == null)
            {
                OnClosed();
                return;
            }

            Title = errorViewModelInfo.Title;

            ErrorMessage = errorViewModelInfo.Exception != null ?
                errorViewModelInfo.Exception.ToString() :
                errorViewModelInfo.ErrorMessage;

            InitializeCommands();
            OnInitialized();
        }

        #region Properties

        private string errorMessage;

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref errorMessage, value);
            }
        }

        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref title, value);
            }
        }

        #endregion


        #region Commands

        public ReactiveCommand OKCommand { get; private set; }

        public ReactiveCommand CopyCommand { get; private set; }

        #endregion

        private void InitializeCommands()
        {
            OKCommand = ReactiveCommand.Create(() =>
            {
                errorViewModelInfo?.OKAction?.Invoke();
                OnClosed();
            });

            CopyCommand = ReactiveCommand.Create(() =>
            {
                Clipboard.SetText(ErrorMessage);
            });
        }
    }
}