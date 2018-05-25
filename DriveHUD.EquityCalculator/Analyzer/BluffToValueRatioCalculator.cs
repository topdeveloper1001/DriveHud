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

        private static readonly Dictionary<PotType, Dictionary<Street, double[]>> PredefinedRatio = new Dictionary<PotType, Dictionary<Street, double[]>>
        {
            [PotType.HU] = new Dictionary<Street, double[]>
            {
                [Street.Flop] = new[] { 1.6, 2.0 },
                [Street.Turn] = new[] { 1.0, 1.0 },
                [Street.River] = new[] { 0.5, 0.5 }
            },
            [PotType.MW] = new Dictionary<Street, double[]>
            {
                [Street.Flop] = new[] { 0.8, 1 },
                [Street.Turn] = new[] { 0.5, 0.5 },
                [Street.River] = new[] { 0.25, 0.25 }
            }
        };

        private static readonly Dictionary<PotType, Dictionary<Street, string>> RecommendedRange = new Dictionary<PotType, Dictionary<Street, string>>()
        {
            [PotType.HU] = new Dictionary<Street, string>
            {
                [Street.Flop] = "1.6-2:1",
                [Street.Turn] = "1:1",
                [Street.River] = "1:2"
            },
            [PotType.MW] = new Dictionary<Street, string>
            {
                [Street.Flop] = "0.8-1:1",
                [Street.Turn] = "1:2",
                [Street.River] = "1:4"
            },
        };

        public static bool CheckRatio(int opponentsCount, int bluffCombos, int valueCombos,
            Street street, out int[] increaseBluffBy, out int[] increaseValueBy)
        {
            increaseBluffBy = new[] { 0, 0 };
            increaseValueBy = new[] { 0, 0 };

            var potType = GetPotType(opponentsCount);

            if ((bluffCombos == 0 && valueCombos == 0) ||
                !PredefinedRatio[potType].TryGetValue(street, out double[] expectedRatio))
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
            HandHistories.Objects.Hand.HandHistory currentHandHistory, int opponentsCount)
        {
            // supports only flop/turn/river
            if (street != Street.Flop &&
                street != Street.Turn &&
                street != Street.River)
            {
                return;
            }

            var handHistory = new HandHistory();
            handHistory.ConverToEquityCalculatorFormat(currentHandHistory, street);

            if (!handHistory.Players.ContainsKey(handHistory.HeroName))
            {
                return;
            }

            var potType = GetPotType(opponentsCount);

            var rangeItems = (from range in ranges
                              let cards = (from hand in HandAnalyzer.UngroupHands(new List<string> { range.Caption }, handHistory, true).Distinct()
                                           select new RangeItemCard
                                           {
                                               Caption = hand
                                           }).ToArray()
                              select new RangeItem
                              {
                                  Range = range,
                                  Cards = cards,
                                  BestHand = postflophand.kNoPair
                              }).ToArray();

            // flop cards
            var boardCard1 = handHistory.CommunityCards[1].Substring(0, 2);
            var boardCard2 = handHistory.CommunityCards[1].Substring(2, 2);
            var boardCard3 = handHistory.CommunityCards[1].Substring(4, 2);
            var boardCard4 = street >= Street.Turn && handHistory.CommunityCards[2] != null ?
                handHistory.CommunityCards[2].Substring(0, 2) : null;
            var boardCard5 = street == Street.River && handHistory.CommunityCards[3] != null ?
                handHistory.CommunityCards[3].Substring(0, 2) : null;

            // flop cards in binary format
            var fBoardCard1 = HandHistory.fastCard(boardCard1[0], boardCard1[1]);
            var fBoardCard2 = HandHistory.fastCard(boardCard2[0], boardCard2[1]);
            var fBoardCard3 = HandHistory.fastCard(boardCard3[0], boardCard3[1]);
            var fBoardCard4 = boardCard4 != null ? HandHistory.fastCard(boardCard4[0], boardCard4[1]) : 0;
            var fBoardCard5 = boardCard5 != null ? HandHistory.fastCard(boardCard5[0], boardCard5[1]) : 0;

            var boardCards = new[] { fBoardCard1, fBoardCard2, fBoardCard3, fBoardCard4, fBoardCard5 };

            // iterate over all possible combinations to get equity of each range
            foreach (var rangeItem in rangeItems)
            {
                foreach (var holeCards in rangeItem.Cards)
                {
                    var holeCard1 = holeCards.Caption.Substring(0, 2);
                    var holeCard2 = holeCards.Caption.Substring(2, 2);

                    var fHoleCard1 = HandHistory.fastCard(holeCard1[0], holeCard1[1]);
                    var fHoleCard2 = HandHistory.fastCard(holeCard2[0], holeCard2[1]);

                    var usedCards = boardCards.Concat(new[] { fHoleCard1, fHoleCard2 }).ToArray();

                    var turnCardIterations = boardCard4 == null ? 52 : 1;

                    for (var i = 0; i < turnCardIterations; i++)
                    {
                        if (boardCard4 == null)
                        {
                            fBoardCard4 = HandHistory.fastCard(Card.CardName[i], Card.CardSuit[i]);

                            if (usedCards.Contains(fBoardCard4))
                            {
                                continue;
                            }
                        }

                        var riverIterationsStartIndex = boardCard4 == null ? i + 1 : 0;
                        var riverCardIterations = boardCard5 == null ? 52 : 1;

                        for (var j = riverIterationsStartIndex; j < riverCardIterations; j++)
                        {
                            if (boardCard5 == null)
                            {
                                fBoardCard5 = HandHistory.fastCard(Card.CardName[j], Card.CardSuit[j]);

                                if (usedCards.Contains(fBoardCard5))
                                {
                                    continue;
                                }
                            }

                            var hand = new[] { fBoardCard1, fBoardCard2, fBoardCard3, fBoardCard4, fBoardCard5, fHoleCard1, fHoleCard2 };
                            var weight = FastEvaluator.eval_7hand(hand);

                            holeCards.Weights.Add(weight);

                            var boardInfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, holeCard1, holeCard2);

                            if (boardInfo.holesused > 0 && rangeItem.BestHand > boardInfo.madehand)
                            {
                                if (boardInfo.madehand == postflophand.kPair)
                                {
                                    var pairType = GetPairType(holeCard1, holeCard2, boardInfo, $"{handHistory.CommunityCards[1]}{boardCard4}{boardCard5}");

                                    rangeItem.IsMiddlePair = pairType == PairType.Middle;
                                    rangeItem.IsBottomPair = pairType == PairType.Bottom;
                                    rangeItem.IsTopPair = pairType == PairType.Top;
                                    rangeItem.IsWeakKicker = boardInfo.kicker <= Card.AllCardsList.IndexOf("T");
                                    rangeItem.IsGoodKicker = !rangeItem.IsWeakKicker && boardInfo.kicker <= Card.AllCardsList.IndexOf("J");
                                    rangeItem.IsTopKicker = boardInfo.kicker >= Card.AllCardsList.IndexOf("Q");
                                }

                                rangeItem.IsConnectedToBoard = boardInfo.madehand == postflophand.kPair && boardInfo.holesused == 1 ||
                                    boardInfo.madehand != postflophand.kPair;

                                rangeItem.BestHand = boardInfo.madehand;
                            }
                        }
                    }
                }
            }
        
            Array.Sort(rangeItems, new RangeItemComparer());

            // set weights
            for (var i = 0; i < rangeItems.Length; i++)
            {
                rangeItems[i].Weight = i;
            }

            // assign groups            
            MarkTopPairsAndHigherToValueBet(rangeItems);
            MarkPocketMiddlePairs(rangeItems);
            MarkPocketBottomPairs(rangeItems);

            MarkConnectedTopPairs(rangeItems);
            MarkConnectedMiddlePairs(rangeItems);
            MarkConnectedBottomPairs(rangeItems);

            MarkBluffs(potType, rangeItems, street);

            rangeItems
                .Where(x => !x.Range.EquitySelectionMode.HasValue)
                .ForEach(x => x.Range.EquitySelectionMode = x.BestHand == postflophand.kPair ? EquitySelectionMode.Call : EquitySelectionMode.FoldCheck);
        }

        public static string GetRecommendedRange(int opponentsCount, Street street)
        {
            var potType = GetPotType(opponentsCount);
            return RecommendedRange[potType][street];
        }

        private static PairType GetPairType(string card1, string card2, boardinfo boardInfo, string boardCards)
        {
            if (boardInfo.madehand != postflophand.kPair)
            {
                return PairType.None;
            }

            var pairRank = boardCards.Contains(card1[0]) ? card1.Substring(0, 1) : card2.Substring(0, 1);

            if (Card.AllCardsList.IndexOf(pairRank) >= Card.AllCardsList.IndexOf("J"))
            {
                return PairType.Top;
            }
            else if (Card.AllCardsList.IndexOf(pairRank) >= Card.AllCardsList.IndexOf("8"))
            {
                return PairType.Middle;
            }

            return PairType.Bottom;
        }

        private static void MarkTopPairsAndHigherToValueBet(RangeItem[] rangeGroup)
        {
            rangeGroup
                .Where(x => x.BestHand < postflophand.kPair ||
                    x.BestHand == postflophand.kPair && x.IsTopPair && !x.IsConnectedToBoard)
                .ForEach(x => x.Range.EquitySelectionMode = EquitySelectionMode.ValueBet);
        }

        private static void MarkPocketMiddlePairs(RangeItem[] rangeGroup)
        {
            var middlePairs = rangeGroup.Where(x => x.IsMiddlePair && !x.IsConnectedToBoard).OrderBy(x => x.Weight).ToArray();

            // 3/4 mark to call
            var rangesToMarkLength = (int)Math.Round(0.75 * middlePairs.Length);

            if (rangesToMarkLength > 0)
            {
                middlePairs.Take(rangesToMarkLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.Call);
                middlePairs.Skip(rangesToMarkLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.ValueBet);
            }
        }

        private static void MarkPocketBottomPairs(RangeItem[] rangeGroup)
        {
            var middlePairs = rangeGroup.Where(x => x.IsBottomPair && !x.IsConnectedToBoard).OrderByDescending(x => x.Weight).ToArray();

            // 1/4 mark to vb
            var rangesToMarkLength = (int)Math.Round(0.25 * middlePairs.Length);

            if (rangesToMarkLength > 0)
            {
                middlePairs.Take(rangesToMarkLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.ValueBet);
            }
        }

        private static void MarkConnectedTopPairs(RangeItem[] rangeGroup)
        {
            var topPairsGoodKicker = rangeGroup
                .Where(x => x.IsTopPair && x.IsConnectedToBoard && (x.IsTopKicker || x.IsGoodKicker))
                .OrderBy(x => x.Weight)
                .ToArray();

            // 1/2 of TPGK+ mark to call
            var rangesToMarkAsCallLength = (int)Math.Round(0.5 * topPairsGoodKicker.Length);

            if (rangesToMarkAsCallLength > 0)
            {
                topPairsGoodKicker.Take(rangesToMarkAsCallLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.Call);
                topPairsGoodKicker.Skip(rangesToMarkAsCallLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.ValueBet);
            }

            var topPairsWeakKicker = rangeGroup
               .Where(x => x.IsTopPair && x.IsConnectedToBoard && x.IsWeakKicker)
               .OrderBy(x => x.Weight)
               .ToArray();

            // 3/4 of TPWK- mark to call
            rangesToMarkAsCallLength = (int)Math.Round(0.75 * topPairsGoodKicker.Length);

            if (rangesToMarkAsCallLength > 0)
            {
                topPairsWeakKicker.Take(rangesToMarkAsCallLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.Call);
                topPairsWeakKicker.Skip(rangesToMarkAsCallLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.ValueBet);
            }
        }

        private static void MarkConnectedMiddlePairs(RangeItem[] rangeGroup)
        {
            var middlePairs = rangeGroup.Where(x => x.IsMiddlePair && x.IsConnectedToBoard).OrderBy(x => x.Weight).ToArray();

            // 1/2 mark to call
            var rangesToMarkLength = (int)Math.Round(0.5 * middlePairs.Length);

            if (rangesToMarkLength > 0)
            {
                middlePairs.Take(rangesToMarkLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.Call);
                middlePairs.Skip(rangesToMarkLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.ValueBet);
            }
        }

        private static void MarkConnectedBottomPairs(RangeItem[] rangeGroup)
        {
            var bottomPairsTopKicker = rangeGroup
                .Where(x => x.IsBottomPair && x.IsConnectedToBoard && x.IsTopKicker)
                .OrderBy(x => x.Weight)
                .ToArray();

            // 3/4 of BPTK+ mark to call
            var rangesToMarkLength = (int)Math.Round(0.75 * bottomPairsTopKicker.Length);

            if (rangesToMarkLength > 0)
            {
                bottomPairsTopKicker.Take(rangesToMarkLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.Call);
                bottomPairsTopKicker.Skip(rangesToMarkLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.ValueBet);
            }

            var bottomPairsNoTopKicker = rangeGroup
                .Where(x => x.IsBottomPair && x.IsConnectedToBoard && !x.IsTopKicker)
                .OrderBy(x => x.Weight)
                .ToArray();

            rangesToMarkLength = (int)Math.Round(0.5 * bottomPairsNoTopKicker.Length);

            if (rangesToMarkLength > 0)
            {
                bottomPairsNoTopKicker.Take(rangesToMarkLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.FoldCheck);
                bottomPairsNoTopKicker.Skip(rangesToMarkLength).ForEach(r => r.Range.EquitySelectionMode = EquitySelectionMode.Call);
            }
        }

        private static void MarkBluffs(PotType potType, RangeItem[] rangeGroup, Street street)
        {
            if (!PredefinedRatio[potType].ContainsKey(street))
            {
                return;
            }

            var ranges = rangeGroup.Where(x => !x.Range.EquitySelectionMode.HasValue)
                .OrderByDescending(x => x.Weight)
                .ToArray();

            var bluffToValueRatio = (PredefinedRatio[potType][street][0] + PredefinedRatio[potType][street][1]) / 2;

            var valueBetCombos = rangeGroup.Where(x => x.Range.EquitySelectionMode == EquitySelectionMode.ValueBet).Sum(x => x.Range.Combos);
            var bluffCombos = bluffToValueRatio * valueBetCombos;

            var currentCombos = 0;

            foreach (var range in ranges)
            {
                if (currentCombos < bluffCombos &&
                    (currentCombos + range.Range.Combos - bluffCombos < bluffCombos - currentCombos))
                {
                    range.Range.EquitySelectionMode = EquitySelectionMode.Bluff;
                    currentCombos += range.Range.Combos;
                    continue;
                }

                break;
            }
        }

        private static PotType GetPotType(int opponentsCount)
        {
            if (opponentsCount > 1)
            {
                return PotType.MW;
            }

            return PotType.HU;
        }

        #region Private helpers

        private class RangeItem
        {
            public EquityRangeSelectorItemViewModel Range { get; set; }

            public RangeItemCard[] Cards { get; set; }

            public postflophand BestHand { get; set; } = postflophand.kNoPair;

            public bool IsMiddlePair { get; set; }

            public bool IsTopKicker { get; set; }

            public bool IsGoodKicker { get; set; }

            public bool IsWeakKicker { get; set; }

            public bool IsTopPair { get; set; }

            public bool IsBottomPair { get; set; }

            public bool IsConnectedToBoard { get; set; }

            public int Weight { get; set; }

#if DEBUG
            public override string ToString()
            {
                return $"{Range.Caption}, Weight={Weight}, BestHand={BestHand}; Equity={Range.EquitySelectionMode}; IsTop={IsTopPair}; IsMiddle={IsMiddlePair}; IsBottom={IsBottomPair}; IsConnected={IsConnectedToBoard}";
            }
#endif
        }

        private class RangeItemCard
        {
            public string Caption { get; set; }

            public List<int> Weights { get; set; } = new List<int>();

#if DEBUG
            public override string ToString()
            {
                return $"Caption: {Caption}";
            }
#endif
        }

        /// <summary>
        /// Comparer to compare equities of the specified range items
        /// </summary>
        private class RangeItemComparer : IComparer
        {
            public int Compare(RangeItem x1, RangeItem x2)
            {
                if (x1 == null)
                {
                    throw new ArgumentNullException(nameof(x1));
                }

                if (x2 == null)
                {
                    throw new ArgumentNullException(nameof(x2));
                }

                var wins1 = 0;
                var wins2 = 0;

                for (var i = 0; i < x1.Cards.Length; i++)
                {
                    var cards1 = x1.Cards[i];

                    for (var j = 0; j < x2.Cards.Length; j++)
                    {
                        var cards2 = x2.Cards[j];

                        if (cards1.Weights.Count != cards2.Weights.Count)
                        {
                            throw new InvalidOperationException("The list of weights of cards of range items to compare must be the same length.");
                        }

                        for (var k = 0; k < cards1.Weights.Count; k++)
                        {
                            if (cards1.Weights[k] < cards2.Weights[k])
                            {
                                wins1++;
                            }
                            else if (cards1.Weights[k] > cards2.Weights[k])
                            {
                                wins2++;
                            }
                        }
                    }
                }

                return wins1.CompareTo(wins2);
            }

            public int Compare(object x, object y)
            {
                return Compare(x as RangeItem, y as RangeItem);
            }
        }

        private enum PotType
        {
            HU,
            MW
        }

        private enum PairType
        {
            None,
            Bottom,
            Middle,
            Top
        }

        #endregion
    }
}