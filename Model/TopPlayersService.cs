using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Events.FilterEvents;
using Model.Interfaces;
using Prism.Events;

namespace Model
{
    public class TopPlayersService : ITopPlayersService
    {
        private readonly IDataService dataService;
        private readonly IList<HandHistoryRecord> handHistoryRecords;
        private readonly IList<Playerstatistic> topCollection;
        private CancellationTokenSource cancellationTokenSource;
        private Task updateTopTask;
        private decimal minNetWon;
        private Expression<Func<Playerstatistic, bool>> currentFilter;
        private readonly IEventAggregator eventAggregator;


        public int TopCount { get; set; }


        public TopPlayersService()
        {
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<UpdateReportEvent>().Subscribe(OnUpdateReport, ThreadOption.BackgroundThread);
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
            topCollection = new List<Playerstatistic>();
            TopCount = 50;
            handHistoryRecords = dataService.GetHandHistoryRecords();
            minNetWon = decimal.MinValue;
        }

        private async Task<CancellationTokenSource> StopBuildingReport()
        {
            var previousCts = cancellationTokenSource;
            var newCts = new CancellationTokenSource();
            cancellationTokenSource = newCts;
            if (previousCts == null)
                return newCts;
            previousCts.Cancel();
            try
            {
                await updateTopTask;
            }
            catch
            {
                topCollection.Clear();
            }
            return newCts;
        }

        private async void OnUpdateReport(Expression<Func<Playerstatistic, bool>> filterPredicate)
        {
            if (filterPredicate == currentFilter)
                return;
            currentFilter = filterPredicate;
            var newCts = await StopBuildingReport();
            newCts.Token.ThrowIfCancellationRequested();
            minNetWon = decimal.MinValue;
            topCollection.Clear();
            updateTopTask = UpdateTop(filterPredicate, newCts.Token);
            await updateTopTask;
        }

        private Task UpdateTop(Expression<Func<Playerstatistic, bool>> filterPredicate,
            CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    var playerCount = topCollection.Count;
                    var orderedHandHistories =
                        handHistoryRecords.GroupBy(
                                x => new {PlayerName = x.Player.Playername, PokersiteId = x.Player.PokersiteId})
                            .Select(
                                x =>
                                    new
                                    {
                                        PlayerName = x.Key.PlayerName,
                                        PokersiteId = x.Key.PokersiteId,
                                        Wins = x.Sum(y => y.NetWonInCents)
                                    })
                            .OrderByDescending(x => x.Wins)
                            .ToList();
                    foreach (var player in orderedHandHistories)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (
                            topCollection.Any(
                                x => x.PlayerName == player.PlayerName && x.PokersiteId == player.PokersiteId))
                            continue;
                        if (player.Wins < minNetWon && playerCount >= TopCount)
                            break;
                        var allStats =
                            dataService.GetPlayerStatisticFromFile(player.PlayerName, player.PokersiteId)
                                .AsQueryable()
                                .Where(x => !x.IsTourney);
                        var stats = allStats.Where(filterPredicate).ToList();
                        if (!stats.Any())
                            continue;
                        var netWon = stats.Sum(x => x.NetWon);
                        if (!topCollection.Any() || playerCount < TopCount)
                        {
                            if (netWon < minNetWon || !topCollection.Any())
                                minNetWon = netWon;
                            topCollection.AddRange(stats);
                            //_minNetWon =
                            //    _topCollection.GroupBy(x => new {x.PlayerId, x.PokersiteId})
                            //        .Select(x => x.Sum(p => p.NetWon))
                            //        .Min();
                            playerCount++;
                            continue;
                        }
                        if (netWon < minNetWon)
                            continue;
                        var lower =
                            topCollection.GroupBy(x => new {x.PlayerName, PokersiteId = (short) x.PokersiteId})
                                .OrderBy(x => x.Sum(p => p.NetWon))
                                .FirstOrDefault()?.Key;
                        if (lower == null)
                            continue;
                        topCollection.RemoveByCondition(
                            x => x.PlayerName == lower.PlayerName && x.PokersiteId == lower.PokersiteId);
                        topCollection.AddRange(stats);
                        minNetWon = netWon;
                    }
                }
                catch (OperationCanceledException)
                {
                    topCollection.Clear();
                }
            }, cancellationToken);
        }

        public async Task<IList<Playerstatistic>> GetTop()
        {
            return await Task.Run(() =>
            {
                if (updateTopTask.Status == TaskStatus.Running)
                    updateTopTask.Wait();
                return topCollection;
            });
        }

        public async void UpdateStatistics(IList<Playerstatistic> playersStats)
        {
            eventAggregator.GetEvent<OpponentAnalysisBuildingEvent>().Publish();
            var newCts = await StopBuildingReport();
            var grouppedStats =
                playersStats.AsQueryable()
                    .Where(x => !x.IsTourney)
                    .Where(currentFilter)
                    .GroupBy(x => new {x.PlayerName, x.PokersiteId})
                    .Select(x => new {x.Key, Sum = x.Sum(p => p.NetWon)});
            foreach (var stat in grouppedStats)
            {
                var handHistory =
                    handHistoryRecords.FirstOrDefault(
                        h => h.Player.Playername == stat.Key.PlayerName && h.Player.PokersiteId == stat.Key.PokersiteId);
                var wins = stat.Sum;
                if (handHistory == null)
                {
                    handHistoryRecords.Add(new HandHistoryRecord
                    {
                        Player =
                            new Players {Playername = stat.Key.PlayerName, PokersiteId = (short) stat.Key.PokersiteId},
                        NetWonInCents = stat.Sum
                    });
                }
                else
                    wins =
                        handHistoryRecords.Where(
                            h =>
                                h.Player.Playername == stat.Key.PlayerName &&
                                h.Player.PokersiteId == stat.Key.PokersiteId).Sum(y => y.NetWonInCents);
                var playerInTop =
                    topCollection.Where(
                        x => x.PlayerName == stat.Key.PlayerName && x.PokersiteId == stat.Key.PokersiteId).ToList();
                if (playerInTop.Any())
                {
                    if (playerInTop.Sum(p => p.NetWon) + wins < minNetWon)
                    {
                        topCollection.RemoveByCondition(
                            x => x.PlayerName == stat.Key.PlayerName && x.PokersiteId == stat.Key.PokersiteId);
                    }
                    else
                    {
                        topCollection.AddRange(
                            playersStats.AsQueryable()
                                .Where(x => !x.IsTourney)
                                .Where(currentFilter)
                                .Where(p => p.PlayerName == stat.Key.PlayerName && p.PokersiteId == stat.Key.PokersiteId));
                    }
                }
                else if (wins > minNetWon)
                {
                    topCollection.AddRange(
                        playersStats.AsQueryable()
                            .Where(x => !x.IsTourney)
                            .Where(currentFilter)
                            .Where(p => p.PlayerName == stat.Key.PlayerName && p.PokersiteId == stat.Key.PokersiteId));
                }
            }
            await ArrangeTopCount(newCts);
            eventAggregator.GetEvent<OpponentAnalysisBuildedEvent>().Publish();
        }

        private async Task ArrangeTopCount(CancellationTokenSource cts)
        {
            var sortedTopList =
                topCollection.GroupBy(x => new {x.PlayerName, x.PokersiteId})
                    .OrderByDescending(x => x.Sum(p => p.NetWon))
                    .Select((x, index) => new {x.Key.PlayerName, x.Key.PokersiteId, Index = index})
                    .ToList();
            if (sortedTopList.Count > TopCount)
            {
                foreach (var outOfRangePlayer in sortedTopList.Where(s => s.Index >= TopCount))
                {
                    topCollection.RemoveByCondition(
                        x =>
                            x.PlayerName == outOfRangePlayer.PlayerName &&
                            x.PokersiteId == outOfRangePlayer.PokersiteId);
                }
            }
            else if (sortedTopList.Count < TopCount)
            {
                cts.Token.ThrowIfCancellationRequested();
                updateTopTask = UpdateTop(currentFilter, cts.Token);
                await updateTopTask;
            }
        }
    }
}