﻿//-----------------------------------------------------------------------
// <copyright file="MTTDetailedInfo.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Importers.Adda52.Model
{
    internal sealed class MTTDetailedInfo
    {
        [JsonProperty("mtst")]
        public string StartDateText { get; set; }

        [JsonProperty("bountyAmount")]
        public int BountyAmount { get; set; }

        [JsonProperty("comissionRate")]
        public decimal ComissionRate { get; set; }

        [JsonProperty("entryChipInfo")]
        public EntryChipInfo EntryChipInfo { get; set; }
    }
}