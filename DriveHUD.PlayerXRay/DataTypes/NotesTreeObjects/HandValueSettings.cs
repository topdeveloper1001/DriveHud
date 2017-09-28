#region Usings

using System.Collections.Generic;

#endregion

namespace AcePokerSolutions.DataTypes.NotesTreeObjects
{
    public class HandValueSettings
    {
        public HandValueSettings()
        {
            AnyHv = true;
            AnyStraightDraws = true;
            AnyFlushDraws = true;

            SelectedFlushDraws = new List<int>();
            SelectedHv = new List<int>();
            SelectedStraighDraws = new List<int>();
        }

        public bool AnyHv { get; set; }
        public bool AnyStraightDraws { get; set; }
        public bool AnyFlushDraws { get; set; }

        public List<int> SelectedHv { get; set; }
        public List<int> SelectedStraighDraws { get; set; }
        public List<int> SelectedFlushDraws { get; set; }

        public override bool Equals(object x)
        {
            HandValueSettings x1 = (HandValueSettings) x;
            HandValueSettings x2 = this;

            return x1.AnyHv == x2.AnyHv &&
                   x1.AnyFlushDraws == x2.AnyFlushDraws &&
                   x1.AnyStraightDraws == x2.AnyStraightDraws &&
                   CompareHelpers.CompareIntegerLists(x1.SelectedFlushDraws, x2.SelectedFlushDraws) &&
                   CompareHelpers.CompareIntegerLists(x1.SelectedHv, x2.SelectedHv) &&
                   CompareHelpers.CompareIntegerLists(x1.SelectedStraighDraws, x2.SelectedStraighDraws);
        }
    }
}