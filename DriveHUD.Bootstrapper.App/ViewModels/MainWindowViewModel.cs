using DriveHUD.Bootstrapper.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using DriveHUD.Bootstrapper.App.Views;
using System.Windows;
using GalaSoft.MvvmLight;
using DriveHUD.Bootstrapper.App.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DriveHUD.Bootstrapper.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private BootstrapperAppSingletonModel _model
        {
            get { return BootstrapperAppSingletonModel.Instance; }
        }
        private bool _isUpdating = false;

        private FrameworkElement _selectedView;
        private PackageState _currentPackageState;

        public PackageState CurrentPackageState
        {
            get { return _currentPackageState; }
            set { _currentPackageState = value; }
        }

        public FrameworkElement SelectedView
        {
            get { return _selectedView; }
            set
            {
                Set(() => SelectedView, ref _selectedView, value);
            }
        }

        public MainWindowViewModel()
        {
            CurrentPackageState = PackageState.Unknown;

            this.WireUpEventHandlers();
        }

        private void EvaluateConditions(Collection<BundleCondition> conditions)
        {
            foreach(var condition in conditions)
            {
                if(!_model.Bootstrapper.Engine.EvaluateCondition(condition.Condition))
                {
                    _model.LogMessage(string.Format("Condition '{0}' evaluated to false. Condition Message: {1}", condition.Condition, condition.Message));
                    _model.ErrorsList.Add(condition.Message);
                }
            }
        }

        #region Events

        private void WireUpEventHandlers()
        {
            _model.Bootstrapper.DetectBegin += DetectBegin;
            _model.Bootstrapper.DetectComplete += DetectComplete;
            _model.Bootstrapper.DetectPackageComplete += DetectPackageComplete;
            _model.Bootstrapper.DetectRelatedMsiPackage += DetectRelatedMsiPackage;
            _model.Bootstrapper.DetectRelatedBundle += DetectRelatedBundle;

            _model.Bootstrapper.Error += ExecuteError;
        }

        private void RemoveEventHandlers()
        {
            _model.Bootstrapper.DetectBegin -= DetectBegin;
            _model.Bootstrapper.DetectComplete -= DetectComplete;
            _model.Bootstrapper.DetectPackageComplete -= DetectPackageComplete;
            _model.Bootstrapper.DetectRelatedMsiPackage -= DetectRelatedMsiPackage;
            _model.Bootstrapper.DetectRelatedBundle -= DetectRelatedBundle;

            _model.Bootstrapper.Error -= ExecuteError;
        }

        private void ExecuteError(object sender, ErrorEventArgs e)
        {
            _model.ProcessError(e);
        }


        private void DetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            if(e.PackageId == "DriveHUD")
            {
                if(e.State == PackageState.Superseded)
                {
                    _model.ErrorsList.Add("The version of the software on your computer is newer than the version provided with this installer.");
                }
                this.CurrentPackageState = e.State == PackageState.Present ? PackageState.Present : PackageState.Absent;
            }
        }

        private void DetectRelatedMsiPackage(object sender, DetectRelatedMsiPackageEventArgs e)
        {
            switch (e.Operation)
            {
                case RelatedOperation.None:
                    break;
                case RelatedOperation.Downgrade:
                    break;
                case RelatedOperation.MinorUpdate:
                case RelatedOperation.MajorUpgrade:
                    _isUpdating = true;
                    break;
                case RelatedOperation.Remove:
                    break;
                case RelatedOperation.Install:
                    break;
                case RelatedOperation.Repair:
                    break;
                default:
                    break;
            }
        }

        private void DetectRelatedBundle(object sender, DetectRelatedBundleEventArgs e)
        {
        }

        private void DetectComplete(object sender, DetectCompleteEventArgs e)
        {
            RemoveEventHandlers();
            EvaluateConditions(_model.BundleInfo.Conditions);

            /* Display final view in case of error/failed condition */
            if(_model.ErrorsList.Any())
            {
                _model.InvokeFinalView();
                return;
            }

            if ((CurrentPackageState == PackageState.Absent) || _isUpdating)
            {
                _model.Bootstrapper.Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        SelectedView = new InstallView();
                    }));
            }
            else
            {
                _model.Bootstrapper.Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        SelectedView = new MaintenanceView();
                    }));
            }
        }

        private void DetectBegin(object sender, DetectBeginEventArgs e)
        {
            Debug.WriteLine("DetectBegin -> installed = " + e.Installed.ToString());
        } 
        #endregion
    }
}
