using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Objects.Cards;
using Model.Enums;
using Model.Filters;

namespace Model.OmahaHoleCardsAnalyzers
{
    public class OmahaHoleCardStructureAcesAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            var item = handGridItem as HoleCardStructureHandGridItem;
            if (cards == null || cards.Count() < 4 || handGridItem == null || item == null)
                return false;

            return cards.Where(x=> x.Rank == "A").Count() == item.SelectedNumber;
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.CardStructureAces;
        }
    }

    public class OmahaHoleCardStructureBroadwaysAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            var item = handGridItem as HoleCardStructureHandGridItem;
            if (cards == null || cards.Count() < 4 || handGridItem == null || item == null)
                return false;

            var rankTen = HandHistories.Objects.Cards.Card.GetRankNumericValue("T");
            var rankKing = HandHistories.Objects.Cards.Card.GetRankNumericValue("K");
            return cards.Where(x => x.RankNumericValue >= rankTen && x.RankNumericValue <= rankKing).Count() == item.SelectedNumber;
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.CardStructureBroadways;
        }
    }

    public class OmahaHoleCardStructureMidHandAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            var item = handGridItem as HoleCardStructureHandGridItem;
            if (cards == null || cards.Count() < 4 || handGridItem == null || item == null)
                return false;

            var rankSix = HandHistories.Objects.Cards.Card.GetRankNumericValue("6");
            var rankNine = HandHistories.Objects.Cards.Card.GetRankNumericValue("9");
            return cards.Where(x => x.RankNumericValue >= rankSix && x.RankNumericValue <= rankNine).Count() == item.SelectedNumber;
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.CardStructureMidHands;
        }
    }

    public class OmahaHoleCardStructureLowCardsAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            var item = handGridItem as HoleCardStructureHandGridItem;
            if (cards == null || cards.Count() < 4 || handGridItem == null || item == null)
                return false;

            var rankTwo = HandHistories.Objects.Cards.Card.GetRankNumericValue("2");
            var rankFive = HandHistories.Objects.Cards.Card.GetRankNumericValue("5");
            return cards.Where(x => x.RankNumericValue >= rankTwo && x.RankNumericValue <= rankFive).Count() == item.SelectedNumber;
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.CardStructureLowCards;
        }
    }
}
