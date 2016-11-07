//-----------------------------------------------------------------------
// <copyright file="ConvertedResult.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.Builders.iPoker;
using System.Collections.Generic;

namespace DriveHUD.Importers.BetOnline
{
    /// <summary>
    /// Result of converting xml
    /// </summary>
    internal class ConvertedResult
    {
        public string ConvertedXml { get; set; }

        public string TableName { get; set; }

        public string TableId { get; set; }

        public string HandNumber { get; set; }

        public List<Player> Players { get; set; }

        public GameInfo GameInfo { get; set; }
    }
}