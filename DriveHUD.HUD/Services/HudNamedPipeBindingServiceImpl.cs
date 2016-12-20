using DriveHUD.Application.ViewModels;
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
    public class HudNamedPipeBindingServiceImpl : HudNamedPipeBindingService
    {
        public override void ConnectCallbackChannel(string name)
        {
            if (_callback != null)
            {
                LogProvider.Log.Info("Callback is not empty.");
            }

            try
            {
                _callback = OperationContext.Current.GetCallbackChannel<IHudNamedPipeBindingCallbackService>();
                LogProvider.Log.Info($"Registered a callback channel for {name}");
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error("Failed to get the callback channel", ex);
            }
        }

        public override void UpdateHUD(byte[] data)
        {
            try
            {
                HudLayout hudLayout;

                using (var afterStream = new MemoryStream(data))
                {
                    hudLayout = Serializer.Deserialize<HudLayout>(afterStream);
                }

                LogProvider.Log.Debug(this, $"Read {data.Length} bytes from DH [handle={hudLayout.WindowId}]");

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
