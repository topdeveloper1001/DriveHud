using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    /// <summary>
    /// utilities for arrays.
    /// </summary>
    internal static class ArrayUtil
    {
        /// <summary>
        /// Populate array with value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="value"></param>
        internal static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        /// <summary>
        /// join two arrays together
        /// </summary>
        internal static T[] join<T>(T[] a, T[] b)
        {
            return a.Concat(b).ToArray();
        }

        /// <summary>
        /// pick a value from a (max length 63) that hasn't been picked before according to picked[0] and update picked[0]
        /// </summary>
        /// <param name="r"></param>
        /// <param name="a"></param>
        /// <param name="picked"></param>
        /// <returns></returns>
        internal static String pick(Random r, String[] a, long[] picked)
        {
            if (a.Length > 63)
            {
                throw new ArgumentException("array is longer than 63");
            }
            if (picked[0] >= ((1L << a.Length) - 1))
            {
                throw new ArgumentException("none left to pick");
            }
            while (true)
            {
                int i = r.Next(a.Length);
                long m = 1L << i;
                if ((picked[0] & m) == 0)
                {
                    picked[0] |= m;
                    return a[i];
                }
            }
        }

        /// <summary>
        /// shuffle array
        /// </summary>
        /// <param name="a"></param>
        /// <param name="r"></param>
        internal static void shuffle(Object[] a, Random r)
        {
            for (int n = 0; n < a.Length; n++)
            {
                // don't just pick random position!
                int x = r.Next(a.Length - n) + n;
                Object o = a[n];
                a[n] = a[x];
                a[x] = o;
            }
        }

        /// <summary>
        /// subtract b from a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static String[] sub(String[] a, String[] b)
        {
            return a.Except(b).ToArray();
        }

        internal static void main(String[] args)
        {
            Random r = new Random();
            for (int n = 0; n < 10; n++)
            {
                int[] a = new int[r.Next(5) + 1];
                for (int i = 0; i < a.Length; i++)
                {
                    a[i] = r.Next(10);
                }
                int[] b = a.ToArray();
                sort(b);
            }
        }

        /// <summary>
        /// sort a small array of numbers, more efficiently than Arrays.sort
        /// </summary>
        /// <param name="a"></param>
        internal static void sort(int[] a)
        {
            // simple insertion sort derived from wikipedia
            for (int i = 1; i < a.Length; i++)
            {
                int v = a[i];
                int h = i;
                while (h > 0 && v < a[h - 1])
                {
                    a[h] = a[h - 1];
                    h--;
                }
                a[h] = v;
            }
        }

    }
}
