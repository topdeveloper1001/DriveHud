using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Calculations
{
    class Weight
    {
        internal float mWeight, mCumWeight;
        internal Weight()
        {
            mWeight = -1.0f;
        }


        internal long mCards;
        internal bool mFlag;
        internal int mCard0;
        internal int mCard1;
        internal int mHandEval = 0;
        internal void Reset()
        {
            mWeight = 0;
            mCumWeight = 0;

        }
    }
}
