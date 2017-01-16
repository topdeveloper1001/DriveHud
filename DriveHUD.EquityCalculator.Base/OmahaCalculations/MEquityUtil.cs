using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    /// <summary>
    /// utility methods for MEquity class
    /// </summary>
    internal class MEquityUtil
    {
        /// <summary>
        /// Make array of multiple hand equities for given equity type, number of remaining cards, calculation method
        /// </summary>
        /// <param name="eqtype"></param>
        /// <param name="hands"></param>
        /// <param name="rem"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
        internal static MEquity[] createMEquities(OmahaEquity.EquityType eqtype, int hands, int rem, bool exact)
        {
            MEquity[] meqs = new MEquity[hands];
            for (int n = 0; n < meqs.Length; n++)
            {
                meqs[n] = MEquity.createMEquity(eqtype, rem, exact);
            }
            return meqs;
        }

        /// <summary>
        /// Make array of multiple hand equities for hi or hi/lo equity type, number of remaining cards, and calculation method
        /// </summary>
        /// <param name="hilo"></param>
        /// <param name="hands"></param>
        /// <param name="rem"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
        internal static MEquity[] createMEquitiesHL(bool hilo, int hands, int rem, bool exact)
        {
            MEquity[] meqs = new MEquity[hands];
            for (int n = 0; n < meqs.Length; n++)
            {
                meqs[n] = MEquity.createMEquityHL(hilo, rem, exact);
            }
            return meqs;
        }

        /// <summary>
        /// Set the current value of the hands, not the equity
        /// </summary>
        /// <param name="meqs"></param>
        /// <param name="eqtype"></param>
        /// <param name="vals"></param>
        internal static void updateCurrent(MEquity[] meqs, OmahaEquity.EquityType eqtype, int[] vals)
        {
            int max = 0, times = 0;
            for (int i = 0; i < vals.Length; i++)
            {
                int v = vals[i];
                if (v > max)
                {
                    max = v;
                    times = 1;
                }
                else if (v == max)
                {
                    times++;
                }
            }
            // only set curwin, curtie if there actually is non zero current value
            if (max > 0)
            {
                for (int i = 0; i < vals.Length; i++)
                {
                    OmahaEquity e = meqs[i].getEquity(eqtype);
                    e.current = vals[i];
                    if (e.current == max)
                    {
                        if (times == 1)
                        {
                            e.curwin = true;
                        }
                        else
                        {
                            e.curtie = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update equities win, tie, win rank and scoop with given hand values for the given cards.
        /// </summary>
        /// <param name="meqs"></param>
        /// <param name="hivals"></param>
        /// <param name="lovals"></param>
        /// <param name="cards"></param>
        internal static void updateMEquitiesHL(MEquity[] meqs, int[] hivals, int[] lovals, String[] cards)
        {
            // high winner
            int hw = MEquityUtil.updateMEquities2(meqs, OmahaEquity.EquityType.HILO_HI_HALF, hivals, cards);
            // low winner
            int lw = MEquityUtil.updateMEquities2(meqs, OmahaEquity.EquityType.HILO_AFLO8_HALF, lovals, cards);
            // have to win hi and low for scoop
            if (hw >= 0 && hw == lw)
            {
                meqs[hw].scoopcount++;
            }
        }

        /**
         * Update equities win, tie, win rank and scoop with given hand values for
         * the given cards.
         */
        internal static void updateMEquities(MEquity[] meqs, OmahaEquity.EquityType eqtype, int[] hivals, String[] cards)
        {
            int hw = MEquityUtil.updateMEquities2(meqs, eqtype, hivals, cards);
            if (hw >= 0)
            {
                meqs[hw].scoopcount++;
            }
        }

        /**
         * Update equities win, tie and win rank with given hand values for the
         * given cards. Return index of single winner (scoop), if any, or -1
         */
        private static int updateMEquities2(MEquity[] meqs, OmahaEquity.EquityType eqtype, int[] vals, String[] cards)
        {
            // find highest hand and number of times it occurs
            int max = 0, maxcount = 0;
            for (int i = 0; i < vals.Length; i++)
            {
                int v = vals[i];
                if (v > max)
                {
                    max = v;
                    maxcount = 1;
                }
                else if (v == max)
                {
                    maxcount++;
                }
            }

            int winner = -1;

            for (int i = 0; i < vals.Length; i++)
            {
                if (vals[i] == max)
                {
                    // update the win/tied/rank count
                    OmahaEquity e = meqs[i].getEquity(eqtype);
                    if (maxcount == 1)
                    {
                        winner = i;
                        e.woncount++;
                    }
                    else
                    {
                        e.tiedcount++;
                        e.tiedwithcount += maxcount;
                    }
                    e.wonrankcount[OmahaPoker.rank(max)]++;

                    // count the cards as outs if this turns losing hand into
                    // win/tie or tying hand into win
                    if (cards != null && e.current > 0 && (!(e.curwin || e.curtie) || (e.curtie && maxcount == 1)))
                    {
                        for (int c = 0; c < cards.Length; c++)
                        {
                            String card = cards[c];
                            int cardIndex = OmahaPoker.cardToIndex(card);
                            e.outcount[cardIndex]++;
                        }
                    }

                    // XXX experimental - String[][] mcards
                    /*
                     * if (mcards != null && e.current > 0 && (!e.curwin ||
                     * (e.curtie && maxcount == 1))) { for (int c = 0; c <
                     * mcards[i].length; c++) { String card = mcards[i][c]; int
                     * cardIndex = Poker.cardToIndex(card); e.outcount[cardIndex]++;
                     * } }
                     */
                }
            }

            return winner;
        }

        /**
         * summarise equities (convert counts to percentages)
         */
        internal static void summariseMEquities(MEquity[] meqs, int count, int lowCount)
        {
            // System.out.println("summarise count=" + count + " hilocount=" +
            // hiloCount);
            foreach (MEquity meq in meqs)
            {
                // System.out.println(" meq " + meq);

                OmahaEquity hionly = meq.eqs[0];
                hionly.summariseEquity(count);
                // System.out.println(" hionly won: " + hionly.won + " tied: " +
                // hionly.tied + " total: " + hionly.total);

                if (lowCount == 0)
                {
                    meq.TotalEq = hionly.total;

                }
                else
                {
                    OmahaEquity hihalf = meq.eqs[1];
                    // high count as it applies to every hand not just hi/lo hands
                    hihalf.summariseEquity(count);
                    // System.out.println(" hihalf won: " + hihalf.won + " tied: " +
                    // hihalf.tied + " total: " + hihalf.total);
                    // System.out.println(" hionly+hihalf won: " + (hionly.won +
                    // hihalf.won) + " tied: " + (hionly.tied+hihalf.tied) + "
                    // total: " + (hionly.total+hihalf.total));

                    OmahaEquity lohalf = meq.eqs[2];
                    lohalf.summariseEquity(count);
                    // System.out.println(" lohalf won: " + lohalf.won + " tied: " +
                    // lohalf.tied + " total: " + lohalf.total);

                    meq.lowPossible = (lowCount * 100f) / count;
                    // System.out.println(" low possible: " + meq.lowPossible);

                    meq.TotalEq = hionly.total + (hihalf.total + lohalf.total) / 2;
                }
                // System.out.println(" total eq: " + meq.totaleq);

                meq.scoop = (meq.scoopcount * 100f) / count;
                // System.out.println(" scoop count: " + meq.scoopcount + " scoop: "
                // + meq.scoop);
            }
        }

        internal static void summariseOuts(MEquity[] meqs, int picks, int samples)
        {
            foreach (MEquity meq in meqs)
            {
                foreach (OmahaEquity eq in meq.eqs)
                {
                    eq.summariseOuts(meq.RemCards, picks, samples);
                }
            }
        }

        /**
         * Return string representing current value of hand
         */
        internal static String currentString(MEquity me)
        {
            String s = OmahaPoker.valueString(me.eqs[0].current);
            if (me.HiLo)
            {
                // s += " Hi: " + Poker.valueString(me.eqs[1].current);
                s += " / " + OmahaPoker.valueString(me.eqs[2].current);
            }
            return s;
        }

        internal static String equityStringShort(MEquity me)
        {
            return String.Format("{0:0.00}", me.TotalEq);
        }

        /**
         * Return string representing current equity of hand
         */
        internal static String equityString(MEquity me)
        {
            String s;
            OmahaEquity hionly = me.eqs[0];

            if (me.HiLo)
            {
                OmahaEquity hihalf = me.eqs[1];
                OmahaEquity lohalf = me.eqs[2];
                if (hionly.tied + hihalf.tied + lohalf.tied > 10)
                {
                    // 50.0% (0:100, 0:70)
                    s = String.Format("{0:0.00} ({1}:{2}, {3}:{4})%%", me.TotalEq, hionly.won + hihalf.won,
                            hionly.tied + hihalf.tied, lohalf.won, lohalf.tied);
                }
                else
                {
                    s = String.Format("%{0:0.00} ({1}, {2})%%", me.TotalEq, hionly.won + hihalf.won, lohalf.won);
                }

            }
            else
            {
                s = String.Format("{0:0.00}%%", hionly.won);
                if (hionly.tied > 1)
                {
                    s += String.Format(" ({0:0.00}%% T)", hionly.tied);
                }
            }

            return s;
        }

    }

}
