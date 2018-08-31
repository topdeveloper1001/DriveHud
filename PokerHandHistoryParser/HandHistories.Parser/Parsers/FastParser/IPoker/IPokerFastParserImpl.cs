//-----------------------------------------------------------------------
// <copyright file="IPokerFastParserImpl.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Objects.Utils;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.FastParsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace HandHistories.Parser.Parsers.FastParser.IPoker
{
    internal class IPokerFastParserImpl : HandHistoryParserFastImpl
    {
        public override EnumPokerSites SiteName
        {
            get
            {
                return EnumPokerSites.IPoker;
            }
        }

        public override bool RequiresAdjustedRaiseSizes
        {
            get
            {
                return true;
            }
        }

        public override bool RequiresActionSorting
        {
            get
            {
                return true;
            }
        }

        public override bool RequiresAllInDetection
        {
            get
            {
                return true;
            }
        }

        public override bool RequiresAllInUpdates
        {
            get
            {
                return true;
            }
        }

        public override bool RequiresSeatTypeAdjustment
        {
            get
            {
                return true;
            }
        }

        public override bool RequiresTournamentSpeedAdjustment
        {
            get
            {
                return true;
            }
        }

        public override bool RequiresBetWinAdjustment
        {
            get
            {
                return true;
            }
        }

        public override bool RequiresUncalledBetCalculations
        {
            get
            {
                return true;
            }
        }

        protected override string[] SplitHandsLines(string handText)
        {
            // convert hand text to XML to be able to parse it string by string
            var handDocument = XDocument.Parse(handText);
            return base.SplitHandsLines(handDocument.ToString());
        }

        private XDocument GetXDocumentFromLines(string[] handLines)
        {
            string handString = string.Join("", handLines);

            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.ConformanceLevel = ConformanceLevel.Fragment;

            XDocument doc = new XDocument(new XElement("root"));
            XElement root = doc.Descendants().First();

            using (StringReader fs = new StringReader(handString))
            {
                using (XmlReader xr = XmlReader.Create(fs, xrs))
                {
                    while (xr.Read())
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            root.Add(XElement.Load(xr.ReadSubtree()));
                        }
                    }
                }
            }

            return doc;
        }

        private int GetSeatNumberFromPlayerLine(string playerLine)
        {
            int seatOffset = playerLine.IndexOf(" seat=", StringComparison.Ordinal) + 7;
            int seatEndOffset = playerLine.IndexOf("\"", seatOffset, StringComparison.Ordinal);
            string seatNumberString = playerLine.Substring(seatOffset, seatEndOffset - seatOffset);
            return int.Parse(seatNumberString);
        }

        private bool IsPlayerLineDealer(string playerLine)
        {
            int dealerOffset = playerLine.IndexOf(" dealer=", StringComparison.Ordinal);
            int dealerValue = int.Parse(" " + playerLine[dealerOffset + 9]);
            return dealerValue == 1;
        }

        private decimal GetBetNumberFromPlayerLine(string playerLine)
        {
            var betStartPos = playerLine.IndexOf(" bet=", StringComparison.Ordinal);

            if (betStartPos == -1)
            {
                return 0;
            }

            betStartPos += 6;

            var betEndPos = playerLine.IndexOf("\"", betStartPos, StringComparison.Ordinal) - 1;

            var betString = playerLine.Substring(betStartPos, betEndPos - betStartPos + 1);

            var bet = ParserUtils.ParseMoney(betString);
            return bet;
        }

        private decimal GetStackFromPlayerLine(string playerLine)
        {
            var stackStartPos = playerLine.IndexOf(" chips=", StringComparison.Ordinal) + 8;
            var stackEndPos = playerLine.IndexOf("\"", stackStartPos, StringComparison.Ordinal) - 1;
            var stackString = playerLine.Substring(stackStartPos, stackEndPos - stackStartPos + 1);
            var stack = ParserUtils.ParseMoney(stackString);
            return stack;
        }

        private decimal GetWinningsFromPlayerLine(string playerLine)
        {
            var winStartPos = playerLine.IndexOf(" win=", StringComparison.Ordinal);

            if (winStartPos == -1)
            {
                return 0;
            }

            winStartPos += 6;

            var winEndPos = playerLine.IndexOf("\"", winStartPos, StringComparison.Ordinal) - 1;

            var winString = playerLine.Substring(winStartPos, winEndPos - winStartPos + 1);

            var win = ParserUtils.ParseMoney(winString);
            return win;
        }

        private string GetNameFromPlayerLine(string playerLine)
        {
            int nameStartPos = playerLine.IndexOf(" n", StringComparison.Ordinal) + 7;
            int nameEndPos = playerLine.IndexOf("\"", nameStartPos, StringComparison.Ordinal) - 1;
            string name = playerLine.Substring(nameStartPos, nameEndPos - nameStartPos + 1);
            return name;
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            string[] playerLines = GetPlayerLinesFromHandLines(handLines);

            for (int i = 0; i < playerLines.Count(); i++)
            {
                string playerLine = playerLines[i];
                if (IsPlayerLineDealer(playerLine))
                {
                    return GetSeatNumberFromPlayerLine(playerLine);
                }
            }

            return 0;
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            var dateLine = GetStartDateFromHandLines(handLines);

            var startPos = dateLine.IndexOf(">", StringComparison.Ordinal) + 1;
            var endPos = dateLine.LastIndexOf("<", StringComparison.Ordinal) - 1;

            var dateString = dateLine.Substring(startPos, endPos - startPos + 1);

            var dateTime = ParserUtils.ParseDate(dateString);

            return dateTime;
        }

        private static readonly Regex SessionGroupRegex = new Regex("<session.*?session>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex GameGroupRegex = new Regex("<game.*?game>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.Multiline);

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            //Remove XML headers if necessary 
            rawHandHistories = rawHandHistories.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n", "");

            //Two Cases - Case 1, we have a single <session> tag holding multiple <game> tags
            //            Case 2, we have multiple <session> tags each holding a single <game> tag.
            //            We need our enumerable to have only case 2 representations for parsing

            if (rawHandHistories.IndexOf("<session", StringComparison.Ordinal) ==
                    rawHandHistories.LastIndexOf("<session", StringComparison.Ordinal))
            {
                //We are case 1 - convert to case 2

                int endOfGeneralInfoIndex = rawHandHistories.IndexOf("</general>", StringComparison.Ordinal);

                if (endOfGeneralInfoIndex == -1)
                {
                    // log the first 1000 chars of the file, so we can at least guess what's the problem
                    LogProvider.Log.Error(this, string.Format("IPokerFastParserImpl.SplitUpMultipleHands(): Encountered a weird file\r\n{0}", rawHandHistories.Substring(0, 1000)));
                }

                string generalInfoString = rawHandHistories.Substring(0, endOfGeneralInfoIndex + 10);

                MatchCollection gameMatches = GameGroupRegex.Matches(rawHandHistories, endOfGeneralInfoIndex + 9);
                foreach (Match gameMatch in gameMatches)
                {
                    string fullGameString = generalInfoString + "\r\n" + gameMatch.Value + "\r\n</session>";
                    yield return fullGameString;
                }
            }
            else
            {
                //We are case 2
                MatchCollection matches = SessionGroupRegex.Matches(rawHandHistories);

                foreach (Match match in matches)
                {
                    yield return match.Value;
                }
            }
        }

        protected override long ParseHandId(string[] handLines)
        {
            foreach (var handLine in handLines)
            {
                if (handLine.StartsWith("<game ", StringComparison.Ordinal))
                {
                    var startIndex = handLine.IndexOf("\"", StringComparison.Ordinal) + 1;
                    var endIndex = handLine.LastIndexOf("\"", StringComparison.Ordinal) - 1;

                    var gamecodeText = handLine.Substring(startIndex, endIndex - startIndex + 1);

                    return long.Parse(gamecodeText);
                }
            }

            throw new HandIdException(handLines[1], "Couldn't find hand id");
        }

        protected override string ParseTableName(string[] handLines)
        {
            //<tablename>Ambia, 98575671</tablename>
            string tableNameLine = GetTableNameLineFromHandLines(handLines);
            int tableNameStartIndex = tableNameLine.IndexOf(">", StringComparison.Ordinal) + 1;
            int tableNameEndIndex = tableNameLine.LastIndexOf("<", StringComparison.Ordinal) - 1;

            string tableName = tableNameLine.Substring(tableNameStartIndex, tableNameEndIndex - tableNameStartIndex + 1);

            return tableName;
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            var maxPlayersText = GetMaxPlayersFromHandLines(handLines);

            if (!string.IsNullOrEmpty(maxPlayersText))
            {
                var maxPlayers = 0;

                if (int.TryParse(maxPlayersText, out maxPlayers) && maxPlayers > 0 && maxPlayers < 11)
                {
                    return SeatType.FromMaxPlayers(maxPlayers);
                }
            }

            var playerLines = GetPlayerLinesFromHandLines(handLines);

            int numPlayers = playerLines.Length;

            if (numPlayers <= 2)
            {
                return SeatType.FromMaxPlayers(2);
            }
            if (numPlayers <= 6)
            {
                return SeatType.FromMaxPlayers(6);
            }
            if (numPlayers <= 9)
            {
                return SeatType.FromMaxPlayers(9);
            }

            return SeatType.FromMaxPlayers(10);
        }

        private string GetGameTypeLineFromHandLines(string[] handLines)
        {
            return GetTagLine(handLines, "gametype");
        }

        private string GetTableNameLineFromHandLines(string[] handLines)
        {
            return GetTagLine(handLines, "tablename");
        }

        private string GetStartDateFromHandLines(string[] handLines)
        {
            for (int i = 0; i < handLines.Length; i++)
            {
                if (handLines[i].IndexOf("gamecode=\"", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    return handLines[i + 2];
                }
            }

            return handLines[8];
        }

        private string GetMaxPlayersFromHandLines(string[] handLines)
        {
            foreach (var handLine in handLines)
            {
                if (handLine.IndexOf("<maxplayers", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return GetTagValue(handLine);
                }
                else if (handLine.IndexOf("</general>", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    break;
                }
            }

            return null;
        }

        protected string[] GetPlayerLinesFromHandLines(string[] handLines)
        {
            /*
              Returns all of the detail lines between the <players> tags
              <players> <-- Line offset 22
                <player seat="1" name="Leichenghui" chips="£1,866.23" dealer="1" win="£0" bet="£5" rebuy="0" addon="0" />
                ...
                <player seat="10" name="CraigyColesBRL" chips="£2,297.25" dealer="0" win="£15" bet="£10" rebuy="0" addon="0" />
              </players>             
             */

            var offset = GetFirstPlayerIndex(handLines);
            var playerLines = new List<string>();

            var line = handLines[offset].TrimStart();

            while (offset < handLines.Length && line[1] != '/')
            {
                playerLines.Add(line);

                offset++;

                if (offset >= handLines.Length)
                {
                    break;
                }

                line = handLines[offset];
                line = line.TrimStart();
            }

            return playerLines.ToArray();
        }

        private int GetFirstPlayerIndex(string[] handLines)
        {
            for (int i = 10; i < handLines.Length; i++)
            {
                if (handLines[i][1] == 'p' && handLines[i][4] == 'y')
                {
                    return i + 1;
                }
            }

            throw new IndexOutOfRangeException("Did not find first player");
        }

        private string[] GetCardLinesFromHandLines(string[] handLines)
        {
            List<string> cardLines = new List<string>();
            for (int i = 0; i < handLines.Length; i++)
            {
                string handLine = handLines[i];
                handLine = handLine.TrimStart();

                if (!handLine.StartsWith("<cards", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                cardLines.Add(handLine);
            }

            return cardLines.ToArray();
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            /*
             * NLH <gametype>Holdem NL $2/$4</gametype>  
             * NLH <gametype>Holdem PL $2/$4</gametype>    
             * FL  <gametype>Holdem L $2/$4</gametype>
             * PLO <gametype>Omaha PL $0.50/$1</gametype>
             * PLO8 <gametype>Omaha Hi-Lo PL $0.05/$0.10</gametype>
             * NLO8 <gametype>Omaha Hi-Lo NL $2/$4</gametype>
             * FLO8 <gametype>Omaha Hi-Lo Limit $2/$4</gametype>
             */

            string gameTypeLine = GetGameTypeLineFromHandLines(handLines);

            if (string.IsNullOrEmpty(gameTypeLine))
            {
                throw new Exception("Could not parse GameType for hand.");
            }

            //If this is an H we're a Holdem, if O, Omaha
            char gameTypeChar = gameTypeLine[10];

            if (gameTypeChar == 'O')
            {
                char hiLoChar = gameTypeLine[16];
                if (hiLoChar == 'H')
                {
                    //HiLo
                    var omahaTypeChar = gameTypeLine[22];
                    switch (omahaTypeChar)
                    {
                        case 'P':
                            return GameType.PotLimitOmahaHiLo;
                        case 'N':
                            return GameType.NoLimitOmahaHiLo;
                        case 'L':
                            return GameType.FixedLimitOmahaHiLo;
                    }
                }
                else
                {
                    //Hi
                    var omahaTypeChar = hiLoChar;
                    switch (omahaTypeChar)
                    {
                        case 'P':
                            return GameType.PotLimitOmaha;
                        case 'N':
                            return GameType.NoLimitOmaha;
                        case 'L':
                            return GameType.FixedLimitOmaha;
                    }
                }

                return GameType.PotLimitOmaha;
            }

            char holdemTypeChar = gameTypeLine[17];

            if (holdemTypeChar == 'L')
            {
                return GameType.FixedLimitHoldem;
            }

            if (holdemTypeChar == 'N')
            {
                return GameType.NoLimitHoldem;
            }

            if (holdemTypeChar == 'P')
            {
                return GameType.PotLimitHoldem;
            }

            if (holdemTypeChar == 'S')
            {
                return GameType.SpreadLimitHoldem;
            }

            throw new Exception("Could not parse GameType for hand.");
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            string tableName = ParseTableName(handLines);

            if (tableName.StartsWith("(Shallow)"))
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Shallow);
            }

            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            throw new NotImplementedException();
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            //<gametype>Holdem NL $0.02/$0.04</gametype>           
            //<gametype>Holdem NL 0.02/0.04</gametype>
            //<gametype>Tournament, 1034034</gametype>
            //<currency>USD</currency>

            var gameTypeLine = GetGameTypeLineFromHandLines(handLines);

            if (string.IsNullOrEmpty(gameTypeLine))
            {
                throw new Exception("Could not parse Limit for hand.");
            }

            int limitStringBeginIndex = gameTypeLine.LastIndexOf(' ') + 1;
            int limitStringEndIndex = gameTypeLine.LastIndexOf("<", StringComparison.Ordinal) - 1;

            string limitString = gameTypeLine.Substring(limitStringBeginIndex, limitStringEndIndex - limitStringBeginIndex + 1);

            char currencySymbol = limitString[0];

            Currency currency;

            switch (currencySymbol)
            {
                case '$':
                    currency = Currency.USD;
                    limitString = limitString.Substring(1);
                    break;

                case '€':
                    currency = Currency.EURO;
                    limitString = limitString.Substring(1);
                    break;

                case '£':
                    currency = Currency.GBP;
                    limitString = limitString.Substring(1);
                    break;

                default:
                    currency = GetCurrency(handLines);
                    break;
            }

            int slashIndex = limitString.IndexOf("/", StringComparison.Ordinal);

            if (slashIndex == -1)
            {
                return Limit.FromSmallBlindBigBlind(0, 0, currency);
            }

            string smallString = limitString.Remove(slashIndex);

            decimal small = decimal.Parse(smallString, System.Globalization.CultureInfo.InvariantCulture);

            string bigString = limitString.Substring(slashIndex + 1);

            if (bigString[0] == '£' || bigString[0] == '$' || bigString[0] == '€')
            {
                bigString = bigString.Substring(1);
            }

            decimal big = decimal.Parse(bigString, CultureInfo.InvariantCulture);

            return Limit.FromSmallBlindBigBlind(small, big, currency);
        }

        private string GetCurrencyTagValue(string[] handLines)
        {
            for (int i = 0; i < handLines.Length; i++)
            {
                string handline = handLines[i];

                if (handline[1] == 'c' && handline[2] == 'u')
                {
                    int endIndex = handline.IndexOf('<', 10);
                    return handline.Substring(10, endIndex - 10);
                }
            }

            return string.Empty;
        }

        private Currency GetCurrency(string[] handLines)
        {
            string tagValue = GetCurrencyTagValue(handLines);

            switch (tagValue)
            {
                case "USD":
                    return Currency.USD;
                case "GBP":
                    return Currency.GBP;
                case "EUR":
                    return Currency.EURO;

                default:
                    throw new CurrencyException(handLines[0], "Unrecognized currency symbol " + tagValue);
            }
        }

        public override bool IsValidHand(string[] handLines)
        {
            // Check 1 - Are we in a Session Tag
            if (handLines[0].StartsWith("<session", StringComparison.Ordinal) == false ||
                handLines[handLines.Length - 1].StartsWith("</session", StringComparison.Ordinal) == false)
            {
                return false;
            }

            // Check 2- Do we have between 2 and 10 players?
            var playerLines = GetPlayerLinesFromHandLines(handLines);

            if (playerLines.Length < 2 || playerLines.Length > 10)
            {
                return false;
            }

            return true;
        }

        public override bool IsValidOrCanceledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;
            return IsValidHand(handLines);
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            List<HandAction> actions = new List<HandAction>();

            string[] playerLines = GetPlayerLinesFromHandLines(handLines);
            //The 2nd line after the </player> line is the beginning of the <round> rows

            int offset = 23;

            int startRow = offset + playerLines.Length + 2;

            Street currentStreet = Street.Null;

            for (int i = startRow; i < handLines.Length - 2; i++)
            {
                string handLine = handLines[i].TrimStart();

                //If we are starting a new round, update the current street 
                if (handLine[1] == 'r')
                {
                    int roundNumber = GetRoundNumberFromLine(handLine);
                    switch (roundNumber)
                    {
                        case 0:
                        case 1:
                            currentStreet = Street.Preflop;
                            break;
                        case 2:
                            currentStreet = Street.Flop;
                            break;
                        case 3:
                            currentStreet = Street.Turn;
                            break;
                        case 4:
                            currentStreet = Street.River;
                            break;
                        default:
                            throw new Exception("Encountered unknown round number " + roundNumber);
                    }
                }
                //If we're an action, parse the action and act accordingly
                else if (handLine[1] == 'a')
                {
                    HandAction action = GetHandActionFromActionLine(handLine, currentStreet);
                    actions.Add(action);
                }
            }

            //Generate the show card + winnings actions
            actions.AddRange(GetWinningAndShowCardActions(handLines));

            return actions;
        }

        private List<HandAction> GetWinningAndShowCardActions(string[] handLines)
        {
            int actionNumber = Int32.MaxValue - 100;

            PlayerList playerList = ParsePlayers(handLines);

            List<HandAction> winningAndShowCardActions = new List<HandAction>();

            foreach (Player player in playerList)
            {
                if (player.hasHoleCards)
                {
                    HandAction showCardsAction = new HandAction(player.PlayerName, HandActionType.SHOW, 0, Street.Showdown, actionNumber++);
                    winningAndShowCardActions.Add(showCardsAction);
                }
            }

            string[] playerLines = GetPlayerLinesFromHandLines(handLines);
            for (int i = 0; i < playerLines.Length; i++)
            {
                string playerLine = playerLines[i];
                decimal winnings = GetWinningsFromPlayerLine(playerLine);
                if (winnings > 0)
                {
                    string playerName = GetNameFromPlayerLine(playerLine);
                    WinningsAction winningsAction = new WinningsAction(playerName, HandActionType.WINS, winnings, 0, actionNumber++);
                    winningAndShowCardActions.Add(winningsAction);
                }
            }

            return winningAndShowCardActions;
        }

        private HandAction GetHandActionFromActionLine(string handLine, Street street)
        {
            int actionTypeNumber = GetActionTypeFromActionLine(handLine);
            string actionPlayerName = GetPlayerFromActionLine(handLine);
            decimal value = GetValueFromActionLine(handLine);
            int actionNumber = GetActionNumberFromActionLine(handLine);
            HandActionType actionType;
            switch (actionTypeNumber)
            {
                case 0:
                    actionType = HandActionType.FOLD;
                    break;
                case 1:
                    actionType = HandActionType.SMALL_BLIND;
                    break;
                case 2:
                    actionType = HandActionType.BIG_BLIND;
                    break;
                case 3:
                    actionType = HandActionType.CALL;
                    break;
                case 4:
                    actionType = HandActionType.CHECK;
                    break;
                case 5:
                    actionType = HandActionType.BET;
                    break;
                case 6:
                case 7:
                    return new AllInAction(actionPlayerName, value, street, false, actionNumber);
                case 8: //Case 8 is when a player sits out at the beginning of a hand 
                case 9: //Case 9 is when a blind isn't posted - can be treated as sitting out
                    actionType = HandActionType.SITTING_OUT;
                    break;
                case 15:
                    actionType = HandActionType.ANTE;
                    break;
                case 23:
                    actionType = HandActionType.RAISE;
                    break;
                default:
                    throw new Exception(string.Format("Encountered unknown Action Type: {0} w/ line \r\n{1}", actionTypeNumber, handLine));
            }
            return new HandAction(actionPlayerName, actionType, value, street, actionNumber);
        }

        private int GetRoundNumberFromLine(string handLine)
        {
            int startPos = handLine.IndexOf(" n") + 5;
            int endPos = handLine.IndexOf('"', startPos) - 1;
            string numString = handLine.Substring(startPos, endPos - startPos + 1);
            return Int32.Parse(numString);
        }

        private int GetActionNumberFromActionLine(string actionLine)
        {
            int actionStartPos = actionLine.IndexOf(" n", StringComparison.Ordinal) + 5;
            int actionEndPos = actionLine.IndexOf("\"", actionStartPos, StringComparison.Ordinal) - 1;
            string actionNumString = actionLine.Substring(actionStartPos, actionEndPos - actionStartPos + 1);
            return Int32.Parse(actionNumString);
        }

        private string GetPlayerFromActionLine(string actionLine)
        {
            int nameStartPos = actionLine.IndexOf(" p", StringComparison.Ordinal) + 9;
            int nameEndPos = actionLine.IndexOf("\"", nameStartPos, StringComparison.Ordinal) - 1;
            string name = actionLine.Substring(nameStartPos, nameEndPos - nameStartPos + 1);
            return name;
        }

        private decimal GetValueFromActionLine(string actionLine)
        {
            var startPos = actionLine.IndexOf("sum=", StringComparison.Ordinal) + 5;
            var endPos = actionLine.IndexOf("\"", startPos, StringComparison.Ordinal) - 1;
            var value = actionLine.Substring(startPos, endPos - startPos + 1);

            var actionValue = ParserUtils.ParseMoney(value);

            return actionValue;
        }

        private int GetActionTypeFromActionLine(string actionLine)
        {
            int actionStartPos = actionLine.IndexOf("type=", StringComparison.Ordinal) + 6;
            int actionEndPos = actionLine.IndexOf("\"", actionStartPos, StringComparison.Ordinal) - 1;
            string actionNumString = actionLine.Substring(actionStartPos, actionEndPos - actionStartPos + 1);
            return int.Parse(actionNumString);
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            /*
                <player seat="3" name="RodriDiaz3" chips="$2.25" dealer="0" win="$0" bet="$0.08" rebuy="0" addon="0" />
                <player seat="8" name="Kristi48ru" chips="$6.43" dealer="1" win="$0.23" bet="$0.16" rebuy="0" addon="0" />
                or
                <player seat="5" name="player5" chips="$100000" dealer="0" win="$0" bet="$0" />
            */

            var playerLines = GetPlayerLinesFromHandLines(handLines);

            var playerList = new PlayerList();

            for (int i = 0; i < playerLines.Length; i++)
            {
                string playerName = GetNameFromPlayerLine(playerLines[i]);
                decimal stack = GetStackFromPlayerLine(playerLines[i]);
                decimal win = GetWinningsFromPlayerLine(playerLines[i]);
                decimal bet = GetBetNumberFromPlayerLine(playerLines[i]);
                int seat = GetSeatNumberFromPlayerLine(playerLines[i]);

                // skip empty player 
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    continue;
                }

                var player = new Player(playerName, stack, seat)
                {
                    IsSittingOut = true,
                    Win = win,
                    Bet = bet
                };

                playerList.Add(player);
            }

            XDocument xDocument = GetXDocumentFromLines(handLines);
            List<XElement> actionElements =
                xDocument.Element("root").Element("session").Element("game").Elements("round").Elements("action").
                    ToList();

            foreach (Player player in playerList)
            {
                Func<XElement, bool> predicate = (action) =>
                {
                    var actionPlayerName = action.Attribute("player").Value;
                    var result = actionPlayerName.Equals(System.Net.WebUtility.HtmlDecode(player.PlayerName), StringComparison.InvariantCulture);

                    return result;
                };

                List<XElement> playerActions = actionElements.Where(predicate).ToList();

                if (playerActions.Count == 0)
                {
                    //Players are marked as sitting out by default, we don't need to update
                    continue;
                }

                //Sometimes the first and only action for a player is to sit out - we should still treat them as sitting out
                bool playerSitsOutAsAction = playerActions[0].Attribute("type").Value == "8";
                if (playerSitsOutAsAction)
                {
                    continue;
                }

                player.IsSittingOut = false;
            }

            /* 
             * Grab known hole cards for players and add them to the player
             * <cards type="Pocket" player="pepealas5">CA CK</cards>
             */

            string[] cardLines = GetCardLinesFromHandLines(handLines);

            for (int i = 0; i < cardLines.Length; i++)
            {
                string handLine = cardLines[i];
                handLine = handLine.TrimStart();

                //To make sure we know the exact character location of each card, turn 10s into Ts (these are recognized by our parser)
                //Had to change this to specific cases so we didn't accidentally change player names
                handLine = handLine.Replace("10 ", "T ");
                handLine = handLine.Replace("10<", "T<");

                //We only care about Pocket Cards
                if (handLine.IndexOf("type=\"Pocket", StringComparison.OrdinalIgnoreCase) < 1)
                {
                    continue;
                }

                //When players fold, we see a line: 
                //<cards type="Pocket" player="pepealas5">X X</cards>
                //or:
                //<cards type="Pocket" player="playername"></cards>
                //We skip these lines
                if (handLine[handLine.Length - 9] == 'X' || handLine[handLine.Length - 9] == '>')
                {
                    continue;
                }

                int playerNameStartIndex = handLine.IndexOf("player=\"", StringComparison.OrdinalIgnoreCase) + 8;

                int playerNameEndIndex = handLine.IndexOf("\"", playerNameStartIndex, StringComparison.OrdinalIgnoreCase) - 1;

                string playerName = handLine.Substring(playerNameStartIndex,
                                                       playerNameEndIndex - playerNameStartIndex + 1);

                Player player = playerList.First(p => p.PlayerName.Equals(playerName));

                int playerCardsStartIndex = handLine.IndexOf(">", StringComparison.OrdinalIgnoreCase) + 1;
                int playerCardsEndIndex = handLine.Length - 9;

                string playerCardString = handLine.Substring(playerCardsStartIndex,
                                                        playerCardsEndIndex - playerCardsStartIndex + 1);

                string[] cards = playerCardString.Split(' ');

                if (cards.Length > 1)
                {
                    player.HoleCards = HoleCards.NoHolecards(player.PlayerName);

                    foreach (string card in cards)
                    {
                        //Suit and rank are reversed in these strings, so we flip them around before adding
                        player.HoleCards.AddCard(new Card(card[1], card[0]));
                    }
                }
            }

            return playerList;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            string boardCards = string.Empty;
            /* 
             * <cards type="Flop" player="">D6 S9 S7</cards>
             * <cards type="Turn" player="">H8</cards>
             * <cards type="River" player="">D5</cards>
             * <cards type="Pocket" player="pepealas5">CA CK</cards>
             */

            string[] cardLines = GetCardLinesFromHandLines(handLines);

            for (int i = 0; i < cardLines.Length; i++)
            {
                string handLine = cardLines[i];
                handLine = handLine.TrimStart();

                //To make sure we know the exact character location of each card, turn 10s into Ts (these are recognized by our parser)
                handLine = handLine.Replace("10", "T");

                var typeIndex = handLine.IndexOf("type=\"");

                if (typeIndex < 1)
                {
                    return BoardCards.FromCards(boardCards); ;
                }

                var typeFirstLetter = handLine[typeIndex + 6];

                //The suit/ranks are reversed, so we need to reverse them when adding them to our board card string

                //Flop
                if (typeFirstLetter == 'F')
                {
                    boardCards += new Card(handLine[30], handLine[29]) + "," + new Card(handLine[33], handLine[32]) + "," + new Card(handLine[36], handLine[35]);
                }
                //Turn
                if (typeFirstLetter == 'T')
                {
                    boardCards += "," + new Card(handLine[30], handLine[29]);
                }
                //River
                if (typeFirstLetter == 'R')
                {
                    boardCards += "," + new Card(handLine[31], handLine[30]);
                    break;
                }
            }

            return BoardCards.FromCards(boardCards);
        }

        protected override string ParseHeroName(string[] handlines, PlayerList playerList)
        {
            const string tag = "<nickname>";

            for (int i = 0; i < handlines.Length; i++)
            {
                if (handlines[i][1] == 'n' && handlines[i].StartsWith(tag, StringComparison.Ordinal))
                {
                    string line = handlines[i];
                    int startindex = tag.Length;
                    int endindex = line.IndexOf("<", startindex, StringComparison.Ordinal);
                    return line.Substring(startindex, endindex - startindex);
                }
            }

            return null;
        }

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            if (GetTagLine(handLines, "tournamentname") != null)
            {
                return PokerFormat.Tournament;
            }

            return PokerFormat.CashGame;
        }

        protected override TournamentDescriptor ParseTournament(string[] handLines)
        {
            Buyin buyin = null;
            short finishPosition = 0;

            string tournamentId = string.Empty;
            string tournamentInGameId = string.Empty;
            string tournamentName = string.Empty;

            decimal winning = 0;
            decimal totalPrize = 0;

            TournamentSpeed speed = TournamentSpeed.Regular;

            DateTime startDate = DateTime.MinValue;

            foreach (var handLine in handLines)
            {
                if (handLine.StartsWith("<session", StringComparison.Ordinal))
                {
                    var sessionCodeStartIndex = handLine.IndexOf("\"", StringComparison.Ordinal) + 1;
                    var sessionCodeEndIndex = handLine.LastIndexOf("\"", StringComparison.Ordinal);

                    if (sessionCodeStartIndex == 0)
                    {
                        throw new TournamentIdException(handLines, "Couldn't get session code attribute value");
                    }

                    tournamentId = handLine.Substring(sessionCodeStartIndex, sessionCodeEndIndex - sessionCodeStartIndex);
                }

                if (handLine.StartsWith("<tournamentcode", StringComparison.Ordinal))
                {
                    tournamentId = GetTagValue(handLine);
                    continue;
                }

                if (handLine.StartsWith("<place", StringComparison.Ordinal))
                {
                    var placeText = GetTagValue(handLine);

                    short.TryParse(placeText, out finishPosition);
                }

                if (handLine.StartsWith("<buyin", StringComparison.Ordinal))
                {
                    var buyinText = GetTagValue(handLine);

                    if (!string.IsNullOrEmpty(buyinText))
                    {
                        var buyinAndRake = buyinText.Split('+');

                        var prizePoolValue = ParserUtils.ParseMoney(buyinAndRake[0]);
                        var rake = 0m;

                        if (buyinAndRake.Length > 1)
                        {
                            rake = ParserUtils.ParseMoney(buyinAndRake[1]);
                        }

                        if (buyinAndRake.Length > 2)
                        {
                            prizePoolValue += ParserUtils.ParseMoney(buyinAndRake[2]);
                        }

                        var currency = GetCurrency(handLines);

                        buyin = Buyin.FromBuyinRake(prizePoolValue, rake, currency);
                    }
                }

                if (handLine.StartsWith("<win", StringComparison.Ordinal))
                {
                    var winningText = GetTagValue(handLine);
                    ParserUtils.TryParseMoney(winningText, out winning);
                }

                if (startDate == DateTime.MinValue && handLine.StartsWith("<startdate", StringComparison.Ordinal))
                {
                    var startdateText = GetTagValue(handLine);

                    DateTime.TryParse(startdateText, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
                }

                if (handLine.StartsWith("<tablename", StringComparison.Ordinal))
                {
                    var tableNameText = GetTagValue(handLine);

                    speed = ParseTournamentSpeed(tableNameText);

                    var startIndex = tableNameText.LastIndexOf(",") + 1;

                    if (startIndex == 0)
                    {
                        continue;
                    }

                    var tournamentInGameIdText = tableNameText.Substring(startIndex, tableNameText.Length - startIndex).Trim();

                    for (var i = 0; i < tournamentInGameIdText.Length; i++)
                    {
                        if (!char.IsNumber(tournamentInGameIdText, i))
                        {
                            tournamentInGameIdText = tournamentInGameIdText.Substring(0, i + 1);
                            break;
                        }
                    }

                    tournamentInGameId = tournamentInGameIdText;
                }

                if (handLine.StartsWith("<tournamentname", StringComparison.Ordinal))
                {
                    tournamentName = GetTagValue(handLine);
                }

                if (handLine.StartsWith("<totalprize", StringComparison.Ordinal))
                {
                    var totalPrizeText = GetTagValue(handLine);
                    ParserUtils.TryParseMoney(totalPrizeText, out totalPrize);
                }
            }

            var tournamentDescriptor = new TournamentDescriptor
            {
                TournamentId = tournamentId,
                TournamentInGameId = tournamentInGameId,
                TournamentName = tournamentName,
                BuyIn = buyin,
                Winning = winning,
                FinishPosition = finishPosition,
                StartDate = startDate,
                Speed = speed,
                TotalPrize = totalPrize
            };

            return tournamentDescriptor;
        }

        private static TournamentSpeed ParseTournamentSpeed(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return TournamentSpeed.Regular;
            }

            var tableNameLower = tableName.ToLowerInvariant();

            if (tableNameLower.Contains("super turbo"))
            {
                return TournamentSpeed.SuperTurbo;
            }

            if (tableNameLower.Contains("hyper"))
            {
                return TournamentSpeed.HyperTurbo;
            }

            if (tableNameLower.Contains("turbo"))
            {
                return TournamentSpeed.Turbo;
            }

            return TournamentSpeed.Regular;
        }

        private static string GetTagValue(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var startIndex = text.IndexOf(">", StringComparison.Ordinal) + 1;
            var endIndex = text.IndexOf("<", startIndex, StringComparison.Ordinal) - 1;

            var tagValue = text.Substring(startIndex, endIndex - startIndex + 1);

            return tagValue;
        }

        private static string GetTagLine(string[] handLines, string tag)
        {
            foreach (var handLine in handLines)
            {
                if (handLine.IndexOf($"<{tag}", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return handLine;
                }
            }

            return null;
        }

        protected override void AdjustSeatTypes(HandHistory handHistory)
        {
            if (handHistory.GameDescription.IsTournament && handHistory.GameDescription.Tournament != null)
            {
                // detect table size using tournament name
                if (!string.IsNullOrEmpty(handHistory.GameDescription.Tournament.TournamentName))
                {
                    var tournamentName = handHistory.GameDescription.Tournament.TournamentName.Trim();

                    // heads up are always 2-max
                    if (tournamentName.IndexOf("Heads Up", StringComparison.OrdinalIgnoreCase) > 0 ||
                        tournamentName.IndexOf(" HU", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(2);
                        return;
                    }

                    // twister are always 3-max && premium step are always 3-max (bet365)
                    if (tournamentName.IndexOf("Premium Step", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        tournamentName.IndexOf("Twister", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(3);
                        return;
                    }

                    // "SUPER TURBO" $100 Double Up*4 
                    var playerNumberIndex = tournamentName.LastIndexOf("*", StringComparison.Ordinal);

                    if (playerNumberIndex > 0 && tournamentName.Length > playerNumberIndex)
                    {
                        var playerNumberText = tournamentName.Substring(playerNumberIndex + 1);

                        if (TryParseSeatNumber(handHistory, playerNumberText))
                        {
                            return;
                        }
                    }

                    playerNumberIndex = tournamentName.LastIndexOf(" x", StringComparison.Ordinal);

                    if (playerNumberIndex > 0 && tournamentName.Length > (playerNumberIndex + 1))
                    {
                        var playerNumberText = tournamentName.Substring(playerNumberIndex + 2);

                        if (TryParseSeatNumber(handHistory, playerNumberText))
                        {
                            return;
                        }
                    }
                }

                // detect table size using predefined numbers
                var chipsSum = handHistory.Players.Sum(x => x.StartingStack);

                var maxPlayers = 0;

                if (chipsSum == 13500)
                {
                    maxPlayers = 9;
                }
                else if (chipsSum == 9000)
                {
                    maxPlayers = 6;
                }
                else if (chipsSum == 4800)
                {
                    maxPlayers = 6;
                }

                if (maxPlayers != 0 && handHistory.Players.Count <= maxPlayers)
                {
                    handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(maxPlayers);
                    return;
                }
            }

            var seatsSet = new HashSet<int>(handHistory.Players.Select(x => x.SeatNumber));

            if (seatsSet.Contains(7))
            {
                handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(10);
            }
            else if (seatsSet.Contains(2) || seatsSet.Contains(4) || seatsSet.Contains(9))
            {
                handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(9);
            }
            else if (seatsSet.Contains(1) || seatsSet.Contains(5) || seatsSet.Contains(6) || seatsSet.Contains(10))
            {
                handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(6);
            }
        }

        protected override void AdjustTournamentSpeed(HandHistory handHistory)
        {
            if (!handHistory.GameDescription.IsTournament || handHistory.GameDescription.Tournament == null)
            {
                return;
            }

            var tournamentName = handHistory.GameDescription.Tournament.TournamentName;

            if (string.IsNullOrEmpty(tournamentName) || tournamentName.IndexOf("Double Or Nothing", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return;
            }

            var chipsSum = handHistory.Players.Sum(x => x.StartingStack);

            var initialStack = chipsSum / handHistory.GameDescription.SeatType.MaxPlayers;

            if (initialStack < 1500)
            {
                handHistory.GameDescription.Tournament.Speed = TournamentSpeed.SuperTurbo;
                return;
            }

            handHistory.GameDescription.Tournament.Speed = TournamentSpeed.Turbo;
        }

        protected override void CalculateUncalledBets(string[] handLines, HandHistory handHistory)
        {
            var uncalledBetEnabled = false;

            for (var i = 0; i < handLines.Length; i++)
            {
                if (handLines[i].IndexOf("<uncalled_bet_enabled>true", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    uncalledBetEnabled = true;
                }
                else if (handLines[i].IndexOf("</general", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    break;
                }
            }

            HandHistoryUtils.CalculateUncalledBets(handHistory, !uncalledBetEnabled);
        }

        private bool TryParseSeatNumber(HandHistory handHistory, string playerNumberText)
        {
            int playerNumber;

            if (int.TryParse(playerNumberText, out playerNumber))
            {
                if (handHistory.Players.Count <= playerNumber)
                {
                    handHistory.GameDescription.SeatType = SeatType.FromMaxPlayers(playerNumber);
                    return true;
                }
            }

            return false;
        }
    }
}