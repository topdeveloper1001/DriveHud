using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    /// <summary>
    ///  Mathematical utility methods
    /// </summary>
    public class MathsUtil
    {

        private static int[,] C = makeBinomialCoefficients(52, 52);

        /// <summary>
        /// Factorial (slow)
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static BigInteger factorial(int n)
        {
            return n <= 1 ? BigInteger.One : BigInteger.Multiply(new BigInteger(n), factorial(n - 1));
        }

        /// <summary>
        /// Binomial coefficient (slow)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static BigInteger binomialCoefficient(int n, int k)
        {
            return n == 0 ? BigInteger.Zero : BigInteger.Divide(factorial(n), BigInteger.Multiply(factorial(k), factorial(n - k)));
        }

        /// <summary>
        /// Calculate binomial coefficients
        /// </summary>
        /// <param name="nm"></param>
        /// <param name="km"></param>
        /// <returns></returns>
        private static int[,] makeBinomialCoefficients(int nm, int km)
        {
            BigInteger max = new  BigInteger(int.MaxValue);
            int[,] r = new int[nm + 1, km + 1];
            for (int n = 0; n <= nm; n++)
            {
                for (int k = 0; k <= km; k++)
                {
                    BigInteger v = binomialCoefficient(n, k);
                    if (v.CompareTo(max) > 0)
                    {
                        r[n, k] = -1;
                    }
                    else
                    {
                        r[n, k] = (int)v;
                    }
                }
            }
            return r;
        }

        /**
         * Return cached binomial coefficient (n pick k).
         * I.e. how many ways can you pick k objects from n
         */
        public static int binomialCoefficientFast(int n, int k)
        {
            int c = C[n, k];
            if (c == -1)
            {
                throw new ArgumentException("no binomial coefficient for " + n + ", " + k);
            }
            return c;
        }

        /// <summary>
        /// Combinatorial number system. Get the k combination at position p and write from 'from' into 'to' at offset.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="p"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="off"></param>
        public static void kCombination(int k, int p, Object[] from, Object[] to, int off)
        {
            // for each digit (starting at the last)
            for (int b = k; b >= 1; b--)
            {
                // find biggest bin coff that will fit p
                for (int a = b - 1; a < 100; a++)
                {
                    int x = binomialCoefficientFast(a, b);
                    if (x > p)
                    {
                        // this is too big, so the last one must have fit
                        p -= binomialCoefficientFast(a - 1, b);
                        to[b - 1 + off] = from[a - 1];
                        break;
                    }
                }
            }
        }

        public static float trunc(float f)
        {
            return (float)Math.Round(f);
        }

        public static float round(float f, int dp)
        {
            float pow = (float)Math.Pow(10, dp);
            float round = (float)Math.Round(f * pow);
            return round / pow;
        }
    }

}
