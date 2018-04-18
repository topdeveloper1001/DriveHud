//-----------------------------------------------------------------------
// <copyright file="ShowdownHandsReportCreator.cs" company="Ace Poker Solutions">
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
using Model.Data;
using Model.HandAnalyzers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cards = HandHistories.Objects.Cards;

namespace Model.Reports
{
    public class ShowdownHandsReportCreator : CashBaseReportCreator<ShowdownHandsReportRecord>
    {
        private Lazy<HandAnalyzer> analyzer = new Lazy<HandAnalyzer>(() => new HandAnalyzer(HandAnalyzer.GetReportAnalyzers()));

        private HandAnalyzer Analyzer
        {
            get
            {
                return analyzer.Value;
            }
        }

        protected override List<ShowdownHandsReportRecord> CombineChunkedIndicators(BlockingCollection<ShowdownHandsReportRecord> chunkedIndicators)
        {
            var reports = new List<ShowdownHandsReportRecord>();

            foreach (var chunkedIndicatorsByCards in chunkedIndicators.GroupBy(x => x.ShowdownHand))
            {
                var report = chunkedIndicatorsByCards.First();

                chunkedIndicatorsByCards.Skip(1).ForEach(r => report.AddIndicator(r));
                reports.Add(report);
            }

            return reports;
        }

        protected override void ProcessChunkedStatistic(List<Playerstatistic> statistics, BlockingCollection<ShowdownHandsReportRecord> chunkedIndicators)
        {
            var showdownStatistic = statistics
                .Where(x => !string.IsNullOrEmpty(x.Cards) &&
                    Cards.CardGroup.Parse(x.Cards).Count() == 2 &&
                    !string.IsNullOrEmpty(x.Board) &&
                    Cards.BoardCards.FromCards(x.Board).Count == 5)
                .GroupBy(x => Analyzer.Analyze(Cards.CardGroup.Parse(x.Cards), Cards.BoardCards.FromCards(x.Board)));

            showdownStatistic = ShowdownHandsReportRecord.FilterHands(showdownStatistic);

            foreach (var showdownStatisticByCards in showdownStatistic)
            {
                if (showdownStatisticByCards.Key == null)
                {
                    continue;
                }

                var report = new ShowdownHandsReportRecord();

                foreach (var playerstatistic in showdownStatisticByCards)
                {
                    report.AddStatistic(playerstatistic);
                    report.ShowdownHand = showdownStatisticByCards.Key.GetRank();
                }

                chunkedIndicators.Add(report);
            }
        }

        protected override IEnumerable<ShowdownHandsReportRecord> OrderResult(IEnumerable<ShowdownHandsReportRecord> reports)
        {
            return reports.OrderBy(x => x.ShowdownHand);
        }
    }  
}