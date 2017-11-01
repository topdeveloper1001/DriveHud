//-----------------------------------------------------------------------
// <copyright file="HandHistory.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DriveHUD.Importers.GGNetwork.Model
{
    internal class HandHistory
    {
        public string Version { get; set; }

        public HandIdInfo HandIdInfo { get; set; }

        public string TableId { get; set; }

        public NameInfo NameInfo { get; set; }

        public string TourneyId { get; set; }

        public string TourneyBrandName { get; set; } 

        public int TourneyEventNumber { get; set; }

        public int BountyType { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsInTheMoney { get; set; }

        public bool IsFinalTable { get; set; }

        public int TableType { get; set; }

        public int GameType { get; set; }

        public int LimitType { get; set; }

        public int BuyInType { get; set; }

        public IList<Player> Players { get; set; }

        public int MaxPlayer { get; set; }

        public HandInformation HandInformation { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int TourneyBuyIn { get; set; }

        public int DefaultBuyIn { get; set; }

        public int MaxBuyIn { get; set; }

        public int MinBuyIn { get; set; }

        public int Ante { get; set; }

        public int BigBlind { get; set; }

        public int SmallBlind { get; set; }

        public int MinChipUnit { get; set; }

        public Summary Summary { get; set; }

        public IList<Pot> Pots { get; set; }

        public int MegaSpinMultiplier { get; set; }

        public int FortuneSpinMultiplier { get; set; }
    }
}