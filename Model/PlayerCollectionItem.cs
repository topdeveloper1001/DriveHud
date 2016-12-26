using DriveHUD.Entities;
using System.Web;

namespace Model
{
    public struct PlayerCollectionItem
    {
        public int PlayerId { get; set; }

        public string Name { get; set; }

        public EnumPokerSites PokerSite { get; set; }

        public string DecodedName
        {
            get
            {
                return HttpUtility.HtmlDecode(Name);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerCollectionItem && this == (PlayerCollectionItem)obj;
        }

        public override int GetHashCode()
        {          
            return PlayerId.GetHashCode();
        }

        public static bool operator ==(PlayerCollectionItem x, PlayerCollectionItem y)
        {
            return x.PlayerId == y.PlayerId;
        }

        public static bool operator !=(PlayerCollectionItem x, PlayerCollectionItem y)
        {
            return !(x == y);
        }
    }
}