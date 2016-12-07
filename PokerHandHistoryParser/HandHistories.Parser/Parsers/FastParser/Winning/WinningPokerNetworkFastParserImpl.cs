using DriveHUD.Common.Log;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.FastParsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandHistories.Parser.Parsers.FastParser.Winning
{
    sealed class WinningPokerNetworkFastParserImpl : HandHistoryParserFastImpl
    {
        #region Fields
        private const int GameIDStartIndex = 9;
        private const int ActionPlayerNameStartIndex = 7;
        private const int SummaryGameTypeStartIndex = 10;

        private static readonly NumberFormatInfo NumberFormatInfo = new NumberFormatInfo
        {
            NegativeSign = "-",
            CurrencyDecimalSeparator = ".",
            CurrencyGroupSeparator = ",",
            CurrencySymbol = "$"
        };

        /// <summary>
        /// Key - (BuyIn + Rake), Value - Rake
        /// </summary>
        private static readonly Dictionary<decimal, decimal> PredefinedRakeDictionary = new Dictionary<decimal, decimal>()
        {
            { 0.11m, 0.01m },
            { 0.28m, 0.03m },
            { 0.53m, 0.03m },
            { 0.55m, 0.05m },
            { 0.66m, 0.06m },
            { 1.1m, 0.1m },
            { 2.1m, 0.1m },
            { 2.2m, 0.2m },
            { 2.75m, 0.25m },
            { 3.15m, 0.15m },
            { 3.20m, 0.2m },
            { 3.30m, 0.3m },
            { 4.40m, 0.4m },
            { 5.50m, 0.5m },
            { 6.30m, 0.3m },
            { 6.60m, 0.6m },
            { 7.35m, 0.35m },
            { 7.50m, 0.5m },
            { 7.70m, 0.7m },
            { 8.40m, 0.4m },
            { 8.80m, 0.8m },
            { 10.30m, 0.3m },
            { 10.50m, 0.5m },
            { 10.75m, 0.75m },
            { 11m, 1m },
            { 11.50m, 0.50m },
            { 12m, 1m },
            { 13m, 1m },
            { 13.20m, 1.2m },
            { 16.50m, 1.5m },
            { 21m, 1m },
            { 22m, 2m },
            { 23m, 1m },
            { 26m, 2m },
            { 26.25m, 1.2m },
            { 27.50m, 2.5m },
            { 30m, 2m },
            { 33m, 3m },
            { 44m, 4m },
            { 55m, 5m },
            { 66m, 6m },
            { 82.5m, 7.5m },
            { 88m, 8m },
            { 109m, 9m },
            { 160m, 10m },
            { 215m, 15m },
            { 320m, 20m },
            { 530m, 30m },
            { 540m, 40m },
        };
        #endregion

        public override EnumPokerSites SiteName
        {
            get { return EnumPokerSites.WinningPokerNetwork; }
        }

        public override bool RequiresTotalPotCalculation
        {
            get { return true; }
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
            isCancelled = false;

            try
            {
                isCancelled = IsCancelledHand(handLines);
            }
            catch (ArgumentException)
            {
                // hand is not full
                return false;
            }

            return IsValidHand(handLines);
        }

        private static readonly Regex HandSplitRegex = new Regex("Game started at: ", RegexOptions.Compiled);
        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                            .Where(s => string.IsNullOrWhiteSpace(s) == false && s.Length > 30)
                            .Select(s => "Game started at: " + s.Trim('\r', '\n'));
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
            //Game started at: 2014/3/8 22:1:43 *** Summary
            const int startindex = 17;
            string dateString = handLines[0].Substring(startindex);

            int summaryStringStartIndex = dateString.LastIndexOf(" ***");
            if (summaryStringStartIndex != -1)
            {
                dateString = dateString.Remove(summaryStringStartIndex);
            }

            DateTime time = DateTime.Parse(dateString, System.Globalization.CultureInfo.CurrentCulture).ToUniversalTime();
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
            string line = handLines[1];
            int startIndex = line.LastIndexOf('(');
            string game = line.Substring(startIndex);

            string gameType = string.Empty;

            // if summary is present
            string summary = GetSummaryString(handLines);
            if (!string.IsNullOrWhiteSpace(summary))
            {
                //GameType: NL
                //GameType: FL
                //GameType: PL
                gameType = summary.Substring(SummaryGameTypeStartIndex, 2);
            }

            switch (game)
            {
                case "(Hold'em)":
                    if (gameType.Equals("FL")) { return GameType.FixedLimitHoldem; }
                    if (gameType.Equals("PL")) { return GameType.PotLimitHoldem; }
                    return GameType.NoLimitHoldem;
                case "(Omaha)":
                    if (gameType.Equals("FL")) { return GameType.FixedLimitOmaha; }
                    if (gameType.Equals("NL")) { return GameType.NoLimitOmaha; }
                    return GameType.PotLimitOmaha;
                case "(Omaha HiLow)":
                    if (gameType.Equals("FL")) { return GameType.FixedLimitOmahaHiLo; }
                    if (gameType.Equals("NL")) { return GameType.NoLimitOmahaHiLo; }
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
            var currency = handLines[1].Contains("PM ") || handLines[1].Contains("Play Money ") ? Currency.PlayMoney : Currency.USD;
            Limit limit = Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, currency);
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
            //Game ID: 775559540 30 / 60 $10 Freeroll - On Demand, Table 27(Hold'em)
            if (handLines[1].Contains("Table "))
            {
                return PokerFormat.Tournament;
            }

            return PokerFormat.CashGame;
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            var Players = ParsePlayers(handLines);

            SeatType? seatType = TryParseSeatTypeByName(handLines);

            if (seatType.HasValue)
            {
                return seatType.Value;
            }
            else
            {
                var maxSeat = Players.Max(x => x.SeatNumber);

                return SeatType.FromMaxPlayers(maxSeat, true);
            }
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
            List<TableTypeDescription> descriptions = new List<TableTypeDescription>();
            if (handLines[1].Contains("(JP)"))
            {
                descriptions.Add(TableTypeDescription.Jackpot);
            }
            if (handLines[1].Contains(" CAP "))
            {
                descriptions.Add(TableTypeDescription.Cap);
            }

            return descriptions.Count == 0
                ? TableType.FromTableTypeDescriptions(TableTypeDescription.Regular)
                : TableType.FromTableTypeDescriptions(descriptions.ToArray());
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            const int MinimumLinesWithoutActions = 8;
            //Allocate the full list so we we dont get a reallocation for every add()
            List<HandAction> handActions = new List<HandAction>(handLines.Length - MinimumLinesWithoutActions);
            Street currentStreet = Street.Preflop;

            PlayerList playerList = ParsePlayers(handLines);
            int dealerPosition = ParseDealerPosition(handLines);
            bool playerWithSpaces = playerList.Any(p => p.PlayerName.Contains(" "));

            //Skipping PlayerList
            int actionsStart = GetActionStart(handLines);

            actionsStart = ParsePosts(handLines, handActions, actionsStart);

            if (handActions.Any(x => x.HandActionType == HandActionType.ALL_IN))
            {
                AdjustAllIn(handActions);
            }

            //Skipping all "received a card."
            actionsStart = SkipDealer(handLines, actionsStart);

            //ParseActions
            for (int i = actionsStart; i < handLines.Length; i++)
            {
                string actionLine = handLines[i];
                if (actionLine[0] == 'P')
                {
                    var action = ParseRegularAction(actionLine, currentStreet, playerList, handActions, playerWithSpaces);
                    if (action != null)
                    {
                        handActions.Add(action);
                    }
                }
                else if (actionLine[0] == '*')
                {
                    //*** FLOP ***: [10s 9c 2c]
                    currentStreet = ParseNextStreet(actionLine);
                }
                else if (actionLine[0] == 'U')
                {
                    const string UncalledBet = ") returned to ";
                    string playerName = actionLine.Substring(actionLine.IndexOf(UncalledBet, StringComparison.Ordinal) + UncalledBet.Length);
                    handActions.Add(new HandAction(playerName, HandActionType.UNCALLED_BET, ParseActionAmountBeforePlayer(actionLine), currentStreet));
                }
                else if (actionLine[0] == '-')
                {
                    actionsStart = i++;
                    break;
                }
            }

            //expected Summary
            //------ Summary ------
            //Pot: 14.95. Rake 0.80
            //Board: [5c 8s 4h 10c 10s]
            for (int i = actionsStart + 2; i < handLines.Length; i++)
            {
                string actionLine = handLines[i];
                //Parse winning action
                if (actionLine[0] == '*')
                {
                    var action = ParseWinningsAction(actionLine, playerList, playerWithSpaces);
                    handActions.Add(action);
                }
            }
            return handActions;
        }

        // at this point we should have only blinds/posts/antes
        private void AdjustAllIn(List<HandAction> handActions)
        {
            var isSmallBlindDefined = handActions.Any(x => x.HandActionType == HandActionType.SMALL_BLIND);
            var isBigBlindDefined = handActions.Any(x => x.HandActionType == HandActionType.BIG_BLIND);

            var predictedSBPlayer = handActions.ElementAtOrDefault(0)?.PlayerName;
            var predictedBBPlayer = handActions.ElementAtOrDefault(1)?.PlayerName;

            if (!isBigBlindDefined)
            {
                if (isSmallBlindDefined)
                {
                    // actions start from sb (if present)
                    if (!string.IsNullOrWhiteSpace(predictedBBPlayer))
                    {
                        var lastBBAction = handActions.LastOrDefault(x => x.HandActionType == HandActionType.ALL_IN
                                            && x.PlayerName == predictedBBPlayer);

                        isBigBlindDefined = OverrideActionType(lastBBAction, HandActionType.BIG_BLIND, handActions);
                    }
                }
                else
                {
                    // if only one action -> bb
                    if (!string.IsNullOrWhiteSpace(predictedSBPlayer) && !string.IsNullOrWhiteSpace(predictedBBPlayer))
                    {
                        if (handActions.Any(x => x.HandActionType == HandActionType.ALL_IN && x.PlayerName == predictedSBPlayer))
                        {
                            if (handActions.Any(x => x.HandActionType == HandActionType.ALL_IN && x.PlayerName == predictedBBPlayer))
                            {
                                // if first 2 players have all-in -> we have small + big
                                var lastSBAction = handActions.LastOrDefault(x => x.HandActionType == HandActionType.ALL_IN
                                            && x.PlayerName == predictedSBPlayer);
                                var lastBBAction = handActions.LastOrDefault(x => x.HandActionType == HandActionType.ALL_IN
                                            && x.PlayerName == predictedBBPlayer);

                                isSmallBlindDefined = OverrideActionType(lastSBAction, HandActionType.SMALL_BLIND, handActions);
                                isBigBlindDefined = OverrideActionType(lastBBAction, HandActionType.BIG_BLIND, handActions);
                            }
                            else
                            {
                                // if only first all-in and no bb -> first action is bb, no small
                                var lastBBAction = handActions.LastOrDefault(x => x.HandActionType == HandActionType.ALL_IN
                                               && x.PlayerName == predictedSBPlayer);
                                isBigBlindDefined = OverrideActionType(lastBBAction, HandActionType.BIG_BLIND, handActions);
                            }
                        }
                    }
                }
            }

            if (!isSmallBlindDefined)
            {
                // if bb defined and it's the second player, then 1st player is sb
                if (isBigBlindDefined && handActions.FirstOrDefault(x => x.HandActionType == HandActionType.BIG_BLIND)?.PlayerName == predictedBBPlayer)
                {
                    var lastSBAction = handActions.LastOrDefault(x => x.HandActionType == HandActionType.ALL_IN
                                            && x.PlayerName == predictedSBPlayer);
                    isSmallBlindDefined = OverrideActionType(lastSBAction, HandActionType.SMALL_BLIND, handActions);
                }
                // game without SB, at this point we should have BB already
                else if (!isBigBlindDefined)
                {
                    //shouldn't be possible
                    LogProvider.Log.Warn($"Cannot parse blinds for: {Environment.NewLine}" +
                        string.Join(Environment.NewLine, handActions));
                }
            }

            // if any ante or any 2 actions with same playername - we have ante
            bool isAnteGame = handActions.Any(x => x.HandActionType == HandActionType.ANTE)
                || handActions.GroupBy(x => x.PlayerName).Any(x => x.Count() > 1);

            if (isAnteGame)
            {
                var processedPlayers = new List<string>();
                for (int i = 0; i < handActions.Count; i++)
                {
                    var action = handActions[i];
                    if (processedPlayers.Contains(action.PlayerName))
                    {
                        break;
                    }
                    // first player's all-in is ante
                    if (action.HandActionType == HandActionType.ALL_IN)
                    {
                        handActions[i] = new HandAction(action.PlayerName, HandActionType.ANTE, action.Amount, action.Street);
                    }
                }
            }

            // other All-In's are Posts action (straddle, dead bet etc.)
            for (int i = 0; i < handActions.Count; i++)
            {
                var action = handActions[i];
                if (action.HandActionType == HandActionType.ALL_IN)
                {
                    handActions[i] = new HandAction(action.PlayerName, HandActionType.POSTS, action.Amount, action.Street);
                }
            }
        }

        private bool OverrideActionType(HandAction actionToOverride, HandActionType newHandActionType, List<HandAction> source)
        {
            if (actionToOverride != null)
            {
                var indexOfBB = source.IndexOf(actionToOverride);
                source[indexOfBB] = new HandAction(actionToOverride.PlayerName, newHandActionType, actionToOverride.Amount, actionToOverride.Street);
                return true;
            }

            return false;
        }

        protected override TournamentDescriptor ParseTournament(string[] handLines)
        {
            //TournamentId: 213213123, TournamentBuyIn: 0.65$
            var tournamentString = GetSummaryString(handLines);

            var regex = new Regex(@"TournamentId: (?<tournament_id>[^\s]+), TournamentBuyIn: (?<buyin>[^-]+)");

            var match = regex.Match(tournamentString);

            if (!match.Success)
            {
                LogProvider.Log.Warn($"Tournament summary wasn't found: {handLines[1]}");
                return new TournamentDescriptor()
                {
                    BuyIn = Buyin.AllBuyin(),
                    TournamentName = "Undefined Tournament",
                    TournamentId = "0",
                    Speed = TournamentSpeed.Regular
                };
            }

            var tournamentName = RemoveTableNumber(ParseTableName(handLines));
            var buyIn = decimal.Parse(match.Groups["buyin"].Value, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, NumberFormatInfo);
            var rake = 0m;

            var speed = ParserUtils.ParseTournamentSpeed(tournamentName);
            var seatType = SeatType.Parse(tournamentName);

            // for majority of tournaments the buyIn is in the tournament name. 
            // the buyIn from the summary line contains rake in it.
            if (buyIn > 0)
            {
                var buyInStartIndex = tournamentName.IndexOf(NumberFormatInfo.CurrencySymbol);
                var buyInEndIndex = tournamentName.IndexOf(' ', buyInStartIndex);

                if (buyInStartIndex != -1 && buyInEndIndex != -1 && buyInEndIndex > buyInStartIndex)
                {
                    var buyInWithoutRake = 0m;
                    if (decimal.TryParse(tournamentName.Substring(buyInStartIndex, buyInEndIndex - buyInStartIndex), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, NumberFormatInfo, out buyInWithoutRake))
                    {
                        if (buyIn > buyInWithoutRake)
                        {
                            rake = buyIn - buyInWithoutRake;
                            buyIn = buyInWithoutRake;
                        }
                    }
                }
            }

            // try to figure out rake and real buyIn values from predefined table
            if (rake == 0m && buyIn > 0)
            {
                rake = GetPredefinedRake(buyIn);
                buyIn = buyIn - rake;
            }

            var tournamentDescriptor = new TournamentDescriptor
            {
                TournamentId = match.Groups["tournament_id"].Value,
                BuyIn = Buyin.FromBuyinRake(buyIn, rake, Currency.USD),
                Speed = speed,
                TournamentName = $"{tournamentName} #{match.Groups["tournament_id"].Value}",
            };

            return tournamentDescriptor;
        }

        private string RemoveTableNumber(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return tableName;
            }

            var tableNumberStartIndex = tableName.LastIndexOf(", Table");
            if (tableNumberStartIndex == -1)
            {
                return tableName;
            }

            return tableName.Remove(tableNumberStartIndex);
        }

        private HandAction ParseRegularAction(string line, Street currentStreet, PlayerList playerList, List<HandAction> actions, bool PlayerWithSpaces)
        {
            string PlayerName = PlayerWithSpaces ?
                GetPlayerNameWithSpaces(line, playerList) :
                GetPlayerNameWithoutSpaces(line);

            int actionIDIndex = ActionPlayerNameStartIndex + PlayerName.Length + 1;
            char actionID = line[actionIDIndex];
            switch (actionID)
            {
                //Player PersnicketyBeagle folds
                case 'f':
                    return new HandAction(PlayerName, HandActionType.FOLD, 0, currentStreet);
                case 'r':
                    return new HandAction(PlayerName, HandActionType.RAISE, ParseActionAmountAfterPlayer(line), currentStreet);
                //checks or calls
                case 'c':
                    //Player butta21 calls (20)
                    //Player jayslowplay caps (3.50)
                    //Player STOPCRYINGB79 checks
                    char actionID2 = line[actionIDIndex + 2];
                    if (actionID2 == 'e')
                    {
                        return new HandAction(PlayerName, HandActionType.CHECK, 0, currentStreet);
                    }
                    else if (actionID2 == 'l')
                    {
                        return new HandAction(PlayerName, HandActionType.CALL, ParseActionAmountAfterPlayer(line), currentStreet);
                    }
                    else if (actionID2 == 'p')
                    {
                        //treat CAP as allin
                        var capAmount = ParseActionAmountAfterPlayer(line);
                        var capAllinActionType = AllInActionHelper.GetAllInActionType(PlayerName, capAmount, currentStreet, actions);
                        return new HandAction(PlayerName, capAllinActionType, capAmount, currentStreet, true);
                    }
                    else
                    {
                        throw new NotImplementedException("HandActionType: " + line);
                    }
                case 'b':
                    if (currentStreet == Street.Preflop)
                    {
                        return new HandAction(PlayerName, HandActionType.RAISE, ParseActionAmountAfterPlayer(line), currentStreet);
                    }
                    else
                    {
                        return new HandAction(PlayerName, HandActionType.BET, ParseActionAmountAfterPlayer(line), currentStreet);
                    }
                //Player PersnicketyBeagle allin (383)
                case 'a':
                    var amount = ParseActionAmountAfterPlayer(line);
                    var actionType = AllInActionHelper.GetAllInActionType(PlayerName, amount, currentStreet, actions);
                    return new HandAction(PlayerName, actionType, amount, currentStreet, true);
                case 'm'://Player PersnicketyBeagle mucks cards
                case 'i'://Player ECU7184 is timed out.
                    return null;
            }
            throw new HandActionException(line, "HandActionType: " + line);
        }

        private HandAction ParseWinningsAction(string line, PlayerList playerList, bool PlayerWithSpaces)
        {
            string playerName = PlayerWithSpaces ? GetWinnerNameWithSpaces(line, playerList) : GetWinnerNameWithoutSpaces(line);
            int winAmountStartIndex = line.LastIndexOf("cts:", StringComparison.Ordinal) + 5;
            int winAmountEndIndex = line.IndexOf(". ", winAmountStartIndex, StringComparison.Ordinal);
            string winString = line.Substring(winAmountStartIndex, winAmountEndIndex - winAmountStartIndex);
            decimal winAmount = winString.ParseAmount();
            return new WinningsAction(playerName, HandActionType.WINS, winAmount, 0);
        }

        private int SkipSitOutLines(string[] handLines, int ActionsStart)
        {
            for (int i = ActionsStart; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line[line.Length - 1] == ')')
                {
                    return i;
                }
            }
            throw new Exception("Did not find end of Sitout Lines");
        }

        private HandAction ParseSmallBlind(string bbPost)
        {
            int playerNameEndIndex = GetPlayerNameEndIndex(bbPost);
            string PlayerName = bbPost.Substring(ActionPlayerNameStartIndex, playerNameEndIndex - ActionPlayerNameStartIndex);
            return new HandAction(PlayerName, HandActionType.SMALL_BLIND, ParseActionAmountAfterPlayer(bbPost), Street.Preflop);
        }

        private HandAction ParseBigBlind(string bbPost)
        {
            int playerNameEndIndex = GetPlayerNameEndIndex(bbPost);
            string PlayerName = bbPost.Substring(ActionPlayerNameStartIndex, playerNameEndIndex - ActionPlayerNameStartIndex);
            return new HandAction(PlayerName, HandActionType.BIG_BLIND, ParseActionAmountAfterPlayer(bbPost), Street.Preflop);
        }

        private HandAction ParseSmallBlindWithSpaces(string sbPost, PlayerList players)
        {
            string PlayerName = GetPlayerNameWithSpaces(sbPost, players);
            return new HandAction(PlayerName, HandActionType.SMALL_BLIND, ParseActionAmountAfterPlayer(sbPost), Street.Preflop);
        }

        private HandAction ParseBigBlindWithSpaces(string bbPost, PlayerList players)
        {
            string PlayerName = GetPlayerNameWithSpaces(bbPost, players);
            return new HandAction(PlayerName, HandActionType.BIG_BLIND, ParseActionAmountAfterPlayer(bbPost), Street.Preflop);
        }

        private static Street ParseNextStreet(string actionLine)
        {
            //*** FLOP ***: [10s 9c 2c]
            const int StreetIndex = 4;
            switch (actionLine[StreetIndex])
            {
                case 'F':
                    return Street.Flop;
                case 'T':
                    return Street.Turn;
                case 'R':
                    return Street.River;
                default:
                    throw new ArgumentException("Street: " + actionLine);
            }
        }

        private string GetWinnerNameWithoutSpaces(string actionLine)
        {
            int playerNameEndIndex = actionLine.IndexOf(' ', ActionPlayerNameStartIndex + 1);
            return actionLine.Substring(ActionPlayerNameStartIndex + 1, playerNameEndIndex - ActionPlayerNameStartIndex - 1);
        }

        private string GetWinnerNameWithSpaces(string actionLine, PlayerList playerList)
        {
            //Must choose the longest name if one name starts with an other name
            int length = 0;
            string result = null;
            foreach (var player in playerList)
            {
                if (actionLine.StartsWith("*Player " + player.PlayerName, StringComparison.Ordinal) && player.PlayerName.Length > length)
                {
                    length = player.PlayerName.Length;
                    result = player.PlayerName;
                }
            }
            return result;
        }

        private string GetPlayerNameWithoutSpaces(string actionLine)
        {
            int endIndex = actionLine.IndexOf(' ', ActionPlayerNameStartIndex);
            return actionLine.Substring(ActionPlayerNameStartIndex, endIndex - ActionPlayerNameStartIndex);
        }

        private string GetPlayerNameWithSpaces(string actionLine, PlayerList playerList)
        {
            //Must choose the longest name if one name starts with another name
            int length = 0;
            string result = null;
            foreach (var player in playerList)
            {
                if (actionLine.StartsWith("Player " + player.PlayerName, StringComparison.Ordinal) && player.PlayerName.Length > length)
                {
                    length = player.PlayerName.Length;
                    result = player.PlayerName;
                }
            }
            return result;
        }

        private int SkipDealer(string[] handLines, int ActionsStart)
        {
            for (int i = ActionsStart; i < handLines.Length; i++)
            {
                //Player LadyStack received a card.
                string Line = handLines[i];
                switch (handLines[i][Line.Length - 1])
                {
                    case 'B':
                    case '.':
                    case ']'://Player {playername} received card: [2h]
                        continue;
                    default:
                        return i;
                }
            }
            throw new ArgumentOutOfRangeException("handlines");
        }

        private int GetPlayerNameEndIndex(string handLine)
        {
            return handLine.IndexOf(' ', ActionPlayerNameStartIndex);
        }

        /// <summary>
        /// The amount must be after the player name or it will fail when players have a parathesis in their name
        /// </summary>
        /// <param name="handLine"></param>
        /// <returns></returns>
        private decimal ParseActionAmountAfterPlayer(string handLine)
        {
            int endIndex = handLine.LastIndexOf(')');
            int startIndex = handLine.LastIndexOf('(', endIndex) + 1;
            string text = handLine.Substring(startIndex, endIndex - startIndex);
            return text.ParseAmount();
        }

        /// <summary>
        /// The amount must be before the player name or it will fail when players have a parathesis in their name
        /// </summary>
        /// <param name="handLine"></param>
        /// <returns></returns>
        private decimal ParseActionAmountBeforePlayer(string handLine)
        {
            int startIndex = handLine.IndexOf('(') + 1;
            int endIndex = handLine.IndexOf(')', startIndex);
            string text = handLine.Substring(startIndex, endIndex - startIndex);
            return text.ParseAmount();
        }

        private int ParsePosts(string[] handLines, List<HandAction> actions, int ActionsStart)
        {
            for (int i = ActionsStart; i < handLines.Length; i++)
            {
                const int PlayerNameStartindex = 7;//"Player ".Length
                string line = handLines[i];

                char endChar = line[line.Length - 1];
                bool deadBet = false;

                switch (endChar)
                {
                    //Player bubblebubble wait BB
                    case 'B':
                        continue;//More posts can still occur

                    //Player Aquasces1 received a card.
                    case '.':
                    //Player WP_Hero received card: [6d]
                    case ']':
                        //No more posts can occur when players start reciving cards
                        return i;

                    //Player TheKunttzz posts (0.25) as a dead bet
                    //Player TheKunttzz posts (0.50)
                    //Player Hero sitting out
                    case 't':
                        //if sitting out go to next action
                        if (line.Substring(line.Length - 3, 3).Equals("out"))
                            continue;//More posts can still occur

                        deadBet = true;
                        break;

                    //Player impala327 allin(50)
                    //Player marbleeye ante (50) 
                    //Player Aquasces1 has small blind (2)
                    //Player COMON-JOE-JUG has big blind (4)
                    //Player TheKunttzz posts (0.25)
                    //Player TheKunttzz straddles (0.50)
                    case ')':
                        break;

                    default:
                        throw new HandActionException(line, "Unrecognized endChar \"" + endChar + "\"");
                }

                HandActionType actionType = HandActionType.UNKNOWN;

                int playerNameEndIndex = line.IndexOf(" posts (", StringComparison.Ordinal);
                if (playerNameEndIndex == -1)
                {
                    playerNameEndIndex = line.IndexOf(" straddle (", StringComparison.Ordinal);
                }

                if (playerNameEndIndex != -1)
                {
                    actionType = HandActionType.POSTS;
                }
                else
                {
                    playerNameEndIndex = line.IndexOf(" has small blind (", StringComparison.Ordinal);
                    actionType = HandActionType.SMALL_BLIND;
                    if (playerNameEndIndex == -1)
                    {
                        playerNameEndIndex = line.IndexOf(" has big blind (", StringComparison.Ordinal);
                        actionType = HandActionType.BIG_BLIND;
                    }
                    if (playerNameEndIndex == -1)
                    {
                        playerNameEndIndex = line.IndexOf(" ante (", StringComparison.Ordinal);
                        actionType = HandActionType.ANTE;
                    }
                    if (playerNameEndIndex == -1)
                    {
                        playerNameEndIndex = line.IndexOf(" allin (", StringComparison.Ordinal);
                        actionType = HandActionType.ALL_IN;
                    }
                }

                string playerName = line.Substring(PlayerNameStartindex, playerNameEndIndex - PlayerNameStartindex);
                decimal Amount = ParseActionAmountAfterPlayer(line);

                if (deadBet)
                {
                    Amount += ParseActionAmountAfterPlayer(handLines[++i]);
                }

                actions.Add(new HandAction(playerName, actionType, Amount, Street.Preflop));
            }
            throw new Exception("Did not find start of Dealing of cards");
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

        private string GetSummaryString(string[] handLines)
        {
            const string SummaryPattern = "*** Summary: ";

            string line = handLines[0];
            int startIndex = line.LastIndexOf(SummaryPattern, StringComparison.Ordinal);
            if (startIndex == -1)
            {
                return string.Empty;
            }
            return line.Substring(startIndex + SummaryPattern.Count());
        }

        private SeatType? TryParseSeatTypeByName(string[] handLines)
        {
            var line = handLines[1];

            if (line.ToLower().Contains("heads-up"))
            {
                return SeatType.FromMaxPlayers(2);
            }

            if (line.Contains("Jackpot"))
            {
                return SeatType.FromMaxPlayers(3);
            }

            if (line.Contains("4-Max"))
            {
                return SeatType.FromMaxPlayers(4);
            }

            if (line.Contains("6-Max"))
            {
                return SeatType.FromMaxPlayers(6);
            }

            if (line.Contains("8-Max"))
            {
                return SeatType.FromMaxPlayers(8);
            }

            if (line.Contains("9-Max"))
            {
                return SeatType.FromMaxPlayers(9);
            }

            return null;
        }

        private decimal GetPredefinedRake(decimal totalBuyIn)
        {
            if (PredefinedRakeDictionary.ContainsKey(totalBuyIn))
            {
                return PredefinedRakeDictionary[totalBuyIn];
            }
            return 0;
        }
    }
}
