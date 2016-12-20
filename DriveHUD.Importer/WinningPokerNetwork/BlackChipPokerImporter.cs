using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.WinningPokerNetwork
{
    internal class BlackChipPokerImporter : WinningPokerNetworkFileBasedImporter, IBlackChipPokerImporter
    {
        protected override EnumPokerSites Site
        {
            get { return EnumPokerSites.BlackChipPoker; }
        }

        protected override string ProcessName
        {
            get { return "BlackChipPoker"; }
        }
    }
}
