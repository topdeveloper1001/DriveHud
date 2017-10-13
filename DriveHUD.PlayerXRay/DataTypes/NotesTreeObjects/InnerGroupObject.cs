//-----------------------------------------------------------------------
// <copyright file="InnerGroupObject.cs" company="Ace Poker Solutions">
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
using System.Collections.ObjectModel;

namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects
{
    public class InnerGroupObject : NoteTreeEditableObject
    {
        public InnerGroupObject()
        {
            notes = new ReactiveList<NoteObject>();
        }      

        private ReactiveList<NoteObject> notes;

        public ReactiveList<NoteObject> Notes
        {
            get
            {
                return notes;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref notes, value);
            }
        }
    }
}