//-----------------------------------------------------------------------
// <copyright file="BovadaParser.cs" company="Ace Poker Solutions">
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
using HandHistories.Parser.Parsers.Factory;
using System.Linq;

namespace HandHistories.Parser.Parsers
{
    public class BovadaParser : IParser
    {       
        public ParsingResult ParseGame(string text)
        {
            IHandHistoryParserFactory factory = new HandHistoryParserFactoryImpl();
            var handParser = factory.GetFullHandHistoryParser(EnumPokerSites.IPoker);

            var parsedHand = handParser.ParseFullHandHistory(text, true);

            ParsingResult pResult = new ParsingResult();
            Handhistory hh = new Handhistory
            {                
                Gamenumber = parsedHand.HandId,
                GametypeId = (int)parsedHand.GameDescription.GameType,
                Handtimestamp = parsedHand.DateOfHandUtc,
                HandhistoryVal = parsedHand.FullHandHistoryText,
                PokersiteId = (int)EnumPokerSites.Ignition,
            };

            if (parsedHand.GameDescription.Tournament != null)
            {
                hh.Tourneynumber = parsedHand.GameDescription.Tournament.TournamentId;
            }

            Gametypes gameType = new Gametypes
            {
                Anteincents = (int)(parsedHand.GameDescription.Limit.Ante * 100),
                Bigblindincents = (int)(parsedHand.GameDescription.Limit.BigBlind * 100),
                CurrencytypeId = (short)parsedHand.GameDescription.Limit.Currency,
                Istourney = parsedHand.GameDescription.IsTournament,
                PokergametypeId = (short)(parsedHand.GameDescription.GameType),
                Smallblindincents = (int)(parsedHand.GameDescription.Limit.SmallBlind * 100),
                Tablesize = (short)parsedHand.GameDescription.SeatType.MaxPlayers
            };

            var players = parsedHand.Players.Select(player => new Players
            {
                Playername = player.PlayerName,
                PokersiteId = (int)EnumPokerSites.Ignition
            }).ToList();

            pResult.HandHistory = hh;
            pResult.Players = players;
            pResult.GameType = gameType;
            pResult.Source = parsedHand;

            return pResult;
        }        
    }
}