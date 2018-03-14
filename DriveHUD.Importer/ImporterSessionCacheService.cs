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
using HandHistories.Objects.GameDescription;
using Model.Enums;
using DriveHUD.Common.Log;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Importer cache service which stores session data
    /// </summary>
    internal class ImporterSessionCacheService : IImporterSessionCacheService
    {
        /// <summary>
        /// Interval in minutes after which session will expire and will be removed from cache
        /// </summary>
        private const int sessionLifeTime = 12;

        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        private ReaderWriterLockSlim globalCacheLock = new ReaderWriterLockSlim();

        private Dictionary<string, SessionCacheData> cachedData;

        private readonly IPlayerStatisticRepository playerStatisticRepository;

        private bool isStarted;

        public ImporterSessionCacheService()
        {
            playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
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

                // clear statistic for all players except for hero if filter was changed                
                if (!cacheInfo.Filter.Equals(sessionData.Filter))
                {
                    sessionData.StatisticByPlayer.RemoveByCondition(x => !x.Value.IsHero);
                    sessionData.Filter = cacheInfo.Filter;
                }

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
                        sessionCacheStatistic = new SessionCacheStatistic
                        {
                            IsHero = true,
                            GameFormat = cacheInfo.GameFormat
                        };
                    }
                    else
                    {
                        sessionCacheStatistic = new SessionCacheStatistic
                        {
                            GameFormat = cacheInfo.GameFormat
                        };

                        playerStatisticRepository
                            .GetPlayerStatistic(cacheInfo.Player.PlayerId)
                            .Where(stat => (stat.PokersiteId == (short)cacheInfo.Player.PokerSite) &&
                                stat.IsTourney == cacheInfo.Stats.IsTourney &&
                                GameTypeUtils.CompareGameType((GameType)stat.PokergametypeId, (GameType)cacheInfo.Stats.PokergametypeId) &&
                                (cacheInfo.Filter == null || cacheInfo.Filter.Apply(stat)))
                            .ForEach(stat =>
                            {
                                var isCurrentGame = stat.GameNumber == cacheInfo.Stats.GameNumber;

                                if (isCurrentGame)
                                {
                                    stat.CalculateTotalPot();
                                    stat.SessionCode = cacheInfo.Session;
                                }

                                sessionCacheStatistic.PlayerData.AddStatistic(stat);

                                if (isCurrentGame)
                                {
                                    sessionCacheStatistic.SessionPlayerData.AddStatistic(stat);

                                    InitSessionCardsCollections(sessionCacheStatistic.PlayerData, stat);
                                    InitSessionCardsCollections(sessionCacheStatistic.SessionPlayerData, stat);
                                    InitSessionStatCollections(sessionCacheStatistic.PlayerData, stat);
                                    InitSessionStatCollections(sessionCacheStatistic.SessionPlayerData, stat);
                                }

                                if (!skipAdding)
                                {
                                    skipAdding = true;
                                }
                            });
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

                if (cacheInfo.IsHero && !sessionCacheStatistic.SessionHands.Contains(cacheInfo.Stats.GameNumber))
                {
                    sessionCacheStatistic.SessionHands.Add(cacheInfo.Stats.GameNumber);
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
        public void AddOrUpdatePlayerStickerStats(PlayerStickersCacheData playerStickersCacheData)
        {
            if (!isStarted || playerStickersCacheData == null || !playerStickersCacheData.IsValid())
            {
                return;
            }

            cacheLock.EnterWriteLock();

            try
            {
                var sessionData = GetSessionData(playerStickersCacheData.Session);

                if (sessionData == null)
                {
                    return;
                }

                var layoutWasChanged = !sessionData.LayoutByPlayer.ContainsKey(playerStickersCacheData.Player) ||
                     sessionData.LayoutByPlayer[playerStickersCacheData.Player] != playerStickersCacheData.Layout;

                if (layoutWasChanged)
                {
                    if (sessionData.StickersStatisticByPlayer.ContainsKey(playerStickersCacheData.Player))
                    {
                        sessionData.StickersStatisticByPlayer[playerStickersCacheData.Player]?.Clear();
                    }

                    if (sessionData.LayoutByPlayer.ContainsKey(playerStickersCacheData.Player))
                    {
                        sessionData.LayoutByPlayer[playerStickersCacheData.Player] = playerStickersCacheData.Layout;
                    }
                    else
                    {
                        sessionData.LayoutByPlayer.Add(playerStickersCacheData.Player, playerStickersCacheData.Layout);
                    }
                }

                Dictionary<string, Playerstatistic> playerStickersDictionary = null;

                if (sessionData.StickersStatisticByPlayer.ContainsKey(playerStickersCacheData.Player))
                {
                    playerStickersDictionary = sessionData.StickersStatisticByPlayer[playerStickersCacheData.Player];
                }
                else
                {
                    playerStickersDictionary = new Dictionary<string, Playerstatistic>();
                    sessionData.StickersStatisticByPlayer.Add(playerStickersCacheData.Player, playerStickersDictionary);
                }

                Action<Playerstatistic> addStatToSticker = stat =>
                {
                    foreach (var stickerFilter in playerStickersCacheData.StickerFilters)
                    {
                        if (!stickerFilter.Value(stat))
                        {
                            continue;
                        }

                        if (playerStickersDictionary.ContainsKey(stickerFilter.Key))
                        {
                            playerStickersDictionary[stickerFilter.Key].Add(stat);
                        }
                        else
                        {
                            playerStickersDictionary.Add(stickerFilter.Key, stat.Copy());
                        }
                    }
                };

                if (layoutWasChanged)
                {
                    var sessionHands = sessionData.StatisticByPlayer.ContainsKey(playerStickersCacheData.Player) ?
                            sessionData.StatisticByPlayer[playerStickersCacheData.Player].SessionHands :
                            new HashSet<long>();

                    playerStatisticRepository
                        .GetPlayerStatistic(playerStickersCacheData.Player.PlayerId)
                        .Where(stat => (stat.PokersiteId == (short)playerStickersCacheData.Player.PokerSite) &&
                            stat.IsTourney == playerStickersCacheData.Statistic.IsTourney &&
                            ((playerStickersCacheData.IsHero && sessionHands.Contains(stat.GameNumber)) || !playerStickersCacheData.IsHero) &&
                            GameTypeUtils.CompareGameType((GameType)stat.PokergametypeId, (GameType)playerStickersCacheData.Statistic.PokergametypeId))
                        .ForEach(stat =>
                        {
                            addStatToSticker(stat);
                        });

                    return;
                }

                addStatToSticker(playerStickersCacheData.Statistic.Copy());
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not add stickers stat to cache.", e);
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
                sessionData.LastModified = DateTime.Now;
            }
            else
            {
                sessionData = new SessionCacheData();
                cachedData.Add(session, sessionData);
            }

            RemoveExpiredSessions();

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

        private void RemoveExpiredSessions()
        {
            Check.Require(!cacheLock.IsWriteLockHeld, "Write lock must be held before using this function");

            var now = DateTime.Now;

            foreach (var sessionKey in cachedData.Keys.ToArray())
            {
                var timeSinceLastUpdate = now - cachedData[sessionKey].LastModified;

                if (timeSinceLastUpdate.TotalMinutes >= sessionLifeTime)
                {
                    cachedData.Remove(sessionKey);
                }
            }
        }
    }
}