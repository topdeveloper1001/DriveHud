using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using HandHistories.Objects.Cards;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper.TextureAnalyzers.Flush
{
    public class FiveOfOneSuitFlushTextureAnalyzer
    {
        public bool Analyze(string boardCards, Street targetStreet)
        {
            var cards = BoardTextureAnalyzerHelpers.ParseStringSequenceOfCards(boardCards);
            cards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(cards, targetStreet);

            if (!BoardTextureAnalyzerHelpers.IsStreetAvailable(cards, targetStreet))
            {
                return false;
            }

            return cards.GroupBy(x => x.CardSuit).Count() == 1;
        }
    }
}