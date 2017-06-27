//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows;

namespace DriveHUD.Updater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (MainWindowWpf.Instance.WindowState == WindowState.Minimized)
                MainWindowWpf.Instance.WindowState = WindowState.Normal;
            else
                MainWindowWpf.Instance.Activate();

            return true;
        }

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            InitializeApplication();
            base.OnStartup(e);
        }

        private void InitializeApplication()
        {
            try
            {
                InitializeSettings();           
              
                MainWindowWpf.Instance.Show();
                MainWindowWpf.Instance.Activate();                
                MainWindowWpf.Instance.ViewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void InitializeSettings()
        {           
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            HandleException(ex);
        }

        private void HandleException(Exception ex)
        {
            if (ex != null)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}