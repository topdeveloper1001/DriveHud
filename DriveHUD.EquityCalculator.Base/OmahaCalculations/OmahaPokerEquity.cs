using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    internal abstract class OmahaPokerEquity
    {
        public static string[] emptyBoard = new String[0];

        /* 
         * poker hand values are represented by 7 x 4 bit values (28 bits total):
         * 0x87654321
         * 8 = 0
         * 7 = hand type (high, a-5 low, 2-7 low, badugi)
         * 6 = rank
         * 5 = most significant card (if any)
         * ...
         * 1 = least significant card
         */

        /** hand value type mask */
        protected static readonly int TYPE = 0x0f000000;
        /** rank mask (allowing 20 bits for hand value, i.e. 4 bits per card) */
        protected static readonly int RANK = 0x00f00000;
        /** rank and hand value mask, NOT type */
        protected static readonly int HAND = 0x00ffffff;

        /** high hand valuation type */
        protected static readonly int HI_TYPE = 0x01000000;
        /**
         * deuce to seven low hand valuation type - do MAX_RANK - (value & HAND) to
         * get apparent high value
         */
        protected static readonly int DS_LOW_TYPE = 0x02000000;
        /**
         * ace to five low hand valuation type - do MAX_RANK - (value & HAND) to get
         * apparent high value
         */
        protected static readonly int AF_LOW_TYPE = 0x03000000;
        /**
         * badugi value type - do B0_RANK - (value & HAND) to get cards
         */
        protected static int BADUGI_TYPE = 0x04000000;

        /** high card bit mask (always zero) */
        protected static int H_RANK = 0;
        /** pair rank bit mask */
        protected static int P_RANK = 0x100000;
        /** two pair rank bit mask */
        protected static int TP_RANK = 0x200000;
        /** three of a kind rank bit mask */
        protected static int TK_RANK = 0x300000;
        /** straight bit mask */
        public static int ST_RANK = 0x400000;
        /** flush bit mask */
        protected static int FL_RANK = 0x500000;
        /** full house bit mask */
        protected static int FH_RANK = 0x600000;
        /** four of a kind bit mask */
        protected static int FK_RANK = 0x700000;
        /** straight flush rank mask */
        protected static int SF_RANK = 0x800000;
        /** impossible rank higher than straight flush for low hand value purposes */
        protected static int MAX_RANK = 0x900000;

        /** max number of possible ranks (all valuation types) */
        public static int RANKS = 9;

        /**
         * short rank names for high hand (value >> 20)
         */
        public static string[] shortRankNames = { "Hc", "P", "Tp", "3K", "St", "Fl", "Fh", "4k", "Sf" };

        /** short rank names for ace to five low hands */
        public static string[] afLowShortRankNames = { "5", "6", "7", "8", "Hc", "P+" };

        public static string[] dsLowShortRankNames = { "7", "8", "9", "T", "Hc", "P+" };

        /** card suit representations */
        public static char H_SUIT = 'h', C_SUIT = 'c', S_SUIT = 's', D_SUIT = 'd';

        /** complete deck in face then suit order, lowest first */
        private static string[] deckArr = {
        "2h", "2s", "2c", "2d",
        "3h", "3s", "3c", "3d", "4h", "4s", "4c", "4d", "5h", "5s", "5c",
        "5d", "6h", "6s", "6c", "6d", "7h", "7s", "7c", "7d", "8h", "8s",
        "8c", "8d", "9h", "9s", "9c", "9d", "Th", "Ts", "Tc", "Td", "Jh",
        "Js", "Jc", "Jd", "Qh", "Qs", "Qc", "Qd", "Kh", "Ks", "Kc", "Kd",
        "Ah", "As", "Ac", "Ad"
    };

        public static string[] deckArrS;

        public static List<String> deck = deckArr.ToList();

        /** complete suits */
        public static char[] suits = { S_SUIT, H_SUIT, C_SUIT, D_SUIT };

        /** array of all possible unique hi hand values (there are only approx 7500) */
        private static int[] uniqueHighValues;

        internal OmahaPokerEquity()
        {
            deckArrS = deckArr.ToArray();
            Array.Sort(deckArrS);
        }

        /** get card representing integer */
        public static string indexToCard(int i)
        {
            return deckArrS[i];
        }

        /** get an integer representing the card, 0-51 */
        public static int cardToIndex(String card)
        {
            int i = Array.BinarySearch(deckArrS, card);
            if (i < 0)
            {
                throw new ArgumentException("no such card: " + card);
            }
            return i;
        }

        /**
         * return a copy of the deck that can be modified
         */
        public static String[] Deck()
        {
            return deckArr.ToArray();
        }

        /**
         * count low cards
         */
        protected static int lowCount(String[] hand, bool acehigh)
        {
            int count = 0;
            for (int n = 0; n < hand.Length; n++)
            {
                if (faceValue(hand[n], acehigh) <= 8)
                {
                    count++;
                }
            }
            return count;
        }

        /**
         * get 8 or better qualified ace to five low value of hand.
         * returns 0 if no low.
         */
        static int afLow8Value(String[] hand)
        {
            validate(hand);
            if (lowCount(hand, false) == 5)
            {
                int p = isPair(hand, false);
                if (p < P_RANK)
                {
                    // no pairs
                    // invert value
                    int v = AF_LOW_TYPE | (MAX_RANK - p);
                    return v;
                }
            }
            return 0;
        }

        /**
         * get unqualified ace to five low value of hand
         */
        static int afLowValue(String[] hand)
        {
            validate(hand);
            // allow pairs but not straights or flushes, ace low
            int p = isPair(hand, false);
            // invert value
            int v = AF_LOW_TYPE | (MAX_RANK - p);
            return v;
        }

        /** check hand is 5 cards and non of the cards are duplicated */
        private static void validate(String[] h)
        {
            if (h.Length != 5)
            {
                throw new ArgumentException("invalid hand length: " + h.ToString());
            }
            for (int n = 0; n < h.Length; n++)
            {
                String c = h[n];
                if ("23456789TJQKA".IndexOf(face(c)) == -1 || "hdsc".IndexOf(suit(c)) == -1)
                {
                    throw new ArgumentException("invalid hand " + h.ToString());
                }
                // check for dupe
                for (int m = n + 1; m < h.Length; m++)
                {
                    if (c.Equals(h[m]))
                    {
                        throw new ArgumentException("invalid hand " + h.ToString());
                    }
                }
            }
        }

        /**
         * return straight value of hand, or 0, no other ranks
         */
        public static int strValue(String[] hand)
        {
            int s = isStraight(hand);
            if (s > 0)
            {
                return ST_RANK | s;
            }
            else
            {
                return 0;
            }
        }

        /**
         * Get high value of 5 card hand with type of HI_TYPE
         */
        public static int value(String[] hand)
        {
            validate(hand);
            int p = isPair(hand, true);
            if (p < P_RANK)
            {
                bool f = isFlush(hand);
                int s = isStraight(hand);
                if (f)
                {
                    if (s > 0)
                    {
                        p = SF_RANK | s;
                    }
                    else
                    {
                        p = FL_RANK | p;
                    }
                }
                else if (s > 0)
                {
                    p = ST_RANK | s;
                }
            }
            return p | HI_TYPE;
        }

        /**
         * return true if flush
         */
        private static bool isFlush(String[] hand)
        {
            char s = suit(hand[0]);
            for (int n = 1; n < 5; n++)
            {
                if (suit(hand[n]) != s)
                {
                    return false;
                }
            }
            return true;
        }

        /** 
         * return value of high card of straight (5-14) or 0 
         */
        private static int isStraight(String[] hand)
        {
            int x = 0;
            // straight value
            int str = 5;
            for (int n = 0; n < hand.Length; n++)
            {
                // sub 1 so bottom bit equals ace low
                int v = faceValueAH(hand[n]) - 1;
                x |= (1 << v);
                if (v == 13)
                {
                    // add ace low as well as ace high
                    x |= 1;
                }
            }
            // [11111000000001]
            while (x >= 31)
            {
                if ((x & 31) == 31)
                {
                    return str;
                }
                x >>= 1;
                str++;
            }
            return 0;
        }

        /**
         * Return pair value or high cards without type mask.
         * Does not require sorted hand
         */
        private static int isPair(String[] hand, bool acehigh)
        {
            // count card face frequencies (3 bits each) -- 0, 1, 2, 3, 4
            long v = 0;
            for (int n = 0; n < hand.Length; n++)
            {
                v += (1L << ((14 - faceValue(hand[n], acehigh)) * 3));
            }
            // get the card faces for each frequency
            int fk = 0, tk = 0, pa = 0, hc = 0;
            for (int f = 14; v != 0; v >>= 3, f--)
            {
                int i = (int)(v & 7);
                if (i == 0)
                {
                    continue;
                }
                else if (i == 1)
                {
                    hc = (hc << 4) | f;
                }
                else if (i == 2)
                {
                    pa = (pa << 4) | f;
                }
                else if (i == 3)
                {
                    tk = f;
                }
                else if (i == 4)
                {
                    fk = f;
                }
            }

            if (fk != 0)
            {
                return FK_RANK | (fk << 4) | hc;
            }
            else if (tk != 0)
            {
                if (pa != 0)
                {
                    return FH_RANK | (tk << 4) | pa;
                }
                else
                {
                    return TK_RANK | (tk << 8) | hc;
                }
            }
            else if (pa >= 16)
            {
                return TP_RANK | (pa << 4) | hc;
            }
            else if (pa != 0)
            {
                return P_RANK | (pa << 12) | hc;
            }
            else
            {
                return H_RANK | hc;
            }
        }

        /**
         * deuce to seven value - exact opposite of high value (i.e. worst high
         * hand, not best low hand) 
         * FIXME this is actually wrong, it should be best low hand not worst high hand
         * i.e. A2345 = A-high, not straight
         */
        static int dsValue(String[] hand)
        {
            return DS_LOW_TYPE | (MAX_RANK - (value(hand) & HAND));
        }

        /**
         * Return integer value of card face, ace high or low (from A = 14 to 2 = 2 or K = 13 to A = 1)
         */
        static int faceValue(String card, bool acehigh)
        {
            if (acehigh)
            {
                return faceValueAH(card);
            }
            else
            {
                return faceValueAL(card);
            }
        }

        /** face value, ace low (A = 1, K = 13) */
        static int faceValueAL(String card)
        {
            int i = "A23456789TJQK".IndexOf(face(card));
            return i + 1;
        }

        /** face value, ace high (2 = 2, A = 14) */
        static int faceValueAH(String card)
        {
            int i = "23456789TJQKA".IndexOf(face(card));
            return i + 2;
        }

        /**
         * Returns lower case character representing suit, i.e. s, d, h or c
         */
        public static char suit(String card)
        {
            return card[1];
        }

        /**
         * return number representing value of suit
         */
        public static int suitValue(String card)
        {
            return "cdhs".IndexOf(card[1]);
        }

        /**
         * Return character symbol of face value
         */
        static char valueFace(int x)
        {
            int v = x & 0xf;
            // allow 0 index and ace low
            return "?A23456789TJQKA"[v];
        }

        /**
         * Return string representation of hand value (any type)
         */
        public static String valueString(int value)
        {
            if (value == 0)
            {
                return "No value";
            }
            var type = value & TYPE;

            if (type == HI_TYPE)
            {
                return valueStringHi(type, true);
            }
            if (type == DS_LOW_TYPE || type == AF_LOW_TYPE)
            {
                return valueStringHi(MAX_RANK - (value & HAND), false);
            }

            throw new ArgumentException("v=" + value.ToString("X4"));
        }

        /**
         * get string representation of bare high value (excluding type)
         */
        private static String valueStringHi(int highValue, bool high)
        {
            char c1 = valueFace(highValue);
            char c2 = valueFace(highValue >> 4);
            char c3 = valueFace(highValue >> 8);
            char c4 = valueFace(highValue >> 12);
            char c5 = valueFace(highValue >> 16);

            string s;
            var rank = highValue & RANK;

            if (rank == SF_RANK)
            {
                s = "Straight Flush - " + c1 + " high";
            }
            else if (rank == FK_RANK)
            {
                s = "Four of a Kind " + c2 + " - " + c1;
            }
            else if (rank == FH_RANK)
            {
                s = "Full House " + c2 + " full of " + c1;
            }
            else if (rank == FL_RANK)
            {
                s = "Flush - " + c5 + " " + c4 + " " + c3 + " " + c2 + " " + c1;
            }
            else if (rank == ST_RANK)
            {
                s = "Straight - " + c1 + " high";
            }
            else if (rank == TK_RANK)
            {
                s = "Three of a Kind " + c3 + " - " + c2 + " " + c1;
            }
            else if (rank == TP_RANK)
            {
                s = "Two Pair " + c3 + " and " + c2 + " - " + c1;
            }
            else if (rank == P_RANK)
            {
                s = "Pair " + c4 + " - " + c3 + " " + c2 + " " + c1;
            }
            else if (rank == H_RANK)
            {
                s = c5 + " " + c4 + " " + c3 + " " + c2 + " " + c1 + (high ? " high" : " low");
            }
            else
            {
                s = "**" + highValue.ToString("X4") + "**";
            }

            return high ? s : "(" + s + ")";
        }

        public static char face(String card)
        {
            return card[0];
        }

        /**
         * return rank of hand, from 0 to 9 (RANKS) (NOT the rank bitmask
         * constants), this is an index into the rank names arrays for the value
         * type
         */
        public static int rank(int value)
        {
            int t = value & TYPE;
            if (t == HI_TYPE)
            {
                return (value & RANK) >> 20;
            }
            if (t == AF_LOW_TYPE)
            {
                return rankLo(value, 5);
            }
            if (t == DS_LOW_TYPE)
            {
                return rankLo(value, 7);
            }

            throw new ArgumentException("v=" + value.ToString("X4"));
        }

        private static int rankLo(int value, int x)
        {
            // get low rank
            int v = MAX_RANK - (value & HAND);
            int r = v & RANK;
            if (r == H_RANK)
            {
                int c = (v >> 16) & 0xf;
                if (c < x + 4)
                {
                    // 5-8 or 7-10
                    return c - x;
                }
                else
                {
                    // other high card
                    return 4;
                }
            }
            else
            {
                // pair or greater
                return 5;
            }
        }

        /**
         * return the remaining cards in the deck.
         * always returns new array
         */
        public static String[] remdeck(String[][] aa, params string[][] a)
        {
            List<string> list = new List<string>(deck);
            if (aa != null)
            {
                foreach (String[] x in aa)
                {
                    rem1(list, x);
                }
            }
            if (a != null)
            {
                foreach (String[] x in a)
                {
                    rem1(list, x);
                }
            }
            return list.ToArray();
        }

        private static void rem1(List<String> list, String[] a)
        {
            if (a != null)
            {
                foreach (String s in a)
                {
                    if (s != null && !list.Remove(s))
                    {
                        throw new ArgumentException("card " + s + " already removed");
                    }
                }
            }
        }

        /**
         * Go through every possible 5 card hand and collect the unique hand values in order
         */
        protected static int[] highValues()
        {
            if (uniqueHighValues != null)
            {
                return uniqueHighValues;
            }

            HashSet<int> uniqueValueSet = new HashSet<int>();
            String[] hand = new String[5];
            int valueCount = 0;
            for (int n0 = 0; n0 < deckArr.Length; n0++)
            {
                hand[0] = deckArr[n0];
                for (int n1 = n0 + 1; n1 < deckArr.Length; n1++)
                {
                    hand[1] = deckArr[n1];
                    for (int n2 = n1 + 1; n2 < deckArr.Length; n2++)
                    {
                        hand[2] = deckArr[n2];
                        for (int n3 = n2 + 1; n3 < deckArr.Length; n3++)
                        {
                            hand[3] = deckArr[n3];
                            for (int n4 = n3 + 1; n4 < deckArr.Length; n4++)
                            {
                                hand[4] = deckArr[n4];
                                uniqueValueSet.Add(OmahaPoker.Value(hand));
                                valueCount++;
                            }
                        }
                    }
                }
            }

            int[] a = new int[uniqueValueSet.Count];
            int i = 0;
            foreach (int v in uniqueValueSet)
            {
                a[i++] = v;
            }

            Array.Sort(a);
            uniqueHighValues = a;
            return a;
        }

        //
        // instance methods
        //

        /**
         * Calculate equity for given board and hands.
         */
        public MEquity[] equity(ICollection<String> board, ICollection<String[]> cards, ICollection<String> blockers, int draws)
        {
            String[] boardArr = board != null ? board.ToArray() : null;
            String[][] cardsArr = cards.ToArray();
            String[] blockersArr = blockers.ToArray();
            return equity(boardArr, cardsArr, blockersArr, draws);
        }

        /** primary valuation method */
        protected OmahaValue Value;

        public OmahaPokerEquity(OmahaValue value)
        {
            this.Value = value;
        }

        /** get the primary value function for this game */
        public OmahaValue getValue()
        {
            return Value;
        }

        /**
         * Calculate equity for given board and hands (implementation)
         */
        protected abstract MEquity[] equity(String[] board, String[][] holeCards, String[] blockers, int draws);

        /**
         * Calculate value of hand. If the hand is invalid (e.g. has board for non
         * board game, or hole cards null/empty), an error is thrown. If the hand is
         * incomplete, 0 is returned.
         */
        public abstract int value(String[] board, String[] hole);

        /**
         * get the minimum number of hole cards for an equity calculation (omaha: 2,
         * all other games: 1)
         */
        public int minHoleCards()
        {
            return 1;
        }
    }
}
