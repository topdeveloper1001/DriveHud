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

using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.Reports
{
    public class TournamentReportCreator : TournamentBaseReportCreator
    {
        public override ObservableCollection<ReportIndicators> Create(IList<Playerstatistic> statistics, bool forceRefresh = false)
        {
            var report = new ObservableCollection<ReportIndicators>();

            if (statistics == null || statistics.Count == 0)
            {
                return report;
            }

            var player = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;

            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(player?.PlayerIds);

            foreach (var tournament in tournaments)
            {
                var reportRecord = new TournamentReportRecord();

                foreach (var playerstatistic in statistics
                    .Where(x => tournament.Tourneynumber == x.TournamentId && tournament.SiteId == x.PokersiteId)
                    .ToArray())
                {
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
                report.Add(reportRecord);
            }

            return report;
        }
    }
}