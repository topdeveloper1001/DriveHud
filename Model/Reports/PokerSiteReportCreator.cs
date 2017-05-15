//-----------------------------------------------------------------------
// <copyright file="PokerSiteReportCreator.cs" company="Ace Poker Solutions">
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

            foreach (var group in statistics.Where(x => !x.IsTourney).GroupBy(x => x.PokersiteId).ToArray())
            {
                var stat = new ReportIndicators();

                foreach (var playerstatistic in group)
                {
                    stat.AddStatistic(playerstatistic);
                }

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
                var stat = new TournamentReportRecord();

                foreach (var playerstatistic in statistics.Where(x => x.IsTourney && group.Any(g => g.Tourneynumber == x.TournamentId)).ToArray())
                {
                    stat.AddStatistic(playerstatistic);
                }

                if (stat.StatisticsCount == 0)
                {
                    continue;
                }

                stat.SetWinning(group.Sum(x => x.Winningsincents));
                stat.Started = group.Min(x => x.Firsthandtimestamp);

                report.Add(stat);
            }

            return report;
        }
    }
}