using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Entities;

namespace DriveHUD.Importers.WinningPokerNetwork
{
    internal class TruePokerImporter : WinningPokerNetworkFileBasedImporter, ITruePokerImporter
    {
        protected override EnumPokerSites Site
        {
            get { return EnumPokerSites.TruePoker; }
        }

        protected override string ProcessName
        {
            get { return "True Poker"; }
        }
    }
}
