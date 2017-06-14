using System.ComponentModel.DataAnnotations;

namespace DriveHUD.Entities
{
    public partial class PlayerAliases
    {
        public virtual int PlayerAliasesId { get; set; }

        [Required]
        public virtual string AliasId { get; set; }

        [Required]
        public virtual string PlayersId { get; set; }
    }
}
