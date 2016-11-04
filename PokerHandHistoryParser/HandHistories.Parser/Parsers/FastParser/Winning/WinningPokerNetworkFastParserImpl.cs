using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils;
using HandHistories.Parser.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandHistories.Parser.Parsers.FastParser.Winning
{
    sealed class WinningPokerNetworkFastParserImpl : HandHistoryParserFastImpl
    {
        private const int GameIDStartIndex = 9;

        public override EnumPokerSites SiteName
        {
            get { return EnumPokerSites.WinningPoker; }
        }

        public override bool IsValidHand(string[] handLines)
        {
            if (handLines[handLines.Length - 1].StartsWith("Game ended at:", StringComparison.Ordinal))
            {
                return true;
            }
            return false;
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = IsCancelledHand(handLines);
            return IsValidHand(handLines);
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            throw new NotImplementedException();
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            // Expect the end of the hand to have something like this:
            //------ Summary ------
            //Pot: 80. Rake 2
            //Board: [3d 6h 2c Ah]

            BoardCards boardCards = BoardCards.ForPreflop();
            for (int lineNumber = handLines.Length - 4; lineNumber >= 0; lineNumber--)
            {
                string line = handLines[lineNumber];
                if (line[0] == '-')
                {
                    return boardCards;
                }

                if (line[0] != 'B')
                {
                    continue;
                }

                const int firstSquareBracketEnd = 8;
                int lastSquareBracket = line.Length - 1;

                return BoardCards.FromCards(line.Substring(firstSquareBracketEnd, lastSquareBracket - firstSquareBracketEnd).Replace("10", "T"));
            }

            return boardCards;
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            //Game started at: 2014/3/8 22:1:43
            const int startindex = 17;
            string dataString = handLines[0].Substring(startindex);
            DateTime time = DateTime.Parse(dataString, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
            return time;
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            //Seat 1 is the button
            const int offset = 5;
            int endIndex = handLines[2].IndexOf(' ', offset);
            string dealerLine = handLines[2].Substring(offset, endIndex - offset);
            return int.Parse(dealerLine);
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            //TODO: parse gametype
            string line = handLines[1];
            int startIndex = line.LastIndexOf('(');
            string game = line.Substring(startIndex);
            switch (game)
            {
                case "(Hold'em)":
                    return GameType.NoLimitHoldem;
                case "(Omaha)":
                    return GameType.PotLimitOmaha;
                case "(Omaha HiLow)":
                    return GameType.PotLimitOmahaHiLo;
                default:
                    throw new UnrecognizedGameTypeException(line, "GameType: " + game);
            }
        }

        protected override long ParseHandId(string[] handLines)
        {
            //Game ID: 261402571 2/4 Braunite (Omaha)
            int endIndex = handLines[1].IndexOf(' ', GameIDStartIndex);
            long handId = long.Parse(handLines[1].Substring(GameIDStartIndex, endIndex - GameIDStartIndex));
            return handId;
        }

        protected override string ParseHeroName(string[] handlines, PlayerList playerList)
        {
            for (int i = 3; i < handlines.Length; i++)
            {
                string line = handlines[i];
                if (line[0] == 'P' && line[line.Length - 1] == ']')
                {
                    const int NameStartIndex = 7;
                    int NameEndIndex = line.LastIndexOf(" r", StringComparison.Ordinal);
                    return line.Substring(NameStartIndex, NameEndIndex - NameStartIndex);
                }
            }
            return null;
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            //Game ID: 261402571 2/4 Braunite (Omaha)
            int startIndex = handLines[1].IndexOf(' ', GameIDStartIndex);
            int endIndex = handLines[1].IndexOf(' ', startIndex + 1);
            string limitText = handLines[1].Substring(startIndex, endIndex - startIndex);
            int splitIndex = limitText.IndexOf('/');
            decimal smallBlind = decimal.Parse(limitText.Remove(splitIndex), System.Globalization.CultureInfo.InvariantCulture);
            decimal bigBlind = decimal.Parse(limitText.Substring(splitIndex + 1), System.Globalization.CultureInfo.InvariantCulture);
            Limit limit = Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, Currency.USD);
            return limit;
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            PlayerList playerList = new PlayerList();
            int CurrentLineIndex = 3;

            while (handLines[CurrentLineIndex][0] == 'S')
            {
                string playerLine = handLines[CurrentLineIndex++];

                const int seatNumberStart = 5;
                int colonIndex = playerLine.IndexOf(':', seatNumberStart + 1);
                int SeatNumber = int.Parse(playerLine.Substring(seatNumberStart, colonIndex - seatNumberStart));

                int NameStartIndex = colonIndex + 2;
                int NameEndIndex = playerLine.LastIndexOf(" (", StringComparison.Ordinal);
                string playerName = playerLine.Substring(NameStartIndex, NameEndIndex - NameStartIndex);

                int stackSizeStartIndex = NameEndIndex + 2;
                int stackSizeEndIndex = playerLine.Length - 2;
                string stack = playerLine.Substring(stackSizeStartIndex, stackSizeEndIndex - stackSizeStartIndex);
                playerList.Add(new Player(playerName, decimal.Parse(stack, System.Globalization.CultureInfo.InvariantCulture), SeatNumber));
            }

            for (int i = CurrentLineIndex; i < handLines.Length; i++)
            {
                const int NameStartIndex = 7;
                string line = handLines[i];

                bool receivingCards = false;
                int NameEndIndex;
                string playerName;

                //Uncalled bet (20) returned to zz7
                if (line[0] == 'U')
                {
                    break;
                }

                switch (line[line.Length - 1])
                {
                    //Player bubblebubble received card: [2h]
                    case ']':
                        receivingCards = true;
                        break;

                    case '.':
                        //Player bubblebubble is timed out.
                        if (line[line.Length - 2] == 't')
                        {
                            continue;
                        }
                        receivingCards = true;
                        break;
                    case ')':
                        continue;
                    case 'B':
                        //Player bubblebubble wait BB
                        NameEndIndex = line.Length - 8;//" wait BB".Length
                        playerName = line.Substring(NameStartIndex, NameEndIndex - NameStartIndex);
                        playerList[playerName].IsSittingOut = true;
                        break;
                    case 't':
                        //Player xx45809 sitting out
                        if (line[line.Length - 2] == 'u')
                        {
                            NameEndIndex = line.Length - 12;//" sitting out".Length
                            playerName = line.Substring(NameStartIndex, NameEndIndex - NameStartIndex);
                            if (playerName == "")//"Player  sitting out"
                            {
                                continue;
                            }
                            playerList[playerName].IsSittingOut = true;
                            break;
                        }
                        //Player TheKunttzz posts (0.25) as a dead bet
                        else continue;
                    default:
                        throw new ArgumentException("Unhandled Line: " + line);
                }
                if (receivingCards)
                {
                    CurrentLineIndex = i;
                    break;
                }
            }

            //Parse HoleCards
            for (int i = CurrentLineIndex; i < handLines.Length; i++)
            {
                const int NameStartIndex = 7;
                string line = handLines[i];
                char endChar = line[line.Length - 1];

                if (endChar == '.')
                {
                    continue;
                }
                else if (endChar == ']')
                {
                    int NameEndIndex = line.LastIndexOf(" rec", line.Length - 12, StringComparison.Ordinal);
                    string playerName = line.Substring(NameStartIndex, NameEndIndex - NameStartIndex);

                    char rank = line[line.Length - 3];
                    char suit = line[line.Length - 2];

                    var player = playerList[playerName];
                    if (!player.hasHoleCards)
                    {
                        player.HoleCards = HoleCards.NoHolecards();
                    }
                    if (rank == '0')
                    {
                        rank = 'T';
                    }

                    player.HoleCards.AddCard(new Card(rank, suit));
                    continue;
                }
                else
                {
                    break;
                }
            }

            for (int i = handLines.Length - playerList.Count - 1; i < handLines.Length - 1; i++)
            {
                const int WinningStartOffset = 1;
                const int PlayerMinLength = 7;

                string summaryLine = handLines[i];

                int playerNameStartIndex = PlayerMinLength + (summaryLine[0] == '*' ? WinningStartOffset : 0);
                int playerNameEndIndex = summaryLine.IndexOf(' ', playerNameStartIndex);

                int ShowIndex = summaryLine.IndexOf(" shows: ", StringComparison.Ordinal);
                if (ShowIndex != -1)
                {
                    string playerName = summaryLine.Substring(playerNameStartIndex, ShowIndex - playerNameStartIndex);

                    int pocketStartIndex = summaryLine.IndexOf('[', playerNameEndIndex) + 1;
                    int pocketEndIndex = summaryLine.IndexOf(']', pocketStartIndex);

                    Player showdownPlayer = playerList[playerName];
                    if (!showdownPlayer.hasHoleCards)
                    {
                        string cards = summaryLine.Substring(pocketStartIndex, pocketEndIndex - pocketStartIndex);
                        cards = cards.Replace("10", "T");
                        showdownPlayer.HoleCards = HoleCards.FromCards(cards);
                    }
                }
            }

            return playerList;
        }

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            // TODO: parse poker format
            return PokerFormat.CashGame;
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            //TODO: add tests
            int Players = ParsePlayers(handLines).Count;
            SeatType seat = SeatType.FromMaxPlayers(Players, true);
            return seat;
        }

        protected override string ParseTableName(string[] handLines)
        {
            //Real money format:
            //Game ID: 261409536 2/4 Braunite (Omaha)
            //Game ID: 258592747 2/4 Gabilite (JP) - CAP - Max - 2 (Hold'em)
            //Play money format:
            //Game ID: 261409536 1/2 Wichita Falls (Omaha)
            //Game ID: 328766507 1/2 Wichita Falls 1/2 - 3 (Hold'em)
            string tablenameLine = handLines[1];
            int StartIndex = tablenameLine.IndexOf('/', GameIDStartIndex) + 2;
            StartIndex = tablenameLine.IndexOf(' ', StartIndex) + 1;
            string tableName = tablenameLine.Substring(StartIndex);

            int GameTypeStartIndex = tableName.LastIndexOf('(');

            return tableName.Remove(GameTypeStartIndex).Trim();
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            // TODO: usage?
            List<TableTypeDescription> descriptions = new List<TableTypeDescription>();
            if (handLines[1].Contains("(JP)"))
            {
                descriptions.Add(TableTypeDescription.Jackpot);
            }
            if (handLines[1].Contains(" CAP "))
            {
                descriptions.Add(TableTypeDescription.Cap);
            }

            return TableType.FromTableTypeDescriptions(descriptions.ToArray());
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            const int MinimumLinesWithoutActions = 8;
            //Allocate the full list so we we dont get a reallocation for every add()
            List<HandAction> handActions = new List<HandAction>(handLines.Length - MinimumLinesWithoutActions);
            Street currentStreet = Street.Preflop;

            PlayerList playerList = ParsePlayers(handLines);
            bool PlayerWithSpaces = playerList.FirstOrDefault(p => p.PlayerName.Contains(" ")) != null;

            //Skipping PlayerList
            int ActionsStart = GetActionStart(handLines);

            ////Parsing Fixed Actions
            //if (PlayerWithSpaces)
            //{
            //    ActionsStart = SkipSitOutLines(handLines, ActionsStart);
            //    handActions.Add(ParseSmallBlindWithSpaces(handLines[ActionsStart++], playerList));
            //    ActionsStart = SkipSitOutLines(handLines, ActionsStart);
            //    handActions.Add(ParseBigBlindWithSpaces(handLines[ActionsStart++], playerList));
            //}
            //else
            //{
            //    ActionsStart = SkipSitOutLines(handLines, ActionsStart);
            //    handActions.Add(ParseSmallBlind(handLines[ActionsStart++]));
            //    ActionsStart = SkipSitOutLines(handLines, ActionsStart);
            //    handActions.Add(ParseBigBlind(handLines[ActionsStart++]));
            //}

            //ActionsStart = ParsePosts(handLines, handActions, ActionsStart);

            ////Skipping all "received a card."
            //ActionsStart = SkipDealer(handLines, ActionsStart);

            ////ParseActions
            //for (int i = ActionsStart; i < handLines.Length; i++)
            //{
            //    string actionLine = handLines[i];
            //    if (actionLine[0] == 'P')
            //    {
            //        var action = ParseRegularAction(actionLine, currentStreet, playerList, handActions, PlayerWithSpaces);
            //        if (action != null)
            //        {
            //            handActions.Add(action);
            //        }
            //    }
            //    else if (actionLine[0] == '*')
            //    {
            //        //*** FLOP ***: [10s 9c 2c]
            //        currentStreet = ParseNextStreet(actionLine);
            //    }
            //    else if (actionLine[0] == 'U')
            //    {
            //        const string UncalledBet = ") returned to ";
            //        string playerName = actionLine.Substring(actionLine.IndexOf(UncalledBet, StringComparison.Ordinal) + UncalledBet.Length);
            //        handActions.Add(new HandAction(playerName, HandActionType.UNCALLED_BET, ParseActionAmountBeforePlayer(actionLine), currentStreet));
            //    }
            //    else if (actionLine[0] == '-')
            //    {
            //        ActionsStart = i++;
            //        break;
            //    }
            //}

            ////expected Summary
            ////------ Summary ------
            ////Pot: 14.95. Rake 0.80
            ////Board: [5c 8s 4h 10c 10s]
            //for (int i = ActionsStart + 2; i < handLines.Length; i++)
            //{
            //    string actionLine = handLines[i];
            //    //Parse winning action
            //    if (actionLine[0] == '*')
            //    {
            //        var action = ParseWinningsAction(actionLine, playerList, PlayerWithSpaces);
            //        handActions.Add(action);
            //    }
            //}
            return handActions;
        }

        protected override TournamentDescriptor ParseTournament(string[] handLines)
        {
            throw new NotImplementedException();
        }

        private int GetActionStart(string[] handLines)
        {
            for (int i = 3; i < handLines.Length; i++)
            {
                if (handLines[i][0] != 'S')
                {
                    return i;
                }
            }
            throw new ArgumentOutOfRangeException("handlines");
        }

        private bool IsCancelledHand(string[] handLines)
        {
            int start = GetActionStart(handLines);

            for (int i = start; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line.EndsWith("received a card.", StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
