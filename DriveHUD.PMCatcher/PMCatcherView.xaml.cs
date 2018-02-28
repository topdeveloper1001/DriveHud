﻿//-----------------------------------------------------------------------
// <copyright file="PMCatcherView.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Actions;
using System;
using System.Windows.Controls;

namespace DriveHUD.PMCatcher
{
    /// <summary>
    /// Interaction logic for PMCatcherView.xaml
    /// </summary>
    public partial class PMCatcherView : UserControl, IViewModelContainer<IPMCatcherViewModel>
    {
        public PMCatcherView(IPMCatcherViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.Initialized += ViewModel_Initialized;
            InitializeComponent();
        }

        private void ViewModel_Initialized(object sender, EventArgs e)
        {
            DataContext = ViewModel;
            ViewModel.Initialized -= ViewModel_Initialized;
        }

        public IPMCatcherViewModel ViewModel
        {
            get;
            private set;
        }
    }
}