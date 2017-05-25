//-----------------------------------------------------------------------
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
using HandHistories.Objects.GameDescription;
using Model.Enums;

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
        /// Stores specified player data in cache
        /// </summary>
        /// <param name="cacheInfo"><see cref="PlayerStatsSessionCacheInfo"/> to store in cache</param>    
        public void AddOrUpdatePlayerStats(PlayerStatsSessionCacheInfo cacheInfo)
        {
            if (!isStarted || cacheInfo == null || string.IsNullOrEmpty(cacheInfo.Session))
            {
                return;
            }

            cacheLock.EnterWriteLock();

            try
            {
                var sessionData = GetOrAddSessionData(cacheInfo.Session);

                SessionCacheStatistic sessionCacheStatistic = null;

                bool skipAdding = false;

                // get or initialize new session 
                if (sessionData.StatisticByPlayer.ContainsKey(cacheInfo.Player))
                {
                    sessionCacheStatistic = sessionData.StatisticByPlayer[cacheInfo.Player];
                }
                else
                {
                    // if not hero read data from storage
                    if (cacheInfo.IsHero)
                    {
                        sessionCacheStatistic = new SessionCacheStatistic();
                    }
                    else
                    {
                        var playerStatistic = dataService.GetPlayerStatisticFromFile(cacheInfo.Player.PlayerId, (short)cacheInfo.Player.PokerSite)
                            .Where(x => x.IsTourney == cacheInfo.Stats.IsTourney &&
                                    GameTypeUtils.CompareGameType((GameType)x.PokergametypeId, (GameType)cacheInfo.Stats.PokergametypeId))
                            .ToList();

                        sessionCacheStatistic = new SessionCacheStatistic();

                        playerStatistic.ForEach(x =>
                        {
                            var isCurrentGame = x.GameNumber == cacheInfo.Stats.GameNumber;

                            if (isCurrentGame)
                            {
                                x.CalculateTotalPot();
                                x.SessionCode = cacheInfo.Session;
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

                    sessionData.StatisticByPlayer.Add(cacheInfo.Player, sessionCacheStatistic);
                }

                if (sessionData.LastHandStatisticByPlayer.ContainsKey(cacheInfo.Player))
                {
                    sessionData.LastHandStatisticByPlayer[cacheInfo.Player] = cacheInfo.Stats.Copy();
                }
                else
                {
                    sessionData.LastHandStatisticByPlayer.Add(cacheInfo.Player, cacheInfo.Stats.Copy());
                }

                if (skipAdding)
                {
                    return;
                }

                sessionCacheStatistic.PlayerData.AddStatistic(cacheInfo.Stats);
                InitSessionCardsCollections(sessionCacheStatistic.PlayerData, cacheInfo.Stats);
                InitSessionStatCollections(sessionCacheStatistic.PlayerData, cacheInfo.Stats);

                if (cacheInfo.Stats.SessionCode == cacheInfo.Session)
                {
                    sessionCacheStatistic.SessionPlayerData.AddStatistic(cacheInfo.Stats);
                    InitSessionCardsCollections(sessionCacheStatistic.SessionPlayerData, cacheInfo.Stats);
                    InitSessionStatCollections(sessionCacheStatistic.SessionPlayerData, cacheInfo.Stats);
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
                        playerStickersDictionary[item.Key] += item.Value;
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
            if (playerData.StatsSessionCollection == null)
            {
                playerData.StatsSessionCollection = new Dictionary<Stat, IList<decimal>>();
                playerData.AddStatsToSession(stats);
            }

            if (playerData.RecentAggList == null)
            {
                playerData.RecentAggList = new Common.Utils.FixedSizeList<Tuple<int, int>>(10);
                playerData.RecentAggList.Add(new Tuple<int, int>(stats.Totalbets, stats.Totalpostflopstreetsplayed));
            }
        }
    }
}