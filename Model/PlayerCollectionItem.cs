using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public struct PlayerCollectionItem
    {
        public string Name { get; set; }
        public EnumPokerSites PokerSite { get; set; }

        public override bool Equals(Object obj)
        {
            return obj is PlayerCollectionItem && this == (PlayerCollectionItem)obj;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ PokerSite.GetHashCode();
        }

        public static bool operator ==(PlayerCollectionItem x, PlayerCollectionItem y)
        {
            return x.Name == y.Name && x.PokerSite == y.PokerSite;
        }

        public static bool operator !=(PlayerCollectionItem x, PlayerCollectionItem y)
        {
            return !(x == y);
        }
    }
}
