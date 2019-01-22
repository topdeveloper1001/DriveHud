//-----------------------------------------------------------------------
// <copyright file="MaintenanceViewModel.cs" company="Ace Poker Solutions">
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
using System.Windows.Forms;
using System.Windows.Input;

namespace DriveHUD.Bootstrapper.App.ViewModels
{
    public class MaintenanceViewModel : PageViewModel
    {
        public MaintenanceViewModel(MainWindowViewModel mainViewModel) : base(mainViewModel)
        {
            InitializeCommands();
        }

        #region Properties

        public override PageType PageType
        {
            get
            {
                return PageType.MaintenancePage;
            }
        }

        private bool isRemovePlayerData;

        public bool IsRemovePlayerData
        {
            get
            {
                return isRemovePlayerData;
            }
            set
            {
                Set(() => IsRemovePlayerData, ref isRemovePlayerData, value);

                Bootstrapper.Engine.StringVariables[Variables.RemovePlayerData] = isRemovePlayerData ?
                    isRemovePlayerData.ToString() : null;
            }
        }

        #endregion

        #region Commands

        public ICommand RepairCommand { get; private set; }

        public ICommand RemoveCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }


        #endregion

        #region Infrastructure

        private void InitializeCommands()
        {
            CancelCommand = new RelayCommand(() =>
            {
                if (NotificationBox.Show(Resources.Common_ExitMessage_Title, Resources.Common_ExitMessage_Text,
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    BootstrapperApp.BootstrapperDispatcher.InvokeShutdown();
                }
            });

            RemoveCommand = new RelayCommand(Uninstall);
            RepairCommand = new RelayCommand(Repair);
        }

        private bool GetDefaultIsRemovePlayerDataState()
        {
            bool result = true;

            if (MainViewModel.Bootstrapper.Engine.StringVariables.Contains(Variables.RemovePlayerData))
            {
                var desktopShortcut = MainViewModel.Bootstrapper.Engine.StringVariables[Variables.RemovePlayerData];

                if (bool.TryParse(desktopShortcut, out result))
                {
                    return result;
                }
            }

            return result;
        }

        private void Uninstall()
        {
            Log(LogLevel.Standard, $"InstallView: Calling {nameof(Uninstall)}");
            MainViewModel.PlanAction(LaunchAction.Uninstall);
        }

        private void Repair()
        {
            Log(LogLevel.Standard, $"InstallView: Calling {nameof(Repair)}");
            MainViewModel.PlanAction(LaunchAction.Repair);
        }

        #endregion
    }
}