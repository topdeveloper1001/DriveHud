//-----------------------------------------------------------------------
// <copyright file="ProgressViewModel.cs" company="Ace Poker Solutions">
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
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace DriveHUD.Bootstrapper.App.ViewModels
{
    public class ProgressViewModel : PageViewModel
    {
        public ProgressViewModel(MainWindowViewModel mainViewModel) : base(mainViewModel)
        {
            InitializeCommands();
            InitializeEvents();

            switch (MainViewModel.LaunchAction)
            {
                case LaunchAction.Install:
                    ActionText = Resources.Common_PlanView_InstallAction;
                    ActionDescription = Resources.Common_PlanView_InstallActionDescription;
                    break;
                case LaunchAction.Uninstall:
                    ActionText = Resources.Common_PlanView_UninstallAction;
                    ActionDescription = Resources.Common_PlanView_UninstallActionDescription;
                    break;
                case LaunchAction.Repair:
                    ActionText = Resources.Common_PlanView_RepairAction;
                    ActionDescription = Resources.Common_PlanView_RepairActionDescription;
                    break;
            }
        }

        #region Properties

        private int cacheProgress;
        private int executeProgress;

        public override PageType PageType
        {
            get
            {
                return PageType.ProgressPage;
            }
        }

        private int progress;

        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                Set(nameof(Progress), ref progress, value);
            }
        }

        private int progressPercentage;

        public int ProgressPercentage
        {
            get
            {
                return progressPercentage;
            }
            set
            {
                Set(nameof(ProgressPercentage), ref progressPercentage, value);
            }
        }

        private string actionText;

        public string ActionText
        {
            get
            {
                return actionText;
            }
            set
            {
                Set(nameof(ActionText), ref actionText, value);
            }
        }

        private string actionDescription;

        public string ActionDescription
        {
            get
            {
                return actionDescription;
            }
            set
            {
                Set(nameof(ActionDescription), ref actionDescription, value);
            }
        }

        private string currentPackage;

        public string CurrentPackage
        {
            get
            {
                return currentPackage;
            }
            set
            {
                Set(nameof(CurrentPackage), ref currentPackage, value);
            }
        }

        private string packageAction;

        public string PackageAction
        {
            get
            {
                return packageAction;
            }
            set
            {
                Set(nameof(PackageAction), ref packageAction, value);
            }
        }

        #endregion

        #region Commands

        public ICommand CancelCommand { get; private set; }

        #endregion

        private void InitializeCommands()
        {
            CancelCommand = new RelayCommand(() =>
            {
                if (NotificationBox.Show(Resources.Common_CancelDialogTitle,
                    Resources.Common_CancelDialogBody, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Log(LogLevel.Standard, $"User has cancelled {MainViewModel.LaunchAction} process.");

                    ActionText = Resources.Common_ProgressView_Cancelling;
                    ActionDescription = Resources.Common_ProgressView_CancellingDescription;

                    MainViewModel.ShouldCancel = true;
                    CommandManager.InvalidateRequerySuggested();
                }
            }, () => !MainViewModel.ShouldCancel);
        }

        private void InitializeEvents()
        {
            Bootstrapper.CacheAcquireProgress += Bootstrapper_CacheAcquireProgress;
            Bootstrapper.CacheAcquireBegin += Bootstrapper_CacheAcquireBegin;
            Bootstrapper.CacheAcquireComplete += Bootstrapper_CacheAcquireComplete;
            Bootstrapper.ExecutePackageBegin += Bootstrapper_ExecuteBegin;
            Bootstrapper.ExecutePackageComplete += Bootstrapper_ExecuteComplete;
            Bootstrapper.ExecuteMsiMessage += Bootstrapper_ExecuteMsiMessage;
            Bootstrapper.ExecuteProgress += Bootstrapper_ExecuteProgress;
            Bootstrapper.Progress += Bootstrapper_Progress;
        }

        private void Bootstrapper_Progress(object sender, ProgressEventArgs e)
        {
            HandleCancellation(e);
        }

        private void Bootstrapper_ExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            executeProgress = e.OverallPercentage;
            ProgressPercentage = e.ProgressPercentage;
            UpdateProgress();
            HandleCancellation(e);
        }

        private void Bootstrapper_ExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
        {
            switch (e.MessageType)
            {
                case InstallMessage.ActionStart:
                    PackageAction = e.Data.LastOrDefault();
                    break;
                case InstallMessage.ActionData:
                    PackageAction = e.Data.FirstOrDefault();
                    break;
                default:
                    break;
            }

            HandleCancellation(e);
        }

        private void Bootstrapper_ExecuteComplete(object sender, ExecutePackageCompleteEventArgs e)
        {
            if (e.Restart == ApplyRestart.RestartInitiated ||
               e.Restart == ApplyRestart.RestartRequired)
            {
                MainViewModel.IsRestartRequired = true;
            }

            HandleCancellation(e);
        }

        private void Bootstrapper_ExecuteBegin(object sender, ExecutePackageBeginEventArgs e)
        {
            CurrentPackage = GetPackageNameById(e.PackageId);
            HandleCancellation(e);
        }

        private void Bootstrapper_CacheAcquireComplete(object sender, CacheAcquireCompleteEventArgs e)
        {
            HandleCancellation(e);
        }

        private void Bootstrapper_CacheAcquireBegin(object sender, CacheAcquireBeginEventArgs e)
        {
            CurrentPackage = GetPackageNameById(e.PackageOrContainerId);
            HandleCancellation(e);
        }

        private void Bootstrapper_CacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
        {
            cacheProgress = e.OverallPercentage;
            UpdateProgress();
            HandleCancellation(e);
        }

        private void HandleCancellation(ResultEventArgs e)
        {
            if (!MainViewModel.ShouldCancel)
            {
                return;
            }

            e.Result = Result.Cancel;
        }

        private void UpdateProgress()
        {
            Progress = (cacheProgress + executeProgress) / 2;
        }

        private string GetPackageNameById(string packageId)
        {
            var package = Bootstrapper.BundleInfo?.Packages?.FirstOrDefault(x => x.Id.Equals(packageId));
            return package != null && !string.IsNullOrEmpty(package.DisplayName) ? package.DisplayName : packageId;
        }
    }
}