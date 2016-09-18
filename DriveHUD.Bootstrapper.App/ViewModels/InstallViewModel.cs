using DriveHUD.Bootstrapper.App.Enums;
using DriveHUD.Bootstrapper.App.Infrastructure;
using DriveHUD.Bootstrapper.App.Models;
using DriveHUD.Bootstrapper.App.Views;
using GalaSoft.MvvmLight;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace DriveHUD.Bootstrapper.App.ViewModels
{
    public class InstallViewModel : ViewModelBase, ICancelable
    {
        #region Fields
        private BootstrapperAppSingletonModel _model
        {
            get { return BootstrapperAppSingletonModel.Instance; }
        }
        #endregion

        #region Properties
        private InstallState _state;
        private bool? _isCreateDesktopShortcut;
        private bool? _isCreateProgramMenuShortcut;
        private string _installDir;
        private string _licenseAgreementSource;
        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }


        public string LicenseAgreementSource
        {
            get { return _licenseAgreementSource; }
            set { Set(() => LicenseAgreementSource, ref _licenseAgreementSource, value); }
        }

        public InstallState State
        {
            get { return this._state; }
            set
            {
                Set(() => State, ref this._state, value);
            }
        }

        public string InstallDir
        {
            get
            {
                if (String.IsNullOrEmpty(_installDir))
                {
                    _installDir = _model.Bootstrapper.Engine.FormatString(_model.InstallDir);
                }
                return _installDir;
            }
            set
            {
                Set(nameof(InstallDir), ref _installDir, value);
                _model.InstallDir = _installDir;
            }
        }

        public bool IsCreateDesktopShortcut
        {
            get
            {
                if (_isCreateDesktopShortcut == null)
                {
                    _isCreateDesktopShortcut = _model.IsCreateDesktopShortcut;
                }
                return _isCreateDesktopShortcut.HasValue ? _isCreateDesktopShortcut.Value : false;
            }
            set
            {
                Set(nameof(IsCreateDesktopShortcut), ref _isCreateDesktopShortcut, value);
                _model.IsCreateDesktopShortcut = value;
            }
        }

        public bool IsCreateProgramMenuShortcut
        {
            get
            {
                if (_isCreateProgramMenuShortcut == null)
                {
                    _isCreateProgramMenuShortcut = _model.IsCreateProgramMenuShortcut;
                }
                return _isCreateProgramMenuShortcut.HasValue ? _isCreateProgramMenuShortcut.Value : false;
            }
            set
            {
                Set(nameof(IsCreateProgramMenuShortcut), ref _isCreateProgramMenuShortcut, value);
                _model.IsCreateProgramMenuShortcut = value;
            }
        }
        #endregion

        #region ICommand

        public ICommand InstallCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand OpenFileDialogCommand { get; private set; }

        #endregion

        public InstallViewModel()
        {
            State = InstallState.Initializing;

            WireUpEventHandlers();
            SetLicenseAgreementText();

            InstallCommand = new RelayCommand(Install);
            CancelCommand = new RelayCommand(Cancel);
            OpenFileDialogCommand = new RelayCommand(OpenFileDialog);
        }

        #region Methods

        public void SetLicenseAgreementText()
        {
            LicenseAgreementSource = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EULA.rtf");
        }

        private void WireUpEventHandlers()
        {
            _model.Bootstrapper.PlanComplete += PlanComplete;
            _model.Bootstrapper.Error += ExecuteError;
        }

        private void RemoveEventHandlers()
        {
            _model.Bootstrapper.PlanComplete -= PlanComplete;
            _model.Bootstrapper.Error -= ExecuteError;
        }

        private void PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            RemoveEventHandlers();

            if (this.State == InstallState.Cancelled)
            {
                _model.InvokeFinalView();
                return;
            }

            Action action = delegate
            {
                _model.MainWindowViewModel.SelectedView = new ProgressView();
            };
            _model.Bootstrapper.Dispatcher.Invoke(action);

            this._model.ApplyAction();
        }

        private void ExecuteError(object sender, Microsoft.Tools.WindowsInstallerXml.Bootstrapper.ErrorEventArgs e)
        {
            _model.ProcessError(e);
        }

        #endregion

        #region ICommand Implementation
        public void Cancel(object obj)
        {
            if (System.Windows.MessageBox.Show("Are you sure you want to cancel?", "Cancel", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this._model.LogMessage("Cancelling...");
                this._model.ErrorsList.Add("Cancelled by user.");

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

        private void Install(object obj)
        {
            this._model.PlanAction(LaunchAction.Install);
        }

        private void OpenFileDialog(object ojb)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    InstallDir = folderBrowserDialog.SelectedPath;
                }
            }
        }
        #endregion
    }
}
