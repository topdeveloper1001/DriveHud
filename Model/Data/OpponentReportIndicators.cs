﻿//-----------------------------------------------------------------------
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
    public class OpponentReportIndicators : ReportIndicators
    {
        [ProtoMember(1)]
        public int PlayerId { get; set; }

        [ProtoMember(2)]
        private List<ShrinkedStatistic> shrinkedStatistics = new List<ShrinkedStatistic>();

        public override void AddStatistic(Playerstatistic statistic)
        {
            base.AddStatistic(statistic);
           
            var shrinkedStat = new ShrinkedStatistic(statistic);
            shrinkedStatistics.Add(shrinkedStat);
        }

        public void ShrinkReportHands(int number)
        {
            if (ReportHands == null || number < 1)
            {
                return;
            }

            ReportHands = new ObservableCollection<ReportHandViewModel>(ReportHands.OrderByDescending(x => x.Time).Take(number));
        }

        protected override void AddStatToStatistic(Playerstatistic statistic)
        {
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