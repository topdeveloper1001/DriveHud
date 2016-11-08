//-----------------------------------------------------------------------
// <copyright file="Pacific888Importer.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Utils.Extensions;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Settings;
using Model.Site;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.Pacific888
{
    internal class Pacific888Importer : FileBasedImporter, IPacific888Importer
    {
        protected override string ProcessName
        {
            get
            {
                return "poker";
            }
        }

        protected override string HandHistoryFilter
        {
            get
            {
                return "*.txt";
            }
        }

        public override string Site
        {
            get
            {
                return EnumPokerSites.Poker888.ToString();
            }
        }

        protected override bool Match(string title, ParsingResult parsingResult)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return false;
            }

            return true;
        }
    }
}