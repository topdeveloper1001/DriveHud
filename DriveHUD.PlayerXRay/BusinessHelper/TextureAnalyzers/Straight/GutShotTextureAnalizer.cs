using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using HandHistories.Objects.Cards;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper.TextureAnalyzers.Straight
{
    public class GutShotBeatNutsTextureAnalyzer
    {
        public int Analyze(string boardCards, Street targetStreet)
        {
            var cards = BoardTextureAnalyzerHelpers.ParseStringSequenceOfCards(boardCards);

            if (string.IsNullOrEmpty(boardCards) || !BoardTextureAnalyzerHelpers.IsStreetAvailable(cards, targetStreet))
                return 0;

            int numberOfOpenEndedStraights = 0;

            var orderedRanks = BoardTextureAnalyzerHelpers.GetOrderedBoardNumericRanks(cards, targetStreet);
            if (orderedRanks.Count < 2)
            {
                return 0;
            }

            int firstRank = orderedRanks.Max() + 3;
            int secondRank = firstRank - 1;
            while (firstRank > orderedRanks.Min() - 3)
            {
                firstRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank);
                secondRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, secondRank);

                if (firstRank == secondRank)
                {
                    secondRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks,
                        secondRank - 1);
                }

                if (secondRank <= 0 || secondRank < orderedRanks.Min() - 3)
                {
                    firstRank--;
                    secondRank = firstRank;
                    continue;
                }

                var tempList = new List<int>(orderedRanks);
                tempList.Add(firstRank);
                tempList.Add(secondRank);
                tempList = tempList.Where(x => x <= firstRank).ToList();
                if (tempList.Count > 3)
                {
                    var hand = tempList.OrderByDescending(x => x).Take(4).ToList();
                    var isGutShot = hand.Max() - hand.Min() == 4;
                    if (isGutShot)
                    {
                        numberOfOpenEndedStraights++;
                    }
                }
                secondRank--;
            }

            return numberOfOpenEndedStraights;
        }
    }
}
