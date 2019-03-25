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
using System.Runtime.InteropServices;
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
                    FinalText = IsError ? Resources.Common_FinalView_InstallFailed :
                        (IsRebootVisible ? Resources.Common_FinalView_InstallSucceededWithReboot :
                            Resources.Common_FinalView_InstallSucceeded);
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
                return ErrorsList.Count > 0 || 
                    MainViewModel.BurnInstallationState == BurnInstallationState.Failed;
            }
        }

        public bool IsCanLaunch
        {
            get
            {
                return !IsError && MainViewModel.ExecutedAction == LaunchAction.Install && !MainViewModel.IsRestartRequired;
            }
        }

        public bool IsRebootVisible
        {
            get
            {
                return MainViewModel.IsRestartRequired;
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

        public ICommand RebootCommand { get; private set; }

        #endregion

        #region Infrastructure

        private void InitializeCommands()
        {
            LaunchCommand = new RelayCommand(Launch);
            RebootCommand = new RelayCommand(Reboot);
            CloseCommand = new RelayCommand(() => Bootstrapper.Engine.Quit(MainViewModel.Status));
        }

        private void Launch()
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

        private void Reboot()
        {
            Log(LogLevel.Error, $"Rebooting system.");

            try
            {
                if (!TokenAdjuster.EnablePrivilege("SeShutdownPrivilege", true) ||
                    !ExitWindowsEx(ExitWindows.Reboot | ExitWindows.ForceIfHung, ShutdownReason.MinorInstallation))
                {
                    Log(LogLevel.Error, $"Failed to reboot: {Marshal.GetLastWin32Error()}");
                }
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, $"Failed to reboot: {ex}");
            }

            Bootstrapper.Engine.Quit(MainViewModel.Status);
        }

        #endregion

        #region Exit pinvoke 

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ExitWindowsEx(ExitWindows uFlags, ShutdownReason dwReason);

        [Flags]
        enum ExitWindows : uint
        {
            // ONE of the following five:
            LogOff = 0x00,
            ShutDown = 0x01,
            Reboot = 0x02,
            PowerOff = 0x08,
            RestartApps = 0x40,
            // plus AT MOST ONE of the following two:
            Force = 0x04,
            ForceIfHung = 0x10,
        }

        [Flags]
        enum ShutdownReason : uint
        {
            MajorApplication = 0x00040000,
            MajorHardware = 0x00010000,
            MajorLegacyApi = 0x00070000,
            MajorOperatingSystem = 0x00020000,
            MajorOther = 0x00000000,
            MajorPower = 0x00060000,
            MajorSoftware = 0x00030000,
            MajorSystem = 0x00050000,

            MinorBlueScreen = 0x0000000F,
            MinorCordUnplugged = 0x0000000b,
            MinorDisk = 0x00000007,
            MinorEnvironment = 0x0000000c,
            MinorHardwareDriver = 0x0000000d,
            MinorHotfix = 0x00000011,
            MinorHung = 0x00000005,
            MinorInstallation = 0x00000002,
            MinorMaintenance = 0x00000001,
            MinorMMC = 0x00000019,
            MinorNetworkConnectivity = 0x00000014,
            MinorNetworkCard = 0x00000009,
            MinorOther = 0x00000000,
            MinorOtherDriver = 0x0000000e,
            MinorPowerSupply = 0x0000000a,
            MinorProcessor = 0x00000008,
            MinorReconfig = 0x00000004,
            MinorSecurity = 0x00000013,
            MinorSecurityFix = 0x00000012,
            MinorSecurityFixUninstall = 0x00000018,
            MinorServicePack = 0x00000010,
            MinorServicePackUninstall = 0x00000016,
            MinorTermSrv = 0x00000020,
            MinorUnstable = 0x00000006,
            MinorUpgrade = 0x00000003,
            MinorWMI = 0x00000015,

            FlagUserDefined = 0x40000000,
            FlagPlanned = 0x80000000
        }

        #endregion
    }
}