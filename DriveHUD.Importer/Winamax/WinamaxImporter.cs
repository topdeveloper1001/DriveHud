//-----------------------------------------------------------------------
// <copyright file="WinamaxImporter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
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
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.Winamax
{
    internal class WinamaxImporter : FileBasedImporter, IWinamaxImporter
    {
        protected override string HandHistoryFilter => "*.txt";

        protected override string ProcessName => "Winamax Poker";

        protected override EnumPokerSites Site => EnumPokerSites.Winamax;

        protected override bool InternalMatch(string title, ParsingResult parsingResult)
        {
            if (string.IsNullOrWhiteSpace(title) || parsingResult == null ||
               parsingResult.Source == null || parsingResult.Source.GameDescription == null || string.IsNullOrEmpty(parsingResult.Source.TableName))
            {
                return false;
            }

            return title.Contains(parsingResult.Source.TableName);
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

                // Winamax supports the following options: top, right, left, lower; so we use 4-max table to configure preferred seating
                var prefferedSeat = preferredSeats.FirstOrDefault(x => x.IsPreferredSeatEnabled && x.TableType == EnumTableType.Four);

                if (prefferedSeat != null && preferredSeatsConvertingMap.ContainsKey(maxPlayers))
                {
                    var shift = (preferredSeatsConvertingMap[maxPlayers][prefferedSeat.PreferredSeat] - heroSeat) % maxPlayers;

                    foreach (var player in playerList)
                    {
                        player.SeatNumber = GeneralHelpers.ShiftPlayerSeat(player.SeatNumber, shift, maxPlayers);
                    }
                }

            }

            return playerList;
        }

        private Dictionary<int, Dictionary<int, int>> preferredSeatsConvertingMap = new Dictionary<int, Dictionary<int, int>>
        {
            [10] = new Dictionary<int, int>
            {
                [1] = 6,
                [2] = 9,
                [3] = 1,
                [4] = 3
            },
            [9] = new Dictionary<int, int>
            {
                [1] = 6,
                [2] = 7,
                [3] = 1,
                [4] = 2
            },
            [8] = new Dictionary<int, int>
            {
                [1] = 5,
                [2] = 7,
                [3] = 1,
                [4] = 3
            },
            [6] = new Dictionary<int, int>
            {
                [1] = 4,
                [2] = 6,
                [3] = 1,
                [4] = 2
            },
            [5] = new Dictionary<int, int>
            {
                [1] = 4,
                [2] = 5,
                [3] = 1,
                [4] = 2
            },
            [4] = new Dictionary<int, int>
            {
                [1] = 3,
                [2] = 4,
                [3] = 1,
                [4] = 2
            },
            [3] = new Dictionary<int, int>
            {
                [1] = 3,
                [2] = 3,
                [3] = 1,
                [4] = 2
            },
            [2] = new Dictionary<int, int>
            {
                [1] = 2,
                [2] = 2,
                [3] = 1,
                [4] = 1
            }
        };
    }
}