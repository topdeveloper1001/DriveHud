﻿//-----------------------------------------------------------------------
// <copyright file="InstallViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Bootstrapper.App.Views;
using GalaSoft.MvvmLight.Command;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;

namespace DriveHUD.Bootstrapper.App.ViewModels
{
    public class InstallViewModel : PageViewModel
    {
        public InstallViewModel(MainWindowViewModel mainViewModel) : base(mainViewModel)
        {
            Initialize();
            InitializeCommands();

            SetLicenseAgreementText();

        }

        #region Properties

        public override PageType PageType
        {
            get
            {
                return PageType.InstallPage;
            }
        }

        private string installationPath;

        public string InstallationPath
        {
            get
            {
                return installationPath;
            }
            set
            {
                Set(() => InstallationPath, ref installationPath, value);

                Bootstrapper.Engine.StringVariables[Variables.InstallFolder] = installationPath;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool createDesktopShortcut;

        public bool CreateDesktopShortcut
        {
            get
            {
                return createDesktopShortcut;
            }
            set
            {
                Set(() => CreateDesktopShortcut, ref createDesktopShortcut, value);

                Bootstrapper.Engine.StringVariables[Variables.InstallDesktopShortcut] = createDesktopShortcut ?
                    createDesktopShortcut.ToString() : null;
            }
        }

        private bool createProgramMenuShortcut;

        public bool CreateProgramMenuShortcut
        {
            get
            {
                return createProgramMenuShortcut;
            }
            set
            {
                Set(() => CreateProgramMenuShortcut, ref createProgramMenuShortcut, value);

                Bootstrapper.Engine.StringVariables[Variables.InstallProgramMenuShortcut] = createProgramMenuShortcut ?
                    createProgramMenuShortcut.ToString() : null;
            }
        }

        private string licenseAgreementSource;

        public string LicenseAgreementSource
        {
            get
            {
                return licenseAgreementSource;
            }
            set
            {
                Set(() => LicenseAgreementSource, ref licenseAgreementSource, value);
            }
        }

        #endregion

        #region Commands

        public ICommand InstallCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        public ICommand OpenFileDialogCommand { get; private set; }


        #endregion

        #region Infrastructure

        public void SetLicenseAgreementText()
        {
            LicenseAgreementSource = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EULA.rtf");
        }

        private void Initialize()
        {
            InstallationPath = GetDefaultInstallPath();
            CreateDesktopShortcut = GetDefaultDesktopShortcutState();
            CreateProgramMenuShortcut = GetDefaultProgramMenuShortcutState();
        }

        private void InitializeCommands()
        {
            OpenFileDialogCommand = new RelayCommand(OpenFileDialog);

            CancelCommand = new RelayCommand(() =>
            {
                if (NotificationBox.Show(Resources.Common_ExitMessage_Title, Resources.Common_ExitMessage_Text,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    BootstrapperApp.BootstrapperDispatcher.InvokeShutdown();
                }
            });

            InstallCommand = new RelayCommand(Install, () => !string.IsNullOrWhiteSpace(InstallationPath));
        }

        #endregion

        #region ICommand Implementation

        private void Install()
        {
            Log(LogLevel.Standard, $"InstallView: Calling {nameof(Install)}");

            MainViewModel.PlanAction(LaunchAction.Install);
        }

        private void OpenFileDialog()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    InstallationPath = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private string GetDefaultInstallPath()
        {
            var subFolder = MainViewModel.Bootstrapper.BundleName;
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), Bootstrapper.Engine.StringVariables[Variables.Manufacturer], subFolder);
        }

        private bool GetDefaultDesktopShortcutState()
        {
            bool result = true;

            if (MainViewModel.Bootstrapper.Engine.StringVariables.Contains(Variables.InstallDesktopShortcut))
            {
                var desktopShortcut = MainViewModel.Bootstrapper.Engine.StringVariables[Variables.InstallDesktopShortcut];

                if (bool.TryParse(desktopShortcut, out result))
                {
                    return result;
                }
            }

            return result;
        }

        private bool GetDefaultProgramMenuShortcutState()
        {
            bool result = true;

            if (MainViewModel.Bootstrapper.Engine.StringVariables.Contains(Variables.InstallProgramMenuShortcut))
            {
                var installProgramMenuShortcut = MainViewModel.Bootstrapper.Engine.StringVariables[Variables.InstallProgramMenuShortcut];

                if (bool.TryParse(installProgramMenuShortcut, out result))

                {
                    return result;
                }
            }

            return result;
        }

        #endregion
    }
}