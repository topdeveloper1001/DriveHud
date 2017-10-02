#region Usings

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AcePokerSolutions.DataTypes;
using AcePokerSolutions.DataTypes.NotesTreeObjects;

#endregion

namespace AcePokerSolutions.BusinessHelper.ApplicationSettings
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
        }

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
                List<NoteObject> result = new List<NoteObject>();

                foreach (StageObject stage in StagesList)
                {
                    foreach (InnerGroupObject group in stage.InnerGroups)
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