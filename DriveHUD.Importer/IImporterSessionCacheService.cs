//-----------------------------------------------------------------------
// <copyright file="IImporterSessionCacheService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using Model.Data;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Interface of session cache service
    /// </summary>
    public interface IImporterSessionCacheService
    {
        /// <summary>
        /// Starts new session
        /// </summary>
        void Begin();

        /// <summary>
        /// Stops the current session
        /// </summary>
        void End();

        /// <summary>
        /// Get player stats from cache
        /// </summary>
        /// <param name="session">Active session</param>
        /// <param name="player">Player which stats has to be retrieved</param>
        /// <returns>Player stats</returns>
        SessionCacheStatistic GetPlayerStats(string session, PlayerCollectionItem player);

        /// <summary>
        /// Stores specified player data in cache
        /// </summary>
        /// <param name="cacheInfo"><see cref="PlayerStatsSessionCacheInfo"/> to store in cache</param>    
        void AddOrUpdatePlayersStats(IEnumerable<PlayerStatsSessionCacheInfo> cacheInfos, string session, ISessionStatisticFilter filter);

        /// <summary>
        /// Gets statistic of the player's last hand
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="player">Player name</param>
        /// <returns><see cref="Playerstatistic"/> of the last hand in session</returns>
        Playerstatistic GetPlayersLastHandStatistics(string session, PlayerCollectionItem player);

        /// <summary>
        /// Store stickers-related statistics in cache
        /// </summary>
        /// <param name="playerStickersCacheData">Cache data object</param>     
        void AddOrUpdatePlayerStickerStats(IEnumerable<PlayerStickersCacheData> playersStickersCacheData, string session);

        /// <summary>
        /// Gets collection of player statistics that are used for Bumper Stickers calculations
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="player">Player name</param>
        /// <returns>Dictionary of sticker-related statistics accessed by sticker name</returns>
        Dictionary<string, Playerstatistic> GetPlayersStickersStatistics(string session, PlayerCollectionItem player);
    }
}