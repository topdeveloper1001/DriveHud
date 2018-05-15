//-----------------------------------------------------------------------
// <copyright file="BluffToValueRatioCalculator.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.EquityCalculator.ViewModels;
using HandHistories.Objects.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class BluffToValueRatioCalculator
    {
        private const double allowedDeviation = 0.05;

        private static readonly Dictionary<Street, double[]> PredefinedRatio = new Dictionary<Street, double[]>
        {
            [Street.Flop] = new[] { 1.6, 2.0 },
            [Street.Turn] = new[] { 1.0, 1.0 },
            [Street.River] = new[] { 0.5, 0.5 },
        };

        public static bool CheckRatio(int bluffCombos, int valueCombos, Street street, out int[] increaseBluffBy, out int[] increaseValueBy)
        {
            increaseBluffBy = new[] { 0, 0 };
            increaseValueBy = new[] { 0, 0 };

            if ((bluffCombos == 0 && valueCombos == 0) ||
                !PredefinedRatio.TryGetValue(street, out double[] expectedRatio))
            {
                return true;
            }

            var actualRatio = (double)bluffCombos / valueCombos;

            if (actualRatio <= expectedRatio[1] + allowedDeviation &&
                actualRatio >= expectedRatio[0] - allowedDeviation)
            {
                return true;
            }

            if (actualRatio > expectedRatio[1])
            {
                increaseValueBy[0] = valueCombos != 0 ? (int)Math.Round((Math.Round(bluffCombos / expectedRatio[0]) / valueCombos - 1) * 100) : 0;
                increaseValueBy[1] = valueCombos != 0 ? (int)Math.Round((Math.Round(bluffCombos / expectedRatio[1]) / valueCombos - 1) * 100) : 0;
            }
            else
            {
                increaseBluffBy[0] = bluffCombos != 0 ? (int)Math.Round((Math.Round(expectedRatio[0] * valueCombos) / bluffCombos - 1) * 100) : 0;
                increaseBluffBy[1] = bluffCombos != 0 ? (int)Math.Round((Math.Round(expectedRatio[1] * valueCombos) / bluffCombos - 1) * 100) : 0;
            }

            return false;
        }

        public static void AdjustPlayerRange(IEnumerable<EquityRangeSelectorItemViewModel> ranges, Street street,
            HandHistories.Objects.Hand.HandHistory currentHandHistory)
        {
            // Preflop
            if (street == Street.Preflop)
            {
                return;
            }

            var handAnalyzer = new HandAnalyzer();

            var handHistory = new HandHistory();
            handHistory.ConverToEquityCalculatorFormat(currentHandHistory, street);

            if (!handHistory.Players.ContainsKey(handHistory.HeroName))
            {
                return;
            }

            var ungroupedHands = (from range in ranges
                                  let ungrouped = handAnalyzer.UngroupHands(new List<string> { range.Caption }, handHistory).Distinct()
                                  select new RangeGroup { Range = range, Ungrouped = ungrouped, BestHand = postflophand.kNoPair }).ToArray();

            foreach (var ungroupedHand in ungroupedHands)
            {
                foreach (var hand in ungroupedHand.Ungrouped)
                {
                    var card1 = hand.Substring(0, 2);
                    var card2 = hand.Substring(2, 2);
                    var boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
                    var boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
                    var boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
                    var boardCard4 = street >= Street.Flop ? handHistory.CommunityCards[2] : null;
                    var boardCard5 = street >= Street.Turn ? handHistory.CommunityCards[3] : null;

                    var boardInfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, card1, card2);

                    if (boardInfo.holesused > 0)
                    {
                        if (ungroupedHand.BestHand > boardInfo.madehand)
                        {
                            ungroupedHand.BestHand = boardInfo.madehand;
                            ungroupedHand.Weight = boardInfo.weight;
                            ungroupedHand.IsMiddlePair = IsMiddlePair(card1, card2,
                                boardCard1, boardCard2, boardCard3, boardInfo);
                            ungroupedHand.IsWeakKicker = IsWeakKicker(boardInfo);
                            ungroupedHand.IsTopPair = IsTopPair(card1, card2, boardInfo, $"{handHistory.CommunityCards[1]}{boardCard4}{boardCard5}");
                        }

                        Console.WriteLine($"{ungroupedHand.Range.Caption} - {hand}: {boardInfo.madehand}; weight: {boardInfo.weight}");
                    }
                }

                if (ungroupedHand.BestHand == postflophand.kNoPair)
                {
                    ungroupedHand.Range.EquitySelectionMode = EquitySelectionMode.FoldCheck;
                }
            }

            MarkMiddlePairsToCall(ungroupedHands);
            MarkTopPairsToCall(ungroupedHands);
            MarkValueBluff(ungroupedHands, street);
        }

        public static readonly Dictionary<Street, string> RecommendedRange = new Dictionary<Street, string>()
        {
            [Street.Flop] = "1.6-2:1",
            [Street.Turn] = "1:1",
            [Street.River] = "1:2",
        };

        private static bool IsMiddlePair(string card1, string card2, string boardCard1, string boardCard2, string boardCard3, boardinfo boardInfo)
        {
            if (boardInfo.holesused != 1 || boardInfo.madehand != postflophand.kPair)
            {
                return false;
            }

            var orderedBoardCards = (new[] { boardCard1, boardCard2, boardCard3 })
                .OrderBy(x => Card.AllCardsList.IndexOf(x.Substring(0, 1)))
                .ToArray();

            return orderedBoardCards[1][0] == card1[0] || orderedBoardCards[1][0] == card2[0];
        }

        private static bool IsTopPair(string card1, string card2, boardinfo boardInfo, string boardCards)
        {
            if (boardInfo.madehand != postflophand.kPair)
            {
                return false;
            }

            var pairRank = boardCards.Contains(card1) ? card1.Substring(0, 1) : card2.Substring(0, 1);

            return Card.AllCardsList.IndexOf(pairRank) >= Card.AllCardsList.IndexOf("J");
        }

        private static bool IsWeakKicker(boardinfo boardInfo)
        {
            if (boardInfo.madehand != postflophand.kPair)
            {
                return false;
            }

            return boardInfo.kicker <= Card.AllCardsList.IndexOf("T");
        }

        private static void MarkMiddlePairsToCall(RangeGroup[] rangeGroup)
        {
            var middlePairs = rangeGroup.Where(x => x.IsMiddlePair).OrderBy(x => x.Weight).ToArray();

            // 3/4 mark to call
            var rangesToMarkLength = (int)Math.Round(3d * middlePairs.Length / 4);

            if (rangesToMarkLength > 0)
            {
                middlePairs.Take(rangesToMarkLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.Call);
            }
        }

        private static void MarkTopPairsToCall(RangeGroup[] rangeGroup)
        {
            var topPairs = rangeGroup.Where(x => x.IsTopPair && x.IsWeakKicker).OrderBy(x => x.Weight).ToArray();

            // 3/4 mark to call
            var rangesToMarkLength = (int)Math.Round(topPairs.Length / 4d);

            if (rangesToMarkLength > 0)
            {
                topPairs.Take(rangesToMarkLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.Call);
            }
        }

        private static void MarkValueBluff(RangeGroup[] rangeGroup, Street street)
        {
            if (!PredefinedRatio.ContainsKey(street))
            {
                return;
            }

            var ranges = rangeGroup.Where(x => !x.Range.EquitySelectionMode.HasValue)
                .OrderBy(x => x.Weight)
                .ToArray();

            var bluffToValueRatio = PredefinedRatio[street][1];

            var totalCombos = ranges.Sum(x => x.Range.Combos);
            var bluffCombos = totalCombos / (1 + bluffToValueRatio);

            var currentCombos = 0;

            foreach (var range in ranges)
            {
                if (currentCombos < bluffCombos)
                {
                    range.Range.EquitySelectionMode = EquitySelectionMode.Bluff;
                    currentCombos += range.Range.Combos;
                    continue;
                }

                range.Range.EquitySelectionMode = EquitySelectionMode.ValueBet;
            }
        }

        private class RangeGroup
        {
            public EquityRangeSelectorItemViewModel Range { get; set; }

            public IEnumerable<string> Ungrouped { get; set; }

            public postflophand BestHand { get; set; } = postflophand.kNoPair;

            public bool IsMiddlePair { get; set; }

            public bool IsWeakKicker { get; set; }

            public bool IsTopPair { get; set; }

            public int Weight { get; set; }
        }
    }
}