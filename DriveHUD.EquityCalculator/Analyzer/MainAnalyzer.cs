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

using DriveHUD.ViewModels;
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

        internal static void GetStrongestOpponent(HandHistories.Objects.Hand.HandHistory currentHandHistory, HandHistories.Objects.Cards.Street currentStreet, out string strongestOpponentName, out IEnumerable<RangeSelectorItemViewModel> strongestOpponentHands)
        {
            strongestOpponentName = null;
            strongestOpponentHands = new List<RangeSelectorItemViewModel>();

            if (init)
            {
                TempConfig.Init();
                HandHistory.Init();
                Card.Init();
                init = false;
            }
            HandAnalyzer handAnalyzer = new HandAnalyzer();
            HandHistory handHistory = new HandHistory();
            handHistory.ConverToEquityCalculatorFormat(currentHandHistory, currentStreet);

            Hashtable hand_range = handAnalyzer.PreflopAnalysis(handHistory);

            Hashtable hand_collective = new Hashtable();
            foreach (String key in hand_range.Keys)
            {
                hand_collective.Add(key, new hand_distribution());
                (hand_collective[key] as hand_distribution).hand_range = (float)Convert.ToDouble(hand_range[key]);
            }
            hand_collective = handAnalyzer.PostflopAnalysis(handHistory, 1, hand_collective); // Flop
            hand_collective = handAnalyzer.PostflopAnalysis(handHistory, 2, hand_collective);	// Turn
            hand_collective = handAnalyzer.PostflopAnalysis(handHistory, 3, hand_collective);	// River

            strongestOpponentHands = GroupHands(handAnalyzer.StrongestOpponentHands);
            strongestOpponentName = handAnalyzer.StrongestOpponentName;
        }

        private static IEnumerable<RangeSelectorItemViewModel> GroupHands(List<String> ungroupedHands)
        {
            List<String> cards = new List<String>(new String[] { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" });
            //CHANGE 6s7h to 76o
            List<RangeSelectorItemViewModel> list = new List<RangeSelectorItemViewModel>();
            foreach (String hand in ungroupedHands)
            {
                String card1 = cards.IndexOf(hand[0].ToString()) < cards.IndexOf(hand[2].ToString()) ? hand[0].ToString() : hand[2].ToString();
                String card2 = cards.IndexOf(hand[0].ToString()) < cards.IndexOf(hand[2].ToString()) ? hand[2].ToString() : hand[0].ToString();
                String suit = hand[0].Equals(hand[2]) ? "" : hand[1].Equals(hand[3]) ? "s" : "o";



                list.Add(new RangeSelectorItemViewModel()
                {
                    ItemLikelihood = Likelihood.Definitely,
                    LikelihoodPercent = (int)(Likelihood.Definitely),
                    FisrtCard = new RangeCardRank().StringToRank(card1.ToString()),
                    SecondCard = new RangeCardRank().StringToRank(card2.ToString()),
                    ItemType = new RangeSelectorItemType().StringToRangeItemType(suit),
                    IsSelected = true
                });

                
            }
            list.ForEach(x => x.HandUpdateAndRefresh());

            return list.Distinct();
        }


    }
}
