﻿//-----------------------------------------------------------------------
// <copyright file="ImporterSessionCacheService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DriveHUD.Entities;
using System.Collections.Concurrent;
using System.Threading;
using Model;
using System.Linq;
using DriveHUD.Common;
using Microsoft.Practices.ServiceLocation;
using Model.Interfaces;
using DriveHUD.Common.Linq;
using Model.Data;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Importer cache service which stores session data
    /// </summary>
    internal class ImporterSessionCacheService : IImporterSessionCacheService
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        private ReaderWriterLockSlim globalCacheLock = new ReaderWriterLockSlim();

        private Dictionary<string, SessionCacheData> cachedData;

        private readonly IDataService dataService;

        private bool isStarted;

        public ImporterSessionCacheService()
        {
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
        }

        /// <summary>
        /// Starts new session
        /// </summary>
        public void Begin()
        {
            cachedData = new Dictionary<string, SessionCacheData>();
            isStarted = true;
        }

        /// <summary>
        /// Stops the current session
        /// </summary>
        public void End()
        {
            cachedData?.Clear();
            isStarted = false;
        }

        /// <summary>
        /// Get player stats
        /// </summary>
        /// <param name="session">Active session</param>
        /// <param name="player">Specified player</param>
        /// <returns>Player stats</returns>
        public SessionCacheStatistic GetPlayerStats(string session, PlayerCollectionItem player)
        {
            if (!isStarted)
            {
                return new SessionCacheStatistic();
            }

            cacheLock.EnterReadLock();

            try
            {
                if (cachedData.ContainsKey(session) && cachedData[session].StatisticByPlayer.ContainsKey(player))
                {
                    return cachedData[session].StatisticByPlayer[player];
                }

                return new SessionCacheStatistic();
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Add specified player data or update it if data already exists
        /// </summary>
        /// <param name="session">Active session</param>
        /// <param name="player">Specified player</param>
        /// <param name="stats">Player stats</param>
        public void AddOrUpdatePlayerStats(string session, PlayerCollectionItem player, Playerstatistic stats, bool isHero)
        {
            if (!isStarted || string.IsNullOrEmpty(session))
            {
                return;
            }

            cacheLock.EnterWriteLock();

            try
            {
                var sessionData = GetOrAddSessionData(session);

                SessionCacheStatistic sessionCacheStatistic = null;

                bool skipAdding = false;

                if (sessionData.StatisticByPlayer.ContainsKey(player))
                {
                    sessionCacheStatistic = sessionData.StatisticByPlayer[player];
                }
                else
                {
                    // if not hero read data from storage
                    if (isHero)
                    {
                        sessionCacheStatistic = new SessionCacheStatistic();
                    }
                    else
                    {
                        var playerStatistic = dataService.GetPlayerStatisticFromFile(player.PlayerId, (short)player.PokerSite);

                        sessionCacheStatistic = new SessionCacheStatistic();

                        playerStatistic.ForEach(x =>
                        {
                            // we don't have this data in the db so update it after loading from file
                            PlayerStatisticCalculator.CalculatePositionalData(x);

                            var isCurrentGame = x.GameNumber == stats.GameNumber;

                            if (isCurrentGame)
                            {
                                PlayerStatisticCalculator.CalculateTotalPotValues(x);
                                x.SessionCode = session;
                            }

                            sessionCacheStatistic.PlayerData.AddStatistic(x);

                            if (isCurrentGame)
                            {
                                sessionCacheStatistic.SessionPlayerData.AddStatistic(x);

                                InitSessionCardsCollections(sessionCacheStatistic.PlayerData, x);
                                InitSessionCardsCollections(sessionCacheStatistic.SessionPlayerData, x);
                                InitSessionStatCollections(sessionCacheStatistic.PlayerData, x);
                                InitSessionStatCollections(sessionCacheStatistic.SessionPlayerData, x);
                            }
                        });

                        skipAdding = true;
                    }

                    sessionData.StatisticByPlayer.Add(player, sessionCacheStatistic);
                }

                if (sessionData.LastHandStatisticByPlayer.ContainsKey(player))
                {
                    sessionData.LastHandStatisticByPlayer[player] = stats.Copy();
                }
                else
                {
                    sessionData.LastHandStatisticByPlayer.Add(player, stats.Copy());
                }

                if (skipAdding)
                {
                    return;
                }

                sessionCacheStatistic.PlayerData.AddStatistic(stats);
                InitSessionCardsCollections(sessionCacheStatistic.PlayerData, stats);
                InitSessionStatCollections(sessionCacheStatistic.PlayerData, stats);

                if (stats.SessionCode == session)
                {
                    sessionCacheStatistic.SessionPlayerData.AddStatistic(stats);
                    InitSessionCardsCollections(sessionCacheStatistic.SessionPlayerData, stats);
                    InitSessionStatCollections(sessionCacheStatistic.SessionPlayerData, stats);
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Store hand history record in cache
        /// </summary>
        /// <param name="session">Active session</param>
        /// <param name="record">Record to be saved</param>
        public void AddRecord(string session, HandHistoryRecord record)
        {
            if (!isStarted)
            {
                return;
            }

            cacheLock.EnterWriteLock();

            try
            {
                var sessionData = GetOrAddSessionData(session);
                sessionData.Records.Add(record);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Get all records from cache of specified session
        /// </summary>
        /// <param name="session">Session</param>
        /// <returns>List of records from cache of specified session</returns>
        public IList<HandHistoryRecord> GetRecords(string session)
        {
            if (!isStarted)
            {
                return new List<HandHistoryRecord>();
            }

            cacheLock.EnterReadLock();

            try
            {
                if (cachedData.ContainsKey(session))
                {
                    return cachedData[session].Records.ToList();
                }

                return new List<HandHistoryRecord>();
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Get all records from cache
        /// </summary>
        /// <param name="session">Session</param>
        /// <returns>List of records from cache</returns>
        public IList<HandHistoryRecord> GetAllRecords()
        {
            if (!isStarted)
            {
                return new List<HandHistoryRecord>();
            }

            cacheLock.EnterReadLock();

            try
            {
                return cachedData.SelectMany(x => x.Value.Records).ToList();
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets statistic of the player's last hand
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="player">Player name</param>
        /// <returns><see cref="Playerstatistic"/> of the last hand in session</returns>
        public Playerstatistic GetPlayersLastHandStatistics(string session, PlayerCollectionItem player)
        {
            if (!isStarted || string.IsNullOrWhiteSpace(session))
            {
                return null;
            }

            cacheLock.EnterReadLock();

            try
            {
                if (cachedData.ContainsKey(session) && cachedData[session].LastHandStatisticByPlayer.ContainsKey(player))
                {
                    return cachedData[session].LastHandStatisticByPlayer[player].Copy();
                }

                return null;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Store stickers-related statistics in cache
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="player">Player name</param>
        /// <param name="stickersStats">Dictionary of statistic accessed by sticker name</param>
        public void AddOrUpdatePlayerStickerStats(string session, PlayerCollectionItem player, IDictionary<string, Playerstatistic> stickersStats)
        {
            if (!isStarted || string.IsNullOrEmpty(session))
            {
                return;
            }

            cacheLock.EnterWriteLock();

            try
            {
                var sessionData = GetSessionData(session);

                if (sessionData == null)
                {
                    return;
                }

                Dictionary<string, Playerstatistic> playerStickersDictionary = null;

                if (sessionData.StickersStatisticByPlayer.ContainsKey(player))
                {
                    playerStickersDictionary = sessionData.StickersStatisticByPlayer[player];
                }
                else
                {
                    playerStickersDictionary = new Dictionary<string, Playerstatistic>();
                    sessionData.StickersStatisticByPlayer.Add(player, playerStickersDictionary);
                }

                foreach (var item in stickersStats)
                {
                    if (playerStickersDictionary.ContainsKey(item.Key))
                    {
                        playerStickersDictionary[item.Key].Add(item.Value);
                    }
                    else
                    {
                        playerStickersDictionary.Add(item.Key, item.Value.Copy());
                    }
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets collection of player statistics that are used for Bumper Stickers calculations
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="playerName">Player name</param>
        /// <returns>Dictionary of bumper stickers and related statistic</returns>
        public Dictionary<string, Playerstatistic> GetPlayersStickersStatistics(string session, PlayerCollectionItem player)
        {
            if (!isStarted || string.IsNullOrWhiteSpace(session))
            {
                return null;
            }

            cacheLock.EnterReadLock();

            try
            {
                if (cachedData.ContainsKey(session) && cachedData[session].StickersStatisticByPlayer.ContainsKey(player))
                {
                    return cachedData[session].StickersStatisticByPlayer[player];
                }

                return null;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Get or add session data to cache for specified session (must be used inside write lock)
        /// </summary>
        /// <param name="session">Session</param>
        /// <returns>Session data of specified session</returns>
        private SessionCacheData GetOrAddSessionData(string session)
        {
            Check.Require(!cacheLock.IsWriteLockHeld, "Write lock must be held before using this function");

            SessionCacheData sessionData = null;

            if (cachedData.ContainsKey(session))
            {
                sessionData = cachedData[session];
            }
            else
            {
                sessionData = new SessionCacheData();
                cachedData.Add(session, sessionData);
            }

            return sessionData;
        }

        private SessionCacheData GetSessionData(string session)
        {
            if (!string.IsNullOrWhiteSpace(session) && cachedData.ContainsKey(session))
            {
                return cachedData[session];
            }

            return null;
        }

        private void InitSessionCardsCollections(HudLightIndicators playerData, Playerstatistic stats)
        {
            if (playerData.CardsList == null)
            {
                playerData.CardsList = new Common.Utils.FixedSizeList<string>(4);

                if (!string.IsNullOrWhiteSpace(stats.Cards))
                {
                    playerData.CardsList.Add(stats.Cards);
                }
            }

            if (playerData.ThreeBetCardsList == null)
            {
                playerData.ThreeBetCardsList = new Common.Utils.FixedSizeList<string>(4);

                if (!string.IsNullOrWhiteSpace(stats.Cards) && stats.Didthreebet != 0)
                {
                    playerData.ThreeBetCardsList.Add(stats.Cards);
                }
            }
        }

        private void InitSessionStatCollections(HudLightIndicators playerData, Playerstatistic stats)
        {
            if (playerData.MoneyWonCollection == null)
            {
                playerData.MoneyWonCollection = new List<decimal>();
                playerData.MoneyWonCollection.Add(stats.NetWon);
            }

            if (playerData.RecentAggList == null)
            {
                playerData.RecentAggList = new Common.Utils.FixedSizeList<Tuple<int, int>>(10);
                playerData.RecentAggList.Add(new Tuple<int, int>(stats.Totalbets, stats.Totalpostflopstreetsplayed));
            }
        }
    }
}