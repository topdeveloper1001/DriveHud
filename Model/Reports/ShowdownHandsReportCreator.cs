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

using DriveHUD.Entities;
using Model.Data;
using Model.HandAnalyzers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cards = HandHistories.Objects.Cards;

namespace Model.Reports
{
    public class ShowdownHandsReportCreator : CashBaseReportCreator
    {
        public override ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null)
            {
                return report;
            }

            var analyzer = new HandAnalyzer(HandAnalyzer.GetReportAnalyzers());

            var s = statistics
                .Where(x => !string.IsNullOrEmpty(x.Cards) &&
                    Cards.CardGroup.Parse(x.Cards).Count() == 2 &&
                    !string.IsNullOrEmpty(x.Board) &&
                    Cards.BoardCards.FromCards(x.Board).Count == 5)
                .ToArray();

            var hands = s
                .GroupBy(x => analyzer.Analyze(Cards.CardGroup.Parse(x.Cards), Cards.BoardCards.FromCards(x.Board)))
                .ToList();

            if (hands == null || hands.Count == 0)
            {
                return report;
            }

            hands = ShowdownHandsReportRecord.FilterHands(hands).ToList();

            foreach (var group in hands)
            {
                if (group.Key == null)
                {
                    continue;
                }

                var stat = new ShowdownHandsReportRecord();

                foreach (var playerstatistic in group)
                {
                    stat.AddStatistic(playerstatistic);
                    stat.ShowdownHand = group.Key.GetRank();
                }

                report.Add(stat);
            }

            return report;
        }
    }

    public class TournamentShowdownHandsReportCreator : TournamentBaseReportCreator
    {
        public override ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();
            var analyzer = new HandAnalyzer(HandAnalyzer.GetReportAnalyzers());

            var s = statistics
                .Where(x => !string.IsNullOrEmpty(x.Cards) &&
                    Cards.CardGroup.Parse(x.Cards).Count() == 2 &&
                    !string.IsNullOrEmpty(x.Board) &&
                    Cards.BoardCards.FromCards(x.Board).Count == 5)
                .ToArray();

            var hands = s
                .GroupBy(x => analyzer.Analyze(Cards.CardGroup.Parse(x.Cards), Cards.BoardCards.FromCards(x.Board)))
                .ToList();

            if (hands == null || hands.Count == 0)
            {
                return report;
            }

            hands = ShowdownHandsReportRecord.FilterHands(hands).ToList();

            foreach (var group in hands)
            {
                if (group.Key == null)
                {
                    continue;
                }

                var stat = new ShowdownHandsReportRecord();
                foreach (var playerstatistic in group)
                {
                    stat.AddStatistic(playerstatistic);
                    stat.ShowdownHand = group.Key.GetRank();
                }

                report.Add(stat);
            }

            return report;
        }
    }
}