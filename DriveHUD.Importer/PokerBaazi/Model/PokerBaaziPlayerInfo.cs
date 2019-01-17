//-----------------------------------------------------------------------
// <copyright file="PokerBaaziPlayerInfo.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Newtonsoft.Json;

namespace DriveHUD.Importers.PokerBaazi.Model
{
    internal class PokerBaaziPlayerInfo
    {
        [JsonProperty("d")]
        public bool IsDealer { get; set; }

        [JsonProperty("userId")]
        public uint PlayerId { get; set; }

        [JsonProperty("userName")]
        public string PlayerName { get; set; }

        [JsonProperty("sb")]
        public bool IsSmallBlind { get; set; }

        [JsonProperty("bb")]
        public bool IsBigBlind { get; set; }

        [JsonProperty("tempBetBalance")]
        public int Chips { get; set; }

        [JsonProperty("playersCards")]
        public string Cards { get; set; }

        [JsonProperty("tableIndex")]
        public int Seat { get; set; }

        [JsonProperty("betAmount", NullValueHandling = NullValueHandling.Ignore)]
        public int BetAmount { get; set; }
    }
}