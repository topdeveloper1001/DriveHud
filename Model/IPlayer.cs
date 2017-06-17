using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IPlayer
    {
        int PlayerId { get; set; }
        string Name { get; set; }
        string DecodedName { get; }
        string ShortDescription { get; }
        string Description { get; }
        EnumPokerSites? PokerSite { get; }
    }
}
