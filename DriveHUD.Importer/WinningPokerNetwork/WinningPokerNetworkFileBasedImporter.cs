//-----------------------------------------------------------------------
// <copyright file="WinningPokerNetworkFileBasedImporter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using DriveHUD.Importers.Helpers;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Factory;
using HandHistories.Parser.Utils.FastParsing;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.WinningPokerNetwork
{
    internal abstract class WinningPokerNetworkFileBasedImporter : FileBasedImporter
    {
        private const string PhhFileExtension = ".phh";
        private const string GameStartedSearchPattern = "Game started at:";
        private const string HandEndedPatternV1 = "Game ended at: ";
        private const string HandV2Prefix = "Game Hand #";

        protected override string HandHistoryFilter
        {
            get { return "*.txt"; }
        }

        private static readonly NumberFormatInfo NumberFormatInfo = new NumberFormatInfo
        {
            NegativeSign = "-",
            CurrencyDecimalSeparator = ".",
            CurrencyGroupSeparator = ",",
            CurrencySymbol = "$"
        };

        // WPN creates separate file for each new table, so we need to restore session number for tournaments
        protected override string GetSessionForFile(string fileName)
        {
            //HH20161125 T6748379-G37397187.txt, where T6748379 is tournament number
            var tournamentNumber = GetTournamentNumberFromFile(fileName);

            var capturedFiles = actualCapturedFiles.Values.Concat(notActualCapturedFiles.Values);

            if (!string.IsNullOrEmpty(tournamentNumber))
            {
                var capturedFile = capturedFiles.FirstOrDefault(x => x.ImportedFile.FileName.Contains(tournamentNumber));

                if (capturedFile != null)
                {
                    return capturedFile.Session;
                }
            }

            return base.GetSessionForFile(fileName);
        }

        protected virtual string GetTournamentNumberFromFile(string fileName)
        {
            var file = Path.GetFileName(fileName);

            // jackpot table
            var jackpotPrefixIndex = file.IndexOf("TJPTG", StringComparison.OrdinalIgnoreCase);

            if (jackpotPrefixIndex > 0)
            {
                var jackpotTournamentEndIndex = file.IndexOf("T", jackpotPrefixIndex + 5);

                if (jackpotTournamentEndIndex > jackpotPrefixIndex)
                {
                    var tornamentNumber = file.Substring(jackpotPrefixIndex + 5, jackpotTournamentEndIndex - jackpotPrefixIndex - 5);
                    return tornamentNumber;
                }
            }

            var startIndex = file.IndexOf("T", StringComparison.Ordinal);

            if (startIndex != -1)
            {
                var endIndex = file.IndexOf("-", StringComparison.Ordinal);

                if (endIndex > startIndex)
                {
                    var tornamentNumber = file.Substring(startIndex + 1, endIndex - startIndex - 1);
                    return tornamentNumber;
                }
            }

            return null;
        }

        protected override bool TryGetPokerSiteName(string handText, out EnumPokerSites siteName)
        {
            if (!base.TryGetPokerSiteName(handText, out siteName))
            {
                return false;
            }

            if (siteName == EnumPokerSites.WinningPokerNetwork)
            {
                siteName = this.Site;
            }

            return true;
        }   

        protected override IEnumerable<ParsingResult> ImportHand(string handHistory, GameInfo gameInfo, IFileImporter dbImporter, DHProgress progress)
        {
            // client window contains some additional information about the game, so add it to the HH if possible            
            handHistory = AddAdditionalData(handHistory, gameInfo);

            return dbImporter.Import(handHistory, progress, gameInfo);
        }

        protected override PlayerList GetPlayerList(HandHistory handHistory, GameInfo gameInfo)
        {
            var playerList = handHistory.Players;

            var maxPlayers = (int)gameInfo.TableType > handHistory.GameDescription.SeatType.MaxPlayers ?
                (int)gameInfo.TableType :
                handHistory.GameDescription.SeatType.MaxPlayers;

            var heroSeat = handHistory.Hero != null ? handHistory.Hero.SeatNumber : 0;

            if (heroSeat != 0)
            {
                var preferredSeats = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().
                                        SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == Site)?.PrefferedSeats;

                var prefferedSeat = preferredSeats?.FirstOrDefault(x => (int)x.TableType == maxPlayers && x.IsPreferredSeatEnabled);

                if (prefferedSeat != null)
                {
                    var shift = (prefferedSeat.PreferredSeat - heroSeat) % maxPlayers;

                    foreach (var player in playerList)
                    {
                        player.SeatNumber = GeneralHelpers.ShiftPlayerSeat(player.SeatNumber, shift, maxPlayers);
                    }
                }

            }

            return playerList;
        }

        protected override string GetHandTextFromStream(Stream fs, Encoding encoding, string fileName)
        {
            // possible for WPN, since they remove partial data if table was closed before hand had been finished            
            if (fs.Position > fs.Length)
            {
                fs.Seek(0, SeekOrigin.End);
                return string.Empty;
            }

            if (fs.Position == fs.Length)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            long lastHandEndedPosition = fs.Position;

            var wpnFormat = WPNFormat.V1;

            using (var streamReader = new StreamReader(fs, encoding, false, 4096, true))
            {
                var tempStringBuilder = new StringBuilder();

                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    tempStringBuilder.AppendLine(line);

                    if (line.StartsWith(HandV2Prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        wpnFormat = WPNFormat.V2;
                    }

                    if ((wpnFormat == WPNFormat.V1 && line.StartsWith(HandEndedPatternV1)) ||
                        (wpnFormat == WPNFormat.V2 && line == string.Empty))
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

        protected override bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult)
        {
            if (parsingResult != null && parsingResult.Source != null &&
                parsingResult.Source.FullHandHistoryText != null &&
                parsingResult.Source.FullHandHistoryText.StartsWith(HandV2Prefix, StringComparison.OrdinalIgnoreCase))
            {
                return InternalMatchV2(title, parsingResult);
            }

            var tableName = parsingResult.Source.TableName;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(tableName))
            {
                return false;
            }

            var tableNameStartIndex = title.IndexOf(tableName);

            if (tableNameStartIndex < 0)
            {
                return false;
            }

            var isTitleMatch = GetTableName(title).Substring(tableNameStartIndex).Trim().Equals(tableName.Trim());

            if (isTitleMatch && parsingResult.GameType != null && parsingResult.GameType.Istourney && !string.IsNullOrWhiteSpace(parsingResult.HandHistory.Tourneynumber))
            {
                return title.Contains(parsingResult.HandHistory.Tourneynumber);
            }

            return isTitleMatch;
        }

        protected virtual bool InternalMatchV2(string title, ParsingResult parsingResult)
        {
            // $2 Jackpot Holdem No Limit Hold'em - 10/20 (8952347)
            return parsingResult.GameType != null && parsingResult.GameType.Istourney &&
                parsingResult.HandHistory != null && title.Contains($" ({parsingResult.HandHistory.Tourneynumber})");
        }

        protected override EnumTableType ParseTableType(ParsingResult parsingResult, GameInfo gameInfo)
        {
            if (!TryParseTableTypeFromFile(gameInfo.FullFileName, out EnumTableType tableType))
            {
                tableType = base.ParseTableType(parsingResult, gameInfo);
            }

            if (gameInfo.TableType > tableType)
            {
                return gameInfo.TableType;
            }

            return tableType;
        }

        private Dictionary<string, EnumTableType> handHistoryFileTableType = new Dictionary<string, EnumTableType>();

        protected bool TryParseTableTypeFromFile(string fileName, out EnumTableType tableType)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                tableType = EnumTableType.Nine;
                return false;
            }

            if (handHistoryFileTableType.ContainsKey(fileName))
            {
                tableType = handHistoryFileTableType[fileName];
                return true;
            }

            tableType = EnumTableType.Nine;

            try
            {
                var phhFileName = Path.ChangeExtension(fileName, PhhFileExtension);

                if (!File.Exists(phhFileName))
                {
                    LogProvider.Log.Error(this, $"Could not find '{phhFileName}'.");
                    return false;
                }

                string phhFileLine = null;

                using (var fs = new FileStream(phhFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        if (!sr.EndOfStream)
                        {
                            phhFileLine = sr.ReadLine();
                        }
                    }
                }

                if (string.IsNullOrEmpty(phhFileLine))
                {
                    LogProvider.Log.Error(this, $".phh file '{phhFileName}' is empty.");
                    return false;
                }

                var splittedPhhFileLine = phhFileLine.Split('~');

                if (splittedPhhFileLine.Length < 14)
                {
                    LogProvider.Log.Error(this, $".phh file '{phhFileName}' data is incorrect.");
                    return false;
                }

                var tableTypeText = splittedPhhFileLine[14];

                if (!Enum.TryParse(tableTypeText, out tableType))
                {
                    LogProvider.Log.Error(this, $".phh file '{phhFileName}' data '{tableTypeText}' couldn't be parsed.");
                    return false;
                }

                handHistoryFileTableType.Add(fileName, tableType);

                return true;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not parse data from .phh file of '{fileName}'.", e);
            }

            return false;
        }

        protected override void Clean()
        {
            base.Clean();
            handHistoryFileTableType.Clear();
            sng2HanhHistoryPrefixes.Clear();
        }

        protected readonly ConcurrentDictionary<string, string> sng2HanhHistoryPrefixes = new ConcurrentDictionary<string, string>();

        private string AddAdditionalDataSNG2(string handHistory, string windowTitle, string tournamentNumber)
        {
            if (string.IsNullOrEmpty(tournamentNumber))
            {
                return handHistory;
            }

            try
            {
                if (!sng2HanhHistoryPrefixes.TryGetValue(tournamentNumber, out string header))
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("<Game Information>");

                    var title = windowTitle.Substring(0, windowTitle.IndexOf(" -"));

                    sb.AppendLine($"Title: {title}");
                    sb.AppendLine($"Entry #: {tournamentNumber}");
                    sb.AppendLine($"Begin Time: {DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss")} UTC");
                    sb.AppendLine($"Buyin to Payout: Real To Real");
                    sb.AppendLine($"Play Type: Sit & Go");

                    var isOmaha = windowTitle.ContainsIgnoreCase("Omaha");
                    var gameType = isOmaha ? "Omaha" : "Hold’em";
                    var betLimitType = isOmaha ? "Pot Limit" : "No Limit";

                    sb.AppendLine($"Game Type: {gameType}");
                    sb.AppendLine($"Bet Limit Type: {betLimitType}");
                    sb.AppendLine();
                    sb.AppendLine("<Hand History>");

                    header = sb.ToString();

                    sng2HanhHistoryPrefixes.AddOrUpdate(tournamentNumber, header, (key, prev) => header);
                }

                handHistory = $"{header}{handHistory}";

                return handHistory;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Failed to add additional data to SNG2 format hand", e);
            }

            return handHistory;
        }

        private string AddAdditionalData(string handHistory, GameInfo gameInfo)
        {
            if (string.IsNullOrWhiteSpace(handHistory))
            {
                return handHistory;
            }

            var handHistoryParserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();
            var parser = handHistoryParserFactory.GetFullHandHistoryParser(handHistory);

            var indexGameStarted = handHistory.IndexOf(GameStartedSearchPattern);

            var isSNG2 = false;

            if (indexGameStarted < 0)
            {
                indexGameStarted = handHistory.IndexOf(HandV2Prefix);

                isSNG2 = indexGameStarted >= 0;
            }

            string windowTitleText = string.Empty;

            ParsingResult parsingResult = null;

            string tournamentNumber = null;

            if (indexGameStarted != -1)
            {
                var tableName = parser.ParseTableName(handHistory.Substring(indexGameStarted));
                tournamentNumber = GetTournamentNumberFromFile(gameInfo.FileName);

                parsingResult = new ParsingResult
                {
                    Source = new HandHistory
                    {
                        TableName = tableName,
                        FullHandHistoryText = handHistory
                    }
                };

                // set tournament number to find proper window
                if (!string.IsNullOrWhiteSpace(tournamentNumber))
                {
                    parsingResult.GameType = new Gametypes
                    {
                        Istourney = true
                    };

                    parsingResult.HandHistory = new Handhistory
                    {
                        Tourneynumber = tournamentNumber
                    };
                }

                var window = FindWindow(parsingResult);

                if (window != IntPtr.Zero)
                {
                    windowTitleText = WinApi.GetWindowText(window);
                    gameInfo.WindowHandle = window.ToInt32();
                }
            }

            if (parsingResult == null || string.IsNullOrEmpty(windowTitleText))
            {
                return handHistory;
            }

            // Get Data From windowTitleText
            var summaryText = string.Empty;
            var gameType = GetGameType(windowTitleText);

            if (string.IsNullOrWhiteSpace(tournamentNumber))
            {
                tournamentNumber = GetTournamentNumber(windowTitleText);
            }

            // add header for sng2 hand
            if (isSNG2)
            {
                return AddAdditionalDataSNG2(handHistory, windowTitleText, tournamentNumber);
            }

            if (!string.IsNullOrWhiteSpace(tournamentNumber))
            {
                var totalBuyIn = GetTournamentBuyIn(windowTitleText);
                summaryText = $" *** Summary: GameType: {gameType}, TournamentId: {tournamentNumber}, TournamentBuyIn: {totalBuyIn.ToString(CultureInfo.InvariantCulture)}";
            }

            if (string.IsNullOrEmpty(summaryText))
            {
                summaryText = $" *** Summary: GameType: {gameType}";
            }

            if (TryParseTableTypeFromFile(gameInfo.FullFileName, out EnumTableType tableType))
            {
                summaryText = $"{summaryText}, TableType: {(int)tableType}";
            }

            // add summary info to the hand history file
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

            return "NL";
        }

        private string GetTableName(string title)
        {
            var gameTypeIndex = title.LastIndexOf("No Limit");

            if (gameTypeIndex == -1)
            {
                gameTypeIndex = title.LastIndexOf("Pot Limit");
            }

            if (gameTypeIndex == -1)
            {
                gameTypeIndex = title.LastIndexOf("Fixed");
            }

            if (gameTypeIndex == -1)
            {
                return title;
            }

            var tableNameEndIndex = title.Substring(0, gameTypeIndex).LastIndexOf('-');

            if (tableNameEndIndex == -1)
            {
                return title;
            }

            return title.Substring(0, tableNameEndIndex);
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
            if (string.IsNullOrWhiteSpace(title) || title.IndexOf("Freeroll", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return 0m;
            }

            if (title.IndexOf(" Jackpot ", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var spaceIndex = title.IndexOf(' ');

                if (spaceIndex != -1)
                {
                    var buyIn = 0m;

                    if (ParserUtils.TryParseMoney(title.Remove(spaceIndex), out buyIn))
                    {
                        return buyIn;
                    }
                }
            }
            else
            {
                var endIndex = title.IndexOf(" -", StringComparison.OrdinalIgnoreCase);

                if (endIndex != -1 && ParserUtils.TryParseMoney(title.Remove(endIndex), out decimal buyIn))
                {
                    return buyIn;
                }
            }

            return 0m;
        }
    
        private enum WPNFormat
        {
            V1,
            V2
        }
    }
}