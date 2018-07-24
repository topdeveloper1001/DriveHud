//-----------------------------------------------------------------------
// <copyright file="HudStoreItem.cs" company="Ace Poker Solutions">
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
using Prism.Mvvm;

namespace Model.AppStore.HudStore.Model
{
    public class HudStoreItem : BindableBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("game_types")]
        public GameType[] GameTypes { get; set; }

        [JsonProperty("game_variants")]
        public GameVariant[] GameVariants { get; set; }

        [JsonProperty("table_types")]
        public TableType[] TableTypes { get; set; }

        [JsonProperty("is_newest")]
        public bool IsNewest { get; set; }

        [JsonProperty("images")]
        public HudStoreImageItem[] Images { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }
    }

    public class HudStoreImageItem
    {
        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }
}