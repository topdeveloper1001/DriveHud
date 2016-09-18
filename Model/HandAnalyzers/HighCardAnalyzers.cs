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
    public class HighCardAnalyzer : IAnalyzer
    {
        public virtual bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null)
            {
                return false;
            }

            var allCards = boardCards.ToList();
            allCards.AddRange(playerCards);

            if (!HandAnalyzerHelpers.IsPair(allCards, 1) && !HandAnalyzerHelpers.IsPair(allCards, 2) && !HandAnalyzerHelpers.IsNofKind(allCards, 3) && !HandAnalyzerHelpers.IsNofKind(allCards, 4))
            {
                if (!allCards.GroupBy(x => x.Suit).Any(x => x.Count() >= 5))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual ShowdownHands GetRank()
        {
            return ShowdownHands.HighCard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.HighCard;
        }
    }

    public class HighCardNoOverCardsAnalyzer : HighCardAnalyzer
    {
        public override bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (base.Analyze(playerCards, boardCards))
            {
                return HandAnalyzerHelpers.IsNoOvercards(playerCards, boardCards);
            }
            return false;
        }

        public override ShowdownHands GetRank()
        {
            return ShowdownHands.HighCardNoOvercards;
        }
    }

    public class HighCardOneOvercardAnalyzer : HighCardAnalyzer
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
            return ShowdownHands.HighCardOneOvercard;
        }
    }

    public class HighCardTwoOvercardsAnalyzer : HighCardAnalyzer
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
            return ShowdownHands.HighCardTwoOvercards;
        }
    }

}
