//-----------------------------------------------------------------------
// <copyright file="PlayerXRayMainView.xaml.cs" company="Ace Poker Solutions">
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
using System.Windows;
using System.Windows.Controls;

namespace DriveHUD.PlayerXRay
{
    /// <summary>
    /// Interaction logic for PlayerXRayMainView.xaml
    /// </summary>
    public partial class PlayerXRayMainView : UserControl, IViewModelContainer<PlayerXRayMainViewModel>, IViewContainer
    {
        public PlayerXRayMainView()
        {
            InitializeComponent();
            Loaded += PlayerXRayMainView_Loaded;
        }

        public PlayerXRayMainViewModel ViewModel
        {
            get
            {
                return DataContext as PlayerXRayMainViewModel;
            }
        }

        public ContentControl Window
        {
            get; set;
        }

        private void PlayerXRayMainView_Loaded(object sender, EventArgs e)
        {
            Loaded -= PlayerXRayMainView_Loaded;
            ViewModel?.Initialize();
        }
    }
}