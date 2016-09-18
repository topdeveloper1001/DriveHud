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
    public class StubAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            return true;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.None;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return true;
        }
    }
}
