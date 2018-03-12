//-----------------------------------------------------------------------
// <copyright file="PopulationReportIndicators.cs" company="Ace Poker Solutions">
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
using Model.Hud;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.Data
{
    [ProtoContract]
    public class PopulationReportIndicators : OpponentReportIndicators
    {
        private const int handsToStore = 1000;

        private SortedDictionary<ReportHandKey, ReportHandViewModel> reportHands = new SortedDictionary<ReportHandKey, ReportHandViewModel>(new ReportHandKeyComparer());

        private HashSet<long> processedHands = new HashSet<long>();

        [ProtoMember(1)]
        public string PlayerTypeName { get; set; }

        public HudPlayerType PlayerType { get; set; }

        protected override void AddReportHand(Playerstatistic statistic)
        {
            if (!CanAddHands)
            {
                return;
            }

            if (processedHands.Contains(statistic.GameNumber))
            {
                return;
            }

            processedHands.Add(statistic.GameNumber);

            if (reportHands.Count >= handsToStore)
            {
                reportHands.Remove(reportHands.Keys.First());
            }

            var reportHandKey = new ReportHandKey
            {
                GameNumber = statistic.GameNumber,
                Time = statistic.Time
            };

            reportHands.Add(reportHandKey, new ReportHandViewModel(statistic));
        }

        public bool CanAddHands { get; set; } = true;

        public void PrepareHands()
        {
            if (!CanAddHands)
            {
                return;
            }

            ReportHands = new ObservableCollection<ReportHandViewModel>(reportHands.Values.Reverse());

            processedHands.Clear();
            reportHands.Clear();
        }

        public void PrepareHands(IEnumerable<Playerstatistic> statistic)
        {
            if (statistic == null)
            {
                return;
            }

            ReportHands = new ObservableCollection<ReportHandViewModel>(statistic.OrderByDescending(x => x.Time).Take(handsToStore).Select(x => new ReportHandViewModel(x)));
        }

        private class ReportHandKey
        {
            public long GameNumber { get; set; }

            public DateTime Time { get; set; }

            public override int GetHashCode()
            {
                return GameNumber.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as ReportHandKey);
            }

            private bool Equals(ReportHandKey reportHandKey)
            {
                return reportHandKey != null && reportHandKey.GameNumber == GameNumber;
            }
        }

        private class ReportHandKeyComparer : IComparer<ReportHandKey>
        {
            public int Compare(ReportHandKey x, ReportHandKey y)
            {
                return x.Time != y.Time ? x.Time.CompareTo(y.Time) : x.GameNumber.CompareTo(y.GameNumber);
            }
        }
    }
}