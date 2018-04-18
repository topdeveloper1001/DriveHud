//-----------------------------------------------------------------------
// <copyright file="ErrorView.xaml.cs" company="Ace Poker Solutions">
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
using System;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.Views.Main
{
    /// <summary>
    /// Interaction logic for ErrorView.xaml
    /// </summary>
    public partial class ErrorView : RadWindow
    {
        public ErrorView(ErrorViewModel errorViewModel)
        {
            ViewModel = errorViewModel;
            ViewModel.Initialized += ViewModel_Initialized;
            InitializeComponent();
        }

        private void ViewModel_Initialized(object sender, EventArgs e)
        {
            ViewModel.Initialized -= ViewModel_Initialized;
            DataContext = ViewModel;
        }

        public ErrorViewModel ViewModel
        {
            get; private set;
        }
    }
}