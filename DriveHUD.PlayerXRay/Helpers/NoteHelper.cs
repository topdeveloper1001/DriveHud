//-----------------------------------------------------------------------
// <copyright file="NoteHelper.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Entities;
using System;
using System.Linq;

namespace DriveHUD.PlayerXRay.Helpers
{
    internal class NoteHelper
    {
        /// <summary>
        /// Combines the specified notes
        /// </summary>
        /// <param name="existingNote">Note to combine</param>
        /// <param name="note">Note to combine</param>
        public static string CombineAutoNotes(Playernotes existingNote, Playernotes note)
        {
            if (existingNote == null)
            {
                throw new ArgumentNullException(nameof(existingNote));
            }

            if (existingNote == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            var noteName = ParseNoteName(note.AutoNote);
            var noteCardRange = ParseCardRange(note.AutoNote);

            var noteLines = existingNote.AutoNote.GetLines(true).ToArray();

            var isNewNote = true;

            for (var i = 0; i < noteLines.Length; i++)
            {
                if (!noteLines[i].StartsWith(noteName))
                {
                    continue;
                }

                isNewNote = false;

                var existingNoteCardRange = ParseCardRange(noteLines[i]);
                var existingNoteCount = ParseNoteCount(noteLines[i]);

                if (string.IsNullOrEmpty(existingNoteCardRange))
                {
                    existingNoteCardRange = $" [{noteCardRange}]";
                }
                else if (!existingNoteCardRange.Contains(noteCardRange))
                {
                    existingNoteCardRange = $" [{existingNoteCardRange},{noteCardRange}]";
                }

                existingNoteCount++;

                noteLines[i] = $"{noteName}{existingNoteCardRange} ({existingNoteCount})";
            }

            if (isNewNote)
            {
                return $"{existingNote.AutoNote}{Environment.NewLine}{note.AutoNote}";
            }

            return string.Join(Environment.NewLine, noteLines);
        }

        /// <summary>
        /// Parses the note name from text
        /// </summary>
        /// <param name="noteText">Text to parse note name</param>
        /// <returns>Note name</returns>
        public static string ParseNoteName(string noteText)
        {
            if (string.IsNullOrEmpty(noteText))
            {
                return noteText;
            }

            var cardRangeStartIndex = noteText.LastIndexOf("[");

            if (cardRangeStartIndex < 1)
            {
                return noteText;
            }

            var noteName = noteText.Substring(0, cardRangeStartIndex - 1).Trim();

            return noteName;
        }

        /// <summary>
        /// Parses the cards range from text
        /// </summary>
        /// <param name="noteText">Text to parse cards range</param>
        /// <returns>Cards range</returns>
        public static string ParseCardRange(string noteText)
        {
            if (string.IsNullOrEmpty(noteText))
            {
                return noteText;
            }

            var cardRangeStartIndex = noteText.LastIndexOf("[");
            var cardRangeEndIndex = noteText.LastIndexOf("]");

            if (cardRangeStartIndex < 1 || cardRangeEndIndex < 1)
            {
                return string.Empty;
            }

            var cardRange = noteText.Substring(cardRangeStartIndex + 1, cardRangeEndIndex - cardRangeStartIndex - 1).Trim();

            return cardRange;
        }

        /// <summary>
        /// Parses the note count value from text
        /// </summary>
        /// <param name="noteText">Text to parse note count</param>
        /// <returns>Note count</returns>
        public static int ParseNoteCount(string noteText)
        {
            if (string.IsNullOrEmpty(noteText))
            {
                return 0;
            }

            var noteCountStartIndex = noteText.LastIndexOf("(");
            var noteCountEndIndex = noteText.LastIndexOf(")");

            if (noteCountStartIndex < 1 || noteCountEndIndex < 1)
            {
                return 1;
            }

            var noteCount = noteText.Substring(noteCountStartIndex + 1, noteCountEndIndex - noteCountStartIndex - 1).Trim();

            int count;

            if (int.TryParse(noteCount, out count))
            {
                return count;
            }

            return 1;
        }
    }
}