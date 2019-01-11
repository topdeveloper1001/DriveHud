//-----------------------------------------------------------------------
// <copyright file="Pacific888Importer.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using DriveHUD.Importers.Helpers;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.Pacific888
{
    internal class Pacific888Importer : FileBasedImporter, IPacific888Importer
    {
        private readonly static string[] processNames = new[] { "poker" };

        protected override string[] ProcessNames
        {
            get
            {
                return processNames;
            }
        }

        protected override string HandHistoryFilter
        {
            get
            {
                return "*.txt";
            }
        }

        protected override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.Poker888;
            }
        }

        private const string tournamentPattern = "#{0} Table {1}";

        protected override bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult)
        {
            if (string.IsNullOrWhiteSpace(title) || parsingResult == null || !parsingResult.WasImported ||
                parsingResult.Source == null || parsingResult.Source.GameDescription == null)
            {
                return false;
            }

            if (parsingResult.Source.GameDescription.IsTournament)
            {
                var tournamentTitle = string.Format(tournamentPattern, parsingResult.Source.GameDescription.Tournament.TournamentId, parsingResult.Source.TableName);

                return title.Contains(tournamentTitle);
            }

            return title.Contains(parsingResult.Source.TableName);
        }

        protected override PlayerList GetPlayerList(HandHistory handHistory, GameInfo gameInfo)
        {
            var playerList = handHistory.Players;

            var maxPlayers = handHistory.GameDescription.SeatType.MaxPlayers;

            var heroSeat = handHistory.Hero != null ? handHistory.Hero.SeatNumber : 0;

            if (seatConversions.ContainsKey(maxPlayers))
            {
                var newSeats = seatConversions[maxPlayers];

                foreach (var player in playerList)
                {
                    if (newSeats.ContainsKey(player.SeatNumber))
                    {
                        if (player.SeatNumber == heroSeat)
                        {
                            heroSeat = newSeats[player.SeatNumber];
                        }

                        player.SeatNumber = newSeats[player.SeatNumber];
                    }
                }
            }

            if (heroSeat != 0)
            {
                var preferredSeats = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().
                                        SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == EnumPokerSites.Poker888)?.PrefferedSeats;

                var prefferedSeat = preferredSeats?.FirstOrDefault(x => (int)x.TableType == maxPlayers && x.IsPreferredSeatEnabled);

                if (prefferedSeat != null && prefferedSeat.PreferredSeat > 0)
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

        static Dictionary<int, Dictionary<int, int>> seatConversions = new Dictionary<int, Dictionary<int, int>>
        {
            [2] = new Dictionary<int, int>
            {
                [4] = 1,
                [9] = 2
            },
            [3] = new Dictionary<int, int>
            {
                [4] = 1,
                [7] = 2,
                [10] = 3
            },
            [4] = new Dictionary<int, int>
            {
                [1] = 1,
                [4] = 2,
                [7] = 3,
                [9] = 4
            },
            [5] = new Dictionary<int, int>
            {
                [2] = 1,
                [4] = 2,
                [6] = 3,
                [8] = 4,
                [10] = 5
            },
            [6] = new Dictionary<int, int>
            {
                [1] = 1,
                [2] = 2,
                [4] = 3,
                [6] = 4,
                [7] = 5,
                [9] = 6
            },
            [8] = new Dictionary<int, int>
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [5] = 4,
                [6] = 5,
                [7] = 6,
                [8] = 7,
                [10] = 8
            },
            [9] = new Dictionary<int, int>
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 4,
                [5] = 5,
                [6] = 6,
                [7] = 7,
                [9] = 8,
                [10] = 9
            },
        };
    }
}