using System.Linq;
using DriveHUD.Entities;
using System.IO;

namespace Model.Site
{
    public class BlackChipPokerConfiguration : AmericasCardroomConfiguration
    {
        public override EnumPokerSites Site
        {
            get { return EnumPokerSites.BlackChipPoker; }
        }

        public override string[] GetHandHistoryFolders()
        {
            var path = GetInstalledPath("BlackChipPoker");

            if (string.IsNullOrEmpty(path))
                return new string[] {};

            return GetHandHistoryFoldersFromProfiles(Path.Combine(path, "profiles")).ToArray();

        }
    }
}