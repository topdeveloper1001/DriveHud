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
    public class OmahaRainbowAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            if (cards == null || cards.Count() < 4 || handGridItem == null)
                return false;

            return cards.GroupBy(x => x.Suit).Count() == 4;
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.Rainbow;
        }
    }

    public class OmahaAceSuitedAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            if (cards == null || cards.Count() < 4 || handGridItem == null)
                return false;

            var groups = cards.GroupBy(x => x.Suit).Where(x => x.Count() > 1);
            if (groups == null || groups.Count() != 1 || groups.First().Count() != 2)
            {
                return false;
            }

            return groups.First().Any(x => x.Rank == HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.First());
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.AceSuited;
        }
    }

    public class OmahaNoAceSuitedAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            if (cards == null || cards.Count() < 4 || handGridItem == null)
                return false;

            var groups = cards.GroupBy(x => x.Suit).Where(x => x.Count() > 1);
            if (groups == null || groups.Count() != 1 || groups.First().Count() != 2)
            {
                return false;
            }

            return groups.First().All(x => x.Rank != HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.First());
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.NoAceSuited;
        }
    }

    public class OmahaDoubleSuitedAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            if (cards == null || cards.Count() < 4 || handGridItem == null)
                return false;

            return cards.GroupBy(x => x.Suit).Where(x=> x.Count() == 2).Count() == 2;
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.DoubleSuited;
        }
    }
}
