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
    public class FullHousePocketPairNoTripsOnBoardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            if (!HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var groupedCards = allCards.GroupBy(x => x.RankNumericValue).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips != null)
            {
                if (groupedCards.Where(x => x.Key != trips.Key).Any(x => x.Count() >= 2))
                {
                    return trips.First().RankNumericValue == playerCards.First().RankNumericValue;
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FullHousePocketPairNoTripsOnBoard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.FullHouse;
        }
    }

    public class FullHousePocketPairTripsOnBoardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            if (!HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var groupedCards = allCards.GroupBy(x => x.RankNumericValue).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips != null)
            {
                var pair = groupedCards.Where(x => x.Key != trips.Key).FirstOrDefault(x => x.Count() >= 2);
                if (pair != null && pair.Key == playerCards.First().RankNumericValue)
                {
                    return true;
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FullHousePocketPairTripsOnBoard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.FullHouse;
        }
    }

    public class FullHouseTwoPocketCardsNoTripsOnBoardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            if (HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var groupedCards = allCards.GroupBy(x => x.RankNumericValue).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips == null)
            {
                return false;
            }

            var pair = groupedCards.Where(x => x.Key != trips.Key).FirstOrDefault(x => x.Count() >= 2);
            if (pair != null)
            {
                return playerCards.Any(x => x.RankNumericValue == trips.Key) && playerCards.Any(x => x.RankNumericValue == pair.Key);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FullHouseTwoPocketCardsNoTripsOnBoard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.FullHouse;
        }
    }

    public class FullHouseOnePocketCardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            if (HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var groupedCards = allCards.GroupBy(x => x.RankNumericValue).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips == null)
            {
                return false;
            }

            var pair = groupedCards.Where(x => x.Key != trips.Key).FirstOrDefault(x => x.Count() >= 2);
            if (pair != null)
            {
                return playerCards.Any(x => x.RankNumericValue == trips.Key || x.RankNumericValue == pair.Key) &&
                    !playerCards.Any(x => x.RankNumericValue == trips.Key || x.RankNumericValue == pair.Key);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FullHouseOnePocketCard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.FullHouse;
        }
    }


    public class FullHouseOneHoleCardNoTripsOnBoardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            if (HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var groupedCards = allCards.GroupBy(x => x.RankNumericValue).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips == null)
            {
                return false;
            }

            var pair = groupedCards.Where(x => x.Key != trips.Key).FirstOrDefault(x => x.Count() >= 2);
            if (pair != null)
            {
                return playerCards.Any(x => x.RankNumericValue == trips.Key) && !playerCards.Any(x => x.RankNumericValue == pair.Key);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FullHouseOneHoleCardNoTripsOnBoard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.FullHouse;
        }
    }

    public class FullHouseOnBoardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || boardCards.Count == 0 || playerCards == null || playerCards.Count() == 0)
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var groupedCards = allCards.GroupBy(x => x.RankNumericValue).OrderByDescending(x => x.Key);

            var trips = groupedCards.FirstOrDefault(x => x.Count() == 3);
            if (trips == null)
            {
                return false;
            }

            var pair = groupedCards.Where(x => x.Key != trips.Key).FirstOrDefault(x => x.Count() >= 2);
            if (pair != null)
            {
                return !playerCards.Any(x => x.RankNumericValue == trips.Key || x.RankNumericValue == pair.Key);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FullHouseOnBoard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.FullHouse;
        }
    }
}
