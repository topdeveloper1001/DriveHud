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
        /// <param name="playerName">Player which stats has to be retrieved</param>
        /// <returns>Player stats</returns>
        IList<Playerstatistic> GetPlayerStats(string session, string playerName);

        /// <summary>
        /// Get all player stats from cache in specified session
        /// </summary>
        /// <param name="session">Active session</param>
        /// <returns>Player stats</returns>
        IList<Playerstatistic> GetAllPlayerStats(string session);

        /// <summary>
        /// Store specified player data in cache
        /// </summary>
        /// <param name="session">Active session</param>
        /// <param name="playerName">Player which stats has to be saved</param>
        /// <param name="stats">Player stats</param>
        void AddOrUpdatePlayerStats(string session, string playerName, Playerstatistic stats, bool isHero);          

        /// <summary>
        /// Store hand history record in cache
        /// </summary>
        /// <param name="session">Active session</param>
        /// <param name="record">Record to be saved</param>
        void AddRecord(string session, HandHistoryRecord record);

        /// <summary>
        /// Get all records from cache of specified session
        /// </summary>
        /// <param name="session">Session</param>
        /// <returns>List of records from cache of specified session</returns>
        IList<HandHistoryRecord> GetRecords(string session);

        /// <summary>
        /// Get all records from cache
        /// </summary>
        /// <param name="session">Session</param>
        /// <returns>List of records from cache</returns>
        IList<HandHistoryRecord> GetAllRecords();
    }
}