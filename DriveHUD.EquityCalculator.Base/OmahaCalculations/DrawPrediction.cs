using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    public class DrawPrediction
    {
        /// <summary>
        /// get the best drawing hand for the given hand, number drawn and hand valuation. optionally returns score of all possible drawing hands.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="hand"></param>
        /// <param name="drawn"></param>
        /// <param name="high"></param>
        /// <param name="blockers"></param>
        /// <returns></returns>
        public static String[] getDrawingHand(List<Draw> list, String[] hand, int drawn, bool high, String[] blockers)
        {
            if (hand.Length > 5)
            {
                throw new ArgumentException("invalid hand: " + (string.Join(", ", hand)));
            }

            // XXX really should take into account multiple draws
            // but thats only really a problem if you draw a greater number on a
            // later street (which is a bad strategy and almost unheard of)
            // e.g. draw 1, 1, 5 - obviously can't use final hand to predict any of them
            // related problem is that a later hand might contain blockers from a
            // reshuffle and so can't possibly occur on an earlier street
            // and you might not even have enough cards that aren't blocked,
            // i.e. bincoff(length(hand - blockers), 5 - drawn) needs to be >= 1
            if (blockers != null && blockers.Length > 0)
            {
                String[] hand2 = ArrayUtil.sub(hand, blockers);
                if (hand2.Length != hand.Length)
                {
                    // some of the cards were blocked
                    // cheat and increase the draw amount (if necessary)
                    drawn = Math.Max(5 - hand2.Length, drawn);
                    hand = hand2;
                }
            }

            BigInteger combs = MathsUtil.binomialCoefficient(hand.Length, 5 - drawn);
            if ((int)combs <= 0)
            {
                throw new ArgumentException("invalid combs: " + combs);
            }

            // XXX if only 1 comb, just return hand?

            // high draw works best with around 0.9, low draw with 0.99
            // generally, you can win in high with any top 10% hand, but low draw
            // pretty much needs 7-high (75432, 76432, 76542, etc) to win
            // XXX actually it probably depends if it's 2-7 single or triple draw
            double bias = high ? 0.9 : 0.99;
            OmahaValue value;
            if (high)
            {
                value = new HiValue();
            }
            else
            {
                value = new DsLowValue();
            }

            if (drawn < 0 || drawn > 5)
            {
                throw new ArgumentException("invalid drawn: " + drawn);

            }
            else if (drawn == 5)
            {
                // special case, no draw and no meaningful score
                return new String[0];

            }
            else if (drawn == 0)
            {
                // special case, nothing to test other than given hand
                if (list != null)
                {
                    float s = score(value.value(hand), bias);
                    list.Add(new Draw(hand, s));
                }
                return hand.ToArray();
            }

            // drawing 1-4

            // from players point of view, all other cards are possible (even the blockers)
            String[] deck = OmahaPoker.remdeck(null, hand);
            String[] drawnHand = new String[5];
            int imax = MathsUtil.binomialCoefficientFast(hand.Length, 5 - drawn);
            int jmax = MathsUtil.binomialCoefficientFast(deck.Length, drawn);

            String[] maxDrawingHand = null;
            float maxScore = -1f;

            for (int i = 0; i < imax; i++)
            {
                ArrayUtil.Populate(drawnHand, null);
                // pick kept from hand
                MathsUtil.kCombination(5 - drawn, i, hand, drawnHand, 0);
                //System.out.println("drawnHand: " + Arrays.toString(drawnHand));
                float score = 0;

                for (int j = 0; j < jmax; j++)
                {
                    // pick drawn from deck
                    MathsUtil.kCombination(drawn, j, deck, drawnHand, 5 - drawn);
                    //System.out.println("  drawnHand: " + Arrays.toString(drawnHand));
                    int v = value.value(drawnHand);
                    score += DrawPrediction.score(v, bias);
                }

                float averageScore = score / (1.0f * jmax);
                string[] drawingHand = drawnHand.Take(5 - drawn).ToArray();

                if (list != null)
                {
                    Array.Sort(drawingHand, Cmp.revCardCmp);
                    list.Add(new Draw(drawingHand, averageScore));
                }

                if (score > maxScore)
                {
                    // copy new max hole cards
                    maxDrawingHand = drawingHand;
                    maxScore = score;
                }
            }

            if (list != null)
            {
                list.Sort();
                list.Reverse();
            }
            return maxDrawingHand;
        }

        /// <summary>
        ///  get normalised score of hand (i.e. hand value is 0-1), optionally 
        ///  inverted.bias is 0.5 to 1, representing how many values are less than 
        ///  0.5, e.g. 0.9 means 90% of values are less than 0.5
        /// </summary>
        /// <param name=""></param>
        /// <param name="value"></param>
        /// <param name=""></param>
        /// <param name="bias"></param>
        /// <returns></returns>
        protected static float score(int value, double bias)
        {
            if (bias < 0.5 || bias > 1.0)
            {
                throw new ArgumentException("invalid bias " + bias);
            }

            // get high value
            bool high;
            int highValue;
            switch (value & OmahaPoker.TYPE)
            {
                case OmahaPoker.HI_TYPE:
                    high = true;
                    highValue = value;
                    break;
                case OmahaPoker.DS_LOW_TYPE:
                    high = false;
                    highValue = OmahaPoker.HI_TYPE | (OmahaPoker.MAX_RANK - (value & OmahaPoker.HAND));
                    break;
                default:
                    // ace to five doesn't include str/fl
                    // but then, no drawing games use ace to five values so doesn't matter
                    throw new ArgumentException("can't get score of " + value.ToString("X4"));
            }

            int[] highValues = OmahaPoker.highValues();
            int p = Array.BinarySearch(highValues, highValue);
            if (p < 0)
            {
                throw new ArgumentException("not a high value: " + highValue.ToString("X4"));
            }

            if (!high)
            {
                // invert score for deuce to seven low
                p = highValues.Length - 1 - p;
            }

            // raise score to some power to bias toward high values
            // note: for k=x^y, y=log(k)/log(x)... i think
            return (float)Math.Pow((1f * p) / (highValues.Length - 1f), Math.Log(0.5) / Math.Log(bias));
        }
    }

}
