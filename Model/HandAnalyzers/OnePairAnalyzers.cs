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
    #region  Pocket Pairs
    public class PocketPairOverpairAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) && HandAnalyzerHelpers.IsPair(allCards, 1))
            {
                if (boardCards.All(x => x.RankNumericValue < playerCards.First().RankNumericValue))
                {
                    return true;
                }
            }
            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.PocketPairOverpair;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Pair;
        }
    }

    public class PocketPairSecondOrWorseAnalyzer : IAnalyzer
    {
        public virtual bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) && HandAnalyzerHelpers.IsPair(allCards, 1))
            {
                if (boardCards.Any(x => x.RankNumericValue > playerCards.First().RankNumericValue))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual ShowdownHands GetRank()
        {
            return ShowdownHands.PocketPairSecondOrWorse;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Pair;
        }
    }

    public class PocketPairSecondPairAnalyzer : PocketPairSecondOrWorseAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                return boardCards.Count(x => x.RankNumericValue > playerCards.First().RankNumericValue) == 1;
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.PocketPairSecondPair;
        }
    }

    public class PocketPairThirdPairAnalyzer : PocketPairSecondOrWorseAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                return boardCards.Count(x => x.RankNumericValue > playerCards.First().RankNumericValue) == 2;
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.PocketPairThirdPair;
        }
    }

    public class PocketPairFourthPairAnalyzer : PocketPairSecondOrWorseAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                return boardCards.Count(x => x.RankNumericValue > playerCards.First().RankNumericValue) == 3;
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.PocketPairFourthPair;
        }
    }

    public class PocketPairUnderPairAnalyzer : PocketPairSecondOrWorseAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                return !boardCards.Any(x => x.RankNumericValue <= playerCards.First().RankNumericValue);
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.PocketPairUnderPair;
        }
    }
    #endregion

    #region Top Pair
    public class TopPairAnalyzer : IAnalyzer
    {
        public virtual bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) || !HandAnalyzerHelpers.IsPair(allCards, 1))
            {
                return false;
            }

            if (playerCards.Any(c => c.RankNumericValue == boardCards.Max(x => x.RankNumericValue)))
            {
                return true;
            }

            return false;
        }

        public virtual ShowdownHands GetRank()
        {
            return ShowdownHands.TopPair;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Pair;
        }
    }

    public class TopPairTopKickerAnalyzer : TopPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsTopKicker(unpairedCard, boardCards);
                }
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.TopPairTopKicker;
        }
    }

    public class TopPairDecentKickerAnalyzer : TopPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsDecentKicker(unpairedCard);
                }
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.TopPairDecentKicker;
        }
    }

    public class TopPairWeakKickerAnalyzer : TopPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsWeakKicker(unpairedCard);
                }
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.TopPairWeakKicker;
        }
    }
    #endregion

    #region Second Pair
    public class SecondPairAnalyzer : IAnalyzer
    {
        public virtual bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 1)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) || !HandAnalyzerHelpers.IsPair(allCards, 1))
            {
                return false;
            }

            var secondCardRank = boardCards.OrderByDescending(x => x.RankNumericValue).ElementAt(1).RankNumericValue;
            if (playerCards.Any(c => c.RankNumericValue == secondCardRank))
            {
                return true;
            }

            return false;
        }

        public virtual ShowdownHands GetRank()
        {
            return ShowdownHands.SecondPair;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Pair;
        }
    }

    public class SecondPairTopKickerAnalyzer : SecondPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsTopKicker(unpairedCard, boardCards);
                }
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.SecondPairTopKicker;
        }

    }

    public class SecondPairDecentKickerAnalyzer : SecondPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsDecentKicker(unpairedCard);
                }
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.SecondPairDecentKicker;
        }
    }


    public class SecondPairWeakKickerAnalyzer : SecondPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsWeakKicker(unpairedCard);
                }
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.SecondPairWeakKicker;
        }
    }
    #endregion

    #region Third Pair

    public class ThirdPairAnalyzer : IAnalyzer
    {
        public virtual bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 2)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) || !HandAnalyzerHelpers.IsPair(allCards, 1))
            {
                return false;
            }

            var thirdCardRank = boardCards.OrderByDescending(x => x.RankNumericValue).ElementAt(2).RankNumericValue;
            if (playerCards.Any(c => c.RankNumericValue == thirdCardRank))
            {
                return true;
            }

            return false;
        }

        public virtual ShowdownHands GetRank()
        {
            return ShowdownHands.ThirdPair;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Pair;
        }
    }

    public class ThirdPairTopKickerAnalyzer : ThirdPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsTopKicker(unpairedCard, boardCards);
                }
            }
            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.ThirdPairTopKicker;
        }
    }

    public class ThirdPairDecentKickerAnalyzer : ThirdPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsDecentKicker(unpairedCard);
                }
            }
            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.ThirdPairDecentKicker;
        }
    }

    public class ThirdPairWeakKickerAnalyzer : ThirdPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsWeakKicker(unpairedCard);
                }
            }
            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.ThirdPairWeakKicker;
        }
    }
    #endregion

    #region Bottom Pair

    public class BottomPairAnalyzer : IAnalyzer
    {
        public virtual bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() == 0)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(boardCards, playerCards);

            if (HandAnalyzerHelpers.IsPair(playerCards, 1) || !HandAnalyzerHelpers.IsPair(allCards, 1))
            {
                return false;
            }

            if (playerCards.Any(c => c.RankNumericValue == boardCards.Min(x => x.RankNumericValue)))
            {
                return true;
            }

            return false;
        }

        public virtual ShowdownHands GetRank()
        {
            return ShowdownHands.BottomPair;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Pair;
        }
    }

    public class BottomPairTopKickerAnalyzer : BottomPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsTopKicker(unpairedCard, boardCards);
                }
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.BottomPairTopKicker;
        }
    }

    public class BottomPairDecentKickerAnalyzer : BottomPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsDecentKicker(unpairedCard);
                }
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.BottomPairDecentKicker;
        }
    }

    public class BottomPairWeakKickerAnalyzer : BottomPairAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                var unpairedCard = HandAnalyzerHelpers.GetUnpairedCard(playerCards, boardCards);
                if (unpairedCard != null)
                {
                    return HandAnalyzerHelpers.IsWeakKicker(unpairedCard);
                }
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.BottomPairWeakKicker;
        }
    }
    #endregion

    #region Paired Board
    public class PairedBoardAnalyzer : IAnalyzer
    {
        public virtual bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || boardCards.Count == 0)
            {
                return false;
            }

            return !HandAnalyzerHelpers.IsPair(playerCards, 1) 
                && !playerCards.Any(p => boardCards.Any(b => b.Rank == p.Rank))
                && HandAnalyzerHelpers.IsPair(boardCards, 1);
        }

        public virtual ShowdownHands GetRank()
        {
            return ShowdownHands.PairedBoard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Pair;
        }
    }

    public class PairedBoardNoOvercardsAnalyzer : PairedBoardAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if(base.Analyze(playerCards, boardCards))
            {
                return HandAnalyzerHelpers.IsNoOvercards(playerCards, boardCards);
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.PairedBoardNoOvercards;
        }
    }

    public  class PairedBoardOneOvercardAnalyzer : PairedBoardAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                return HandAnalyzerHelpers.IsOneOvercard(playerCards, boardCards);
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.PairedBoardOneOvercard;
        }
    }

    public class PairedBoardTwoOvercardsAnalyzer : PairedBoardAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                return HandAnalyzerHelpers.IsTwoOvercards(playerCards, boardCards);
            }

            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.PairedBoardTwoOvercards;
        }
    }

    #endregion
}
