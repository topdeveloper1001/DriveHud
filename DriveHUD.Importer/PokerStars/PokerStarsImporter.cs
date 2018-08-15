//-----------------------------------------------------------------------
// <copyright file="PokerStarsImporter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using DriveHUD.Importers.Helpers;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Utils.FastParsing;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.PokerStars
{
    internal class PokerStarsImporter : FileBasedImporter, IPokerStarsImporter
    {
        protected override string ProcessName
        {
            get
            {
                return "PokerStars";
            }
        }

        protected override string HandHistoryFilter
        {
            get
            {
                return "*.txt";
            }
        }

        protected string WindowClassName
        {
            get
            {
                return "PokerStarsTableFrameClass";
            }
        }

        protected override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.PokerStars;
            }
        }

        protected override void ProcessHand(string handHistory, GameInfo gameInfo)
        {
            gameInfo.UpdateAction = UpdateGameInfo;

            base.ProcessHand(handHistory, gameInfo);
        }

        protected override PlayerList GetPlayerList(HandHistory handHistory, GameInfo gameInfo)
        {
            var playerList = handHistory.Players;

            var maxPlayers = handHistory.GameDescription.SeatType.MaxPlayers;

            var heroSeat = handHistory.Hero != null ? handHistory.Hero.SeatNumber : 0;

            if (heroSeat != 0)
            {
                var preferredSeats = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().
                                        SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == EnumPokerSites.PokerStars)?.PrefferedSeats;

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

        private const string tournamentPattern = "{0} Table {1}";

        protected override bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult)
        {
            if (string.IsNullOrWhiteSpace(title) || parsingResult == null ||
               parsingResult.Source == null || parsingResult.Source.GameDescription == null || string.IsNullOrEmpty(parsingResult.Source.TableName) ||
               !WindowClassName.Equals(WinApi.GetClassName(handle), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (parsingResult.Source.GameDescription.IsTournament)
            {
                var tableNumber = parsingResult.Source.TableName.Substring(parsingResult.Source.TableName.LastIndexOf(' ') + 1);

                var tournamentTitle = string.Format(tournamentPattern, parsingResult.Source.GameDescription.Tournament.TournamentId, tableNumber);

                return title.Contains(tournamentTitle);
            }

            var tableName = parsingResult.Source.TableName;

            // check if there is more than 1 opened table
            if (!string.IsNullOrWhiteSpace(parsingResult.FileName))
            {
                var tableNameIndex = parsingResult.FileName.IndexOf($"{tableName} #", StringComparison.OrdinalIgnoreCase);

                if (tableNameIndex >= 0)
                {
                    //HH20180514 Halley #2 - $0.01-$0.02 - USD No Limit Hold'em.txt
                    var tableNameEndIndex = parsingResult.FileName.IndexOf(" ", tableNameIndex + tableName.Length + 2);
                    tableName = parsingResult.FileName.Substring(tableNameIndex, tableNameEndIndex - tableNameIndex).Trim();
                }
            }

            return title.Contains(tableName);
        }

        protected override void PublishImportedResults(DataImportedEventArgs args)
        {
            // we don't publish imported data for zoom
            if (args.GameInfo.GameFormat == GameFormat.Zoom)
            {
                args.DoNotUpdateHud = true;
            }

            base.PublishImportedResults(args);
        }

        private void UpdateGameInfo(IEnumerable<ParsingResult> parsingResults, GameInfo gameInfo)
        {
            var parsingResult = parsingResults?.FirstOrDefault();

            if (parsingResult == null || parsingResult.Source == null || gameInfo == null)
            {
                return;
            }

            var window = FindWindow(parsingResult);

            if (window == IntPtr.Zero)
            {
                return;
            }

            var title = WinApi.GetWindowText(window);

            gameInfo.Session = window.ToInt32().ToString();
            gameInfo.WindowHandle = window.ToInt32();
            gameInfo.TournamentSpeed = ParserUtils.ParseTournamentSpeed(title);
        }
    }
}