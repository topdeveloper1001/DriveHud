//-----------------------------------------------------------------------
// <copyright file="PlayerStatsSessionCacheInfo.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Importers
{
    /// <summary>
    /// Provides properties for adding player statistic to <see cref="IImporterSessionCacheService"/> 
    /// </summary>
    public class PlayerStatsSessionCacheInfo
    {
        /// <summary>
        /// Gets or sets session 
        /// </summary>
        public string Session { get; set; }

        /// <summary>
        /// Gets or sets <see cref="PlayerCollectionItem"/> 
        /// </summary>
        public PlayerCollectionItem Player { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Playerstatistic"/>
        /// </summary>
        public Playerstatistic Stats { get; set; }

        /// <summary>
        /// Gets or sets whenever statistic belongs to hero
        /// </summary>
        public bool IsHero { get; set; }
    
        /// <summary>
        /// Gets or sets the game format
        /// </summary>
        public GameFormat GameFormat { get; set; }

        /// <summary>
        /// Gets or sets the game number
        /// </summary>
        public long GameNumber { get; set; }
    }
}