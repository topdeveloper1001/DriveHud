using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    /**
  * generate sample remaining boards
  */
    class HEBoardSample : HEBoard
    {

        private long[] picked = new long[1];
        private int count;
        private Random r = new Random();

        public HEBoardSample(String[] deck, String[] current, int count) : base(deck, current)
        {
            this.count = count;
            for (int n = 0; n < current.Length; n++)
            {
                board[n] = current[n];
            }
        }

        internal override int Count()
        {
            return count;
        }

        internal override int Pick()
        {
            return 5;
        }

        internal override void Next()
        {
            picked[0] = 0;
            for (int n = current.Length; n < 5; n++)
            {
                board[n] = ArrayUtil.pick(r, deck, picked);
            }
        }

        internal override bool Exact()
        {
            return false;
        }

    }
}
