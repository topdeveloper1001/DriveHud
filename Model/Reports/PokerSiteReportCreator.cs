using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.Reports
{
    public class PokerSiteReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null)
            {
                return report;
            }

            foreach (var group in statistics.Where(x => !x.IsTourney).GroupBy(x => x.PokersiteId))
            {
                Indicators stat = new Indicators();
                foreach (var playerstatistic in group)
                    stat.AddStatistic(playerstatistic);

                report.Add(stat);
            }

            return report;
        }
    }

    public class TournamentPokerSiteReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null)
            {
                return report;
            }

            var player = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(player.Name, (short)player.PokerSite);

            foreach (var group in tournaments.GroupBy(x => x.SiteId))
            {
                TournamentReportRecord stat = new TournamentReportRecord();
                foreach (var playerstatistic in statistics.Where(x => x.IsTourney && group.Any(g => g.Tourneynumber == x.TournamentId)))
                {
                    stat.AddStatistic(playerstatistic);
                }
                if (!stat.Statistics.Any()) continue;

                stat.SetWinning(group.Sum(x => x.Winningsincents));
                stat.Started = group.Min(x => x.Firsthandtimestamp);

                report.Add(stat);
            }

            return report;
        }
    }
}
