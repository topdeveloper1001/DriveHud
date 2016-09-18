using DriveHUD.Bootstrapper.App.Enums;
using DriveHUD.Bootstrapper.App.Infrastructure;
using DriveHUD.Bootstrapper.App.Models;
using DriveHUD.Bootstrapper.App.Views;
using GalaSoft.MvvmLight;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DriveHUD.Bootstrapper.App.ViewModels
{
    public class MaintenanceViewModel : ViewModelBase, ICancelable
    {
        #region Fields
        private BootstrapperAppSingletonModel _model
        {
            get { return BootstrapperAppSingletonModel.Instance; }
        }
        #endregion

        #region Properties
        private InstallState _state;
        private bool? _isRemovePlayerData;

        public bool IsRemovePlayerData
        {
            get
            {
                if (_isRemovePlayerData == null)
                {
                    _isRemovePlayerData = _model.IsRemovePlayerData;
                }
                return _isRemovePlayerData.HasValue ? _isRemovePlayerData.Value : false;
            }
            set
            {
                Set(nameof(IsRemovePlayerData), ref _isRemovePlayerData, value);
                _model.IsRemovePlayerData = value;
            }
        }

        public InstallState State
        {
            get { return this._state; }
            set
            {
                Set(() => State, ref this._state, value);
            }
        }

        #endregion

        #region ICommand

        public ICommand RepairCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        #endregion

        public MaintenanceViewModel()
        {
            State = InstallState.Initializing;

            WireUpEventHandlers();

            RepairCommand = new RelayCommand(Repair);
            RemoveCommand = new RelayCommand(Remove);
            CancelCommand = new RelayCommand(Cancel);
        }

        #region Methods
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

            Action action = delegate ()
            {
                _model.MainWindowViewModel.SelectedView = new ProgressView();
            };
            _model.Bootstrapper.Dispatcher.Invoke(action);

            this._model.ApplyAction();
        }

        private void ExecuteError(object sender, ErrorEventArgs e)
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

        private void Repair(object obj)
        {
            this._model.PlanAction(LaunchAction.Repair);
        }

        private void Remove(object obj)
        {
            this._model.PlanAction(LaunchAction.Uninstall);
        }

        #endregion
    }
}
