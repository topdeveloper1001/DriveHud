//-----------------------------------------------------------------------
// <copyright file="PartyPokerConfiguration.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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
using Microsoft.Win32;
using Model.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model.Site
{
    public class PartyPokerConfiguration : BaseSiteConfiguration, ISiteConfiguration
    {
        private static string[] uninstallRegistryKeys = new[] { "PartyPoker", "partypokerNJ" };
        private static string[] pathRegistryKeys = new[]
        {
            @"Software\PartyGaming\Partypoker",
            @"Software\partyNJ\partypokerNJ"

        };

        private const string PathRegistyKeyValue = "AppPath";
        private const string HandHistoryFolderName = "HandHistory";
        private const string FolderNameToExclude = "XMLHandHistory";

        public PartyPokerConfiguration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,
                EnumTableType.Three,
                EnumTableType.Four,
                EnumTableType.Six,
                EnumTableType.Eight,
                EnumTableType.Nine,
                EnumTableType.Ten
            };
        }

        public override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.PartyPoker;
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
                return true;
            }
        }

        public override bool IsPrefferedSeatsAllowed
        {
            get
            {
                return true;
            }
        }

        public override string LogoSource
        {
            get { return "/DriveHUD.Common.Resources;Component/images/SiteLogos/partypoker_logo.png"; }
        }

        public override string[] GetHandHistoryFolders()
        {
            var handHistoryFolders = new List<string>();

            foreach (var key in pathRegistryKeys)
            {
                var pathRegistryKey = Registry.CurrentUser.OpenSubKey(key);
                if (pathRegistryKey != null)
                {
                    var path = pathRegistryKey.GetValue(PathRegistyKeyValue)?.ToString();
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        var hhFolder = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(pathRegistryKey.Name), HandHistoryFolderName);
                        if (Directory.Exists(hhFolder))
                        {
                            var directories = new DirectoryInfo(hhFolder).GetDirectories().Where(x => x.Name != FolderNameToExclude).Select(x => x.FullName);
                            handHistoryFolders.AddRange(directories);
                        }
                    }
                }
            }

            return handHistoryFolders.Distinct().ToArray();
        }

        public override ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                IsDetected = RegistryUtils.UninstallRegistryContainsKeys(uninstallRegistryKeys),
                IsEnabled = siteModel.Enabled,
                HandHistoryLocations = GetHandHistoryFolders().ToList(),
            };

            if (!validationResult.IsDetected)
            {
                return validationResult;
            }

            return validationResult;
        }
    }
}