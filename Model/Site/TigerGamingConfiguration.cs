//-----------------------------------------------------------------------
// <copyright file="TigerGamingConfiguration.cs" company="Ace Poker Solutions">
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

namespace Model.Site
{
    public class TigerGamingConfiguration : BetOnlineConfiguration
    {
        private readonly string[] registryKeys = new[] { "TigerGaming 0" };

        private readonly string[] defaultUninstallDisplayNames = new[] { "TigerGaming" };

        public override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.TigerGaming;
            }
        }

        public override string LogoSource
        {
            get
            {
                return "/DriveHUD.Common.Resources;Component/images/SiteLogos/tigergaming_logo.png";
            }
        }

        protected override string[] RegistryKeys
        {
            get
            {
                return registryKeys;
            }
        }

        protected override string[] UninstallDisplayNames
        {
            get
            {
                return defaultUninstallDisplayNames;
            }
        }
    }
}