using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Settings;
using DriveHUD.Common.Utils;
using DriveHUD.Common.Log;
using Microsoft.Win32;

namespace Model.Site
{
    public class PartyPokerConfiguration : ISiteConfiguration
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

        public EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.PartyPoker;
            }
        }

        private readonly IEnumerable<EnumTableType> tableTypes;

        public IEnumerable<EnumTableType> TableTypes
        {
            get
            {
                return tableTypes;
            }
        }

        public string HeroName
        {
            get;
            set;
        }

        private readonly Dictionary<int, int> prefferedSeat;

        public Dictionary<int, int> PreferredSeats
        {
            get { return prefferedSeat; }
        }

        public TimeSpan TimeZoneOffset
        {
            get;
            set;
        }

        public bool IsHandHistoryLocationRequired
        {
            get { return true; }
        }

        public bool IsPrefferedSeatsAllowed
        {
            get { return true; }
        }

        public bool IsAutoCenterAllowed
        {
            get
            {
                return false;
            }
        }

        public string LogoSource
        {
            get { return "/DriveHUD.Common.Resources;Component/images/SiteLogos/partypoker_logo.png"; }
        }

        public string[] GetHandHistoryFolders()
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

        public ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                IsDetected = RegistryUtils.UninstallRegistryKeysExist(uninstallRegistryKeys),
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
