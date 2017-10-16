//-----------------------------------------------------------------------
// <copyright file="CompareHelpers.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.DataTypes
{
    public static class CompareHelpers
    {
        public static bool CompareIntegerLists(IEnumerable<int> l1, IEnumerable<int> l2)
        {
            if (l1.Count() != l2.Count())
            {
                return false;
            }

            foreach (int i in l1)
            {
                if (!l2.Contains(i))
                {
                    return false;
                }
            }

            foreach (int i in l2)
            {
                if (!l1.Contains(i))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CompareStringLists(IEnumerable<string> l1, IEnumerable<string> l2)
        {
            if (l1.Count() != l2.Count())
            {
                return false;
            }

            foreach (string i in l1)
            {
                if (!l2.Contains(i))
                {
                    return false;
                }
            }

            foreach (string i in l2)
            {
                if (!l1.Contains(i))
                {
                    return false;
                }
            }

            return true;
        }
    }
}