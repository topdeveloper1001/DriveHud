//-----------------------------------------------------------------------
// <copyright file="PokerBaaziUserActionResponse.cs" company="Ace Poker Solutions">
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
    internal class PokerBaaziUserActionResponse
    {
        [JsonProperty("actionPerformed")]
        public string Action { get; set; }

        [JsonProperty("currentAmount", NullValueHandling = NullValueHandling.Ignore)]
        public decimal Amount { get; set; }

        [JsonProperty("betAmount", NullValueHandling = NullValueHandling.Ignore)]
        public decimal BetAmount { get; set; }

        [JsonProperty("potAmount", NullValueHandling = NullValueHandling.Ignore)]
        public decimal PotAmount { get; set; }

        [JsonProperty("tempBetBalance", NullValueHandling = NullValueHandling.Ignore)]
        public decimal RemainingStack { get; set; }

        [JsonProperty("userId")]
        public int PlayerId { get; set; }

        [JsonProperty("userName")]
        public string PlayerName { get; set; }

        [JsonProperty("playGroupId")]
        public long HandId { get; set; }

        [JsonProperty("roomId")]
        public uint RoomId { get; set; }
    }
}