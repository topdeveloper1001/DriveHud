//-----------------------------------------------------------------------
// <copyright file="SolidColorBrushDto.cs" company="Ace Poker Solutions">
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
    public class SolidColorBrushDto
    {
        [ProtoMember(1)]
        public Color Color { get; set; }

        public static implicit operator SolidColorBrush(SolidColorBrushDto value)
        {
            return value == null ? null : new SolidColorBrush(value.Color);
        }

        public static implicit operator SolidColorBrushDto(SolidColorBrush value)
        {
            return value == null ? null : new SolidColorBrushDto { Color = value.Color };
        }
    }
}