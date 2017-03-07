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
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Factory;
using HandHistories.Parser.Utils.FastParsing;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.WinningPokerNetwork
{
    internal abstract class WinningPokerNetworkFileBasedImporter : FileBasedImporter
    {
        private const string GameStartedSearchPattern = "Game started at:";
        private const string HandEndedPattern = "Game ended at: ";

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

            if (!string.IsNullOrEmpty(tournamentNumber))
            {
                var session = capturedFiles.Keys.FirstOrDefault(x => x.Contains(tournamentNumber));

                if (!string.IsNullOrWhiteSpace(session))
                {
                    return capturedFiles[session].Session;
                }
            }

            return base.GetSessionForFile(fileName);
        }

        private string GetTournamentNumberFromFile(string fileName)
        {
            var file = Path.GetFileName(fileName);

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

        protected override PlayerList GetPlayerList(HandHistory handHistory)
        {
            var playerList = handHistory.Players;

            var maxPlayers = handHistory.GameDescription.SeatType.MaxPlayers;

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

        protected override string GetHandTextFromStream(Stream fs, Encoding encoding)
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

            StringBuilder builder = new StringBuilder();

            long lastHandEndedPosition = fs.Position;

            using (var streamReader = new StreamReader(fs, encoding, false, 1024, true))
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

        protected override bool InternalMatch(string title, ParsingResult parsingResult)
        {
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

        private string AddAdditionalData(string handHistory, GameInfo gameInfo)
        {
            if (string.IsNullOrWhiteSpace(handHistory))
            {
                return handHistory;
            }

            var handHistoryParserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();
            var parser = handHistoryParserFactory.GetFullHandHistoryParser(handHistory);

            var indexGameStarted = handHistory.IndexOf(GameStartedSearchPattern);

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
                        TableName = tableName
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

            if (!string.IsNullOrWhiteSpace(tournamentNumber))
            {
                var totalBuyIn = GetTournamentBuyIn(windowTitleText);
                summaryText = $" *** Summary: GameType: {gameType}, TournamentId: {tournamentNumber}, TournamentBuyIn: {totalBuyIn.ToString(CultureInfo.InvariantCulture)}";
            }

            if (string.IsNullOrEmpty(summaryText))
            {
                summaryText = $" *** Summary: GameType: {gameType}";
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

            return "XX";
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

            if (title.IndexOf("Jackpot Poker", StringComparison.OrdinalIgnoreCase) >= 0)
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

                if (endIndex != -1)
                {
                    var buyIn = 0m;

                    if (ParserUtils.TryParseMoney(title.Remove(endIndex), out buyIn))
                    {
                        return buyIn;
                    }
                }
            }

            return 0m;
        }
    }
}
