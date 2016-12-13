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
        private readonly IDataService _dataService;
        private readonly IList<HandHistoryRecord> _handHistoryRecords;
        private readonly IList<Playerstatistic> _topCollection;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _updateTopTask;
        private decimal _minNetWon;
        private Expression<Func<Playerstatistic, bool>> _currentFilter;
        private readonly IEventAggregator _eventAggregator;


        public int TopCount { get; set; }


        public TopPlayersService()
        {
            _eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _eventAggregator.GetEvent<UpdateReportEvent>().Subscribe(OnUpdateReport, ThreadOption.BackgroundThread);
            _dataService = ServiceLocator.Current.GetInstance<IDataService>();
            _topCollection = new List<Playerstatistic>();
            TopCount = 50;
            _handHistoryRecords = _dataService.GetHandHistoryRecords();
            _minNetWon = decimal.MinValue;
        }

        private async Task<CancellationTokenSource> StopBuildingReport()
        {
            var previousCts = _cancellationTokenSource;
            var newCts = new CancellationTokenSource();
            _cancellationTokenSource = newCts;
            if (previousCts == null)
                return newCts;
            previousCts.Cancel();
            try
            {
                await _updateTopTask;
            }
            catch
            {
                _topCollection.Clear();
            }
            return newCts;
        }

        private async void OnUpdateReport(Expression<Func<Playerstatistic, bool>> filterPredicate)
        {
            if (filterPredicate == _currentFilter)
                return;
            _currentFilter = filterPredicate;
            var newCts = await StopBuildingReport();
            newCts.Token.ThrowIfCancellationRequested();
            _minNetWon = decimal.MinValue;
            _topCollection.Clear();
            _updateTopTask = UpdateTop(filterPredicate, newCts.Token);
            await _updateTopTask;
        }

        private Task UpdateTop(Expression<Func<Playerstatistic, bool>> filterPredicate,
            CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    var playerCount = _topCollection.Count;
                    var orderedHandHistories =
                        _handHistoryRecords.GroupBy(
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
                        if (_topCollection.Any(x=>x.PlayerName == player.PlayerName && x.PokersiteId == player.PokersiteId))
                        if (player.Wins < _minNetWon && playerCount >= TopCount)
                            break;
                        var stats =
                            _dataService.GetPlayerStatisticFromFile(player.PlayerName, player.PokersiteId)
                                .AsQueryable()
                                .Where(filterPredicate)
                                .ToList();
                        if (!stats.Any())
                            continue;
                        var netWon = stats.Sum(x => x.NetWon);
                        if (!_topCollection.Any() || playerCount < TopCount)
                        {
                            _topCollection.AddRange(stats);
                            _minNetWon =
                                _topCollection.GroupBy(x => new {x.PlayerId, x.PokersiteId})
                                    .Select(x => x.Sum(p => p.NetWon))
                                    .Min();
                            playerCount++;
                            continue;
                        }
                        if (netWon < _minNetWon)
                            continue;
                        var lower =
                            _topCollection.GroupBy(x => new {x.PlayerId, x.PokersiteId})
                                .OrderBy(x => x.Sum(p => p.NetWon))
                                .FirstOrDefault()?.Key;
                        if (lower == null)
                            continue;
                        {
                            _topCollection.RemoveByCondition(
                                x => x.PlayerId == lower.PlayerId && x.PokersiteId == lower.PokersiteId);
                            _topCollection.AddRange(stats);
                            _minNetWon = netWon;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _topCollection.Clear();
                }
            }, cancellationToken);
        }

        public async Task<IList<Playerstatistic>> GetTop()
        {
            return await Task.Run(() =>
            {
                if (_updateTopTask.Status == TaskStatus.Running)
                    _updateTopTask.Wait();
                return _topCollection;
            });
        }

        public async void UpdateStatistics(IList<Playerstatistic> playersStats)
        {
            _eventAggregator.GetEvent<OpponentAnalysisBuildingEvent>().Publish();
            var newCts = await StopBuildingReport();
            var grouppedStats =
                playersStats.AsQueryable().Where(_currentFilter).GroupBy(x => new {x.PlayerName, x.PokersiteId})
                    .Select(x => new {x.Key, Sum = x.Sum(p => p.NetWon)});
            foreach (var stat in grouppedStats)
            {
                var handHistory =
                    _handHistoryRecords.FirstOrDefault(
                        h => h.Player.Playername == stat.Key.PlayerName && h.Player.PokersiteId == stat.Key.PokersiteId);
                var wins = stat.Sum;
                if (handHistory == null)
                {
                    _handHistoryRecords.Add(new HandHistoryRecord
                    {
                        Player =
                            new Players {Playername = stat.Key.PlayerName, PokersiteId = (short) stat.Key.PokersiteId},
                        NetWonInCents = stat.Sum
                    });
                }
                else
                    wins =
                        _handHistoryRecords.Where(
                            h =>
                                h.Player.Playername == stat.Key.PlayerName &&
                                h.Player.PokersiteId == stat.Key.PokersiteId).Sum(y => y.NetWonInCents);
                var playerInTop =
                    _topCollection.Where(
                        x => x.PlayerName == stat.Key.PlayerName && x.PokersiteId == stat.Key.PokersiteId).ToList();
                if (playerInTop.Any())
                {
                    if (playerInTop.Sum(p => p.NetWon) + wins < _minNetWon)
                    {
                        _topCollection.RemoveByCondition(
                            x => x.PlayerName == stat.Key.PlayerName && x.PokersiteId == stat.Key.PokersiteId);
                    }
                    else
                    {
                        _topCollection.AddRange(playersStats.Where(p=>p.PlayerName == stat.Key.PlayerName && p.PokersiteId == stat.Key.PokersiteId));
                    }
                }
                else if (wins > _minNetWon)
                {
                    _topCollection.AddRange(playersStats.Where(p => p.PlayerName == stat.Key.PlayerName && p.PokersiteId == stat.Key.PokersiteId));
                }
            }
            await ArrangeTopCount(newCts);
            _eventAggregator.GetEvent<OpponentAnalysisBuildedEvent>().Publish();
        }

        private async Task ArrangeTopCount(CancellationTokenSource cts)
        {
            var sortedTopList =
                _topCollection.GroupBy(x => new {x.PlayerName, x.PokersiteId})
                    .OrderByDescending(x => x.Sum(p => p.NetWon))
                    .Select((x, index) => new {x.Key.PlayerName, x.Key.PokersiteId, Index = index}).ToList();
            if (sortedTopList.Count > TopCount)
            {
                foreach (var outOfRangePlayer in sortedTopList.Where(s=>s.Index >= TopCount))
                {
                    _topCollection.RemoveByCondition(
                        x =>
                            x.PlayerName == outOfRangePlayer.PlayerName &&
                            x.PokersiteId == outOfRangePlayer.PokersiteId);
                }
            }
            else if (sortedTopList.Count < TopCount)
            {
                cts.Token.ThrowIfCancellationRequested();
                _updateTopTask = UpdateTop(_currentFilter, cts.Token);
                await _updateTopTask;
            }
        }
    }
}