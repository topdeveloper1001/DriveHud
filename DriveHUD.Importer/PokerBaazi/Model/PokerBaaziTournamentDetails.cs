//-----------------------------------------------------------------------
// <copyright file="PokerBaaziTournamentDetails.cs" company="Ace Poker Solutions">
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
    internal class PokerBaaziTournamentDetails
    {
        [JsonProperty("tournmentName")]
        public string TournamentName { get; set; }

        [JsonProperty("tournmentEntryFee")]
        public int EntryFee { get; set; }

        [JsonProperty("tournmentBuyIn")]
        public int BuyIn { get; set; }

        [JsonProperty("startingStake")]
        public decimal StartingStake { get; set; }

        [JsonProperty("tournmentRegPlayerCount")]
        public int TotalPlayers { get; set; }

        [JsonProperty("tournmentMaxEntries")]
        public int MaxEntries { get; set; }
    }
}