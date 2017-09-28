#region Usings

using System.Collections.Generic;

#endregion

namespace AcePokerSolutions.DataTypes
{
    public static class CompareHelpers
    {
        public static bool CompareIntegerLists(List<int> l1, List<int> l2)
        {
            if (l1.Count != l2.Count)
                return false;

            foreach (int i in l1)
            {
                if (!l2.Contains(i))
                    return false;
            }

            foreach (int i in l2)
            {
                if (!l1.Contains(i))
                    return false;
            }

            return true;
        }

        public static bool CompareStringLists(List<string> l1, List<string> l2)
        {
            if (l1.Count != l2.Count)
                return false;

            foreach (string i in l1)
            {
                if (!l2.Contains(i))
                    return false;
            }

            foreach (string i in l2)
            {
                if (!l1.Contains(i))
                    return false;
            }

            return true;
        }
    }
}