using HandHistories.Objects.Cards;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.HandAnalyzers
{
    public interface IAnalyzer
    {
        ShowdownHands GetRank();

        bool IsValidAnalyzer(HoldemHand.Hand hand);

        bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards);
    }
}
