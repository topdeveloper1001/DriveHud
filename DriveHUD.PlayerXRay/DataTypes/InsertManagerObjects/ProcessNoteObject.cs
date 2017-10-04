using System;
using System.Collections.Generic;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;

namespace DriveHUD.PlayerXRay.DataTypes.InsertManagerObjects
{
    public class ProcessNoteObject
    {
        public List<ProcessPlayerObject> Players { get; set; }
        public List<ProcessPlayerObject> ComparisonPlayers { get; set; }
        public NoteObject Note { get; set; }
        public bool Cash { get; set; }

        public IEnumerable<long> PlayerIDs
        {
            get
            {
                List<long> res = new List<long>();
                foreach (ProcessPlayerObject player in Players)
                {
                    res.Add(player.ID);
                }
                return res;
            }
        }

        public IEnumerable<long> ComparisonPlayerIDs
        {
            get
            {
                if (ComparisonPlayers == null || ComparisonPlayers.Count == 0)
                    return new List<long>();

                List<long> res = new List<long>();
                foreach (ProcessPlayerObject player in ComparisonPlayers)
                {
                    res.Add(player.ID);
                }
                return res;
            }
        }
    }
}
