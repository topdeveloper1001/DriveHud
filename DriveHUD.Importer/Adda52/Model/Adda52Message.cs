﻿//-----------------------------------------------------------------------
// <copyright file="Adda52Message.cs" company="Ace Poker Solutions">
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
    internal class Adda52Message<T> where T : class
    {
        [JsonProperty("c")]
        public int C { get; set; }

        [JsonProperty("p")]
        public Adda52Command<T> Body { get; set; }

        [JsonProperty("a")]
        public int A { get; set; }        
    }
}