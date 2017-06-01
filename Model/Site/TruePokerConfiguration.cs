using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Site
{
    public class TruePokerConfiguration : AmericasCardroomConfiguration
    {
        public override EnumPokerSites Site
        {
            get { return EnumPokerSites.TruePoker; }
        }

        public override string[] GetHandHistoryFolders()
        {
            var path = GetInstalledPath("True Poker");

            if (string.IsNullOrEmpty(path))
                return new string[] { };

            return GetHandHistoryFoldersFromProfiles(Path.Combine(path, "profiles")).ToArray();
        }
    }
}
