using DriveHUD.Bootstrapper.App.Infrastructure;
using DriveHUD.Bootstrapper.App.Models;
using GalaSoft.MvvmLight;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DriveHUD.Bootstrapper.App.ViewModels
{
    public class FinalView : ViewModelBase, ICancelable
    {
        #region Fields
        private BootstrapperAppSingletonModel _model
        {
            get { return BootstrapperAppSingletonModel.Instance; }
        }
        #endregion

        public FinalView()
        {
            CloseCommand = new RelayCommand(Cancel);
            LaunchCommand = new RelayCommand(Launch);

            UpdateActionText();
        }

        #region Properties
        private string _currentAction;
        private string _description;

        public List<String> ErrorsList
        {
            get { return _model.ErrorsList; }
        }

        public bool IsError
        {
            get { return ErrorsList.Any(); }
        }

        public bool IsCanLaunch
        {
            get { return !IsError && _model.LastPlannedAction == LaunchAction.Install; }
        }

        public string Description
        {
            get { return _description; }
            set { Set(() => Description, ref _description, value); }
        }

        public string CurrentAction
        {
            get { return _currentAction; }
            set { Set(() => CurrentAction, ref _currentAction, value); }
        }

        public ICommand CloseCommand { get; private set; }
        public ICommand LaunchCommand { get; private set; }
        #endregion

        #region Methods

        private void UpdateActionText()
        {
            Description = IsError ? "One or more issues caused the setup to fail. Please fix the issues and then retry setup." : "Thank you for using DriveHUD!";
            switch (_model.LastPlannedAction)
            {
                case LaunchAction.Uninstall:
                    CurrentAction = IsError ? "Uninstall failed" : "Uninstall completed";
                    break;
                case LaunchAction.Install:
                    CurrentAction = IsError ? "Install failed" : "Install completed";
                    break;
                case LaunchAction.Repair:
                    CurrentAction = IsError ? "Repair failed" : "Repair completed";
                    break;
                default:
                    CurrentAction = "Failed";
                    break;
            }
        }

        public void Cancel(object obj)
        {
            _model.Bootstrapper.Dispatcher.InvokeShutdown();
        }

        public void Launch(object obj)
        {
            Cancel(null);

            var root = _model.InstallDir.Replace("\"", "");
            var relativeExecutablePath = _model.RelativeExecutablePath.Replace("\"", "");
            var executable = _model.ExecutableName.Replace("\"", "");

            var fullpath = Path.Combine(root, relativeExecutablePath, executable);
            bool isFailedToLaunch = false;
            if(File.Exists(fullpath))
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WorkingDirectory = Path.GetDirectoryName(fullpath);
                    startInfo.FileName = executable;
                    startInfo.UseShellExecute = true;

                    Process.Start(startInfo);
                }
                catch(Exception ex)
                {
                    _model.LogMessage(ex.Message);
                    isFailedToLaunch = true;
                }
            }
            else
            {
                isFailedToLaunch = true;
            }

            if(isFailedToLaunch)
            {
                MessageBox.Show("Failed to launch the application", "DriveHUD", MessageBoxButton.OK);
            }
        }
        #endregion
    }
}
