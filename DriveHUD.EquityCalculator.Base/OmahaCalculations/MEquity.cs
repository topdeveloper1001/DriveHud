using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    /// <summary>
    /// Multiple Equity - represents the only or high/low equity of a hand
    /// </summary>
    public class MEquity
    {
        /// <summary>
        /// Equity instances
        /// </summary>
        public OmahaEquity[] eqs;
        /** is this equity exact or sampled */
        public bool Exact;
        /** number of cards remaining in deck */
        public int RemCards;
        /** is hi/hilo-hi/hilo-lo combination */
        public bool HiLo;

        /** percentage of hands with low possible */
        public float lowPossible;
        /** percentage total equity, including hi/lo and ties */
        public float TotalEq;
        /** percentage times the player will win the entire pot */
        public float scoop;

        /** number of times won all pots, no ties */
        internal int scoopcount;

        internal static MEquity createMEquity(OmahaEquity.EquityType type, int rem, bool exact)
        {
            OmahaEquity[] eqs = new OmahaEquity[] {
                new OmahaEquity(type, exact)
        };
            return new MEquity(eqs, false, rem, exact);
        }

        internal static MEquity createMEquityHL(bool hilo, int rem, bool exact)
        {
            OmahaEquity[] eqs;
            if (hilo)
            {
                eqs = new OmahaEquity[] {
                new OmahaEquity(OmahaEquity.EquityType.HI_ONLY, exact),
                new OmahaEquity(OmahaEquity.EquityType.HILO_HI_HALF, exact),
                new OmahaEquity(OmahaEquity.EquityType.HILO_AFLO8_HALF, exact)
            };
            }
            else
            {
                eqs = new OmahaEquity[] {
                new OmahaEquity(OmahaEquity.EquityType.HI_ONLY, exact)
            };
            }
            return new MEquity(eqs, hilo, rem, exact);
        }

        private MEquity(OmahaEquity[] eqs, bool hilo, int rem, bool exact)
        {
            this.eqs = eqs;
            this.HiLo = hilo;
            this.RemCards = rem;
            this.Exact = exact;
        }

        /** get the equity instance for the given equity type */
        public OmahaEquity getEquity(OmahaEquity.EquityType type)
        {
            int i;
            switch (type)
            {
                case OmahaEquity.EquityType.DSLO_ONLY:
                case OmahaEquity.EquityType.AFLO_ONLY:
                case OmahaEquity.EquityType.AFLO8_ONLY:
                case OmahaEquity.EquityType.HI_ONLY:
                case OmahaEquity.EquityType.BADUGI_ONLY:
                    i = 0;
                    break;
                case OmahaEquity.EquityType.HILO_HI_HALF:
                    i = 1;
                    break;
                case OmahaEquity.EquityType.HILO_AFLO8_HALF:
                    i = 2;
                    break;
                default:
                    throw new ArgumentException("no such equity type " + type);
            }
            OmahaEquity e = eqs[i];
            if (e.type != type)
            {
                throw new ArgumentException("eq is type " + e.type + " not " + type);
            }
            return e;
        }
    }
}
