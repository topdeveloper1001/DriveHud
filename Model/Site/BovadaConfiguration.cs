//-----------------------------------------------------------------------
// <copyright file="BovadaConfiguration.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model.Site
{
    public class BovadaConfiguration : BaseSiteConfiguration, ISiteConfiguration
    {
        private static readonly string[] registryKeys = new[] { "{D7CA2DF8-95CE-4C80-9296-98E21219A1E4}}_is1", "{D7CA2DF8-95CE-4C80-9296-98E21219A1E5}}_is1", "{D7CA2DF8-95CE-4C80-9296-98E21219A1E7}}_is1" };

        private static readonly string[] defaultInstallFolders = new[] { "c:\\Bodog", "c:\\Bovada", "c:\\Ignition" };

        private static readonly string[] defaultUninstallDisplayNames = new[] { "Ignition Casino", "BovadaPoker", "BodogPoker" };

        private const string heroName = "Hero";

        public BovadaConfiguration()
        {
            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,
                EnumTableType.Three,
                EnumTableType.Six,
                EnumTableType.Nine
            };

            HeroName = heroName;
        }

        public override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.Ignition;
            }
        }

        private readonly IEnumerable<EnumTableType> tableTypes;

        public override IEnumerable<EnumTableType> TableTypes
        {
            get
            {
                return tableTypes;
            }
        }

        public override string HeroName
        {
            get;
            set;
        }

        public override bool IsPrefferedSeatsAllowed
        {
            get
            {
                return true;
            }
        }

        public override Dictionary<int, int> PreferredSeats
        {
            get
            {
                var preferredSeats = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == Site)?.PrefferedSeats;
                var filteredSeatsList = preferredSeats?.Where(x => x.IsPreferredSeatEnabled && x.PreferredSeat != -1);
                var seatsDictonary = new Dictionary<int, int>();

                if (filteredSeatsList == null)
                {
                    return seatsDictonary;
                }

                foreach (var preferredSeat in filteredSeatsList)
                {
                    seatsDictonary.Add((int)preferredSeat.TableType, preferredSeat.PreferredSeat);
                }

                return seatsDictonary;
            }
        }

        public override string LogoSource
        {
            get
            {
                return "/DriveHUD.Common.Resources;Component/images/SiteLogos/ignition_logo.png";
            }
        }

        public override string[] GetHandHistoryFolders()
        {
            return new string[] { };
        }

        public override ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                IsDetected = DetectSite(),
                IsEnabled = siteModel.Enabled,
            };

            return validationResult;
        }

        /// <summary>
        /// Detects whenever Ignition/Bodog/Bovada poker client is installed
        /// </summary>
        /// <returns>True if installed, otherwise - false</returns>
        private bool DetectSite()
        {
            // check the registry for the specific keys
            var result = RegistryUtils.UninstallRegistryContainsKeys(registryKeys);

            if (result)
            {
                return true;
            }

            // check for default paths
            if (defaultInstallFolders.Any(x => Directory.Exists(x)))
            {
                return true;
            }

            // check registry for installed programs
            return RegistryUtils.UninstallRegistryContainsDisplayNames(defaultUninstallDisplayNames);
        }
    }
}