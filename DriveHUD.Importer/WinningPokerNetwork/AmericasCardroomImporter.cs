using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Parser.Parsers;
using DriveHUD.Common.Progress;
using Microsoft.Practices.ServiceLocation;
using HandHistories.Parser.Parsers.Exceptions;
using DriveHUD.Common.Log;
using HandHistories.Parser.Parsers.Factory;
using DriveHUD.Common.WinApi;
using System.Diagnostics;
using HandHistories.Objects.GameDescription;
using System.Globalization;
using System.IO;
using DriveHUD.Common.Extensions;
using HandHistories.Parser.Utils.FastParsing;
using Model.Settings;
using HandHistories.Objects.Players;
using HandHistories.Objects.Hand;
using DriveHUD.Importers.Helpers;

namespace DriveHUD.Importers.WinningPokerNetwork
{
    internal class AmericasCardroomImporter : WinningPokerNetworkFileBasedImporter, IAmericasCardroomImporter
    {
        protected override EnumPokerSites Site
        {
            get { return EnumPokerSites.AmericasCardroom; }
        }

        protected override string ProcessName
        {
            get { return "AmericasCardroom"; }
        }
    }
}
