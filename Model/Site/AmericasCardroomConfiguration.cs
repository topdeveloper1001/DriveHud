using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Entities;
using Model.Enums;
using System.IO;
using System.Xml;
using DriveHUD.Common.Linq;
using Microsoft.Win32;
using NHibernate.Util;

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
            var result = new List<string>();

            if (!IsInstalled(@"HKEY_CURRENT_USER\SOFTWARE\Americas Cardroom"))
                return result.ToArray();

            var folders = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Path.GetPathRoot(Environment.SystemDirectory)
            }.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct();

            var dirs = (from possibleFolder in folders
                let folder = Path.Combine(possibleFolder, "AmericasCardroom", "profiles")
                select folder).ToArray();

            foreach (var dir in dirs)
            {
                result.AddRange(GetHandHistoryFoldersFromProfiles(dir));
            }

            return result.ToArray();
        }

        protected bool IsInstalled(string registryKey)
        {
            return (string) Registry.GetValue(registryKey, "hasBeenOpened", string.Empty) == "1";
        }

        protected IEnumerable<string> GetHandHistoryFoldersFromProfiles(string path)
        {
            var result = new List<string>();

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
            return result;
        }

        protected string GetHandHistoryFolderFromPrefs(string prefFileName)
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
            return string.Empty;
        }

        public void ValidateSiteConfiguration()
        {
        }
    }
}
