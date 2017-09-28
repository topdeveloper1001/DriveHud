#region Usings

using System.Collections.Generic;

#endregion

namespace AcePokerSolutions.DataTypes.NotesTreeObjects
{
    public class StageObject
    {
        public StageObject()
        {
            InnerGroups = new List<InnerGroupObject>();
            Notes = new List<NoteObject>();
        }

        public NoteStageType StageType { get; set; }
        public List<InnerGroupObject> InnerGroups { get; set; }
        public List<NoteObject> Notes { get; set; }
    }
}