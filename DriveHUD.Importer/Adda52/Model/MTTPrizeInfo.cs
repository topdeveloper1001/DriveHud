//-----------------------------------------------------------------------
// <copyright file="MTTPrizeInfo.cs" company="Ace Poker Solutions">
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
    internal sealed class MTTPrizeInfo
    {
        [JsonProperty("chipType")]
        public string ChipType { get; set; }

        [JsonProperty("gameDetail")]
        public string GameDetail { get; set; }

        public bool IsReal
        {
            get
            {
                return !string.IsNullOrEmpty(ChipType) && ChipType.Equals("real", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}