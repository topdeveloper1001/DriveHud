//-----------------------------------------------------------------------
// <copyright file="SplashWindow.cs" company="Ace Poker Solutions">
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
using System.Windows.Threading;

namespace DriveHUD.Application.SplashScreen
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashWindow : Window, ISplashScreen
    {
        internal SplashWindow()
        {
            InitializeComponent();
        }

        public void CloseSplashScreen()
        {
            Dispatcher.Invoke(() => Close());
            Dispatcher.InvokeShutdown();
        }
    }
}