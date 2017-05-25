//-----------------------------------------------------------------------
// <copyright file="HudPlayerTypeStat.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Hud player type stat range
    /// </summary>
    [Serializable]
    public class HudPlayerTypeStat : BaseHudRangeStat
    {
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 23;
                hash = (hash * 31) + Low.GetHashCode();
                hash = (hash * 31) + High.GetHashCode();
                hash = (hash * 31) + Stat.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var hudPlayerTypeStat = obj as HudPlayerTypeStat;

            if (hudPlayerTypeStat == null)
            {
                return false;
            }

            return Equals(hudPlayerTypeStat);
        }

        private bool Equals(HudPlayerTypeStat hudPlayerTypeStat)
        {
            var result = ((Low.HasValue && hudPlayerTypeStat.Low == Low) || (!Low.HasValue && !hudPlayerTypeStat.Low.HasValue)) &&
                ((High.HasValue && hudPlayerTypeStat.High == High) || (!High.HasValue && !hudPlayerTypeStat.High.HasValue)) &&
                hudPlayerTypeStat.Stat == Stat;

            return result;
        }
    }
}