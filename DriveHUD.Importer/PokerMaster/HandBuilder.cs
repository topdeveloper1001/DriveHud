//-----------------------------------------------------------------------
// <copyright file="HandBuilder.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.Importers.PokerMaster.Model;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Objects.Utils;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.PokerMaster
{
    internal class HandBuilder : IHandBuilder
    {
        private Dictionary<long, Dictionary<long, List<SCGameRoomStateChange>>> handsRoomStateChanges = new Dictionary<long, Dictionary<long, List<SCGameRoomStateChange>>>();

        private EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.PokerMaster;
            }
        }

        public bool TryBuild(SCGameRoomStateChange gameRoomStateChange, long heroId, out HandHistory handHistory)
        {
            handHistory = null;

            if (gameRoomStateChange == null)
            {
                return false;
            }

            if (!handsRoomStateChanges.TryGetValue(heroId, out Dictionary<long, List<SCGameRoomStateChange>> userGameRoomStateChanges))
            {
                userGameRoomStateChanges = new Dictionary<long, List<SCGameRoomStateChange>>();
                handsRoomStateChanges.Add(heroId, userGameRoomStateChanges);
            }

            if (!userGameRoomStateChanges.TryGetValue(gameRoomStateChange.GameNumber, out List<SCGameRoomStateChange> gameRoomStateChanges))
            {
                gameRoomStateChanges = new List<SCGameRoomStateChange>();
                userGameRoomStateChanges.Add(gameRoomStateChange.GameNumber, gameRoomStateChanges);
            }

            if (gameRoomStateChange.GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_SHOWCARD
                || gameRoomStateChange.GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_GameWait
                || gameRoomStateChange.GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_GamePrepare)
            {
                return false;
            }

            gameRoomStateChanges.Add(gameRoomStateChange);

            if (gameRoomStateChange.GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_Result)
            {
                handHistory = BuildHand(gameRoomStateChange.GameNumber, gameRoomStateChanges, heroId);
            }

            return handHistory != null;
        }

        private HandHistory BuildHand(long gameNumber, List<SCGameRoomStateChange> gameRoomStateChanges, long heroId)
        {
            try
            {
                var startRoomStateChange = gameRoomStateChanges.FirstOrDefault(x => x.GameRoomInfo != null &&
                    x.GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_GameStart);

                if (startRoomStateChange == null)
                {
                    var roomStateChange = gameRoomStateChanges.FirstOrDefault();

                    if (roomStateChange != null && roomStateChange.GameRoomInfo != null)
                    {
                        var roomId = roomStateChange.GameRoomInfo.IsTournament ?
                            roomStateChange.GameRoomInfo.SNGGameRoomBaseInfo?.GameRoomId :
                            roomStateChange.GameRoomInfo.GameRoomBaseInfo?.GameRoomId;

                        LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"Hand #{gameNumber} has no start info. Room #{roomId} [{Site}]");
                    }
                    else
                    {
                        LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"Hand #{gameNumber} has no start info. [{Site}]");
                    }

                    return null;
                }

                var handHistory = new HandHistory
                {
                    HandId = gameNumber
                };

                ParseStartRoomStateChange(startRoomStateChange, handHistory);

                var previousUserGameInfoDict = new Dictionary<string, UserGameInfoNet>();

                foreach (var gameRoomStateChange in gameRoomStateChanges)
                {
                    ParseActionsRoomStateChange(gameRoomStateChange, handHistory, previousUserGameInfoDict, heroId);

                    if (gameRoomStateChange.IsSummary)
                    {
                        ParseSummaryRoomStateChange(gameRoomStateChange, handHistory, heroId);
                        break;
                    }
                }

                AdjustHandHistory(handHistory, heroId);

                return handHistory;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"Could not build hand #{gameNumber} [{heroId}]", e);
            }
            finally
            {
                handsRoomStateChanges.Remove(gameNumber);
            }

            return null;
        }

        private void ParseStartRoomStateChange(SCGameRoomStateChange startRoomStateChange, HandHistory handHistory)
        {
            var gameRoomInfo = GetGameRoomInfo(startRoomStateChange, handHistory);

            if (gameRoomInfo.IsTournament)
            {
                ParseTournamentStartRoomStateChange(startRoomStateChange, gameRoomInfo, handHistory);
                return;
            }

            ParseCashStartRoomStateChange(startRoomStateChange, gameRoomInfo, handHistory);
        }

        private void ParseCashStartRoomStateChange(SCGameRoomStateChange startRoomStateChange, GameRoomInfo gameRoomInfo, HandHistory handHistory)
        {
            var gameRoomBaseInfo = gameRoomInfo.GameRoomBaseInfo ?? throw new HandBuilderException(handHistory.HandId, "GameRoomBaseInfo must be not empty.");

            handHistory.TableName = startRoomStateChange.GameRoomInfo?.GameRoomBaseInfo?.RoomName;
            handHistory.GameDescription = new GameDescriptor(
                    EnumPokerSites.PokerMaster,
                    ParseGameType(gameRoomInfo),
                    Limit.FromSmallBlindBigBlind(gameRoomBaseInfo.SmallBlinds, gameRoomBaseInfo.BigBlinds, Currency.YUAN, gameRoomBaseInfo.IsAnteRoom, gameRoomBaseInfo.Ante),
                    ParseTableType(gameRoomInfo),
                    SeatType.FromMaxPlayers(gameRoomBaseInfo.GameRoomUserMaxNums),
                    null
                );

            handHistory.GameDescription.IsStraddle = gameRoomBaseInfo.Straddle;
            handHistory.GameDescription.Identifier = gameRoomBaseInfo.GameRoomId;
        }

        private void ParseTournamentStartRoomStateChange(SCGameRoomStateChange startRoomStateChange, GameRoomInfo gameRoomInfo, HandHistory handHistory)
        {
            var gameRoomBaseInfo = gameRoomInfo.SNGGameRoomBaseInfo ?? throw new HandBuilderException(handHistory.HandId, "SNGGameRoomBaseInfo must be not empty.");

            handHistory.TableName = startRoomStateChange.GameRoomInfo?.SNGGameRoomBaseInfo?.RoomName;
            handHistory.GameDescription = new GameDescriptor(
                    PokerFormat.Tournament,
                    EnumPokerSites.PokerMaster,
                    ParseGameType(gameRoomInfo),
                    Limit.FromSmallBlindBigBlind(gameRoomBaseInfo.SmallBlinds, gameRoomBaseInfo.BigBlinds, Currency.YUAN, true, gameRoomBaseInfo.Ante),
                    ParseTableType(gameRoomInfo),
                    SeatType.FromMaxPlayers(gameRoomBaseInfo.GameRoomUserMaxNums),
                    ParseTournamentDescriptor(gameRoomInfo)
                )
            {
                Identifier = gameRoomBaseInfo.GameRoomId
            };
        }

        /// <summary>
        /// Parses hand action of the specified <see cref="SCGameRoomStateChange"/>
        /// </summary>
        /// <param name="startRoomStateChange">Room state change to parse</param>
        /// <param name="handHistory">Hand history</param>
        private void ParseActionsRoomStateChange(SCGameRoomStateChange startRoomStateChange, HandHistory handHistory, Dictionary<string, UserGameInfoNet> previousUserGameInfoDict, long heroId)
        {
            var gameRoomInfo = GetGameRoomInfo(startRoomStateChange, handHistory);
            var userGameInfos = GetUserGameInfos(gameRoomInfo, handHistory);

            // first action time is hand time
            if (userGameInfos.Length > 0 && handHistory.DateOfHandUtc == DateTime.MinValue)
            {
                var actTime = userGameInfos.Max(x => x.ActTime);

                if (actTime != 0)
                {
                    handHistory.DateOfHandUtc = DateTimeHelper.UnixTimeToDateTime(actTime / 1000);
                }
            }

            for (var seat = 0; seat < userGameInfos.Length; seat++)
            {
                var userGameInfo = userGameInfos[seat];

                if (!userGameInfo.IsActive)
                {
                    continue;
                }

                if (userGameInfo.UserInfo == null)
                {
                    throw new HandBuilderException(handHistory.HandId, "UserGameInfo.UserInfo has not been found.");
                }

                // game dealer
                if (handHistory.DealerButtonPosition == 0 && userGameInfos[seat].GameDealer)
                {
                    handHistory.DealerButtonPosition = seat + 1;
                }

                userGameInfo.RoomGameState = gameRoomInfo.GameState;

                AddOrUpdatePlayer(userGameInfo, seat + 1, handHistory, heroId);

                previousUserGameInfoDict.TryGetValue(userGameInfo.UserInfo.ShowID, out UserGameInfoNet previousUserGameInfo);

                var handAction = ParseHandAction(userGameInfo, previousUserGameInfo, handHistory);

                if (handAction != null)
                {
                    handHistory.HandActions.Add(handAction);
                }

                if (previousUserGameInfo == null)
                {
                    previousUserGameInfoDict.Add(userGameInfo.UserInfo.ShowID, userGameInfo);
                    continue;
                }

                previousUserGameInfoDict[userGameInfo.UserInfo.ShowID] = userGameInfo;
            }
        }

        private Player AddOrUpdatePlayer(UserGameInfoNet userGameInfo, int seat, HandHistory handHistory, long heroId)
        {
            var player = handHistory.Players[userGameInfo.UserInfo.ShowID];

            if (player == null)
            {
                player = new Player(userGameInfo.UserInfo.ShowID, 0, seat)
                {
                    PlayerNick = userGameInfo.UserInfo.Nick
                };

                handHistory.Players.Add(player);

                if (heroId == userGameInfo.UserInfo.Uuid)
                {
                    handHistory.Hero = player;
                }
            }

            if (userGameInfo.CurrentHands == null || player.hasHoleCards)
            {
                return player;
            }

            handHistory.Players[userGameInfo.UserInfo.ShowID].HoleCards = HoleCards.FromCards(userGameInfo.UserInfo.ShowID,
                userGameInfo.CurrentHands.Select(c => Card.GetPMCardFromIntValue(c)).ToArray());

            return player;
        }

        private void ParseSummaryRoomStateChange(SCGameRoomStateChange startRoomStateChange, HandHistory handHistory, long heroId)
        {
            var gameRoomInfo = GetGameRoomInfo(startRoomStateChange, handHistory);
            var userGameInfos = GetUserGameInfos(gameRoomInfo, handHistory);
            var gameResultInfo = gameRoomInfo.GameResultInfo ?? throw new HandBuilderException(handHistory.HandId, "GameResultInfo has not been found.");
            var gamePotResultInfo = gameResultInfo.GamePotResultInfo ?? throw new HandBuilderException(handHistory.HandId, "GamePotResultInfo has not been found.");

            if (gameResultInfo.OriginalUserStacks == null || gameResultInfo.OriginalUserStacks.Length != userGameInfos.Length)
            {
                throw new HandBuilderException(handHistory.HandId, "OriginalUserStacks are empty or has wrong length.");
            }

            // update original stacks
            for (var seat = 0; seat < userGameInfos.Length; seat++)
            {
                if (!userGameInfos[seat].IsActive)
                {
                    continue;
                }

                var player = AddOrUpdatePlayer(userGameInfos[seat], seat + 1, handHistory, heroId);
                player.StartingStack = gameResultInfo.OriginalUserStacks[seat];
            }

            // parse win amounts
            for (var potResultNum = 0; potResultNum < gamePotResultInfo.Length; potResultNum++)
            {
                var userWinners = gamePotResultInfo[potResultNum].UserWiner;
                var userPots = gamePotResultInfo[potResultNum].UserPots;

                if (userWinners.Length != userPots.Length)
                {
                    throw new HandBuilderException(handHistory.HandId, "UserWinners length isn't equal to UserPots length.");
                }

                for (var winnerNum = 0; winnerNum < userWinners.Length; winnerNum++)
                {
                    var userWinner = userWinners[winnerNum];

                    var winner = handHistory.Players[userWinner.ShowID];

                    if (winner == null)
                    {
                        throw new HandBuilderException(handHistory.HandId, $"UserWinner [{userWinner.ShowID}] has not been found in the player list.");
                    }

                    winner.Win += userPots[winnerNum];
                }
            }

            // parse community cards
            if (gameRoomInfo.CurrentCards != null && gameRoomInfo.CurrentCards.Length > 0)
            {
                handHistory.CommunityCards = BoardCards.FromCards(gameRoomInfo
                    .CurrentCards
                    .Select(c => Card.GetPMCardFromIntValue(c))
                    .ToArray());
            }
        }

        private void AdjustHandHistory(HandHistory handHistory, long heroId)
        {
            // replace 1st raise with bet
            ReplaceFirstRaiseWithBet(handHistory.Flop);
            ReplaceFirstRaiseWithBet(handHistory.Turn);
            ReplaceFirstRaiseWithBet(handHistory.River);

            HandHistoryUtils.AddShowActions(handHistory);
            HandHistoryUtils.AddWinningActions(handHistory);
            HandHistoryUtils.CalculateBets(handHistory);
            HandHistoryUtils.CalculateUncalledBets(handHistory, true);
            HandHistoryUtils.CalculateTotalPot(handHistory);
            HandHistoryUtils.SortHandActions(handHistory);
            HandHistoryUtils.RemoveSittingOutPlayers(handHistory);
        }

        private void ReplaceFirstRaiseWithBet(IEnumerable<HandAction> handActions)
        {
            var raiseAction = handActions.FirstOrDefault(x => x.IsRaise());

            if (raiseAction == null)
            {
                return;
            }

            if (raiseAction.IsAllInAction && (raiseAction is AllInAction allInAction))
            {
                allInAction.SourceActionType = HandActionType.BET;
                return;
            }

            raiseAction.HandActionType = HandActionType.BET;
        }

        private TableType ParseTableType(GameRoomInfo gameRoomInfo)
        {
            if (gameRoomInfo.GameRoomType == GameRoomType.GAME_ROOM_SNG && gameRoomInfo.SNGGameRoomBaseInfo != null)
            {
                switch (gameRoomInfo.SNGGameRoomBaseInfo.SNGRoomtype)
                {
                    case SNGRoomType.DEEP_SNG:
                        return TableType.FromTableTypeDescriptions(TableTypeDescription.Deep);
                    case SNGRoomType.LONG_SNG:
                        return TableType.FromTableTypeDescriptions(TableTypeDescription.Slow);
                    case SNGRoomType.QUICK_SNG:
                        return TableType.FromTableTypeDescriptions(TableTypeDescription.Speed);
                }
            }

            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        private GameType ParseGameType(GameRoomInfo gameRoomInfo)
        {
            if (gameRoomInfo.GameRoomType == GameRoomType.GAME_ROOM_OMAHA ||
                gameRoomInfo.GameRoomType == GameRoomType.GAME_ROOM_OMAHA_INSURANCE)
            {
                return GameType.NoLimitOmaha;
            }

            return GameType.NoLimitHoldem;
        }

        private TournamentDescriptor ParseTournamentDescriptor(GameRoomInfo gameRoomInfo)
        {
            if (!gameRoomInfo.IsTournament)
            {
                return null;
            }

            var tournamentDescriptor = new TournamentDescriptor
            {
                BuyIn = Buyin.FromBuyinRake(PMImporterHelper.ConvertSNGTypeToBuyIn(gameRoomInfo.SNGGameRoomBaseInfo.SNGRoomtype), 0, Currency.YUAN),
                Speed = gameRoomInfo.SNGGameRoomBaseInfo.SNGRoomtype == SNGRoomType.QUICK_SNG ? TournamentSpeed.Turbo : TournamentSpeed.Regular,
                StartDate = DateTimeHelper.UnixTimeToDateTime(gameRoomInfo.SNGGameRoomBaseInfo.StartTime / 1000),
                TournamentName = gameRoomInfo.SNGGameRoomBaseInfo.RoomName,
                TournamentId = gameRoomInfo.SNGGameRoomBaseInfo.GameRoomId.ToString(),
                TotalPlayers = (short)gameRoomInfo.SNGGameRoomBaseInfo.GameRoomUserMaxNums
            };

            return tournamentDescriptor;
        }

        private HandAction ParseHandAction(UserGameInfoNet userGameInfoNet, UserGameInfoNet previousUserGameInfo, HandHistory handHistory)
        {
            if (previousUserGameInfo == null)
            {
                return null;
            }
           
            // ante 
            if (previousUserGameInfo.RoomGameState == GameRoomGameState.ROOM_GAME_STATE_GameStart &&
                userGameInfoNet.RoomGameState == GameRoomGameState.ROOM_GAME_STATE_Ante)
            {
                return new HandAction(userGameInfoNet.UserInfo.ShowID,
                   HandActionType.ANTE,
                   userGameInfoNet.BetStacks,
                   Street.Preflop);
            }

            // Small blind
            if (IsPrePreflopAction(previousUserGameInfo.RoomGameState) &&
                userGameInfoNet.GameRole == UserGameRoles.USER_GAME_ROLE_SMALL_BLIND &&
                (userGameInfoNet.GameState == UserGameStates.USER_GAME_STATE_BETTING || userGameInfoNet.GameState == UserGameStates.USER_GAME_STATE_BLIND) &&
                userGameInfoNet.BetStacks > 0)
            {
                return new HandAction(userGameInfoNet.UserInfo.ShowID,
                    HandActionType.SMALL_BLIND,
                    userGameInfoNet.BetStacks,
                    Street.Preflop);
            }

            // Big blind
            if (IsPrePreflopAction(previousUserGameInfo.RoomGameState) &&
                userGameInfoNet.GameRole == UserGameRoles.USER_GAME_ROLE_BIG_BLIND &&
                (userGameInfoNet.GameState == UserGameStates.USER_GAME_STATE_BETTING || userGameInfoNet.GameState == UserGameStates.USER_GAME_STATE_BLIND) &&
                userGameInfoNet.BetStacks > 0)
            {
                return new HandAction(userGameInfoNet.UserInfo.ShowID,
                  HandActionType.BIG_BLIND,
                  userGameInfoNet.BetStacks,
                  Street.Preflop);
            }

            // Straddle
            if (IsPrePreflopAction(previousUserGameInfo.RoomGameState) &&
                userGameInfoNet.GameRole == UserGameRoles.USER_GAME_ROLE_STRADDLE &&
                (userGameInfoNet.GameState == UserGameStates.USER_GAME_STATE_BETTING || userGameInfoNet.GameState == UserGameStates.USER_GAME_STATE_BLIND) &&
                userGameInfoNet.BetStacks > 0)
            {
                return new HandAction(userGameInfoNet.UserInfo.ShowID,
                  HandActionType.STRADDLE,
                  userGameInfoNet.BetStacks,
                  Street.Preflop);
            }

            var handActionType = ParseHandActionType(userGameInfoNet);

            if (handActionType == HandActionType.UNKNOWN ||
                userGameInfoNet.GameState == previousUserGameInfo.GameState &&
                    previousUserGameInfo.BettingID == userGameInfoNet.BettingID)
            {
                return null;
            }

            var street = ParseRoomGameState(userGameInfoNet.RoomGameState);

            if (street == Street.Null)
            {
                if (userGameInfoNet.RoomGameState == GameRoomGameState.ROOM_GAME_STATE_Result)
                {
                    street = ParseRoomGameState(previousUserGameInfo.RoomGameState);
                }

                if (street == Street.Null)
                {
                    throw new HandBuilderException(handHistory.HandId, $"UserGameInfo.RoomGameState has unexpected value: {userGameInfoNet.RoomGameState}");
                }
            }

            var amount = userGameInfoNet.BetStacks - previousUserGameInfo.BetStacks;

            if (amount < 0)
            {
                if (handActionType != HandActionType.FOLD)
                {
                    throw new HandBuilderException(handHistory.HandId, $"Incorrect hand data: action = {handActionType}, amount = {amount}");
                }

                amount = 0;
            }

            var handAction = userGameInfoNet.RemainStacks == 0 ?
                new AllInAction(userGameInfoNet.UserInfo.ShowID, amount, street,
                    handActionType == HandActionType.BET || handActionType == HandActionType.RAISE,
                    handActionType) :
                new HandAction(userGameInfoNet.UserInfo.ShowID, handActionType, amount, street);

            return handAction;
        }

        private bool IsPrePreflopAction(GameRoomGameState gameRoomGameState)
        {
            return gameRoomGameState == GameRoomGameState.ROOM_GAME_STATE_GameStart ||
                gameRoomGameState == GameRoomGameState.ROOM_GAME_STATE_GameWait ||
                gameRoomGameState == GameRoomGameState.ROOM_GAME_STATE_Ante;
        }

        private HandActionType ParseHandActionType(UserGameInfoNet userGameInfoNet)
        {
            switch (userGameInfoNet.GameState)
            {
                case UserGameStates.USER_GAME_STATE_CALL:
                    return HandActionType.CALL;
                case UserGameStates.USER_GAME_STATE_CHECK:
                    return HandActionType.CHECK;
                case UserGameStates.USER_GAME_STATE_FOLD:
                    return HandActionType.FOLD;
                case UserGameStates.USER_GAME_STATE_RAISE:
                    return HandActionType.RAISE;
            }

            return HandActionType.UNKNOWN;
        }

        private Street ParseRoomGameState(GameRoomGameState roomGameState)
        {
            switch (roomGameState)
            {
                case GameRoomGameState.ROOM_GAME_STATE_Flop:
                case GameRoomGameState.ROOM_GAME_STATE_Flop_One:
                case GameRoomGameState.ROOM_GAME_STATE_Flop_Two:
                case GameRoomGameState.ROOM_GAME_STATE_Flop_Three:
                    return Street.Flop;
                case GameRoomGameState.ROOM_GAME_STATE_GameStart:
                    return Street.Init;
                case GameRoomGameState.ROOM_GAME_STATE_PreFlop:
                    return Street.Preflop;
                case GameRoomGameState.ROOM_GAME_STATE_River:
                    return Street.River;
                case GameRoomGameState.ROOM_GAME_STATE_Turn:
                    return Street.Turn;
                default:
                    return Street.Null;
            }
        }

        private static GameRoomInfo GetGameRoomInfo(SCGameRoomStateChange startRoomStateChange, HandHistory handHistory)
        {
            return startRoomStateChange.GameRoomInfo ?? throw new HandBuilderException(handHistory.HandId, "GameRoomInfo must be not empty.");
        }

        private static UserGameInfoNet[] GetUserGameInfos(GameRoomInfo gameRoomInfo, HandHistory handHistory)
        {
            return gameRoomInfo.UserGameInfos ?? throw new HandBuilderException(handHistory.HandId, "UserGameInfo has not been found.");
        }
    }
}