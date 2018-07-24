//-----------------------------------------------------------------------
// <copyright file="HudStoreGetHudsResponse.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.AppStore.HudStore.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Model.AppStore.HudStore.ServiceResponses
{
    public class HudStoreHudsData
    {
        [JsonProperty("items")]
        public IEnumerable<HudStoreItem> Items { get; set; }

        [JsonProperty("items_count")]
        public int ItemsCount { get; set; }
    }
}