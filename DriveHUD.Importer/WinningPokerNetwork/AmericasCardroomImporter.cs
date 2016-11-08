using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.WinningPokerNetwork
{
    internal class AmericasCardroomImporter : BaseImporter, IWinningPokerNetworkImporter
    {
        public override string Site
        {
            get
            {
                return EnumPokerSites.AmericasCardroom.ToString();
            }
        }

        protected override void DoImport()
        {
            throw new NotImplementedException();
        }
    }
}
