//-----------------------------------------------------------------------
// <copyright file="TournamentSettings.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model.Enums;
using System.Collections.Generic;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Default data for tournament processing (probably need to make it more complicated, for other poker clients)
    /// </summary>
    internal static class TournamentSettings
    {
        #region Results Dictionaries

        private static readonly Dictionary<int, decimal[]> BovadaSnGWinningsMultiplierDictionary = new Dictionary<int, decimal[]>()
        {
            { 2, new decimal[] { 1m } },
            { 6, new decimal[] { 0.65m, 0.35m } },
            { 9, new decimal[] { 0.5m, 0.3m, 0.2m } }
        };

        private static readonly Dictionary<int, decimal[]> BovadaBeginnerSnGWinningsMultiplierDictionary = new Dictionary<int, decimal[]>()
        {
            { 6, new decimal[] { 0.5m, 0.3m, 0.2m} }
        };

        private static readonly Dictionary<int, decimal[]> BetOnlineSnGWinningsMultiplierDictionary = new Dictionary<int, decimal[]>()
        {
            { 2, new decimal[] { 1m } },
            { 6, new decimal[] { 0.7m, 0.3m } },
            { 8, new decimal[] { 0.25m, 0.25m, 0.25m, 0.25m } },
            { 10, new decimal[] { 0.5m, 0.3m, 0.2m } },
        };

        private static readonly Dictionary<int, decimal[]> WinningPokerNetworkSnGWinningsMultiplierDictionary = new Dictionary<int, decimal[]>()
        {
            { 2, new decimal[] { 1m } },
            { 6, new decimal[] { 0.65m, 0.35m } },
            { 9, new decimal[] { 0.5m, 0.3m, 0.2m } }
        };

        #endregion

        public const int MaximumPlayersPerTable = 10;

        public const int DefaultInitialStackSize = 1500;

        /// <summary>
        /// Get prize pool multiplier for tournament
        /// </summary>
        /// <returns>Dictionary where key is number of players in SnG and value is array of ordered prize rates starting from the 1st place</returns>
        public static Dictionary<int, decimal[]> GetWinningsMultiplier(EnumPokerSites pokerSite, bool isBeginner = false)
        {
            switch (pokerSite)
            {
                case EnumPokerSites.Unknown:
                    break;
                case EnumPokerSites.Ignition:
                case EnumPokerSites.Bovada:
                case EnumPokerSites.IPoker:
                    return isBeginner ? BovadaBeginnerSnGWinningsMultiplierDictionary : BovadaSnGWinningsMultiplierDictionary;
                case EnumPokerSites.Bodog:
                case EnumPokerSites.BetOnline:
                    return BetOnlineSnGWinningsMultiplierDictionary;
                case EnumPokerSites.WinningPokerNetwork:
                case EnumPokerSites.AmericasCardroom:
                case EnumPokerSites.BlackChipPoker:
                case EnumPokerSites.TruePoker:
                case EnumPokerSites.YaPoker:
                    return WinningPokerNetworkSnGWinningsMultiplierDictionary;
                case EnumPokerSites.SportsBetting:
                default:
                    break;
            }

            return null;
        }
    }
}