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
    public class TwoPairNoTopPairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 2)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 3) || HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var pairs = allCards.GroupBy(x => x.RankNumericValue).Where(x => x.Count() == 2);

            if (pairs != null && pairs.Count() > 1)
            {
                return boardCards.Any(x => x.RankNumericValue > pairs.Max(p => p.Max(m => m.RankNumericValue)));
            }

            return false;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.TwoPair;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.TwoPairNoTopPair;
        }
    }

    #region Both Cards Paired

    public class TwoPairTopTwoPairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 2)
                return false;

            if (HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 3) || HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var topTwo = boardCards.OrderByDescending(x => x.RankNumericValue).Take(2).Distinct();
            if (topTwo != null && topTwo.Count() == 2)
            {
                return playerCards.All(x => topTwo.Any(t => t.RankNumericValue == x.RankNumericValue));
            }

            return false;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.TwoPair;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.TwoPairTopTwoPair;
        }
    }

    public class TwoPairTopAndBottomPairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 2)
                return false;

            if (HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 3) || HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var pairs = allCards.GroupBy(x => x.RankNumericValue).OrderByDescending(x => x.Key).Where(x => x.Count() == 2);
            var sortedBoard = boardCards.OrderBy(x => x.RankNumericValue);

            if (pairs != null && pairs.Count() > 1 && sortedBoard != null && sortedBoard.Count() >= 2)
            {
                pairs = pairs.Take(2);
                var bottomTwo = boardCards.OrderBy(x => x.RankNumericValue).Take(2).Distinct();

                if (bottomTwo != null && bottomTwo.Count() == 2)
                {
                    return pairs.Any(x => x.Key == sortedBoard.First().RankNumericValue)
                        && pairs.Any(x => x.Key == sortedBoard.Last().RankNumericValue)
                        && playerCards.Any(x => x.RankNumericValue == sortedBoard.First().RankNumericValue)
                        && playerCards.Any(x => x.RankNumericValue == sortedBoard.Last().RankNumericValue);
                }
            }

            return false;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.TwoPair;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.TwoPairTopAndBottomPair;
        }
    }

    public class TwoPairBottomTwoPairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 2)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 3) || HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var pairs = allCards.GroupBy(x => x.RankNumericValue).OrderByDescending(x => x.Key).Where(x => x.Count() == 2);
            if (pairs != null && pairs.Count() > 1)
            {
                var bottomTwo = boardCards.OrderBy(x => x.RankNumericValue).Take(2).Distinct();
                if (bottomTwo != null && bottomTwo.Count() == 2)
                {
                    return pairs.Take(2).All(p => bottomTwo.Any(t => t.RankNumericValue == p.Key))
                        && playerCards.All(x => bottomTwo.Any(t => t.RankNumericValue == x.RankNumericValue));
                }
            }

            return false;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.TwoPair;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.TwoPairBottomTwoPair;
        }
    }

    #endregion

    #region Paired Board

    public class TwoPairPairedBoardPocketPairOverpairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 2)
                return false;

            if (!HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            var pairs = allCards.GroupBy(x => x.RankNumericValue).Where(x => x.Count() == 2);
            if (pairs != null && pairs.Count() > 1)
            {
                return playerCards.First().RankNumericValue > boardCards.Max(x => x.RankNumericValue);
            }

            return false;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.TwoPair;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.TwoPairPairedBoardPocketPairOverpair;
        }
    }

    public class TwoPairPairedBoardPocketPairSecondPairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 2)
                return false;

            if (!HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 3) || HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var pairs = allCards.GroupBy(x => x.RankNumericValue).Where(x => x.Count() == 2);
            if (pairs != null && pairs.Count() > 1)
            {
                return pairs.ElementAt(1).Key == playerCards.First().RankNumericValue;
            }

            return false;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.TwoPair;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.TwoPairPairedBoardPocketPairSecondPair;
        }
    }

    public class TwoPairPairedBoardTopPairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 2)
                return false;

            if (HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 3) || HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var pairs = allCards.GroupBy(x => x.RankNumericValue).Where(x => x.Count() == 2);
            if (pairs != null && pairs.Count() > 1)
            {
                var topPair = boardCards.Max(x => x.RankNumericValue);
                return pairs.First().Key == topPair
                    && playerCards.Any(x => x.RankNumericValue == topPair)
                    && !playerCards.Any(x => pairs.ElementAt(1).Key == x.RankNumericValue);
            }

            return false;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.TwoPair;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.TwoPairPairedBoardTopPair;
        }
    }

    public class TwoPairPairedBoardBottomPairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 2)
                return false;

            if (HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 3) || HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            var pairs = allCards.GroupBy(x => x.RankNumericValue).Where(x => x.Count() == 2);
            if (pairs != null && pairs.Count() > 1)
            {
                var bottomPair = boardCards.Min(x => x.RankNumericValue);
                return pairs.ElementAt(1).Key == bottomPair
                    && playerCards.Any(x => x.RankNumericValue == bottomPair)
                    && !playerCards.Any(x => pairs.First().Key == x.RankNumericValue);
            }

            return false;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.TwoPair;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.TwoPairPairedBoardBottomPair;
        }
    }

    #endregion

    public class TwoPairOnBoardAnaluzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || playerCards == null || playerCards.Count() == 0 || boardCards.Count == 0)
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsNofKind(allCards, 3) || HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                return false;
            }

            return !HandAnalyzerHelpers.IsPair(playerCards, 1)
                   && !playerCards.Any(p => boardCards.Any(b => b.Rank == p.Rank))
                   && HandAnalyzerHelpers.IsPair(boardCards, 2);
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.TwoPair;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.OnBoardTwoPair;
        }
    }
}
