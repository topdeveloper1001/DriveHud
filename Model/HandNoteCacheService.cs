//-----------------------------------------------------------------------
// <copyright file="HandNoteCacheService.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using System;
using System.Collections.Concurrent;

namespace Model
{
    internal class HandNoteCacheService : IHandNoteCacheService
    {
        private readonly ConcurrentDictionary<HandNoteKey, Handnotes> handNotes = new ConcurrentDictionary<HandNoteKey, Handnotes>();

        public void AddHandNote(Handnotes handNote)
        {
            if (handNote == null)
            {
                return;
            }

            try
            {
                var noteKey = new HandNoteKey(handNote.Gamenumber, handNote.PokersiteId);                            
                handNotes.AddOrUpdate(noteKey, handNote, (key, oldValue) => handNote);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Failed to add hand note to note cache service.", e);
            }
        }

        public bool TryGetNote(long gameNumber, int pokerSite, out Handnotes handNote)
        {
            var noteKey = new HandNoteKey(gameNumber, pokerSite);              
            return handNotes.TryRemove(noteKey, out handNote);
        }

        private class HandNoteKey
        {
            private readonly long gameNumber;
            private readonly int pokerSite;

            public HandNoteKey(long gameNumber, int pokerSite)
            {
                this.gameNumber = gameNumber;
                this.pokerSite = pokerSite;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + gameNumber.GetHashCode();
                    hashcode = (hashcode * 31) + pokerSite.GetHashCode();
                    return hashcode;
                }
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as HandNoteKey);
            }

            protected bool Equals(HandNoteKey noteKey)
            {
                return noteKey != null && noteKey.gameNumber == gameNumber &&
                    noteKey.pokerSite == pokerSite;
            }
        }
    }
}