//-----------------------------------------------------------------------
// <copyright file="PointDto.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Application.Surrogates
{
    [ProtoContract]
    public class PointDto
    {
        [ProtoMember(1)]
        public double X { get; set; }

        [ProtoMember(2)]
        public double Y { get; set; }

        public static implicit operator System.Windows.Point(PointDto value)
        {
            return value == null ? new System.Windows.Point(0, 0) : new System.Windows.Point(value.X, value.Y);
        }

        public static implicit operator PointDto(System.Windows.Point value)
        {
            return new PointDto { X = value.X, Y = value.Y };
        }
    }
}