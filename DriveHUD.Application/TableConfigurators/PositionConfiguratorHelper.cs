using DriveHUD.Entities;

namespace DriveHUD.Application.TableConfigurators
{
    internal static class PositionConfiguratorHelper
    {
        public static string GetServiceName(EnumPokerSites pokerSite, HudViewType hudViewType)
        {
            var hudType = hudViewType == HudViewType.Plain ? "Plain" : "Rich";
            var ps = "Common";
            if (hudViewType != HudViewType.Plain)
                return $"{ps}_{hudType}";
            switch (pokerSite)
            {
                case EnumPokerSites.Ignition:
                case EnumPokerSites.PokerStars:
                case EnumPokerSites.Poker888:
                case EnumPokerSites.WinningPokerNetwork:
                case EnumPokerSites.AmericasCardroom:
                case EnumPokerSites.BlackChipPoker:
                case EnumPokerSites.Bodog:
                case EnumPokerSites.PartyPoker:                        
                    ps = pokerSite.ToString();
                    break;
            }
            return $"{ps}_{hudType}";
        }
    }
}