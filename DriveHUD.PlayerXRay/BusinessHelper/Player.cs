using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.Entities;

namespace DriveHUD.PlayerXRay.BusinessHelper
{   /// <summary>
/// This class is only needed to retrieve data from DH WCF, 
/// PlayerObject (DataType project) is a class for the x-ray note calculations
/// </summary>
    public class Player
    {
        public int PlayerId { get; set; }

        public IEnumerable<int> PlayerIds { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> LinkedAliases { get; set; }

        public EnumPokerSites? PokerSite { get; set; }

        public IEnumerable<EnumPokerSites> PokerSites { get; set; }
    }
}
