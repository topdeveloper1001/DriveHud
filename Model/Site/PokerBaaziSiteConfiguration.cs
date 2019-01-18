//-----------------------------------------------------------------------
// <copyright file="PokerBaaziSiteConfiguration.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
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
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;

namespace Model.Site
{
    public class PokerBaaziSiteConfiguration : BaseSiteConfiguration, ISiteConfiguration
    {
        private readonly string[] possibleFolders = new[]
       {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PokerBaazi")
        };

        private const string launcherFile = "PokerBaazi.exe";

        private readonly string[] defaultUninstallDisplayNames = new[] { "PokerBaazi" };

        public PokerBaaziSiteConfiguration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,                
                EnumTableType.Six,
                EnumTableType.Nine
            };
        }

        public override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.PokerBaazi;
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

        private readonly Dictionary<int, int> prefferedSeat;

        public override Dictionary<int, int> PreferredSeats
        {
            get
            {
                return prefferedSeat;
            }
        }

        public override bool IsHandHistoryLocationRequired
        {
            get
            {
                return false;
            }
        }

        public override bool IsPrefferedSeatsAllowed
        {
            get
            {
                return false;
            }
        }

        public override string LogoSource
        {
            get
            {
                return "/DriveHUD.Common.Resources;Component/images/SiteLogos/pokerbaazi_logo.png";
            }
        }

        public override string[] GetHandHistoryFolders()
        {
            return new string[0];
        }

        public override ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            if (siteModel == null)
            {
                return null;
            }

            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                IsDetected = DetectSite(),
                IsEnabled = siteModel.Enabled,
            };

            return validationResult;
        }

        /// <summary>
        /// Detects whenever PokerBaazi poker client is installed
        /// </summary>
        /// <returns>True if installed, otherwise - false</returns>
        protected virtual bool DetectSite()
        {
            foreach (var possibleFolder in possibleFolders)
            {
                var launcher = Path.Combine(possibleFolder, launcherFile);

                if (File.Exists(launcher))
                {
                    return true;
                }
            }

            // check registry for installed programs
            return RegistryUtils.UninstallRegistryContainsDisplayNames(defaultUninstallDisplayNames);
        }
    }
}