using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal class PokerConfiguration
    {
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public const string ZeroRoundActionCards = "[cards]";

        public const string CardSeparator = " ";

        public const string UnknownCard = "X";

        public const string DefaultMode = "real";

        public const string DefaultDuration = "N/A";

        public const string DecimalFormat = "0.##";

        public const string DefaultUserName = "BCCDefaultUser";

        public const string TournamentTableTitleTemplate = "{0} (#{1})";

        public const string ZonePokerTableTemplate = "!{0}!";

        public const int DefaultTournamentPlace = 0;

        public const string DefaultBuyIn = "0";

        public const string BuyInFormat = "${0}+${1}";

        public const string TotalBuyInFormat = "${0}";
    }
}