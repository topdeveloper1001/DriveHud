using DriveHUD.Common.Linq;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Importer;
using Model.Interfaces;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ChartData
{
    public class DayTournamentChartData : ITournamentChartData
    {
        public IEnumerable<TournamentReportRecord> Create()
        {
            var report = new List<TournamentReportRecord>();

            string playerName = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(playerName);

            if (tournaments == null || tournaments.Count() == 0)
                return report;

            tournaments.ForEach(x => Converter.ToLocalizedDateTime(x.Firsthandtimestamp));

            var firstDate = tournaments.Max(x => x.Firsthandtimestamp).AddDays(-1);
            foreach (var group in tournaments.Where(x => x.Firsthandtimestamp >= firstDate).OrderBy(x => x.Firsthandtimestamp).GroupBy(x => new { x.Firsthandtimestamp.Year, x.Firsthandtimestamp.Month, x.Firsthandtimestamp.Day, x.Firsthandtimestamp.Hour }))
            {
                TournamentReportRecord stat = new TournamentReportRecord();
                stat.Started = new System.DateTime(group.Key.Year, group.Key.Month, group.Key.Day, group.Key.Hour, 0, 0);

                var tournamentStats = tournaments.Where(t => t.Firsthandtimestamp >= firstDate &&
                                                       (t.Firsthandtimestamp <= stat.Started || group.Any(g => g.Tourneynumber == t.Tourneynumber)));

                stat.SetTotalBuyIn(tournamentStats.Sum(x => x.Buyinincents));
                stat.SetRake(tournamentStats.Sum(x => x.Rakeincents));
                stat.SetRebuy(tournamentStats.Sum(x => x.Rebuyamountincents));
                stat.SetWinning(tournamentStats.Sum(x => x.Winningsincents));

                stat.TournamentsInPrizes = tournamentStats.Where(x => x.Winningsincents > 0).Count();
                stat.TournamentsPlayed = tournamentStats.Count();

                report.Add(stat);
            }

            return report;
        }
    }

    public class WeekTournamentChartData : ITournamentChartData
    {
        public IEnumerable<TournamentReportRecord> Create()
        {
            var report = new List<TournamentReportRecord>();

            string playerName = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(playerName);

            if (tournaments == null || tournaments.Count() == 0)
                return report;

            tournaments.ForEach(x => Converter.ToLocalizedDateTime(x.Firsthandtimestamp));

            var firstDate = tournaments.Max(x => x.Firsthandtimestamp).AddDays(-7);
            foreach (var group in tournaments.Where(x => x.Firsthandtimestamp >= firstDate).OrderBy(x => x.Firsthandtimestamp).GroupBy(x => new { x.Firsthandtimestamp.Year, x.Firsthandtimestamp.Month, x.Firsthandtimestamp.Day }))
            {
                TournamentReportRecord stat = new TournamentReportRecord();
                stat.Started = new System.DateTime(group.Key.Year, group.Key.Month, group.Key.Day);

                var tournamentStats = tournaments.Where(t => t.Firsthandtimestamp >= firstDate &&
                                                       (t.Firsthandtimestamp <= stat.Started || group.Any(g => g.Tourneynumber == t.Tourneynumber)));

                stat.SetTotalBuyIn(tournamentStats.Sum(x => x.Buyinincents));
                stat.SetRake(tournamentStats.Sum(x => x.Rakeincents));
                stat.SetRebuy(tournamentStats.Sum(x => x.Rebuyamountincents));
                stat.SetWinning(tournamentStats.Sum(x => x.Winningsincents));

                stat.TournamentsInPrizes = tournamentStats.Where(x => x.Winningsincents > 0).Count();
                stat.TournamentsPlayed = tournamentStats.Count();

                report.Add(stat);
            }

            return report;
        }
    }

    public class MonthTournamentChartData : ITournamentChartData
    {
        public IEnumerable<TournamentReportRecord> Create()
        {
            var report = new List<TournamentReportRecord>();

            string playerName = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(playerName);

            if (tournaments == null || tournaments.Count() == 0)
                return report;

            tournaments.ForEach(x => Converter.ToLocalizedDateTime(x.Firsthandtimestamp));

            var firstDate = tournaments.Max(x => x.Firsthandtimestamp).AddMonths(-1);
            foreach (var group in tournaments.Where(x => x.Firsthandtimestamp >= firstDate).OrderBy(x => x.Firsthandtimestamp).GroupBy(x => new { x.Firsthandtimestamp.Year, x.Firsthandtimestamp.Month, x.Firsthandtimestamp.Day }))
            {
                TournamentReportRecord stat = new TournamentReportRecord();
                stat.Started = new System.DateTime(group.Key.Year, group.Key.Month, group.Key.Day);

                var tournamentStats = tournaments.Where(t => t.Firsthandtimestamp >= firstDate &&
                                                       (t.Firsthandtimestamp <= stat.Started || group.Any(g => g.Tourneynumber == t.Tourneynumber)));

                stat.SetTotalBuyIn(tournamentStats.Sum(x => x.Buyinincents));
                stat.SetRake(tournamentStats.Sum(x => x.Rakeincents));
                stat.SetRebuy(tournamentStats.Sum(x => x.Rebuyamountincents));
                stat.SetWinning(tournamentStats.Sum(x => x.Winningsincents));

                stat.TournamentsInPrizes = tournamentStats.Where(x => x.Winningsincents > 0).Count();
                stat.TournamentsPlayed = tournamentStats.Count();

                report.Add(stat);
            }

            return report;
        }
    }

    public class YearTournamentChartData : ITournamentChartData
    {
        public IEnumerable<TournamentReportRecord> Create()
        {
            var report = new List<TournamentReportRecord>();

            string playerName = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(playerName);

            if (tournaments == null || tournaments.Count() == 0)
                return report;

            tournaments.ForEach(x => Converter.ToLocalizedDateTime(x.Firsthandtimestamp));

            var firstDate = tournaments.Max(x => x.Firsthandtimestamp).AddYears(-1);
            foreach (var group in tournaments.Where(x => x.Firsthandtimestamp >= firstDate).OrderBy(x => x.Firsthandtimestamp).GroupBy(x => new { x.Firsthandtimestamp.Year, x.Firsthandtimestamp.Month }))
            {
                TournamentReportRecord stat = new TournamentReportRecord();
                stat.Started = new System.DateTime(group.Key.Year, group.Key.Month, 1);

                var tournamentStats = tournaments.Where(t => t.Firsthandtimestamp >= firstDate &&
                                                       (t.Firsthandtimestamp <= stat.Started || group.Any(g => g.Tourneynumber == t.Tourneynumber)));

                stat.SetTotalBuyIn(tournamentStats.Sum(x => x.Buyinincents));
                stat.SetRake(tournamentStats.Sum(x => x.Rakeincents));
                stat.SetRebuy(tournamentStats.Sum(x => x.Rebuyamountincents));
                stat.SetWinning(tournamentStats.Sum(x => x.Winningsincents));

                stat.TournamentsInPrizes = tournamentStats.Where(x => x.Winningsincents > 0).Count();
                stat.TournamentsPlayed = tournamentStats.Count();

                report.Add(stat);
            }

            return report;
        }
    }
}

