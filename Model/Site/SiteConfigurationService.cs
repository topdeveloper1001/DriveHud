//-----------------------------------------------------------------------
// <copyright file="SiteConfigurationService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using DriveHUD.Entities;
using System;

namespace Model.Site
{
    /// <summary>
    /// Provides site configurations
    /// </summary>
    public class SiteConfigurationService : ISiteConfigurationService
    {
        private List<ISiteConfiguration> configurations;

        public SiteConfigurationService()
        {
            configurations = new List<ISiteConfiguration>();
        }

        /// <summary>
        /// Initialize sites configurations
        /// </summary>
        public void Initialize()
        {
            var sites = new EnumPokerSites[]
            {
                EnumPokerSites.Ignition,
                EnumPokerSites.BetOnline,
                EnumPokerSites.TigerGaming,
                EnumPokerSites.SportsBetting,
                EnumPokerSites.PokerStars,
                EnumPokerSites.Poker888,
                EnumPokerSites.AmericasCardroom,
                EnumPokerSites.BlackChipPoker,
                EnumPokerSites.TruePoker,
                EnumPokerSites.YaPoker,
            };

            foreach (EnumPokerSites site in sites)
            {
                try
                {
                    var configuration = ServiceLocator.Current.GetInstance<ISiteConfiguration>(site.ToString());
                    configurations.Add(configuration);
                }
                catch
                {
                    throw new DHInternalException(new NonLocalizableString("Not supported site [{0}]", site));
                }
            }
        }

        /// <summary>
        /// Checks the Poker Sites Validity
        /// </summary>
        public void ValidateSiteConfigurations()
        {
            foreach (var configuration in configurations)
            {
                configuration.ValidateSiteConfiguration();
            }
        }

        /// <summary>
        /// Get site configuration
        /// </summary>
        /// <param name="site">Site name</param>
        /// <returns>Site configuration</returns>
        public ISiteConfiguration Get(string site)
        {
            switch (site)
            {
                case "Bovada":
                case "Ignition":
                    return Get(EnumPokerSites.Ignition);
                case "BetOnline":
                    return Get(EnumPokerSites.BetOnline);
                case "TigerGaming":
                    return Get(EnumPokerSites.TigerGaming);
                case "SportsBetting":
                    return Get(EnumPokerSites.SportsBetting);
                case "PokerStars":
                    return Get(EnumPokerSites.PokerStars);
                case "Poker888":
                    return Get(EnumPokerSites.Poker888);
                case "AmericasCardroom":
                    return Get(EnumPokerSites.AmericasCardroom);
                case "BlackChipPoker":
                    return Get(EnumPokerSites.BlackChipPoker);
                case "TruePoker":
                    return Get(EnumPokerSites.TruePoker);
                case "YaPoker":
                    return Get(EnumPokerSites.YaPoker);
                default:
                    throw new DHInternalException(new NonLocalizableString("Not supported site [{0}]", site));

            }
        }

        /// <summary>
        /// Get site configuration
        /// </summary>
        /// <param name="site">Site</param>
        /// <returns>Site configuration</returns>
        public ISiteConfiguration Get(EnumPokerSites site)
        {
            var configuration = configurations.FirstOrDefault(x => x.Site == site);

            if (configuration == null)
            {
                throw new DHInternalException(new NonLocalizableString("Not supported site"));
            }

            return configuration;
        }

        /// <summary>
        /// Get all sites configurations
        /// </summary>        
        /// <returns>Collection of sites configurations</returns>
        public IEnumerable<ISiteConfiguration> GetAll()
        {
            return configurations;
        }
    }
}