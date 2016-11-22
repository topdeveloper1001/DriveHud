//-----------------------------------------------------------------------
// <copyright file="PokerStarsImporter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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

        public override string Site
        {
            get
            {
                return EnumPokerSites.PokerStars.ToString();
            }
        }

        protected override void ImportHand(string handHistory, GameInfo gameInfo)
        {
            gameInfo.UpdateInfo = UpdateGameInfo;

            base.ImportHand(handHistory, gameInfo);
        }

        protected override PlayerList GetPlayerList(HandHistory handHistory)
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

        protected override bool Match(string title, ParsingResult parsingResult)
        {
            if (string.IsNullOrWhiteSpace(title) || parsingResult == null ||
               parsingResult.Source == null || parsingResult.Source.GameDescription == null)
            {
                return false;
            }

            if (parsingResult.Source.GameDescription.IsTournament)
            {
                var tableNumber = parsingResult.Source.TableName.Substring(parsingResult.Source.TableName.LastIndexOf(' ') + 1);

                var tournamentTitle = string.Format(tournamentPattern, parsingResult.Source.GameDescription.Tournament.TournamentId, tableNumber);

                return title.Contains(tournamentTitle);
            }

            return title.Contains(parsingResult.Source.TableName);
        }

        private void UpdateGameInfo(ParsingResult parsingResult, GameInfo gameInfo)
        {
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

            gameInfo.WindowHandle = window.ToInt32();
            gameInfo.TournamentSpeed = ParserUtils.ParseTournamentSpeed(title);
        }
    }
}