using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using HandHistories.Objects.Cards;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper.TextureAnalyzers.Straight
{
    public class OpenEndedStraightTextureAnalyzer
    {
        public int Analyze(string boardCards, Street targetStreet)
        {
            var cards = BoardTextureAnalyzerHelpers.ParseStringSequenceOfCards(boardCards);

            if (string.IsNullOrEmpty(boardCards) || !BoardTextureAnalyzerHelpers.IsStreetAvailable(cards, targetStreet))
                return 0;  

            int numberOfOpenEndedStraights = 0;  

            List<int> orderedRanks = BoardTextureAnalyzerHelpers.GetOrderedBoardNumericRanks(cards, targetStreet);
            if (orderedRanks.Count < 2)
            {
                return 0;
            }

            int firstRank = orderedRanks.Max() + 2;
            while (firstRank > orderedRanks.Min() - 2)
            {
                firstRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank);
                var secondRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank - 1);
                if (secondRank <= 0)
                {
                    break;
                }

                List<int> connectedList = new List<int>(orderedRanks);
                connectedList.Add(secondRank);
                connectedList = HandAnalyzerHelpers.GetConnectedCards(firstRank, connectedList);
                
                if (connectedList.Count > 3)
                {
                    numberOfOpenEndedStraights += HandAnalyzerHelpers.IsStraight(connectedList.OrderByDescending(x => x).Take(4).ToList()) ? 1 : 0;
                }
                firstRank--;
            }

            return numberOfOpenEndedStraights;
        }

        
    }
}
