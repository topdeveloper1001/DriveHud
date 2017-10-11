//-----------------------------------------------------------------------
// <copyright file="StageObject.cs" company="Ace Poker Solutions">
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
    public class StageObject : NoteTreeObjectBase
    {
        public StageObject()
        {
            innerGroups = new ReactiveList<InnerGroupObject>();
            notes = new ReactiveList<NoteObject>();
        }

        private NoteStageType stageType;

        public NoteStageType StageType
        {
            get
            {
                return stageType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref stageType, value);
            }
        }

        private ReactiveList<InnerGroupObject> innerGroups;

        public ReactiveList<InnerGroupObject> InnerGroups
        {
            get
            {
                return innerGroups;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref innerGroups, value);
            }
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