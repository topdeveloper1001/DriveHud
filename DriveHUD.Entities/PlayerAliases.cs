using System.ComponentModel.DataAnnotations;

namespace DriveHUD.Entities
{
    public partial class AliasPlayer
    {
        public virtual int AliasId { get; set; }

        public virtual int PlayersId { get; set; }

        public override int GetHashCode()
        {
            return AliasId;
        }

        public override bool Equals(object obj)
        {
            var other = obj as AliasPlayer;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.AliasId == other.AliasId &&
                this.PlayersId == other.PlayersId;
        }
    }
}
