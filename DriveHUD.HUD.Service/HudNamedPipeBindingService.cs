using DriveHUD.Entities;

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

        public static void LoadLayout(string layoutName, EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType)
        {
            _callback?.LoadLayout(layoutName, pokerSite, gameType, tableType);
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
