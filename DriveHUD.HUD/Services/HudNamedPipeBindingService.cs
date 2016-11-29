﻿using DriveHUD.Application.ViewModels;
using DriveHUD.Common.Log;
using DriveHUD.HUD.Service;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.HUD.Services
{
    internal class HudNamedPipeBindingService : IHudNamedPipeBindingService
    {
        private static IHudNamedPipeBindingCallbackService _callback;

        public void ConnectCallbackChannel()
        {
            if (_callback != null)
            {
                LogProvider.Log.Info("Callback is not empty.");
            }

            try
            {
                _callback = OperationContext.Current.GetCallbackChannel<IHudNamedPipeBindingCallbackService>();
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error("Failed to get the callback channel", ex);
            }
        }

        public void UpdateHUD(byte[] data)
        {
            try
            {
                var settingsModel = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

                HudLayout hudLayout;

                using (var afterStream = new MemoryStream(data))
                {
                    hudLayout = Serializer.Deserialize<HudLayout>(afterStream);
                }

                if (settingsModel.GeneralSettings.IsAdvancedLoggingEnabled)
                {
                    LogProvider.Log.Info(this, $"Read {data.Length} bytes from DH [handle={hudLayout.WindowId}]");
                }

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    HudPainter.UpdateHud(hudLayout);
                });
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "HUD service failed to read data", e);
            }
        }
    }
}
