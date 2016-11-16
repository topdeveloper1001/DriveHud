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

        // Import hand
        protected override void ImportHand(string handHistory, GameInfo gameInfo, out bool handProcessed)
        {
            handProcessed = true;

            var dbImporter = ServiceLocator.Current.GetInstance<IFileImporter>();
            var progress = new DHProgress();

            IEnumerable<ParsingResult> parsingResult = null;

            try
            {
                // client window contains some additional information about the game, so add it to the HH if possible
                handHistory = AddAdditionalData(handHistory);

                // ACP appends the current action to file right after it was performed instead of making the chunk update after the hand had been finished
                parsingResult = dbImporter.Import(handHistory, progress, gameInfo, true);
            }
            catch (InvalidHandException)
            {
                //hand is not finished yet
                handProcessed = false;
                return;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, string.Format("Hand(s) has not been imported"), e);
            }

            if (parsingResult == null)
            {
                return;
            }

            foreach (var result in parsingResult)
            {
                if (result.HandHistory == null)
                {
                    continue;
                }

                if (result.IsDuplicate)
                {
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported. Duplicate.", result.HandHistory.Gamenumber));
                    continue;
                }

                if (!result.WasImported)
                {
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported.", result.HandHistory.Gamenumber));
                    continue;
                }

                LogProvider.Log.Info(this, string.Format("Hand {0} imported", result.HandHistory.Gamenumber));

                var playerList = GetPlayerList(result.Source);

                gameInfo.WindowHandle = FindWindow(result).ToInt32();
                gameInfo.GameFormat = ParseGameFormat(result);
                gameInfo.GameType = ParseGameType(result);
                gameInfo.TableType = ParseTableType(result);

                var dataImportedArgs = new DataImportedEventArgs(playerList, gameInfo);

                eventAggregator.GetEvent<DataImportedEvent>().Publish(dataImportedArgs);
            }
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
        private string AddAdditionalData(string handHistory)
        {
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
                Debug.WriteLine($"TableName: {tableName}");

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
                var speed = GetTournamentSpeed(windowTitleText);

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

                indexGameStarted = handHistory.IndexOf(GameStartedSearchPattern, newLineIndex );
            }

            LogProvider.Log.Debug(handHistory);

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
            bool isTournament = title.Contains("Table ") && title.Last() == ')';

            if (isTournament)
            {
                var tournamentNumberStartIndex = title.LastIndexOf('(');
                var tournamentNumberEndIndex = title.LastIndexOf(')', tournamentNumberStartIndex);

                return title.Substring(tournamentNumberStartIndex, tournamentNumberEndIndex);
            }

            return string.Empty;
        }

        private decimal GetTournamentBuyIn(string title)
        {
            if (string.IsNullOrWhiteSpace(title) || title.Contains("Freeroll"))
            {
                return 0m;
            }

            var endIndex = title.IndexOf(" -");
            if (endIndex != -1)
            {
                var buyIn = 0m;
                if (decimal.TryParse(title.Remove(endIndex), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, NumberFormatInfo, out buyIn))
                {
                    return buyIn;
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
