//-----------------------------------------------------------------------
// <copyright file="IPokerImporter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Entities;
using HandHistories.Parser.Parsers;

namespace DriveHUD.Importers.IPoker
{
    internal class IPokerImporter : FileBasedImporter, IIPokerImporter
    {
        protected override string HandHistoryFilter
        {
            get
            {
                return "*.xml";
            }
        }

        protected override string ProcessName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.IPoker;
            }
        }

        protected override bool InternalMatch(string title, ParsingResult parsingResult)
        {
            throw new NotImplementedException();
        }
    }
}
