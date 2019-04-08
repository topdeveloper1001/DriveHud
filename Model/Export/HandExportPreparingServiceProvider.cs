//-----------------------------------------------------------------------
// <copyright file="HandExportPreparingServiceProvider.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;

namespace Model.Export
{
    internal class HandExportPreparingServiceProvider
    {
        public const string Common = "Common";

        public const string WPN = "WPN";

        public const string DriveHUD = "DriveHUD";

        public const string CustomIPoker = "CustomIPoker";

        public static string GetServiceName(EnumPokerSites site)
        {
            switch (site)
            {
                case EnumPokerSites.Adda52:
                case EnumPokerSites.GGN:
                case EnumPokerSites.PokerBaazi:
                case EnumPokerSites.PPPoker:
                case EnumPokerSites.PokerKing:
                case EnumPokerSites.PokerMaster:
                case EnumPokerSites.RedDragon:
                    return DriveHUD;
                case EnumPokerSites.AmericasCardroom:
                case EnumPokerSites.BlackChipPoker:
                case EnumPokerSites.TruePoker:
                case EnumPokerSites.YaPoker:
                case EnumPokerSites.WinningPokerNetwork:
                    return WPN;
                case EnumPokerSites.Ignition:
                case EnumPokerSites.Bodog:
                case EnumPokerSites.Bovada:
                case EnumPokerSites.BetOnline:
                case EnumPokerSites.SpartanPoker:
                case EnumPokerSites.SportsBetting:
                case EnumPokerSites.TigerGaming:
                    return CustomIPoker;
                default:
                    return Common;
            }
        }
    }
}