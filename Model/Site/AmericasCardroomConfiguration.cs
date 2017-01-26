using System;
using System.Collections.Generic;
using System.Linq;
using DriveHUD.Entities;
using System.IO;
using System.Xml;
using DriveHUD.Common.Log;
using Microsoft.Win32;

namespace Model.Site
{
    public class AmericasCardroomConfiguration : ISiteConfiguration
    {
        public AmericasCardroomConfiguration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,
                EnumTableType.Three,
                EnumTableType.Four,
                EnumTableType.Six,
                EnumTableType.Eight,
                EnumTableType.Nine
            };
        }

        public string HeroName
        {
            get;
            set;
        }

        private readonly Dictionary<int, int> prefferedSeat;

        public Dictionary<int, int> PreferredSeats
        {
            get
            {
                return prefferedSeat;
            }
        }

        public virtual EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.AmericasCardroom;
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

        public TimeSpan TimeZoneOffset
        {
            get;
            set;
        }

        public virtual string[] GetHandHistoryFolders()
        {
            var path = GetInstalledPath("Americas Cardroom");

            if (string.IsNullOrEmpty(path))
                return new string[] { };

            return GetHandHistoryFoldersFromProfiles(Path.Combine(path, "profiles")).ToArray();
        }

        protected string GetInstalledPath(string softwareName)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall") ??
                          Registry.LocalMachine.OpenSubKey(
                              @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

                if (key == null)
                    return string.Empty;

                foreach (var subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    if (string.Equals(subkey?.GetValue("DisplayName") as string, softwareName, StringComparison.InvariantCultureIgnoreCase))
                        return subkey?.GetValue("InstallLocation") as string;
                }

            }
            catch (Exception e)
            {
                LogProvider.Log.Error(e);
            }

            return string.Empty;
        }

        protected IEnumerable<string> GetHandHistoryFoldersFromProfiles(string path)
        {
            var result = new List<string>();

            try
            {
                if (!Directory.Exists(path))
                    return result;

                var profilesDirectory = new DirectoryInfo(path);
                foreach (var profileDir in profilesDirectory.GetDirectories())
                {
                    var prefFileName = Path.Combine(profileDir.FullName, "pref.xml");

                    var configuredHandHistoryPath = GetHandHistoryFolderFromPrefs(prefFileName);

                    if (!string.IsNullOrEmpty(configuredHandHistoryPath))
                    {
                        result.Add(configuredHandHistoryPath);
                        continue;
                    }

                    var handHistoryPath = Path.Combine(profileDir.FullName, "HandHistory");

                    if (Directory.Exists(handHistoryPath))
                        result.Add(handHistoryPath);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(e);
            }
            return result;
        }

        protected string GetHandHistoryFolderFromPrefs(string prefFileName)
        {
            try
            {
                if (!File.Exists(prefFileName))
                    return string.Empty;

                var prefFile = new XmlDocument();
                prefFile.Load(prefFileName);

                var node = prefFile.SelectSingleNode("property_set_list");

                var props = node?.SelectSingleNode("property_set")?.SelectNodes("property");

                if (props == null)
                    return string.Empty;

                foreach (XmlNode property in props)
                {
                    if (property.Attributes?.OfType<XmlAttribute>().FirstOrDefault(a => a.Name == "id")?.Value != "3")
                        continue;

                    if (Directory.Exists(property.InnerText))
                        return property.InnerText;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(e);
            }

            return string.Empty;
        }

        public void ValidateSiteConfiguration()
        {
        }
    }
}
