using Microsoft.Practices.ServiceLocation;
using Model.Data;
using DriveHUD.Entities;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Reports
{
    public class TournamentReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null)
            {
                return report;
            }

            string playerName = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(playerName);

            foreach (var tournament in tournaments)
            {
                TournamentReportRecord stat = new TournamentReportRecord();
                foreach (var playerstatistic in statistics.Where(x => x.IsTourney && tournament.Tourneynumber == x.TournamentId))
                {
                    stat.AddStatistic(playerstatistic);
                }
                if (!stat.Statistcs.Any()) continue;

                stat.PlayerName = tournament.PlayerName;
                stat.TournamentId = tournament.Tourneynumber;

                stat.SetBuyIn(tournament.Buyinincents);
                stat.SetTotalBuyIn(tournament.Buyinincents);
                stat.SetRake(tournament.Rakeincents);
                stat.SetRebuy(tournament.Rebuyamountincents);
                stat.SetTableType(tournament.Tourneytagscsv);
                stat.SetSpeed(tournament.SpeedtypeId);
                stat.SetGameType(tournament.PokergametypeId);
                stat.SetTournamentLength(tournament.Firsthandtimestamp, tournament.Lasthandtimestamp);
                stat.SetWinning(tournament.Winningsincents);

                stat.Started = tournament.Firsthandtimestamp;
                stat.FinishPosition = tournament.Finishposition;
                stat.TableSize = tournament.Tablesize;
                report.Add(stat);
            }

            return report;
        }
    }
}