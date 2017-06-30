//-----------------------------------------------------------------------
// <copyright file="PositionalStat.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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
    /// <summary>
    /// Represents stat with separate value for each possible position
    /// </summary>
    public sealed class PositionalStat
    {
        public int EP { get; set; }

        public int MP { get; set; }

        public int CO { get; set; }

        public int BN { get; set; }

        public int SB { get; set; }

        public int BB { get; set; }

        /// <summary>
        /// Adds positional values of the specified <see cref="PositionalStat"/> to the current <see cref="PositionalStat"/>
        /// </summary>  
        public void Add(PositionalStat stat)
        {
            if (stat == null)
            {
                return;
            }

            EP += stat.EP;
            MP += stat.MP;
            CO += stat.CO;
            BN += stat.BN;
            SB += stat.SB;
            BB += stat.BB;
        }

        /// <summary>
        /// Create new instance of <see cref="PositionalStat"/> which is the sum of the specified <see cref="PositionalStat"/>
        /// </summary>  
        public static PositionalStat Sum(PositionalStat a, PositionalStat b)
        {
            if (a == null)
            {
                a = new PositionalStat();
            }

            if (b == null)
            {
                b = new PositionalStat();
            }

            return new PositionalStat
            {
                EP = a.EP + b.EP,
                MP = a.MP + b.MP,
                CO = a.CO + b.CO,
                BN = a.BN + b.BN,
                SB = a.SB + b.SB,
                BB = a.BB + b.BB,
            };
        }

        /// <summary>
        /// Sets the value for the specified <see cref="EnumPosition"/>
        /// </summary>      
        public void SetPositionValue(EnumPosition position, int value)
        {
            switch (position)
            {
                case EnumPosition.BTN:
                    BN = value;
                    break;
                case EnumPosition.SB:
                    SB = value;
                    break;
                case EnumPosition.BB:
                    BB = value;
                    break;
                case EnumPosition.CO:
                    CO = value;
                    break;
                case EnumPosition.MP3:
                case EnumPosition.MP2:
                case EnumPosition.MP1:
                case EnumPosition.MP:
                    MP = value;
                    break;
                case EnumPosition.UTG:
                case EnumPosition.UTG_1:
                case EnumPosition.UTG_2:
                case EnumPosition.EP:
                    EP = value;
                    break;
                case EnumPosition.Undefined:
                default:
                    break;
            }
        }
    }
}