using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Objects.Cards;
using HoldemHand;
using Model.Enums;

namespace Model.HandAnalyzers
{
    public class LessThanMidPairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            var pairs = allCards.GroupBy(x => x.RankNumericValue).Where(x => x.Count() > 1);
            if (pairs.Count() == 0)
            {
                return true;
            }

            if (pairs.Count() == 1)
            {
                var midIndex = boardCards.Count / 2;
                if (midIndex < boardCards.Count)
                {
                    var midCard = boardCards.OrderByDescending(x => x.RankNumericValue).ElementAt(midIndex).RankNumericValue;
                    return pairs.First().Key < midIndex;
                }
            }

            return false;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Pair;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.LessThanMidPair;
        }
    }

    public class TopPairOrBetter : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            HoldemHand.Hand hand = new HoldemHand.Hand(string.Join("", playerCards.Select(c => c.CardStringValue)), boardCards.ToString());

            if (hand.HandTypeValue > HoldemHand.Hand.HandTypes.Pair)
            {
                return true;
            }

            if (hand.HandTypeValue == HoldemHand.Hand.HandTypes.Pair)
            {
                return playerCards.Any(x => x.RankNumericValue == boardCards.Max(b => b.RankNumericValue));
            }

            return false;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Pair;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.TopPairOrBetter;
        }
    }
}
