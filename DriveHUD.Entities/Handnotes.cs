//-----------------------------------------------------------------------
// <copyright file="Handnotes.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DriveHUD.Entities
{

    public partial class Handnotes
    {
        public virtual int HandNoteId { get; set; }

        [Required]
        public virtual long Gamenumber { get; set; }

        public virtual int? IconId { get; set; }

        public virtual string Note { get; set; }

        [Required]
        public virtual short PokersiteId { get; set; }

        public virtual int? DisplayOrder { get; set; }

        public virtual int? HandTag { get; set; }  
    }
}