//-----------------------------------------------------------------------
// <copyright file="MainWindowView.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Bootstrapper.App.Common;
using DriveHUD.Bootstrapper.App.ViewModels;
using DriveHUD.Bootstrapper.App.Views;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace DriveHUD.Bootstrapper.App
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            DataContext = new MainWindowViewModel(null);
            InitializeComponent();
        }

        public MainWindowView(BootstrapperApp bootstrapperApp)
        {
            DataContext = new MainWindowViewModel(bootstrapperApp)
            {
                WindowHandle = new WindowInteropHelper(this).Handle
            };

            InitializeComponent();
        }

        public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ViewModel.BurnInstallationState != BurnInstallationState.Applying &&
                 (ViewModel.PageViewModel != null && (ViewModel.PageViewModel.PageType == PageType.FinishPage
                  || ViewModel.PageViewModel.PageType == PageType.FinishErrorPage)))
            {
                return;
            }

            if (ViewModel.ShouldCancel)
            {
                NotificationBox.Show(Properties.Resources.Common_CancelAlreadyInProgressMessageBoxTitle,
                    Properties.Resources.Common_CancelAlreadyInProgressMessageBoxBody, MessageBoxButtons.OK);

                e.Cancel = true;

                return;
            }
        }
    }
}