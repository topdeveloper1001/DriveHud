//-----------------------------------------------------------------------
// <copyright file="HudStoreUploadInfo.cs" company="Ace Poker Solutions">
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

namespace Model.AppStore.HudStore
{
    public class HudStoreUploadInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("game_types")]
        public GameType[] GameTypes { get; set; }

        [JsonProperty("game_variants")]
        public GameVariant[] GameVariants { get; set; }

        [JsonProperty("table_types")]
        public TableType[] TableTypes { get; set; }

        [JsonProperty("cost")]
        public decimal Cost { get; set; }

        [JsonProperty("images")]
        public HudStoreUploadImageInfo[] Images { get; set; }
    }   
}