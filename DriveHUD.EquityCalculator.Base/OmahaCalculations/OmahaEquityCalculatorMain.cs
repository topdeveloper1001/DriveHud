using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    /*
     C# port for https://github.com/alexyz/poker
     Can be used for Holdem equity as well, but we use another tool for holdem equity calculations
         */
    /// <summary>
    /// Hold'em and Omaha hand analysis, using a combinatorial number system.
    /// </summary>
    public class OmahaEquityCalculatorMain : OmahaPoker
    {
        /// <summary>
        /// check board is either null or no more than 5 cards
        /// </summary>
        /// <param name="board"></param>
        private static void validateBoard(String[] board)
        {
            if (board != null && board.Length > 5)
            {
                throw new ArgumentException("invalid board: " + string.Concat(board));
            }
        }

        //
        // instance stuff
        //

        /** must use 2 cards */
        private bool omaha;
        private int min;
        private bool hilo;
        private OmahaValue loValue;


        /// <summary>
        /// create holdem equity calculator for given game type
        /// </summary>
        /// <param name="omaha"></param>
        /// <param name="hilo"></param>
        public OmahaEquityCalculatorMain(bool omaha, bool hilo) : this(omaha, hilo, new HiValue(), new AfLow8Value())
        {
        }

        /// <summary>
        /// create holdem equity calculator for given game type
        /// </summary>
        /// <param name="omaha"></param>
        /// <param name="hilo"></param>
        /// <param name="hi"></param>
        /// <param name="lo"></param>
        public OmahaEquityCalculatorMain(bool omaha, bool hilo, OmahaValue hi, OmahaValue lo) : base(hi)
        {
            this.omaha = omaha;
            this.loValue = lo;
            this.min = omaha ? 2 : 0;
            this.hilo = hilo;
        }

        /// <summary>
        /// check hole has at least 1 or 2 cards and at most 2 or 4 cards
        /// </summary>
        /// <param name="hole"></param>
        private void validateHoleCards(String[] hole)
        {
            int min = omaha ? 2 : 1;
            int max = omaha ? 5 : 2;
            if (hole.Length < min || hole.Length > max)
            {
                throw new ArgumentException("invalid hole cards: " + string.Concat(hole));
            }
        }

        public override MEquity[] Equity(string[] board, string[][] holeCards, string[] blockers, int draws)
        {
            if (draws != 0)
            {
                throw new ArgumentException("invalid draws: " + draws);
            }

            validateBoard(board);
            foreach (var hole in holeCards)
            {
                validateHoleCards(hole);
            }

            // cards not used by hands or board
            String[] deck = OmahaPoker.remdeck(holeCards, board, blockers);

            if (board.Length <= 1)
            {
                // monte carlo (random sample boards)
                return equityImpl(new HEBoardSample(deck, board, 10000), holeCards);

            }
            else
            {
                // all possible boards
                return equityImpl(new HEBoardEnum(deck, board), holeCards);
            }
        }

        internal override int Value(String[] board, String[] hole)
        {
            validateBoard(board);
            validateHoleCards(hole);

            if (board == null || board.Length < 3)
            {
                // could use the draw poker getPair method...
                return 0;

            }
            else
            {
                return heValue(value, board, hole, new String[5]);
            }
        }

        /// <summary>
        ///  Calc exact tex/omaha hand equity for each hand for given board
        /// </summary>
        /// <param name="HEBoard"></param>
        /// <param name=""></param>
        /// <param name="String"></param>
        /// <param name=""></param>
        /// <returns></returns>
        private MEquity[] equityImpl(HEBoard heboard, String[][] holeCards)
        {

            // XXX low possible should really be a method on Value
            bool lowPossible;
            if (hilo)
            {
                if (heboard.current != null && heboard.current.Length > 2)
                {
                    // only possible if there are no more than 2 high cards on board
                    lowPossible = heboard.current.Length - lowCount(heboard.current, false) <= 2;
                }
                else
                {
                    lowPossible = true;
                }
            }
            else
            {
                lowPossible = false;
            }

            // note: HL MEquity actually contains 3 equity types, so can be treated as high only
            MEquity[] meqs = MEquityUtil.createMEquitiesHL(hilo, holeCards.Length, heboard.deck.Length, heboard.Exact());
            int[] hivals = new int[holeCards.Length];
            int[] lovals = lowPossible ? new int[holeCards.Length] : null;
            String[] temp = new String[5];

            // get current high hand values (not equity)
            if (heboard.current != null)
            {
                for (int n = 0; n < holeCards.Length; n++)
                {
                    if (heboard.current.Length >= 3)
                    {
                        hivals[n] = heValue(value, heboard.current, holeCards[n], temp);
                    }
                }
                MEquityUtil.updateCurrent(meqs, OmahaEquity.EquityType.HI_ONLY, hivals);

                if (lowPossible)
                {
                    MEquityUtil.updateCurrent(meqs, OmahaEquity.EquityType.HILO_HI_HALF, hivals);
                    // get current low values
                    for (int n = 0; n < holeCards.Length; n++)
                    {
                        lovals[n] = heValue(loValue, heboard.current, holeCards[n], temp);
                    }
                    MEquityUtil.updateCurrent(meqs, OmahaEquity.EquityType.HILO_AFLO8_HALF, lovals);
                }
            }

            // get equity
            int count = heboard.Count();
            int pick = heboard.Pick();
            String[] outs = pick <= 2 ? new String[pick] : null;
            int hiloCount = 0;

            for (int p = 0; p < count; p++)
            {
                // get board
                heboard.Next();
                //System.out.println("board p: " + p + " current: " + Arrays.toString(heboard.current) + " next: " + Arrays.toString(heboard.board));

                // hi equity
                for (int i = 0; i < holeCards.Length; i++)
                {
                    hivals[i] = heValue(value, heboard.board, holeCards[i], temp);
                }

                // low equity - only counts if at least one hand makes low
                bool hasLow = false;
                if (lowPossible)
                {
                    for (int i = 0; i < holeCards.Length; i++)
                    {
                        int v = heValue(loValue, heboard.board, holeCards[i], temp);
                        if (v > 0)
                        {
                            hasLow = true;
                        }
                        lovals[i] = v;
                    }
                }

                // XXX this is ugly, should be in HEBoardEnum class only
                if (outs != null)
                {
                    for (int n = 0; n < pick; n++)
                    {
                        outs[n] = heboard.board[5 - pick + n];
                    }
                }

                if (hasLow)
                {
                    hiloCount++;
                    MEquityUtil.updateMEquitiesHL(meqs, hivals, lovals, outs);

                }
                else
                {
                    // high winner
                    MEquityUtil.updateMEquities(meqs, OmahaEquity.EquityType.HI_ONLY, hivals, null);
                }
            }

            MEquityUtil.summariseMEquities(meqs, count, hiloCount);
            // XXX shouldn't be here, just need to store pick and count on mequity
            MEquityUtil.summariseOuts(meqs, pick, count);
            return meqs;
        }


        /// <summary>
        /// Calculate value of holdem/omaha hand (using at least min cards from hand). Board can be 3-5 cards.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name=""></param>
        /// <param name="String"></param>
        /// <param name=""></param>
        /// <param name="String"></param>
        /// <param name=""></param>
        /// <param name="String"></param>
        /// <param name=""></param>
        /// <returns></returns>
        private int heValue(OmahaValue v, String[] board, String[] hole, String[] temp)
        {
            int hv = 0;
            for (int n = min; n <= 2; n++)
            {
                int nh = MathsUtil.binomialCoefficientFast(hole.Length, n);
                int nb = MathsUtil.binomialCoefficientFast(board.Length, 5 - n);
                for (int kh = 0; kh < nh; kh++)
                {
                    MathsUtil.kCombination(n, kh, hole, temp, 0);
                    for (int kb = 0; kb < nb; kb++)
                    {
                        MathsUtil.kCombination(5 - n, kb, board, temp, n);
                        int val = v.value(temp);
                        //System.out.println(Arrays.asList(h5) + " - " + Poker.desc(v));
                        if (val > hv)
                        {
                            hv = val;
                        }
                    }
                }
            }
            return hv;
        }

        internal override int  minHoleCards()
        {
            return min;
        }
    }
}
