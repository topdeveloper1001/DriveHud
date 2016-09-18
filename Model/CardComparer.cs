using System.Collections.Generic;
using HoldemHand;

namespace Model
{
    public class CardComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var v1 = Hand.CardRank(Hand.ParseCard(x));
            var v2 = Hand.CardRank(Hand.ParseCard(y));

            return v2.CompareTo(v1); // descending
        }
    }
}