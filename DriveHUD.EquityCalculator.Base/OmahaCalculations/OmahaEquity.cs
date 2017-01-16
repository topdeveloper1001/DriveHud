using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    /// <summary>
    /// Represents the equity of a hand according to a specific valuation type
    /// </summary>
    public class OmahaEquity
    {
        public class Out : IComparable<Out>
        {
            internal float pc;
            internal string card;

            internal Out(String card, float pc)
            {
                this.card = card;
                this.pc = pc;
            }

            public int CompareTo(Out o)
            {
                float c = pc - o.pc;
                if (c != 0)
                {
                    return (int)Math.Sign(c);
                }
                return Cmp.cardCmp.Compare(card, o.card);
            }

            public override string ToString()
            {
                return string.Format("{0}[{1:0.00}]", card, pc);
            }
        }

        /// <summary>
        /// equity types. (note: hi/lo (8 or better) is not a type, it is actually
        /// three types, hence the MEquity class). These look similar to the
        /// constants in the Poker class, such as AF_LOW_TYPE, but they deal with
        /// hand valuation only, whereas these include the context of how that
        /// valuation is used(e.g.whether an equity is high only or just the high
        /// half of a high/low split).
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="better"></param>
        /// <returns></returns>
        public enum EquityType
        {
            /** deuce to seven low only equity type (single draw/triple draw) */
            DSLO_ONLY,
            /** ace to five low only equity type  (razz) */
            AFLO_ONLY,
            /** high only equity type (holdem, omaha hi, 5 card draw, etc) */
            HI_ONLY,
            /** high half of hi/lo equity type (omaha 8, stud 8, etc) */
            HILO_HI_HALF,
            /** ace to five low 8 or better half of hi/lo equity type (omaha 8, stud 8, etc) */
            HILO_AFLO8_HALF,
            /** ace to five low 8 or better only equity type (not used alone by any game, as it's qualified) */
            AFLO8_ONLY,
            /** badugi value */
            BADUGI_ONLY
        }

        /// <summary>
        /// equity type
        /// </summary>
        internal EquityType type;

        /// <summary>
        /// current value
        /// </summary>
        internal int current;

        /// <summary>
        /// Currently winning and not tying
        /// </summary>
        internal bool curwin;

        /// <summary>
        /// currently tying
        /// </summary>
        internal bool curtie;

        /// <summary>
        ///  percentage of hands won but not tied.note: for hi/lo, it is possible to
        /// just add hi only won and hi half won for total high won, as they are
        ///  exclusive.same for tied.
        /// </summary>
        public float won;

        /// <summary>
        /// percentage of hands tied but not won
        /// </summary>
        public float tied;

        /// <summary>
        /// total equity percentage
        /// </summary>
        public float total;

        /// <summary>
        /// percentage of hands won or tied by rank (value >> 20)
        /// </summary>
        public float[] wonrank = new float[OmahaPoker.RANKS];

        /// <summary>
        /// list of cards and percentage times that card is included in a pick that will make best hand
        /// </summary>
        public List<Out> outs;

        // transient stuff

        /// <summary>
        /// number of samples won
        /// </summary>
        internal int woncount;
        /// <summary>
        /// number of samples tied
        /// </summary>
        internal int tiedcount;
        /// <summary>
        /// number of people tied with including self
        /// </summary>
        internal int tiedwithcount;
        /// <summary>
        /// winning ranks
        /// </summary>
        internal int[] wonrankcount = new int[OmahaPoker.RANKS];
        /// <summary>
        /// count that each card (as part of group of cards) will make the best hand
        /// </summary>
        internal  int[] outcount;

        public OmahaEquity(EquityType eqtype, bool hasouts)
        {
            this.type = eqtype;
            this.outcount = hasouts ? new int[52] : null;
            this.outs = hasouts ? new List<Out>() : null;
        }

        /// <summary>
        /// update percentage won, tied and by rank
        /// </summary>
        /// <param name="hands"></param>
        internal void summariseEquity(int hands)
        {
            int wontiedcount = woncount + tiedcount;
            won = (woncount * 100f) / hands;
            tied = (tiedcount * 100f) / hands;
            for (int n = 0; n < wonrankcount.Count(); n++)
            {
                wonrank[n] = wontiedcount != 0 ? (wonrankcount[n] * 100f) / wontiedcount : 0;
            }

            total = (woncount * 100f) / hands;
            if (tiedcount > 0)
            {
                total += (tied * ((tiedcount * 1f) / tiedwithcount));
            }
        }

        /// <summary>
        ///  Summarise out probabilities for given number of picks from remaining cards
        /// </summary>
        /// <param name="remCards"></param>
        /// <param name="picks"></param>
        /// <param name="samples"></param>
        internal void summariseOuts(float remCards, float picks, float samples)
        {
            if (outcount != null)
            {
                // maximum number of times an out can appear (average if sampled)
                // prob of appearing once is picks/remCards, just multiply by samples
                // (n,k,s) = (k*s)/n
                // (52,1,52) = 1,  (52,2,1326) = 51,  (52,3,100000) = 5769 
                float max = (picks * samples) / remCards;
                //System.out.println(String.format("sum outs(%f,%f,%f) max=%f", remCards, picks, samples, max));
                for (int n = 0; n < outcount.Count(); n++)
                {
                    int count = outcount[n];
                    if (count > 0)
                    {
                        String card = OmahaPoker.indexToCard(n);
                        float pc = (count * 100f) / max;
                        outs.Add(new Out(card, pc));
                    }
                }
                outs.Sort();
                outs.Reverse();
            }
        }
    }
}
