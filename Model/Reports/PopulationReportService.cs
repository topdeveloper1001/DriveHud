//-----------------------------------------------------------------------
// <copyright file="PopulationReportService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Enums;
using Model.Filters;
using Model.Hud;
using Model.Importer;
using Model.Interfaces;
using Model.Settings;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Model.Reports
{
    internal class PopulationReportService : IPopulationReportService
    {
        private const string ReportFileName = "PopulationReport.data";

        private const long MaxReportFileSize = 50000000;

        private readonly string ReportFile = Path.Combine(StringFormatter.GetAppDataFolderPath(), ReportFileName);

        private readonly IDataService dataService;

        private readonly IPlayerStatisticRepository playerStatisticRepository;

        private readonly IHudPlayerTypeService hudPlayerTypeService;

        private readonly SingletonStorageModel storageModel;

        private PopulationReportCache populationData;

        private bool resetOnRead = false;

        private static readonly object syncLock = new object();

        public PopulationReportService()
        {
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
            playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
            hudPlayerTypeService = ServiceLocator.Current.GetInstance<IHudPlayerTypeService>();
            storageModel = ServiceLocator.Current.GetInstance<SingletonStorageModel>();
        }

        /// <summary>
        /// Gets population data
        /// </summary>        
        public IEnumerable<ReportIndicators> GetReport(bool forceRefresh, CancellationToken cancellationToken)
        {
            try
            {
                if (!TryLoadCachedData())
                {
                    ValidateCache();
                    BuildReportData(cancellationToken);
                }
                else if (!ValidateCache() || forceRefresh)
                {
                    BuildReportData(cancellationToken);
                }

                return populationData.Report.ToList();
            }
            finally
            {
                if (resetOnRead)
                {
                    populationData = null;
                }
            }
        }

        private bool ValidateCache()
        {
            if (populationData == null)
            {
                return false;
            }

            // load existing filters
            var filterModelManagerService = ServiceLocator.Current.GetInstance<IFilterModelManagerService>(FilterServices.Main.ToString());
            var filterModelDictionary = filterModelManagerService.GetFilterModelDictionary();
            var cashFilterModel = filterModelDictionary[EnumFilterType.Cash];

            var filterStandardModel = (FilterStandardModel)cashFilterModel.FirstOrDefault(x => x is FilterStandardModel);
            var filterDateModel = (FilterDateModel)cashFilterModel.FirstOrDefault(x => x is FilterDateModel);

            var playersFrom = filterStandardModel.PlayerCountMinSelectedItem ?? 0;
            var playersTo = filterStandardModel.PlayerCountMaxSelectedItem ?? 10;

            var startDate = DateTime.MinValue;
            var endDate = DateTime.MaxValue;

            var filtersHashCode = filterModelManagerService.GetFiltersHashCode();

            switch (filterDateModel.DateFilterType.EnumDateRange)
            {
                case EnumDateFiterStruct.EnumDateFiter.Today:
                    startDate = DateTime.Now.StartOfDay();
                    break;
                case EnumDateFiterStruct.EnumDateFiter.ThisWeek:
                    var firstDayOfWeek = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings.StartDayOfWeek;
                    startDate = DateTime.Now.FirstDayOfWeek(firstDayOfWeek);
                    break;
                case EnumDateFiterStruct.EnumDateFiter.ThisMonth:
                    startDate = DateTime.Now.FirstDayOfMonth();
                    break;
                case EnumDateFiterStruct.EnumDateFiter.ThisYear:
                    startDate = DateTime.Now.FirstDayOfYear();
                    break;
                case EnumDateFiterStruct.EnumDateFiter.CustomDateRange:
                    startDate = filterDateModel.DateFilterType.DateFrom;
                    endDate = filterDateModel.DateFilterType.DateTo;
                    break;
                case EnumDateFiterStruct.EnumDateFiter.LastMonth:
                    var storageModel = ServiceLocator.Current.GetInstance<SingletonStorageModel>();

                    var lastCashAvailableDate = storageModel
                        .GetStatisticCollection()
                        .Where(x => !x.IsTourney)
                        .MaxOrDefault(x => Converter.ToLocalizedDateTime(x.Time));

                    lastCashAvailableDate = lastCashAvailableDate == DateTime.MinValue ?
                        DateTime.Now :
                        lastCashAvailableDate;

                    startDate = lastCashAvailableDate.FirstDayOfMonth();
                    break;
            }

            // compare with filters which was used to build that cache
            var isValid = playersFrom == populationData.PlayersFrom && playersTo == populationData.PlayersTo &&
                startDate == populationData.StartDate && endDate == populationData.EndDate && filtersHashCode == populationData.FiltersHashCode;

            populationData.PlayersFrom = playersFrom;
            populationData.PlayersTo = playersTo;
            populationData.StartDate = startDate;
            populationData.EndDate = endDate;
            populationData.FiltersHashCode = filtersHashCode;

            return isValid;
        }

        /// <summary>
        /// Tries to load data from the population report cache
        /// </summary>
        /// <returns>True if data is loaded; otherwise - false</returns>
        private bool TryLoadCachedData()
        {
            if (populationData != null)
            {
                return true;
            }

            lock (syncLock)
            {
                if (populationData != null)
                {
                    return true;
                }

                var reportFileInfo = new FileInfo(ReportFile);

                if (reportFileInfo.Exists)
                {
                    resetOnRead = reportFileInfo.Length > MaxReportFileSize;

                    try
                    {
                        using (var fs = new FileStream(ReportFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                        {
                            populationData = Serializer.Deserialize<PopulationReportCache>(fs);
                            LoadPlayerTypes();
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogProvider.Log.Error(this, $"Population report could not be loaded from '{ReportFile}'", ex);
                    }
                }

                populationData = new PopulationReportCache
                {
                    Report = new List<PopulationReportIndicators>()
                };
            }

            return false;
        }

        /// <summary>
        /// Builds report data
        /// </summary>
        private void BuildReportData(CancellationToken cancellationToken)
        {
            LogProvider.Log.Info("Building population report.");

            lock (syncLock)
            {
                try
                {
                    var tableType = GetTableType();

                    var playerTypes = hudPlayerTypeService.CreateDefaultPlayerTypes(tableType);

                    var players = dataService.GetPlayersList();

                    var populationIndicators = playerTypes.Select(x => new PopulationReportIndicators
                    {
                        PlayerTypeName = x.Name,
                        PlayerType = x
                    }).ToDictionary(x => x.PlayerTypeName);

                    var allPlayersType = CreateAllPlayersType();

                    var allPlayersIndicator = new PopulationReportIndicators
                    {
                        PlayerTypeName = allPlayersType.Name,
                        PlayerType = allPlayersType,
                        CanAddHands = true
                    };

                    var filterPredicate = storageModel.CashFilterPredicate.Compile();
                    var heroId = storageModel.PlayerSelectedItem?.PlayerId ?? 0;

                    Parallel.ForEach(players, (player, loopState) =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            loopState.Break();
                        }

                        if (player.PlayerId == heroId)
                        {
                            return;
                        }

                        var playerIndicators = new LightIndicators();

                        playerStatisticRepository.GetPlayerStatistic(player.PlayerId)
                            .Where(x => !x.IsTourney && FilterStatistic(x) && filterPredicate(x))
                            .ForEach(x =>
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    return;
                                }

                                playerIndicators.AddStatistic(x);

                                if (filterPredicate(x))
                                {
                                    lock (allPlayersIndicator)
                                    {
                                        allPlayersIndicator.AddStatistic(x);
                                    }
                                }
                            }
                        );

                        var playerType = hudPlayerTypeService.Match(playerIndicators, playerTypes, true);

                        if (playerType != null)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            playerStatisticRepository.GetPlayerStatistic(player.PlayerId)
                                .Where(x => !x.IsTourney && FilterStatistic(x) && filterPredicate(x))
                                .ForEach(x =>
                                {
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        return;
                                    }

                                    lock (populationIndicators[playerType.Name])
                                    {
                                        populationIndicators[playerType.Name].AddStatistic(x);
                                    }
                                }
                            );
                        }
                    });

                    populationIndicators.Add(allPlayersIndicator.PlayerTypeName, allPlayersIndicator);

                    populationIndicators.Values.ForEach(x => x.PrepareHands());

                    populationData.Report = populationIndicators.Values.ToList();

                    if (cancellationToken.IsCancellationRequested)
                    {
                        LogProvider.Log.Info("Population report has been cancelled.");
                        populationData.Report = new List<PopulationReportIndicators>();
                        return;
                    }

                    SaveCachedData();

                    LogProvider.Log.Info("Population report has been built.");
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Could build population report data.", e);
                }
            }
        }

        /// <summary>
        /// Saves data to the opponent report file
        /// </summary>
        private bool SaveCachedData()
        {
            try
            {
                using (var fs = new FileStream(ReportFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    Serializer.Serialize(fs, populationData);
                    return true;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not save population report to '{ReportFile}'", e);
            }

            return false;
        }

        /// <summary>
        /// Load player types and set them to report indicators
        /// </summary>
        private void LoadPlayerTypes()
        {
            var tableType = GetTableType();

            var playerTypes = hudPlayerTypeService
                .CreateDefaultPlayerTypes(tableType)
                .Concat(new[] { CreateAllPlayersType() })
                .ToDictionary(x => x.Name);

            populationData.Report.ForEach(x =>
            {
                if (playerTypes.ContainsKey(x.PlayerTypeName))
                {
                    x.PlayerType = playerTypes[x.PlayerTypeName];
                }
            });
        }

        private HudPlayerType CreateAllPlayersType()
        {
            var hudPlayerType = new HudPlayerType
            {
                Name = CommonResourceManager.Instance.GetResourceString("Reports_AllPlayers_PlayerType")
            };

            return hudPlayerType;
        }

        /// <summary>
        /// Gets table type based on filters
        /// </summary>
        /// <returns></returns>
        private EnumTableType GetTableType()
        {
            if (populationData != null && populationData.PlayersFrom > 6)
            {
                return EnumTableType.Nine;
            }

            return EnumTableType.Six;
        }

        /// <summary>
        /// Filters the specified <see cref="Playerstatistic"/>
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        private bool FilterStatistic(Playerstatistic stat)
        {
            return populationData != null &&
                stat.Numberofplayers >= populationData.PlayersFrom && stat.Numberofplayers <= populationData.PlayersTo &&
                stat.Time >= populationData.StartDate && stat.Time <= populationData.EndDate;
        }
    }
}