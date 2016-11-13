﻿//-----------------------------------------------------------------------
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
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using System.Collections.Generic;

namespace DriveHUD.Importers.Pacific888
{
    internal class Pacific888Importer : FileBasedImporter, IPacific888Importer
    {
        protected override string ProcessName
        {
            get
            {
                return "poker";
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
                return EnumPokerSites.Poker888.ToString();
            }
        }

        private const string tournamentPattern = "#{0} Table {1}";

        protected override bool Match(string title, ParsingResult parsingResult)
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

        protected override PlayerList GetPlayerList(HandHistory handHistory)
        {
            var playerList = handHistory.Players;

            var maxPlayers = handHistory.GameDescription.SeatType.MaxPlayers;

            if (seatConversions.ContainsKey(maxPlayers))
            {
                var newSeats = seatConversions[maxPlayers];

                foreach (var player in playerList)
                {
                    if (newSeats.ContainsKey(player.SeatNumber))
                    {
                        player.SeatNumber = newSeats[player.SeatNumber];
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