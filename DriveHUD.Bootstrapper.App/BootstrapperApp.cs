using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Tools.WindowsInstallerXml;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Diagnostics;
using DriveHUD.Bootstrapper.App.ViewModels;
using DriveHUD.Bootstrapper.App.Views;
using DriveHUD.Bootstrapper.App.Models;
using System.Xml.Linq;
using System.IO;
namespace DriveHUD.Bootstrapper.App
{

    public class BootstrapperApp : BootstrapperApplication
    {
        private BootstrapperAppSingletonModel _model
        {
            get { return BootstrapperAppSingletonModel.Instance; }
        }

        private MainWindowView _mainView;
        private SilentModeHelper _silenModeHelper;

        public Dispatcher Dispatcher { get; set; }

        protected override void Run()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            _model.Bootstrapper = this;


            if (IsUninstall() && IsSilent())
            {
                _silenModeHelper = new SilentModeHelper();
                _silenModeHelper.LaunchActionType = LaunchAction.Uninstall;
                _silenModeHelper.EnableEventHandling();
            }
            else
            {
                var viewModel = new MainWindowViewModel();
                _mainView = new MainWindowView(viewModel);

                _model.SetMainViewModel(viewModel);
                _model.SetWindowHandle(_mainView);
            }

            this.Engine.Detect();
            _mainView?.Show(); 
            Dispatcher.Run();
            _silenModeHelper?.CleanUp();
            this.Engine.Quit(_model.FinalResult);
        }

        private bool IsUninstall()
        {
            return this.Command.Action == LaunchAction.Uninstall;
        }

        private bool IsSilent()
        {
            return this.Command.Display == Display.Embedded;
        }
    }
}
