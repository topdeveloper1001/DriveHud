//-----------------------------------------------------------------------
// <copyright file="EnumPokerSitesExtensions.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Entities
{
    public static class EnumPokerSitesExtensions
    {
        public static string ToShortPokerSiteName(this EnumPokerSites site)
        {
            switch (site)
            {
                case EnumPokerSites.Adda52:
                    return "ADDA";
                case EnumPokerSites.AmericasCardroom:
                    return "ACR";
                case EnumPokerSites.BetOnline:
                    return "BOL";
                case EnumPokerSites.BlackChipPoker:
                    return "BCP";
                case EnumPokerSites.GGN:
                    return "GG";
                case EnumPokerSites.Horizon:
                    return "REV";
                case EnumPokerSites.PartyPoker:
                    return "PP";
                case EnumPokerSites.Poker888:
                    return "888";
                case EnumPokerSites.PokerBaazi:
                    return "PB";
                case EnumPokerSites.PokerKing:
                    return "PK";
                case EnumPokerSites.PokerMaster:
                    return "PM";
                case EnumPokerSites.PokerStars:
                    return "PS";
                case EnumPokerSites.PPPoker:
                    return "PPP";
                case EnumPokerSites.SpartanPoker:
                    return "TSP";
                case EnumPokerSites.SportsBetting:
                    return "SB";
                case EnumPokerSites.TigerGaming:
                    return "TG";
                case EnumPokerSites.TruePoker:
                    return "TP";
                case EnumPokerSites.Winamax:
                    return "WMX";
                case EnumPokerSites.WinningPokerNetwork:
                    return "WPN";
                case EnumPokerSites.YaPoker:
                    return "YP";
                default:
                    break;
            }

            return site.ToString();
        }

        public static string ToShortPokerSiteName(this EnumPokerSites? site)
        {
            if (!site.HasValue)
            {
                return string.Empty;
            }

            return site.Value.ToShortPokerSiteName();
        }
    }
}