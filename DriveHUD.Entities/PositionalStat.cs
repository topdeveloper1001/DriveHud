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

using ProtoBuf;

namespace DriveHUD.Entities
{
    /// <summary>
    /// Represents stat with separate value for each possible position
    /// </summary>
    [ProtoContract]
    public sealed class PositionalStat
    {
        [ProtoMember(1)]
        public int EP { get; set; }

        [ProtoMember(2)]
        public int MP { get; set; }

        [ProtoMember(3)]
        public int CO { get; set; }

        [ProtoMember(4)]
        public int BN { get; set; }

        [ProtoMember(5)]
        public int SB { get; set; }

        [ProtoMember(6)]
        public int BB { get; set; }

        /// <summary>
        /// Adds positional values for the specified <see cref="EnumPosition"/>
        /// </summary>  
        public void Add(EnumPosition position, int value)
        {
            switch (position)
            {
                case EnumPosition.BTN:
                    BN += value;
                    break;
                case EnumPosition.SB:
                    SB += value;
                    break;
                case EnumPosition.BB:
                    BB += value;
                    break;
                case EnumPosition.CO:
                    CO += value;
                    break;
                case EnumPosition.MP3:
                case EnumPosition.MP2:
                case EnumPosition.MP1:
                case EnumPosition.MP:
                    MP += value;
                    break;
                case EnumPosition.UTG:
                case EnumPosition.UTG_1:
                case EnumPosition.UTG_2:
                case EnumPosition.EP:
                    EP += value;
                    break;
                case EnumPosition.Undefined:
                default:
                    break;
            }
        }

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

        /// <summary>
        /// Resets all values
        /// </summary>
        public void Reset()
        {
            EP = 0;
            MP = 0;
            CO = 0;
            BN = 0;
            SB = 0;
            BB = 0;
        }
    }
}