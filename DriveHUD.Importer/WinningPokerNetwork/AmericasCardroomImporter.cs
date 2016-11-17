using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Parser.Parsers;
using DriveHUD.Common.Progress;
using Microsoft.Practices.ServiceLocation;
using HandHistories.Parser.Parsers.Exceptions;
using DriveHUD.Common.Log;
using HandHistories.Parser.Parsers.Factory;
using DriveHUD.Common.WinApi;
using System.Diagnostics;
using HandHistories.Objects.GameDescription;
using System.Globalization;
using System.IO;
using DriveHUD.Common.Extensions;
using HandHistories.Parser.Utils.FastParsing;

namespace DriveHUD.Importers.WinningPokerNetwork
{
    internal class AmericasCardroomImporter : FileBasedImporter, IAmericasCardroomImporter
    {
        public override string Site
        {
            get { return EnumPokerSites.AmericasCardroom.ToString(); }
        }

        protected override string HandHistoryFilter
        {
            get { return "*.txt"; }
        }

        protected override string ProcessName
        {
            get { return "AmericasCardroom"; }
        }

        protected override Encoding HandHistoryFileEncoding
        {
            get { return Encoding.Unicode; }
        }

        private static readonly NumberFormatInfo NumberFormatInfo = new NumberFormatInfo
        {
            NegativeSign = "-",
            CurrencyDecimalSeparator = ".",
            CurrencyGroupSeparator = ",",
            CurrencySymbol = "$"
        };

        protected override IEnumerable<ParsingResult> ImportHand(string handHistory, GameInfo gameInfo, IFileImporter dbImporter, DHProgress progress)
        {
            // client window contains some additional information about the game, so add it to the HH if possible
            bool isWindowFound;
            handHistory = AddAdditionalData(handHistory, out isWindowFound);

            // import only hands that we can find the open window for (until we find another way of processing tournaments)
            if (isWindowFound)
            {
                // ACP appends the current action to file right after it was performed instead of making the chunk update after the hand had been finished
                return dbImporter.Import(handHistory, progress, gameInfo);
            }

            return null;
        }

        private const string HandEndedPattern = "Game ended at: ";
        protected override string GetHandTextFromStream(Stream fs)
        {
            // possible for ACR, since they remove partial data if table was closed before hand had been finished
            if (fs.Position > fs.Length)
            {
                fs.Seek(0, SeekOrigin.End);
                return string.Empty;
            }

            if (fs.Position == fs.Length)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            long lastHandEndedPosition = fs.Position;

            using (var streamReader = new StreamReader(fs, HandHistoryFileEncoding, false, 1024, true))
            {
                StringBuilder tempStringBuilder = new StringBuilder();

                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    tempStringBuilder.AppendLine(line);

                    if (line.StartsWith(HandEndedPattern))
                    {
                        lastHandEndedPosition = streamReader.GetPosition();

                        builder.Append(tempStringBuilder.ToString());
                        tempStringBuilder.Clear();
                    }
                }

                if (lastHandEndedPosition != fs.Position && lastHandEndedPosition < fs.Length)
                {
                    streamReader.SetPosition(lastHandEndedPosition);
                }
            }

            return builder.ToString();
        }

        protected override bool Match(string title, ParsingResult parsingResult)
        {
            var tableName = parsingResult.Source.TableName;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(tableName))
            {
                return false;
            }

            var isTitleMatch = title.Contains(tableName);

            if (isTitleMatch && parsingResult.GameType != null && parsingResult.GameType.Istourney && !string.IsNullOrWhiteSpace(parsingResult.HandHistory.Tourneynumber))
            {
                return title.Contains(parsingResult.HandHistory.Tourneynumber);
            }

            return isTitleMatch;
        }

        private const string GameStartedSearchPattern = "Game started at:";
        private const string GameIdSearchPatter = "Game ID:";
        private string AddAdditionalData(string handHistory, out bool isWindowFound)
        {
            isWindowFound = false;
            if (string.IsNullOrWhiteSpace(handHistory))
            {
                return handHistory;
            }

            var handHistoryParserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();
            var parser = handHistoryParserFactory.GetFullHandHistoryParser(handHistory);

            var indexGameStarted = handHistory.IndexOf(GameStartedSearchPattern);
            var indexGameId = handHistory.IndexOf(GameIdSearchPatter);

            string windowTitleText = string.Empty;
            ParsingResult parsingResult = null;

            if (indexGameStarted != -1 && indexGameId != -1)
            {
                string tableName = parser.ParseTableName(handHistory.Substring(indexGameStarted));

                parsingResult = new ParsingResult()
                {
                    Source = new HandHistories.Objects.Hand.HandHistory()
                    {
                        TableName = tableName,
                    },
                };

                IntPtr window = FindWindow(parsingResult);
                if (window != IntPtr.Zero)
                {
                    windowTitleText = WinApi.GetWindowText(window);
                    isWindowFound = true;
                }
            }

            if (parsingResult == null || string.IsNullOrEmpty(windowTitleText))
            {
                return handHistory;
            }

            // Get Data From windowTitleText

            string summaryText = string.Empty;
            var gameType = GetGameType(windowTitleText);
            var tournamentNumber = GetTournamentNumber(windowTitleText);

            if (!string.IsNullOrWhiteSpace(tournamentNumber))
            {
                var buyIn = GetTournamentBuyIn(windowTitleText);
                var speed = ParserUtils.ParseTournamentSpeed(windowTitleText);

                summaryText = $" *** Summary: GameType: {gameType}, TournamentId: {tournamentNumber}, TournamentBuyIn: {buyIn}, TournamentSpeed: {speed}";
            }
            else
            {
                summaryText = $" *** Summary: GameType: {gameType}";
            }

            // add summary info to the handhistory file
            while (indexGameStarted != -1)
            {
                var newLineIndex = handHistory.IndexOf(Environment.NewLine, indexGameStarted);
                if (newLineIndex > 0)
                {
                    handHistory = handHistory.Insert(newLineIndex, summaryText);
                }
                else
                {
                    newLineIndex = indexGameStarted + 1;
                }

                indexGameStarted = handHistory.IndexOf(GameStartedSearchPattern, newLineIndex);
            }
#if DEBUG
            LogProvider.Log.Debug(handHistory);
#endif
            return handHistory;
        }

        private string GetGameType(string title)
        {
            if (title.Contains("No Limit"))
            {
                return "NL";
            }

            if (title.Contains("Pot Limit"))
            {
                return "PL";
            }

            if (title.Contains("Fixed"))
            {
                return "FL";
            }

            return "XX";
        }

        private string GetTournamentNumber(string title)
        {
            bool isTournament = title.Contains("Table ") && title.LastOrDefault() == ')';

            if (isTournament)
            {
                var tournamentNumberStartIndex = title.LastIndexOf('(');
                var tournamentNumberEndIndex = title.Length - 2;

                if (tournamentNumberStartIndex == -1)
                {
                    return string.Empty;
                }

                return title.Substring(tournamentNumberStartIndex + 1, tournamentNumberEndIndex - tournamentNumberStartIndex);
            }

            return string.Empty;
        }

        private decimal GetTournamentBuyIn(string title)
        {
            if (string.IsNullOrWhiteSpace(title) || title.Contains("Freeroll"))
            {
                return 0m;
            }

            if (title.Contains("Jackpot Poker"))
            {
                var spaceIndex = title.IndexOf(' ');
                if (spaceIndex != -1)
                {
                    var buyIn = 0m;
                    if (decimal.TryParse(title.Remove(spaceIndex), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, NumberFormatInfo, out buyIn))
                    {
                        return buyIn;
                    }
                }
            }
            else
            {
                var endIndex = title.IndexOf(" -");
                if (endIndex != -1)
                {
                    var buyIn = 0m;
                    if (decimal.TryParse(title.Remove(endIndex), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, NumberFormatInfo, out buyIn))
                    {
                        return buyIn;
                    }
                }
            }

            return 0m;
        }

        private TournamentSpeed GetTournamentSpeed(string title)
        {
            if (title.Contains("Hyper Turbo"))
            {
                return TournamentSpeed.HyperTurbo;
            }

            if (title.Contains("Turbo"))
            {
                return TournamentSpeed.Turbo;
            }

            return TournamentSpeed.Regular;
        }
    }
}
