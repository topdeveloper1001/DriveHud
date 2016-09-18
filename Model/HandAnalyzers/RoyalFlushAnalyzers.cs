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
    public class RoyalFlushTwoPocketCardsAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            if (HandAnalyzerHelpers.IsFlush(playerCards))
            {
                var allCards = new List<HandHistories.Objects.Cards.Card>(playerCards);
                var boardCopy = new List<HandHistories.Objects.Cards.Card>(boardCards.Where(x => x.Suit == allCards.First().Suit));
                allCards.AddRange(boardCopy);

                var royalFlushCards = HandAnalyzerHelpers.GetRoyalFlushCards(allCards);
                if (royalFlushCards != null)
                {
                    if (!playerCards.Any(c => !royalFlushCards.Contains(c)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.RoyalFlushTwoPocketCards;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.StraightFlush;
        }
    }

    public class RoyalFlushOneHoleCardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            var allCards = new List<HandHistories.Objects.Cards.Card>(playerCards);
            var boardCopy = new List<HandHistories.Objects.Cards.Card>(boardCards.Where(x => x.Suit == allCards.First().Suit));
            allCards.AddRange(boardCopy);

            var royalFlushCards = HandAnalyzerHelpers.GetRoyalFlushCards(allCards);
            if (royalFlushCards != null)
            {
                if (playerCards.Any(c => royalFlushCards.Contains(c)) && playerCards.Any(c => !royalFlushCards.Contains(c)))
                {
                    return true;
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.RoyalFlushOneHoleCard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.StraightFlush;
        }
    }

    public class RoyalFlushOnBoardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || boardCards.Count() == 0)
                return false;

            if (HandAnalyzerHelpers.IsFlush(boardCards) && HandAnalyzerHelpers.IsStraight(boardCards) && boardCards.Any(x => x.Rank == HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.First()))
            {
                return true;
            }
            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.OnBoardRoyalFlush;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.StraightFlush;
        }
    }


}
