using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    public class ACRHelper
    {
        public const string TournamentHandHistoryFileTemplate = "HH{0} T{1}-G{2}.txt";

        private static Random random = new Random();

        public static ACRTestData GenerateTournamentTestData()
        {            
            var tournamentId = random.Next(6800000, 9999999);
            var gameId = random.Next(38697295, 99999999);

            var handHistoryFile = string.Format(TournamentHandHistoryFileTemplate, DateTime.Now.ToString("yyyyMMdd"), tournamentId, gameId);

            return new ACRTestData
            {
                TournamentId = tournamentId.ToString(),
                GameId = gameId.ToString(),
                HandHistoryFile = handHistoryFile
            };
        }

        public static int GenerateGameId()
        {
            var gameId = random.Next(816309085, 999999999);
            return gameId;
        }

        public const int ClientWidth = 1016;

        public const int ClientHeight = 759;
    }

    public class ACRTestData
    {
        public string TournamentId { get; set; }

        public string GameId { get; set; }

        public string HandHistoryFile { get; set; }
    }
}
