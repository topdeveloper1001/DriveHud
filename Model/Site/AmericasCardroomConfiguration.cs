using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Entities;
using Model.Enums;
using System.IO;

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
                EnumTableType.Four,
                EnumTableType.Six,
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

        public EnumPokerSites Site
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

        public string[] GetHandHistoryFolders()
        {
            var folders = new string[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Path.GetPathRoot(Environment.SystemDirectory)
            }.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct();

            var dirs = (from possibleFolder in folders
                        let folder = Path.Combine(possibleFolder, "AmericasCardroom", "profiles")
                        select folder).ToArray();

            return dirs;
        }
    }
}
