using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.UnitTests
{
    internal static class DbConfiguration
    {
        public static string CashHandHistoriesTable = "cash_hand_histories";

        public static string CashHandSummaryTable = "cash_hand_summary";

        public static string TourneyHandHistoriesTable = "tourney_hand_histories";

        public static string TourneyHandSummaryTable = "tourney_hand_summary";
    }

    internal class HandHistorySchema
    {
        public string HandHistoriesTable { get; set; }

        public string HandSummaryTable { get; set; }
    }
}
