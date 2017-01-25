using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    public static class Cmp
    {
        /// <summary>
        /// compare by face (ace high) then suit, lowest first
        /// </summary>
        /// <typeparam name="String"></typeparam>
        /// <param name=""></param>
        /// <returns></returns>
        public static readonly IComparer<String> cardCmp = new CardCmp(true);
        /**
         * compare by face (ace high) then suit, highest first
         */
        public static readonly IComparer<String> revCardCmp = new CardCmp(false);
        /**
         * compare by face (ace low) only, highest first
         */
        public static readonly IComparer<String> faceCmpAL = new FaceCmp(true, false);
    }

    class CardCmp : IComparer<String>
    {
        private readonly int polarity;

        public CardCmp(bool asc)
        {
            polarity = asc ? 1 : -1;
        }

        public int Compare(string c1, string c2)
        {
            int v = OmahaPoker.faceValueAH(c1) - OmahaPoker.faceValueAH(c2);
            if (v == 0)
            {
                v = OmahaPoker.suit(c1) - OmahaPoker.suit(c2);
            }
            return polarity * v;
        }
    }

    class FaceCmp : IComparer<String>
    {
        private int polarity;
        private bool aceHigh;

        public FaceCmp(bool asc, bool aceHigh)
        {
            this.aceHigh = aceHigh;
            this.polarity = asc ? 1 : -1;
        }

        public int Compare(String c1, String c2)
        {
            // highest first
            return polarity * (OmahaPoker.faceValueAH(c2) - OmahaPoker.faceValueAH(c1));
        }
    }
}
