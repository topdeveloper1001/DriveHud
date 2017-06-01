using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Entities;

namespace DriveHUD.Importers.WinningPokerNetwork
{
    internal class YaPokerImporter : WinningPokerNetworkFileBasedImporter, IYaPokerImporter
    {
        protected override EnumPokerSites Site
        {
            get { return EnumPokerSites.YaPoker; }
        }

        protected override string ProcessName
        {
            get { return "YaPoker"; }
        }
    }
}
