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
using DriveHUD.Common.Log;
using System.Threading.Tasks;

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
        private const int SessionLifeTime = 12;

        /// <summary>
        /// Amount of hands required to start using existing sessions instead of loading full data
        /// </summary>
        private const int TotalHandsToUseExistingSession = 1000;

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
        public SessionCacheStatistic GetPlayerStats(string session, PlayerCollectionItem player, out bool exists)
        {
            exists = false;

            if (!isStarted)
            {
                return new SessionCacheStatistic();
            }

            cacheLock.EnterReadLock();

            try
            {
                if (cachedData.ContainsKey(session) && cachedData[session].StatisticByPlayer.ContainsKey(player))
                {
                    exists = true;
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
        /// Get player stats
        /// </summary>
        /// <param name="session">Active session</param>
        /// <param name="player">Specified player</param>
        /// <returns>Player stats</returns>
        public SessionCacheStatistic GetPlayerStats(string session, PlayerCollectionItem player)
        {
            return GetPlayerStats(session, player, out bool exists);
        }

        /// <summary>
        /// Stores specified player data in cache
        /// </summary>
        /// <param name="cacheInfo"><see cref="PlayerStatsSessionCacheInfo"/> to store in cache</param>    
        public void AddOrUpdatePlayersStats(IEnumerable<PlayerStatsSessionCacheInfo> cacheInfos, string session, SessionStatisticCondition condition)
        {
            if (!isStarted || cacheInfos == null || !cacheInfos.Any() || string.IsNullOrEmpty(session))
            {
                return;
            }

            cacheLock.EnterWriteLock();

            try
            {
                var cacheCreationInfo = cacheInfos.First();

                var sessionCreationInfo = new SessionCreationInfo
                {
                    Session = session,
                    PokerSiteId = (int)cacheCreationInfo.Player.PokerSite,
                    IsTourney = cacheCreationInfo.Stats.IsTourney,
                    PokerGameTypeId = cacheCreationInfo.Stats.PokergametypeId
                };

                var sessionData = GetOrAddSessionData(sessionCreationInfo);

                // clear statistic for all players except for hero if filter was changed                
                if (!condition.Filter.Equals(sessionData.Filter))
                {
                    sessionData.StatisticByPlayer.RemoveByCondition(x => !x.Value.IsHero);
                    sessionData.Filter = condition.Filter;
                }

                // clear statistic for all players if heat map stats was changed            
                if (condition.HeatMapStats.Count() > 0 &&
                        condition.HeatMapStats.Any(x => !sessionData.HeatMapStats.Contains(x)))
                {
                    sessionData.StatisticByPlayer.Clear();
                    sessionData.HeatMapStats = condition.HeatMapStats;
                }

                var playerSessionCacheStatistic = GetSessionCacheStatistic(cacheInfos, sessionData);

                foreach (var playerSessionCache in playerSessionCacheStatistic)
                {
                    var cacheInfo = playerSessionCache.Key;
                    var sessionCacheStatistic = playerSessionCache.Value;

                    // Add last hand info
                    if (sessionData.LastHandStatisticByPlayer.ContainsKey(cacheInfo.Player))
                    {
                        sessionData.LastHandStatisticByPlayer[cacheInfo.Player] = cacheInfo.Stats.Copy();
                    }
                    else
                    {
                        sessionData.LastHandStatisticByPlayer.Add(cacheInfo.Player, cacheInfo.Stats.Copy());
                    }

                    if (cacheInfo.IsHero && !sessionCacheStatistic.SessionHands.Contains(cacheInfo.Stats.GameNumber))
                    {
                        sessionCacheStatistic.SessionHands.Add(cacheInfo.Stats.GameNumber);
                    }

                    sessionCacheStatistic.PlayerData.AddStatistic(cacheInfo.Stats);

                    InitSessionCardsCollections(sessionCacheStatistic.PlayerData, cacheInfo.Stats);
                    InitSessionStatCollections(sessionCacheStatistic.PlayerData, cacheInfo.Stats);

                    if (cacheInfo.Stats.SessionCode == session)
                    {
                        sessionCacheStatistic.SessionPlayerData.AddStatistic(cacheInfo.Stats);

                        InitSessionCardsCollections(sessionCacheStatistic.SessionPlayerData, cacheInfo.Stats);
                        InitSessionStatCollections(sessionCacheStatistic.SessionPlayerData, cacheInfo.Stats);
                    }
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets player session cache data using existing data or loading from DB
        /// </summary>
        /// <param name="cacheInfos"></param>
        /// <param name="sessionData"></param>
        /// <returns></returns>
        private Dictionary<PlayerStatsSessionCacheInfo, SessionCacheStatistic> GetSessionCacheStatistic(IEnumerable<PlayerStatsSessionCacheInfo> cacheInfos,
            SessionCacheData sessionData)
        {
            var playerSessionCacheStatistic = new Dictionary<PlayerStatsSessionCacheInfo, SessionCacheStatistic>();

            var playerSessionCacheStatisticTasks = new List<Task>();

            foreach (var cacheInfo in cacheInfos)
            {
                if (playerSessionCacheStatistic.ContainsKey(cacheInfo))
                {
                    continue;
                }

                SessionCacheStatistic sessionCacheStatistic = null;

                var isNewSessionCacheStatistic = true;

                // existing statistic
                if (sessionData.StatisticByPlayer.ContainsKey(cacheInfo.Player))
                {
                    sessionCacheStatistic = sessionData.StatisticByPlayer[cacheInfo.Player];
                    isNewSessionCacheStatistic = false;
                }
                // hero starts empty session
                else if (cacheInfo.IsHero)
                {
                    sessionCacheStatistic = new SessionCacheStatistic
                    {
                        IsHero = true,
                        GameFormat = cacheInfo.GameFormat
                    };
                }
                // try to check existing sessions for this player
                else
                {
                    var similarPlayerSession = cachedData.Values.FirstOrDefault(x => x.PokerSiteId == sessionData.PokerSiteId &&
                        x.IsTourney == sessionData.IsTourney &&
                        GameTypeUtils.CompareGameType((GameType)x.PokerGameTypeId, (GameType)sessionData.PokerGameTypeId) &&
                        ((x.Filter == null && sessionData.Filter == null) || x.Filter.Equals(sessionData.Filter)) &&
                        x.StatisticByPlayer.ContainsKey(cacheInfo.Player));

                    if (similarPlayerSession != null &&
                        similarPlayerSession.StatisticByPlayer[cacheInfo.Player].PlayerData.TotalHands >= TotalHandsToUseExistingSession)
                    {
                        sessionCacheStatistic = similarPlayerSession.StatisticByPlayer[cacheInfo.Player].Clone();
                    }
                }

                var needToReadStatistic = sessionCacheStatistic == null;

                if (needToReadStatistic)
                {
                    sessionCacheStatistic = new SessionCacheStatistic
                    {
                        GameFormat = cacheInfo.GameFormat
                    };
                }

                playerSessionCacheStatistic.Add(cacheInfo, sessionCacheStatistic);

                if (isNewSessionCacheStatistic)
                {
                    sessionData.StatisticByPlayer.Add(cacheInfo.Player, sessionCacheStatistic); ;
                }

                if (!needToReadStatistic)
                {
                    continue;
                }

                sessionCacheStatistic.PlayerData.InitializeHeatMaps(sessionData.HeatMapStats);

                // read data from statistic
                var taskToReadPlayerStats = Task.Run(() =>
                    playerStatisticRepository
                        .GetPlayerStatistic(cacheInfo.Player.PlayerId)
                        .Where(stat => (stat.PokersiteId == (short)cacheInfo.Player.PokerSite) &&
                            stat.IsTourney == cacheInfo.Stats.IsTourney &&
                            GameTypeUtils.CompareGameType((GameType)stat.PokergametypeId, (GameType)cacheInfo.Stats.PokergametypeId) &&
                            (sessionData.Filter == null || sessionData.Filter.Apply(stat))
                            && stat.GameNumber != cacheInfo.Stats.GameNumber)
                        .ForEach(stat => sessionCacheStatistic.PlayerData.AddStatistic(stat))
                );

                playerSessionCacheStatisticTasks.Add(taskToReadPlayerStats);
            }

            Task.WhenAll(playerSessionCacheStatisticTasks).Wait();

            return playerSessionCacheStatistic;
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
        public void AddOrUpdatePlayerStickerStats(IEnumerable<PlayerStickersCacheData> playersStickersCacheData, string session)
        {
            if (!isStarted || playersStickersCacheData == null || !playersStickersCacheData.Any())
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

                var playersStickersCacheDataInfo = PreparePlayerStickersCacheDataInfo(playersStickersCacheData, sessionData);

                playersStickersCacheDataInfo.Where(x => !x.ReloadRequired).ForEach(x => AddStatToSticker(x.PlayerStickersCacheData.Statistic.Copy(),
                    x.PlayerStickersCacheData, x.PlayerStickersDictionary));

                var tasksToReloadStickersData = new List<Task>();

                playersStickersCacheDataInfo.Where(x => x.ReloadRequired).ForEach(x =>
                {
                    var playerStickersCacheData = x.PlayerStickersCacheData;

                    var sessionHands = sessionData.StatisticByPlayer.ContainsKey(playerStickersCacheData.Player) ?
                            sessionData.StatisticByPlayer[x.PlayerStickersCacheData.Player].SessionHands :
                            new HashSet<long>();

                    var task = Task.Run(() =>
                        playerStatisticRepository
                            .GetPlayerStatistic(playerStickersCacheData.Player.PlayerId)
                            .Where(stat => (stat.PokersiteId == (short)playerStickersCacheData.Player.PokerSite) &&
                                stat.IsTourney == playerStickersCacheData.Statistic.IsTourney &&
                                ((playerStickersCacheData.IsHero && sessionHands.Contains(stat.GameNumber)) || !playerStickersCacheData.IsHero) &&
                                GameTypeUtils.CompareGameType((GameType)stat.PokergametypeId, (GameType)playerStickersCacheData.Statistic.PokergametypeId))
                            .ForEach(stat =>
                            {
                                AddStatToSticker(stat, playerStickersCacheData, x.PlayerStickersDictionary);
                            })
                    );

                    tasksToReloadStickersData.Add(task);
                });

                Task.WhenAll(tasksToReloadStickersData).Wait();
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
        /// Adds stat to sticker data
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="playerStickersCacheData"></param>
        /// <param name="playerStickersDictionary"></param>
        private void AddStatToSticker(Playerstatistic stat, PlayerStickersCacheData playerStickersCacheData, Dictionary<string, HudLightIndicators> playerStickersDictionary)
        {
            foreach (var stickerFilter in playerStickersCacheData.StickerFilters)
            {
                if (!stickerFilter.Value(stat))
                {
                    continue;
                }

                if (playerStickersDictionary.ContainsKey(stickerFilter.Key))
                {
                    playerStickersDictionary[stickerFilter.Key].AddStatistic(stat);
                }
                else
                {
                    playerStickersDictionary.Add(stickerFilter.Key, new HudLightIndicators(new[] { stat.Copy() }));
                }
            }
        }

        /// <summary>
        /// Prepares data to update player stickers
        /// </summary>
        /// <param name="playersStickersCacheData"></param>
        /// <param name="sessionData"></param>
        /// <returns></returns>
        private List<PlayerStickersCacheDataInfo> PreparePlayerStickersCacheDataInfo(IEnumerable<PlayerStickersCacheData> playersStickersCacheData,
            SessionCacheData sessionData)
        {
            var playersStickersCacheDataInfo = new List<PlayerStickersCacheDataInfo>();

            foreach (var playerStickersCacheData in playersStickersCacheData)
            {
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

                Dictionary<string, HudLightIndicators> playerStickersDictionary = null;

                if (sessionData.StickersStatisticByPlayer.ContainsKey(playerStickersCacheData.Player))
                {
                    playerStickersDictionary = sessionData.StickersStatisticByPlayer[playerStickersCacheData.Player];
                }
                else
                {
                    playerStickersDictionary = new Dictionary<string, HudLightIndicators>();
                    sessionData.StickersStatisticByPlayer.Add(playerStickersCacheData.Player, playerStickersDictionary);
                }


                var playerStickersCacheDataInfo = new PlayerStickersCacheDataInfo
                {
                    PlayerStickersCacheData = playerStickersCacheData,
                    PlayerStickersDictionary = playerStickersDictionary,
                    ReloadRequired = layoutWasChanged
                };

                playersStickersCacheDataInfo.Add(playerStickersCacheDataInfo);
            }

            return playersStickersCacheDataInfo;
        }

        /// <summary>
        /// Gets collection of player statistics that are used for Bumper Stickers calculations
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="playerName">Player name</param>
        /// <returns>Dictionary of bumper stickers and related statistic</returns>
        public Dictionary<string, HudLightIndicators> GetPlayersStickersStatistics(string session, PlayerCollectionItem player)
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
        private SessionCacheData GetOrAddSessionData(SessionCreationInfo sessionCreationInfo)
        {
            Check.Require(!cacheLock.IsWriteLockHeld, "Write lock must be held before using this function");

            SessionCacheData sessionData = null;

            if (cachedData.ContainsKey(sessionCreationInfo.Session))
            {
                sessionData = cachedData[sessionCreationInfo.Session];
                sessionData.LastModified = DateTime.Now;
            }
            else
            {
                sessionData = new SessionCacheData
                {
                    PokerSiteId = sessionCreationInfo.PokerSiteId,
                    IsTourney = sessionCreationInfo.IsTourney,
                    PokerGameTypeId = sessionCreationInfo.PokerGameTypeId
                };

                cachedData.Add(sessionCreationInfo.Session, sessionData);
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

                if (timeSinceLastUpdate.TotalMinutes >= SessionLifeTime)
                {
                    cachedData.Remove(sessionKey);
                }
            }
        }

        private class SessionCreationInfo
        {
            public string Session { get; set; }

            public int PokerSiteId { get; set; }

            public bool IsTourney { get; set; }

            public short PokerGameTypeId { get; set; }
        }

        private class PlayerStickersCacheDataInfo
        {
            public Dictionary<string, HudLightIndicators> PlayerStickersDictionary { get; set; }

            public PlayerStickersCacheData PlayerStickersCacheData { get; set; }

            public bool ReloadRequired { get; set; }
        }
    }
}