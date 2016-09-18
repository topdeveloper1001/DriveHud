using DriveHUD.Bootstrapper.App.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using DriveHUD.Bootstrapper.App.Enums;
using System.Diagnostics;
using System.Windows.Input;
using DriveHUD.Bootstrapper.App.Infrastructure;
using System.Windows;
using GalaSoft.MvvmLight;

namespace DriveHUD.Bootstrapper.App.ViewModels
{
    public class ProgressViewModel : ViewModelBase, ICancelable
    {
        #region Fields
        private BootstrapperAppSingletonModel _model
        {
            get { return BootstrapperAppSingletonModel.Instance; }
        }

        private int _cacheProgress;
        private int _executeProgress;
        #endregion

        public ProgressViewModel()
        {
            State = InstallState.Initializing;

            CancelCommand = new RelayCommand(Cancel);

            WireUpEventHandlers();
            UpdateActionText();
        }

        #region Properties
        private int _progressPercentage;
        private int _progress;
        private string _currentPackage;
        private string _packageAction;
        private string _currentAction;
        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string CurrentAction
        {
            get { return _currentAction; }
            set { Set(() => CurrentAction, ref _currentAction, value); }
        }

        public string PackageAction
        {
            get { return _packageAction; }
            set { Set(() => PackageAction, ref _packageAction, value); }
        }

        public string CurrentPackage
        {
            get { return _currentPackage; }
            set { Set(() => CurrentPackage, ref _currentPackage, value); }
        }

        public int Progress
        {
            get { return _progress; }
            set { Set(() => Progress, ref _progress, value); }
        }

        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            set { Set(() => ProgressPercentage, ref _progressPercentage, value); }
        }

        public InstallState State { get; set; }
        #endregion

        #region ICommand
        public ICommand CancelCommand { get; private set; }
        #endregion

        #region  Methods
        private void UpdateActionText()
        {
            switch (_model.LastPlannedAction)
            {
                case LaunchAction.Uninstall:
                    CurrentAction = "Uninstalling...";
                    Description = "Please wait while setup uninstalls DriveHUD from your computer. This may take a few minutes..";
                    break;
                case LaunchAction.Install:
                    CurrentAction = "Installing...";
                    Description = "Please wait while setup installs DriveHUD from your computer. This may take a few minutes..";
                    break;
                case LaunchAction.Repair:
                    CurrentAction = "Repairing...";
                    Description = "Please wait while setup repairs DriveHUD from your computer. This may take a few minutes..";
                    break;
                default:
                    break;
            }
        }

        private void WireUpEventHandlers()
        {
            _model.Bootstrapper.ApplyBegin += ApplyBegin;
            _model.Bootstrapper.ApplyComplete += ApplyComplete;

            _model.Bootstrapper.ExecutePackageBegin += ExecutePackageBegin;
            _model.Bootstrapper.ExecutePackageComplete += ExecutePackageComplete;

            _model.Bootstrapper.CacheAcquireProgress += CacheAcquireProgress;
            _model.Bootstrapper.ExecuteProgress += ExecuteProgress;

            _model.Bootstrapper.ExecuteMsiMessage += ExecuteMsiMessage;

            _model.Bootstrapper.Error += ExecuteError;

            _model.Bootstrapper.ResolveSource += ResolveSource;
        }

        private void RemoveEventHandlers()
        {
            _model.Bootstrapper.ApplyBegin -= ApplyBegin;
            _model.Bootstrapper.ApplyComplete -= ApplyComplete;

            _model.Bootstrapper.ExecutePackageBegin -= ExecutePackageBegin;
            _model.Bootstrapper.ExecutePackageComplete -= ExecutePackageComplete;

            _model.Bootstrapper.CacheAcquireProgress -= CacheAcquireProgress;
            _model.Bootstrapper.ExecuteProgress -= ExecuteProgress;

            _model.Bootstrapper.ExecuteMsiMessage -= ExecuteMsiMessage;

            _model.Bootstrapper.Error -= ExecuteError;

            _model.Bootstrapper.ResolveSource -= ResolveSource;
        }

        private void ResolveSource(object sender, ResolveSourceEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.DownloadSource))
            {
                e.Result = Result.Download;
            }
            else
            {
                e.Result = Result.Ok;
            }
        }

        private void ExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
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
        }

        private void ApplyBegin(object sender, ApplyBeginEventArgs e)
        {
            State = InstallState.Applying;

        }

        private void ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            RemoveEventHandlers();
            _model.FinalResult = e.Status;
            _model.InvokeFinalView();
        }

        private void ExecutePackageBegin(object sender, ExecutePackageBeginEventArgs e)
        {
            var package = _model.BundleInfo.Packages.FirstOrDefault(x => x.Id == e.PackageId);
            CurrentPackage = package?.DisplayName;
            if (this.State == InstallState.Cancelled)
            {
                e.Result = Result.Cancel;
            }
        }

        private void ExecutePackageComplete(object sender, ExecutePackageCompleteEventArgs e)
        {
            var package = _model.BundleInfo.Packages.FirstOrDefault(x => x.Id == e.PackageId);
            _model.LogMessage(string.Format("package: {0}, status: {1}, result: {2}", package?.DisplayName, e.Status, e.Result));
            if (e.Status != 0)
            {
                _model.ErrorsList.Add(string.Format("Failed  to install {0}, the installation has been cancelled", package?.DisplayName));
                this.State = InstallState.Cancelled;
            }

            if (this.State == InstallState.Cancelled)
            {
                e.Result = Result.Cancel;
            }
        }

        private void UpdateProgress()
        {
            this.Progress = (this._cacheProgress + this._executeProgress) / 2;
        }

        private void ExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            this._executeProgress = e.OverallPercentage;
            this.ProgressPercentage = e.ProgressPercentage;
            UpdateProgress();

            if (this.State == InstallState.Cancelled)
            {
                e.Result = Result.Cancel;
            }
        }

        private void CacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
        {
            this._cacheProgress = e.OverallPercentage;
            UpdateProgress();
        }

        private void ExecuteError(object sender, ErrorEventArgs e)
        {
            _model.ProcessError(e);
        }
        #endregion

        #region ICommand Implementation
        public void Cancel(object ojb)
        {
            if (System.Windows.MessageBox.Show("Are you sure you want to cancel?", "Cancel", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this._model.LogMessage("Cancelling...");
                this._model.ErrorsList.Add("Cancelled by user.");
                this.CurrentAction = "Cancelling...";
                this.Description = "Please wait while setup cancels the installation. This may take a few minutes.";

                if (this.State == InstallState.Applying)
                {
                    this.State = InstallState.Cancelled;
                }
                else
                {
                    _model.InvokeFinalView();
                }
            }
        }
        #endregion

    }
}
