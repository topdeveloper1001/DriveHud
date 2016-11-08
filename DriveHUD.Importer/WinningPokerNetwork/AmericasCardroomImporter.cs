using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.WinningPokerNetwork
{
    internal class AmericasCardroomImporter : FileBasedImporter, IWinningPokerNetworkImporter
    {
        public override string Site
        {
            get { return EnumPokerSites.AmericasCardroom.ToString(); }
        }

        protected override string HandHistoryFilter
        {
            get { return "*.txt"; }
        }

        protected override string ProcessName
        {
            get { return "AmericasCardroom"; }
        }
    }
}
