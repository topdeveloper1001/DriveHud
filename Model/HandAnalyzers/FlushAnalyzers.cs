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
    public class FlushTwoCardNutAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (!HandAnalyzerHelpers.IsFlush(playerCards))
            {
                return false;
            }

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                if (playerCards.All(x => flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue)))
                {
                    var nutCardSuit = flushBoardCards.First().Suit;
                    var nutCardRank = Cards.Card.PossibleRanksHighCardFirst.First(rank => !boardCards.Where(b => b.Suit == nutCardSuit).Any(b => b.Rank == rank));
                    return playerCards.Any(x => x.Rank == nutCardRank);
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushTwoCardNut;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

    public class FlushTwoCardHighAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (!HandAnalyzerHelpers.IsFlush(playerCards))
            {
                return false;
            }

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                if (playerCards.All(x => flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue)))
                {
                    return playerCards.Any(x => HandAnalyzerHelpers.IsDecentKicker(x));
                }
            }
            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushTwoCardHigh;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

    public class FlushTwoCardLowAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (!HandAnalyzerHelpers.IsFlush(playerCards))
            {
                return false;
            }

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                if (playerCards.All(x => flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue)))
                {
                    return playerCards.All(x => HandAnalyzerHelpers.IsWeakKicker(x));
                }
            }
            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushTwoCardLow;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

    public class FlushOneCardNutAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                if (playerCards.Any(x => flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue))
                    && playerCards.Any(x => !flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue)))
                {
                    var nutCardSuit = flushBoardCards.First().Suit;
                    var nutCardRank = Cards.Card.PossibleRanksHighCardFirst.First(rank => !boardCards.Where(b => b.Suit == nutCardSuit).Any(b => b.Rank == rank));
                    return playerCards.Any(x => x.Rank == nutCardRank && x.Suit == nutCardSuit);
                }
            }
            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushOneCardNut;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

    public class FlushOneCardHighAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                if (playerCards.Any(x => flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue) && HandAnalyzerHelpers.IsDecentKicker(x))
                    && playerCards.Any(x => !flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue)))
                {
                    return true;
                }
            }
            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushOneCardHigh;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

    public class FlushOneCardLowAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                if (playerCards.Any(x => flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue) && HandAnalyzerHelpers.IsWeakKicker(x))
                    && playerCards.Any(x => !flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue)))
                {
                    return true;
                }
            }
            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushOneCardLow;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

    public class FlushNutAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                var nutCardSuit = flushBoardCards.First().Suit;
                var nutCardRank = Cards.Card.PossibleRanksHighCardFirst.First();
                return flushBoardCards.Any(x => x.Rank == nutCardRank && x.Suit == nutCardSuit);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushNut;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

    public class FlushHighAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                return HandAnalyzerHelpers.IsDecentKicker(flushBoardCards.OrderByDescending(x => x.RankNumericValue).First()); 
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushHigh;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

    public class FlushLowAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                return HandAnalyzerHelpers.IsWeakKicker(flushBoardCards.OrderByDescending(x => x.RankNumericValue).First());
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushLow;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }


    public class FlushOnBoardAnalyzer : IAnalyzer
    {
        public virtual bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || boardCards.Count == 0 || playerCards == null || playerCards.Count() == 0)
            {
                return false;
            }

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                return playerCards.All(x => !flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue));
            }
            return false;
        }

        public virtual ShowdownHands GetRank()
        {
            return ShowdownHands.FlushOnBoard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

    public class FlushOnBoardNutAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || boardCards.Count == 0 || playerCards == null || playerCards.Count() == 0)
            {
                return false;
            }

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                if (playerCards.All(x => !flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue)))
                {
                    return flushBoardCards.Max(x => x.RankNumericValue) == Cards.Card.GetRankNumericValue(Cards.Card.PossibleRanksHighCardFirst.First());
                }
            }
            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushOnBoardNut;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

    public class FlushOnBoardHighAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || boardCards.Count == 0 || playerCards == null || playerCards.Count() == 0)
            {
                return false;
            }

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                if (playerCards.All(x => !flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue)))
                {
                    return HandAnalyzerHelpers.IsDecentKicker(flushBoardCards.First());
                }
            }
            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushOnBoardHigh;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

    public class FlushOnBoardLowAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || boardCards.Count == 0 || playerCards == null || playerCards.Count() == 0)
            {
                return false;
            }

            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);

            var suitGroup = allCards.GroupBy(x => x.Suit);
            if (suitGroup.Any(g => g.Count() >= 5))
            {
                var flushBoardCards = suitGroup.Where(g => g.Count() >= 5).First().OrderByDescending(x => x.RankNumericValue).Take(5);
                if (playerCards.All(x => !flushBoardCards.Any(f => f.CardStringValue == x.CardStringValue)))
                {
                    return HandAnalyzerHelpers.IsWeakKicker(flushBoardCards.First());
                }
            }
            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushOnBoardLow;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Flush;
        }
    }

}
