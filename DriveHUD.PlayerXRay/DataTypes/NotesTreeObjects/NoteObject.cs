//-----------------------------------------------------------------------
// <copyright file="NoteObject.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ReactiveUI;

namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects
{
    public class NoteObject : NoteTreeEditableObject
    {
        public NoteObject()
        {
            Settings = new NoteSettingsObject();
            DisplayedNote = "Unknown";
        }

        private int id;

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref id, value);
            }
        }      

        private string description;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref description, value);
            }
        }

        private string displayedNote;

        public string DisplayedNote
        {
            get
            {
                return displayedNote;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref displayedNote, value);
            }
        }

        private NoteSettingsObject settings;

        public NoteSettingsObject Settings
        {
            get
            {
                return settings;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref settings, value);
            }
        }
    }
}