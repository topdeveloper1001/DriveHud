using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Objects.Cards;
using Model.Enums;
using Model.Filters;
using Model.HandAnalyzers;

namespace Model.OmahaHoleCardsAnalyzers
{
    public static class OmahaWrapAnalyzerHelper
    {
        public static bool IsWrap(IEnumerable<HandHistories.Objects.Cards.Card> cards, int gapSize, int wrapSize)
        {
            var gap = gapSize + 1;
            var orderedRanks = HandAnalyzerHelpers.GetOrderedNumericRanksList(cards);

            for (int i = 1; i < orderedRanks.Count; i++)
            {
                var firstRank = orderedRanks[i - 1];
                var secondRank = orderedRanks[i];
                if ((firstRank + gap) == secondRank)
                {
                    if (gap == 1)
                    {
                        if (HandAnalyzerHelpers.GetConnectedCards(firstRank, orderedRanks).Count == wrapSize)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (HandAnalyzerHelpers.GetConnectedCards(firstRank, orderedRanks).Count
                            + HandAnalyzerHelpers.GetConnectedCards(secondRank, orderedRanks).Count
                            == wrapSize)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    public class OmahaTwoCardWrapAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            var item = handGridItem as WrapsAndRundownsHandGridItem;
            if (cards == null || cards.Count() < 4 || item == null)
                return false;

            return OmahaWrapAnalyzerHelper.IsWrap(cards, item.SelectedGap.Item2, 2);
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.TwoCardWrap;
        }
    }

    public class OmahaThreeCardWrapAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            var item = handGridItem as WrapsAndRundownsHandGridItem;
            if (cards == null || cards.Count() < 4 || item == null)
                return false;

            return OmahaWrapAnalyzerHelper.IsWrap(cards, item.SelectedGap.Item2, 3);

        }

        public OmahaHoleCards GetRank()
        {

            return OmahaHoleCards.ThreeCardWrap;
        }
    }

    public class OmahaFourCardWrapAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            var item = handGridItem as WrapsAndRundownsHandGridItem;
            if (cards == null || cards.Count() < 4 || item == null)
                return false;

            return OmahaWrapAnalyzerHelper.IsWrap(cards, item.SelectedGap.Item2, 4);
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.FourCardWrap;
        }
    }
}
