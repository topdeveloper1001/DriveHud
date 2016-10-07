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
            cachedData.Clear();
            isStarted = false;
        }

        /// <summary>
        /// Get player stats
        /// </summary>
        /// <param name="session">Active session</param>
        /// <param name="playerName">Specified player</param>
        /// <returns>Player stats</returns>
        public IList<Playerstatistic> GetPlayerStats(string session, string playerName)
        {
            if (!isStarted)
            {
                return new List<Playerstatistic>();
            }

            cacheLock.EnterReadLock();

            try
            {
                if (cachedData.ContainsKey(session) && cachedData[session].StatisticByPlayer.ContainsKey(playerName))
                {
                    return cachedData[session].StatisticByPlayer[playerName].ToList();
                }

                return new List<Playerstatistic>();
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
        /// <param name="playerName">Specified player</param>
        /// <param name="stats">Player stats</param>
        public void AddOrUpdatePlayerStats(string session, string playerName, Playerstatistic stats, bool isHero)
        {
            if (!isStarted || string.IsNullOrEmpty(session))
            {
                return;
            }

            cacheLock.EnterWriteLock();

            try
            {
                var sessionData = GetOrAddSessionData(session);

                IList<Playerstatistic> playerData = null;

                bool skipAdding = false;

                if (sessionData.StatisticByPlayer.ContainsKey(playerName))
                {
                    playerData = sessionData.StatisticByPlayer[playerName];
                }
                else
                {
                    // if not hero read data from storage
                    if (isHero)
                    {
                        playerData = new List<Playerstatistic>();
                    }
                    else
                    {
                        playerData = dataService.GetPlayerStatisticFromFile(playerName);

                        playerData.ForEach(x =>
                        {
                            // we don't have this data in the db so update it after loading from file
                            PlayerStatisticCalculator.CalculatePositionalData(x);

                            if (x.SessionCode == session)
                            {
                                PlayerStatisticCalculator.CalculateTotalPotValues(x);
                                InitSessionCardsCollections(x);
                                InitSessionStatCollections(x);
                            }
                        });

                        skipAdding = true;
                    }

                    sessionData.StatisticByPlayer.Add(playerName, playerData);
                }

                if (skipAdding)
                {
                    return;
                }

                if (playerData.Any(x => !string.IsNullOrWhiteSpace(x.SessionCode) && x.SessionCode == session))
                {
                    playerData.First(x => x.SessionCode == session).Add(stats);
                }
                else
                {
                    InitSessionCardsCollections(stats);
                    InitSessionStatCollections(stats);
                    playerData.Add(stats);
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Get all player stats from cache in specified session
        /// </summary>
        /// <param name="session">Active session</param>
        /// <returns>Player stats</returns>
        public IList<Playerstatistic> GetAllPlayerStats(string session)
        {
            if (!isStarted || string.IsNullOrEmpty(session))
            {
                return new List<Playerstatistic>();
            }

            cacheLock.EnterReadLock();

            try
            {
                if (cachedData.ContainsKey(session))
                {
                    var result = cachedData[session].StatisticByPlayer.SelectMany(x => x.Value).ToList();
                    return result;
                }

                return new List<Playerstatistic>();
            }
            finally
            {
                cacheLock.ExitReadLock();
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

        private void InitSessionCardsCollections(Playerstatistic stats)
        {
            if (stats.CardsList == null)
            {
                stats.CardsList = new Common.Utils.FixedSizeList<string>(4);
                if (!string.IsNullOrWhiteSpace(stats.Cards))
                {
                    stats.CardsList.Add(stats.Cards);
                }
            }

            if (stats.ThreeBetCardsList == null)
            {
                stats.ThreeBetCardsList = new Common.Utils.FixedSizeList<string>(4);
                if (!string.IsNullOrWhiteSpace(stats.Cards) && stats.Didthreebet != 0)
                {
                    stats.ThreeBetCardsList.Add(stats.Cards);
                }
            }
        }

        private void InitSessionStatCollections(Playerstatistic stats)
        {
            if (stats.MoneyWonCollection == null)
            {
                stats.MoneyWonCollection = new List<decimal>();
                stats.MoneyWonCollection.Add(stats.NetWon);
            }

            if (stats.RecentAggList == null)
            {
                stats.RecentAggList = new Common.Utils.FixedSizeList<Tuple<int, int>>(10);
                stats.RecentAggList.Add(new Tuple<int, int>(stats.Totalbets, stats.Totalpostflopstreetsplayed));
            }
        }
    }
}