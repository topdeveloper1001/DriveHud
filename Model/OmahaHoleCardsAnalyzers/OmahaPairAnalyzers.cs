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
    public class OmahaNoPairAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            if (cards == null || cards.Count() < 4 || handGridItem == null)
                return false;

            return cards.GroupBy(x => x.Rank).All(x => x.Count() == 1);
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.NoPair;
        }
    }

    public class OmahaOnePairAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            var item = handGridItem as OnePairHandGridItem;
            if (cards == null || cards.Count() < 4 || item == null)
                return false;

            var groups = cards.GroupBy(x => x.Rank).Where(x => x.Count() >= 2);
            if (groups == null || groups.Count() != 1 || groups.First().Count() != 2)
            {
                return false;
            }

            var selectedRank = item.SelectedRank.Item2;
            return string.IsNullOrEmpty(selectedRank) ? true : groups.First().Key == selectedRank;
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.OnePair;
        }
    }

    public class OmahaTwoPairsAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            if (cards == null || cards.Count() < 4 || handGridItem == null)
                return false;

            return cards.GroupBy(x => x.Rank).All(x => x.Count() == 2);
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.TwoPairs;
        }
    }

    public class OmahaTripsAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            if (cards == null || cards.Count() < 4 || handGridItem == null)
                return false;

            return cards.GroupBy(x => x.Rank).Any(x => x.Count() == 3);
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.Trips;
        }
    }

    public class OmahaQuadsAnalyzer : IOmahaHoleCardsAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem)
        {
            if (cards == null || cards.Count() < 4 || handGridItem == null)
                return false;

            return cards.GroupBy(x => x.Rank).Any(x => x.Count() == 4);
        }

        public OmahaHoleCards GetRank()
        {
            return OmahaHoleCards.Quads;
        }
    }

}
