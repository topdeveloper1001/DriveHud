//-----------------------------------------------------------------------
// <copyright file="OpponentReportService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Interfaces;
using Model.Settings;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model.Reports
{
    /// <summary>
    /// Service to read/update/cache data for opponent report
    /// </summary>
    internal class OpponentReportService : IOpponentReportService
    {
        private const string ReportFileName = "OpponentAnalysisReport.data";

        private const int top = 20;

        private readonly string ReportFile = Path.Combine(StringFormatter.GetAppDataFolderPath(), ReportFileName);

        private readonly SingletonStorageModel storageModel;

        private readonly IDataService dataService;

        private readonly ISettingsService settingsService;

        private static readonly object syncLock = new object();

        private Dictionary<int, OpponentReportIndicators> opponentsData;

        private HashSet<HandHistoryKey> playerHands;

        private int[] playerIds;

        public OpponentReportService()
        {
            storageModel = ServiceLocator.Current.GetInstance<SingletonStorageModel>();
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
            settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
        }

        /// <summary>
        /// Gets the opponent report 
        /// </summary>
        /// <returns>The list of <see cref="Indicators"/></returns>
        public IEnumerable<ReportIndicators> GetReport()
        {
            if (storageModel == null ||
                storageModel.PlayerSelectedItem == null)
            {
                return null;
            }

            TryLoadData();

            var selectedPlayers = storageModel.PlayerSelectedItem.PlayerIds.ToArray();

            if (!playerIds.ItemsEqual(selectedPlayers))
            {
                lock (syncLock)
                {
                    opponentsData.Clear();
                }

                playerIds = selectedPlayers;

                LoadPlayerHands();
            }

            var playersNetWons = dataService.GetTopPlayersByNetWon(top, playerIds);

            var playersNetWonsToUpdate = playersNetWons.Where(x => !opponentsData.ContainsKey(x.PlayerId)).ToArray();

            if (playersNetWonsToUpdate.Length > 0)
            {
                lock (syncLock)
                {
                    playersNetWonsToUpdate.ForEach(x => LoadPlayerData(x.PlayerId));
                    SaveData();
                }
            }

            return opponentsData.Values
                .Where(x => !playerIds.Contains(x.PlayerId))
                .OrderByDescending(x => x.NetWon)
                .Take(top)
                .ToList();
        }

        /// <summary>
        /// Updates stats in the opponents report
        /// </summary>
        /// <param name="stat">Stat to add to the report</param>
        public void UpdateReport(Playerstatistic stat)
        {
            if (stat == null || !TryLoadData() || !opponentsData.ContainsKey(stat.PlayerId))
            {
                return;
            }

            lock (syncLock)
            {
                opponentsData[stat.PlayerId].AddStatistic(stat);
                opponentsData[stat.PlayerId].ShrinkReportHands();

                var handKey = new HandHistoryKey(stat.GameNumber, stat.PokersiteId);

                if (!playerHands.Contains(handKey))
                {
                    playerHands.Add(handKey);
                }
            }
        }

        /// <summary>
        /// Checks whenever player is presented in the report
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <returns>True is player exists, otherwise - false</returns>
        public bool IsPlayerInReport(int playerId)
        {
            return opponentsData != null && opponentsData.ContainsKey(playerId);
        }

        /// <summary>
        /// Flush cached data to the file
        /// </summary>
        public void Flush()
        {
            if (opponentsData == null || opponentsData.Count == 0)
            {
                Reset();
                return;
            }

            lock (syncLock)
            {
                if (!SaveData())
                {
                    return;
                }

                UpdateCacheStatus(true);
            }
        }

        /// <summary>
        /// Resets cached data
        /// </summary>
        public void Reset()
        {
            lock (syncLock)
            {
                opponentsData?.Clear();

                if (File.Exists(ReportFile))
                {
                    try
                    {
                        File.Delete(ReportFile);
                    }
                    catch (Exception e)
                    {
                        LogProvider.Log.Error(this, $"Opponent report could not be delete at '{ReportFile}'", e);
                    }
                }
            }
        }

        /// <summary>
        /// Loads only those hands in which hero was involved for the specified player
        /// </summary>
        /// <param name="playerId">Player to load hands</param>
        /// <param name="count">Amount of hands to load</param>
        /// <returns>Collection of hands</returns>
        public IEnumerable<Playerstatistic> LoadPlayerHands(int playerId, int count)
        {
            HashSet<HandHistoryKey> tempPlayerHands = null;

            lock (syncLock)
            {
                tempPlayerHands = new HashSet<HandHistoryKey>(playerHands);
            }

            var statistic = dataService.GetPlayerStatisticFromFile(playerId, x => playerHands.Contains(new HandHistoryKey(x.GameNumber, x.PokersiteId)) && !x.IsTourney)
                       .OrderByDescending(x => x.Time)
                       .Take(count)
                       .ToArray();

            return statistic;
        }

        /// <summary>
        /// Loads data from the opponent report file
        /// </summary>
        /// <returns>True if data is loaded, otherwise - false</returns>
        private bool TryLoadData()
        {
            if (opponentsData != null)
            {
                return true;
            }

            lock (syncLock)
            {
                if (opponentsData != null)
                {
                    return true;
                }

                if (File.Exists(ReportFile) && InvalidateCache())
                {
                    try
                    {
                        using (var fs = new FileStream(ReportFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                        {
                            var deserializedData = Serializer.Deserialize<OpponentReportCache>(fs);
                            playerIds = deserializedData.PlayerIds;
                            opponentsData = deserializedData.Report.ToDictionary(x => x.PlayerId);
                            LoadPlayerHands();
                            UpdateCacheStatus(false);
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogProvider.Log.Error(this, $"Opponent report could not be loaded from '{ReportFile}'", ex);
                    }
                }

                playerIds = new int[0];
                LoadPlayerHands();
                opponentsData = new Dictionary<int, OpponentReportIndicators>();
                UpdateCacheStatus(false);
            }

            return false;
        }

        /// <summary>
        /// Saves data to the opponent report file
        /// </summary>
        private bool SaveData()
        {
            var opponentsDataToSerialize = new OpponentReportCache
            {
                PlayerIds = playerIds,
                Report = opponentsData.Values.ToList()
            };

            try
            {
                using (var fs = new FileStream(ReportFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    Serializer.Serialize(fs, opponentsDataToSerialize);
                    return true;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not save opponent report to '{ReportFile}'", e);
            }

            return false;
        }

        /// <summary>
        /// Loads and adds the data of the specified player to the report
        /// </summary>
        /// <param name="player">Player to load data</param>
        private void LoadPlayerData(int playerId)
        {
            var opponentReportIndicators = new OpponentReportIndicators
            {
                PlayerId = playerId
            };

            opponentsData.Add(playerId, opponentReportIndicators);

            dataService.ActOnPlayerStatisticFromFile(playerId,
                x => playerHands.Contains(new HandHistoryKey(x.GameNumber, x.PokersiteId)) && !x.IsTourney,
                x => opponentReportIndicators.AddStatistic(x));

            opponentReportIndicators.ShrinkReportHands();
        }

        /// <summary>
        /// Invalidates cache
        /// </summary>
        /// <returns>True if cache is valid, otherwise - false</returns>
        private bool InvalidateCache()
        {
            var settingsModel = settingsService.GetSettings();

            return settingsModel != null && settingsModel.GeneralSettings != null ?
                settingsModel.GeneralSettings.IsOpponentReportCacheSaved :
                false;
        }

        /// <summary>
        /// Updates cache status in settings
        /// </summary>
        /// <param name="isValid">Status</param>
        private void UpdateCacheStatus(bool isValid)
        {
            var settingsModel = settingsService.GetSettings();

            if (settingsModel != null && settingsModel.GeneralSettings != null)
            {
                settingsModel.GeneralSettings.IsOpponentReportCacheSaved = isValid;
                settingsService.SaveSettings(settingsModel);
            }
        }

        /// <summary>
        /// Loads player played hands
        /// </summary>
        private void LoadPlayerHands()
        {
            if (storageModel.StatisticCollection == null)
            {
                playerHands = new HashSet<HandHistoryKey>();
                return;
            }

            playerHands = new HashSet<HandHistoryKey>(storageModel.StatisticCollection
                .ToArray()
                .Select(x => new HandHistoryKey(x.GameNumber, x.PokersiteId)));
        }

        private class HandHistoryKey
        {
            public HandHistoryKey(long gameNumber, int pokerSiteId) : this(gameNumber, (short)pokerSiteId)
            {
            }

            public HandHistoryKey(long gameNumber, short pokerSiteId)
            {
                GameNumber = gameNumber;
                PokerSiteId = pokerSiteId;
            }

            public long GameNumber { get; private set; }

            public short PokerSiteId { get; private set; }

            public override bool Equals(object obj)
            {
                var handHistoryKey = obj as HandHistoryKey;

                return Equals(handHistoryKey);
            }

            private bool Equals(HandHistoryKey handHistoryKey)
            {
                return handHistoryKey != null && handHistoryKey.GameNumber == GameNumber && handHistoryKey.PokerSiteId == handHistoryKey.PokerSiteId;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + GameNumber.GetHashCode();
                    hashcode = (hashcode * 31) + PokerSiteId;
                    return hashcode;
                }
            }
        }
    }
}