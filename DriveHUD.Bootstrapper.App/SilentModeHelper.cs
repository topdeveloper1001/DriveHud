using DriveHUD.Bootstrapper.App.Models;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriveHUD.Bootstrapper.App
{
    public class SilentModeHelper
    {
        private BootstrapperAppSingletonModel _model
        {
            get { return BootstrapperAppSingletonModel.Instance; }
        }

        public LaunchAction LaunchActionType { get; set; }

        public SilentModeHelper()
        {
        }

        public void EnableEventHandling()
        {
            _model.Bootstrapper.DetectComplete += Bootstrapper_DetectComplete;
            _model.Bootstrapper.ApplyComplete += Bootstrapper_ApplyComplete;
        }

        public void CleanUp()
        {
            _model.Bootstrapper.DetectComplete -= Bootstrapper_DetectComplete;
            _model.Bootstrapper.ApplyComplete -= Bootstrapper_ApplyComplete;
        }

        private void Bootstrapper_ApplyComplete(object sender, Microsoft.Tools.WindowsInstallerXml.Bootstrapper.ApplyCompleteEventArgs e)
        {
            _model.Bootstrapper.Dispatcher.InvokeShutdown();
        }

        private void Bootstrapper_DetectComplete(object sender, Microsoft.Tools.WindowsInstallerXml.Bootstrapper.DetectCompleteEventArgs e)
        {
            _model.PlanAction(LaunchActionType);
            _model.ApplyAction();
        }
    }
}
