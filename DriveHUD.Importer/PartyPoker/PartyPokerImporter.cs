//-----------------------------------------------------------------------
// <copyright file="PartyPokerImporter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.Importers.Helpers;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DriveHUD.Importers.PartyPoker
{
    internal class PartyPokerImporter : FileBasedImporter, IPartyPokerImporter
    {
        private readonly Dictionary<string, string[]> playerNamesDictionary;

        private readonly static string[] processNames = new[] { "PartyGaming", "PartyEspana", "PartyFrance" };

        public PartyPokerImporter()
        {
            playerNamesDictionary = new Dictionary<string, string[]>();
        }

        protected override string HandHistoryFilter
        {
            get
            {
                return "*.txt";
            }
        }

        protected override string[] ProcessNames
        {
            get
            {
                return processNames;
            }
        }

        protected override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.PartyPoker;
            }
        }

        private const string tournamentPattern = "({0}) Table {1}";

        protected override bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult)
        {
            if (string.IsNullOrWhiteSpace(title) || parsingResult == null ||
              parsingResult.Source == null || parsingResult.Source.GameDescription == null || string.IsNullOrEmpty(parsingResult.Source.TableName))
            {
                return false;
            }

            if (parsingResult.Source.GameDescription.IsTournament)
            {               
                var tableNumber = parsingResult.Source.TableName.Substring(parsingResult.Source.TableName.LastIndexOf(' ') + 1);

                var tournamentTitle = string.Format(tournamentPattern, parsingResult.Source.GameDescription.Tournament.TournamentId, tableNumber);

                return title.Replace(" - ", " ").Contains(tournamentTitle);
            }

            return title.Contains(parsingResult.Source.TableName);
        }

        private Dictionary<string, string> jackpotSngTournamentIdToTableId = new Dictionary<string, string>();

        protected virtual bool IsJackpotMatch(string title, ParsingResult parsingResult)
        {
            var tournamentId = parsingResult.Source.GameDescription?.Tournament?.TournamentId;

            if (string.IsNullOrEmpty(tournamentId))
            {
                return false;
            }

            bool jackpotTitleMatchTableId(string id)
            {
                return title.ContainsIgnoreCase($"Jackpot ({id})");
            }

            if (jackpotSngTournamentIdToTableId.TryGetValue(tournamentId, out string tableId))
            {
                return jackpotTitleMatchTableId(tableId);
            }

            try
            {
                tableId = PartyPokerJackpotTableFinder.FindTableId(tournamentId, parsingResult.FileName);

                if (string.IsNullOrEmpty(tableId))
                {
                    return false;
                }

                jackpotSngTournamentIdToTableId[tournamentId] = tableId;

                return jackpotTitleMatchTableId(tableId);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to get jackpot table id.", e);
            }

            return false;
        }

        protected override PlayerList GetPlayerList(HandHistory handHistory, GameInfo gameInfo)
        {
            var playerList = handHistory.Players;

            var maxPlayers = handHistory.GameDescription.SeatType.MaxPlayers;

            var heroSeat = handHistory.Hero != null ? handHistory.Hero.SeatNumber : 0;

            if (heroSeat != 0)
            {
                var preferredSeats = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().
                                        SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == EnumPokerSites.PartyPoker)?.PrefferedSeats;

                var prefferedSeat = preferredSeats.FirstOrDefault(x => (int)x.TableType == maxPlayers && x.IsPreferredSeatEnabled);

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

        protected override void ProcessHand(string handHistory, GameInfo gameInfo)
        {
            gameInfo.UpdateAction = UpdatePlayerNames;

            base.ProcessHand(handHistory, gameInfo);
        }

        protected override void Clean()
        {
            playerNamesDictionary.Clear();
            jackpotSngTournamentIdToTableId.Clear();

            base.Clean();
        }

        protected override string GetSessionForFile(string fileName)
        {
            //$1.10 Sat Tickets - Centroll. 20 Gtd (141689215) Table #21.txt
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

        private string GetTournamentNumberFromFile(string fileName)
        {
            var file = Path.GetFileName(fileName);

            var tableIndex = file.IndexOf("Table #", StringComparison.OrdinalIgnoreCase);

            if (tableIndex != -1)
            {
                var tournamentStartIndex = file.LastIndexOf("(", tableIndex, StringComparison.Ordinal);
                var tournamentEndIndex = file.LastIndexOf(")", tableIndex, StringComparison.Ordinal);

                if (tournamentEndIndex > tournamentStartIndex)
                {
                    var tornamentNumber = file.Substring(tournamentStartIndex + 1, tournamentEndIndex - tournamentStartIndex - 1);
                    return tornamentNumber;
                }
            }

            return null;
        }

        private void UpdatePlayerNames(IEnumerable<ParsingResult> parsingResults, GameInfo gameInfo)
        {
            parsingResults?.ForEach(x => UpdatePlayerName(x, gameInfo));
        }

        private void UpdatePlayerName(ParsingResult parsingResult, GameInfo gameInfo)
        {
            if (parsingResult == null || parsingResult.Source == null || gameInfo == null ||
                parsingResult.GameType == null || parsingResult.GameType.Istourney || parsingResult.Source.GameDescription == null)
            {
                return;
            }

            try
            {
                var tableSize = parsingResult.Source.GameDescription.SeatType.MaxPlayers;

                var playersToUpdate = parsingResult.Source.Players.Where(x => x.PlayerName != parsingResult.Source.Hero?.PlayerName);

                if (!playersToUpdate.All(x => x.PlayerName == $"Player{x.SeatNumber}"))
                {
                    return;
                }

                if (!playerNamesDictionary.ContainsKey(parsingResult.Source.TableName))
                {
                    playerNamesDictionary.Add(parsingResult.Source.TableName, new string[tableSize]);
                }

                var dictEntry = playerNamesDictionary[parsingResult.Source.TableName];

                for (int i = 0; i < tableSize; i++)
                {
                    if (i >= dictEntry.Length)
                    {
                        break;
                    }

                    var player = playersToUpdate.FirstOrDefault(x => x.SeatNumber == i + 1);

                    if (player == null)
                    {
                        dictEntry[i] = null;
                        continue;
                    }

                    var currentName = dictEntry[i];

                    if (string.IsNullOrWhiteSpace(currentName))
                    {
                        currentName = Utils.GenerateRandomPlayerName(player.SeatNumber);
                        dictEntry[player.SeatNumber - 1] = currentName;
                    }

                    var originalPlayerName = $"Player{player.SeatNumber}";

                    foreach (var action in parsingResult.Source.HandActions)
                    {
                        if (action.PlayerName == originalPlayerName)
                        {
                            action.PlayerName = currentName;
                        }
                    }

                    var dbPlayer = parsingResult.Players.FirstOrDefault(x => x.Playername == originalPlayerName);

                    if (dbPlayer != null)
                    {
                        dbPlayer.Playername = currentName;
                    }

                    player.PlayerName = currentName;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not update player names. [{SiteString}]", e);
            }
        }

        private static bool IsJackpot(string title)
        {
            return title.ContainsIgnoreCase("Sit & Go 3-Handed");
        }
    }
}