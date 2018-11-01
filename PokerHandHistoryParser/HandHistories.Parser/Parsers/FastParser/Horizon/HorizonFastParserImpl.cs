//-----------------------------------------------------------------------
// <copyright file="HorizonFastParserImpl.cs" company="Ace Poker Solutions">
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
using System.Globalization;
using System.Linq;

namespace HandHistories.Parser.Parsers.FastParser.Horizon
{
    /// <summary>
    /// Implements parser for Horizon Gaming Network (InterTops Poker, Juicy Stakes, BetUS.com, BrucePoker, ChocolatePoker, ColtPoker) 
    /// </summary>
    internal class HorizonFastParserImpl : HandHistoryParserFastImpl
    {
        // until DH doesn't support different currencies we will use USD as default
        private const Currency DefaultCurrency = Currency.USD;

        public override EnumPokerSites SiteName => EnumPokerSites.Horizon;

        public override bool RequiresSeatTypeAdjustment => true;

        public override bool RequiresBetWinAdjustment => true;

        /// <summary>
        /// Determines whenever the specified hand history is valid
        /// </summary>
        /// <param name="handLines">Lines of HH to validate</param>
        /// <returns>True if valid; otherwise - false</returns>
        public override bool IsValidHand(string[] handLines)
        {
            return handLines != null && handLines.Length > 2;
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

        /// <summary>
        /// Parses description of game from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Game description</returns>
        protected override GameDescriptor ParseGameDescriptor(string[] handLines)
        {
            var gameDescriptor = base.ParseGameDescriptor(handLines);

            if (gameDescriptor.GameType == GameType.FixedLimitHoldem ||
                gameDescriptor.GameType == GameType.FixedLimitOmaha ||
                gameDescriptor.GameType == GameType.FixedLimitOmahaHiLo)
            {
                gameDescriptor.Limit.SmallBlind /= 2m;
                gameDescriptor.Limit.BigBlind /= 2m;
            }

            return gameDescriptor;
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

            foreach (var handLine in handLines)
            {
                if (handLine.StartsWith("*** FLOP *** ", StringComparison.OrdinalIgnoreCase) ||
                    handLine.StartsWith("*** TURN *** ", StringComparison.OrdinalIgnoreCase) ||
                    handLine.StartsWith("*** RIVER *** ", StringComparison.OrdinalIgnoreCase))
                {
                    var cardText = handLine.TakeBetween("[", "]", true);

                    if (string.IsNullOrEmpty(cardText))
                    {
                        throw new CardException(handLine, "Could not parse community cards.");
                    }

                    boardCards.AddCards(BoardCards.FromCards(cardText));
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
            // possible formats:
            // 2018/04/18 - 15:53:10
            // 2018-04-18 - 15-53-10
            // 2018.04.18 - 15/53/10

            var dateIndex = handLines[0].LastIndexOf("-- ", StringComparison.OrdinalIgnoreCase);

            if (dateIndex < 0)
            {
                throw new ParseHandDateException(handLines[0], "Date couldn't be parsed.");
            }

            var dateString = handLines[0].Substring(dateIndex + 3).Trim();

            var splittedDate = dateString.Split(new[] { " - " }, StringSplitOptions.None);

            if (splittedDate.Length != 2)
            {
                throw new ParseHandDateException(handLines[0], "Date has wrong format.");
            }

            string[] dateParts;

            if (splittedDate[0].Contains("/"))
            {
                dateParts = splittedDate[0].Split('/');
            }
            else if (splittedDate[0].Contains("-"))
            {
                dateParts = splittedDate[0].Split('-');
            }
            else if (splittedDate[0].Contains("."))
            {
                dateParts = splittedDate[0].Split('.');
            }
            else
            {
                throw new ParseHandDateException(handLines[0], "Date part is in wrong format.");
            }

            string[] timeParts;

            if (splittedDate[1].Contains(":"))
            {
                timeParts = splittedDate[1].Split(':');
            }
            else if (splittedDate[1].Contains("-"))
            {
                timeParts = splittedDate[1].Split('-');
            }
            else if (splittedDate[1].Contains("/"))
            {
                timeParts = splittedDate[1].Split('/');
            }
            else
            {
                throw new ParseHandDateException(handLines[0], "Time part is in wrong format.");
            }

            var dateTime = new DateTime(FastInt.Parse(dateParts[0]), FastInt.Parse(dateParts[1]), FastInt.Parse(dateParts[2]),
                FastInt.Parse(timeParts[0]), FastInt.Parse(timeParts[1]), FastInt.Parse(timeParts[2]));

            var dateTimeUtc = TimeZoneInfo.ConvertTimeToUtc(dateTime);

            return DateTime.SpecifyKind(dateTimeUtc, DateTimeKind.Utc);
        }

        /// <summary>
        /// Parses dealer position from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Dealer position</returns>
        protected override int ParseDealerPosition(string[] handLines)
        {
            var handLine = handLines[1];

            var dealerPositionText = handLine.Substring(handLine.LastIndexOf('t') + 2);

            if (!int.TryParse(dealerPositionText, out int dealerPosition))
            {
                throw new DealerPositionException(handLine, "Couldn't not parse dealer position.");
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

            if (handLine.Contains("NL Hold'em"))
            {
                return GameType.NoLimitHoldem;
            }

            if (handLine.Contains("PL Hold'em"))
            {
                return GameType.PotLimitHoldem;
            }

            if (handLine.Contains("Hold'em"))
            {
                return GameType.FixedLimitHoldem;
            }

            if (handLine.Contains("NL Omaha H/L") || handLine.Contains("NL OmahaHiLo"))
            {
                return GameType.NoLimitOmahaHiLo;
            }

            if (handLine.Contains("PL Omaha H/L") || handLine.Contains("PL OmahaHiLo"))
            {
                return GameType.PotLimitOmahaHiLo;
            }

            if (handLine.Contains("Omaha H/L") || handLine.Contains("OmahaHiLo"))
            {
                return GameType.FixedLimitOmahaHiLo;
            }

            if (handLine.Contains("NL Omaha"))
            {
                return GameType.NoLimitOmaha;
            }

            if (handLine.Contains("PL Omaha"))
            {
                return GameType.PotLimitOmaha;
            }

            if (handLine.Contains("Omaha"))
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
                    continue;
                }

                if (handLine.StartsWith("*** TURN"))
                {
                    street = Street.Turn;
                    putInThisStreet.Clear();
                    continue;
                }

                if (handLine.StartsWith("*** RIVER"))
                {
                    street = Street.River;
                    putInThisStreet.Clear();
                    continue;
                }

                if (handLine.StartsWith("***SHOW DOWN"))
                {
                    street = Street.Showdown;
                    continue;
                }

                if (handLine.Contains(": posts ante"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": posts ante")).Trim();
                    var anteText = handLine.TakeBetween("ante of ", string.Empty, true);

                    if (!ParserUtils.TryParseMoney(anteText, out decimal ante))
                    {
                        throw new HandActionException(handLine, "Could not parse ante.");
                    }

                    handActions.Add(new HandAction(playerName, HandActionType.ANTE, ante, street, actionNumber++));
                    addToThisStreet(playerName, ante);
                }

                if (handLine.Contains(": posts small blind"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": posts ")).Trim();
                    var smallBlindText = handLine.TakeBetween(" blind ", string.Empty, true);

                    if (!ParserUtils.TryParseMoney(smallBlindText, out decimal smallBlind))
                    {
                        throw new HandActionException(handLine, "Could not parse small blind.");
                    }

                    handActions.Add(new HandAction(playerName, HandActionType.SMALL_BLIND, smallBlind, street, actionNumber++));
                    addToThisStreet(playerName, smallBlind);
                }

                if (handLine.Contains(": posts big blind"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": posts ")).Trim();
                    var bigBlindText = handLine.TakeBetween(" blind ", string.Empty, true);

                    if (!ParserUtils.TryParseMoney(bigBlindText, out decimal bigBlind))
                    {
                        throw new HandActionException(handLine, "Could not parse big blind.");
                    }

                    handActions.Add(new HandAction(playerName, HandActionType.BIG_BLIND, bigBlind, street, actionNumber++));
                    addToThisStreet(playerName, bigBlind);
                }

                if (handLine.Contains(":posts dead blind "))
                {
                    var start = handLine.IndexOf(":posts ");

                    var playerName = handLine.Substring(0, start).Trim();
                    var deadBlindTexts = handLine.Substring(start)
                        .Replace(":posts dead blind ", string.Empty)
                        .Replace("and big blind ", string.Empty)
                        .Split(' ');

                    if (!ParserUtils.TryParseMoney(deadBlindTexts[0], out decimal deadBlind))
                    {
                        throw new HandActionException(handLine, "Could not parse dead blind.");
                    }

                    handActions.Add(new HandAction(playerName, HandActionType.POSTS, deadBlind, street, actionNumber++));
                    addToThisStreet(playerName, deadBlind);

                    if (deadBlindTexts.Length > 1)
                    {
                        if (!ParserUtils.TryParseMoney(deadBlindTexts[1], out decimal bigBlind))
                        {
                            throw new HandActionException(handLine, "Could not parse dead and big blind.");
                        }

                        handActions.Add(new HandAction(playerName, HandActionType.BIG_BLIND, bigBlind, street, actionNumber++));
                        addToThisStreet(playerName, bigBlind);
                    }
                }

                if (handLine.EndsWith(": folds", StringComparison.OrdinalIgnoreCase))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": folds")).Trim();
                    handActions.Add(new HandAction(playerName, HandActionType.FOLD, 0, street, actionNumber++));
                }

                if (handLine.EndsWith(": checks", StringComparison.OrdinalIgnoreCase))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": checks")).Trim();
                    handActions.Add(new HandAction(playerName, HandActionType.CHECK, 0, street, actionNumber++));
                }

                if (handLine.Contains(": bets"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": bets")).Trim();
                    var betText = handLine.TakeBetween(": bets ", string.Empty);

                    if (!ParserUtils.TryParseMoney(betText, out decimal bet))
                    {
                        throw new HandActionException(handLine, "Could not parse bets.");
                    }

                    handActions.Add(new HandAction(playerName, HandActionType.BET, bet, street, actionNumber++));
                    addToThisStreet(playerName, bet);
                }

                if (handLine.Contains(": calls"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": calls")).Trim();
                    var callText = handLine.TakeBetween(": calls ", string.Empty);

                    if (!ParserUtils.TryParseMoney(callText, out decimal call))
                    {
                        throw new HandActionException(handLine, "Could not parse calls.");
                    }

                    handActions.Add(new HandAction(playerName, HandActionType.CALL, call, street, actionNumber++));
                    addToThisStreet(playerName, call);
                }

                if (handLine.Contains(": raises to"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": raises to")).Trim();
                    var raiseText = handLine.TakeBetween(": raises to ", string.Empty);

                    if (!ParserUtils.TryParseMoney(raiseText, out decimal raise))
                    {
                        throw new HandActionException(handLine, "Could not parse raise.");
                    }

                    var amount = putInThisStreet.ContainsKey(playerName) ? raise - putInThisStreet[playerName] : raise;
                    handActions.Add(new HandAction(playerName, HandActionType.RAISE, amount, street, actionNumber++));
                    addToThisStreet(playerName, amount);
                }

                if (handLine.Contains(": is all in"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": is all in")).Trim();
                    var allinText = handLine.TakeBetween(": is all in ", string.Empty);

                    if (!ParserUtils.TryParseMoney(allinText, out decimal allIn))
                    {
                        throw new HandActionException(handLine, "Could not parse all-in.");
                    }

                    var mostPutInPot = putInThisStreet.MaxOrDefault(x => x.Value);

                    var playerPutInThisStreet = putInThisStreet.ContainsKey(playerName) ? putInThisStreet[playerName] : 0;

                    // bet
                    if (mostPutInPot == 0)
                    {
                        handActions.Add(new AllInAction(playerName, allIn, street, true, HandActionType.BET, actionNumber++));
                    }
                    else if (mostPutInPot >= allIn + playerPutInThisStreet)
                    {
                        handActions.Add(new AllInAction(playerName, allIn, street, false, HandActionType.CALL, actionNumber++));
                    }
                    else
                    {
                        handActions.Add(new AllInAction(playerName, allIn, street, true, HandActionType.RAISE, actionNumber++));
                    }

                    addToThisStreet(playerName, allIn);
                }

                if (handLine.Contains(": returns uncalled"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": returns uncalled")).Trim();
                    var uncalledBetText = handLine.TakeBetween(" uncalled bet ", string.Empty);

                    if (!ParserUtils.TryParseMoney(uncalledBetText, out decimal uncalledBet))
                    {
                        throw new HandActionException(handLine, "Could not parse uncalled bet.");
                    }

                    handActions.Add(new HandAction(playerName, HandActionType.UNCALLED_BET, uncalledBet, street, actionNumber++));
                }

                if (handLine.Contains(": shows "))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": shows ")).Trim();
                    handActions.Add(new HandAction(playerName, HandActionType.SHOW, 0, Street.Showdown, actionNumber++));
                }

                if (handLine.Contains(": mucks"))
                {
                    var playerName = handLine.Substring(0, handLine.IndexOf(": mucks")).Trim();
                    handActions.Add(new HandAction(playerName, HandActionType.MUCKS, 0, Street.Showdown, actionNumber++));
                }

                var winsMarkers = new[] { " wins high pot ", " wins low pot ", " wins " };

                foreach (var winsMarker in winsMarkers)
                {
                    if (handLine.Contains(winsMarker))
                    {
                        var playerName = handLine.Substring(0, handLine.LastIndexOf(winsMarker)).Trim();
                        var winText = (handLine + " ").TakeBetween(winsMarker, " ", true);

                        if (!ParserUtils.TryParseMoney(winText, out decimal win))
                        {
                            throw new HandActionException(handLine, $"Could not parse {winsMarker.Trim()}.");
                        }

                        handActions.Add(new WinningsAction(playerName, HandActionType.WINS, win, 0, actionNumber++));

                        break;
                    }
                }
            }

            // merge wins
            var winsActionsToMerge = handActions.Where(x => x.IsWinningsAction)
                .GroupBy(x => x.PlayerName)
                .Where(x => x.Count() > 1)
                .ToArray();

            foreach (var winsActions in winsActionsToMerge)
            {
                var firstWinAction = winsActions.First();

                foreach (var winAction in winsActions.Skip(1))
                {
                    firstWinAction.Amount += winAction.Amount;
                    handActions.Remove(winAction);
                }
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
            // HandId: 5126023113000128 = 5126 (day since start) + 023113 (table id) + 128 (incremental number)
            // HandId: 5127FD7A47000373 = 5127 (day since start) + FD7A47 (tournament id) + 373 (incremental number)

            var handLine = handLines[0];
            var handIdTextEnd = handLine.IndexOf(' ');

            var handIdText = handLines[0].Substring(5, handIdTextEnd - 5);

            if (IsTournament(handLines))
            {
                var prefixText = handIdText.Substring(0, 4);
                var tableIdText = handIdText.Substring(4, 6);
                var handNumber = handIdText.Substring(10);

                if (!long.TryParse(tableIdText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long tableId))
                {
                    throw new HandIdException(handLine, "Hand id couldn't be parsed. Table id part is in wrong format.");
                }

                handIdText = $"{prefixText}{tableId}{handNumber}";
            }

            if (long.TryParse(handIdText, out long handId))
            {
                return handId;
            }

            throw new HandIdException(handLine, "Hand id couldn't be parsed.");
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

                if (handlines[i].StartsWith("***", StringComparison.Ordinal))
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
            // Hand#5126023113000127 - No Limit Hold'em 10/20 23113 -- 10/20 NL Hold'em -- 2018/04/18 - 15:53:10
            var splittedLine = handLines[0].Split(new string[] { " -- " }, StringSplitOptions.None);

            if (splittedLine.Length < 3)
            {
                throw new LimitException(handLines[0], "Couldn't parse limit.");
            }

            var limitText = splittedLine[splittedLine.Length - 2];
            limitText = limitText.Substring(0, limitText.IndexOf(' '));

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

            return Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, DefaultCurrency);
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

                // Seat 1: Swindler11 (4 214 in chips) 
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
                    var playerStackText = handLine.TakeBetween("(", ")", true)
                        .Replace(" in chips", string.Empty)
                        .Replace("EUR", string.Empty)
                        .Replace(" ", string.Empty);

                    if (!ParserUtils.TryParseMoney(playerStackText, out decimal startingStack))
                    {
                        throw new PlayersException(handLine, "Couldn't parse stack of player.");
                    }

                    var player = new Player(playerName, startingStack, seatNumber);

                    playerList.Add(player);
                }

                // dankpot: shows [Kc 9h] (Full House Threes full of Nines)
                // keda777: mucks [Kh,Ac] (ThreeOfAKind) .
                if (handLine.IndexOf(": shows [", StringComparison.OrdinalIgnoreCase) > 0 ||
                    handLine.IndexOf(": mucks [", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    var playerName = handLine.Substring(0, handLine.LastIndexOf(":", StringComparison.OrdinalIgnoreCase));

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
            if (IsTournament(handLines))
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
            var seatLine = handLines[0];

            if (seatLine.IndexOf("10 Max", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return SeatType.FromMaxPlayers(10);
            }

            if (seatLine.IndexOf("9 Max", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return SeatType.FromMaxPlayers(9);
            }

            if (seatLine.IndexOf("8 Max", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return SeatType.FromMaxPlayers(8);
            }

            if (seatLine.IndexOf("6 Max", StringComparison.OrdinalIgnoreCase) >= 0 ||
                seatLine.IndexOf("Six Max", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return SeatType.FromMaxPlayers(6);
            }

            if (seatLine.IndexOf("3 Max", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return SeatType.FromMaxPlayers(3);
            }

            var maxPlayers = 0;

            var maxPlayersIndex = seatLine.IndexOf("-max", StringComparison.OrdinalIgnoreCase);

            if (maxPlayersIndex > 0)
            {
                var maxPlayersText = seatLine.Substring(maxPlayersIndex - 1, 1);

                maxPlayers = int.Parse(maxPlayersText);

                if (maxPlayers == 0)
                {
                    maxPlayers = 10;
                }
            }

            return SeatType.FromMaxPlayers(maxPlayers);
        }

        /// <summary>
        /// Adjusts seat types of the specified <see cref="HandHistory"/>
        /// </summary>
        /// <param name="handHistory"><see cref="HandHistory"/> to adjust</param>
        protected override void AdjustSeatTypes(HandHistory handHistory)
        {
            if (!handHistory.GameDescription.SeatType.IsUnknown)
            {
                return;
            }

            var maxSeatNumber = handHistory.Players.MaxOrDefault(x => x.SeatNumber);

            if (maxSeatNumber > 9)
            {
                handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(10);
            }
            else if (maxSeatNumber > 8)
            {
                handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(9);
            }
            else if (maxSeatNumber > 6)
            {
                handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(8);
            }
            else if (maxSeatNumber > 3)
            {
                handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(6);
            }
            else if (maxSeatNumber > 2 && handHistory.GameDescription.IsTournament)
            {
                handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(3);
            }
            else
            {
                handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(2);
            }
        }

        /// <summary>
        /// Parses table name from the specified HH lines
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Table name</returns>
        protected override string ParseTableName(string[] handLines)
        {
            var header = ParseHeader(handLines);

            if (header.Length < 3)
            {
                throw new TableNameException(handLines[0], "Table name couldn't be parsed.");
            }

            var tableName = header[header.Length - 3].Trim();

            if (tableName.StartsWith("- ", StringComparison.Ordinal))
            {
                tableName = tableName.Substring(2);
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
            var header = ParseHeader(handLines);

            var tournamentHeader = header[0].Replace("Table", string.Empty)
                .Replace("Turbo", string.Empty);

            var tournamentId = tournamentHeader.Substring(tournamentHeader.IndexOf(" T") + 2).Trim();

            var buyIn = Buyin.FromBuyinRake(0, 0, DefaultCurrency);

            if (header.Length == 7 && header[2].Contains("+"))
            {
                var buyInHeaders = header[2].Split('+');

                if (buyInHeaders.Length == 2)
                {
                    if (!ParserUtils.TryParseMoney(buyInHeaders[0].Trim(), out decimal prizePoolValue))
                    {
                        throw new BuyinException(handLines[0], "Could not parse buyin.");
                    }

                    if (!ParserUtils.TryParseMoney(buyInHeaders[1].Trim(), out decimal entryFee))
                    {
                        throw new BuyinException(handLines[0], "Could not parse entry fee.");
                    }

                    buyIn.PrizePoolValue = prizePoolValue;
                    buyIn.Rake = entryFee;
                }
            }

            var tournamentName = header[0].Replace($" T{tournamentId}", string.Empty).Trim();

            if (tournamentName.StartsWith("- ", StringComparison.Ordinal))
            {
                tournamentName = tournamentName.Substring(2);
            }

            var tournamentDescriptor = new TournamentDescriptor
            {
                TournamentId = tournamentId,
                Speed = ParserUtils.ParseTournamentSpeed(header[0]),
                TournamentName = tournamentName,
                BuyIn = buyIn
            };

            for (var i = handLines.Length - 1; i > 0; i--)
            {
                if (handLines[i].Contains(" finished "))
                {
                    var totalPlayersText = handLines[i].TakeBetween(" out of ", " players");

                    if (int.TryParse(totalPlayersText, out int totalPlayers))
                    {
                        tournamentDescriptor.TotalPlayers = totalPlayers;

                        if (totalPlayers > 1)
                        {
                            var placeText = handLines[i].TakeBetween(" finished ", " out of ");

                            if (int.TryParse(placeText, out int place))
                            {
                                tournamentDescriptor.FinishPosition = place;
                            }
                        }
                    }
                }
                else if (handLines[i].StartsWith("***SHOW", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            return tournamentDescriptor;
        }

        /// <summary>
        /// Clears the text of hand history from redudant data
        /// </summary>
        /// <param name="handText">Text of HH</param>
        /// <returns>Clean text</returns>
        protected override string ClearHandHistory(string handText)
        {
            var startIndex = handText.IndexOf("Hand#");

            if (startIndex < 0)
            {
                return string.Empty;
            }

            return handText.Substring(startIndex); ;
        }

        #region Internal Horizon specific methods

        /// <summary>
        /// Checks if HH is tournament HH
        /// </summary>
        /// <param name="handLines">Lines to check</param>
        /// <returns>True if tournament; otherwise - false</returns>
        private static bool IsTournament(string[] handLines)
        {
            var headers = ParseHeader(handLines);

            return headers.Length > 0 && headers[0]
                .Replace("Table", string.Empty)
                .Replace("Turbo", string.Empty)
                .IndexOf(" T", StringComparison.Ordinal) > 0;
        }

        /// <summary>
        /// Parses header into array of sub-headers
        /// </summary>
        /// <param name="handLines">Lines to parse</param>
        /// <returns>Array of sub-headers</returns>
        private static string[] ParseHeader(string[] handLines)
        {
            var handLine = handLines[0];
            var header = handLine.Substring(handLine.IndexOf(" - ") + 3).Split(new string[] { " -- " }, StringSplitOptions.None);

            return header;
        }

        #endregion
    }
}