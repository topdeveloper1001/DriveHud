//-----------------------------------------------------------------------
// <copyright file="TournamentChartData.cs" company="Ace Poker Solutions">
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.ChartData
{
    public abstract class TournamentChartDataBase : ITournamentChartData
    {
        public virtual IEnumerable<TournamentReportRecord> Create()
        {
            var report = new List<TournamentReportRecord>();

            var player = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;

            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(player?.PlayerIds);

            if (tournaments == null || tournaments.Count == 0)
            {
                return report;
            }

            var firstDate = GetFirstDate(tournaments.Max(x => x.Firsthandtimestamp));

            var groupedTournaments = tournaments
                .Where(x => x.Firsthandtimestamp >= firstDate)
                .OrderBy(x => x.Firsthandtimestamp)
                .GroupBy(x => BuildGroupedDateKey(x));

            foreach (var group in groupedTournaments)
            {
                var stat = new TournamentReportRecord();

                stat.Started = CreateDateTimeFromDateKey(group.Key);

                var tournamentStats = tournaments
                    .Where(t => t.Firsthandtimestamp >= firstDate
                        && (t.Firsthandtimestamp <= stat.Started || group.Any(g => g.Tourneynumber == t.Tourneynumber))).
                    ToArray();

                stat.SetTotalBuyIn(tournamentStats.Sum(x => x.Buyinincents));
                stat.SetRake(tournamentStats.Sum(x => x.Rakeincents));
                stat.SetRebuy(tournamentStats.Sum(x => x.Rebuyamountincents));
                stat.SetWinning(tournamentStats.Sum(x => x.Winningsincents));

                stat.TournamentsInPrizes = tournamentStats.Where(x => x.Winningsincents > 0).Count();
                stat.TournamentsPlayed = tournamentStats.Length;

                report.Add(stat);
            }

            return report;
        }

        protected abstract DateTime GetFirstDate(DateTime maxDateTime);

        protected abstract GroupedDateKey BuildGroupedDateKey(Tournaments tournament);

        protected abstract DateTime CreateDateTimeFromDateKey(GroupedDateKey dateKey);

        protected class GroupedDateKey
        {
            public int Year { get; set; }

            public int Month { get; set; }

            public int Day { get; set; }

            public int Hour { get; set; }

            public override bool Equals(object obj)
            {
                var dateKey = obj as GroupedDateKey;

                if (dateKey == null)
                {
                    return false;
                }

                return Equals(dateKey);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + Year;
                    hashcode = (hashcode * 31) + Month;
                    hashcode = (hashcode * 31) + Day;
                    hashcode = (hashcode * 31) + Hour;
                    return hashcode;
                }
            }

            private bool Equals(GroupedDateKey dateKey)
            {
                return Year == dateKey.Year && Month == dateKey.Month && Day == dateKey.Day && Hour == dateKey.Hour;
            }
        }
    }

    public class DayTournamentChartData : TournamentChartDataBase, ITournamentChartData
    {
        protected override DateTime GetFirstDate(DateTime maxDateTime)
        {
            return maxDateTime.AddDays(-1);
        }

        protected override GroupedDateKey BuildGroupedDateKey(Tournaments tournament)
        {
            var dateKey = new GroupedDateKey
            {
                Year = tournament.Firsthandtimestamp.Year,
                Month = tournament.Firsthandtimestamp.Month,
                Day = tournament.Firsthandtimestamp.Day,
                Hour = tournament.Firsthandtimestamp.Hour
            };

            return dateKey;
        }

        protected override DateTime CreateDateTimeFromDateKey(GroupedDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, dateKey.Day, dateKey.Hour, 0, 0);
            return dateTime;
        }
    }

    public class WeekTournamentChartData : TournamentChartDataBase, ITournamentChartData
    {
        protected override DateTime GetFirstDate(DateTime maxDateTime)
        {
            return maxDateTime.AddDays(-7);
        }

        protected override GroupedDateKey BuildGroupedDateKey(Tournaments tournament)
        {
            var dateKey = new GroupedDateKey
            {
                Year = tournament.Firsthandtimestamp.Year,
                Month = tournament.Firsthandtimestamp.Month,
                Day = tournament.Firsthandtimestamp.Day
            };

            return dateKey;
        }

        protected override DateTime CreateDateTimeFromDateKey(GroupedDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, dateKey.Day);
            return dateTime;
        }
    }

    public class MonthTournamentChartData : TournamentChartDataBase, ITournamentChartData
    {
        protected override DateTime GetFirstDate(DateTime maxDateTime)
        {
            return maxDateTime.AddMonths(-1);
        }

        protected override GroupedDateKey BuildGroupedDateKey(Tournaments tournament)
        {
            var dateKey = new GroupedDateKey
            {
                Year = tournament.Firsthandtimestamp.Year,
                Month = tournament.Firsthandtimestamp.Month,
                Day = tournament.Firsthandtimestamp.Day
            };

            return dateKey;
        }

        protected override DateTime CreateDateTimeFromDateKey(GroupedDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, dateKey.Day);
            return dateTime;
        }
    }

    public class YearTournamentChartData : TournamentChartDataBase, ITournamentChartData
    {
        protected override DateTime GetFirstDate(DateTime maxDateTime)
        {
            return maxDateTime.AddYears(-1);
        }

        protected override GroupedDateKey BuildGroupedDateKey(Tournaments tournament)
        {
            var dateKey = new GroupedDateKey
            {
                Year = tournament.Firsthandtimestamp.Year,
                Month = tournament.Firsthandtimestamp.Month
            };

            return dateKey;
        }

        protected override DateTime CreateDateTimeFromDateKey(GroupedDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, 1);
            return dateTime;
        }
    }
}