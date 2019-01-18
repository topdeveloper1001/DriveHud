//-----------------------------------------------------------------------
// <copyright file="PokerBaaziSpectatorResponse.cs" company="Ace Poker Solutions">
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
using System.Collections.Generic;

namespace DriveHUD.Importers.PokerBaazi.Model
{
    internal class PokerBaaziStartGameResponse
    {
        [JsonProperty("roomId")]
        public uint RoomId { get; set; }

        [JsonProperty("playGroupId")]
        public long HandId { get; set; }

        [JsonProperty("totalAnteAmount", NullValueHandling = NullValueHandling.Ignore)]
        public decimal TotalAnte { get; set; }

        [JsonProperty("smallBlind", NullValueHandling = NullValueHandling.Ignore)]
        public decimal SmallBlind { get; set; }

        [JsonProperty("bigBlind", NullValueHandling = NullValueHandling.Ignore)]
        public decimal BigBlind { get; set; }

        [JsonProperty("ante", NullValueHandling = NullValueHandling.Ignore)]
        public decimal Ante { get; set; }

        [JsonProperty("tournamentId", NullValueHandling = NullValueHandling.Ignore)]
        public long TournamentId { get; set; }

        [JsonProperty("details")]
        public Dictionary<int, PokerBaaziPlayerInfo> Players { get; set; }
    }
}