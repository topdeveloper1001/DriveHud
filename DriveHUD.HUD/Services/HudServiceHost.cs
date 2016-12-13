using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.HUD.Services
{
    internal class HudServiceHost : IHudServiceHost
    {
        private ServiceHost _serviceHost;
        private SettingsModel _settingsModel;

        public void Initialize()
        {
            try
            {
                _settingsModel = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

                _serviceHost = new ServiceHost(typeof(HudNamedPipeBindingServiceImpl));
                _serviceHost.Faulted += ServiceHost_Faulted;

                LogProvider.Log.Info(this, $"HUD service has been initialized");
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error("Canno start HUD service.", ex);
                ShutDown();
            }
        }

        public void OpenHost()
        {
            if (_serviceHost == null)
            {
                LogProvider.Log.Error(this, "HUD service hasn't been initialized");
                ShutDown();
            }

            try
            {
                _serviceHost.Open();
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
                ShutDown();
            }

            if (_settingsModel.GeneralSettings.IsAdvancedLoggingEnabled)
            {
                LogProvider.Log.Info($"Host is in {_serviceHost.State} state.");
            }
        }

        private void ServiceHost_Faulted(object sender, EventArgs e)
        {
            LogProvider.Log.Info("Service Faulted.");
            ShutDown();
        }

        private void ShutDown()
        {
            LogProvider.Log.Info("Shutting down the HUD");
            App.Current.Shutdown();

        }

        public void Dispose()
        {
            if (_serviceHost != null)
            {
                _serviceHost.Faulted -= ServiceHost_Faulted;
                _serviceHost.Close();
                _serviceHost = null;
            }
        }
    }
}
