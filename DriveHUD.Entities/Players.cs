//-----------------------------------------------------------------------
// <copyright file="Players.cs" company="Ace Poker Solutions">
// Copyright � 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace DriveHUD.Entities
{
    [ProtoContract]
    public partial class Players
    {
        [ProtoMember(1)]
        public virtual int PlayerId { get; set; }

        [Required, ProtoMember(2)]
        public virtual string Playername { get; set; }

        [Required, ProtoMember(3)]
        public virtual short PokersiteId { get; set; }

        [Required]
        public virtual int Tourneyhands { get; set; }

        [Required]
        public virtual int Cashhands { get; set; }
    }
}