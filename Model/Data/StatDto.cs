//-----------------------------------------------------------------------
// <copyright file="StatDto.cs" company="Ace Poker Solutions">
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

namespace Model.Data
{
    [ProtoContract]
    [ProtoInclude(4, typeof(HeatMapStatDto))]
    public class StatDto
    {
        public StatDto()
        {
        }

        public StatDto(int? occurred, int? couldOccurred)
        {
            Occurred = occurred.HasValue ? occurred.Value : 0;
            CouldOccurred = couldOccurred.HasValue ? couldOccurred.Value : 0;
        }

        [ProtoMember(1)]
        public virtual decimal Value { get; set; }

        [ProtoMember(2)]
        public virtual int Occurred { get; set; }

        [ProtoMember(3)]
        public virtual int CouldOccurred { get; set; }

        public override string ToString()
        {
            return string.Format("{0:n1} {1}/{2}", Value, Occurred, CouldOccurred);
        }
    }
}