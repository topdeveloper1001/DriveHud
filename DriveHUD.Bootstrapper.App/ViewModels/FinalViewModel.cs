//-----------------------------------------------------------------------
// <copyright file="FinalViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Bootstrapper.App.Properties;
using GalaSoft.MvvmLight.Command;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DriveHUD.Bootstrapper.App.ViewModels
{
    public class FinalViewModel : PageViewModel
    {
        public FinalViewModel(MainWindowViewModel mainViewModel) : base(mainViewModel)
        {
            InitializeCommands();

            Description = IsError ? Resources.Common_FinalView_ErrorText : Resources.Common_FinalView_ThankYouText;

            switch (MainViewModel.ExecutedAction)
            {
                case LaunchAction.Uninstall:
                    FinalText = IsError ? Resources.Common_FinalView_UninstallFailed : Resources.Common_FinalView_UninstallSucceeded;
                    break;
                case LaunchAction.Install:
                    FinalText = IsError ? Resources.Common_FinalView_InstallFailed : Resources.Common_FinalView_InstallSucceeded;
                    break;
                case LaunchAction.Repair:
                    FinalText = IsError ? Resources.Common_FinalView_RepairFailed : Resources.Common_FinalView_RepairSucceeded;
                    break;
                default:
                    FinalText = Resources.Common_FinalView_Failed;
                    break;
            }
        }

        #region Properties

        public override PageType PageType
        {
            get
            {
                return PageType.ProgressPage;
            }
        }

        public List<string> ErrorsList
        {
            get
            {
                return MainViewModel.ErrorMessages;
            }
        }

        public bool IsError
        {
            get
            {
                return ErrorsList.Count > 0;
            }
        }

        public bool IsCanLaunch
        {
            get
            {
                return !IsError && MainViewModel.ExecutedAction == LaunchAction.Install;
            }
        }

        private string description;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                Set(() => Description, ref description, value);
            }
        }

        private string finalText;

        public string FinalText
        {
            get
            {
                return finalText;
            }
            set
            {
                Set(() => FinalText, ref finalText, value);
            }
        }

        #endregion

        #region Commmands

        public ICommand CloseCommand { get; private set; }

        public ICommand LaunchCommand { get; private set; }

        #endregion

        #region Infrastructure

        private void InitializeCommands()
        {
            LaunchCommand = new RelayCommand(Launch);

            CloseCommand = new RelayCommand(() => Bootstrapper.Engine.Quit(MainViewModel.Status));
        }

        public void Launch()
        {
            var installDirectory = Bootstrapper.Engine.StringVariables[Variables.InstallFolder].Trim('"');
            var executable = Bootstrapper.Engine.StringVariables[Variables.LaunchProgram].Trim('"');

            var fileToLaunch = Path.Combine(installDirectory, executable);

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Path.GetDirectoryName(fileToLaunch),
                    FileName = executable,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, $"Could not launch {fileToLaunch}: {ex}");

                MessageBox.Show("Failed to launch the application", "DriveHUD", MessageBoxButton.OK);
            }

            Bootstrapper.Engine.Quit(MainViewModel.Status);
        }

        #endregion
    }
}