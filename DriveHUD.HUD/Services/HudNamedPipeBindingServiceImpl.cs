using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Log;
using DriveHUD.Common.WinApi;
using DriveHUD.HUD.Service;
using ProtoBuf;
using System;
using System.IO;
using System.ServiceModel;

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

                var windowTitle = WinApi.GetWindowText(new IntPtr(hudLayout.WindowId));
                LogProvider.Log.Debug($"Sent data to '{windowTitle}'[{hudLayout.WindowId}]: s={hudLayout.PokerSite};mp={hudLayout.TableType};gt={hudLayout.GameType};gn={hudLayout.GameNumber};l='{hudLayout.LayoutName}' [{data.Length} bytes]");

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
