//-----------------------------------------------------------------------
// <copyright file="HandAnalyzerTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.EquityCalculator.Analyzer;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Analyzer = DriveHUD.EquityCalculator.Analyzer;

namespace DriveHud.Tests.HandAnalyzerTests
{
    [TestFixture]
    class HandAnalyzerTests
    {
        //[TestCase('2', 'h', 73730)]
        //[TestCase('3', 'h', 139523)]
        //[TestCase('A', 'h', 268446761)]
        public void FastCardTest(char rank, char suit, int expectedValue)
        {
            var actualValue = HandHistory.fastCard(rank, suit);
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        //[TestCase("Ah2h3h7h6h", 807)]
        //[TestCase("Ah2h3h6h7h", 807)]
        //[TestCase("Ah2h4h6h7h", 806)]
        //[TestCase("Ah2h4h6h8h", 796)]
        //[TestCase("AhKhQhJhTh", 1)]
        //[TestCase("AhAsAcAdKs", 11)]
        //[TestCase("AhAsAcAdQs", 12)]
        public void Eval5Hand(string cards, int expected)
        {
            var card1 = cards.Substring(0, 2);
            var card2 = cards.Substring(2, 2);
            var card3 = cards.Substring(4, 2);
            var card4 = cards.Substring(6, 2);
            var card5 = cards.Substring(8, 2);

            var fCard1 = HandHistory.fastCard(card1[0], card1[1]);
            var fCard2 = HandHistory.fastCard(card2[0], card2[1]);
            var fCard3 = HandHistory.fastCard(card3[0], card3[1]);
            var fCard4 = HandHistory.fastCard(card4[0], card4[1]);
            var fCard5 = HandHistory.fastCard(card5[0], card5[1]);

            var actual = FastEvaluator.eval_5hand(new[] { fCard1, fCard2, fCard3, fCard4, fCard5 });

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void BluffToValueTests()
        {
            var board = "QsAd6d";
            var ranges = new[] { "77", "88", "99", "TT", "JJ", "QQ", "KK", "AA", "A2s", "A3s", "A4s", "A5s", "A6s", "A7s", "A8s", "A9s", "ATs", "AJs", "AQs", "AKs", "K7s", "K8s", "K9s", "KTs", "KJs", "KQs", "Q7s", "Q8s", "Q9s", "QTs", "QJs", "J8s", "J9s", "JTs", "T8s", "T9s", "97s", "98s", "86s", "87s", "76s", "A2o", "A3o", "A4o", "A5o", "A6o", "A7o", "A8o", "A9o", "ATo", "AJo", "AQo", "AKo", "K9o", "KTo", "KJo", "KQo", "QTo", "QJo", "JTo", "T8o", "T9o", "97o", "98o", "86o", "87o", "76o" };

            var handHistory = new HandHistory();
            handHistory.CommunityCards[1] = board;

            var rangeItems = (from range in ranges
                              select new RangeItem
                              {
                                  Caption = range,
                                  Cards = (from ungroupedHand in HandAnalyzer.UngroupHands(new List<string> { range }, handHistory, true)
                                           select new RangeItemCard { Caption = ungroupedHand }).ToArray()
                              }).ToArray();

            var boardCard1 = board.Substring(0, 2);
            var boardCard2 = board.Substring(2, 2);
            var boardCard3 = board.Substring(4, 2);

            var fBoardCard1 = HandHistory.fastCard(boardCard1[0], boardCard1[1]);
            var fBoardCard2 = HandHistory.fastCard(boardCard2[0], boardCard2[1]);
            var fBoardCard3 = HandHistory.fastCard(boardCard3[0], boardCard3[1]);

            var counter = 0;

            // all possible cases
            foreach (var rangeItem in rangeItems)
            {
                foreach (var holeCards in rangeItem.Cards)
                {
                    var holeCard1 = holeCards.Caption.Substring(0, 2);
                    var holeCard2 = holeCards.Caption.Substring(2, 2);

                    var fHoleCard1 = HandHistory.fastCard(holeCard1[0], holeCard1[1]);
                    var fHoleCard2 = HandHistory.fastCard(holeCard2[0], holeCard2[1]);

                    var usedCards = new[] { fBoardCard1, fBoardCard2, fBoardCard3, fHoleCard1, fHoleCard2 };

                    for (var i = 0; i < 52; i++)
                    {
                        var boardCard4 = HandHistory.fastCard(Analyzer.Card.CardName[i], Analyzer.Card.CardSuit[i]);

                        if (usedCards.Contains(boardCard4))
                        {
                            continue;
                        }

                        for (var j = i + 1; j < 52; j++)
                        {
                            var boardCard5 = HandHistory.fastCard(Analyzer.Card.CardName[j], Analyzer.Card.CardSuit[j]);

                            if (usedCards.Contains(boardCard5))
                            {
                                continue;
                            }

                            var weight = FastEvaluator.eval_7hand(usedCards.Concat(new[] { boardCard4, boardCard5 }).ToArray());

                            holeCards.Weights.Add(weight);

                            counter++;
                        }
                    }
                }
            }

            var comparer = new RangeItemComparer();

            //var rangeItem1 = rangeItems.FirstOrDefault(x => x.Caption == "AA");
            //var rangeItem2 = rangeItems.FirstOrDefault(x => x.Caption == "KK");

            //Assert.That(comparer.Compare(rangeItem1, rangeItem2), Is.EqualTo(1));

            //var arr = new ArrayList(rangeItems);
            //arr.Sort(comparer);

            Array.Sort(rangeItems, comparer);

            //rangeItems = rangeItems.OrderByDescending(x => x, comparer).ToArray();

            Debug.WriteLine($"Combos: {rangeItems.Sum(x => x.Cards.Length)}");
            Debug.WriteLine($"Simulations: {counter}");
            Debug.WriteLine($"Ordered range: {string.Join(", ", rangeItems.Reverse().Select(x => x.Caption).ToArray())}");
        }

        private class RangeItem
        {
            public string Caption { get; set; }

            public RangeItemCard[] Cards { get; set; }

            public int Weight { get; set; }

            public int MinWeight { get; set; } = 9999;

            public string BestCombo { get; set; }

            public override string ToString()
            {
                return $"Caption: {Caption} MinWeight: {MinWeight} Weigth: {Weight} Combos: {Cards.Length} Cards: {string.Join(", ", Cards.Select(x => x.Caption).ToArray())}";
            }
        }

        private class RangeItemCard
        {
            public string Caption { get; set; }

            public List<int> Weights { get; set; } = new List<int>();

            public override string ToString()
            {
                return $"Caption: {Caption}";
            }
        }

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
                            throw new InvalidOperationException("List of weights of cards of range items to compare must be the same length.");
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
    }
}