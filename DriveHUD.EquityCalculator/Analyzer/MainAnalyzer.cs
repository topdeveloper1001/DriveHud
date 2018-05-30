//-----------------------------------------------------------------------
// <copyright file="MainAnalyzer.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.EquityCalculator.ViewModels;
using Model.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal static class MainAnalyzer
    {
        private static bool init = true;

        internal static void GetStrongestOpponent(HandHistories.Objects.Hand.HandHistory currentHandHistory, HandHistories.Objects.Cards.Street currentStreet, out string strongestOpponentName, out IEnumerable<EquityRangeSelectorItemViewModel> strongestOpponentHands)
        {
            strongestOpponentName = null;
            strongestOpponentHands = new List<EquityRangeSelectorItemViewModel>();

            if (init)
            {
                TempConfig.Init();
                HandHistory.Init();
                Card.Init();
                init = false;
            }

            var handAnalyzer = new HandAnalyzer();

            var handHistory = new HandHistory();
            handHistory.ConverToEquityCalculatorFormat(currentHandHistory, currentStreet);

            // analyze preflop ranges
            var hand_range = handAnalyzer.PreflopAnalysis(handHistory);

            var hand_collective = new Hashtable();

            foreach (string key in hand_range.Keys)
            {
                var hand_distribution = new hand_distribution
                {
                    hand_range = (float)Convert.ToDouble(hand_range[key])
                };

                hand_collective.Add(key, hand_distribution);
            }

            if (currentStreet != HandHistories.Objects.Cards.Street.Preflop)
            {
                var street = currentStreet == HandHistories.Objects.Cards.Street.Flop ? 1 :
                                currentStreet == HandHistories.Objects.Cards.Street.Turn ? 2 : 3;

                hand_collective = handAnalyzer.PostflopAnalysis(handHistory, street, hand_collective);
            }

            strongestOpponentHands = GroupHands(handAnalyzer.StrongestOpponentHands);
            strongestOpponentName = handAnalyzer.StrongestOpponentName;
        }

        internal static IEnumerable<EquityRangeSelectorItemViewModel> GetHeroRange(HandHistories.Objects.Hand.HandHistory currentHandHistory, HandHistories.Objects.Cards.Street currentStreet)
        {
            if (currentHandHistory.Hero == null)
            {
                return null;
            }

            if (init)
            {
                TempConfig.Init();
                HandHistory.Init();
                Card.Init();
                init = false;
            }

            var handAnalyzer = new HandAnalyzer();

            var handHistory = new HandHistory();

            handHistory.ConverToEquityCalculatorFormat(currentHandHistory, currentStreet);

            var heroRange = handAnalyzer.BuildPlayerRange(handHistory, currentHandHistory.Hero.PlayerName);

            return GroupHands(heroRange);
        }

        internal static Dictionary<MadeHandType, int> GetCombosByHandType(IEnumerable<string> range, string boardCards)
        {
            var result = new Dictionary<MadeHandType, int>();

            void AddResult(MadeHandType handType)
            {
                if (!result.ContainsKey(handType))
                {
                    result.Add(handType, 1);
                    return;
                }

                result[handType]++;
            }

            var ungroupedHands = HandAnalyzer.UngroupHands(range, boardCards, null);

            var boardCard1 = boardCards.Substring(0, 2);
            var boardCard2 = boardCards.Substring(2, 2);
            var boardCard3 = boardCards.Substring(4, 2);
            var boardCard4 = boardCards.Length >= 8 ? boardCards.Substring(6, 2) : null;
            var boardCard5 = boardCards.Length >= 10 ? boardCards.Substring(8, 2) : null;

            // flop cards in binary format
            var fBoardCard1 = HandHistory.fastCard(boardCard1[0], boardCard1[1]);
            var fBoardCard2 = HandHistory.fastCard(boardCard2[0], boardCard2[1]);
            var fBoardCard3 = HandHistory.fastCard(boardCard3[0], boardCard3[1]);
            var fBoardCard4 = boardCard4 != null ? HandHistory.fastCard(boardCard4[0], boardCard4[1]) : 0;
            var fBoardCard5 = boardCard5 != null ? HandHistory.fastCard(boardCard5[0], boardCard5[1]) : 0;

            var board = new[] { fBoardCard1, fBoardCard2, fBoardCard3, fBoardCard4, fBoardCard5 };

            foreach (var holeCards in ungroupedHands)
            {
                var holeCard1 = holeCards.Substring(0, 2);
                var holeCard2 = holeCards.Substring(2, 2);

                var fHoleCard1 = HandHistory.fastCard(holeCard1[0], holeCard1[1]);
                var fHoleCard2 = HandHistory.fastCard(holeCard2[0], holeCard2[1]);

                if (board.Any(x => x == fHoleCard1 || x == fHoleCard2))
                {
                    continue;
                }

                var boardInfo = Jacob.AnalyzeHand(boardCard1, boardCard2, boardCard3, boardCard4, boardCard5, holeCard1, holeCard2);

                var handType = GetHandType(holeCard1, holeCard2, boardInfo, boardCards);

                if (handType == MadeHandType.NoPair)
                {
                    var added = false;

                    if (boardInfo.ifflushdraw)
                    {
                        AddResult(MadeHandType.FlushDraw);
                        added = true;
                    }

                    if (boardInfo.ifstraightdraw)
                    {
                        AddResult(MadeHandType.StraightDraw);
                        added = true;
                    }

                    if (added)
                    {
                        continue;
                    }
                }

                AddResult(handType);
            }

            return result;
        }

        private static MadeHandType GetHandType(string card1, string card2, boardinfo boardInfo, string boardCards)
        {
            if (boardInfo.holesused == 0)
            {
                return MadeHandType.NoPair;
            }

            switch (boardInfo.madehand)
            {
                case PostFlopHand.kPair:
                    var pairType = BluffToValueRatioCalculator.GetPairType(card1, card2, boardInfo, boardCards);

                    switch (pairType)
                    {
                        case PairType.Bottom:
                            return MadeHandType.WeakPair;
                        case PairType.Middle:
                            return MadeHandType.MiddlePair;
                        case PairType.Top:
                            return MadeHandType.TopPair;
                        default:
                            return MadeHandType.NoPair;
                    }
                case PostFlopHand.k2Pair:
                    return MadeHandType.TwoPairs;
                case PostFlopHand.k3ofKind:
                    return MadeHandType.ThreeOfKind;
                case PostFlopHand.k4ofKind:
                    return MadeHandType.FourOfKind;
                case PostFlopHand.kFlush:
                    return MadeHandType.Flush;
                case PostFlopHand.kFullHouse:
                    return MadeHandType.FullHouse;
                case PostFlopHand.kStraight:
                    return MadeHandType.Straight;
                case PostFlopHand.kStraightFlush:
                    return MadeHandType.StraightFlush;
                default:
                    return MadeHandType.NoPair;
            }
        }

        private static IEnumerable<EquityRangeSelectorItemViewModel> GroupHands(List<String> ungroupedHands)
        {
            var cards = new List<string> { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" };

            var list = (from hand in ungroupedHands
                        let card1 = cards.IndexOf(hand[0].ToString()) < cards.IndexOf(hand[2].ToString()) ? hand[0].ToString() : hand[2].ToString()
                        let card2 = cards.IndexOf(hand[0].ToString()) < cards.IndexOf(hand[2].ToString()) ? hand[2].ToString() : hand[0].ToString()
                        let suit = hand[0].Equals(hand[2]) ? "" : hand[1].Equals(hand[3]) ? "s" : "o"
                        group hand by new { card1, card2, suit } into grouped
                        select new EquityRangeSelectorItemViewModel()
                        {
                            ItemLikelihood = Likelihood.Definitely,
                            LikelihoodPercent = (int)(Likelihood.Definitely),
                            FisrtCard = new RangeCardRank().StringToRank(grouped.Key.card1.ToString()),
                            SecondCard = new RangeCardRank().StringToRank(grouped.Key.card2.ToString()),
                            ItemType = new RangeSelectorItemType().StringToRangeItemType(grouped.Key.suit),
                            IsSelected = true
                        }).ToList();

            list.ForEach(x => x.HandUpdateAndRefresh());

            return list.Distinct();
        }
    }
}