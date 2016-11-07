//-----------------------------------------------------------------------
// <copyright file="ColorDto.cs" company="Ace Poker Solutions">
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
using System.Windows.Media;

namespace DriveHUD.Application.Surrogates
{
    [ProtoContract]
    public class ColorDto
    {
        [ProtoMember(1, IsRequired = true)]
        public byte A { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public byte R { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public byte G { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public byte B { get; set; }

        public static implicit operator System.Windows.Media.Color(ColorDto value)
        {
            var result = value == null ? Colors.Black : Color.FromArgb(value.A, value.R, value.G, value.B);
            return result;
        }

        public static implicit operator ColorDto(System.Windows.Media.Color value)
        {
            return new ColorDto { A = value.A, B = value.B, G = value.G, R = value.R };
        }
    }
}
