﻿//-----------------------------------------------------------------------
// <copyright file="HudServiceHost.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.ServiceModel;

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
                LogProvider.Log.Error("Could not start HUD service.", ex);
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
