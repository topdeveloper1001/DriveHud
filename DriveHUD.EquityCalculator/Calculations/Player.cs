using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Calculations
{
    class Player
    {
        internal bool mActive;

        internal int mHoleCount;

        internal double mWins;
        internal double WinPrct;
        internal double TiePrct;
        internal double mEquity;
        internal double mTies;
        internal float mFolds;
        internal String Range;
        internal double mCurrentWeight;
        internal long mCurrentHand;
        internal WeightTable mHoleTable = new WeightTable();
        internal WeightTable mFoldTable = new WeightTable();

        public double Equity;

        internal Player(String range)
        {
            this.Range = range;
        }
        internal void Reset()
        {
            mWins = mEquity = mTies = 0;
            mCurrentWeight = 0;
            mCurrentHand = 0;

            mActive = false;
        }
    }
}
