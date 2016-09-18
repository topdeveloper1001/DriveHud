using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Objects.Cards;
using Model.Enums;
using HoldemHand;

namespace Model.HandAnalyzers
{
    #region Two Card
    public class FlushDrawTwoCardNutAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2))
            {
                return false;
            }

            return HandAnalyzerHelpers.IsTopKicker(playerCards.OrderByDescending(x => x.RankNumericValue).First(), boardCards.Where(b => b.Suit == playerCards.First().Suit));
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushDrawTwoCardNut;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsFlushDraw(hand.MaskValue, 0UL);
        }
    }

    public class FlushDrawTwoCardHighAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2))
            {
                return false;
            }

            return HandAnalyzerHelpers.IsDecentKicker(playerCards.OrderByDescending(x => x.RankNumericValue).First());
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushDrawTwoCardHigh;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsFlushDraw(hand.MaskValue, 0UL);
        }
    }

    public class FlushDrawTwoCardLowAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2))
            {
                return false;
            }

            return HandAnalyzerHelpers.IsWeakKicker(playerCards.OrderByDescending(x => x.RankNumericValue).First());
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushDrawTwoCardLow;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsFlushDraw(hand.MaskValue, 0UL);
        }
    }
    #endregion

    #region One Card
    public class FlushDrawOneCardNutAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 1))
            {
                return false;
            }

            var drawSuit = boardCards.GroupBy(x => x.Suit).FirstOrDefault(x => x.Count() == 3).Key;

            return (drawSuit != null) && HandAnalyzerHelpers.IsTopKicker(playerCards.OrderByDescending(x => x.RankNumericValue).First(x => x.Suit == drawSuit), boardCards.Where(b => b.Suit == drawSuit));
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushDrawOneCardNut;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsFlushDraw(hand.MaskValue, 0UL);
        }
    }

    public class FlushDrawOneCardHighAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 1))
            {
                return false;
            }

            var drawSuit = boardCards.GroupBy(x => x.Suit).FirstOrDefault(x => x.Count() == 3).Key;
            return (drawSuit != null) && HandAnalyzerHelpers.IsDecentKicker(playerCards.OrderByDescending(x => x.RankNumericValue).First(x => x.Suit == drawSuit));
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushDrawOneCardHigh;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsFlushDraw(hand.MaskValue, 0UL);
        }
    }

    public class FlushDrawOneCardLowAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 1))
            {
                return false;
            }

            var drawSuit = boardCards.GroupBy(x => x.Suit).FirstOrDefault(x => x.Count() == 3).Key;
            return (drawSuit != null) && HandAnalyzerHelpers.IsWeakKicker(playerCards.OrderByDescending(x => x.RankNumericValue).First(x => x.Suit == drawSuit));
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushDrawOneCardLow;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsFlushDraw(hand.MaskValue, 0UL);
        }
    }
    #endregion

    #region Backdoor
    public class FlushDrawBackdoorTwoCardNutAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (!HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2, isBackdoor: true))
            {
                return false;
            }

            return HandAnalyzerHelpers.IsTopKicker(playerCards.OrderByDescending(x => x.RankNumericValue).First(), boardCards.Where(b => b.Suit == playerCards.First().Suit));

        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushDrawBackdoorTwoCardNut;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return true;
        }
    }

    public class FlushDrawBackdoorTwoCardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            return HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2, isBackdoor: true);
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushDrawBackdoorTwoCard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return true;
        }
    }

    public class FlushDrawBackdoorOneCardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            return HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 1, isBackdoor: true);
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushDrawBackdoorOneCard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return true;
        }
    }

    #endregion

    public class FlushDrawNoFlushDrawAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            return !HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 0)
                && !HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 1)
                && !HandAnalyzerHelpers.IsFlushDraw(playerCards, boardCards, 2);
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.FlushNoFlushDraw;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return !Hand.IsFlushDraw(hand.MaskValue, 0UL);
        }
    }

}
