//-----------------------------------------------------------------------
// <copyright file="EnumPositionExtensions.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Entities
{
    public static class EnumPositionExtensions
    {
        public static bool IsBBPosition(this EnumPosition position)
        {
            return position == EnumPosition.BB;
        }

        public static bool IsSBPosition(this EnumPosition position)
        {
            return position == EnumPosition.SB;
        }

        public static bool IsEPPosition(this EnumPosition position)
        {
            return position == EnumPosition.EP || position == EnumPosition.UTG ||
                   position == EnumPosition.UTG_1 || position == EnumPosition.UTG_2;
        }

        public static bool IsMPPosition(this EnumPosition position)
        {
            return position == EnumPosition.MP || position == EnumPosition.MP1 ||
                    position == EnumPosition.MP2 || position == EnumPosition.MP3;
        }

        public static bool IsCOPosition(this EnumPosition position)
        {
            return position == EnumPosition.CO;
        }

        public static bool IsBTNPosition(this EnumPosition position)
        {
            return position == EnumPosition.BTN;
        }

        public static bool IsStraddlePosition(this EnumPosition position)
        {
            return position == EnumPosition.STRDL;
        }
    }
}