//-----------------------------------------------------------------------
// <copyright file="TournamentPokerSiteReportCreator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Model.Reports
{
    public class TournamentPokerSiteReportCreator : TournamentBaseReportCreator<TournamentReportRecord>
    {
        private Dictionary<short, TournamentGrouped> groupedTournaments;

        public override ObservableCollection<ReportIndicators> Create(List<Playerstatistic> statistics, CancellationToken cancellationToken, bool forceRefresh = false)
        {
            var player = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;

            groupedTournaments = ServiceLocator.Current.GetInstance<IDataService>()
                .GetPlayerTournaments(player?.PlayerIds)
                .GroupBy(x => x.SiteId)
                .ToDictionary(x => x.Key, x => new TournamentGrouped
                {
                    TournamentIds = new HashSet<string>(x.Select(t => t.Tourneynumber).Distinct()),
                    Started = x.Min(t => t.Firsthandtimestamp),
                    Winning = x.Sum(t => t.Winningsincents)
                });

            return base.Create(statistics, cancellationToken, forceRefresh);
        }

        protected override List<TournamentReportRecord> CombineChunkedIndicators(BlockingCollection<TournamentReportRecord> chunkedIndicators, CancellationToken cancellationToken)
        {
            var reports = new List<TournamentReportRecord>();

            foreach (var chunkedIndicatorsGroup in chunkedIndicators.GroupBy(x => x.PokerSiteId))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return reports;
                }

                var report = chunkedIndicatorsGroup.First();

                chunkedIndicatorsGroup.Skip(1).ForEach(r => report.AddIndicator(r));
                reports.Add(report);
            }

            return reports;
        }

        protected override void ProcessChunkedStatistic(List<Playerstatistic> statistics, BlockingCollection<TournamentReportRecord> chunkedIndicators, CancellationToken cancellationToken)
        {
            if (groupedTournaments == null)
            {
                return;
            }

            foreach (var tournaments in groupedTournaments)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var report = new TournamentReportRecord
                {
                    PokerSiteId = tournaments.Key
                };

                foreach (var playerstatistic in statistics.Where(x => tournaments.Key == x.PokersiteId && tournaments.Value.TournamentIds.Contains(x.TournamentId)))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    report.AddStatistic(playerstatistic);
                }

                if (report.TotalHands == 0)
                {
                    continue;
                }

                report.Started = tournaments.Value.Started;
                report.SetWinning(tournaments.Value.Winning);

                chunkedIndicators.Add(report);
            }
        }

        private class TournamentGrouped
        {
            public HashSet<string> TournamentIds { get; set; }

            public DateTime Started { get; set; }

            public int Winning { get; set; }
        }
    }
}