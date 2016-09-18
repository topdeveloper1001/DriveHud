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
    public class StraightFlushTwoPocketCardsAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (!HandAnalyzerHelpers.IsFlush(playerCards))
            {
                return false;
            }

            var sortedBoard = boardCards.OrderBy(x => x.RankNumericValue);

            for (int i = 0; i < sortedBoard.Count() - 3; i++)
            {
                var cards = sortedBoard.Take(3).ToList();

                if (cards.Any(x => x.Suit != playerCards.First().Suit))
                {
                    return false;
                }

                cards.AddRange(playerCards);
                if (HandAnalyzerHelpers.IsStraight(cards, true))
                {
                    return true;
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightFlushTwoPocketCards;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.StraightFlush;
        }
    }

    public class StraightFlushOneHoleCardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            if (!HandAnalyzerHelpers.IsFlush(playerCards))
            {
                return false;
            }

            var sortedBoard = boardCards.OrderBy(x => x.RankNumericValue);

            for (int i = 0; i < sortedBoard.Count() - 4; i++)
            {
                var cards = sortedBoard.Take(4).ToList();

                if (!HandAnalyzerHelpers.IsFlush(cards))
                {
                    return false;
                }

                foreach (var card in playerCards)
                {
                    var straightFlushCards = new List<Cards.Card>(cards);
                    straightFlushCards.Add(card);
                    if (HandAnalyzerHelpers.IsFlush(straightFlushCards) && HandAnalyzerHelpers.IsStraight(straightFlushCards, true))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightFlushOneHoleCard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.StraightFlush;
        }
    }

    public class StraightFlushOnBoardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || boardCards.Count() == 0)
                return false;

            if (HandAnalyzerHelpers.IsFlush(boardCards) && HandAnalyzerHelpers.IsStraight(boardCards, true))
            {
                return true;
            }
            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.OnBoardStraightFlush;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.StraightFlush;
        }
    }

}
