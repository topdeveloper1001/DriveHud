//-----------------------------------------------------------------------
// <copyright file="TournamentOverAllReportCreator.cs" company="Ace Poker Solutions">
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

namespace Model.Reports
{
    public class TournamentOverAllReportCreator : TournamentBaseReportCreator<TournamentReportRecord>
    {
        protected Dictionary<TournamentGroup, TournamentGrouped> groupedTournaments;

        public override ObservableCollection<ReportIndicators> Create(List<Playerstatistic> statistics, bool forceRefresh = false)
        {
            var player = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;

            if (player == null)
            {
                return new ObservableCollection<ReportIndicators>();
            }

            groupedTournaments = ServiceLocator.Current.GetInstance<IDataService>()
                .GetPlayerTournaments(player?.PlayerIds)
                .GroupBy(x => new TournamentGroup
                {
                    Buyinincents = x.Buyinincents,
                    Tourneytagscsv = x.Tourneytagscsv,
                    SpeedtypeId = x.SpeedtypeId,
                    PokergametypeId = x.PokergametypeId
                })
                .ToDictionary(x => x.Key, x => new TournamentGrouped
                {
                    TournamentIds = new HashSet<string>(x.Select(t => t.Tourneynumber).Distinct()),
                    Started = x.Min(t => t.Firsthandtimestamp),
                    TotalBuyIn = x.Sum(t => t.Buyinincents),
                    Rake = x.Sum(t => t.Rakeincents),
                    Rebuy = x.Sum(t => t.Rebuyamountincents),
                    Winning = x.Sum(t => t.Winningsincents),
                    FinalTables = x.Count(t => (t.Tourneytables > 1) && (t.Finishposition != 0) && (t.Finishposition < t.Tablesize)),
                    TournamentsInPrizes = x.Count(t => t.Winningsincents > 0),
                    TournamentsPlayed = x.Count()
                });

            return base.Create(statistics, forceRefresh);
        }

        protected override List<TournamentReportRecord> CombineChunkedIndicators(BlockingCollection<TournamentReportRecord> chunkedIndicators)
        {
            var reports = new List<TournamentReportRecord>();

            var groupedChunkedIndicators = chunkedIndicators.GroupBy(x => new
            {
                x.BuyIn,
                x.TableType,
                x.TournamentSpeed,
                x.GameType
            });

            foreach (var chunkedIndicatorsGroup in groupedChunkedIndicators)
            {
                var report = chunkedIndicatorsGroup.First();

                chunkedIndicatorsGroup.Skip(1).ForEach(r => report.AddIndicator(r));
                reports.Add(report);
            }

            return reports;
        }

        protected override void ProcessChunkedStatistic(List<Playerstatistic> statistics, BlockingCollection<TournamentReportRecord> chunkedIndicators)
        {
            if (groupedTournaments == null)
            {
                return;
            }

            foreach (var tournaments in groupedTournaments)
            {
                var report = new TournamentReportRecord();

                foreach (var playerstatistic in statistics.Where(x => tournaments.Value.TournamentIds.Contains(x.TournamentId)))
                {
                    report.AddStatistic(playerstatistic);
                }

                if (report.TotalHands == 0)
                {
                    continue;
                }

                report.SetBuyIn(tournaments.Key.Buyinincents);
                report.SetTableType(tournaments.Key.Tourneytagscsv);
                report.SetSpeed(tournaments.Key.SpeedtypeId);
                report.SetGameType(tournaments.Key.PokergametypeId);

                report.Started = tournaments.Value.Started;
                report.SetTotalBuyIn(tournaments.Value.TotalBuyIn);
                report.SetRake(tournaments.Value.Rake);
                report.SetRebuy(tournaments.Value.Rebuy);
                report.SetWinning(tournaments.Value.Winning);
                report.FinalTables = tournaments.Value.FinalTables;
                report.TournamentsInPrizes = tournaments.Value.TournamentsInPrizes;
                report.TournamentsPlayed = tournaments.Value.TournamentsPlayed;

                chunkedIndicators.Add(report);
            }
        }

        protected class TournamentGroup
        {
            public int Buyinincents { get; set; }

            public string Tourneytagscsv { get; set; }

            public short SpeedtypeId { get; set; }

            public short PokergametypeId { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as TournamentGroup);
            }

            private bool Equals(TournamentGroup tournamentGroup)
            {
                return tournamentGroup != null && tournamentGroup.Buyinincents == Buyinincents &&
                    tournamentGroup.Tourneytagscsv == Tourneytagscsv &&
                    tournamentGroup.SpeedtypeId == SpeedtypeId &&
                    tournamentGroup.PokergametypeId == PokergametypeId;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = 23;
                    hash = hash * 31 + Buyinincents.GetHashCode();
                    hash = hash * 31 + Tourneytagscsv.GetHashCode();
                    hash = hash * 31 + SpeedtypeId.GetHashCode();
                    hash = hash * 31 + PokergametypeId.GetHashCode();

                    return hash;
                }
            }
        }

        protected class TournamentGrouped
        {
            public HashSet<string> TournamentIds { get; set; }

            public DateTime Started { get; set; }

            public int TotalBuyIn { get; set; }

            public int Rake { get; set; }

            public int Rebuy { get; set; }

            public int Winning { get; set; }

            public int FinalTables { get; set; }

            public int TournamentsInPrizes { get; set; }

            public int TournamentsPlayed { get; set; }
        }
    }
}