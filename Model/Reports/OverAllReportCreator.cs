using System.Collections.Generic;
using System.Collections.ObjectModel;
using Model.Data;
using DriveHUD.Entities;
using NHibernate.Util;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Model.Interfaces;
using Model.Enums;
using System;
using HandHistories.Objects.GameDescription;

namespace Model.Reports
{
    /// <summary>
    /// Report will all basic indicators. No grouping
    /// </summary>
    public class OverAllReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null || !statistics.Any())
            {
                return report;
            }

            var stat = new Indicators();

            foreach (var playerstatistic in statistics.Where(x => !x.IsTourney))
            {
                stat.AddStatistic(playerstatistic);
            }

            report.Add(stat);

            return report;
        }
    }

    public class TournamentOverAllReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null || !statistics.Any())
            {
                return report;
            }

            var player = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(player.Name, (short)player.PokerSite);

            foreach (var group in tournaments.GroupBy(x => new { x.Buyinincents, x.Tourneytagscsv, x.SpeedtypeId, x.PokergametypeId }))
            {
                var stat = new TournamentReportRecord();

                var stats = statistics.Where(x => x.IsTourney && group.Any(g => g.Tourneynumber == x.TournamentId)).ToArray();

                if (stats.Length == 0)
                {
                    continue;
                }

                foreach (var playerstatistic in stats)
                {
                    stat.AddStatistic(playerstatistic);
                }

                stat.Started = group.Min(x => x.Firsthandtimestamp);

                stat.SetBuyIn(group.Key.Buyinincents);
                stat.SetTotalBuyIn(group.Sum(x => x.Buyinincents));
                stat.SetRake(group.Sum(x => x.Rakeincents));
                stat.SetRebuy(group.Sum(x => x.Rebuyamountincents));
                stat.SetWinning(group.Sum(x => x.Winningsincents));

                stat.SetTableType(group.Key.Tourneytagscsv);
                stat.SetSpeed(group.Key.SpeedtypeId);
                stat.SetGameType(group.Key.PokergametypeId);

                stat.FinalTables = group.Count(x => (x.Tourneytables > 1) && (x.Finishposition != 0) && (x.Finishposition < x.Tablesize));
                stat.TournamentsInPrizes = group.Where(x => x.Winningsincents > 0).Count();
                stat.TournamentsPlayed = group.Count();

                report.Add(stat);
            }

            return report;
        }
    }
}