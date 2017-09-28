#region Usings

using System.Collections.Generic;

#endregion

namespace AcePokerSolutions.DataTypes.NotesTreeObjects
{
    public class InnerGroupObject
    {
        public InnerGroupObject()
        {
            Notes = new List<NoteObject>();
        }

        public string Name { get; set; }
        public List<NoteObject> Notes { get; set; }
    }
}