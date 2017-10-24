//-----------------------------------------------------------------------
// <copyright file="NotesAppSettings.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings
{
    [Serializable]
    public class NotesAppSettings
    {
        public NotesAppSettings()
        {
            LastBackupDate = DateTime.MinValue;
            Profiles = new List<ProfileObject>();
            StagesList = new List<StageObject>();
            ShowHoleCards = true;
            HoleCardsNumber = 3;
            AutoNotesEnabled = true;
            NoteCreationSinceDate = DateTime.Today.AddMonths(-2);
        }

        public bool AutoNotesEnabled { get; set; }

        public bool TakesNotesOnHero { get; set; }

        public bool IsNoteCreationSinceDate { get; set; }

        public DateTime NoteCreationSinceDate { get; set; }

        public bool ShowHoleCards { get; set; }

        public bool AllHoleCards { get; set; }

        public int HoleCardsNumber { get; set; }

        public string ServerConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public DateTime LastBackupDate { get; set; }

        public List<StageObject> StagesList { get; set; }

        public List<ProfileObject> Profiles { get; set; }

        [XmlIgnore]
        public List<NoteObject> AllNotes
        {
            get
            {
                var result = new List<NoteObject>();

                foreach (var stage in StagesList)
                {
                    foreach (var group in stage.InnerGroups)
                    {
                        result.AddRange(group.Notes);
                    }

                    result.AddRange(stage.Notes);
                }

                return result;
            }
        }
    }
}