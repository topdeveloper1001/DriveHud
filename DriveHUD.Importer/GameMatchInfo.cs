//-----------------------------------------------------------------------
// <copyright file="GameMatchInfo.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.GameDescription;

namespace DriveHUD.Importers
{
    public class GameMatchInfo
    {
        public GameType GameType { get; set; }

        /// <summary>
        /// Tournament buyin in $ (or other currency)
        /// </summary>
        public decimal TournamentBuyIn { get; set; }

        /// <summary>
        /// Max table buyin in NL (100NL = 1$)
        /// </summary>
        public int CashBuyIn { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// Currency of tournament
        /// </summary>
        public Currency TournamentCurrency { get; set; }
    }
}