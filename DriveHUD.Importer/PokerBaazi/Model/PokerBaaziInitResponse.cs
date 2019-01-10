//-----------------------------------------------------------------------
// <copyright file="PokerBaaziInitResponse.cs" company="Ace Poker Solutions">
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
    internal class PokerBaaziInitResponse
    {
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("tournamentName")]
        public string TournamentName { get; set; }

        [JsonProperty("maxPlayers")]
        public int MaxPlayers { get; set; }

        [JsonProperty("roomId")]
        public uint RoomId { get; set; }

        [JsonProperty("smallBlind")]
        public int SmallBlind { get; set; }

        [JsonProperty("bigBlind")]
        public int BigBlind { get; set; }

        [JsonProperty("userId")]
        public uint UserId { get; set; }

        [JsonProperty("tournamentId")]
        public uint TournamentId { get; set; }

        [JsonProperty("straddle")]
        public bool Straddle { get; set; }
    }
}