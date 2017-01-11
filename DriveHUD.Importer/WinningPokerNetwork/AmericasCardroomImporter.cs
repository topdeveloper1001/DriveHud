//-----------------------------------------------------------------------
// <copyright file="AmericasCardroomImporter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;

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
