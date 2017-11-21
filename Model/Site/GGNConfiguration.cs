//-----------------------------------------------------------------------
// <copyright file="GGNConfiguration.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model.Site
{
    public class GGNConfiguration : BaseSiteConfiguration, ISiteConfiguration
    {
        private static readonly string defaultLaunchFile = "bin\\GGnet.exe";

        private static readonly string[] networkSites = new[] { "Natural8", "AllNewPoker", "BestPoker", "DakaPoker", "LotosPoker", "Pokamania", "PPI POKER",
            "Tianlong", "TiltKing Poker", "W88", "YouLe" };

        public GGNConfiguration()
        {
            tableTypes = new EnumTableType[]
            {
                EnumTableType.Six,
                EnumTableType.Nine
            };
        }

        public override string LogoSource
        {
            get
            {
                return string.Empty;
            }
        }

        public override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.GGN;
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

        public override bool IsAddon
        {
            get
            {
                return true;
            }
        }

        public override string AddonText
        {
            get
            {
                return CommonResourceManager.Instance.GetResourceString("Settings_GGNAddonText");
            }
        }

        public override string AddonTooltip
        {
            get
            {
                return CommonResourceManager.Instance.GetResourceString("Settings_GGNAddonTooltip");
            }
        }

        public override string[] GetHandHistoryFolders()
        {
            return new string[0];
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
        /// Detects whenever ggn poker client is installed
        /// </summary>
        /// <returns>True if installed, otherwise - false</returns>
        private bool DetectSite()
        {
            var installDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            var potentialLaunchFiles = networkSites.Select(path => Path.Combine(installDir, path, defaultLaunchFile)).ToArray();

            // check for default paths
            if (potentialLaunchFiles.Any(x => File.Exists(x)))
            {
                return true;
            }

            // check registry for installed programs
            return RegistryUtils.UninstallRegistryContainsDisplayNames(networkSites);
        }
    }
}