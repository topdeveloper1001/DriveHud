using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Site
{
    public class YaPokerConfiguration : AmericasCardroomConfiguration
    {
        public override EnumPokerSites Site
        {
            get { return EnumPokerSites.YaPoker; }
        }

        public override string[] GetHandHistoryFolders()
        {
            var path = GetInstalledPath("YaPoker");

            if (string.IsNullOrEmpty(path))
                return new string[] { };

            return GetHandHistoryFoldersFromProfiles(Path.Combine(path, "profiles")).ToArray();
        }
    }
}
