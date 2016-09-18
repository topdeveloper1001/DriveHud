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
    #region Set Analyzers
    public class ThreeOfAKindTopSetAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (!HandAnalyzerHelpers.IsPair(playerCards, 1) || !HandAnalyzerHelpers.IsNofKind(allCards, 3))
            {
                return false;
            }

            var topBoardCardRank = boardCards.Max(x => x.RankNumericValue);

            if (playerCards.Any(x => x.RankNumericValue == topBoardCardRank))
            {
                return true;
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.ThreeOfAKindTopSet;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }

    public class ThreeOfAKindMiddleSetAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (!HandAnalyzerHelpers.IsPair(playerCards, 1) || !HandAnalyzerHelpers.IsNofKind(allCards, 3))
            {
                return false;
            }

            var middleBoardCardRank = boardCards.OrderByDescending(x => x.RankNumericValue).ElementAt(boardCards.Count / 2).RankNumericValue;

            if (playerCards.Any(x => x.RankNumericValue == middleBoardCardRank))
            {
                return true;
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.ThreeOfAKindMiddleSet;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }

    public class ThreeOfAKindSecondSetAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (!HandAnalyzerHelpers.IsPair(playerCards, 1) || !HandAnalyzerHelpers.IsNofKind(allCards, 3))
            {
                return false;
            }

            var secondBoardCardRank = boardCards.OrderByDescending(x => x.RankNumericValue).ElementAt(1).RankNumericValue;

            if (playerCards.Any(x => x.RankNumericValue == secondBoardCardRank))
            {
                return true;
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.ThreeOfAKindSecondSet;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }

    public class ThreeOfAKindBottomSetAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (!HandAnalyzerHelpers.IsPair(playerCards, 1) || !HandAnalyzerHelpers.IsNofKind(allCards, 3))
            {
                return false;
            }

            var bottomBoardCardRank = boardCards.Min(x => x.RankNumericValue);

            if (playerCards.Any(x => x.RankNumericValue == bottomBoardCardRank))
            {
                return true;
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.ThreeOfAKindBottomSet;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }

    #endregion

    #region Trips Analyzers
    public class ThreeOfAKindTripsHighKickerAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) || HandAnalyzerHelpers.IsNofKind(boardCards, 3) || !HandAnalyzerHelpers.IsNofKind(allCards, 3))
            {
                return false;
            }

            bool isThreeOfAKind = boardCards.Where(x => x.RankNumericValue == playerCards.ElementAt(0).RankNumericValue).Count() == 2
                                || boardCards.Where(x => x.RankNumericValue == playerCards.ElementAt(1).RankNumericValue).Count() == 2;

            if (isThreeOfAKind)
            {
                var kicker = playerCards.FirstOrDefault(p => !boardCards.Any(b => b.Rank == p.Rank));
                if (kicker != null)
                {
                    return HandAnalyzerHelpers.IsDecentKicker(kicker);
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.ThreeOfAKindTripsHighKicker;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }

    public class ThreeOfAKindTripsTopSetHighKickerAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) || HandAnalyzerHelpers.IsNofKind(boardCards, 3) || !HandAnalyzerHelpers.IsNofKind(allCards, 3))
            {
                return false;
            }

            if (boardCards.Count(x => x.RankNumericValue == boardCards.Max(b => b.RankNumericValue)) == 2
                && playerCards.Any(x => x.RankNumericValue == boardCards.Max(b => b.RankNumericValue)))
            {
                var kicker = playerCards.FirstOrDefault(p => !boardCards.Any(b => b.Rank == p.Rank));
                if (kicker != null)
                {
                    return HandAnalyzerHelpers.IsDecentKicker(kicker);
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.ThreeOfAKindTripsTopSetHighKicker;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }

    public class ThreeOfAKindTripsTopSetWeakKickerAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) || HandAnalyzerHelpers.IsNofKind(boardCards, 3) || !HandAnalyzerHelpers.IsNofKind(allCards, 3))
            {
                return false;
            }

            if (boardCards.Count(x => x.RankNumericValue == boardCards.Max(b => b.RankNumericValue)) == 2
                && playerCards.Any(x => x.RankNumericValue == boardCards.Max(b => b.RankNumericValue)))
            {
                var kicker = playerCards.FirstOrDefault(p => !boardCards.Any(b => b.Rank == p.Rank));
                if (kicker != null)
                {
                    return HandAnalyzerHelpers.IsWeakKicker(kicker);
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.ThreeOfAKindTripsTopSetWeakKicker;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }

    public class ThreeOfAKindTripsSecondSetHighKickerAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) || HandAnalyzerHelpers.IsNofKind(boardCards, 3) || !HandAnalyzerHelpers.IsNofKind(allCards, 3))
            {
                return false;
            }

            var secondSetCard = boardCards.OrderByDescending(x => x.RankNumericValue).ElementAt(1);
            if (boardCards.Count(x => x.RankNumericValue == secondSetCard.RankNumericValue) == 2
                && playerCards.Any(x => x.RankNumericValue == secondSetCard.RankNumericValue))
            {
                var kicker = playerCards.FirstOrDefault(p => !boardCards.Any(b => b.Rank == p.Rank));
                if (kicker != null)
                {
                    return HandAnalyzerHelpers.IsDecentKicker(kicker);
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.ThreeOfAKindTripsSecondSetHighKicker;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }

    public class ThreeOfAKindTripsSecondSetWeakKickerAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) || HandAnalyzerHelpers.IsNofKind(boardCards, 3) || !HandAnalyzerHelpers.IsNofKind(allCards, 3))
            {
                return false;
            }

            var secondSetCard = boardCards.OrderByDescending(x => x.RankNumericValue).ElementAt(1);
            if (boardCards.Count(x => x.RankNumericValue == secondSetCard.RankNumericValue) == 2
                && playerCards.Any(x => x.RankNumericValue == secondSetCard.RankNumericValue))
            {
                var kicker = playerCards.FirstOrDefault(p => !boardCards.Any(b => b.Rank == p.Rank));
                if (kicker != null)
                {
                    return HandAnalyzerHelpers.IsWeakKicker(kicker);
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.ThreeOfAKindTripsSecondSetWeakKicker;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }

    public class ThreeOfAKindTripsWeakKickerAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) || !HandAnalyzerHelpers.IsNofKind(allCards, 3) || HandAnalyzerHelpers.IsNofKind(boardCards, 3))
            {
                return false;
            }

            var kicker = playerCards.FirstOrDefault(p => !boardCards.Any(b => b.Rank == p.Rank));
            if (kicker != null)
            {
                return HandAnalyzerHelpers.IsWeakKicker(kicker);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.ThreeOfAKindTripsWeakKicker;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }

    public class ThreeOfAKindTripsOnFlopAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) || !HandAnalyzerHelpers.IsNofKind(allCards, 3))
            {
                return false;
            }

            var flopCards = boardCards.GetBoardOnStreet(Street.Flop);
            var flopPair = boardCards.GroupBy(x => x.Rank).FirstOrDefault(x => x.Count() == 2);
            if (flopPair != null)
            {
                return flopPair.Any(f => playerCards.Any(p => p.CardIntValue == f.CardIntValue));
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.ThreeOfAKindTripsOnFlop;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }

    #endregion
    public class ThreeOfAKindOnBoardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || boardCards.Count < 3)
            {
                return false;
            }

            return HandAnalyzerHelpers.IsNofKind(boardCards, 3);
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.OnBoardThreeOfAKind;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Trips;
        }
    }
}
