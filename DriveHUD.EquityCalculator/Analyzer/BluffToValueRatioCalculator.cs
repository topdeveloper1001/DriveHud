﻿//-----------------------------------------------------------------------
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
using DriveHUD.ViewModels;
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
            HandHistories.Objects.Hand.HandHistory currentHandHistory, string board, int opponentsCount)
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

            if (!string.IsNullOrEmpty(board) && street >= Street.Flop)
            {
                var customBoardCards = BoardCards.FromCards(board);

                handHistory.CommunityCards[1] = string.Join(string.Empty, customBoardCards.GetBoardOnStreet(Street.Flop));

                if (street >= Street.Turn)
                {
                    handHistory.CommunityCards[2] = customBoardCards.GetBoardOnStreet(Street.Turn).Last().ToString();

                    if (street >= Street.River)
                    {
                        handHistory.CommunityCards[3] = customBoardCards.GetBoardOnStreet(Street.River).Last().ToString();
                    }
                }
            }

            if (!handHistory.Players.ContainsKey(handHistory.HeroName))
            {
                return;
            }

            var potType = GetPotType(opponentsCount);

            var rangeItems = (from range in ranges
                              let cards = (from hand in HandAnalyzer.UngroupHands(new List<string> { range.Caption }, handHistory, true).Distinct()
                                           select new RangeItemCard
                                           {
                                               HandSuit = range.HandSuitsModelList.Where(x => x.IsVisible).First(x => CompareHandToHandSuit(x.HandSuit, hand)),
                                               Caption = hand
                                           }).ToArray()
                              select new RangeItem
                              {
                                  Range = range,
                                  Cards = cards
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

            var rangeItemCards = rangeItems.SelectMany(x => x.Cards).ToArray();

            // iterate over all possible combinations to get equity of each range

            foreach (var rangeItemCard in rangeItemCards)
            {
                var holeCard1 = rangeItemCard.Caption.Substring(0, 2);
                var holeCard2 = rangeItemCard.Caption.Substring(2, 2);

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

                        rangeItemCard.Weights.Add(weight);

                        var boardInfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, holeCard1, holeCard2);

                        if (boardInfo.holesused > 0 && rangeItemCard.BestHand > boardInfo.madehand)
                        {
                            if (boardInfo.madehand == PostFlopHand.kPair || boardInfo.madehand == PostFlopHand.k2Pair)
                            {
                                var pairType = boardInfo.madehand == PostFlopHand.k2Pair ?
                                    GetPairType(holeCard1, holeCard2, new boardinfo { madehand = PostFlopHand.kPair }, RemovePairFromBoard(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5)) :
                                    GetPairType(holeCard1, holeCard2, boardInfo, $"{handHistory.CommunityCards[1]}{boardCard4}{boardCard5}");

                                rangeItemCard.IsMiddlePair = pairType == PairType.Middle;
                                rangeItemCard.IsBottomPair = pairType == PairType.Bottom;
                                rangeItemCard.IsTopPair = pairType == PairType.Top;
                                rangeItemCard.IsWeakKicker = boardInfo.kicker <= Card.AllCardsList.IndexOf("T");
                                rangeItemCard.IsGoodKicker = !rangeItemCard.IsWeakKicker && boardInfo.kicker <= Card.AllCardsList.IndexOf("J");
                                rangeItemCard.IsTopKicker = boardInfo.kicker >= Card.AllCardsList.IndexOf("Q");
                            }

                            rangeItemCard.IsConnectedToBoard = boardInfo.madehand == PostFlopHand.kPair && boardInfo.holesused == 1 ||
                                boardInfo.madehand != PostFlopHand.kPair;

                            rangeItemCard.BestHand = boardInfo.madehand;
                        }
                    }
                }
            }

            rangeItemCards.ForEach(x => x.Weights = x.Weights.OrderBy(w => w).ToList());

            Array.Sort(rangeItemCards, new RangeItemCardComparer());

            // set weights
            for (var i = 0; i < rangeItemCards.Length; i++)
            {
                rangeItemCards[i].Weight = i;
            }

            // assign groups            
            MarkTopPairsAndHigherToValueBet(rangeItemCards);
            MarkPocketMiddlePairs(rangeItemCards);
            MarkPocketBottomPairs(rangeItemCards);

            MarkConnectedTopPairs(rangeItemCards);
            MarkConnectedMiddlePairs(rangeItemCards);
            MarkConnectedBottomPairs(rangeItemCards);

            MarkBluffs(potType, rangeItemCards, street);

            rangeItemCards
                .Where(x => x.HandSuit.SelectionMode == EquitySelectionMode.None)
                .ForEach(x => x.HandSuit.SelectionMode = x.BestHand == PostFlopHand.kPair ? EquitySelectionMode.Call : EquitySelectionMode.FoldCheck);

            rangeItems.ForEach(x =>
            {
                var handSuit = x.Range
                    .HandSuitsModelList
                    .Where(p => p.IsVisible)
                    .GroupBy(p => p.SelectionMode)
                    .OrderByDescending(p => p.Count()).First().Key;

                x.Range.EquitySelectionMode = handSuit;
                x.Range.RefreshCombos();
            });
        }

        public static string GetRecommendedRange(int opponentsCount, Street street)
        {
            var potType = GetPotType(opponentsCount);
            return RecommendedRange[potType][street];
        }

        internal static PairType GetPairType(string card1, string card2, boardinfo boardInfo, string boardCards)
        {
            if (boardInfo.madehand != PostFlopHand.kPair)
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

        private static void MarkTopPairsAndHigherToValueBet(RangeItemCard[] rangeGroup)
        {
            rangeGroup
                .Where(x => x.BestHand < PostFlopHand.kPair ||
                    x.BestHand == PostFlopHand.kPair && x.IsTopPair && !x.IsConnectedToBoard)
                .ForEach(x => x.HandSuit.SelectionMode = EquitySelectionMode.ValueBet);
        }

        private static void MarkPocketMiddlePairs(RangeItemCard[] rangeGroup)
        {
            var middlePairs = rangeGroup.Where(x => x.IsMiddlePair && !x.IsConnectedToBoard).OrderBy(x => x.Weight).ToArray();

            // 3/4 mark to call
            var rangesToMarkLength = (int)Math.Round(0.75 * middlePairs.Length);

            if (rangesToMarkLength > 0)
            {
                middlePairs.Take(rangesToMarkLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.Call);
                middlePairs.Skip(rangesToMarkLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.ValueBet);
            }
        }

        private static void MarkPocketBottomPairs(RangeItemCard[] rangeGroup)
        {
            var middlePairs = rangeGroup.Where(x => x.IsBottomPair && !x.IsConnectedToBoard).OrderByDescending(x => x.Weight).ToArray();

            // 1/4 mark to vb
            var rangesToMarkLength = (int)Math.Round(0.25 * middlePairs.Length);

            if (rangesToMarkLength > 0)
            {
                middlePairs.Take(rangesToMarkLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.ValueBet);
            }
        }

        private static void MarkConnectedTopPairs(RangeItemCard[] rangeGroup)
        {
            var topPairsGoodKicker = rangeGroup
                .Where(x => x.IsTopPair && x.IsConnectedToBoard && (x.IsTopKicker || x.IsGoodKicker))
                .OrderBy(x => x.Weight)
                .ToArray();

            // 1/2 of TPGK+ mark to call
            var rangesToMarkAsCallLength = (int)Math.Round(0.5 * topPairsGoodKicker.Length);

            if (rangesToMarkAsCallLength > 0)
            {
                topPairsGoodKicker.Take(rangesToMarkAsCallLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.Call);
                topPairsGoodKicker.Skip(rangesToMarkAsCallLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.ValueBet);
            }

            var topPairsWeakKicker = rangeGroup
               .Where(x => x.IsTopPair && x.IsConnectedToBoard && x.IsWeakKicker)
               .OrderBy(x => x.Weight)
               .ToArray();

            // 3/4 of TPWK- mark to call
            rangesToMarkAsCallLength = (int)Math.Round(0.75 * topPairsGoodKicker.Length);

            if (rangesToMarkAsCallLength > 0)
            {
                topPairsWeakKicker.Take(rangesToMarkAsCallLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.Call);
                topPairsWeakKicker.Skip(rangesToMarkAsCallLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.ValueBet);
            }
        }

        private static void MarkConnectedMiddlePairs(RangeItemCard[] rangeGroup)
        {
            var middlePairs = rangeGroup.Where(x => x.IsMiddlePair && x.IsConnectedToBoard).OrderBy(x => x.Weight).ToArray();

            // 1/2 mark to call
            var rangesToMarkLength = (int)Math.Round(0.5 * middlePairs.Length);

            if (rangesToMarkLength > 0)
            {
                middlePairs.Take(rangesToMarkLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.Call);
                middlePairs.Skip(rangesToMarkLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.ValueBet);
            }
        }

        private static void MarkConnectedBottomPairs(RangeItemCard[] rangeGroup)
        {
            var bottomPairsTopKicker = rangeGroup
                .Where(x => x.IsBottomPair && x.IsConnectedToBoard && x.IsTopKicker)
                .OrderBy(x => x.Weight)
                .ToArray();

            // 3/4 of BPTK+ mark to call
            var rangesToMarkLength = (int)Math.Round(0.75 * bottomPairsTopKicker.Length);

            if (rangesToMarkLength > 0)
            {
                bottomPairsTopKicker.Take(rangesToMarkLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.Call);
            }

            bottomPairsTopKicker.Skip(rangesToMarkLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.ValueBet);

            var bottomPairsNoTopKicker = rangeGroup
                .Where(x => x.IsBottomPair && x.IsConnectedToBoard && !x.IsTopKicker)
                .OrderBy(x => x.Weight)
                .ToArray();

            rangesToMarkLength = (int)Math.Round(0.5 * bottomPairsNoTopKicker.Length);

            if (rangesToMarkLength > 0)
            {
                bottomPairsNoTopKicker.Take(rangesToMarkLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.FoldCheck);
            }

            bottomPairsNoTopKicker.Skip(rangesToMarkLength).ForEach(r => r.HandSuit.SelectionMode = EquitySelectionMode.Call);
        }

        private static void MarkBluffs(PotType potType, RangeItemCard[] rangeGroup, Street street)
        {
            if (!PredefinedRatio[potType].ContainsKey(street))
            {
                return;
            }

            var ranges = rangeGroup.Where(x => x.HandSuit.SelectionMode == EquitySelectionMode.None)
                .OrderByDescending(x => x.Weight)
                .ToArray();

            var bluffToValueRatio = (PredefinedRatio[potType][street][0] + PredefinedRatio[potType][street][1]) / 2;

            var valueBetCombos = rangeGroup.Count(x => x.HandSuit.SelectionMode == EquitySelectionMode.ValueBet);
            var bluffCombos = bluffToValueRatio * valueBetCombos;

            var currentCombos = 0;

            foreach (var range in ranges)
            {
                if (currentCombos < bluffCombos)
                {
                    range.HandSuit.SelectionMode = EquitySelectionMode.Bluff;
                    currentCombos++; ;
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

        private static string RemovePairFromBoard(params string[] boardCards)
        {
            if (boardCards == null || boardCards.Length == 0)
            {
                return string.Empty;
            }

            var boardCardsList = boardCards.Where(x => !string.IsNullOrEmpty(x)).ToList();

            for (var i = 0; i < boardCardsList.Count; i++)
            {
                for (var j = i + 1; j < boardCardsList.Count; j++)
                {
                    if (boardCards[i][0] == boardCards[j][0])
                    {
                        boardCardsList.Remove(boardCards[i]);
                        boardCardsList.Remove(boardCards[j]);

                        return string.Join(string.Empty, boardCardsList.ToArray());
                    }
                }
            }

            return string.Join(string.Empty, boardCardsList.ToArray());
        }

        private static bool CompareHandToHandSuit(HandSuitsEnum handSuit, string hand)
        {
            var handSuitText = $"{hand[1]}{hand[3]}";

            if (handSuit.ToString().Equals(handSuitText, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // if pair try reverted order
            if (hand[0] == hand[2])
            {
                handSuitText = $"{hand[3]}{hand[1]}";
                return handSuit.ToString().Equals(handSuitText, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        #region Private helpers

        private class RangeItem
        {
            public EquityRangeSelectorItemViewModel Range { get; set; }

            public RangeItemCard[] Cards { get; set; }

            public PostFlopHand BestHand { get; set; } = PostFlopHand.kNoPair;

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
            public HandSuitsViewModel HandSuit { get; set; }

            public string Caption { get; set; }

            public int Weight { get; set; }

            public List<int> Weights { get; set; } = new List<int>();

            public PostFlopHand BestHand { get; set; } = PostFlopHand.kNoPair;

            public bool IsMiddlePair { get; set; }

            public bool IsTopKicker { get; set; }

            public bool IsGoodKicker { get; set; }

            public bool IsWeakKicker { get; set; }

            public bool IsTopPair { get; set; }

            public bool IsBottomPair { get; set; }

            public bool IsConnectedToBoard { get; set; }

#if DEBUG
            public override string ToString()
            {
                return $"Caption: {Caption}; Weight={Weight}; BestHand: {BestHand}; IsTop={IsTopPair}; IsMiddle={IsMiddlePair}; IsBottom={IsBottomPair}; IsConnected={IsConnectedToBoard}";
            }
#endif
        }

        private class RangeItemCardComparer : IComparer
        {
            public int Compare(RangeItemCard cards1, RangeItemCard cards2)
            {
                if (cards1 == null)
                {
                    throw new ArgumentNullException(nameof(cards1));
                }

                if (cards2 == null)
                {
                    throw new ArgumentNullException(nameof(cards2));
                }

                var wins1 = 0;
                var wins2 = 0;


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

                return wins1.CompareTo(wins2);
            }

            public int Compare(object x, object y)
            {
                return Compare(x as RangeItemCard, y as RangeItemCard);
            }
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

        #endregion
    }
}