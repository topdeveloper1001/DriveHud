using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DriveHUD.HUD.Service
{
    public abstract class HudNamedPipeBindingService : IHudNamedPipeBindingService
    {
        #region Callback

        protected static IHudNamedPipeBindingCallbackService _callback;

        public static void RaiseReplayHand(long gameNumber, short pokerSiteId)
        {
            _callback?.ReplayHand(gameNumber, pokerSiteId);
        }

        public static void RaiseSaveHudLayout(HudLayoutContract hudLayout)
        {
            _callback?.SaveHudLayout(hudLayout);
        }

        public static void LoadLayout(int layoutId, string layoutName)
        {
            _callback?.LoadLayout(layoutId, layoutName);
        }

        public static void TagHand(long gameNumber, short pokerSiteId, int tag)
        {
            _callback?.TagHand(gameNumber, pokerSiteId, tag);
        }

        #endregion

        #region Interface

        public abstract void ConnectCallbackChannel(string name);

        public abstract void UpdateHUD(byte[] data); 

        #endregion
    }
}
