//-----------------------------------------------------------------------
// <copyright file="TournamentReportCreator.cs" company="Ace Poker Solutions">
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Model.Reports
{
    public class TournamentReportCreator : TournamentBaseReportCreator<TournamentReportRecord>
    {
        private IList<Tournaments> tournaments;

        public override ObservableCollection<ReportIndicators> Create(List<Playerstatistic> statistics, CancellationToken cancellationToken, bool forceRefresh = false)
        {            
            var player = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;

            tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(player?.PlayerIds);

            return base.Create(statistics, cancellationToken, forceRefresh);
        }

        protected override List<TournamentReportRecord> CombineChunkedIndicators(BlockingCollection<TournamentReportRecord> chunkedIndicators, CancellationToken cancellationToken)
        {
            var reports = new List<TournamentReportRecord>();

            foreach (var chunkedIndicatorsGroup in chunkedIndicators.GroupBy(x => new { x.TournamentId, x.PokerSiteId }))
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
            if (tournaments == null)
            {
                return;
            }

            foreach (var tournament in tournaments)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var reportRecord = new TournamentReportRecord();

                foreach (var playerstatistic in statistics
                    .Where(x => tournament.Tourneynumber == x.TournamentId && tournament.SiteId == x.PokersiteId))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    reportRecord.AddStatistic(playerstatistic);
                }

                if (reportRecord.StatisticsCount == 0)
                {
                    continue;
                }

                reportRecord.PlayerName = tournament.PlayerName;
                reportRecord.TournamentId = tournament.Tourneynumber;
                reportRecord.PokerSiteId = tournament.SiteId;

                reportRecord.SetBuyIn(tournament.Buyinincents);
                reportRecord.SetTotalBuyIn(tournament.Buyinincents);
                reportRecord.SetRake(tournament.Rakeincents);
                reportRecord.SetRebuy(tournament.Rebuyamountincents);
                reportRecord.SetTableType(tournament.Tourneytagscsv);
                reportRecord.SetSpeed(tournament.SpeedtypeId);
                reportRecord.SetGameType(tournament.PokergametypeId);
                reportRecord.SetTournamentLength(tournament.Firsthandtimestamp, tournament.Lasthandtimestamp);
                reportRecord.SetWinning(tournament.Winningsincents);

                reportRecord.Started = tournament.Firsthandtimestamp;
                reportRecord.FinishPosition = tournament.Finishposition;
                reportRecord.TableSize = tournament.Tablesize;

                chunkedIndicators.Add(reportRecord);
            }
        }
    }
}