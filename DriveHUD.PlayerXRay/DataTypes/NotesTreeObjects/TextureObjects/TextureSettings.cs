#region Usings

using System.Collections.Generic;
using System.Xml.Serialization;

#endregion

namespace AcePokerSolutions.DataTypes.NotesTreeObjects.TextureObjects
{
    public class TextureSettings
    {
        public bool IsPairedFilter { get; set; }
        public bool IsGutshotsFilter { get; set; }
        public bool IsCardTextureFilter { get; set; }
        public bool IsHighcardFilter { get; set; }
        public bool IsPossibleStraightsFilter { get; set; }

        public CompareEnum PossibleStraightsCompare { get; set; }
        public int PossibleStraights { get; set; }

        public int Gutshots { get; set; }
        public string HighestCard { get; set; }
        public string SelectedCardTexture { get; set; }
        public bool IsPairedFilterTrue { get; set; }

        [XmlIgnore]
        public List<string> SelectedCardTextureList
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedCardTexture))
                    return new List<string>();
                return !SelectedCardTexture.Contains(",")
                           ? new List<string> {SelectedCardTexture}
                           : new List<string>(SelectedCardTexture.Split(','));
            }
            set
            {
                SelectedCardTexture = string.Empty;

                foreach (string card in value)
                {
                    SelectedCardTexture += card + ',';
                }
                if (SelectedCardTexture.Contains(","))
                    SelectedCardTexture = SelectedCardTexture.Remove(SelectedCardTexture.LastIndexOf(','), 1);
            }
        }

        public bool EqualsBase(object x)
        {
            TextureSettings x1 = (TextureSettings) x;
            TextureSettings x2 = this;

            if (x1.IsGutshotsFilter != x2.IsGutshotsFilter)
                goto False;
            if (x1.IsGutshotsFilter)
            {
                if (x1.Gutshots != x2.Gutshots)
                    goto False;
            }

            if (x1.IsHighcardFilter != x2.IsHighcardFilter)
                goto False;
            if (x1.IsHighcardFilter)
            {
                if (x1.HighestCard != x2.HighestCard)
                    goto False;
            }

            if (x1.IsPairedFilter != x2.IsPairedFilter)
                goto False;
            if (x1.IsPairedFilter)
            {
                if (x1.IsPairedFilterTrue != x2.IsPairedFilterTrue)
                    goto False;
            }

            if (x1.IsPossibleStraightsFilter != x2.IsPossibleStraightsFilter)
                goto False;
            if (x1.IsPossibleStraightsFilter)
            {
                if (x1.PossibleStraights != x2.PossibleStraights ||
                    x1.PossibleStraightsCompare != x2.PossibleStraightsCompare)
                    goto False;
            }

            if (x1.IsCardTextureFilter != x2.IsCardTextureFilter)
                goto False;
            if (x1.IsCardTextureFilter)
            {
                if (!CompareHelpers.CompareStringLists(x1.SelectedCardTextureList, x2.SelectedCardTextureList))
                    goto False;
            }

            return true;

            False:
            return false;
        }
    }
}