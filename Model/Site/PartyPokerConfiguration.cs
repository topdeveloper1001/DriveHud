using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Site
{
    public class PartyPokerConfiguration : ISiteConfiguration
    {
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
            get
            {
                return prefferedSeat;
            }
        }

        public TimeSpan TimeZoneOffset
        {
            get;
            set;
        }

        public bool IsHandHistoryLocationRequired
        {
            get
            {
                return true;
            }
        }

        public bool IsPrefferedSeatsAllowed
        {
            get
            {
                return true;
            }
        }

        public string[] GetHandHistoryFolders()
        {
            var myDocumentData = "C:\\Programs";
            string[] possibleFolders = new string[] { "PartyGaming\\PartyPoker", "partyNJ\\partypokerNJ" };

            var dirs = (from possibleFolder in possibleFolders
                        let folder = Path.Combine(myDocumentData, possibleFolder, "HandHistory")
                        select folder).ToArray();

            return dirs;
        }

        public void ValidateSiteConfiguration()
        {
        }
    }
}
