using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    internal class HEBoardEnum : HEBoard
    {
        /// <summary>
        /// number of boards
        /// </summary>
        private int count;
        /// <summary>
        /// number of cards to pick
        /// </summary>
        private int k;
        /// <summary>
        /// board number
        /// </summary>
        private int p = 0;

        internal HEBoardEnum(String[] deck, String[] current) : base(deck, current)
        {
            for (int n = 0; n < current.Length; n++)
            {
                board[n] = current[n];
            }
            k = 5 - current.Length;
            count = MathsUtil.binomialCoefficientFast(deck.Length, k);
        }

        internal override int Count()
        {
            return count;
        }

        internal override int Pick()
        {
            return k;
        }

        internal override void Next()
        {
            // get board combination
            MathsUtil.kCombination(k, p++, deck, board, current.Length);
        }

        internal override bool Exact()
        {
            return true;
        }
    }
}
