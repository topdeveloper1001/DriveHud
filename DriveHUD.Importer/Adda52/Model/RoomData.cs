//-----------------------------------------------------------------------
// <copyright file="RoomData.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Newtonsoft.Json;
using System;

namespace DriveHUD.Importers.Adda52.Model
{
    internal sealed class RoomData
    {
        public const string Command = "game.roomdata";

        [JsonProperty("chipType")]
        public string ChipType { get; set; }

        [JsonProperty("gameType")]
        public string GameType { get; set; }

        [JsonProperty("isAnonynounsTable")]
        public bool IsAnonymousTable { get; set; }

        [JsonProperty("bettingRule")]
        public string BettingRule { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("roomName")]
        public string RoomName { get; set; }

        [JsonProperty("ringVariant")]
        public string RingVariant { get; set; }

        [JsonProperty("players")]
        public int MaxPlayers { get; set; }

        [JsonProperty("bigBlind")]
        public int BigBlind { get; set; }

        [JsonProperty("smallBlind")]
        public int SmallBlind { get; set; }

        [JsonProperty("buyinFees")]
        public string BuyinFees { get; set; }

        [JsonProperty("turnTime")]
        public int TurnTime { get; set; }

        [JsonProperty("buyInHigh")]
        public int BuyinHigh { get; set; }

        public bool IsFreeroll
        {
            get
            {
                return !string.IsNullOrEmpty(ChipType) && ChipType.Equals("Freeroll", StringComparison.OrdinalIgnoreCase);
            }
        }    
    }
}