//-----------------------------------------------------------------------
// <copyright file="WinningPokerSnG2FastParserImpl.cs" company="Ace Poker Solutions">
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
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.FastParsing;
using HandHistories.Parser.Utils.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HandHistories.Objects.Hand;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Extensions;
using System.Text;

namespace HandHistories.Parser.Parsers.FastParser.Winning
{
    internal class WinningPokerSnG2FastParserImpl : HandHistoryParserFastImpl
    {
        private const string GameLinePrefix = "Game Hand #";

        private int TournamentIdStartindex = -1;

        public override EnumPokerSites SiteName
        {
            get
            {
                return EnumPokerSites.WinningPokerNetwork;
            }
        }

        public override bool RequiresAdjustedRaiseSizes
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

        /// <summary>
        /// Splits the specified text on multiple hands
        /// </summary>
        /// <param name="rawHandHistories">Text to split</param>
        /// <returns>The list of texts of hands</returns>
        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            if (string.IsNullOrWhiteSpace(rawHandHistories))
            {
                return new string[0];
            }

            rawHandHistories = rawHandHistories.Replace("\r", string.Empty);

            var handsDraft = rawHandHistories
                .LazyStringSplit("\n\n")
                .Where(s => string.IsNullOrWhiteSpace(s) == false && s.Equals("\r\n") == false && !s.StartsWith("--"))
                .ToArray();

            var hands = new List<string>();

            var tournamentTitle = string.Empty;

            var gameInfoHeader = string.Empty;

            // need to add tournament id to game information
            for (var i = 0; i < handsDraft.Length; i++)
            {
                if (handsDraft[i].StartsWith("<Game Information>", StringComparison.OrdinalIgnoreCase))
                {
                    gameInfoHeader = handsDraft[i].Trim();
                    continue;
                }

                if (!string.IsNullOrEmpty(gameInfoHeader))
                {
                    if (handsDraft[i].StartsWith("<Hand History>", StringComparison.OrdinalIgnoreCase))
                    {
                        handsDraft[i] = $"{gameInfoHeader}{Environment.NewLine}{Environment.NewLine}{handsDraft[i]}";
                    }
                    else
                    {
                        handsDraft[i] = $"{gameInfoHeader}{Environment.NewLine}{Environment.NewLine}<Hand History>{Environment.NewLine}{handsDraft[i]}";
                    }
                }

                hands.Add(handsDraft[i]);
            }

            return hands;
        }

        /// <summary>
        /// Determines if the specified lines of hand are lines of summary hand
        /// </summary>      
        protected override bool IsSummaryHand(string[] handLines)
        {
            return false;
        }

        /// <summary>
        /// Determines whenever hand is valid
        /// </summary>     
        public override bool IsValidHand(string[] handLines)
        {
            return handLines.Length > 0 && handLines.Count(x => x.StartsWith(GameLinePrefix, StringComparison.OrdinalIgnoreCase)) == 1;
        }

        /// <summary>
        /// Determines whenever hand is valid or canceled
        /// </summary>     
        public override bool IsValidOrCanceledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;
            return IsValidHand(handLines);
        }

        /// <summary>
        /// Parses the specified hand to get date and time
        /// </summary>     
        protected override DateTime ParseDateUtc(string[] handLines)
        {
            // Game Hand #12908866 - Tournament #8034884 - Holdem(No Limit) - Level 1 (10.00/20.00)- 2017/05/08 08:32:30 UTC
            var line = GetGameHandLine(handLines);

            var regex = new Regex(@"(?<year>\d{4})/(?<month>\d{2})/(?<day>\d{2}) (?<hour>\d{1,2}):(?<minute>\d{2}):(?<second>\d{2})");

            var match = regex.Match(line);

            if (!match.Success)
            {
                throw new ParseHandDateException("Date couldn't be recognized.", line);
            }

            var dateString = match.Value;

            var year = FastInt.Parse(dateString);
            var month = FastInt.Parse(dateString, 5);
            var day = FastInt.Parse(dateString, 8);
            var hour = FastInt.Parse(dateString, 11);

            var minuteStartIndex = dateString.IndexOf(':', 12) + 1;

            var minute = FastInt.Parse(dateString, minuteStartIndex);
            var second = FastInt.Parse(dateString, minuteStartIndex + 3);

            var dateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

            return dateTime;
        }

        #region Game Descriptor parsers

        /// <summary>
        /// Parses the specified hand to get <see cref="PokerFormat"/> 
        /// </summary>     
        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            var tournamentStartIndex = GetGameHandLine(handLines).IndexOf("Tournament #", StringComparison.OrdinalIgnoreCase);

            if (tournamentStartIndex > 0)
            {
                TournamentIdStartindex = tournamentStartIndex + 12;

                return PokerFormat.Tournament;
            }

            return PokerFormat.CashGame;
        }

        /// <summary>
        /// Parses the specified hand to get <see cref="GameType"/> 
        /// </summary>    
        protected override GameType ParseGameType(string[] handLines)
        {
            // currently only SnG2 games are only No Limit Holdem and Pot Limit Omaha
            var gameLine = GetGameHandLine(handLines);

            return gameLine.ContainsIgnoreCase("Pot Limit") && gameLine.ContainsIgnoreCase("Omaha") ?
                GameType.PotLimitOmaha :
                GameType.NoLimitHoldem;
        }

        /// <summary>
        /// Parses the specified hand to get <see cref="Limit"/> 
        /// </summary>    
        protected override Limit ParseLimit(string[] handLines)
        {
            var gameLine = GetGameHandLine(handLines);

            var startIndex = gameLine.LastIndexOf('(') + 1;
            var lastIndex = gameLine.LastIndexOf(')') - 1;

            var limitSubstring = gameLine.Substring(startIndex, lastIndex - startIndex + 1);

            var slashIndex = limitSubstring.IndexOf('/');

            var smallBlindText = limitSubstring.Substring(0, slashIndex);
            var bigBlindText = limitSubstring.Substring(slashIndex + 1);

            var smallBlind = ParserUtils.ParseMoney(smallBlindText);
            var bigBlind = ParserUtils.ParseMoney(bigBlindText);

            return Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, Currency.All);
        }

        /// <summary>
        /// Parses the specified hand to get <see cref="TableType"/> 
        /// </summary>    
        protected override TableType ParseTableType(string[] handLines)
        {
            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        /// <summary>
        /// Parses the specified hand to get <see cref="SeatType"/> 
        /// </summary>    
        protected override SeatType ParseSeatType(string[] handLines)
        {
            var handLine = handLines.FirstOrDefault(x => x.StartsWith("Table '", StringComparison.OrdinalIgnoreCase));

            if (handLine == null)
            {
                throw new SeatTypeException(handLines, "Seat type couldn't be recognized");
            }

            var secondDash = handLine.LastIndexOf('\'');

            var maxPlayers = FastInt.Parse(handLine[secondDash + 2], 1);

            // can't have 1max so must be 10max
            if (maxPlayers == 1)
            {
                maxPlayers = 10;
            }

            return SeatType.FromMaxPlayers(maxPlayers);
        }

        #endregion

        /// <summary>
        /// Parses the specified hand to get <see cref="TournamentDescriptor"/> 
        /// </summary>     
        protected override TournamentDescriptor ParseTournament(string[] handLines)
        {
            // Game Hand #12908866 - Tournament #8034884 - Holdem(No Limit) - Level 1 (10.00/20.00)- 2017/05/08 08:32:30 UTC
            var gameLine = GetGameHandLine(handLines);

            var endIndex = gameLine.IndexOf(" -", TournamentIdStartindex, StringComparison.OrdinalIgnoreCase);

            var tournamentId = gameLine.Substring(TournamentIdStartindex, endIndex - TournamentIdStartindex);

            // Title: SNG 2.0 $5.50
            // Title: $2 Jackpot Holdem
            var titleLine = handLines.FirstOrDefault(x => x.StartsWith("Title: ", StringComparison.OrdinalIgnoreCase));

            var tournamentName = string.Empty;

            if (titleLine != null)
            {
                tournamentName = titleLine.Substring(7, titleLine.Length - 8);
            }
            else
            {
                tournamentName = gameLine.ContainsIgnoreCase(" *** ") ?
                    $"Jackpot #{tournamentId}" :
                    $"SNG 2.0 #{tournamentId}";
            }

            var isJackpot = tournamentName.ContainsIgnoreCase("Jackpot");

            var buyin = isJackpot ? ParseJackpotBuyin(handLines, tournamentName) : ParseBuyin(handLines);

            var totalPrize = isJackpot ? 2 * (buyin.PrizePoolValue + buyin.Rake) : 0;

            var tournamentDescriptor = new TournamentDescriptor
            {
                TournamentId = tournamentId,
                Speed = TournamentSpeed.Regular,
                TournamentName = tournamentName,
                BuyIn = buyin,
                TotalPrize = totalPrize
            };

            return tournamentDescriptor;
        }

        private Buyin ParseJackpotBuyin(string[] handLines, string tournamentName)
        {
            var handLine = handLines.FirstOrDefault(x => x.ContainsIgnoreCase(" *** Summary:"));

            var buyinWithRake = 0m;

            if (handLine == null)
            {
                var spaceIndex = tournamentName.IndexOf(' ');

                var buyinText = tournamentName.Substring(0, spaceIndex);

                ParserUtils.TryParseMoney(buyinText, out buyinWithRake);
            }
            else
            {
                var buyinIndex = handLine.IndexOf("TournamentBuyIn: ", StringComparison.OrdinalIgnoreCase);

                if (buyinIndex > 0)
                {
                    var buyinText = handLine.Substring(buyinIndex + 17);

                    ParserUtils.TryParseMoney(buyinText, out buyinWithRake);
                }
            }

            var buyin = Math.Round(buyinWithRake / 1.06m, 2);
            var rake = buyinWithRake - buyin;

            return Buyin.FromBuyinRake(buyin, rake, Currency.USD);
        }

        /// <summary>
        /// Parses the specified hand to get <see cref="Buyin"/> 
        /// </summary>     
        protected override Buyin ParseBuyin(string[] handLines)
        {
            // Title 'SNG 2.0 $5.50'
            var handLine = handLines.FirstOrDefault(x => x.StartsWith("Title: ", StringComparison.OrdinalIgnoreCase));

            if (handLine == null)
            {
                return Buyin.AllBuyin();
            }

            var lastSpaceIndex = handLine.LastIndexOf(' ');

            if (lastSpaceIndex < 0)
            {
                return Buyin.AllBuyin();
            }

            var buyInAndRakeText = handLine.Substring(lastSpaceIndex + 1).TrimEnd('\'');

            if (string.IsNullOrEmpty(buyInAndRakeText))
            {
                return Buyin.AllBuyin();
            }

            if (!char.IsDigit(buyInAndRakeText[0]))
            {
                buyInAndRakeText = buyInAndRakeText.Substring(1);
            }

            var buyInAndRake = ParserUtils.ParseMoney(buyInAndRakeText);

            var buyIn = buyInAndRake / 1.1m;
            var rake = buyInAndRake - buyIn;

            return Buyin.FromBuyinRake(buyIn, rake, Currency.USD);
        }

        /// <summary>
        /// Parses the specified hand to get id of hand
        /// </summary>  
        protected override long ParseHandId(string[] handLines)
        {
            // Game Hand #12908866 - Tournament #8034884 - Holdem(No Limit) - Level 1 (10.00/20.00)- 2017/05/08 08:32:30 UTC
            var gameLine = GetGameHandLine(handLines);

            var startIndexOfHandId = gameLine.IndexOf("#") + 1;
            var endIndexOfHandId = gameLine.IndexOf(" -", startIndexOfHandId);

            var handId = gameLine.Substring(startIndexOfHandId, endIndexOfHandId - startIndexOfHandId);

            return long.Parse(handId);
        }

        /// <summary>
        /// Parses the specified hand to get the name of table
        /// </summary>  
        protected override string ParseTableName(string[] handLines)
        {
            // Table '1' 9-max Seat #9 is the button
            var handLine = handLines.FirstOrDefault(x => x.StartsWith("Table '", StringComparison.OrdinalIgnoreCase));

            if (handLine == null)
            {
                throw new TableNameException(handLines, "Table name couldn't be recognized");
            }

            const int firstDashIndex = 7;
            var secondDash = handLine.LastIndexOf('\'');

            return handLine.Substring(firstDashIndex, secondDash - firstDashIndex);
        }

        /// <summary>
        /// Parses the specified hand to get the dealer position
        /// </summary>  
        protected override int ParseDealerPosition(string[] handLines)
        {
            // Table '1' 9-max Seat #9 is the button
            var handLine = handLines.FirstOrDefault(x => x.StartsWith("Table '", StringComparison.OrdinalIgnoreCase));

            if (handLine == null)
            {
                throw new DealerPositionException(handLines, "Table name couldn't be recognized");
            }

            var startIndex = handLine.LastIndexOf('#') + 1;

            return FastInt.Parse(handLine, startIndex);
        }

        /// <summary>
        /// Parses the specified hand to get the community cards
        /// </summary>  
        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            // Expect the end of the hand to have something like this:
            // *** SUMMARY ***
            // Total pot 3030.00 | Rake 0.00
            // Board [Ts 7c 5c 3d Qc]
            // Seat 1: Villain1 (small blind) folded on the Pre-Flop

            var boardCards = BoardCards.ForPreflop();

            for (int lineNumber = handLines.Length - 2; lineNumber >= 0; lineNumber--)
            {
                var line = handLines[lineNumber];

                if (line[0] == '*')
                {
                    return boardCards;
                }

                if (line[0] == 'B')
                {
                    var firstSquareBracket = line.IndexOf("[", StringComparison.Ordinal) + 1;
                    var lastSquareBracket = line.Length - 1;

                    return BoardCards.FromCards(line.Substring(firstSquareBracket, lastSquareBracket - firstSquareBracket));
                }
            }

            throw new CardException(string.Empty, "Read through hand backwards and didn't find a board or summary.");
        }

        /// <summary>
        /// Parses the specified hand to get <see cref="PlayerList"/>
        /// </summary>  
        protected override PlayerList ParsePlayers(string[] handLines)
        {
            var playerList = new PlayerList();
            var lastLineRead = -1;
            var foundSeats = false;

            // We start on line index 2 as first 2 lines are table and limit info.
            for (int lineNumber = 2; lineNumber < handLines.Length - 1; lineNumber++)
            {
                var line = handLines[lineNumber];

                if (!foundSeats && !line.StartsWith("Seat ", StringComparison.OrdinalIgnoreCase) && line[6] != ':')
                {
                    continue;
                }

                if (foundSeats && !line.StartsWith("Seat ", StringComparison.OrdinalIgnoreCase))
                {
                    lastLineRead = lineNumber;
                    break;
                }

                foundSeats = true;

                var endChar = line[line.Length - 1];

                // Seat 1: Villain1 (1500.00)
                // Seat 1: Villain1 (1500.00) is sitting out
                if (endChar != ')' && endChar != 't')
                {
                    lastLineRead = lineNumber;
                    break;
                }

                // seat info expected in format: 
                // Seat 1: Villain1 (1500.00)
                const int seatNumberStartIndex = 4;
                var spaceIndex = line.IndexOf(' ', seatNumberStartIndex);
                var colonIndex = line.IndexOf(':', spaceIndex + 1);
                var seatNumber = FastInt.Parse(line, spaceIndex + 1);

                // we need to find the ( before the number. players can have ( in their name so we need to go backwards and skip the last one
                var openParenIndex = line.LastIndexOf('(');

                //Seat 2: Villain1 (1500.00) out of hand (moved from another table into small blind)
                if (line[openParenIndex + 1] == 'm')
                {
                    line = line.Remove(openParenIndex);
                    openParenIndex = line.LastIndexOf('(');
                }

                var closeParenIndex = line.IndexOf(')', openParenIndex);

                var playerName = line.Substring(colonIndex + 2, (openParenIndex - 1) - (colonIndex + 2));

                var stackString = line.Substring(openParenIndex + 1, closeParenIndex - (openParenIndex + 1));
                var stack = ParserUtils.ParseMoney(stackString);

                playerList.Add(new Player(playerName, stack, seatNumber));
            }

            if (lastLineRead == -1)
            {
                throw new PlayersException(string.Empty, "Didn't break out of the seat reading block.");
            }

            // Looking for the showdown info which looks like this      
            // *** SHOW DOWN ***
            // Main pot 3030.00
            // Villain4 shows [Jd 7d] (a pair of Sevens [7d 7c Qc Jd Ts])
            // Villain5 shows [As Ah] (a pair of Aces [As Ah Qc Ts 7c])
            // Villain5 collected 3030.00 from main pot
            // *** SUMMARY ***

            int summaryIndex = GetSummaryStartIndex(handLines, lastLineRead);
            int showDownIndex = GetShowDownStartIndex(handLines, lastLineRead, summaryIndex);

            //Starting from the bottom to parse faster
            if (showDownIndex != -1)
            {
                for (int lineNumber = showDownIndex + 1; lineNumber < summaryIndex; lineNumber++)
                {
                    // Villain4 shows [7h 6h] (a full house, Sevens full of Jacks)
                    // Villain2 mucks hand
                    // Villain1 collected 550.00 from main pot
                    var line = handLines[lineNumber];

                    var lastChar = line[line.Length - 1];

                    if (lastChar == '*')
                    {
                        break;
                    }

                    if (lastChar == 'd' || lastChar == 't' || lastChar == '"')
                    {
                        continue;
                    }

                    var lastSquareBracket = line.LastIndexLoopsBackward(']', line.Length - 3);

                    if (lastSquareBracket == -1)
                    {
                        continue;
                    }

                    var firstSquareBracket = line.LastIndexOf('[', lastSquareBracket);

                    // can show single cards:
                    // Villain1 shows [Qc]
                    if (line[firstSquareBracket + 3] == ']')
                    {
                        continue;
                    }

                    var lastNameIndex = line.LastIndexOf(' ', firstSquareBracket - 2);

                    var playerName = line.Substring(0, lastNameIndex);

                    if (playerList[playerName].HoleCards != null)
                    {
                        continue;
                    }

                    var cards = line.Substring(firstSquareBracket + 1, lastSquareBracket - (firstSquareBracket + 1));

                    playerList[playerName].HoleCards = HoleCards.FromCards(cards);
                }
            }

            return playerList;
        }

        /// <summary>
        /// Parses the specified hand to get the list of <see cref="HandAction"/>
        /// </summary>  
        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            var actionIndex = GetFirstActionIndex(handLines);

            var handActions = new List<HandAction>(handLines.Length - actionIndex);

            actionIndex = ParseBlindActions(handLines, ref handActions, actionIndex);

            actionIndex = ParseGameActions(handLines, ref handActions, actionIndex, out Street currentStreet);

            if (currentStreet == Street.Showdown)
            {
                ParseShowDown(handLines, ref handActions, actionIndex, gameType);
                ParseSummary(handLines, ref handActions, actionIndex, gameType);
            }

            return handActions;
        }

        /// <summary>
        /// Parses the specified hand to get the name of Hero
        /// </summary>     
        protected override string ParseHeroName(string[] handlines, PlayerList playerList)
        {
            var isHoleCards = false;

            for (int i = 0; i < handlines.Length; i++)
            {
                string line = handlines[i];

                if (line.StartsWith("*** HOLE", StringComparison.OrdinalIgnoreCase))
                {
                    isHoleCards = true;
                }

                if (line.StartsWith("Dealt to ", StringComparison.Ordinal) && isHoleCards)
                {
                    int startIndex = line.IndexOf('[');

                    var heroName = line.Substring(9, startIndex - 10);

                    if (playerList != null && playerList[heroName] != null && playerList[heroName].HoleCards == null)
                    {
                        var cards = line.Substring(startIndex + 1, line.Length - startIndex - 2)
                                    .Replace("[", string.Empty).Replace("]", string.Empty).Replace(",", " ");

                        playerList[heroName].HoleCards = HoleCards.FromCards(cards);
                    }

                    return heroName;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the index of Summary line
        /// </summary> 
        private int GetSummaryStartIndex(string[] handLines, int lastLineRead)
        {
            for (int lineNumber = handLines.Length - 3; lineNumber > lastLineRead; lineNumber--)
            {
                var line = handLines[lineNumber];

                if (line[0] != 'S' &&
                    line[0] != 'T' &&
                    line[0] != 'B')
                {
                    return lineNumber;
                }
            }

            // Summary must exist or it is not a valid hand
            throw new IndexOutOfRangeException("Could not find *** SUMMARY ***");
        }

        /// <summary>
        /// Gets the index of Showdown line
        /// </summary>       
        private int GetShowDownStartIndex(string[] handLines, int lastLineRead, int summaryIndex)
        {
            for (int i = lastLineRead; i < summaryIndex; i++)
            {
                string line = handLines[i];

                char lastChar = line[line.Length - 1];

                if (lastChar != '*')
                {
                    continue;
                }

                if (line.StartsWith("*** SHOW", StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the index of the first action
        /// </summary>                
        private int GetFirstActionIndex(string[] handLines)
        {
            for (int lineNumber = 2; lineNumber < handLines.Length; lineNumber++)
            {
                // Seat 9: Villain5 (5496.00)
                // Villain1 posts the small blind 10.00
                var line = handLines[lineNumber];

                // dealt to?
                if (line[0] != 'S' || line[line.Length - 1] != ')')
                {
                    return lineNumber;
                }
            }

            throw new HandActionException(string.Empty, "Couldn't find the first action");
        }

        /// <summary>
        /// Parses all blind actions from the specified index, returns the index where HandActions will start
        /// </summary>            
        public int ParseBlindActions(string[] handLines, ref List<HandAction> handActions, int firstActionIndex)
        {
            // required for distinction between small blind/big blind/posts 
            var smallBlind = false;
            var bigBlind = false;

            for (int i = firstActionIndex; i < handLines.Length; i++)
            {
                var line = handLines[i];

                var lastChar = line[line.Length - 1];

                switch (lastChar)
                {
                    // blind actions (BB, SB ANTE) may end in a number during the blinds
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;

                    case 'n':
                        // Villain4 posts ante 15.00 and is all-in
                        if (line[line.Length - 2] == 'o' || line[line.Length - 3] == 'o')
                        {
                            continue;
                        }

                        break;

                    // *** HOLE CARDS ***
                    case '*':
                        return i + 1;

                    // Dealt to Villain1 [5d]
                    // *** FLOP *** [6d 7c 6h]
                    // *** TURN *** [6d 7c 6h] [2s]
                    // *** RIVER *** [6d 7c 6h 2s] [Qc]
                    case ']':

                        // Dealt to Villain1 [5d]
                        if (line.StartsWith("Dealt to", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        throw new HandActionException(string.Join(Environment.NewLine, handLines), "Unexpected Line: " + line);

                    default:
                        continue;
                }

                var action = ParsePostingActionLine(line, smallBlind, bigBlind);

                if (action != null)
                {
                    if (action.HandActionType == HandActionType.SMALL_BLIND)
                    {
                        smallBlind = true;
                    }

                    if (action.HandActionType == HandActionType.BIG_BLIND)
                    {
                        bigBlind = true;
                    }

                    handActions.Add(action);
                }

            }

            throw new HandActionException(string.Join(Environment.NewLine, handLines), "No end of posting actions");
        }

        /// <summary>
        /// Parses the specified hand to get the posting <see cref="HandAction"/>
        /// </summary>  
        private HandAction ParsePostingActionLine(string actionLine, bool smallBlindPosted, bool bigBlindPosted)
        {
            var isAllIn = false;

            // Expect lines to look like one of these:
            // Villain3 posts ante 25.00
            // Villain4 posts ante 15.00 and is all-in            
            // Villain3 posts the small blind 100.00
            // Villain4 posts the big blind 0.00 and is all-in

            // the column w/ the & is a unique identifier            
            char lastChar = actionLine[actionLine.Length - 1];

            // Villain4 posts the big blind 0.00 and is all-in
            if (lastChar == 'n')
            {
                isAllIn = true;
                actionLine = actionLine.Substring(0, actionLine.Length - 14);
            }

            // Villain3 is connected 
            if (lastChar == 'd')
            {
                return null;
            }

            HandActionType handActionType;

            int actionStartIndex = -1;
            int firstDigitIndex;

            if ((actionStartIndex = actionLine.IndexOf("posts the small blind", StringComparison.OrdinalIgnoreCase)) > 0)
            {
                handActionType = smallBlindPosted ? HandActionType.POSTS : HandActionType.SMALL_BLIND;
                firstDigitIndex = actionStartIndex + 21;
            }
            else if ((actionStartIndex = actionLine.IndexOf("posts the big blind", StringComparison.OrdinalIgnoreCase)) > 0)
            {
                handActionType = bigBlindPosted ? HandActionType.POSTS : HandActionType.BIG_BLIND;
                firstDigitIndex = actionStartIndex + 19;
            }
            else if ((actionStartIndex = actionLine.IndexOf("posts ante", StringComparison.OrdinalIgnoreCase)) > 0)
            {
                handActionType = HandActionType.ANTE;
                firstDigitIndex = actionStartIndex + 10;
            }
            else
            {
                return null;
            }

            var playerName = actionLine.Substring(0, actionStartIndex - 1);

            decimal amount = ParserUtils.ParseMoney(actionLine.Substring(firstDigitIndex, actionLine.Length - firstDigitIndex));

            return new HandAction(playerName, handActionType, amount, Street.Preflop, isAllIn);
        }

        /// <summary>
        /// Parses the specified hand to get the <see cref="HandAction"/> of the game
        /// </summary>  
        public int ParseGameActions(string[] handLines, ref List<HandAction> handActions, int firstActionIndex, out Street currentStreet)
        {
            currentStreet = Street.Preflop;

            for (int lineNumber = firstActionIndex; lineNumber < handLines.Length; lineNumber++)
            {
                string handLine = handLines[lineNumber];

                try
                {
                    bool isFinished = ParseLine(handLine, ref currentStreet, ref handActions);

                    if (isFinished)
                    {
                        return lineNumber + 1;
                    }
                }
                catch
                {
                    continue;
                }
            }

            throw new InvalidHandException(string.Join(Environment.NewLine, handLines));
        }

        /// <summary>
        /// Parses the action line to add the <see cref="HandAction"/> to the list of actions of the game
        /// </summary>  
        private bool ParseLine(string line, ref Street currentStreet, ref List<HandAction> handActions)
        {
            // skip main pot lines
            if (line.StartsWith("Main pot ", StringComparison.Ordinal))
            {
                return false;
            }

            // We filter out only possible line endings we want
            var lastChar = line[line.Length - 1];

            // Uncalled bet lines look like:
            // Uncalled bet (500.00) returned to Villain5
            if (line.Length > 29 && line[13] == '(')
            {
                handActions.Add(ParseUncalledBetLine(line, currentStreet));
                currentStreet = Street.Showdown;
                return true;
            }

            switch (lastChar)
            {
                // All actions with an amount(BET, CALL, RAISE)
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 's':
                    break;

                // Villain5 will be allowed to play after the button
                // Villain5 raises 8.94 to 10.94 and is all-in
                // Villain5 Re-join
                case 'n':
                    if (line.EndsWith("on", StringComparison.Ordinal) || line.EndsWith("oin", StringComparison.Ordinal))
                    {
                        return false;
                    }

                    break;

                // Villain5 has timed out
                // Villain5 was removed from the table for failing to post
                case 't':
                    if (line[line.Length - 2] == 'u' || line[line.Length - 2] == 's')
                    {
                        return false;
                    }

                    break;

                // *** SUMMARY ***
                // *** SHOW DOWN ***
                case '*':
                // *** FLOP *** [Qs Js 3h]
                // Dealt to PS_Hero [4s 7h]
                case ']':
                    char firstChar = line[0];

                    if (firstChar == '*')
                    {
                        return ParseCurrentStreet(line, ref currentStreet);
                    }

                    return false;

                // Villain5 raises 2.50 to 6.50 and has reached the $10 cap
                case 'p':
                    break;
                case 'w':
                    return false;

                default:
                    return false;
            }

            // zeranex88 joins the table at seat #5 
            if (line[line.Length - 2] == '#')
            {
                // joins action
                // don't bother parsing it or adding it
                return false;
            }

            var isAllIn = line[line.Length - 1] == 'n';

            // Remove the  ' and is all-in' and just proceed like normal
            if (isAllIn)
            {
                line = line.Remove(line.Length - 14);
            }

            int actionIndex = -1;
            int firstDigitIndex = -1;
            decimal amount = 0m;
            HandActionType actionType;

            if ((actionIndex = line.LastIndexOf("folds")) > 0)
            {
                actionType = HandActionType.FOLD;
            }
            else if ((actionIndex = line.LastIndexOf("checks")) > 0)
            {
                actionType = HandActionType.CHECK;
            }
            else if ((actionIndex = line.LastIndexOf("calls")) > 0)
            {
                actionType = HandActionType.CALL;
                firstDigitIndex = actionIndex + 6;
            }
            else if ((actionIndex = line.LastIndexOf("bets")) > 0)
            {
                actionType = HandActionType.BET;
                firstDigitIndex = actionIndex + 5;
            }
            else if ((actionIndex = line.LastIndexOf("raises")) > 0)
            {
                actionType = HandActionType.RAISE;
                firstDigitIndex = line.LastIndexOf(' ') + 1;
            }
            else
            {
                return false;
            }

            var playerName = line.Substring(0, actionIndex - 1);

            if (firstDigitIndex > 0)
            {
                amount = ParserUtils.ParseMoney(line.Substring(firstDigitIndex, line.Length - firstDigitIndex));
            }

            var handAction = new HandAction(playerName, actionType, amount, currentStreet, isAllIn);

            handActions.Add(handAction);

            return false;
        }

        /// <summary>
        /// Parses action line to get uncalled bet <see cref="HandAction"/>
        /// </summary>       
        private HandAction ParseUncalledBetLine(string actionLine, Street currentStreet)
        {
            // Uncalled bet lines look like:
            // Uncalled bet (500.00) returned to Villain5            
            var closeParenIndex = actionLine.IndexOf(')', 14);

            var amount = ParserUtils.ParseMoney(actionLine.Substring(14, closeParenIndex - 14));

            var firstLetterOfName = closeParenIndex + 14;

            var playerName = actionLine.Substring(firstLetterOfName, actionLine.Length - firstLetterOfName);

            return new HandAction(playerName, HandActionType.UNCALLED_BET, amount, currentStreet);
        }

        /// <summary>
        /// Parses the line of hand to get the current <see cref="Street"/>
        /// </summary>       
        private bool ParseCurrentStreet(string line, ref Street currentStreet)
        {
            char typeOfEventChar = line[7];

            // this way we implement the collected lines in the regular showdown for the hand
            // both showdowns will be included in the regular hand actions, so the regular hand actions can be used for betting/pot/rake verification
            // might be readjusted so that only the first one is the regular hand actions, and the second one goes to run it twice

            // *** FIRST FLOP
            // *** FIRST TURN
            if (typeOfEventChar == 'S')
            {
                typeOfEventChar = line[13];
            }

            // *** SECOND FLOP
            // *** SECOND TURN
            if (typeOfEventChar == 'O')
            {
                typeOfEventChar = line[14];
            }

            switch (typeOfEventChar)
            {
                case 'P':
                    currentStreet = Street.Flop;
                    return false;
                case 'N':
                    currentStreet = Street.Turn;
                    return false;
                case 'E':
                    currentStreet = Street.River;
                    return false;
                case 'W':
                    currentStreet = Street.Showdown;
                    return true;
                case 'M':
                    return true;
                default:
                    throw new HandActionException(line, "Unrecognized line w/ a *:" + line);
            }
        }

        /// <summary>
        /// Parses showdown lines 
        /// </summary>     
        private void ParseShowDown(string[] handLines, ref List<HandAction> handActions, int actionIndex, GameType gameType)
        {
            for (int i = actionIndex; i < handLines.Length; i++)
            {
                var line = handLines[i];

                var lastChar = line[line.Length - 1];

                switch (lastChar)
                {
                    // Villain4 collected 90.00 from main pot
                    // Villain4 sits out 
                    case 't':
                        if (line.EndsWith("pot", StringComparison.Ordinal))
                        {
                            handActions.Add(ParseCollectedLine(line, Street.Showdown));
                        }

                        continue;

                    // Villain4 collected 850.00 from side pot-2
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':

                        // skip lines like
                        // Hudison747 was removed from the table for failing to post
                        if (line[line.Length - 1] == 't' && line[line.Length - 2] == 's')
                            continue;

                        if (line[line.Length - 2] == '-')
                        {
                            handActions.Add(ParseCollectedLine(line, Street.Showdown));
                        }

                        continue;

                    //*** FLOP *** [6d 7c 6h]
                    //*** TURN *** [6d 7c 6h] [2s]
                    //*** RIVER *** [6d 7c 6h 2s] [Qc]
                    case ']':
                        continue;

                    //*** SUMMARY ***
                    //*** SHOW DOWN ***
                    //*** FIRST SHOW DOWN ***
                    //*** SECOND SHOW DOWN ***
                    case '*':
                        char starId = line[5];

                        switch (starId)
                        {
                            //*** SHOW DOWN ***
                            //*** FIRST SHOW DOWN ***
                            case 'H':
                            case 'I':
                                continue;

                            //*** SUMMARY ***
                            case 'U':
                                return;
                            //Skipping Second showdown, that is parsed with ParseRunItTwice
                            //*** SECOND SHOW DOWN ***
                            case 'E':
                                continue;

                            default:
                                throw new ArgumentException("Unhandled line: " + line);
                        }

                    //No low hand qualified
                    //EASSA: mucks hand
                    case 'd':
                        if (line.EndsWith("hand", StringComparison.Ordinal))
                        {
                            break;
                        }
                        continue;

                    //Player1: shows [6d Ad] (a pair of Sixes)
                    case ')':
                        break;

                    //skip unidentified actions such as
                    //leaves table
                    //stands up
                    default:
                        continue;
                }

                var action = ParseMiscShowdownLine(line, gameType);
                handActions.Add(action);
            }
        }

        /// <summary>
        /// Parses collected lines
        /// </summary>       
        private HandAction ParseCollectedLine(string actionLine, Street currentStreet)
        {
            // 0 = main pot
            int potNumber = 0;
            HandActionType handActionType = HandActionType.WINS;

            // check for side pot lines like
            // CinderellaBD collected $7 from side pot-2
            if (actionLine[actionLine.Length - 2] == '-')
            {
                handActionType = HandActionType.WINS_SIDE_POT;
                potNumber = int.Parse(actionLine[actionLine.Length - 1].ToString());

                // This removes the ' from side pot-2' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 16);
            }
            // check for a side pot line like
            // bozzoTHEclow collected $136.80 from side pot
            else if (actionLine[actionLine.Length - 8] == 's')
            {
                potNumber = 1;
                handActionType = HandActionType.WINS_SIDE_POT;
                // This removes the ' from side pot' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 14);
            }
            // check for main pot line like 
            //bozzoTHEclow collected $245.20 from main pot
            else if (actionLine[actionLine.Length - 8] == 'm')
            {
                // This removes the ' from main pot' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 14);
            }
            // otherwise is basic line like
            // alecc frost collected $1.25 from pot
            else
            {
                // This removes the ' from pot' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 9);
            }

            // Collected bet lines look like:
            // alecc frost collected $1.25 from pot
            int firstAmountDigit = actionLine.LastIndexOf(' ') + 1;
            decimal amount = ParserUtils.ParseMoney(actionLine.Substring(firstAmountDigit, actionLine.Length - firstAmountDigit));

            // 12 characters from first digit to the space in front of collected
            string playerName = actionLine.Substring(0, firstAmountDigit - 11);

            return new WinningsAction(playerName, handActionType, amount, potNumber);
        }

        /// <summary>
        /// Parses miscellaneous showdown lines
        /// </summary>        
        private HandAction ParseMiscShowdownLine(string actionLine, GameType gameType = GameType.Unknown)
        {
            var actionIndex = -1;
            HandActionType actionType;

            if ((actionIndex = actionLine.LastIndexOf("does not show", StringComparison.OrdinalIgnoreCase)) > 0)
            {
                actionType = HandActionType.MUCKS;
            }
            else if ((actionIndex = actionLine.LastIndexOf("mucks hand", StringComparison.OrdinalIgnoreCase)) > 0)
            {
                actionType = HandActionType.MUCKS;
            }
            else if ((actionIndex = actionLine.LastIndexOf("shows", StringComparison.OrdinalIgnoreCase)) > 0)
            {
                actionType = HandActionType.SHOW;
            }
            else
            {
                throw new HandActionException(actionLine, "ParseMiscShowdownLine: Unrecognized line '" + actionLine + "'");
            }

            var playerName = actionLine.Substring(0, actionIndex - 1);

            return new HandAction(playerName, actionType, Street.Showdown);
        }

        /// <summary>
        /// Parses summary to complete hand 
        /// </summary>        
        private void ParseSummary(string[] handLines, ref List<HandAction> handActions, int actionIndex, GameType gameType)
        {
            for (int i = handLines.Length - 1; i > 0; i--)
            {
                var line = handLines[i];

                if (line.StartsWith("*** SUMMARY"))
                {
                    return;
                }

                var wonIndex = line.IndexOf(" and won ", StringComparison.OrdinalIgnoreCase);

                if (wonIndex > 0)
                {
                    var wonEndIndex = line.IndexOf(' ', wonIndex + 9);

                    string wonText;

                    if (wonEndIndex < 0)
                    {
                        wonText = line.Substring(wonIndex + 9);
                    }
                    else
                    {
                        wonText = line.Substring(wonIndex + 9, wonEndIndex - wonIndex - 9);
                    }

                    if (ParserUtils.TryParseMoney(wonText, out decimal won))
                    {
                        // get player name
                        var nameStartIndex = line.IndexOf(':') + 2;

                        // Seat 2: Peon_84 (button)
                        // Seat 2: Peon_84 (big blind)
                        // Seat 2: Peon_84 (small blind)
                        var nameEndIndex = line.LastIndexOf(" (");

                        if (nameEndIndex < 0)
                        {
                            nameEndIndex = line.LastIndexOf(" showed [");

                            if (nameEndIndex < 0)
                            {
                                nameEndIndex = line.LastIndexOf(" did not show");

                                if (nameEndIndex < 0)
                                {
                                    continue;
                                }
                            }
                        }

                        var playerName = line.Substring(nameStartIndex, nameEndIndex - nameStartIndex);

                        if (!handActions.Any(x => x.IsWinningsAction && x.PlayerName == playerName))
                        {
                            var winningAction = new WinningsAction(playerName, HandActionType.WINS, won, 0);
                            handActions.Add(winningAction);
                        }
                    }
                }
            }
        }

        protected override void ParseExtraHandInformation(string[] handLines, HandHistorySummary handHistorySummary)
        {
            var handHistory = handHistorySummary as HandHistory;

            if (handHistory == null)
            {
                return;
            };

            AdjustVillainPlayers(handHistory);
        }

        private void AdjustVillainPlayers(HandHistory handHistory)
        {
            var nonHeroPlayers = handHistory.Players
                .Where(x => (handHistory.Hero == null) || x.SeatNumber != handHistory.Hero.SeatNumber).ToArray();

            if (nonHeroPlayers.Any(x => !x.PlayerName.StartsWith("Villain")))
            {
                return;
            }

            var players = nonHeroPlayers
                .Select(x => new { OldPlayerName = x.PlayerName, NewPlayerName = $"Villain{x.SeatNumber}" })
                .ToDictionary(x => x.OldPlayerName, x => x.NewPlayerName);

            if (handHistory.Hero != null)
            {
                players.Add(handHistory.Hero.PlayerName, handHistory.Hero.PlayerName);
            }

            handHistory.Players.ForEach(x => x.PlayerName = players[x.PlayerName]);

            handHistory.HandActions.ForEach(x => x.PlayerName = players[x.PlayerName]);
        }

        private string GetGameHandLine(string[] handLines)
        {
            return handLines.First(x => x.StartsWith(GameLinePrefix, StringComparison.OrdinalIgnoreCase));
        }

        private string GetGameHandLine(string[] handLines, out int index)
        {
            var gameLine = handLines.First(x => x.StartsWith(GameLinePrefix, StringComparison.OrdinalIgnoreCase));
            index = Array.IndexOf(handLines, gameLine);

            return gameLine;
        }
    }
}