//-----------------------------------------------------------------------
// <copyright file="WinamaxFastParserImpl.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.FastParsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HandHistories.Parser.Parsers.FastParser.Winamax
{
    /// <summary>
    /// Implements parser for Winamax
    /// </summary>
    internal class WinamaxFastParserImpl : HandHistoryParserFastImpl
    {
        // until DH doesn't support different currencies we will use USD as default
        private const Currency DefaultCurrency = Currency.EURO;

        public override EnumPokerSites SiteName => EnumPokerSites.Winamax;

        public override bool RequiresBetWinAdjustment => true;

        public override bool RequiresUncalledBetCalculations => true;

        /// <summary>
        /// Determines whenever the specified hand history is valid
        /// </summary>
        /// <param name="handLines">Lines of HH to validate</param>
        /// <returns>True if valid; otherwise - false</returns>
        public override bool IsValidHand(string[] handLines)
        {
            var isWinamax = false;
            var hasBlindsPosted = false;
            var hasSummary = false;

            foreach (var line in handLines)
            {
                if (!isWinamax && line.Contains("Winamax Poker"))
                {
                    isWinamax = true;
                }

                if (!hasBlindsPosted && line.Contains("*** ANTE/BLINDS ***"))
                {
                    hasBlindsPosted = true;
                }

                if (!hasSummary && line.Contains("*** SUMMARY ***"))
                {
                    hasSummary = true;
                }
            }

            return isWinamax && hasBlindsPosted && hasSummary;
        }

        /// <summary>
        /// Determines whenever hand history is valid or canceled 
        /// </summary>
        /// <param name="handLines">Lines of HH to validate</param>
        /// <param name="isCancelled">True if canceled; otherwise - false</param>
        /// <returns>True if valid; otherwise - false</returns>
        public override bool IsValidOrCanceledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;
            return IsValidHand(handLines);
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses community cards
        /// </summary>
        /// <param name="handLines">Lines of hand history to parse</param>
        /// <returns>Community cards</returns>
        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            var boardCards = BoardCards.ForPreflop();

            for (var i = handLines.Length - 1; i > 0; i--)
            {
                if (handLines[i].StartsWith("Board: ", StringComparison.OrdinalIgnoreCase))
                {
                    var cardText = handLines[i].TakeBetween("[", "]", true);

                    if (string.IsNullOrEmpty(cardText))
                    {
                        throw new CardException(handLines[i], "Could not parse community cards.");
                    }

                    boardCards.AddCards(BoardCards.FromCards(cardText));
                }
                else if (handLines[i].StartsWith("***", StringComparison.Ordinal))
                {
                    break;
                }
            }

            return boardCards;
        }

        /// <summary>
        /// Parses date (UTC) from hand history lines
        /// </summary>
        /// <param name="handLines">Lines of hand history to parse</param>
        /// <returns>Date and time of hand</returns>
        protected override DateTime ParseDateUtc(string[] handLines)
        {
            var dateText = handLines[0].TakeBetween(" - ", string.Empty, true);

            try
            {
                return ParseDateFromText(dateText);
            }
            catch
            {
                throw new ParseHandDateException(handLines[0], "Date is in wrong format.");
            }
        }

        /// <summary>
        /// Parses dealer position from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Dealer position</returns>
        protected override int ParseDealerPosition(string[] handLines)
        {
            var tableInfoLine = GetTableInfoLine(handLines);

            var dealerText = tableInfoLine.TakeBetween("Seat #", " is the button");

            if (string.IsNullOrEmpty(dealerText) || !int.TryParse(dealerText, out int dealerPosition))
            {
                throw new DealerPositionException(tableInfoLine, "Couldn't not parse dealer position.");
            }

            return dealerPosition;
        }

        /// <summary>
        /// Parses game type from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Game type</returns>
        protected override GameType ParseGameType(string[] handLines)
        {
            var handLine = handLines[0];

            if (handLine.ContainsIgnoreCase("Holdem no limit"))
            {
                return GameType.NoLimitHoldem;
            }

            if (handLine.ContainsIgnoreCase("Holdem pot limit"))
            {
                return GameType.PotLimitHoldem;
            }

            if (handLine.ContainsIgnoreCase("Holdem limit"))
            {
                return GameType.FixedLimitHoldem;
            }

            if (handLine.ContainsIgnoreCase("Omaha H/L no limit"))
            {
                return GameType.NoLimitOmahaHiLo;
            }

            if (handLine.ContainsIgnoreCase("Omaha H/L pot limit"))
            {
                return GameType.PotLimitOmahaHiLo;
            }

            if (handLine.ContainsIgnoreCase("Omaha H/L limit"))
            {
                return GameType.FixedLimitOmahaHiLo;
            }

            if (handLine.ContainsIgnoreCase("Omaha no limit"))
            {
                return GameType.NoLimitOmaha;
            }

            if (handLine.ContainsIgnoreCase("Omaha pot limit"))
            {
                return GameType.PotLimitOmaha;
            }

            if (handLine.ContainsIgnoreCase("Omaha limit"))
            {
                return GameType.FixedLimitOmaha;
            }

            throw new UnrecognizedGameTypeException(handLine, "Unsupported or unrecognized game type.");
        }

        /// <summary>
        /// Parses actions from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <param name="gameType">Game type</param>
        /// <returns>List of <see cref="HandAction"/></returns>
        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            var handActions = new List<HandAction>();

            var street = Street.Preflop;

            var actionNumber = 0;

            var putInThisStreet = new Dictionary<string, decimal>();

            var winsPerPlayer = new Dictionary<string, decimal>();

            void addToThisStreet(string playerName, decimal amount)
            {
                if (!putInThisStreet.ContainsKey(playerName))
                {
                    putInThisStreet.Add(playerName, amount);
                    return;
                }

                putInThisStreet[playerName] += amount;
            }

            for (var i = 2; i < handLines.Length; i++)
            {
                var handLine = handLines[i];

                if (handLine.StartsWith("*** FLOP"))
                {
                    street = Street.Flop;
                    putInThisStreet.Clear();
                }
                else if (handLine.StartsWith("*** TURN"))
                {
                    street = Street.Turn;
                    putInThisStreet.Clear();
                }
                else if (handLine.StartsWith("*** RIVER"))
                {
                    street = Street.River;
                    putInThisStreet.Clear();
                }
                else if (handLine.StartsWith("*** SHOW DOWN"))
                {
                    street = Street.Showdown;
                }
                else if (handLine.StartsWith("*** SUMMARY"))
                {
                    street = Street.Summary;
                }
                else if (handLine.Contains(" posts ante"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(" posts ante")).Trim();

                    var anteText = TryHandleAllIn(handLine, out bool isAllIn).TakeBetween("ante ", string.Empty, true);

                    if (!ParserUtils.TryParseMoney(anteText, out decimal ante))
                    {
                        throw new HandActionException(handLine, "Could not parse ante.");
                    }

                    handActions.Add(BuildAction(playerName, HandActionType.ANTE, street, ante, actionNumber, isAllIn));
                    addToThisStreet(playerName, ante);
                }
                else if (handLine.Contains(" posts small blind"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(" posts small blind")).Trim();

                    var smallBlindText = TryHandleAllIn(handLine, out bool isAllIn).TakeBetween(" blind ", string.Empty, true);

                    if (!ParserUtils.TryParseMoney(smallBlindText, out decimal smallBlind))
                    {
                        throw new HandActionException(handLine, "Could not parse small blind.");
                    }

                    handActions.Add(BuildAction(playerName, HandActionType.SMALL_BLIND, street, smallBlind, actionNumber, isAllIn));
                    addToThisStreet(playerName, smallBlind);
                    continue;
                }
                else if (handLine.Contains(" posts big blind"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(" posts big blind")).Trim();

                    var bigBlindText = TryHandleAllIn(handLine, out bool isAllIn).TakeBetween(" blind ", string.Empty, true);

                    if (!ParserUtils.TryParseMoney(bigBlindText, out decimal bigBlind))
                    {
                        throw new HandActionException(handLine, "Could not parse big blind.");
                    }

                    handActions.Add(BuildAction(playerName, HandActionType.BIG_BLIND, street, bigBlind, actionNumber, isAllIn));
                    addToThisStreet(playerName, bigBlind);
                    continue;
                }
                else if (handLine.EndsWith(" folds", StringComparison.OrdinalIgnoreCase))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(" folds")).Trim();
                    handActions.Add(new HandAction(playerName, HandActionType.FOLD, 0, street, actionNumber++));
                }
                else if (handLine.EndsWith(" checks", StringComparison.OrdinalIgnoreCase))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(" checks")).Trim();
                    handActions.Add(new HandAction(playerName, HandActionType.CHECK, 0, street, actionNumber++));
                }
                else if (handLine.Contains(" bets "))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(" bets ")).Trim();
                    var betText = TryHandleAllIn(handLine, out bool isAllIn).TakeBetween(" bets ", string.Empty);

                    if (!ParserUtils.TryParseMoney(betText, out decimal bet))
                    {
                        throw new HandActionException(handLine, "Could not parse bets.");
                    }

                    handActions.Add(BuildAction(playerName, HandActionType.BET, street, bet, actionNumber, isAllIn));
                    addToThisStreet(playerName, bet);
                    continue;
                }
                else if (handLine.Contains(" calls "))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(" calls ")).Trim();
                    var callText = TryHandleAllIn(handLine, out bool isAllIn).TakeBetween(" calls ", string.Empty);

                    if (!ParserUtils.TryParseMoney(callText, out decimal call))
                    {
                        throw new HandActionException(handLine, "Could not parse calls.");
                    }

                    handActions.Add(BuildAction(playerName, HandActionType.CALL, street, call, actionNumber, isAllIn));
                    addToThisStreet(playerName, call);
                    continue;
                }
                else if (handLine.Contains(" raises "))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(" raises ")).Trim();
                    var raiseText = TryHandleAllIn(handLine, out bool isAllIn).TakeBetween(" to ", string.Empty);

                    if (!ParserUtils.TryParseMoney(raiseText, out decimal raise))
                    {
                        throw new HandActionException(handLine, "Could not parse raise.");
                    }

                    var amount = putInThisStreet.ContainsKey(playerName) ? raise - putInThisStreet[playerName] : raise;
                    handActions.Add(BuildAction(playerName, HandActionType.RAISE, street, amount, actionNumber, isAllIn));
                    addToThisStreet(playerName, amount);
                    continue;
                }
                else if (handLine.Contains(" shows "))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(" shows ")).Trim();
                    handActions.Add(new HandAction(playerName, HandActionType.SHOW, 0, Street.Showdown, actionNumber++));
                }
                else if (handLine.Contains(" collected "))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(" collected ")).Trim();

                    var winText = handLine.TakeBetween(" collected ", " ", true);

                    if (!ParserUtils.TryParseMoney(winText, out decimal win))
                    {
                        throw new HandActionException(handLine, "Could not parse win.");
                    }

                    if (!winsPerPlayer.ContainsKey(playerName))
                    {
                        winsPerPlayer.Add(playerName, win);
                        continue;
                    }

                    winsPerPlayer[playerName] += win;
                }
            }

            foreach (var winPlayer in winsPerPlayer)
            {
                handActions.Add(new WinningsAction(winPlayer.Key, HandActionType.WINS, winPlayer.Value, 0, actionNumber++));
            }

            return handActions;
        }

        /// <summary>
        /// Parses hand if from the specified HH lines 
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Hand id</returns>
        protected override long ParseHandId(string[] handLines)
        {
            var handIdText = handLines[0].TakeBetween("HandId: #", " ");

            if (string.IsNullOrEmpty(handIdText))
            {
                throw new HandIdException(handLines[0], "Hand id not found.");
            }

            handIdText = handIdText.Replace("-", string.Empty);

            if (handIdText.Length > 18)
            {
                handIdText = handIdText.Substring(handIdText.Length - 18, 18);
            }

            if (long.TryParse(handIdText, out long handId))
            {
                return handId;
            }

            throw new HandIdException(handLines[0], "Hand id couldn't be parsed.");
        }

        /// <summary>
        /// Parses Hero from the specified HH lines 
        /// </summary>
        /// <param name="handlines">Lines to parse</param>
        /// <param name="playerList">List of players</param>
        /// <returns>Name of hero</returns>
        protected override string ParseHeroName(string[] handlines, PlayerList playerList)
        {
            for (var i = 2; i < handlines.Length; i++)
            {
                if (handlines[i].StartsWith("Dealt to", StringComparison.OrdinalIgnoreCase))
                {
                    var heroName = handlines[i].TakeBetween("Dealt to ", " [", false, true);

                    var hero = playerList.FirstOrDefault(p => p.PlayerName == heroName);

                    if (hero != null)
                    {
                        var cardsText = handlines[i].TakeBetween("[", "]", true);

                        if (!string.IsNullOrEmpty(cardsText))
                        {
                            hero.HoleCards = HoleCards.FromCards(heroName, cardsText);
                        }
                    }

                    return heroName;
                };

                if (handlines[i].StartsWith("*** PRE-FLOP", StringComparison.Ordinal))
                {
                    break;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Parses game limits from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Limits</returns>
        protected override Limit ParseLimit(string[] handLines)
        {
            var headers = ParseHeader(handLines);

            if (headers.Length < 2)
            {
                throw new LimitException(handLines[0], "Couldn't parse limit.");
            }

            var limitHeader = headers[headers.Length - 2];

            var limitText = limitHeader.TakeBetween("(", ")");

            if (string.IsNullOrEmpty(limitText))
            {
                throw new LimitException(handLines[0], "Limit is in wrong format.");
            }

            var splittedLimitText = limitText.Split('/');

            if (splittedLimitText.Length != 2 && splittedLimitText.Length != 3)
            {
                throw new LimitException(handLines[0], "Limit is in wrong format.");
            }

            var smallBlindIndex = splittedLimitText.Length == 2 ? 0 : 1;
            var bigBlindIndex = splittedLimitText.Length == 2 ? 1 : 2;

            if (!ParserUtils.TryParseMoney(splittedLimitText[smallBlindIndex], out decimal smallBlind))
            {
                throw new LimitException(handLines[0], "Couldn't parse small blind.");
            }

            if (!ParserUtils.TryParseMoney(splittedLimitText[bigBlindIndex], out decimal bigBlind))
            {
                throw new LimitException(handLines[0], "Couldn't parse big blind.");
            }

            var ante = 0m;
            var isAnteTable = false;

            if (splittedLimitText.Length == 3)
            {
                isAnteTable = true;

                if (!ParserUtils.TryParseMoney(splittedLimitText[0], out ante))
                {
                    throw new LimitException(handLines[0], "Couldn't parse ante.");
                }
            }

            return Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, DefaultCurrency, isAnteTable, ante);
        }

        /// <summary>
        /// Parses players from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>List of players</returns>
        protected override PlayerList ParsePlayers(string[] handLines)
        {
            var playerList = new PlayerList();

            for (var i = 2; i < handLines.Length; i++)
            {
                var handLine = handLines[i];

                // Seat 1: FL25(48910)
                if (handLine.StartsWith("Seat ", StringComparison.OrdinalIgnoreCase))
                {
                    var seatNumberText = handLine.TakeBetween("Seat ", ":");

                    if (string.IsNullOrEmpty(seatNumberText) || !int.TryParse(seatNumberText, out int seatNumber))
                    {
                        throw new PlayersException(handLine, "Couldn't parse player seat number.");
                    }

                    var playerName = handLine.TakeBetween(": ", " (", false, true);

                    if (string.IsNullOrEmpty(playerName))
                    {
                        throw new PlayersException(handLine, "Couldn't parse player name.");
                    }

                    // (EUR 5.23)
                    // ($5.23)
                    // (5.23 in chips)
                    var playerStackText = handLine.TakeBetween("(", ")", true);

                    if (!ParserUtils.TryParseMoney(playerStackText, out decimal startingStack))
                    {
                        throw new PlayersException(handLine, "Couldn't parse stack of player.");
                    }

                    var player = new Player(playerName, startingStack, seatNumber);

                    playerList.Add(player);
                }
                else if (handLine.StartsWith("***", StringComparison.Ordinal))
                {
                    break;
                }
            }

            for (var i = handLines.Length - 1; i > 0; i--)
            {
                var handLine = handLines[i];

                if (handLine.StartsWith("*** SHOW", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                var showsIndex = handLine.IndexOf(" shows [", StringComparison.OrdinalIgnoreCase);

                // Call-hymn-he shows [4d 6d] (Two pairs : Aces and 6)
                // billyboy7 shows [Qd 2d] (Two pairs : Aces and Queens)
                if (showsIndex > 0)
                {
                    var playerName = handLine.Substring(0, showsIndex);

                    var player = playerList.FirstOrDefault(p => p.PlayerName == playerName);

                    if (player != null)
                    {
                        var cardsText = handLine.TakeBetween("[", "]", true);

                        if (!string.IsNullOrEmpty(cardsText))
                        {
                            player.HoleCards = HoleCards.FromCards(playerName, cardsText);
                        }
                    }
                }
            }

            return playerList;
        }

        /// <summary>
        /// Parses poker format from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Poker format</returns>
        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            if (handLines[0].Contains("Tournament"))
            {
                return PokerFormat.Tournament;
            }

            return PokerFormat.CashGame;
        }

        /// <summary>
        /// Parses seat types from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Seat type</returns>
        protected override SeatType ParseSeatType(string[] handLines)
        {
            var seatTypeLine = GetTableInfoLine(handLines);

            var seatTypeText = seatTypeLine.TakeBetween("' ", "-max");

            if (!int.TryParse(seatTypeText, out int maxPlayers))
            {
                throw new SeatTypeException(seatTypeLine, "Couldn't parse seat type.");
            }

            return SeatType.FromMaxPlayers(maxPlayers);
        }

        /// <summary>
        /// Parses table name from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Table name</returns>
        protected override string ParseTableName(string[] handLines)
        {
            var tableInfoLine = GetTableInfoLine(handLines);
            var tableName = tableInfoLine.TakeBetween("'", "'");

            if (string.IsNullOrEmpty(tableName))
            {
                throw new TableNameException(tableInfoLine, "Table name couldn't be parsed.");
            }

            return tableName;
        }

        /// <summary>
        /// Parses table type from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Table type</returns>
        protected override TableType ParseTableType(string[] handLines)
        {
            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        /// <summary>
        /// Parses tournament description from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Description of tournament</returns>
        protected override TournamentDescriptor ParseTournament(string[] handLines)
        {
            Buyin buyIn;

            if (handLines[0].Contains("Freeroll"))
            {
                buyIn = Buyin.FromBuyinRake(0, 0, DefaultCurrency);
            }
            else
            {
                var prizePoolText = handLines[0].TakeBetween("buyIn: ", " +");
                var entryFeeText = handLines[0].TakeBetween(" + ", " ");

                if (!ParserUtils.TryParseMoney(prizePoolText, out decimal prizePoolValue))
                {
                    throw new BuyinException(handLines[0], "Could not parse buyin.");
                }

                if (!ParserUtils.TryParseMoney(entryFeeText, out decimal entryFee))
                {
                    throw new BuyinException(handLines[0], "Could not parse entry fee.");
                }

                buyIn = Buyin.FromBuyinRake(prizePoolValue, entryFee, DefaultCurrency);
            }

            var tableInfoLine = GetTableInfoLine(handLines);

            var tournamentName = handLines[0].TakeBetween("Tournament \"", "\"");
            var tournamentId = tableInfoLine.TakeBetween("(", ")");

            var tournamentDescriptor = new TournamentDescriptor
            {
                TournamentId = tournamentId,
                TournamentName = tournamentName,
                BuyIn = buyIn
            };

            return tournamentDescriptor;
        }

        /// <summary>
        /// Parses the hand line to determine whenever hand is summary hand
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>True if hand is summary hand; otherwise - false</returns>
        protected override bool IsSummaryHand(string[] handLines)
        {
            return handLines[0].ContainsIgnoreCase("Tournament summary");
        }

        /// <summary>
        /// Parses summary hand
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <param name="handHistory">Handhistory to update</param>
        /// <returns>Handhistory with summary data</returns>
        protected override HandHistory ParseSummaryHand(string[] handLines, HandHistory handHistory)
        {
            var tournament = new TournamentDescriptor
            {
                Summary = string.Join(Environment.NewLine, handLines)
            };

            var gameType = GameType.Unknown;

            foreach (var handLine in handLines)
            {
                if (handLine.ContainsIgnoreCase("Tournament summary"))
                {
                    var tournamentName = handLine.TakeBetween(" : ", "(");
                    var tournamentId = handLine.TakeBetween("(", ")", true);

                    if (string.IsNullOrEmpty(tournamentId))
                    {
                        throw new TournamentIdException(handLine, "Couldn't parse tournament id");
                    }

                    tournament.TournamentId = tournamentId;
                    tournament.TournamentName = tournamentName;
                }
                else if (handLine.StartsWith("Player :", StringComparison.OrdinalIgnoreCase))
                {
                    var playerName = handLine.TakeBetween(" : ", string.Empty);
                    handHistory.Hero = new Player(playerName, 0, 0);
                }
                else if (handLine.StartsWith("Buy-In :", StringComparison.OrdinalIgnoreCase))
                {
                    var prizePoolText = handLine.TakeBetween(": ", " +");
                    var entryFeeText = handLine.TakeBetween(" + ", string.Empty);

                    if (!ParserUtils.TryParseMoney(prizePoolText, out decimal prizePoolValue))
                    {
                        throw new BuyinException(handLines[0], "Could not parse buyin.");
                    }

                    if (!ParserUtils.TryParseMoney(entryFeeText, out decimal entryFee))
                    {
                        throw new BuyinException(handLines[0], "Could not parse entry fee.");
                    }

                    tournament.BuyIn = Buyin.FromBuyinRake(prizePoolValue, entryFee, DefaultCurrency);
                }
                else if (handLine.StartsWith("Registered players :", StringComparison.OrdinalIgnoreCase))
                {
                    var totalPlayersText = handLine.TakeBetween(" : ", string.Empty);

                    if (int.TryParse(totalPlayersText, out int totalPlayers))
                    {
                        tournament.TotalPlayers = totalPlayers;
                    }
                }
                else if (handLine.StartsWith("Speed : ", StringComparison.OrdinalIgnoreCase))
                {
                    var speedText = handLine.TakeBetween(": ", string.Empty);

                    tournament.Speed = speedText.Equals("turbo", StringComparison.OrdinalIgnoreCase) ?
                        TournamentSpeed.Turbo :
                        TournamentSpeed.Regular;
                }
                else if (handLine.StartsWith("Levels :", StringComparison.OrdinalIgnoreCase))
                {
                    var levelLine = handLine.Replace('-', ' ');

                    try
                    {
                        gameType = ParseGameType(new[] { levelLine });
                    }
                    catch
                    {
                        // do nothing, type will be set in importer
                    }
                }
                else if (handLine.StartsWith("Tournament started", StringComparison.OrdinalIgnoreCase))
                {
                    var dateText = handLine.TakeBetween("started ", string.Empty);

                    try
                    {
                        tournament.StartDate = ParseDateFromText(dateText);
                    }
                    catch
                    {
                        // do nothing, date will be set in importer
                    }
                }
                else if (handLine.StartsWith("You finished in", StringComparison.OrdinalIgnoreCase))
                {
                    var finishedPositionText = handLine
                        .TakeBetween("finished in ", " place")
                        .Replace("th", string.Empty)
                        .Replace("st", string.Empty)
                        .Replace("nd", string.Empty)
                        .Replace("rd", string.Empty);

                    if (int.TryParse(finishedPositionText, out int finishPosition))
                    {
                        tournament.FinishPosition = finishPosition;
                    }
                }
                else if (handLine.StartsWith("You won", StringComparison.OrdinalIgnoreCase))
                {
                    var wonText = handLine.TakeBetween("You won ", string.Empty);

                    if (ParserUtils.TryParseMoney(wonText, out decimal won))
                    {
                        tournament.Winning = won;
                    }
                }
                else if (handLine.StartsWith("Your rebuys", StringComparison.OrdinalIgnoreCase))
                {
                    var rebuysText = handLine.TakeBetween(":", string.Empty);

                    if (ParserUtils.TryParseMoney(rebuysText, out decimal rebuy))
                    {
                        tournament.Rebuy = rebuy;
                    }
                }
                else if (handLine.StartsWith("Your addons", StringComparison.OrdinalIgnoreCase))
                {
                    var addonsText = handLine.TakeBetween(":", string.Empty);

                    if (ParserUtils.TryParseMoney(addonsText, out decimal addon))
                    {
                        tournament.Addon = addon;
                    }
                }
            }

            handHistory.GameDescription = new GameDescriptor(SiteName,
                gameType,
                null,
                TableType.FromTableTypeDescriptions(),
                SeatType.AllSeatType(),
                tournament);

            return handHistory;
        }

        /// <summary>
        /// Parses header into array of sub-headers
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Array of sub-headers</returns>
        private static string[] ParseHeader(string[] handLines)
        {
            return handLines[0].Split(new string[] { " - " }, StringSplitOptions.None);
        }

        /// <summary>
        /// Get line of hand history which contains info about table
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Line with table info</returns>
        private static string GetTableInfoLine(string[] handLines)
        {
            var tableInfoLine = handLines[1];

            const string seatLineStart = "Table:";

            if (!tableInfoLine.StartsWith(seatLineStart, StringComparison.OrdinalIgnoreCase))
            {
                tableInfoLine = null;

                foreach (var handLine in handLines)
                {
                    if (handLine.StartsWith(seatLineStart, StringComparison.OrdinalIgnoreCase))
                    {
                        tableInfoLine = handLine;
                    }
                }

                if (string.IsNullOrEmpty(tableInfoLine))
                {
                    throw new InvalidHandException(handLines[1], "Couldn't find table info line.");
                }
            }

            return tableInfoLine;
        }

        private static string TryHandleAllIn(string handLine, out bool isAllIn)
        {
            const string isAllInText = " and is all-in";

            isAllIn = handLine.Contains(isAllInText);

            if (isAllIn)
            {
                return handLine.Replace(isAllInText, string.Empty);
            }

            return handLine;
        }

        private static HandAction BuildAction(string playerName, HandActionType actionType, Street street, decimal amount, int actionNumber, bool isAllIn = false)
        {
            if (isAllIn)
            {
                return new AllInAction(playerName, amount, street, actionType == HandActionType.BET || actionType == HandActionType.RAISE, actionType, actionNumber);
            }

            return new HandAction(playerName, actionType, amount, street, actionNumber);
        }

        private static DateTime ParseDateFromText(string dateText)
        {
            var splittedDate = dateText.Split(' ');

            if (splittedDate.Length < 2)
            {
                throw new ParseHandDateException(dateText, "Date is in wrong format.");
            }

            var dateParts = splittedDate[0].Split('/');
            var timeParts = splittedDate[1].Split(':');

            var dateTime = new DateTime(FastInt.Parse(dateParts[0]), FastInt.Parse(dateParts[1]), FastInt.Parse(dateParts[2]),
                FastInt.Parse(timeParts[0]), FastInt.Parse(timeParts[1]), FastInt.Parse(timeParts[2]));

            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
    }
}