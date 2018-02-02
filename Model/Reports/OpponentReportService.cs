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

using DriveHud.Common.Log;
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
            using (var pf = new PerformanceMonitor(nameof(GetReport)))
            {
                if (storageModel == null ||
                    storageModel.PlayerSelectedItem == null)
                {
                    return null;
                }

                TryLoadData();

                var selectedPlayers = storageModel.PlayerSelectedItem.PlayerIds;

                var playersNetWons = dataService.GetTopPlayersByNetWon(top, selectedPlayers);

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
                    .Where(x => !selectedPlayers.Contains(x.PlayerId))
                    .OrderByDescending(x => x.NetWon)
                    .Take(top)
                    .ToList();
            }
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
        /// </summary>k
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
                            var deserializedData = Serializer.Deserialize<List<OpponentReportIndicators>>(fs);
                            opponentsData = deserializedData.ToDictionary(x => x.PlayerId);
                            UpdateCacheStatus(false);
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogProvider.Log.Error(this, $"Opponent report could not be loaded from '{ReportFile}'", ex);
                    }
                }

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
            using (var pf = new PerformanceMonitor(nameof(SaveData)))
            {
                var opponentsDataToSerialize = opponentsData.Values.ToList();

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

            dataService.ActOnPlayerStatisticFromFile(playerId, x => !x.IsTourney, x => opponentReportIndicators.AddStatistic(x));

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
    }
}