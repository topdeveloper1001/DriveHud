//-----------------------------------------------------------------------
// <copyright file="Utils.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Annotations;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Data
{
    public class NoteBuilder
    {
        private const string autoNoteSeparateLine = "---- X-Ray Notes ----";

        public static string BuildNote([NotNull] IEnumerable<Playernotes> notes)
        {
            if (notes == null)
            {
                throw new ArgumentNullException(nameof(notes));
            }

            var manualNoteText = string.Join(Environment.NewLine, notes.Where(x => !x.IsAutoNote)
                .OrderBy(x => x.Timestamp)
                .Select(x => x.Note)
                .ToArray());

            var autoNotesLines = (from note in notes
                                  where note.IsAutoNote
                                  group note by note.Note into groupedNotes
                                  let noteText = $"{groupedNotes.Key}"
                                  let cardRange = string.Join(", ", groupedNotes
                                        .Where(x => !string.IsNullOrEmpty(x.CardRange))
                                        .Select(x => x.CardRange)
                                        .Distinct()
                                        .ToArray())
                                  let count = groupedNotes.Count()
                                  let cardRangeText = !string.IsNullOrWhiteSpace(cardRange) ? $" {cardRange}" : string.Empty
                                  let countText = count > 1 ? $" ({count})" : string.Empty
                                  select $"{noteText}{cardRangeText}{countText}").ToArray();

            var autoNoteText = string.Join(Environment.NewLine, autoNotesLines);

            var combinedNoteText = string.Empty;

            if (!string.IsNullOrEmpty(manualNoteText))
            {
                combinedNoteText += manualNoteText;
            }

            if (!string.IsNullOrEmpty(autoNoteText))
            {
                if (!string.IsNullOrEmpty(combinedNoteText))
                {
                    combinedNoteText += Environment.NewLine;
                }

                combinedNoteText += autoNoteSeparateLine;
                combinedNoteText += Environment.NewLine;
                combinedNoteText += autoNoteText;
            }

            return combinedNoteText;
        }

        public static Tuple<string, string> ParseNotes(string noteText)
        {
            if (string.IsNullOrWhiteSpace(noteText))
            {
                new Tuple<string, string>(null, null);
            }

            var indexOfSeparateLine = noteText.LastIndexOf(autoNoteSeparateLine);

            if (indexOfSeparateLine < 0)
            {
                return new Tuple<string, string>(noteText, null);
            }

            var autoNotes = noteText.Length > indexOfSeparateLine + autoNoteSeparateLine.Length ?
                noteText.Substring(indexOfSeparateLine + autoNoteSeparateLine.Length).Trim() :
                null;

            var manualNotes = indexOfSeparateLine > 0 ? noteText.Substring(0, indexOfSeparateLine).Trim() : null;

            return new Tuple<string, string>(manualNotes, autoNotes);
        }
    }
}