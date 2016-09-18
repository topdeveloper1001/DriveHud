using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Objects.Cards;
using Cards = HandHistories.Objects.Cards;
using Model.Enums;
using HoldemHand;

namespace Model.HandAnalyzers
{
    public class FourOfAKindPocketPairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            if (!HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            return boardCards.GroupBy(x => x.Rank).Any(g => (g.Count() == 2) && (g.Key == playerCards.First().Rank));
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FourOfAKindPocketPair;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.FourOfAKind;
        }
    }

    public class FourOfAKindNoPocketPairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            if (HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var trips = boardCards.GroupBy(x => x.Rank).FirstOrDefault(x => x.Count() == 3);
            if(trips != null)
            {
                return playerCards.Any(x => x.Rank == trips.Key);
            }


            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FourOfAKindNoPocketPair;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.FourOfAKind;
        }
    }


    public class FourOfAKindOnBoardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || boardCards.Count == 0)
            {
                return false;
            }

            return HandAnalyzerHelpers.IsNofKind(boardCards, 4);
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.OnBoardFourOfAKind;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.FourOfAKind;
        }
    }

}
