//-----------------------------------------------------------------------
// <copyright file="ZoneTableInfo.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.Bovada.DataObjects
{
    public class ZoneTableInfo
    {
        public uint gameId { get; set; }

        public string gameName { get; set; }

        public string playMode { get; set; }

        public string gameType { get; set; }

        public string limit { get; set; }

        public string buyin { get; set; }

        public int seats { get; set; }

        public int players { get; set; }

        public int minStake { get; set; }

        public int maxStake { get; set; }

        public int avgPot { get; set; }

        public bool isSeated { get; set; }

        public int playersPerFlopPct { get; set; }
    }
}