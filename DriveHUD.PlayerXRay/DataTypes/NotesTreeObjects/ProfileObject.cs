#region Usings

using System.Collections.Generic;

#endregion

namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects
{

    public class ProfileObject
    {
        public ProfileObject()
        {
            ContainingNotes = new List<int>();
        }

        public List<int> ContainingNotes { get; set; }
        public string Name { get; set; }
    }
}