//-----------------------------------------------------------------------
// <copyright file="Poker888FastParserImpl.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.FastParsing;
using HandHistories.Parser.Utils.Strings;
using HandHistories.Parser.Utils.Time;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace HandHistories.Parser.Parsers.FastParser._888
{
    sealed class Poker888FastParserImpl : HandHistoryParserFastImpl
    {
        private const string tournamentSummaryHeader = "***** Cassava Tournament Summary *****";

        static readonly CultureInfo invariant = CultureInfo.InvariantCulture;

        public override EnumPokerSites SiteName
        {
            get { return EnumPokerSites.Poker888; }
        }

        public override bool RequiresAllInDetection
        {
            get { return true; }
        }

        public override bool RequiresTotalPotCalculation
        {
            get { return true; }
        }

        public override bool RequiresBetWinAdjustment
        {
            get { return true; }
        }

        public override bool RequiresUncalledBetCalculations
        {
            get
            {
                return true;
            }
        }

        private static readonly NumberFormatInfo NumberFormatInfo = new NumberFormatInfo
        {
            NegativeSign = "-",
            CurrencyDecimalSeparator = ".",
            CurrencyGroupSeparator = ",",
            CurrencySymbol = "$"
        };

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            rawHandHistories = rawHandHistories.Replace("\r", "");

            return rawHandHistories.LazyStringSplit("\n\n").Where(s => string.IsNullOrWhiteSpace(s) == false && s.Equals("\r\n") == false);
        }

        private static readonly Regex DealerPositionRegex = new Regex(@"(?<=Seat )\d+", RegexOptions.Compiled);
        protected override int ParseDealerPosition(string[] handLines)
        {
            return Int32.Parse(DealerPositionRegex.Match(handLines[4]).Value);
        }

        private static readonly Regex DateLineRegex = new Regex(@"\d+ \d+ \d+ \d+\:\d+\:\d+", RegexOptions.Compiled);
        private static readonly Regex DateRegex = new Regex(@"(\d+) (\d+) (\d+) ", RegexOptions.Compiled);
        protected override DateTime ParseDateUtc(string[] handLines)
        {
            //Date looks like: 04 02 2012 23:59:48
            string dateString = DateLineRegex.Match(handLines[2]).Value;
            //Change string so it becomes 2012-02-04 23:59:48
            dateString = DateRegex.Replace(dateString, "$3-$2-$1 ");

            DateTime dateTime = DateTime.Parse(dateString);

            TimeZoneType timeZone = TimeZoneType.GMT;

            // WSOP and TowerTorneosPoker used local time zone for hand history.
            if (handLines[1].Contains("WSOP.com") || handLines[1].Contains("TowerTorneosPoker"))
                timeZone = TimeZoneType.Current;

            DateTime utcTime = TimeZoneUtil.ConvertDateTimeToUtc(dateTime, timeZone);

            return utcTime;
        }

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            if (handLines[3].StartsWith("Tournament", StringComparison.InvariantCultureIgnoreCase))
            {
                return PokerFormat.Tournament;
            }

            return PokerFormat.CashGame;
        }

        private static readonly Regex HandIdRegex = new Regex(@"(?<=#Game No \: )\d+", RegexOptions.Compiled);
        protected override long ParseHandId(string[] handLines)
        {
            return long.Parse(HandIdRegex.Match(handLines[0]).Value);
        }

        private static readonly Regex TableNameRegex = new Regex(@"(?<=Table ).*$", RegexOptions.Compiled);
        protected override string ParseTableName(string[] handLines)
        {
            //"Table Athens 10 Max (Real Money)" -> "Athens"
            var tableName = TableNameRegex.Match(handLines[3]).Value;
            tableName = tableName.Substring(0, tableName.Length - 19).TrimEnd();
            return tableName;
        }

        private static readonly Regex NumPlayersRegex = new Regex(@"(?<=Total number of players : )\d+", RegexOptions.Compiled);
        private static readonly Regex SeatTypeRegex = new Regex(@"\d+ Max", RegexOptions.Compiled);

        protected override SeatType ParseSeatType(string[] handLines)
        {
            var seatCount = int.Parse(SeatTypeRegex.Match(handLines[3]).Value.Replace(" Max", string.Empty));

            if (seatCount > 0 && seatCount < 10)
            {
                return SeatType.FromMaxPlayers(seatCount);
            }

            return SeatType.FromMaxPlayers(10);
        }

        private static readonly Regex GameTypeRegex = new Regex(@"(?<=Blinds ).*(?= - )", RegexOptions.Compiled);
        protected override GameType ParseGameType(string[] handLines)
        {
            string gameTypeString = GameTypeRegex.Match(handLines[2]).Value;
            gameTypeString = gameTypeString.Replace(" Jackpot table", "");
            switch (gameTypeString)
            {
                case "No Limit Holdem":
                case "No Limit Holdem Jackpot table":
                    return GameType.NoLimitHoldem;
                case "Fix Limit Holdem":
                    return GameType.FixedLimitHoldem;
                case "Pot Limit Holdem":
                    return GameType.PotLimitHoldem;
                case "Pot Limit Omaha":
                    return GameType.PotLimitOmaha;
                case "No Limit Omaha":
                    return GameType.NoLimitOmaha;
                case "Pot Limit OmahaHL":
                    return GameType.PotLimitOmahaHiLo;
                default:
                    throw new NotImplementedException("Unrecognized game type " + gameTypeString);
            }
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            // 888 does not store any information about the table type in the hand history

            // we assume the table is push or fold if
            // - there is at least one player with exactly 5bb
            // - the average stack is < 17.5bb 
            // - at least two players have < 10bb
            // - there is no player with > 20bb

            // the min buyin for standard table is > 30bb, so this should work in most cases
            // furthermore if on a regular table the average stack is < 17.5, the play is just like on a push fold table and vice versa
            bool isjackPotTable = handLines[2].Contains(" Jackpot table");
            bool isPushOrFoldTable = handLines[3].Contains("Push or Fold");

            if (isPushOrFoldTable)
            {
                if (isjackPotTable)
                {
                    return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold, TableTypeDescription.Jackpot);
                }

                return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold);
            }

            if (isjackPotTable)
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Jackpot);
            }

            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            string line = handLines[2];

            int LimitEndIndex = line.IndexOf(" Blinds", StringComparison.Ordinal);
            string limitString = line.Remove(LimitEndIndex)
                .RemoveWhitespace()
                .Replace("$", "")
                ;

            int splitIndex = limitString.IndexOf('/');

            string lowLimitString = limitString.Remove(splitIndex);
            string highLimitString = limitString.Substring(splitIndex + 1);

            decimal lowLimit = ParseAmount(lowLimitString);
            decimal highLimit = ParseAmount(highLimitString);

            string tableNameLine = handLines[3];

            var currency = tableNameLine.IndexOf("Play Money", StringComparison.InvariantCultureIgnoreCase) > 0 ? Currency.PlayMoney : Currency.USD;

            return Limit.FromSmallBlindBigBlind(lowLimit, highLimit, currency);
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            throw new NotImplementedException();
        }

        public override bool IsValidHand(string[] handLines)
        {
            return handLines[handLines.Length - 1].Contains(" collected ");
        }

        public override bool IsValidOrCanceledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;

            var seatedPlayersLine = handLines[5];

            if (seatedPlayersLine[seatedPlayersLine.Length - 1] == '1')
            {
                isCancelled = true;
            }

            return IsValidHand(handLines);
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            Street currentStreet = Street.Preflop;

            List<HandAction> handActions = new List<HandAction>();

            for (int i = 6; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if (currentStreet == Street.Preflop)
                {
                    if (handLine.IndexOf(':') != -1)
                    {
                        continue;
                    }
                    else if (handLine[0] == 'D' && handLine.StartsWith("Dealt "))
                    {
                        continue;
                    }
                }

                if (handLine[0] == '*')
                {
                    if (handLine[3] == 'S')
                    {
                        currentStreet = Street.Showdown;
                        continue;
                    }

                    switch (handLine[11])
                    {
                        case 'r':
                            currentStreet = Street.River;
                            break;
                        case 'f':
                            currentStreet = Street.Flop;
                            break;
                        case 't':
                            currentStreet = Street.Turn;
                            break;
                    }

                    continue;
                }

                if (currentStreet == Street.Showdown)
                {
                    // ignore lines such as:
                    //  OprahTiltfre did not show his hand
                    if (handLine[handLine.Length - 1] != ']')
                    {
                        continue;
                    }

                    int openSquareIndex = handLine.LastIndexOf('[');

                    // winnings hands have numbers such as:
                    // OprahTiltfre collected [ $2,500 ]
                    // Peon_84 collected [ 0,06 $ ]

                    string textInSquare = handLine.Substring(openSquareIndex + 1, handLine.Length - openSquareIndex - 1 - 1);

                    decimal amount;

                    if (ParserUtils.TryParseMoney(textInSquare, out amount))
                    {
                        string playerName = handLine.Substring(0, openSquareIndex - 11);

                        handActions.Add(new WinningsAction(playerName, HandActionType.WINS, amount, 0));
                        continue;
                    }

                    string action = handLine.Substring(openSquareIndex - 6, 5);

                    if (action.Equals("shows"))
                    {
                        string playerName = handLine.Substring(0, openSquareIndex - 7);
                        handActions.Add(new HandAction(playerName, HandActionType.SHOW, 0, currentStreet));
                        continue;
                    }
                    else if (action.Equals("mucks"))
                    {
                        string playerName = handLine.Substring(0, openSquareIndex - 7);
                        handActions.Add(new HandAction(playerName, HandActionType.MUCKS, 0, currentStreet));
                        continue;
                    }

                    throw new HandActionException(handLine, "Unparsed");
                }

                if (handLine[handLine.Length - 1] == ']')
                {
                    int openSquareIndex = handLine.LastIndexOf('[');
                    string amountString = handLine.Substring(openSquareIndex + 1, handLine.Length - openSquareIndex - 1 - 1);

                    decimal amount = ParseAmount(amountString);

                    string action = handLine.Substring(openSquareIndex - 8, 7);

                    if (currentStreet == Street.Preflop)
                    {
                        if (action.Equals("l blind", StringComparison.Ordinal)) // small blind
                        {
                            string playerName = handLine.Substring(0, openSquareIndex - 19);
                            amount = ParseAmount(amountString);
                            handActions.Add(new HandAction(playerName, HandActionType.SMALL_BLIND, amount, currentStreet));
                            continue;
                        }
                        else if (action.Equals("g blind", StringComparison.Ordinal)) // big blind
                        {
                            string playerName = handLine.Substring(0, openSquareIndex - 17);
                            amount = ParseAmount(amountString);
                            handActions.Add(new HandAction(playerName, HandActionType.BIG_BLIND, amount, currentStreet));
                            continue;
                        }
                        else if (action.Equals("d blind", StringComparison.Ordinal))//dead blind
                        {
                            string playerName = handLine.Substring(0, openSquareIndex - 18);
                            amount = ParseDeadBlindAmount(amountString);
                            handActions.Add(new HandAction(playerName, HandActionType.POSTS, amount, currentStreet));
                            continue;
                        }
                        else if (action.Equals("ts ante", StringComparison.Ordinal)) // ante
                        {
                            string playerName = handLine.Substring(0, openSquareIndex - 12);
                            amount = ParseAmount(amountString);
                            handActions.Add(new HandAction(playerName, HandActionType.ANTE, amount, currentStreet));
                            continue;
                        }
                    }

                    amount = ParseAmount(amountString);

                    if (action.EndsWith("raises"))
                    {
                        string playerName = handLine.Substring(0, openSquareIndex - 8);
                        handActions.Add(new HandAction(playerName, HandActionType.RAISE, amount, currentStreet));
                        continue;
                    }
                    else if (action.EndsWith("bets"))
                    {
                        string playerName = handLine.Substring(0, openSquareIndex - 6);
                        handActions.Add(new HandAction(playerName, HandActionType.BET, amount, currentStreet));
                        continue;
                    }
                    else if (action.EndsWith("calls"))
                    {
                        string playerName = handLine.Substring(0, openSquareIndex - 7);
                        handActions.Add(new HandAction(playerName, HandActionType.CALL, amount, currentStreet));
                        continue;
                    }
                }
                else if (handLine.FastEndsWith("folds"))
                {
                    string playerName = handLine.Substring(0, handLine.Length - 6);
                    handActions.Add(new HandAction(playerName, HandActionType.FOLD, currentStreet));
                    continue;
                }
                else if (handLine.EndsWith("checks"))
                {
                    string playerName = handLine.Substring(0, handLine.Length - 7);
                    handActions.Add(new HandAction(playerName, HandActionType.CHECK, currentStreet));
                    continue;
                }

                throw new HandActionException(handLine, "Unknown hand line.");
            }

            return handActions;
        }

        private static decimal ParseAmount(string amountString)
        {
            // this split helps us parsing dead posts like [$0.10 + $0.05]
            var splittedAmounts = amountString.Split('+');
            var result = 0.0m;

            foreach (var amount in splittedAmounts)
            {
                result += ParserUtils.ParseMoney(amount);
            }

            return result;
        }

        static decimal ParseDeadBlindAmount(string amountString)
        {
            int plusIndex = amountString.IndexOf('+');
            string amountStr1 = amountString.Remove(plusIndex);
            string amountStr2 = amountString.Substring(plusIndex + 1);

            decimal amount1 = ParseAmount(amountStr1);
            decimal amount2 = ParseAmount(amountStr2);

            return amount1 + amount2;
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            int seatCount = Int32.Parse(NumPlayersRegex.Match(handLines[5]).Value);

            PlayerList playerList = new PlayerList();

            for (int i = 0; i < seatCount; i++)
            {
                string handLine = handLines[6 + i];

                int colonIndex = handLine.IndexOf(':');
                int openParenIndex = handLine.IndexOf('(');

                int seat = int.Parse(handLine.Substring(5, colonIndex - 5));
                string playerName = handLine.Substring(colonIndex + 2, openParenIndex - colonIndex - 3);
                decimal amount = ParseAmount(handLine.Substring(openParenIndex + 2, handLine.Length - openParenIndex - 3 - 1).Trim());

                playerList.Add(new Player(playerName, amount, seat));
            }

            // Add hole-card info
            for (int i = handLines.Length - 2; i >= 0; i--)
            {
                string handLine = handLines[i];

                if (handLine[0] == '*')
                {
                    break;
                }

                if (handLine.EndsWith("]") &&
                    char.IsDigit(handLine[handLine.Length - 3]) == false)
                {
                    int openSquareIndex = handLine.IndexOf('[');

                    string cards = handLine.Substring(openSquareIndex + 2, handLine.Length - openSquareIndex - 2 - 2);
                    HoleCards holeCards = HoleCards.FromCards(CardNormalization(handLines, cards.Replace(",", string.Empty).RemoveWhitespace()));

                    string playerName = handLine.Substring(0, openSquareIndex - 7);

                    Player player = playerList.First(p => p.PlayerName.Equals(playerName));
                    player.HoleCards = holeCards;
                }
            }

            return playerList;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            string boardCards = string.Empty;

            for (int i = 0; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if (handLine[0] != '*')
                {
                    continue;
                }

                if (handLine[3] != 'D')
                {
                    continue;
                }

                int openSquareIndex;
                switch (handLine[11])
                {
                    case 'r':
                        openSquareIndex = 20;
                        break;
                    default:
                        openSquareIndex = 19;
                        break;
                }

                string cards = handLine.Substring(openSquareIndex + 2, handLine.Length - openSquareIndex - 2 - 2);

                boardCards += cards.Replace(",", string.Empty).RemoveWhitespace();
            }

            return BoardCards.FromCards(CardNormalization(handLines, boardCards));
        }

        private string CardNormalization(string[] handlines, string boardCards)
        {
            if (!handlines[1].Contains("TowerTorneosPoker"))
                return boardCards;

            // In tower torneos used French names of suits:
            // tréboles     - clubs
            // diamantes    - diamonds
            // picas        - spades
            // corazones    - hearts
            boardCards = boardCards.Replace('c', 'h')
                .Replace('t', 'c')
                .Replace('d', 'd')
                .Replace('p', 's');

            // Same for 10
            // dix - ten
            return boardCards.Replace('D', 'T');
            //.Replace('J', 'J')
            //.Replace('Q', 'Q')
            //.Replace('K', 'K')
            //.Replace('A', 'A');
        }

        protected override string ParseHeroName(string[] handlines, PlayerList playerList)
        {
            const string DealText = "Dealt to ";

            for (int i = 0; i < handlines.Length; i++)
            {
                if (handlines[i][0] == 'D' && handlines[i].StartsWith(DealText))
                {
                    string line = handlines[i];

                    int startIndex = line.IndexOf('[');

                    var heroName = line.Substring(9, startIndex - 10);

                    if (playerList != null && playerList[heroName] != null && playerList[heroName].HoleCards == null)
                    {
                        var cards = line.Substring(startIndex + 1, line.Length - startIndex - 2)
                                    .Replace("[", string.Empty).Replace("]", string.Empty).Replace(",", " ");

                        playerList[heroName].HoleCards = HoleCards.FromCards(CardNormalization(handlines, cards));
                    }

                    return heroName;
                }
            }

            return null;
        }

        protected override TournamentDescriptor ParseTournament(string[] handLines)
        {
            var line = handLines[3];

            var regex = new Regex(@"Tournament #(?<tournament_id>[^\s]+) (?<buyin>[^-]+) - Table #(?<tablenum>\d+) (?<tabletype>[^\(]+) \((?<money>[^\)]+)\)");

            var match = regex.Match(line);

            if (!match.Success)
            {
                return null;
            }

            var buyinText = match.Groups["buyin"].Value;

            var splittedBuyin = buyinText.Split('+');

            decimal prizePool = 0;

            ParserUtils.TryParseMoney(splittedBuyin[0], out prizePool);

            var rake = splittedBuyin.Length > 1 ? ParserUtils.ParseMoney(splittedBuyin[1]) : 0m;

            var currency = match.Groups["money"].Value.Contains("Real Money") ? Currency.USD : Currency.PlayMoney;

            var tournamentDescriptor = new TournamentDescriptor
            {
                TournamentId = match.Groups["tournament_id"].Value,
                BuyIn = Buyin.FromBuyinRake(prizePool, rake, currency),
                Speed = TournamentSpeed.Regular,
                TournamentName = string.Format("Tournament #{0}", match.Groups["tournament_id"].Value)
            };

            return tournamentDescriptor;
        }

        protected override bool IsSummaryHand(string[] handLines)
        {
            return handLines.Length > 0 && handLines[0].StartsWith(tournamentSummaryHeader, StringComparison.InvariantCultureIgnoreCase);
        }

        private static readonly Regex SummaryFinishedRegex = new Regex(@"(?<player>.*) finished (?<position>\d+)/(?<total>\d+)(?: and won (?<won>.*))?", RegexOptions.Compiled);

        protected override HandHistory ParseSummaryHand(string[] handLines, HandHistory handHistory)
        {
            var tournament = new TournamentDescriptor
            {
                Summary = string.Join(Environment.NewLine, handLines)
            };

            foreach (var handLine in handLines)
            {
                // parse tournament id
                if (handLine.StartsWith("Tournament ID", StringComparison.InvariantCultureIgnoreCase))
                {
                    var indexOfColon = handLine.IndexOf(":");
                    tournament.TournamentId = handLine.Substring(indexOfColon + 1).Trim();
                }
                else if (handLine.StartsWith("Buy-In", StringComparison.InvariantCultureIgnoreCase))
                {
                    var buyInText = ParseTournamentSummaryMoneyLine(handLine);

                    var buyInSplit = buyInText.Split('+');

                    decimal buyIn = 0;
                    decimal rake = 0;
                    Currency currency = Currency.USD;

                    if (ParserUtils.TryParseMoney(buyInSplit[0], out buyIn, out currency))
                    {
                        if (buyInSplit.Length > 1)
                        {
                            ParserUtils.TryParseMoney(buyInSplit[1], out rake, out currency);
                        }
                    }

                    tournament.BuyIn = Buyin.FromBuyinRake(buyIn, rake, currency);
                }
                else if (handLine.StartsWith("Rebuy", StringComparison.InvariantCultureIgnoreCase))
                {
                    var rebuyText = ParseTournamentSummaryMoneyLine(handLine);

                    decimal rebuy = 0;
                    Currency currency = Currency.USD;

                    if (ParserUtils.TryParseMoney(rebuyText, out rebuy, out currency))
                    {
                        tournament.Rebuy = rebuy;
                    }
                }
                else if (handLine.StartsWith("Add-On", StringComparison.InvariantCultureIgnoreCase))
                {
                    var addonText = ParseTournamentSummaryMoneyLine(handLine);

                    decimal addon = 0;
                    Currency currency = Currency.USD;

                    if (ParserUtils.TryParseMoney(addonText, out addon, out currency))
                    {
                        tournament.Addon = addon;
                    }
                }
                else if (handLine.Contains("finished"))
                {
                    var match = SummaryFinishedRegex.Match(handLine);

                    if (!match.Success)
                    {
                        LogProvider.Log.Error(string.Format("'{0}' wasn't parsed", handLine));
                        continue;
                    }

                    var playerName = match.Groups["player"].Value;
                    var positionText = match.Groups["position"].Value;
                    var totalPlayersText = match.Groups["total"].Value;
                    var wonText = match.Groups["won"] != null ? match.Groups["won"].Value.Replace(",", ".").Trim() : string.Empty;

                    handHistory.Hero = new Player(playerName, 0, 0);

                    short position = 0;
                    short totalPlayers = 0;
                    decimal won = 0;
                    Currency wonCurrency = Currency.USD;

                    if (!short.TryParse(positionText, out position))
                    {
                        LogProvider.Log.Error(string.Format("'{0}' position wasn't parsed", handLine));
                        continue;
                    }

                    if (!short.TryParse(totalPlayersText, out totalPlayers))
                    {
                        LogProvider.Log.Error(string.Format("'{0}' total players weren't parsed", handLine));
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(wonText) && !ParserUtils.TryParseMoney(wonText, out won, out wonCurrency))
                    {
                        LogProvider.Log.Error(string.Format("'{0}' won data wasn't parsed", handLine));
                        continue;
                    }

                    tournament.FinishPosition = position;
                    tournament.TotalPlayers = totalPlayers;
                    tournament.Winning = won;
                }
            }

            handHistory.GameDescription = new GameDescriptor(EnumPokerSites.Poker888,
                GameType.Unknown,
                null,
                TableType.FromTableTypeDescriptions(),
                SeatType.AllSeatType(),
                tournament);

            return handHistory;
        }

        private string ParseTournamentSummaryMoneyLine(string line)
        {
            line = line.Replace(",", ".");
            var indexOfColon = line.IndexOf(":");
            var parsedText = line.Substring(indexOfColon + 1).Trim();
            return parsedText;
        }

        protected override void CalculateUncalledBets(string[] handLines, HandHistory handHistory)
        {
            HandHistoryUtils.CalculateUncalledBets(handHistory, false);
        }
    }
}