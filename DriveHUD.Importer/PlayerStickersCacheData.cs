//-----------------------------------------------------------------------
// <copyright file="PlayerStickersCacheData.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model;
using System;
using System.Collections.Generic;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Represents cached data of stickers for player
    /// </summary>
    public class PlayerStickersCacheData
    {
        /// <summary>
        /// Gets or sets session
        /// </summary>
        public string Session { get; set; }

        /// <summary>
        /// Gets or sets player
        /// </summary>
        public PlayerCollectionItem Player { get; set; }

        /// <summary>
        /// Gets or sets layout name
        /// </summary>
        public string Layout { get; set; }

        /// <summary>
        /// Gets or sets sticker filters
        /// </summary>
        public IDictionary<string, Func<Playerstatistic, bool>> StickerFilters { get; set; }

        /// <summary>
        /// Gets or sets statistic of player
        /// </summary>
        public Playerstatistic Statistic { get; set; }

        /// <summary>
        /// Gets or sets whenever player is hero
        /// </summary>
        public bool IsHero { get; set; }

        /// <summary>
        /// Checks if <see cref="PlayerStickersCacheData"/> is valid
        /// </summary>
        /// <returns>True if data is valid, otherwise - false</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Session) && Player != null && !string.IsNullOrEmpty(Layout) &&
                StickerFilters != null && StickerFilters.Count > 0 && Statistic != null;
        }
    }
}