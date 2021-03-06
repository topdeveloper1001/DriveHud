﻿//-----------------------------------------------------------------------
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
using Model.Enums;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.ChartData
{
    public abstract class TournamentChartDataBase : ITournamentChartData
    {
        public virtual IEnumerable<TournamentReportRecord> Create(IList<Tournaments> tournaments, TournamentChartFilterType tournamentChartFilterType)
        {
            var report = new List<TournamentReportRecord>();

            if (tournaments == null || tournaments.Count == 0)
            {
                return report;
            }

            var firstDate = GetFirstDate(tournaments.Max(x => x.Firsthandtimestamp));

            var groupedTournaments = tournaments
                .Where(x => x.Firsthandtimestamp >= firstDate &&
                    (tournamentChartFilterType == TournamentChartFilterType.All ||
                        tournamentChartFilterType == TournamentChartFilterType.MTT && x.Tourneytagscsv == TournamentsTags.MTT.ToString() ||
                        tournamentChartFilterType == TournamentChartFilterType.STT && x.Tourneytagscsv == TournamentsTags.STT.ToString()))
                .OrderBy(x => x.Firsthandtimestamp)
                .GroupBy(x => BuildGroupedDateKey(x));

            TournamentReportRecord previousRecord = null;

            foreach (var group in groupedTournaments)
            {
                var reportRecord = new TournamentReportRecord();

                reportRecord.Started = CreateDateTimeFromDateKey(group.Key);
                reportRecord.SetTotalBuyIn(group.Sum(x => x.Buyinincents));
                reportRecord.SetRake(group.Sum(x => x.Rakeincents));
                reportRecord.SetRebuy(group.Sum(x => x.Rebuyamountincents));
                reportRecord.SetWinning(group.Sum(x => x.Winningsincents));

                reportRecord.TournamentsInPrizes = group.Where(x => x.Winningsincents > 0).Count();
                reportRecord.TournamentsPlayed = group.Count();

                if (previousRecord != null)
                {
                    reportRecord.TotalBuyIn += previousRecord.TotalBuyIn;
                    reportRecord.Rake += previousRecord.Rake;
                    reportRecord.Rebuy += previousRecord.Rebuy;
                    reportRecord.Won += previousRecord.Won;
                    reportRecord.TournamentsInPrizes += previousRecord.TournamentsInPrizes;
                    reportRecord.TournamentsPlayed += previousRecord.TournamentsPlayed;
                }

                report.Add(reportRecord);

                previousRecord = reportRecord;
            }

            return report;
        }

        public abstract DateTime GetFirstDate(DateTime maxDateTime);

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

    public class WeekTournamentChartData : TournamentChartDataBase, ITournamentChartData
    {
        public override DateTime GetFirstDate(DateTime maxDateTime)
        {
            if (DateTime.MinValue.AddDays(7) >= maxDateTime)
            {
                return DateTime.MinValue;
            }

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
        public override DateTime GetFirstDate(DateTime maxDateTime)
        {
            if (DateTime.MinValue.AddMonths(1) >= maxDateTime)
            {
                return DateTime.MinValue;
            }

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
        public override DateTime GetFirstDate(DateTime maxDateTime)
        {
            if (DateTime.MinValue.AddYears(1) >= maxDateTime)
            {
                return DateTime.MinValue;
            }

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