//-----------------------------------------------------------------------
// <copyright file="OpponentReportIndicators.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.Data
{
    [ProtoContract]
    [ProtoInclude(100, typeof(PopulationReportIndicators))]
    public class OpponentReportIndicators : ReportIndicators
    {
        private const int handsToStore = 100;

        [ProtoMember(1)]
        private List<ShrinkedStatistic> shrinkedStatistics = new List<ShrinkedStatistic>();

        [ProtoMember(2)]
        public int PlayerId { get; set; }

        public bool HasAllHands
        {
            get
            {
                return ReportHands != null && ReportHands.Count < handsToStore;
            }
        }

        public override void AddStatistic(Playerstatistic statistic)
        {
            base.AddStatistic(statistic);
        }

        public void ShrinkReportHands()
        {
            if (ReportHands == null)
            {
                return;
            }

            ReportHands = new ObservableCollection<ReportHandViewModel>(ReportHands.OrderByDescending(x => x.Time).Take(handsToStore));
        }

        protected override void AddStatToStatistic(Playerstatistic statistic)
        {
        }

        protected virtual void AddShrinkedStatistic(Playerstatistic statistic)
        {
            var shrinkedStat = new ShrinkedStatistic(statistic);
            shrinkedStatistics.Add(shrinkedStat);
        }

        protected override void CalculateStdDeviation()
        {
            if (shrinkedStatistics == null || shrinkedStatistics.Count == 0)
            {
                return;
            }

            var statistic = shrinkedStatistics.ToArray();

            var netWonCollection = statistic
                .Select(x => new NetWonDeviationItem
                {
                    NetWon = x.NetWon,
                    NetWonInBB = GetDivisionResult(x.NetWon, x.BigBlind)
                })
                .ToArray();

            CalculateStdDeviation(netWonCollection);
        }

        protected override decimal CalculateNetWonPerHour()
        {
            if (shrinkedStatistics == null || shrinkedStatistics.Count == 0)
            {
                return 0m;
            }

            var orderedTime = shrinkedStatistics.OrderBy(x => x.Time).Select(x => x.Time).ToArray();

            var netWonPerHour = CalculateNetWonPerHour(orderedTime);

            return netWonPerHour;
        }

        [ProtoContract]
        private class ShrinkedStatistic
        {
            private ShrinkedStatistic()
            {
            }

            public ShrinkedStatistic(Playerstatistic statistic)
            {
                NetWon = statistic.NetWon;
                BigBlind = statistic.BigBlind;
                Time = statistic.Time;
            }

            [ProtoMember(1)]
            public decimal NetWon { get; set; }

            [ProtoMember(2)]
            public decimal BigBlind { get; set; }

            [ProtoMember(3)]
            public DateTime Time { get; set; }
        }
    }
}