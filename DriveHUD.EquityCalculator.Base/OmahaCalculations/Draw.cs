using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    /// <summary>
    /// represents a possible draw and its average score
    /// </summary>
    public class Draw : IComparable<Draw>
    {


        public readonly String[] cards;
        public float score;

        public Draw(String[] hole, float score)
        {
            this.cards = hole;
            this.score = score;
        }

        public int CompareTo(Draw other)
        {
            return (int)Math.Sign(score - other.score);
        }

        public override string ToString()
        {
            return string.Format("{0:0.00} -> ", score) + string.Concat(cards);
        }
    }
}
