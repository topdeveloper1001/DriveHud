using DriveHUD.Entities;
using Model.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Replayer
{
    public static class ReplayerHelpers
    {
        /// <summary>
        /// Selects last 30 hands that meet requirements ( Pot > 6 BB; is VPIP) from the same session as specified statistic 
        /// </summary>
        /// <param name="statistics"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public static IEnumerable<ReplayerDataModel> CreateSessionHandsList(IEnumerable<Playerstatistic> statistics, Playerstatistic current)
        {
            IEnumerable<Playerstatistic> potStat = new List<Playerstatistic>();
            if (statistics != null && statistics.Count() > 0)
            {
                if (current.IsTourney)
                {
                    potStat = statistics.Where(x => x.TournamentId == current.TournamentId).OrderByDescending(x => x.Time).Take(30);
                }
                else
                {
                    var session = new SessionsReportCreator().Create(statistics.ToList()).Where(x => x.Statistics.Any(s => s.GameNumber == current.GameNumber));
                    if (session != null && session.Count() > 0)
                    {
                        potStat = session.FirstOrDefault().Statistics.OrderByDescending(x => x.Time).Take(30);
                    }
                }
            }

            var result = potStat.Where(x => x.Pot > (6 * x.BigBlind) && x.Vpiphands > 0).Select(x => new ReplayerDataModel(x));

            return result;
        }

    }
}
