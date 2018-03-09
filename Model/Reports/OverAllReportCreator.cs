//-----------------------------------------------------------------------
// <copyright file="OverAllReportCreator.cs" company="Ace Poker Solutions">
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
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Interfaces;
using NHibernate.Util;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.Reports
{
    /// <summary>
    /// Report will all basic indicators. No grouping
    /// </summary>
    public class OverAllReportCreator : CashBaseReportCreator
    {
        public override ObservableCollection<ReportIndicators> Create(IList<Playerstatistic> statistics, bool forceRefresh = false)
        {
            var report = new ObservableCollection<ReportIndicators>();

            if (statistics == null || !statistics.Any())
            {
                return report;
            }

            var stat = new ReportIndicators();

            foreach (var playerstatistic in statistics.ToArray())
            {
                stat.AddStatistic(playerstatistic);
            }

            report.Add(stat);

            return report;
        }
    }

    public class TournamentOverAllReportCreator : TournamentBaseReportCreator
    {
        public override ObservableCollection<ReportIndicators> Create(IList<Playerstatistic> statistics, bool forceRefresh = false)
        {
            var report = new ObservableCollection<ReportIndicators>();

            if (statistics == null || !statistics.Any())
            {
                return report;
            }

            var player = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;

            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(player?.PlayerIds);

            foreach (var group in tournaments.GroupBy(x => new { x.Buyinincents, x.Tourneytagscsv, x.SpeedtypeId, x.PokergametypeId }))
            {
                var stat = new TournamentReportRecord();

                var stats = statistics.Where(x => group.Any(g => g.Tourneynumber == x.TournamentId)).ToArray();

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