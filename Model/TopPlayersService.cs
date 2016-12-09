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


        public int TopCount { get; set; }


        public TopPlayersService()
        {
            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<UpdateReportEvent>().Subscribe(OnUpdateReport, ThreadOption.BackgroundThread);
            _dataService = ServiceLocator.Current.GetInstance<IDataService>();
            _topCollection = new List<Playerstatistic>();
            TopCount = 50;
            _handHistoryRecords = _dataService.GetHandHistoryRecords();
            _minNetWon = decimal.MinValue;
        }

        private async void OnUpdateReport(Expression<Func<Playerstatistic, bool>> filterPredicate)
        {
            if (filterPredicate == _currentFilter)
                return;
            _currentFilter = filterPredicate;
            var previousCts = _cancellationTokenSource;
            var newCts = new CancellationTokenSource();
            _cancellationTokenSource = newCts;
            if (previousCts != null)
            {
                previousCts.Cancel();
                try
                {
                    await _updateTopTask;
                }
                catch
                {
                    _topCollection.Clear();
                }
            }
            newCts.Token.ThrowIfCancellationRequested();
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
                    _topCollection.Clear();
                    var playerCount = 0;
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
    }
}