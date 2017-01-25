using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    /// <summary>
    /// poker hand value and draw prediction function
    /// </summary>
    public abstract class OmahaValue
    {
        /// <summary>
        /// default (non-hi/lo) equity type for this valuation function - see Equity class constants
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public OmahaEquity.EquityType eqtype;
        /// <summary>
        /// number of cards required by value function
        /// </summary>
        public int cards;

        internal OmahaValue(OmahaEquity.EquityType eqtype, int cards)
        {
            this.eqtype = eqtype;
            this.cards = cards;
        }

        /// <summary>
        /// get hand value from subclass
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public abstract int value(String[] hand);

        /// <summary>
        /// get estimated drawing hand. this method is on Value and not Poker because it largely depends on the valuation method rather than the game rules
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="drawn"></param>
        /// <param name="blockers"></param>
        /// <param name="drawList"></param>
        /// <returns></returns>
        public virtual String[] draw(String[] cards, int drawn, String[] blockers, List<Draw> drawList)
        {
            throw new ArgumentException("yawn");
        }
    }

    /// <summary>
    /// calculates high value of hand
    /// </summary>
    internal class HiValue : OmahaValue
    {
        internal HiValue() : base(OmahaEquity.EquityType.HI_ONLY, 5)
        {
        }

        public override int value(string[] hand)
        {
            return OmahaPoker.Value(hand);
        }

        public override string[] draw(String[] cards, int drawn, String[] blockers, List<Draw> drawList)
        {
            return DrawPrediction.getDrawingHand(drawList, cards, drawn, true, blockers);
        }
    }

    /// <summary>
    /// calculates unconditional ace to five low value of hand
    /// </summary>
    internal class AfLowValue : OmahaValue
    {
        internal AfLowValue() : base(OmahaEquity.EquityType.AFLO_ONLY, 5)
        {
        }

        public override int value(string[] hand)
        {
            return OmahaPoker.afLowValue(hand);
        }

        public override string[] draw(String[] cards, int drawn, String[] blockers, List<Draw> drawList)
        {
            return DrawPrediction.getDrawingHand(drawList, cards, drawn, true, blockers);
        }

        // draw - just the lowest unique cards?
    }

    /// <summary>
    /// Calculates ace to five low 8 or better value of hand
    /// </summary>
    internal class AfLow8Value : OmahaValue
    {
        internal AfLow8Value() : base(OmahaEquity.EquityType.AFLO8_ONLY, 5)
        {
        }

        public override int value(string[] hand)
        {
            return OmahaPoker.afLow8Value(hand);
        }
    }

    /// <summary>
    /// deuce to seven low value function
    /// </summary>
    internal class DsLowValue : OmahaValue
    {
        internal DsLowValue() : base(OmahaEquity.EquityType.DSLO_ONLY, 5)
        {
        }

        public override int value(string[] hand)
        {
            return OmahaPoker.dsValue(hand);
        }

        public override string[] draw(String[] cards, int drawn, String[] blockers, List<Draw> drawList)
        {
            return DrawPrediction.getDrawingHand(drawList, cards, drawn, false, blockers);
        }
    }

    /// <summary>
    /// straight hi value only (hi equity type)
    /// </summary>
    internal class StrHiValue : OmahaValue
    {
        internal StrHiValue() : base(OmahaEquity.EquityType.HI_ONLY, 5)
        {
        }

        public override int value(string[] hand)
        {
            return OmahaPoker.strValue(hand);
        }
    }

}


