//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace DriveHUD.Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RadWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }      

        private void MainWindow_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!RadRibbonDropDownButton.IsOpen || RadRibbonDropDownButton.IsMouseOver) return;
            RadRibbonDropDownButton.KeepOpen = false;
            RadRibbonDropDownButton.IsOpen = false;
            RadRibbonDropDownButton.KeepOpen = true;
        }
    }
}
