using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal class HiCardsComparer : IComparer<string>
    {
        public int Compare(string cards1, string cards2)
        {
            var hand1 = cards1.GetHandFromString();
            var hand2 = cards2.GetHandFromString();

            // simple combination comparing
            var compareResult = hand1.Combination.CompareTo(hand2.Combination);

            if (compareResult != 0)
            {
                return compareResult;
            }

            // if combinations are equal we need to compare them deeper
            switch (hand1.Combination)
            {
                case HandCombo.Quads:
                case HandCombo.FullHouse:
                case HandCombo.Set:
                    return CompareByHistogram(hand1, hand2);
                case HandCombo.Flush:
                case HandCombo.HighCard:
                    return CompareByRank(hand1, hand2);
                case HandCombo.OnePair:
                    return CompareOnePair(hand1, hand2);
                case HandCombo.Straight:
                case HandCombo.StraightFlush:
                    return CompareStraight(hand1, hand2);
                case HandCombo.TwoPair:
                    return CompareTwoPair(hand1, hand2);
            }

            throw new Exception("Unexpected result of hand comparison.");
        }

        protected int CompareByHistogram(Hand hand1, Hand hand2)
        {
            var rank1 = hand1.CardsHistogram.Keys.First();
            var rank2 = hand2.CardsHistogram.Keys.First();

            // we don't need to comparer kickers because it's impossible to have 2 hands with similiar quads          
            return rank1.CompareTo(rank2);
        }

        protected int CompareByRank(Hand hand1, Hand hand2)
        {
            var cardRanks1 = hand1.Cards.Select(x => x.Rank).OrderByDescending(x => x).ToArray();
            var cardRanks2 = hand2.Cards.Select(x => x.Rank).OrderByDescending(x => x).ToArray();

            if (cardRanks1.Length != cardRanks2.Length)
            {
                throw new ArgumentException("Hands must have the same amount of cards");
            }

            for (var i = 0; i < cardRanks1.Length; i++)
            {
                var compareResult = cardRanks1[i].CompareTo(cardRanks2[i]);

                if (compareResult != 0)
                {
                    return compareResult;
                }
            }

            return 0;
        }

        protected int CompareOnePair(Hand hand1, Hand hand2)
        {
            var ranks1 = hand1.CardsHistogram.Select(x => x.Key).ToArray();
            var ranks2 = hand2.CardsHistogram.Select(x => x.Key).ToArray();

            var pairCompareResult = ranks1[0].CompareTo(ranks2[0]);

            if (pairCompareResult != 0)
            {
                return pairCompareResult;
            }

            ranks1 = ranks1.Skip(1).OrderByDescending(x => x).ToArray();
            ranks2 = ranks2.Skip(1).OrderByDescending(x => x).ToArray();

            for (var i = 0; i < ranks1.Length; i++)
            {
                var compareResult = ranks1[i].CompareTo(ranks2[i]);

                if (compareResult != 0)
                {
                    return compareResult;
                }
            }

            return 0;
        }

        protected int CompareStraight(Hand hand1, Hand hand2)
        {
            var highestCard1 = hand1.Cards.Select(x => x.Rank).Max();
            var highestCard2 = hand2.Cards.Select(x => x.Rank).Max();

            return highestCard1.CompareTo(highestCard2);
        }

        protected int CompareTwoPair(Hand hand1, Hand hand2)
        {
            var ranks1 = hand1.CardsHistogram.Select(x => x.Key).ToArray();
            var ranks2 = hand2.CardsHistogram.Select(x => x.Key).ToArray();

            var pairs1 = ranks1.Take(2).OrderByDescending(x => x).ToArray();
            var pairs2 = ranks2.Take(2).OrderByDescending(x => x).ToArray();

            for (var i = 0; i < pairs1.Length; i++)
            {
                var compareResult = pairs1[i].CompareTo(pairs2[i]);

                if (compareResult != 0)
                {
                    return compareResult;
                }
            }

            return ranks1.Last().CompareTo(ranks2.Last());
        }
    }
}