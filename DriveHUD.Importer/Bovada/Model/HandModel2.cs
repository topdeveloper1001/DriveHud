//-----------------------------------------------------------------------
// <copyright file="HandModel2.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.Importers.Helpers;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Hand model class from BCC, must be removed and never used anymore as well as all other crap from BCC, but tests must be created before doing that
    /// </summary>
    internal class HandModel2
    {
        public List<Command> Commands { get; set; }

        public ulong HandNumber { get; set; }

        #region Tournament attributes

        public long TournamentNumber { get; set; }

        public CashOrTournament CashOrTournament { get; set; }

        public decimal TournamentBuyIn { get; set; }

        public decimal TournamentRake { get; set; }

        public uint TableID { get; set; }

        public List<TourneyPlayerFinished> PlayersFinishedTournament { get; set; }

        public bool IsHeroFinishedTournament { get; set; }

        public GameFormat GameFormat { get; set; }

        public GameLimit GameLimit { get; set; }

        public int LastNotHeroPlayerFinishedPlace { get; set; }

        #endregion

        public string TableTitle { get; set; }

        public int HeroSeat { get; set; }

        public bool HeroHasBeenAdded { get; set; }

        public int SmallBlindSeat { get; set; }

        public int BigBlindSeat { get; set; }

        public int DealerSeat { get; set; }

        public int TableType { get; set; }

        public int LinesCount { get; set; }

        public GameType GameType { get; set; }

        public string TableName { get; set; }

        public string OriginalTableName { get; set; }

        public string Stake { get; set; }

        public decimal Ante { get; set; }

        public decimal SmallBlind { get; set; }

        public decimal BigBlind { get; set; }

        public Dictionary<int, decimal> DeadBlinds { get; set; }

        public decimal TotalRake { get; set; }

        public decimal BrutoPot { get; set; }

        public decimal TotalRakeFinal { get; set; }

        public decimal BrutoPotFinal { get; set; }

        public IntPtr Handle { get; set; }

        public string HeroCards { get; set; }

        public List<int> SeatsOnTable { get; set; }

        public List<int> SeatsDealt { get; set; }

        public List<Command> SeatsRemovedOrAddedCommands { get; set; }

        public List<int> SeatsRemoved { get; set; }

        public List<int> SeatsAdded { get; set; }

        public Dictionary<int, decimal> StacksBeforeHand { get; set; }

        public Dictionary<int, decimal> StacksAfterBlinds { get; set; }

        public Dictionary<int, decimal> StacksAfterHand { get; set; }

        public Dictionary<int, int> SeatsPlayerID { get; set; }

        public HandPhaseEnum LastHandPhase { get; set; }             // last handPhase before showdown (32168)

        public List<Command> PreflopCommands { get; set; }

        public List<Command> PreflopActionAndSetStackCommands { get; set; }

        public List<Command> PostflopCommands { get; set; }

        public List<Command> PostflopActionAndSetStackCommands { get; set; }

        public List<Command> TurnCommands { get; set; }
        public List<Command> TurnActionAndSetStackCommands { get; set; }

        public List<Command> RiverCommands { get; set; }
        public List<Command> RiverActionAndSetStackCommands { get; set; }

        public List<KeyValuePair<int, decimal>> PlayersAddedAmountToStack { get; set; }
        public List<int> PlayersBroke { get; set; }
        public List<int> PlayersWentAllin { get; set; }

        public List<Command> ShowDownCommands { get; set; }

        public bool IsSmallBlindAllIn { get; set; }
        public bool IsBigBlindAllIn { get; set; }

        public bool IsLongTableName { get; private set; }

        public bool IsZonePoker { get; private set; }

        public bool HeroWasMoved { get; private set; }

        public HandModel2(List<Command> commandLines)
        {
            commandLines = CleanUpCommandsFromDuplicates(commandLines);

            Commands = commandLines;

            var commands = Commands;

            PlayersAddedAmountToStack = new List<KeyValuePair<int, decimal>>();

            InitializeTableData(commands);

            StacksBeforeHand = GetStacksBeforeHand(commands);                      // empty for first hand after entrance
            PlayersBroke = GetPlayersBroke(commands);
            PlayersWentAllin = GetPlayersWentAllin(commands);
            StacksAfterBlinds = GetStacksAfterBlinds(commands);                    // might be empty for first hand after entrance
            StacksAfterHand = GetStacksAfterHand(commands);                        // should not be empty (maybe for the last hand)

            SeatsPlayerID = GetSeatsPlayerID(commands);

            TableTitle = string.Empty;

            HeroSeat = GetHeroSeat(commands);

            // blind seats
            var blindsSeats = GetBlindsSeats(commands);
            SmallBlindSeat = blindsSeats.Item1;
            BigBlindSeat = blindsSeats.Item2;

            SeatsDealt = GetSeatsDealt(commands);

            // blinds & ante
            Ante = GetAnte(commands, StacksBeforeHand, CashOrTournament);
            SmallBlind = GetSeatBlindsStackChange(SmallBlindSeat, Ante);
            BigBlind = GetSeatBlindsStackChange(BigBlindSeat, Ante);
            DeadBlinds = GetDeadBlinds(commands);

            // dealer seat
            DealerSeat = GetDealerSeat(commands);

            // pot size issue
            // if player was added in the end of previous hand then StacksBeforeHand must be calculated
            if (StacksBeforeHand.Count != StacksAfterBlinds.Count)
            {
                RebuildMissedBeforeHandStacks();
            }

            #region Tournament attributes

            TournamentNumber = GetTournamentNumber(commands);
            TournamentBuyIn = GetTournamentBuyIn(commands);
            TournamentRake = GetTournamentRake(commands);
            Stake = GetStake(CashOrTournament, SmallBlind, BigBlind);   // from sb and bb            
            PlayersFinishedTournament = GetPlayerFinishedTournament();
            IsHeroFinishedTournament = PlayersFinishedTournament.Any(x => x.SeatNumber == HeroSeat);

            #endregion

            LinesCount = Commands.Count;

            var tableName = GetTableName(commands);

            IsZonePoker = BovadaHelper.IsZonePoker(tableName);

            TableName = tableName;
            OriginalTableName = tableName;
            IsLongTableName = GetIsLongTableName(tableName);

            Handle = GetHandle(commands);
            HeroCards = GetHeroCards(commands);

            SeatsOnTable = GetSeatsOnTable(commands);

            // all from the same source!
            SeatsRemovedOrAddedCommands = GetSeatsRemovedOrAddedCommands(commands);
            SeatsRemoved = GetSeatsRemoved(commands);
            SeatsAdded = GetSeatsAdded(commands);

            BrutoPot = 0.0m;
            TotalRakeFinal = 0.0m;
            BrutoPotFinal = 0.0m;
            TotalRake = 0.0m;

            if (IsZonePoker)
            {
                var zonePokerRoundCommands = GetZonePokerRoundCommands(commands);

                PreflopCommands = zonePokerRoundCommands[HandPhaseEnum.Preflop];
                PreflopActionAndSetStackCommands = GetGivenHandphaseActionAndSetStackCommands(PreflopCommands);

                PostflopCommands = zonePokerRoundCommands[HandPhaseEnum.PostFlop];
                PostflopActionAndSetStackCommands = GetGivenHandphaseActionAndSetStackCommands(PostflopCommands);

                TurnCommands = zonePokerRoundCommands[HandPhaseEnum.Turn];
                TurnActionAndSetStackCommands = GetGivenHandphaseActionAndSetStackCommands(TurnCommands);

                RiverCommands = zonePokerRoundCommands[HandPhaseEnum.River];
                RiverActionAndSetStackCommands = GetGivenHandphaseActionAndSetStackCommands(RiverCommands);

                ShowDownCommands = zonePokerRoundCommands[HandPhaseEnum.ShowDown];
            }
            else
            {
                LastHandPhase = GetLastHandPhase(commands);

                PreflopCommands = GetGivenHandphaseCommands(commands, HandPhaseEnum.Preflop, LastHandPhase);
                PreflopActionAndSetStackCommands = GetGivenHandphaseActionAndSetStackCommands(PreflopCommands);

                PostflopCommands = GetGivenHandphaseCommands(commands, HandPhaseEnum.PostFlop, LastHandPhase);
                PostflopActionAndSetStackCommands = GetGivenHandphaseActionAndSetStackCommands(PostflopCommands);

                TurnCommands = GetGivenHandphaseCommands(commands, HandPhaseEnum.Turn, LastHandPhase);
                TurnActionAndSetStackCommands = GetGivenHandphaseActionAndSetStackCommands(TurnCommands);

                RiverCommands = GetGivenHandphaseCommands(commands, HandPhaseEnum.River, LastHandPhase);
                RiverActionAndSetStackCommands = GetGivenHandphaseActionAndSetStackCommands(RiverCommands);

                ShowDownCommands = GetShowDownCommands(commands);
            }

            if (CashOrTournament == CashOrTournament.Cash)
            {
                AdjustPlayersAddedChipsField(commands, PreflopCommands, PostflopCommands, TurnCommands, RiverCommands, ShowDownCommands); // adjusts this.PlayerAddedAmountToStack                
                TotalRake = GetTotalRake(commands);    // it is already set to 0 if tournament
            }

        }

        private void InitializeTableData(List<Command> commands)
        {
            var handNumberTableName = commands.First(x => x.CommandCodeEnum == CommandCodeEnum.HandNumberTableName).CommandObject as HandNumberTableName;

            if (handNumberTableName == null)
            {
                throw new Exception("Invalid hand number");
            }

            HandNumber = handNumberTableName.HandNumber;
            CashOrTournament = handNumberTableName.IsTournament ? CashOrTournament.Tournament : CashOrTournament.Cash;
            TableType = handNumberTableName.TableType;
            TableID = handNumberTableName.TableID;
            HeroWasMoved = handNumberTableName.HeroWasMoved;
            GameFormat = handNumberTableName.GameFormat;
            LastNotHeroPlayerFinishedPlace = handNumberTableName.LastNotHeroPlayerFinishedPlace;
        }

        /// <summary>
        /// Clean up hand from duplicates 
        /// </summary>
        private List<Command> CleanUpCommandsFromDuplicates(List<Command> commands)
        {
            var duplicatesExist = commands.FilterCommands<HandPhaseV2>(CommandCodeEnum.HandPhaseV2)
                                            .Where(x => x.HandPhaseEnum == HandPhaseEnum.WaitingForSB).Count() > 1;

            if (duplicatesExist)
            {
                var cleanedCommands = new List<Command>();

                for (var i = 0; i < commands.Count; i++)
                {
                    if (i % 2 != 0)
                    {
                        continue;
                    }

                    cleanedCommands.Add(commands[i]);
                }

                commands = cleanedCommands;
            }

            return commands;
        }

        /// <summary>
        /// Build and add missed hand stacks to BeforeHandStacks
        /// </summary>
        private void RebuildMissedBeforeHandStacks()
        {
            var missedStacks = StacksAfterBlinds.Except(StacksBeforeHand, new LambdaComparer<KeyValuePair<int, decimal>>((x, y) => x.Key == y.Key)).ToArray();

            if (missedStacks.Length < 1)
            {
                return;
            }

            var actionsBeforeStacksAfterBlinds = new[] { PlayerActionEnum.Ante, PlayerActionEnum.PostSB, PlayerActionEnum.PostBB, PlayerActionEnum.Allin };

            var allInPlayers = new List<int>();

            var playersActions = Commands
                                        .TakeWhile(x => x.CommandCodeEnum != CommandCodeEnum.HandPhaseV2 ||
                                                        (x.CommandCodeEnum == CommandCodeEnum.HandPhaseV2 && ((HandPhaseV2)x.CommandObject).HandPhaseEnum != HandPhaseEnum.PostFlop))
                                        .Where(x => x.CommandCodeEnum == CommandCodeEnum.PlayerAction)
                                        .Select(x => x.CommandObject)
                                        .OfType<PlayerAction>()
                                        .ToArray();

            foreach (var missedStack in missedStacks)
            {
                var playerActions = playersActions.Where(x => x.SeatNumber == missedStack.Key && actionsBeforeStacksAfterBlinds.Contains(x.PlayerActionEnum)).ToArray();

                var stackSize = missedStack.Value;

                foreach (var playerAction in playerActions)
                {
                    switch (playerAction.PlayerActionEnum)
                    {
                        case PlayerActionEnum.Ante:
                            stackSize += Ante;
                            break;
                        case PlayerActionEnum.PostSB:
                            stackSize += SmallBlind;
                            break;
                        case PlayerActionEnum.PostBB:
                            stackSize += BigBlind;
                            break;
                        case PlayerActionEnum.Allin:
                            allInPlayers.Add(missedStack.Key);
                            break;
                        default:
                            break;

                    }
                }

                StacksBeforeHand.Add(missedStack.Key, stackSize);
            }

            var allInPlayersStacks = GetAllInPlayersStacks(allInPlayers);

            foreach (var allInPlayerStack in allInPlayersStacks)
            {
                StacksBeforeHand.Add(allInPlayerStack.Key, allInPlayerStack.Value);
            }

            StacksBeforeHand = StacksBeforeHand.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Get players stacks for newly added players if they go all-in before preflop
        /// </summary>
        /// <param name="allInPlayers">List of all-in players</param>
        /// <returns>Players stacks</returns>
        private Dictionary<int, decimal> GetAllInPlayersStacks(IList<int> allInPlayers)
        {
            var allInPlayerStacks = new Dictionary<int, decimal>();

            if (allInPlayers.Count < 1)
            {
                return allInPlayerStacks;
            }

            var afterShowStacksCommands = Commands
                                        .SkipWhile(x => x.CommandCodeEnum != CommandCodeEnum.HandPhaseV2 ||
                                            (x.CommandCodeEnum == CommandCodeEnum.HandPhaseV2 && ((HandPhaseV2)x.CommandObject).HandPhaseEnum != HandPhaseEnum.ShowStacks))
                                        .ToArray();

            var showStacksCommands = afterShowStacksCommands
                                        .Where(x => x.CommandCodeEnum == CommandCodeEnum.SetStack)
                                        .Select(x => x.CommandObject)
                                        .OfType<Stack>()
                                        .GroupBy(x => x.SeatNumber)
                                        .Select(x => x.FirstOrDefault())
                                        .ToDictionary(x => x.SeatNumber);

            var playerRemovedCommands = afterShowStacksCommands
                                  .Where(x => x.CommandCodeEnum == CommandCodeEnum.PlayerRemoved)
                                  .Select(x => x.CommandObject)
                                  .OfType<PlayerRemoved>()
                                  .GroupBy(x => x.SeatNumber)
                                  .Select(x => x.FirstOrDefault())
                                  .ToDictionary(x => x.SeatNumber);

            var winners = (from stackBeforeHand in StacksBeforeHand
                           join stackAfterHand in StacksAfterHand on stackBeforeHand.Key equals stackAfterHand.Key
                           where stackAfterHand.Value - stackBeforeHand.Value >= 0
                           select stackBeforeHand.Key).Concat(showStacksCommands.Select(x => x.Key)).ToArray();

            var playersWon = allInPlayers.Where(x => showStacksCommands.ContainsKey(x)).ToArray();
            var playersLost = allInPlayers.Except(playersWon).ToArray();

            var totalBeforeHand = StacksBeforeHand.Sum(x => x.Value);
            var totalAfterHand = StacksAfterHand.Sum(x => x.Value);
            var addedPlayerAddedStacks = totalAfterHand - totalBeforeHand;

            // only 1 all-in added player and he won then his initial stack size = total - total_prev_hand
            if (allInPlayers.Count == 1 && playersWon.Length > 0)
            {
                allInPlayerStacks.Add(allInPlayers.First(), addedPlayerAddedStacks);
            }
            // all all-in players lost, so we can't get determine their initial stack correctly, but it doesn't matter, just let stacks be equal to each other
            else if (playersWon.Length < 1)
            {
                var addedPlayerInitialStack = addedPlayerAddedStacks / allInPlayers.Count;

                foreach (var playerLost in playersLost)
                {
                    allInPlayerStacks.Add(playerLost, addedPlayerInitialStack);
                }
            }

            return allInPlayerStacks;
        }

        private class PotHeap
        {
            public decimal Size { get; set; }

            public HashSet<int> Seats { get; set; }
        }

        private List<int> GetPlayersWentAllin(List<Command> commands)
        {
            List<int> playersWentAllin = new List<int>();
            if (commands == null)
            {
                return null;
            }

            List<Command> setStackCommands = new List<Command>();

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum == CommandCodeEnum.SetStack)
                {
                    setStackCommands.Add(commands[i]);
                }
            }

            foreach (var item in setStackCommands)
            {
                var stackCommand = (Stack)item.CommandObject;
                if (stackCommand.StackValue == 0.0m)
                {
                    playersWentAllin.Add(stackCommand.SeatNumber);
                }
            }
            return playersWentAllin;
        }

        private IntPtr GetHandle(List<Command> commands)
        {
            return new IntPtr(commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.HandNumberTableName)
                           .Select(x => (HandNumberTableName)x.CommandObject)
                           .First().WindowHandle);
        }

        private decimal GetTournamentRake(List<Command> commands)
        {
            if (CashOrTournament == CashOrTournament.Cash)
                return 0.0m;
            else
                return commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.HandNumberTableName)
                           .Select(x => (HandNumberTableName)x.CommandObject)
                           .First().Rake;
        }

        private decimal GetTournamentBuyIn(List<Command> commands)
        {
            if (CashOrTournament == CashOrTournament.Cash)
                return 0.0m;
            else
                return commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.HandNumberTableName)
                           .Select(x => (HandNumberTableName)x.CommandObject)
                           .First().BuyIn;
        }

        private long GetTournamentNumber(List<Command> commands)
        {
            if (CashOrTournament == CashOrTournament.Cash)
                return 0;
            else
                return Convert.ToInt64(commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.HandNumberTableName)
                           .Select(x => (HandNumberTableName)x.CommandObject)
                           .First().TournamentID);
        }

        private void AdjustPlayersAddedChipsField(List<Command> commands, List<Command> preflopCommands, List<Command> postflopCommands, List<Command> turnCommands, List<Command> riverCommands, List<Command> showDownCommands)
        {
            // preflopCommands contains all commands between preflop handPhase command and postflop handPhase command
            Dictionary<int, PlayerLastActionAndStack> currentActionAndStack = new Dictionary<int, PlayerLastActionAndStack>();

            #region Initialization

            // initialize with stacks after blinds values
            if (StacksAfterBlinds.Count == 0)
            {
                return;
            }

            foreach (var item in StacksAfterBlinds)
            {
                currentActionAndStack.Add(item.Key, new PlayerLastActionAndStack(PlayerActionEnum.None, item.Value));
            }

            #endregion

            #region PREFLOP

            int firstPocketCardsIndex = 0;
            int firstStackOrActionIndexPreflop = 0;

            if (preflopCommands == null && postflopCommands == null && turnCommands == null && riverCommands == null && showDownCommands == null)
            {
                return;
            }

            if (preflopCommands != null)
            {
                // search for first pocketcards index
                for (int i = 0; i < preflopCommands.Count; i++)
                {
                    if (preflopCommands[i].CommandCodeEnum != CommandCodeEnum.PocketCards)
                    {
                        continue;
                    }
                    firstPocketCardsIndex = i;
                    break;
                }

                // search for first SetStack or PlayerAction index
                if (firstPocketCardsIndex < preflopCommands.Count &&
                    preflopCommands[firstPocketCardsIndex].CommandCodeEnum == CommandCodeEnum.PocketCards)
                {
                    for (int i = firstPocketCardsIndex; i < preflopCommands.Count; i++)
                    {
                        if (preflopCommands[i].CommandCodeEnum == CommandCodeEnum.PocketCards)
                        {
                            continue;
                        }
                        if (preflopCommands[i].CommandCodeEnum != CommandCodeEnum.SetStack &&
                            preflopCommands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                        {
                            continue;
                        }
                        firstStackOrActionIndexPreflop = i;
                        break;
                    }
                }
                else
                {
                    return;
                }

                if (firstStackOrActionIndexPreflop < preflopCommands.Count &&
                    (preflopCommands[firstStackOrActionIndexPreflop].CommandCodeEnum == CommandCodeEnum.SetStack ||
                     preflopCommands[firstStackOrActionIndexPreflop].CommandCodeEnum == CommandCodeEnum.PlayerAction))
                {
                    // check if first action is SetStack
                    if (preflopCommands[firstStackOrActionIndexPreflop].CommandCodeEnum == CommandCodeEnum.SetStack)
                    {// if first command is set stack, it is the command which adds chips
                        var ss = (Stack)preflopCommands[firstStackOrActionIndexPreflop].CommandObject;
                        if (SeatsDealt.Contains(ss.SeatNumber))
                        {
                            PlayersAddedAmountToStack.Add(new KeyValuePair<int, decimal>(ss.SeatNumber, ss.StackValue - currentActionAndStack[ss.SeatNumber].StackValue));
                        }
                    }

                    // first command is PlayerAction, but there can still be commands that add chips. Search them!
                    // those commands can exist only if player folded in previous turn 
                    bool addedChipsFlag = false;
                    for (int i = firstStackOrActionIndexPreflop; i < preflopCommands.Count; i++)
                    {
                        if (preflopCommands[i].CommandCodeEnum != CommandCodeEnum.SetStack &&
                            preflopCommands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                        {
                            continue;
                        }

                        if (preflopCommands[i].CommandCodeEnum == CommandCodeEnum.PlayerAction)
                        {
                            var pa = (PlayerAction)preflopCommands[i].CommandObject;
                            currentActionAndStack[pa.SeatNumber].Action = pa.PlayerActionEnum;
                            addedChipsFlag = false;
                        }
                        if (preflopCommands[i].CommandCodeEnum == CommandCodeEnum.SetStack)
                        {
                            var ss = (Stack)preflopCommands[i].CommandObject;
                            if (SeatsDealt.Contains(ss.SeatNumber) && !(SeatsAdded.Contains(ss.SeatNumber) && SeatsRemoved.Contains(ss.SeatNumber)) &&
                                currentActionAndStack.ContainsKey(ss.SeatNumber) && (currentActionAndStack[ss.SeatNumber].Action == PlayerActionEnum.Fold || currentActionAndStack[ss.SeatNumber].Action == PlayerActionEnum.FoldShow) && addedChipsFlag)   //trmt
                            { // player added some chips
                                PlayersAddedAmountToStack.Add(new KeyValuePair<int, decimal>(ss.SeatNumber, ss.StackValue - currentActionAndStack[ss.SeatNumber].StackValue));
                                addedChipsFlag = false;
                            }
                            else if (currentActionAndStack.ContainsKey(ss.SeatNumber))
                            {
                                currentActionAndStack[ss.SeatNumber].StackValue = ss.StackValue;
                                addedChipsFlag = true;
                            }
                        }
                    }

                }
            }
            #endregion

            #region FLOP

            int firstCommunityCardsIndex = 0;
            int firstStackOrActionIndexPostflop = 0;

            if (postflopCommands == null && turnCommands == null && riverCommands == null && showDownCommands == null)
            {
                return;
            }
            if (postflopCommands != null)
            {
                // search for first community cards index
                for (int i = 0; i < postflopCommands.Count; i++)
                {
                    if (postflopCommands[i].CommandCodeEnum != CommandCodeEnum.CommunityCard)
                    {
                        continue;
                    }
                    firstCommunityCardsIndex = i;
                    break;
                }

                // search for first SetStack or PlayerAction index
                if (firstCommunityCardsIndex < postflopCommands.Count &&
                    postflopCommands[firstCommunityCardsIndex].CommandCodeEnum == CommandCodeEnum.CommunityCard)
                {
                    for (int i = firstCommunityCardsIndex; i < postflopCommands.Count; i++)
                    {
                        if (postflopCommands[i].CommandCodeEnum == CommandCodeEnum.CommunityCard)
                        {
                            continue;
                        }
                        if (postflopCommands[i].CommandCodeEnum != CommandCodeEnum.SetStack &&
                            postflopCommands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                        {
                            continue;
                        }
                        firstStackOrActionIndexPostflop = i;
                        break;
                    }
                }
                else
                {
                    return;
                }

                if (firstStackOrActionIndexPostflop < postflopCommands.Count &&
                    (postflopCommands[firstStackOrActionIndexPostflop].CommandCodeEnum == CommandCodeEnum.SetStack ||
                     postflopCommands[firstStackOrActionIndexPostflop].CommandCodeEnum == CommandCodeEnum.PlayerAction))
                {
                    // check if first action is SetStack
                    if (postflopCommands[firstStackOrActionIndexPostflop].CommandCodeEnum == CommandCodeEnum.SetStack)
                    {// if first command is set stack, it is the command which adds chips
                        var ss = (Stack)postflopCommands[firstStackOrActionIndexPostflop].CommandObject;
                        if (SeatsDealt.Contains(ss.SeatNumber))
                        {
                            PlayersAddedAmountToStack.Add(new KeyValuePair<int, decimal>(ss.SeatNumber, ss.StackValue - currentActionAndStack[ss.SeatNumber].StackValue));
                        }
                    }

                    // first command is PlayerAction, but there can still be commands that add chips. Search them!
                    // those commands can exist only if player folded in previous turn 
                    bool addedChipsFlag = false;
                    for (int i = firstStackOrActionIndexPostflop; i < postflopCommands.Count; i++)
                    {
                        if (postflopCommands[i].CommandCodeEnum != CommandCodeEnum.SetStack &&
                            postflopCommands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                        {
                            continue;
                        }

                        if (postflopCommands[i].CommandCodeEnum == CommandCodeEnum.PlayerAction)
                        {
                            var pa = (PlayerAction)postflopCommands[i].CommandObject;
                            currentActionAndStack[pa.SeatNumber].Action = pa.PlayerActionEnum;
                            addedChipsFlag = false;
                        }
                        if (postflopCommands[i].CommandCodeEnum == CommandCodeEnum.SetStack)
                        {
                            var ss = (Stack)postflopCommands[i].CommandObject;
                            if (SeatsDealt.Contains(ss.SeatNumber) && !(SeatsAdded.Contains(ss.SeatNumber) && SeatsRemoved.Contains(ss.SeatNumber)) &&
                                currentActionAndStack.ContainsKey(ss.SeatNumber) && (currentActionAndStack[ss.SeatNumber].Action == PlayerActionEnum.Fold || currentActionAndStack[ss.SeatNumber].Action == PlayerActionEnum.FoldShow) && addedChipsFlag) // trmt
                            { // player added some chips
                                PlayersAddedAmountToStack.Add(new KeyValuePair<int, decimal>(ss.SeatNumber, ss.StackValue - currentActionAndStack[ss.SeatNumber].StackValue));
                                addedChipsFlag = false;
                            }
                            else if (currentActionAndStack.ContainsKey(ss.SeatNumber))
                            {
                                currentActionAndStack[ss.SeatNumber].StackValue = ss.StackValue;
                                addedChipsFlag = true;
                            }
                        }
                    }
                }
            }
            #endregion

            #region TURN

            int firstTurnCardIndex = 0;
            int firstStackOrActionIndexTurn = 0;

            if (turnCommands == null && riverCommands == null && showDownCommands == null)
            {
                return;
            }

            if (turnCommands != null)
            {
                // search for first community cards index
                for (int i = 0; i < turnCommands.Count; i++)
                {
                    if (turnCommands[i].CommandCodeEnum != CommandCodeEnum.CommunityCard)
                    {
                        continue;
                    }
                    firstTurnCardIndex = i;
                    break;
                }

                // search for first SetStack or PlayerAction index
                if (firstTurnCardIndex < turnCommands.Count &&
                    turnCommands[firstTurnCardIndex].CommandCodeEnum == CommandCodeEnum.CommunityCard)
                {
                    for (int i = firstTurnCardIndex; i < turnCommands.Count; i++)
                    {
                        if (turnCommands[i].CommandCodeEnum == CommandCodeEnum.CommunityCard)
                        {
                            continue;
                        }
                        if (turnCommands[i].CommandCodeEnum != CommandCodeEnum.SetStack &&
                            turnCommands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                        {
                            continue;
                        }
                        firstStackOrActionIndexTurn = i;
                        break;
                    }
                }
                else
                {
                    return;
                }

                if (firstStackOrActionIndexTurn < turnCommands.Count &&
                    (turnCommands[firstStackOrActionIndexTurn].CommandCodeEnum == CommandCodeEnum.SetStack ||
                     turnCommands[firstStackOrActionIndexTurn].CommandCodeEnum == CommandCodeEnum.PlayerAction))
                {
                    // check if first action is SetStack
                    if (turnCommands[firstStackOrActionIndexTurn].CommandCodeEnum == CommandCodeEnum.SetStack)
                    {// if first command is set stack, it is the command which adds chips
                        var ss = (Stack)turnCommands[firstStackOrActionIndexTurn].CommandObject;
                        if (SeatsDealt.Contains(ss.SeatNumber))
                        {
                            PlayersAddedAmountToStack.Add(new KeyValuePair<int, decimal>(ss.SeatNumber, ss.StackValue - currentActionAndStack[ss.SeatNumber].StackValue));
                        }
                    }

                    // first command is PlayerAction, but there can still be commands that add chips. Search them!
                    // those commands can exist only if player folded in previous turn 
                    bool addedChipsFlag = false;
                    for (int i = firstStackOrActionIndexTurn; i < turnCommands.Count; i++)
                    {
                        if (turnCommands[i].CommandCodeEnum != CommandCodeEnum.SetStack &&
                            turnCommands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                        {
                            continue;
                        }

                        if (turnCommands[i].CommandCodeEnum == CommandCodeEnum.PlayerAction)
                        {
                            var pa = (PlayerAction)turnCommands[i].CommandObject;
                            currentActionAndStack[pa.SeatNumber].Action = pa.PlayerActionEnum;
                            addedChipsFlag = false;
                        }
                        if (turnCommands[i].CommandCodeEnum == CommandCodeEnum.SetStack)
                        {
                            var ss = (Stack)turnCommands[i].CommandObject;
                            if (SeatsDealt.Contains(ss.SeatNumber) && !(SeatsAdded.Contains(ss.SeatNumber) && SeatsRemoved.Contains(ss.SeatNumber)) &&
                                currentActionAndStack.ContainsKey(ss.SeatNumber) && (currentActionAndStack[ss.SeatNumber].Action == PlayerActionEnum.Fold || currentActionAndStack[ss.SeatNumber].Action == PlayerActionEnum.FoldShow) && addedChipsFlag) //trmt
                            { // player added some chips
                                PlayersAddedAmountToStack.Add(new KeyValuePair<int, decimal>(ss.SeatNumber, ss.StackValue - currentActionAndStack[ss.SeatNumber].StackValue));
                                addedChipsFlag = false;
                            }
                            else if (currentActionAndStack.ContainsKey(ss.SeatNumber))
                            {
                                currentActionAndStack[ss.SeatNumber].StackValue = ss.StackValue;
                                addedChipsFlag = true;
                            }
                        }
                    }
                }
            }
            #endregion

            #region RIVER

            int firstRiverCardIndex = 0;
            int firstStackOrActionIndexRiver = 0;

            if (riverCommands == null && showDownCommands == null)
            {
                return;
            }

            if (riverCommands != null)
            {


                // search for first community cards index
                for (int i = 0; i < riverCommands.Count; i++)
                {
                    if (riverCommands[i].CommandCodeEnum != CommandCodeEnum.CommunityCard)
                    {
                        continue;
                    }
                    firstRiverCardIndex = i;
                    break;
                }

                // search for first SetStack or PlayerAction index
                if (firstRiverCardIndex < riverCommands.Count &&
                    riverCommands[firstRiverCardIndex].CommandCodeEnum == CommandCodeEnum.CommunityCard)
                {
                    for (int i = firstRiverCardIndex; i < riverCommands.Count; i++)
                    {
                        if (riverCommands[i].CommandCodeEnum == CommandCodeEnum.CommunityCard)
                        {
                            continue;
                        }
                        if (riverCommands[i].CommandCodeEnum != CommandCodeEnum.SetStack &&
                            riverCommands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                        {
                            continue;
                        }
                        firstStackOrActionIndexRiver = i;
                        break;
                    }
                }
                else
                {
                    return;
                }

                if (firstStackOrActionIndexRiver < riverCommands.Count &&
                    (riverCommands[firstStackOrActionIndexRiver].CommandCodeEnum == CommandCodeEnum.SetStack ||
                     riverCommands[firstStackOrActionIndexRiver].CommandCodeEnum == CommandCodeEnum.PlayerAction))
                {
                    // check if first action is SetStack
                    if (riverCommands[firstStackOrActionIndexRiver].CommandCodeEnum == CommandCodeEnum.SetStack)
                    {// if first command is set stack, it is the command which adds chips
                        var ss = (Stack)riverCommands[firstStackOrActionIndexRiver].CommandObject;
                        if (SeatsDealt.Contains(ss.SeatNumber))
                        {
                            PlayersAddedAmountToStack.Add(new KeyValuePair<int, decimal>(ss.SeatNumber, ss.StackValue - currentActionAndStack[ss.SeatNumber].StackValue));
                        }
                    }
                    // first command is PlayerAction, but there can still be commands that add chips. Search them!
                    // those commands can exist only if player folded in previous turn 
                    bool addedChipsFlag = false;
                    for (int i = firstStackOrActionIndexRiver; i < riverCommands.Count; i++)
                    {
                        if (riverCommands[i].CommandCodeEnum != CommandCodeEnum.SetStack &&
                            riverCommands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                        {
                            continue;
                        }

                        if (riverCommands[i].CommandCodeEnum == CommandCodeEnum.PlayerAction)
                        {
                            var pa = (PlayerAction)riverCommands[i].CommandObject;
                            currentActionAndStack[pa.SeatNumber].Action = pa.PlayerActionEnum;
                            addedChipsFlag = false;
                        }
                        if (riverCommands[i].CommandCodeEnum == CommandCodeEnum.SetStack)
                        {
                            var ss = (Stack)riverCommands[i].CommandObject;
                            if (SeatsDealt.Contains(ss.SeatNumber) && !(SeatsAdded.Contains(ss.SeatNumber) && SeatsRemoved.Contains(ss.SeatNumber)) &&
                                currentActionAndStack.ContainsKey(ss.SeatNumber) && (currentActionAndStack[ss.SeatNumber].Action == PlayerActionEnum.Fold || currentActionAndStack[ss.SeatNumber].Action == PlayerActionEnum.FoldShow) && addedChipsFlag) // trmt
                            { // player added some chips
                                PlayersAddedAmountToStack.Add(new KeyValuePair<int, decimal>(ss.SeatNumber, ss.StackValue - currentActionAndStack[ss.SeatNumber].StackValue));
                                addedChipsFlag = false;
                            }
                            else if (currentActionAndStack.ContainsKey(ss.SeatNumber))
                            {
                                currentActionAndStack[ss.SeatNumber].StackValue = ss.StackValue;
                                addedChipsFlag = true;
                            }
                        }
                    }
                }
            }
            #endregion

            #region SHOWDOWN

            int firstShowDownCardIndex = 0;
            int firstStackOrActionIndexShowDown = 0;

            if (showDownCommands == null)
            {
                return;
            }

            if (showDownCommands != null)
            {
                // search for first community cards index
                for (int i = 0; i < showDownCommands.Count; i++)
                {
                    if (showDownCommands[i].CommandCodeEnum != CommandCodeEnum.CommunityCard)
                    {
                        continue;
                    }
                    firstShowDownCardIndex = i;
                    break;
                }

                // search for first SetStack or PlayerAction index
                if (firstShowDownCardIndex < showDownCommands.Count)
                {
                    for (int i = firstShowDownCardIndex; i < showDownCommands.Count; i++)
                    {
                        if (showDownCommands[i].CommandCodeEnum == CommandCodeEnum.CommunityCard)
                        {
                            continue;
                        }
                        if (showDownCommands[i].CommandCodeEnum != CommandCodeEnum.SetStack &&
                            showDownCommands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                        {
                            continue;
                        }
                        firstStackOrActionIndexShowDown = i;
                        break;
                    }
                }
                else
                {
                    return;
                }

                if (firstStackOrActionIndexShowDown < showDownCommands.Count &&
                    (showDownCommands[firstStackOrActionIndexShowDown].CommandCodeEnum == CommandCodeEnum.SetStack ||
                     showDownCommands[firstStackOrActionIndexShowDown].CommandCodeEnum == CommandCodeEnum.PlayerAction))
                {
                    // check if first action is SetStack
                    if (showDownCommands[firstStackOrActionIndexShowDown].CommandCodeEnum == CommandCodeEnum.SetStack)
                    {// if first command is set stack, it is the command which adds chips
                        var ss = (Stack)showDownCommands[firstStackOrActionIndexShowDown].CommandObject;
                        if (SeatsDealt.Contains(ss.SeatNumber))
                        {
                            PlayersAddedAmountToStack.Add(new KeyValuePair<int, decimal>(ss.SeatNumber, ss.StackValue - currentActionAndStack[ss.SeatNumber].StackValue));
                        }
                    }
                    // first command is PlayerAction, but there can still be commands that add chips. Search them!
                    // those commands can exist only if player folded in previous turn 
                    bool addedChipsFlag = false;
                    for (int i = firstStackOrActionIndexShowDown; i < showDownCommands.Count; i++)
                    {
                        if (showDownCommands[i].CommandCodeEnum != CommandCodeEnum.SetStack &&
                            showDownCommands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                        {
                            continue;
                        }

                        if (showDownCommands[i].CommandCodeEnum == CommandCodeEnum.PlayerAction)
                        {
                            var pa = (PlayerAction)showDownCommands[i].CommandObject;
                            currentActionAndStack[pa.SeatNumber].Action = pa.PlayerActionEnum;
                            addedChipsFlag = false;
                        }
                        if (showDownCommands[i].CommandCodeEnum == CommandCodeEnum.SetStack)
                        {
                            var ss = (Stack)showDownCommands[i].CommandObject;
                            if (SeatsDealt.Contains(ss.SeatNumber) && !(SeatsAdded.Contains(ss.SeatNumber) && SeatsRemoved.Contains(ss.SeatNumber)) &&
                                currentActionAndStack.ContainsKey(ss.SeatNumber) && (currentActionAndStack[ss.SeatNumber].Action == PlayerActionEnum.Fold || currentActionAndStack[ss.SeatNumber].Action == PlayerActionEnum.FoldShow) && addedChipsFlag) // trmt
                            { // player added some chips
                                PlayersAddedAmountToStack.Add(new KeyValuePair<int, decimal>(ss.SeatNumber, ss.StackValue - currentActionAndStack[ss.SeatNumber].StackValue));
                                addedChipsFlag = false;
                            }
                            else if (currentActionAndStack.ContainsKey(ss.SeatNumber))
                            {
                                currentActionAndStack[ss.SeatNumber].StackValue = ss.StackValue;
                                addedChipsFlag = true;
                            }
                        }
                    }
                }
            }
            #endregion

            #region fix values to add up for every seat number separately


            List<KeyValuePair<int, decimal>> tempPlayersAddedAmountToStack = new List<KeyValuePair<int, decimal>>();
            foreach (var item in PlayersAddedAmountToStack)
            {
                tempPlayersAddedAmountToStack.Add(item);
            }
            PlayersAddedAmountToStack.Clear();

            Dictionary<int, decimal> dictPlayersAddedAmountToStack = new Dictionary<int, decimal>();
            for (int i = tempPlayersAddedAmountToStack.Count() - 1; i >= 0; i--)
            {
                if (!dictPlayersAddedAmountToStack.ContainsKey(tempPlayersAddedAmountToStack[i].Key))
                {
                    dictPlayersAddedAmountToStack.Add(tempPlayersAddedAmountToStack[i].Key, tempPlayersAddedAmountToStack[i].Value);
                }
            }
            foreach (var item in dictPlayersAddedAmountToStack)
            {
                PlayersAddedAmountToStack.Add(item);
            }

            #endregion
            return;
        }

        private List<int> GetPlayersBroke(List<Command> commands)
        {
            List<int> playersBroke = new List<int>();
            if (commands == null)
            {
                return null;
            }

            List<Command> setStackCommands = new List<Command>();

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum == CommandCodeEnum.SetStack)
                {
                    setStackCommands.Add(commands[i]);
                }
            }

            foreach (var item in setStackCommands)
            {
                var stackCommand = (Stack)item.CommandObject;
                if (stackCommand.StackValue == 0.0m)
                {
                    playersBroke.Add(stackCommand.SeatNumber);
                }
            }

            // fix players broke for HU (only 1 player can be broke, and this is not the one with bigger stack at the beginning!
            if (TableType == 2 && playersBroke.Count == 2)
            {
                if (StacksBeforeHand[1] > StacksBeforeHand[2])
                {
                    playersBroke.Remove(1);
                }
                else
                {
                    playersBroke.Remove(2);
                }
            }

            return playersBroke;
        }

        private List<int> GetSeatsOnTable(List<Command> commands)
        {
            return StacksAfterBlinds.Keys.ToList();
        }

        private List<Command> GetShowDownCommands(List<Command> commands)
        {
            List<Command> showDownCommands = new List<Command>();

            // search for showDown handphase command
            int k = 0;
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                {
                    continue;
                }
                var a = (HandPhaseV2)commands[i].CommandObject;
                if (a.HandPhaseEnum != HandPhaseEnum.ShowDown)
                {
                    continue;
                }
                else
                {
                    k = i;
                    break;
                }
            }

            int firstShowDownIndex = k + 1;

            k = 0;
            for (int i = firstShowDownIndex; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                {
                    continue;
                }
                else
                {
                    k = i;
                    break;
                }
            }
            for (int i = firstShowDownIndex; i < k; i++)
            {
                showDownCommands.Add(commands[i]);
            }

            return showDownCommands;
        }



        private List<Command> GetGivenHandphaseActionAndSetStackCommands(List<Command> givenHandPhaseCommands)
        {
            // returns only commands that are action or set stack
            // fixes list if new players are added to the table
            // updates list of players who added chips after folding
            if (givenHandPhaseCommands == null)
            {
                return null; // theres no approptriate hand phase in the hand
            }

            List<Command> givenHandPhaseActionCommands = new List<Command>();
            int k = 0;

            for (int i = 0; i < givenHandPhaseCommands.Count; i++)
            {
                if (givenHandPhaseCommands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                {
                    continue;
                }
                k = i;
                break;
            }

            int givenHandPhaseAction = k;   // first action command index in given handPhase 
            for (int i = givenHandPhaseAction; i < givenHandPhaseCommands.Count; i++)
            {
                if (givenHandPhaseCommands[i].CommandCodeEnum == CommandCodeEnum.PlayerAction || givenHandPhaseCommands[i].CommandCodeEnum == CommandCodeEnum.SetStack)
                {
                    givenHandPhaseActionCommands.Add(givenHandPhaseCommands[i]);
                }
            }

            // givenHandPhaseActionCommands contains only PlayerACtion and SetStack commands
            // next section filters list to have only elements that are in the game (filters out added players stacks)

            List<Command> givenHandPhaseActionCommandsFixed = new List<Command>();

            var listOfStackSeats = givenHandPhaseActionCommands.Where(x => x.CommandCodeEnum == CommandCodeEnum.SetStack)
                                                                        .Select(x => (Stack)x.CommandObject).ToList()
                                                                        .Select(x => x.SeatNumber).ToList();

            var listOfPlayerActionSeats = givenHandPhaseActionCommands.Where(x => x.CommandCodeEnum == CommandCodeEnum.PlayerAction)
                                                                        .Select(x => (PlayerAction)x.CommandObject)
                                                                        .ToList().Select(x => x.SeatNumber).ToList();

            for (int i = 0; i < givenHandPhaseActionCommands.Count; i++)
            {
                if (givenHandPhaseActionCommands[i].CommandCodeEnum == CommandCodeEnum.PlayerAction)
                {
                    var paObject = (PlayerAction)givenHandPhaseActionCommands[i].CommandObject;
                    int currentSeat = paObject.SeatNumber;
                    if (listOfStackSeats.Contains(currentSeat))
                    {
                        givenHandPhaseActionCommandsFixed.Add(givenHandPhaseActionCommands[i]);
                    }
                }

                if (givenHandPhaseActionCommands[i].CommandCodeEnum == CommandCodeEnum.SetStack)
                {
                    var ssObject = (Stack)givenHandPhaseActionCommands[i].CommandObject;
                    int currentSeat = ssObject.SeatNumber;
                    if (listOfPlayerActionSeats.Contains(currentSeat) && givenHandPhaseActionCommandsFixed.Last().CommandCodeEnum == CommandCodeEnum.PlayerAction)
                    {
                        givenHandPhaseActionCommandsFixed.Add(givenHandPhaseActionCommands[i]);
                    }
                }
            }

            return givenHandPhaseActionCommandsFixed;
        }

        private List<Command> GetGivenHandphaseCommands(List<Command> commands, HandPhaseEnum givenHandPhase, HandPhaseEnum lastHandPhase)
        {
            // returns only actions in given hand phase
            List<Command> givenHandPhaseCommands = new List<Command>();

            // if theres no givenHandPhase in the hand return null
            if (givenHandPhase > lastHandPhase)
            {
                return null;
            }

            HandPhaseEnum nextHandPhase = GetNextHandPhase(givenHandPhase, lastHandPhase);

            // if givenHandPhase is equal to lastHandPhase, it means that nextHandPhase ShowDown
            if (givenHandPhase == lastHandPhase)
            {
                nextHandPhase = HandPhaseEnum.ShowDown;
            }

            // search for preflop handphase command
            int k = 0;

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                {
                    continue;
                }
                var a = (HandPhaseV2)commands[i].CommandObject;
                if (a.HandPhaseEnum != givenHandPhase)
                {
                    continue;
                }
                else
                {
                    k = i;
                    break;
                }
            }

            int firstGivenHandphaseIndex = k;

            if (firstGivenHandphaseIndex == 0)
            {
                return givenHandPhaseCommands;
            }

            k = 0;

            for (int i = firstGivenHandphaseIndex + 1; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                {
                    continue;
                }

                k = i;
                break;
            }

            int firstNextHandphaseIndex = k;

            for (int i = firstGivenHandphaseIndex; i < firstNextHandphaseIndex; i++)
            {
                givenHandPhaseCommands.Add(commands[i]);
            }

            return givenHandPhaseCommands;
        }

        private HandPhaseEnum GetNextHandPhase(HandPhaseEnum givenHandPhase, HandPhaseEnum lastHandPhase)
        {
            HandPhaseEnum nextHandPhase = HandPhaseEnum.NotEnoughPlayers;

            if (givenHandPhase == lastHandPhase)
            {
                return lastHandPhase;
            }

            foreach (HandPhaseEnum handPhase in Enum.GetValues(typeof(HandPhaseEnum)))
            {
                if (handPhase > givenHandPhase)
                {
                    nextHandPhase = handPhase;
                    break;
                }
            }

            return nextHandPhase;
        }

        private HandPhaseEnum GetLastHandPhase(List<Command> commands)
        {
            HandPhaseEnum lastHandPhase = HandPhaseEnum.NotEnoughPlayers;
            List<HandPhaseEnum> handPhases = new List<HandPhaseEnum>();

            foreach (var item in commands)
            {
                if (item.CommandCodeEnum == CommandCodeEnum.HandPhaseV2)
                {
                    var a = (HandPhaseV2)item.CommandObject;
                    handPhases.Add(a.HandPhaseEnum);
                }
            }
            foreach (var item in handPhases)
            {
                if (item != HandPhaseEnum.ShowDown)
                {
                    lastHandPhase = (HandPhaseEnum)item;
                }
                else
                {
                    break;
                }
            }
            return lastHandPhase;
        }

        private Dictionary<HandPhaseEnum, List<Command>> GetZonePokerRoundCommands(List<Command> commands)
        {
            var zonePokerRoundCommands = new Dictionary<HandPhaseEnum, List<Command>>();

            foreach (HandPhaseEnum handPhase in Enum.GetValues(typeof(HandPhaseEnum)))
            {
                zonePokerRoundCommands.Add(handPhase, new List<Command>());
            }

            var currentHandPhase = HandPhaseEnum.NotEnoughPlayers;
            var preflopPhaseFound = false;

            foreach (var command in commands)
            {
                if (command.CommandCodeEnum == CommandCodeEnum.HandPhaseV2)
                {
                    var currentHandPhaseObject = (HandPhaseV2)command.CommandObject;
                    currentHandPhase = currentHandPhaseObject.HandPhaseEnum;

                    if (!preflopPhaseFound && currentHandPhase == HandPhaseEnum.Preflop)
                    {
                        preflopPhaseFound = true;
                    }

                    continue;
                }

                if (!preflopPhaseFound)
                {
                    continue;
                }

                zonePokerRoundCommands[currentHandPhase].Add(command);
            }

            return zonePokerRoundCommands;
        }

        private Dictionary<int, decimal> GetStacksAfterHand(List<Command> commands)
        {
            // it might happen that player immediately adds some more chips to his stack after the hands is over
            // in that case, for this seat, return the smaller stack size, ats it is a base to calculate hand winner, rake, etc
            Dictionary<int, decimal> endStacks = new Dictionary<int, decimal>();
            List<Command> endStacksList = new List<Command>();
            int k = 0;

            // get index of stateChanged = 0 command (not enough players)
            int indexOfstateChanged0Command = -1;
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                {
                    continue;
                }
                var a = (HandPhaseV2)commands[i].CommandObject;
                if (a.HandPhaseEnum != HandPhaseEnum.NotEnoughPlayers)
                {
                    continue;
                }
                indexOfstateChanged0Command = i;
                // no break because we are searching for last command with those conditions                        
            }

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                {
                    continue;
                }
                var a = (HandPhaseV2)commands[i].CommandObject;
                if (a.HandPhaseEnum != HandPhaseEnum.ShowStacks)
                {
                    continue;
                }
                else
                {
                    k = i;
                    break;
                }
            }
            if (k < commands.Count)
            {
                while (k < commands.Count)
                {
                    if (indexOfstateChanged0Command == -1)
                    {
                        if (commands[k].CommandCodeEnum == CommandCodeEnum.SetStack)
                        {
                            endStacksList.Add(commands[k]);
                        }
                        k++;
                    }
                    else
                    {
                        if (commands[k].CommandCodeEnum == CommandCodeEnum.SetStack && k < indexOfstateChanged0Command)
                        {
                            endStacksList.Add(commands[k]);
                        }
                        k++;
                    }

                }
            }

            List<int> seatsNonUnique = new List<int>();
            List<int> seatsUnique = new List<int>();
            foreach (var item in endStacksList)
            {
                var stackCom = (Stack)item.CommandObject;
                seatsNonUnique.Add(stackCom.SeatNumber);
                if (!seatsUnique.Contains(stackCom.SeatNumber))
                {
                    seatsUnique.Add(stackCom.SeatNumber);
                }
            }

            Dictionary<int, decimal> endStacksFixed = new Dictionary<int, decimal>();

            foreach (var item in seatsUnique)
            {
                if (seatsNonUnique.Where(x => x == item).Count() == 1)
                {
                    foreach (var el in endStacksList)
                    {
                        var stackCom = (Stack)el.CommandObject;
                        if (stackCom.SeatNumber == item)
                        {
                            endStacksFixed.Add(stackCom.SeatNumber, stackCom.StackValue);
                        }
                    }
                }
                else
                {
                    //Seatnumber: item shows more than once in endStackList
                    List<KeyValuePair<int, decimal>> doubled = new List<KeyValuePair<int, decimal>>();
                    foreach (var el in endStacksList)
                    {
                        var stackCom = (Stack)el.CommandObject;
                        if (stackCom.SeatNumber == item)
                        {
                            doubled.Add(new KeyValuePair<int, decimal>(stackCom.SeatNumber, stackCom.StackValue));
                        }
                    }
                    // get smaller stack                    
                    decimal min = decimal.MaxValue;
                    KeyValuePair<int, decimal> minimalStackElement = new KeyValuePair<int, decimal>();
                    foreach (var el in doubled)
                    {
                        if (el.Value < min)
                        {
                            minimalStackElement = el;
                            min = el.Value;
                        }
                    }
                    endStacksFixed.Add(minimalStackElement.Key, minimalStackElement.Value);
                }
            }

            if (CashOrTournament == CashOrTournament.Tournament)
            {
                Dictionary<int, decimal> endStacksTournament = new Dictionary<int, decimal>();
                foreach (var item in endStacksFixed)
                {
                    endStacksTournament.Add(item.Key, item.Value * 100);
                }
                endStacksFixed = endStacksTournament;
            }

            return endStacksFixed;
        }

        private int GetSetStackSingleIndex(List<Command> commands, int seat)
        {
            // get last index before showdown that is single set stack command for the seat
            int showDownIndex = 0;
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                {
                    continue;
                }
                var a = (HandPhaseV2)commands[i].CommandObject;
                if (a.HandPhaseEnum == HandPhaseEnum.ShowDown)
                {
                    showDownIndex = i;
                }
            }
            if (showDownIndex == 0)
            {
                return -1;
            }
            for (int i = showDownIndex; i > 0; i--)
            {
                if (commands[i].CommandCodeEnum != CommandCodeEnum.SetStack)
                {
                    continue;
                }
                var a = (Stack)commands[i].CommandObject;
                if (a.SeatNumber != seat)
                {
                    continue;
                }
                if (i > 0 && commands[i--].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                {
                    return i;
                }
            }
            return -1;
        }

        private Dictionary<int, decimal> GetStacksBeforeHand(List<Command> commands)
        {
            // add fix to get second value if some seat number repeats
            Dictionary<int, decimal> beginStacks = new Dictionary<int, decimal>();
            try
            {
                List<Command> beginStacksList = commands.Skip(1).TakeWhile(x => x.CommandCodeEnum == CommandCodeEnum.SetStack).ToList();

                List<int> seatsNonUnique = new List<int>();
                List<int> seatsUnique = new List<int>();
                foreach (var item in beginStacksList)
                {
                    var stackCom = (Stack)item.CommandObject;
                    seatsNonUnique.Add(stackCom.SeatNumber);
                    if (!seatsUnique.Contains(stackCom.SeatNumber))
                    {
                        seatsUnique.Add(stackCom.SeatNumber);
                    }
                }

                Dictionary<int, decimal> beginStacksFixed = new Dictionary<int, decimal>();

                foreach (var item in seatsUnique)
                {
                    if (seatsNonUnique.Where(x => x == item).Count() == 1)
                    {
                        foreach (var el in beginStacksList)
                        {
                            var stackCom = (Stack)el.CommandObject;
                            if (stackCom.SeatNumber == item)
                            {
                                beginStacksFixed.Add(stackCom.SeatNumber, stackCom.StackValue);
                            }
                        }
                    }
                    else
                    {
                        //Seatnumber: item shows more than once in endStackList
                        List<KeyValuePair<int, decimal>> doubled = new List<KeyValuePair<int, decimal>>();
                        foreach (var el in beginStacksList)
                        {
                            var stackCom = (Stack)el.CommandObject;
                            if (stackCom.SeatNumber == item)
                            {
                                doubled.Add(new KeyValuePair<int, decimal>(stackCom.SeatNumber, stackCom.StackValue));
                            }
                        }
                        // get bigger stack                    
                        decimal max = decimal.MinValue;
                        KeyValuePair<int, decimal> maximalStackElement = new KeyValuePair<int, decimal>();
                        foreach (var el in doubled)
                        {
                            if (el.Value > max)
                            {
                                maximalStackElement = el;
                                max = el.Value;
                            }
                        }
                        beginStacksFixed.Add(maximalStackElement.Key, maximalStackElement.Value);
                    }
                }

                beginStacks = beginStacksFixed;

                if (CashOrTournament == CashOrTournament.Tournament)
                {
                    Dictionary<int, decimal> beginStacksTournament = new Dictionary<int, decimal>();
                    foreach (var item in beginStacksFixed)
                    {
                        beginStacksTournament.Add(item.Key, item.Value * 100);
                    }

                    beginStacks = beginStacksTournament;
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Error getting StacksBeforeHand", ex);
            }
            return beginStacks;
        }

        private Dictionary<int, int> GetSeatsPlayerID(List<Command> commands)
        {
            Dictionary<int, int> seatsPlayerID = new Dictionary<int, int>();
            try
            {
                List<Command> afterBlindsStacksList = new List<Command>();
                int k = 0;
                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                    {
                        continue;
                    }
                    var a = (HandPhaseV2)commands[i].CommandObject;
                    if (a.HandPhaseEnum != HandPhaseEnum.Preflop)
                    {
                        continue;
                    }
                    else
                    {
                        k = i;
                        break;
                    }
                }

                if (k < commands.Count)
                {
                    while (k < commands.Count)
                    {
                        if (commands[k].CommandCodeEnum == CommandCodeEnum.SetStack)
                        {
                            afterBlindsStacksList.Add(commands[k]);
                        }
                        else if (commands[k].CommandCodeEnum == CommandCodeEnum.PocketCards)
                        {
                            break;
                        }
                        k++;
                    }
                }

                foreach (var item in afterBlindsStacksList)
                {
                    var stackCom = (Stack)item.CommandObject;

                    if (!seatsPlayerID.ContainsKey(stackCom.SeatNumber))
                    {
                        seatsPlayerID.Add(stackCom.SeatNumber, stackCom.PlayerID);
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Error: GetSeatsPlayerID in hand", ex);
            }

            return seatsPlayerID;
        }

        private Dictionary<int, decimal> GetStacksAfterBlinds(List<Command> commands)
        {
            var afterBlindsStacks = new Dictionary<int, decimal>();

            try
            {
                var afterBlindsStacksList = new List<Command>();

                int k = 0;

                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                    {
                        continue;
                    }

                    var a = (HandPhaseV2)commands[i].CommandObject;

                    if (a.HandPhaseEnum != HandPhaseEnum.Preflop)
                    {
                        continue;
                    }
                    else
                    {
                        k = i;
                        break;
                    }
                }

                if (k < commands.Count)
                {
                    while (k < commands.Count)
                    {
                        if (commands[k].CommandCodeEnum == CommandCodeEnum.SetStack)
                        {
                            afterBlindsStacksList.Add(commands[k]);
                        }
                        else if (commands[k].CommandCodeEnum == CommandCodeEnum.PocketCards)
                        {
                            break;
                        }

                        k++;
                    }
                }

                foreach (var item in afterBlindsStacksList)
                {
                    var stackCom = (Stack)item.CommandObject;

                    if (!afterBlindsStacks.ContainsKey(stackCom.SeatNumber))
                    {
                        afterBlindsStacks.Add(stackCom.SeatNumber, stackCom.StackValue);
                    }
                }

                if (CashOrTournament == CashOrTournament.Tournament)
                {
                    var afterBlindsStacksTournament = new Dictionary<int, decimal>();

                    foreach (var item in afterBlindsStacks)
                    {
                        afterBlindsStacksTournament.Add(item.Key, item.Value * 100);
                    }

                    afterBlindsStacks = afterBlindsStacksTournament;
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Error: GetStacksAfterBlinds in hand: ", ex);
            }

            return afterBlindsStacks;
        }

        private Dictionary<int, decimal> GetDeadBlinds(List<Command> commands)        // update after update of dll to post stacks after blinds posting
        {
            Dictionary<int, decimal> deadBlinds = new Dictionary<int, decimal>();

            var postDeadCommands = commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.PlayerAction)
                                            .Select(x => (PlayerAction)x.CommandObject)
                                            .ToList()
                                            .Where(x => x.PlayerActionEnum == PlayerActionEnum.PostDead || x.PlayerActionEnum == PlayerActionEnum.Post)
                                            .ToList();

            List<int> seatsPostingBlind = new List<int>();
            foreach (var item in postDeadCommands)
            {
                seatsPostingBlind.Add(item.SeatNumber);
            }

            Dictionary<int, decimal> beginStacks = StacksBeforeHand;
            Dictionary<int, decimal> afterBlindsStacks = StacksAfterBlinds;

            foreach (var item in seatsPostingBlind)
            {
                decimal stackBeforeBlinds = 0.0m;
                decimal stackAfterBlinds = 0.0m;

                try
                {
                    stackBeforeBlinds = beginStacks.Where(x => x.Key == item).First().Value;
                    stackAfterBlinds = afterBlindsStacks.Where(x => x.Key == item).First().Value;
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(this, "Missing stack for dead blind", ex);
                }

                deadBlinds.Add(item, stackBeforeBlinds - stackAfterBlinds);
            }
            return deadBlinds;
        }
        private List<int> GetPlayersBrokeAndAddedChips()
        {
            List<int> playersBrokeAndAddedChips = new List<int>();
            // get only players removed
            //List<int> seatsRemoved = this.SeatsRemoved;//new List<int>();
            //foreach (var item in this.SeatsRemovedOrAddedCommands)
            //{
            //    if (item.CommandCodeEnum == CommandCodeEnum.PlayerRemoved)
            //    {
            //        var pr = (PlayerRemoved)item.CommandObject;
            //        seatsRemoved.Add(pr.SeatNumber);
            //    }
            //}

            // get if player added chips (if he is out of order in stacks after hand
            var arr = StacksAfterHand.Keys.ToArray();
            List<int> playersOutOfOrderInStacksAfterHandList = new List<int>();
            int prev = 0;
            foreach (var item in arr)
            {
                if (item < prev)
                {
                    playersOutOfOrderInStacksAfterHandList.Add(item);
                }
                prev = item;
            }

            foreach (var item in SeatsDealt)
            {
                if (PlayersBroke.Contains(item) && !SeatsRemoved.Contains(item) && playersOutOfOrderInStacksAfterHandList.Contains(item))
                {// if player went all in and lost and he did not leavfe the table, exclude him from rake calculation!
                    playersBrokeAndAddedChips.Add(item);
                }
            }
            return playersBrokeAndAddedChips;
        }

        private decimal GetTotalRake(List<Command> commands)
        {
            decimal valueToReturn = 0.0m;
            try
            {
                //Dictionary<int, decimal> beginStacks = this.StacksBeforeHand;
                Dictionary<int, decimal> beginStacksDealt = GetDealtBeginEndStacks(StacksBeforeHand, SeatsDealt);
                Dictionary<int, decimal> endStacksDealt = GetDealtBeginEndStacks(StacksAfterHand, SeatsDealt);
                List<int> playersLeft = GetPlayersLeft(SeatsRemovedOrAddedCommands, commands);
                List<int> playersAdded = GetPlayersAdded(SeatsRemovedOrAddedCommands, commands);
                List<int> playersBrokeAndAddedChips = GetPlayersBrokeAndAddedChips();
                List<int> intersection = FindIntersection(beginStacksDealt.Keys.ToList(), endStacksDealt.Keys.ToList());

                decimal sumBeginExcludeLeft = GetSumExcludeLeft(beginStacksDealt, playersLeft);
                decimal sumEndExcludeAdded = GetSumExcludeAdded(endStacksDealt, playersAdded, playersBrokeAndAddedChips);
                //decimal sumBegin = GetSumOfStacks(beginStacksDealt);
                //decimal sumEnd = GetSumOfStacks(endStacksDealt);

                //decimal sumBeginIntersect = GetSumIntersection(beginStacksDealt, intersection);
                //decimal sumEndIntersect = GetSumIntersection(endStacksDealt, intersection);

                decimal leftPlayersInvestments = GetPossibleLeftPlayerInvestments(playersLeft, SmallBlindSeat, SmallBlind, BigBlindSeat, BigBlind, DeadBlinds, commands);

                decimal sumOfAddedAmounts = GetSumOfAddedAmounts(PlayersAddedAmountToStack, playersLeft, playersBrokeAndAddedChips, StacksAfterHand);

                valueToReturn = sumBeginExcludeLeft - sumEndExcludeAdded + leftPlayersInvestments + sumOfAddedAmounts;
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Error calculating rake", ex);
            }
            return valueToReturn;
        }

        private decimal GetSumOfAddedAmounts(List<KeyValuePair<int, decimal>> playersAddedAmountToStack, List<int> playersLeft, List<int> playersBrokeAndAddedChips, Dictionary<int, decimal> stacksAfterHand)
        {
            decimal result = 0.0m;

            if (playersAddedAmountToStack == null)
            {
                return 0.0m;
            }

            foreach (var item in playersAddedAmountToStack)
            {
                if (!playersLeft.Contains(item.Key))
                {
                    result += item.Value;
                }
            }

            //foreach (var item in playersBrokeAndAddedChips)
            //{
            //    if (stacksAfterHand.Keys.Contains(item))
            //    {
            //        result -= stacksAfterHand[item];
            //    }
            //}
            return result;
        }

        private decimal GetSumExcludeAdded(Dictionary<int, decimal> endStacksDealt, List<int> playersAdded, List<int> playersBrokeAndAddedChips)
        {
            decimal sum = 0.0m;
            foreach (var item in endStacksDealt)
            {
                // test
                //if (!playersBrokeAndAddedChips.Contains(item.Key))
                //{
                //    sum += item.Value;
                //}
                // was before test
                if (!playersAdded.Contains(item.Key) && !playersBrokeAndAddedChips.Contains(item.Key))
                {
                    sum += item.Value;
                }
            }
            return sum;
        }
        private decimal GetSumExcludeLeft(Dictionary<int, decimal> stacksDict, List<int> playersLeft)
        {
            decimal sum = 0.0m;
            foreach (var item in stacksDict)
            {
                if (!playersLeft.Contains(item.Key))
                {
                    sum += item.Value;
                }
            }
            return sum;
        }

        private decimal GetSumOfStacks(Dictionary<int, decimal> stacksDict)
        {
            decimal sum = 0.0m;
            foreach (var item in stacksDict)
            {
                sum += item.Value;
            }
            return sum;
        }

        private decimal GetSumIntersection(Dictionary<int, decimal> stacksDict, List<int> intersection)
        {
            decimal sum = 0.0m;
            foreach (var item in stacksDict)
            {
                if (intersection.Contains(item.Key))
                {
                    sum += item.Value;
                }
            }
            return sum;
        }

        private Dictionary<int, decimal> GetDealtBeginEndStacks(Dictionary<int, decimal> stacks, List<int> seatsDealt)
        {
            Dictionary<int, decimal> result = new Dictionary<int, decimal>();
            foreach (var item in seatsDealt)
            {
                if (stacks.Keys.Contains(item))
                {
                    result.Add(item, stacks[item]);
                }
            }
            return result;
        }

        private decimal GetPossibleLeftPlayerInvestments(List<int> playersLeft, int sbSeat, decimal sb, int bbSeat, decimal bb, Dictionary<int, decimal> deadBlinds, List<Command> commands)
        {
            // players that have setStack 0 command should not be treated as left!
            decimal result = 0.0m;

            foreach (var item in playersLeft)
            {
                if (StacksBeforeHand.Keys.Contains(item))
                {
                    decimal stackValue = GetLastIndexOfPlayerLeftAction(item, commands);
                    result += StacksBeforeHand[item] - stackValue;
                }
            }
            if (result >= 0)
            {
                return result;
            }
            else
            {
                return 0.0m;
            }
        }

        private decimal GetLastIndexOfPlayerLeftAction(int seat, List<Command> commands)
        {
            int indexOfPlayerRemovedLine = GetIndexOfPlayerRemovedLine(seat, commands);

            decimal valueToReturn = 0.0m;
            int k = 0;

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum != CommandCodeEnum.PlayerAction)
                {
                    continue;
                }
                var a = (PlayerAction)commands[i].CommandObject;
                if (a.SeatNumber != seat)
                {
                    continue;
                }
                if (a.PlayerActionEnum == PlayerActionEnum.Call
                    || a.PlayerActionEnum == PlayerActionEnum.Check
                    || a.PlayerActionEnum == PlayerActionEnum.Bet
                    || a.PlayerActionEnum == PlayerActionEnum.RaiseTo
                    || a.PlayerActionEnum == PlayerActionEnum.Allin
                    || a.PlayerActionEnum == PlayerActionEnum.AllinRaise
                    // test if next 3 lines are fine!
                    || a.PlayerActionEnum == PlayerActionEnum.PostSB
                    || a.PlayerActionEnum == PlayerActionEnum.PostBB
                    || a.PlayerActionEnum == PlayerActionEnum.PostDead)
                {
                    k = i;
                    break;
                }
            }
            if (k < commands.Count)
            {
                while (k < commands.Count)
                {
                    if (commands[k].CommandCodeEnum == CommandCodeEnum.SetStack)
                    {
                        var s = (Stack)commands[k].CommandObject;
                        if (s.SeatNumber == seat)
                        {
                            //return s.StackValue;
                            if (k < indexOfPlayerRemovedLine)
                            {
                                valueToReturn = s.StackValue;
                            }
                        }
                    }
                    k++;
                }
            }
            return valueToReturn;
        }

        private int GetIndexOfPlayerRemovedLine(int seat, List<Command> commands)
        {
            //int playerRemovedIndex = 0;
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum == CommandCodeEnum.PlayerRemoved)
                {
                    var pa = (PlayerRemoved)commands[i].CommandObject;
                    if (pa.SeatNumber == seat)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

        private List<int> FindIntersection(List<int> list1, List<int> list2)
        {
            List<int> result = new List<int>();
            foreach (var item in list1)
            {
                if (list2.Contains(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private List<int> GetPlayersLeft(List<Command> seatsRemovedOrAdded, List<Command> commands)
        {
            List<int> seatsToExclude = new List<int>();
            foreach (var item in seatsRemovedOrAdded)
            {
                if (item.CommandCodeEnum == CommandCodeEnum.PlayerRemoved)
                {
                    var pr = (PlayerRemoved)item.CommandObject;

                    // get index of stateChanged = 0 command (not enough players)
                    int indexOfstateChanged0Command = -1;
                    for (int i = 0; i < commands.Count; i++)
                    {
                        if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                        {
                            continue;
                        }
                        var a = (HandPhaseV2)commands[i].CommandObject;
                        if (a.HandPhaseEnum != HandPhaseEnum.NotEnoughPlayers)
                        {
                            continue;
                        }
                        indexOfstateChanged0Command = i;
                        // no break because we are searching for last command with those conditions                        
                    }


                    // get index of a command where player is left and determine if it is after his stack is shown at the end of the hand
                    int indexOfLastSetStackCommand = 0;
                    for (int i = 0; i < commands.Count; i++)
                    {
                        if (commands[i].CommandCodeEnum != CommandCodeEnum.SetStack)
                        {
                            continue;
                        }
                        var a = (Stack)commands[i].CommandObject;
                        if (a.SeatNumber != pr.SeatNumber)
                        {
                            continue;
                        }

                        if (indexOfstateChanged0Command != -1)
                        { // exclude commands that are after "Not enought players" command
                            if (i < indexOfstateChanged0Command)
                            {
                                indexOfLastSetStackCommand = i;
                            }
                        }
                        else
                        {
                            indexOfLastSetStackCommand = i;
                        }

                        //if (indexOfstateChanged0Command != -1 && i < indexOfstateChanged0Command)        // exclude commands that are after "Not enought players" command
                        //{
                        //    indexOfLastSetStackCommand = i;
                        //}

                        // no break because we are searching for last command with those conditions                        
                    }
                    // get index of playerRemovedCommand
                    int indexOfPlayerRemovedCommand = 0;
                    for (int i = 0; i < commands.Count; i++)
                    {
                        if (commands[i].CommandCodeEnum != CommandCodeEnum.PlayerRemoved)
                        {
                            continue;
                        }
                        var a = (PlayerRemoved)commands[i].CommandObject;
                        if (a.SeatNumber != pr.SeatNumber)
                        {
                            continue;
                        }
                        indexOfPlayerRemovedCommand = i;
                        break;
                    }

                    // get index of stateChanged 65536 command
                    int showStacksIndex = 0;
                    for (int i = 0; i < commands.Count; i++)
                    {
                        if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                        {
                            continue;
                        }
                        var a = (HandPhaseV2)commands[i].CommandObject;
                        if (a.HandPhaseEnum != HandPhaseEnum.ShowStacks)
                        {
                            continue;
                        }
                        else
                        {
                            showStacksIndex = i;
                            break;
                        }
                    }



                    var b = (PlayerRemoved)item.CommandObject;
                    if (!PlayersWentAllin.Contains(b.SeatNumber))
                    { // if player went allin and left should not be treated as left                        
                        if (showStacksIndex < indexOfLastSetStackCommand && indexOfLastSetStackCommand < indexOfPlayerRemovedCommand)
                        {// do not add this seat to removed players if he is left after show stacks commands and his stack has been shown to the stream
                            continue;
                        }
                        seatsToExclude.Add(b.SeatNumber);
                    }
                }
            }

            return seatsToExclude;
        }

        private List<int> GetPlayersAdded(List<Command> seatsRemovedOrAdded, List<Command> commands)
        {// should not take into account players that are added after + stateChanged 0  command             (10.06.2015)


            //List<int> seatsToExclude = new List<int>();           // previous solution
            //foreach (var item in seatsRemovedOrAdded)
            //{
            //    if (item.CommandCodeEnum == CommandCodeEnum.PlayerAdded)
            //    {
            //        var b = (PlayerAdded)item.CommandObject;
            //        seatsToExclude.Add(b.SeatNumber);
            //    }
            //}
            //return seatsToExclude;

            List<int> seatsToExclude = new List<int>();
            foreach (var item in seatsRemovedOrAdded)
            {
                if (item.CommandCodeEnum == CommandCodeEnum.PlayerAdded)
                {
                    var pa = (PlayerAdded)item.CommandObject;
                    // get index of a command where player is added and determine if it is after stateChanged 0  command
                    int indexOfstateChanged0Command = -1;
                    for (int i = 0; i < commands.Count; i++)
                    {
                        if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                        {
                            continue;
                        }
                        var a = (HandPhaseV2)commands[i].CommandObject;
                        if (a.HandPhaseEnum != HandPhaseEnum.NotEnoughPlayers)
                        {
                            continue;
                        }
                        indexOfstateChanged0Command = i;
                        // no break because we are searching for last command with those conditions                        
                    }

                    // get index of playerAddedCommand
                    int indexOfPlayerAddedCommand = 0;
                    for (int i = 0; i < commands.Count; i++)
                    {
                        if (commands[i].CommandCodeEnum != CommandCodeEnum.PlayerAdded)
                        {
                            continue;
                        }
                        var a = (PlayerAdded)commands[i].CommandObject;
                        if (a.SeatNumber != pa.SeatNumber)
                        {
                            continue;
                        }
                        indexOfPlayerAddedCommand = i;
                        break;
                    }


                    if (indexOfstateChanged0Command != -1 && indexOfPlayerAddedCommand > indexOfstateChanged0Command)
                    {
                        continue;
                    }

                    seatsToExclude.Add(pa.SeatNumber);
                }
            }
            return seatsToExclude;

        }

        private List<Command> GetSeatsRemovedOrAddedCommands(List<Command> commands)
        {
            var seatsRemovedOrAdded = new List<Command>();

            for (int i = 0; i < commands.Count; i++)
            {
                if ((commands[i].CommandCodeEnum == CommandCodeEnum.PlayerAdded || commands[i].CommandCodeEnum == CommandCodeEnum.PlayerRemoved))
                {
                    seatsRemovedOrAdded.Add(commands[i]);
                }
            }
            return seatsRemovedOrAdded;
        }

        private List<int> GetSeatsAdded(List<Command> commands)
        {
            List<Command> playerAddedCommands = new List<Command>();
            List<int> seatsAdded = new List<int>();

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum == CommandCodeEnum.PlayerAdded)
                {
                    playerAddedCommands.Add(commands[i]);
                }
            }
            foreach (var item in playerAddedCommands)
            {
                var playerAddedObject = (PlayerAdded)item.CommandObject;
                seatsAdded.Add(playerAddedObject.SeatNumber);
            }
            return seatsAdded;

            // old solution:

            //List<int> seatsAdded = new List<int>();
            //List<PlayerAdded> playersAdded = commands.Where(x=>x.CommandCodeEnum == CommandCodeEnum.PlayerAdded)
            //                                        .Select(x=>(PlayerAdded)x.CommandObject)
            //                                        .ToList();
            //foreach (var item in playersAdded)
            //{
            //    seatsAdded.Add(item.SeatNumber);                
            //}
            //return seatsAdded;
        }

        private List<int> GetSeatsRemoved(List<Command> commands)
        {
            List<Command> playerRemovedCommands = new List<Command>();
            List<int> seatsRemoved = new List<int>();

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum == CommandCodeEnum.PlayerRemoved)
                {
                    playerRemovedCommands.Add(commands[i]);
                }
            }
            foreach (var item in playerRemovedCommands)
            {
                var playerAddedObject = (PlayerRemoved)item.CommandObject;
                seatsRemoved.Add(playerAddedObject.SeatNumber);
            }
            return seatsRemoved;

            // old solution:

            //List<int> seatsRemoved = new List<int>();
            //List<PlayerRemoved> playersRemoved = commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.PlayerRemoved)
            //                                        .Select(x => (PlayerRemoved)x.CommandObject)
            //                                        .ToList();
            //foreach (var item in playersRemoved)
            //{
            //    seatsRemoved.Add(item.SeatNumber);
            //}            
            //return seatsRemoved;
        }

        private decimal GetAnte(List<Command> commands, Dictionary<int, decimal> beginStacks, CashOrTournament cashOrTournament)
        {
            decimal result = 0.0m;
            //Dictionary<int, decimal> beginStacks = this.StacksBeforeHand;
            if (cashOrTournament != CashOrTournament.Tournament)
            {
                return 0.0m;
            }

            PlayerAction playerAction = new PlayerAction();
            if (commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.PlayerAction)
                                        .Select(x => (PlayerAction)x.CommandObject)
                                        .Where(x => x.PlayerActionEnum == PlayerActionEnum.Ante)
                                        .Count() > 0)
            {
                int anteSeatNumber = commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.PlayerAction)
                                        .Select(x => (PlayerAction)x.CommandObject)
                                        .Where(x => x.PlayerActionEnum == PlayerActionEnum.Ante)
                                        .First().SeatNumber;

                // get first SetStack command of a player (seatnumber) after dealer command

                decimal postAnteStackValue = 0.0m;

                int k = 0;
                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i].CommandCodeEnum != CommandCodeEnum.DealerSeat)
                    {
                        continue;
                    }
                    else
                    {
                        k = i;
                        break;
                    }
                }
                int firstPostButtonCommand = k + 1;

                for (int i = firstPostButtonCommand; i < commands.Count; i++)
                {
                    if (commands[i].CommandCodeEnum != CommandCodeEnum.SetStack)
                    {
                        continue;
                    }
                    var a = (Stack)commands[i].CommandObject;
                    if (a.SeatNumber != anteSeatNumber)
                    {
                        continue;
                    }
                    else
                    {
                        postAnteStackValue = a.StackValue * 100;
                        break;
                    }
                }
                return beginStacks[anteSeatNumber] - postAnteStackValue;
            }
            return result;
        }

        private decimal GetSeatBlindsStackChange(int seatNumber, decimal ante)
        {
            var result = 0m;

            if (seatNumber < 1)
            {
                LogProvider.Log.Warn(this, string.Format("Couldn't get blinds stack change for seat #{0} [Hand = {1}]", seatNumber, HandNumber));
                return result;
            }

            if (!StacksBeforeHand.ContainsKey(seatNumber))
            {
                LogProvider.Log.Error(this, string.Format("Couldn't get blinds stack change for seat #{0}, initial stack size is unknown [Hand = {1}]", seatNumber, HandNumber));
                return result;
            }

            if (!StacksAfterBlinds.ContainsKey(seatNumber))
            {
                LogProvider.Log.Error(this, string.Format("Couldn't get blinds stack change for seat #{0}, after ante/blinds stack size is unknown [Hand = {1}]", seatNumber, HandNumber));
                return result;
            }

            var stackBeforeBlinds = StacksBeforeHand[seatNumber];
            var stackAfterBlinds = StacksAfterBlinds[seatNumber];

            result = stackBeforeBlinds - stackAfterBlinds - ante;

            return result;
        }

        private string GetStake(CashOrTournament cashOrTournament, decimal smallBlind, decimal bigBlind)
        {
            if (cashOrTournament == CashOrTournament.Cash)
            {
                return "";
            }

            var expectedSmallBlind = bigBlind / 2;

            // big blind is less than it should be
            if (expectedSmallBlind <= smallBlind)
            {
                return DecimalToString(smallBlind) + "/" + DecimalToString(smallBlind * 2);
            }
            else
            {
                return DecimalToString(bigBlind / 2) + "/" + DecimalToString(bigBlind);
            }
        }

        private static string DecimalToString(decimal? myNumber)
        {
            var s = string.Format(CultureInfo.InvariantCulture, "{0:0.00}", myNumber);

            if (s.EndsWith("00"))
            {
                return ((int)myNumber).ToString();
            }
            else
            {
                return s;
            }
        }

        private List<int> GetSeatsDealt(List<Command> commands)
        {
            List<int> seatsDealt = new List<int>();
            List<PocketCards> pocketCardsList = commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.PocketCards)
                                          .Select(x => (PocketCards)x.CommandObject)
                                          .ToList();
            foreach (var item in pocketCardsList)
            {
                if (!seatsDealt.Contains(item.SeatNumber))
                {
                    seatsDealt.Add(item.SeatNumber);
                }
            }
            return seatsDealt;
        }

        /// <summary>
        /// Get table name from specified commands
        /// </summary>
        /// <param name="commands">Specified commands list</param>
        /// <returns>Table name and original table name (Item 1 - table name, Item 2 - original table name)</returns>
        private string GetTableName(List<Command> commands)
        {
            var tableName = commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.HandNumberTableName)
                           .Select(x => (HandNumberTableName)x.CommandObject)
                           .First().TableName;

            return tableName;
        }

        /// <summary>
        /// Detect if table name is longer than 2 words
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <returns>True - if table contains more than 2 words, otherwise false</returns>
        private bool GetIsLongTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return false;
            }

            var tableNameWords = tableName.Split(' ');

            return tableNameWords.Length > 2;
        }

        private int GetHeroSeat(List<Command> commands)
        {
            var pocketCards = new PocketCards();

            var pocketCardsCommands = commands.FilterCommands<PocketCards>(CommandCodeEnum.PocketCards).ToArray();

            if (pocketCardsCommands.Where(x => !x.Card.Contains("Error") && !string.IsNullOrWhiteSpace(x.Card)).Count() > 0)
            {
                pocketCards = pocketCardsCommands
                                .Where(x => !x.Card.Contains("Error") && !string.IsNullOrWhiteSpace(x.Card))
                                .First();
            }
            else
            {
                // try to find hero add command
                if (pocketCards.SeatNumber == -1)
                {
                    var heroAddedCommand = commands.FilterCommands<PlayerAdded>(CommandCodeEnum.PlayerAdded).Where(x => x.IsHero).FirstOrDefault();

                    if (heroAddedCommand != null)
                    {
                        HeroHasBeenAdded = pocketCardsCommands.Any(x => x.SeatNumber == heroAddedCommand.SeatNumber);

                        return heroAddedCommand.SeatNumber;
                    }
                }
            }

            return pocketCards.SeatNumber;
        }

        /// <summary>
        /// Get blind commands
        /// </summary>
        /// <param name="commands">Hand commands</param>
        /// <returns>Commands between WaitingForBB and Preflop</returns>
        private List<Command> GetBlindsCommands(List<Command> commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            var blindsCommands = new List<Command>();

            // Index of WaitingForBB command
            int waitingForBBCommandIndex = 0;

            // Get index of WaitingForBB command
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                {
                    continue;
                }

                var handPhase = (HandPhaseV2)commands[i].CommandObject;

                if (handPhase.HandPhaseEnum != HandPhaseEnum.WaitingForBB)
                {
                    continue;
                }
                else
                {
                    waitingForBBCommandIndex = i;
                    break;
                }
            }

            // Take all PlayerAction commands until Preflop
            for (int i = waitingForBBCommandIndex + 1; i < commands.Count; i++)
            {
                if (commands[i].CommandCodeEnum == CommandCodeEnum.PlayerAction)
                {
                    blindsCommands.Add(commands[i]);
                }
                else
                {
                    if (commands[i].CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
                    {
                        continue;
                    }

                    var handPhase = (HandPhaseV2)commands[i].CommandObject;

                    if (handPhase.HandPhaseEnum == HandPhaseEnum.Preflop)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return blindsCommands;
        }

        /// <summary>
        /// Get blind seats
        /// </summary>
        /// <param name="commands">Hand commands</param>
        /// <returns>Small blind seat and Big blind seat (SB - Item1, BB - Item2)</returns>
        private Tuple<int, int> GetBlindsSeats(List<Command> commands)
        {
            var blindsCommands = GetBlindsCommands(commands);

            var invalidSeatNumber = -1;

            var blindsPlayerActions = blindsCommands.Select(x => x.CommandObject).OfType<PlayerAction>().ToArray();

            var sbPlayerAction = blindsPlayerActions.FirstOrDefault(x => x.PlayerActionEnum == PlayerActionEnum.PostSB);
            var bbPlayerAction = blindsPlayerActions.FirstOrDefault(x => x.PlayerActionEnum == PlayerActionEnum.PostBB);

            // SB goes all-in
            if (sbPlayerAction == null && bbPlayerAction != null)
            {
                sbPlayerAction = blindsPlayerActions.FirstOrDefault(x => x.PlayerActionEnum == PlayerActionEnum.Allin);
            }
            // BB goes all-in
            else if (bbPlayerAction == null && sbPlayerAction != null)
            {
                bbPlayerAction = blindsPlayerActions.FirstOrDefault(x => x.PlayerActionEnum == PlayerActionEnum.Allin);
            }
            // SB & BB go all-in
            else if (bbPlayerAction == null && sbPlayerAction == null && blindsPlayerActions.Length > 1)
            {
                sbPlayerAction = blindsPlayerActions.FirstOrDefault();
                bbPlayerAction = blindsPlayerActions.LastOrDefault();
            }

            var sbSeatNumber = sbPlayerAction != null ? sbPlayerAction.SeatNumber : invalidSeatNumber;
            var bbSeatNumber = bbPlayerAction != null ? bbPlayerAction.SeatNumber : invalidSeatNumber;

            var blindsSeats = new Tuple<int, int>(sbSeatNumber, bbSeatNumber);

            return blindsSeats;
        }

        private int GetDealerSeat(List<Command> commands)
        {
            var dealerSeat = commands.FilterCommands<DealerSeat>(CommandCodeEnum.DealerSeat).FirstOrDefault();

            if (dealerSeat != null)
            {
                return dealerSeat.SeatNumber;
            }

            if (TableType == 2)
            {
                return SmallBlindSeat;
            }

            if (SeatsDealt == null)
            {
                return -1;
            }
            else if (SeatsDealt.Count == 2)
            {
                return SmallBlindSeat;
            }
            else if (SeatsDealt.Count == 3)
            {
                var button = FindButton3Players(SeatsDealt, SmallBlindSeat, BigBlindSeat);
                return button;
            }
            else if (SeatsDealt.Count >= 4)
            {
                var button = FindButton4Players(SeatsDealt, SmallBlindSeat, BigBlindSeat, DeadBlinds);
                return button;
            }

            return -1;
        }

        private static int FindButton3Players(List<int> seatsDealt, int smallBlindSeat, int bigBlindSeat)
        {
            int button = -1;

            foreach (var item in seatsDealt)
            {
                if (item != smallBlindSeat && item != bigBlindSeat)
                {
                    button = item;
                }
            }
            return button;
        }

        private static int FindButton4Players(List<int> seatsDealt, int smallBlindSeat, int bigBlindSeat, Dictionary<int, decimal> deadBlinds)
        {
            int button = -1;

            foreach (var item in seatsDealt)
            {
                if (item != smallBlindSeat && item != bigBlindSeat && !deadBlinds.Keys.Contains(item))
                {
                    button = item;
                }
            }

            return button;
        }

        private string GetHeroCards(List<Command> commands)
        {
            string heroCards = "";
            List<PocketCards> pocketCardsList = commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.PocketCards)
                                          .Select(x => (PocketCards)x.CommandObject)
                                          .Where(x => !x.Card.Contains("Error") && !String.IsNullOrWhiteSpace(x.Card))
                                          .ToList();

            foreach (var item in pocketCardsList)
            {
                heroCards += item.Card + " ";
            }

            return heroCards.TrimEnd(' ');
        }

        private CashOrTournament GetCashOrTournament(List<Command> commands)
        {
            if (commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.HandNumberTableName)
                           .Select(x => (HandNumberTableName)x.CommandObject)
                           .First().IsTournament)
            {
                return CashOrTournament.Tournament;
            }
            else
            {
                return CashOrTournament.Cash;
            }
        }

        private bool? GetIsLastHandOfTheTournament()
        {
            // Pattern for recognizing last hand of the tournament
            // HERO at seat 3 (playerId 2) has finished the tournament in # 3 place. (cmd: 11 size: 32)
            // ++ Player left from seat 3 (cmd: 8 size: 24)
            // + stateChanged 0 (cmd: 4 size: 24 handNnr: 3171051336)

            // When second:

            //++ HERO at seat 8 (playerId 6) has finished the tournament in # 2 place. (cmd: 11 size: 32)
            //++ Player at seat 5 (playerId 5) has finished the tournament in # 1 place. (cmd: 11 size: 32)
            //++ Player left from seat 8 (cmd: 8 size: 24)
            //++ Player left from seat 5 (cmd: 8 size: 24)
            //+ stateChanged 0 (cmd: 4 size: 24 handNnr: 3192893408)

            if (Commands.Last().CommandCodeEnum != CommandCodeEnum.HandPhaseV2)
            {// last command is not + stateChanged X (cmd: 4 size: 24 handNnr: 3171051336)
                return false;
            }
            var lastCommand = (HandPhaseV2)Commands.Last().CommandObject;
            if (lastCommand.HandPhaseEnum != HandPhaseEnum.NotEnoughPlayers)
            {// last command is not + stateChanged 0 (cmd: 4 size: 24 handNnr: 3171051336)
                return false;
            }

            var heroFinishedComand = Commands.Where(x => x.CommandCodeEnum == CommandCodeEnum.TourneyPlayerFinished)
                                                .Select(x => (TourneyPlayerFinished)x.CommandObject)
                                                .ToList().Select(x => x.IsHero == true).ToList();

            if (heroFinishedComand != null && heroFinishedComand.Count > 0 && heroFinishedComand.Where(x => x == true).Count() > 0)
            {
                return true;
            }

            return false;
        }

        private List<TourneyPlayerFinished> GetPlayerFinishedTournament()
        {
            var tourneyFinishedCommands = Commands.FilterCommands<TourneyPlayerFinished>(CommandCodeEnum.TourneyPlayerFinished).ToList();
            return tourneyFinishedCommands;
        }
    }
}