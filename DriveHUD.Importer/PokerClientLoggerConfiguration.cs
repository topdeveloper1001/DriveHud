//-----------------------------------------------------------------------
// <copyright file="IPokerClientLoggerConfiguration.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers
{
    /// <summary>
    /// Logger configuration
    /// </summary>
    public class PokerClientLoggerConfiguration
    {
        public string LogDirectory { get; set; }

        public string LogTemplate { get; set; }

        public string LogCleanupTemplate { get; set; }

        public string DateFormat { get; set; }

        public string DateTimeFormat { get; set; }

        public string PublicKey { get; set; }
    }
}