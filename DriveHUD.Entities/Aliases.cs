using System.ComponentModel.DataAnnotations;

namespace DriveHUD.Entities
{
    public partial class Aliases
    {
        public virtual int AliasId { get; set; }

        [Required]
        public virtual string AliasName { get; set; }
    }
}
