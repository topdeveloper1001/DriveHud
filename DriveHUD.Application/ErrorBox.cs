//-----------------------------------------------------------------------
// <copyright file="ErrorBox.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Main;
using DriveHUD.Application.Views.Main;
using System;

namespace DriveHUD.Application
{
    internal class ErrorBox
    {
        public static void Show(string title, Exception exception, string errorMessage = null)
        {
            var errorViewModel = new ErrorViewModel();
            var errorView = new ErrorView(errorViewModel);

            var owner = System.Windows.Application.Current.MainWindow;

            if (owner != null && errorView.Owner != owner)
            {
                errorView.Owner = owner;
            }

            var errorViewModelInfo = new ErrorViewModelInfo
            {
                Title = title,
                ErrorMessage = errorMessage,
                Exception = exception
            };

            errorViewModel.Configure(errorViewModelInfo);
            errorViewModel.Closed += (s, a) => errorView?.Close();
            errorView.ShowDialog();
        }
    }
}