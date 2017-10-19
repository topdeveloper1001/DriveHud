//-----------------------------------------------------------------------
// <copyright file="RegistrationView.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.PlayerXRay.ViewModels;
using System.Windows.Controls;

namespace DriveHUD.PlayerXRay.Views.PopupViews
{
    /// <summary>
    /// Interaction logic for RegistrationView.xaml
    /// </summary>
    public partial class RegistrationView : UserControl, IPopupContainerView
    {
        public RegistrationView(IPopupInteractionAware viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;

            InitializeComponent();
        }

        public IPopupInteractionAware ViewModel
        {
            get;
            private set;
        }
    }
}