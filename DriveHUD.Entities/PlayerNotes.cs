﻿//-----------------------------------------------------------------------
// <copyright file="Playernotes.cs" company="Ace Poker Solutions">
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
using System.ComponentModel.DataAnnotations;

namespace DriveHUD.Entities
{
    public partial class Playernotes
    {        
        public virtual int PlayerNoteId { get; set; }

        public virtual Players Player { get; set; }

        [Required]
        public virtual string Note { get; set; }

        [Required]
        public virtual int PlayerId { get; set; }

        [Required]
        public virtual short PokersiteId { get; set; }

        public virtual string CardRange { get; set; }

        public virtual DateTime? Timestamp { get; set; }

        public virtual bool IsAutoNote { get; set; }

        public virtual long GameNumber { get; set; }

        public virtual string PlayerName
        {
            get
            {
                return Player?.Playername ?? string.Empty;
            }
        }
    }
}