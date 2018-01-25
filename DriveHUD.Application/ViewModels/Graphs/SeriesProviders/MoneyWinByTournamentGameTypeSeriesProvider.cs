//-----------------------------------------------------------------------
// <copyright file="MoneyWinByTournamentGameTypeSeriesProvider.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Enums;
using Model.Filters;
using Model.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Graphs.SeriesProviders
{
    internal class MoneyWinByTournamentGameTypeSeriesProvider : IGraphSeriesProvider
    {
        private SingletonStorageModel StorageModel
        {
            get { return ServiceLocator.Current.TryResolve<SingletonStorageModel>(); }
        }

        public IEnumerable<GraphSerie> GetSeries()
        {
            var dataService = ServiceLocator.Current.GetInstance<IDataService>();

            var tournaments = dataService.GetPlayerTournaments(StorageModel.PlayerSelectedItem.PlayerIds);

            var filterModelManagerService = ServiceLocator.Current.GetInstance<IFilterModelManagerService>(FilterServices.Main.ToString());

            var dateFilter = filterModelManagerService.FilterModelCollection?.OfType<FilterDateModel>().FirstOrDefault();

            var filteredTournaments = dateFilter != null ? dateFilter.FilterTournaments(tournaments) : tournaments;

            var series = new List<GraphSerie>();

            var dataPoints = new Dictionary<string, GraphSerieDataPoint>();

            foreach (var tournament in filteredTournaments)
            {
                var gameType = (GameType)tournament.PokergametypeId;

                var tournamentType = $"{gameType} {tournament.Tourneytagscsv}";

                if (!dataPoints.ContainsKey(tournamentType))
                {
                    var dataPoint = new GraphSerieDataPoint
                    {
                        Category = tournamentType,
                    };

                    dataPoints.Add(tournamentType, dataPoint);

                    var serie = new GraphSerie
                    {
                        DataPoints = new ObservableCollection<GraphSerieDataPoint> { dataPoint },
                        Legend = tournamentType
                    };

                    series.Add(serie);
                }

                dataPoints[tournamentType].Value += (tournament.Winningsincents - tournament.Buyinincents - tournament.Rakeincents) / 100m;
            }

            return series;
        }

        public void Process(Playerstatistic statistic)
        {
        }
    }
}