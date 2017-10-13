using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using HandHistories.Objects.Cards;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper.TextureAnalyzers.Flush
{
    public class TwoOfTwoSuitFlushTextureAnalyzer
    {
        public bool Analyze(string boardCards, Street targetStreet)
        {
            var cards = BoardTextureAnalyzerHelpers.ParseStringSequenceOfCards(boardCards);
            cards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(cards, targetStreet);

            if (!BoardTextureAnalyzerHelpers.IsStreetAvailable(cards, targetStreet))
                return false;

            return cards.GroupBy(x => x.CardSuit).Count(x => x.Count() == 2) == 2;
        }
    }
}
