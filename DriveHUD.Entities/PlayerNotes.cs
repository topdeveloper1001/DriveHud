//-----------------------------------------------------------------------
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
        private const string autoNoteSeparateLine = "---- XRay Notes ----";

        public virtual int PlayerNoteId { get; set; }

        public virtual Players Player { get; set; }

        public virtual string Note
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(AutoNote))
                {
                    return $"{ManualNote}{Environment.NewLine}{autoNoteSeparateLine}{Environment.NewLine}{AutoNote}";
                }

                return ManualNote;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    ManualNote = AutoNote = null;
                }

                var splittedNotes = value.Split(new[] { autoNoteSeparateLine }, StringSplitOptions.RemoveEmptyEntries);

                if (splittedNotes.Length == 0)
                {
                    ManualNote = AutoNote = null;
                }

                if (splittedNotes.Length > 1)
                {
                    AutoNote = splittedNotes[1];
                }
                else
                {
                    AutoNote = null;
                }

                ManualNote = splittedNotes[0];
            }
        }

        public virtual string ManualNote { get; set; }

        public virtual string AutoNote { get; set; }

        [Required]
        public virtual int PlayerId { get; set; }

        [Required]
        public virtual short PokersiteId { get; set; }

        public virtual string PlayerName
        {
            get
            {
                return Player?.Playername ?? string.Empty;
            }
        }
    }
}