using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcePokerSolutions.BusinessHelper.TextureHelpers;
using AcePokerSolutions.DataTypes;

namespace AcePokerSolutions.BusinessHelper.TextureAnalyzers.Flush
{
    public class ThreeOfOneSuitFlushTextureAnalyzer
    {
        public bool Analyze(string boardCards, Street targetStreet)
        {
            List<Card> cards = BoardTextureAnalyzerHelpers.ParseStringSequenceOfCards(boardCards);
            cards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(cards, targetStreet);

            if (!BoardTextureAnalyzerHelpers.IsStreetAvailable(cards, targetStreet))
                return false;

            return cards.GroupBy(x => x.CardSuit).Count(x => x.Count() == 3) == 1;
        }
    }
}
