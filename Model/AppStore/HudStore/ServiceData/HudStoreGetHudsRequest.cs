﻿//-----------------------------------------------------------------------
// <copyright file="HudStoreGetHudsRequest.cs" company="Ace Poker Solutions">
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

namespace Model.AppStore.HudStore.ServiceData
{
    public class HudStoreGetHudsRequest
    {
        [JsonProperty("serial")]
        public string Serial { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("filter")]
        public int? Filter { get; set; }

        [JsonProperty("sorting")]
        public int? Sorting { get; set; }

        [JsonProperty("search")]
        public string Search { get; set; }
    }
}