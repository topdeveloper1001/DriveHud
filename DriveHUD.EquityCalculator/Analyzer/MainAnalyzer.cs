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

        internal static void GetHeroRange(HandHistories.Objects.Hand.HandHistory currentHandHistory, HandHistories.Objects.Cards.Street currentStreet, 
            out string strongestOpponentName, out IEnumerable<EquityRangeSelectorItemViewModel> strongestOpponentHands)
        {
            strongestOpponentName = null;
            strongestOpponentHands = new List<EquityRangeSelectorItemViewModel>();

            if (currentHandHistory.Hero == null)
            {
                return;
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

            var street = currentStreet == HandHistories.Objects.Cards.Street.Flop ? 1 :
                              currentStreet == HandHistories.Objects.Cards.Street.Turn ? 2 :
                               currentStreet == HandHistories.Objects.Cards.Street.River ? 3 : 0;
            

            handAnalyzer.BuildPlayerRange(handHistory, street, hand_collective, currentHandHistory.Hero.PlayerName);

            strongestOpponentHands = GroupHands(handAnalyzer.StrongestOpponentHands);
            strongestOpponentName = handAnalyzer.StrongestOpponentName;
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