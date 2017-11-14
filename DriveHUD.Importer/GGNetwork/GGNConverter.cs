//-----------------------------------------------------------------------
// <copyright file="GGNConverter.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using DriveHUD.Importers.GGNetwork.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.GGNetwork
{
    internal class GGNConverter
    {
        /// <summary>
        /// </summary>
        /// <param name="handHistory"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Hand.HandHistory ConvertCashHandHistory(HandHistoryInformation handHistory)
        {
            var tableNumber = handHistory.HandHistory.NameInfo.Suffix < 10 ?
                $"0{handHistory.HandHistory.NameInfo.Suffix}" :
                handHistory.HandHistory.NameInfo.Suffix.ToString();

            var tableName = $"{handHistory.HandHistory.NameInfo.Id.Remove(0, 9)} {tableNumber}";

            var gameType = ConvertGameType((GameType)handHistory.HandHistory.GameType, (GameLimitType)handHistory.HandHistory.LimitType);

            var limit = ConvertLimit(ConvertAmount(handHistory.HandHistory.SmallBlind),
                ConvertAmount(handHistory.HandHistory.BigBlind),
                handHistory.HandHistory.Ante);

            var tableType = ConverterUtils.GetTableType();

            var seatType = ConverterUtils.GetSeatType((HandIdPlayType)handHistory.HandHistory.HandIdInfo.PlayType);

            var gameDescriptor = new HandHistories.Objects.GameDescription.GameDescriptor(EnumPokerSites.GGN, gameType, limit, tableType, seatType, null);

            var dealerPos = ConverterUtils.GetDealerPos(handHistory.HandHistory.Players);

            var handActions = new List<HandHistories.Objects.Actions.HandAction>();

            handActions.AddRange(ConvertInitStage(handHistory.HandHistory.Players));

            handActions.AddRange(ConvertActions(handHistory.HandHistory.Players, handHistory.HandHistory.HandInformation.Sequences, handHistory.HandHistory.Pots));

            var playerList = ConvertPlayers(handHistory.HandHistory.Players);

            HandHistories.Objects.Cards.BoardCards cards = null;

            if (handHistory.HandHistory.Summary.Board != null && handHistory.HandHistory.Summary.Board.Count != 0)
            {
                cards = ConvertCards(handHistory.HandHistory.Summary.Board[0]);
            }

            var winner = ConverterUtils.GetWinner(handHistory.HandHistory.Players, handHistory.HandHistory.Pots);

            return new HandHistories.Objects.Hand.HandHistory(gameDescriptor)
            {
                DateOfHandUtc = handHistory.HandHistory.StartTime,
                HandId = handHistory.HandHistory.HandIdInfo.SequenceId,
                DealerButtonPosition = dealerPos,
                TableName = tableName,
                NumPlayersSeated = handHistory.HandHistory.Players.Count,
                TotalPot = ConvertAmount(handHistory.HandHistory.Summary.TotalPot),
                Rake = ConvertAmount(handHistory.HandHistory.Summary.Rake),
                HandActions = handActions,
                Players = playerList,
                CommunityCards = cards,
                Hero = winner
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="handHistory"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Hand.HandHistory ConvertCashHandHistory(History handHistory)
        {
            var tableNumber = handHistory.NameInfo.Suffix < 10 ?
               $"0{handHistory.NameInfo.Suffix}" :
               handHistory.NameInfo.Suffix.ToString();

            var tableName = $"{handHistory.NameInfo.Id.Remove(0, 9)} {tableNumber}";

            var gameType = ConvertGameType((GameType)handHistory.GameType, (GameLimitType)handHistory.LimitType);

            var limit = ConvertLimit(ConvertAmount(handHistory.SmallBlind), ConvertAmount(handHistory.BigBlind),
                handHistory.Ante);

            var tableType = ConverterUtils.GetTableType();
            var seatType = ConverterUtils.GetSeatType((HandIdPlayType)handHistory.HandIdInfo.PlayType);

            var gameDescriptor = new HandHistories.Objects.GameDescription.GameDescriptor(EnumPokerSites.GGN, gameType, limit, tableType, seatType, null);

            var dealerPos = ConverterUtils.GetDealerPos(handHistory.Players);

            var handActions = new List<HandHistories.Objects.Actions.HandAction>();

            handActions.AddRange(ConvertInitStage(handHistory.Players));
            handActions.AddRange(ConvertActions(handHistory.Players, handHistory.HandInformation.Sequences, handHistory.Pots));

            var playerList = ConvertPlayers(handHistory.Players);

            HandHistories.Objects.Cards.BoardCards cards = null;

            if (handHistory.Summary.Board != null && handHistory.Summary.Board.Count != 0)
            {
                cards = ConvertCards(handHistory.Summary.Board[0]);
            }

            var winner = ConverterUtils.GetWinner(handHistory.Players, handHistory.Pots);

            return new HandHistories.Objects.Hand.HandHistory(gameDescriptor)
            {
                DateOfHandUtc = handHistory.StartTime,
                HandId = handHistory.HandIdInfo.SequenceId,
                DealerButtonPosition = dealerPos,
                TableName = tableName,
                NumPlayersSeated = handHistory.Players.Count,
                TotalPot = ConvertAmount(handHistory.Summary.TotalPot),
                Rake = ConvertAmount(handHistory.Summary.Rake),
                HandActions = handActions,
                Players = playerList,
                CommunityCards = cards,
                Hero = winner
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="handHistory"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Hand.HandHistory ConvertTournamentHandHistory(HandHistoryInformation handHistory)
        {
            var tableName = $"{handHistory.HandHistory.TourneyBrandName} {ConvertAmount(handHistory.HandHistory.TourneyBuyIn)}";

            var gameType = ConvertGameType((GameType)handHistory.HandHistory.GameType, (GameLimitType)handHistory.HandHistory.LimitType);

            var limit = ConvertLimit(ConvertAmount(handHistory.HandHistory.SmallBlind),
                ConvertAmount(handHistory.HandHistory.BigBlind),
                handHistory.HandHistory.Ante);

            var tableType = ConverterUtils.GetTableType();
            var seatType = ConverterUtils.GetSeatType((HandIdPlayType)handHistory.HandHistory.HandIdInfo.PlayType);

            var gameDescriptor = new HandHistories.Objects.GameDescription.GameDescriptor(EnumPokerSites.GGN, gameType, limit, tableType, seatType, null);

            var dealerPos = ConverterUtils.GetDealerPos(handHistory.HandHistory.Players);

            var handActions = new List<HandHistories.Objects.Actions.HandAction>();

            handActions.AddRange(ConvertInitStage(handHistory.HandHistory.Players));

            handActions.AddRange(ConvertActions(handHistory.HandHistory.Players,
                handHistory.HandHistory.HandInformation.Sequences, handHistory.HandHistory.Pots));

            var playerList = ConvertPlayers(handHistory.HandHistory.Players);

            HandHistories.Objects.Cards.BoardCards cards = null;

            if (handHistory.HandHistory.Summary.Board != null && handHistory.HandHistory.Summary.Board.Count != 0)
            {
                cards = ConvertCards(handHistory.HandHistory.Summary.Board[0]);
            }

            var winner = ConverterUtils.GetWinner(handHistory.HandHistory.Players, handHistory.HandHistory.Pots);

            return new HandHistories.Objects.Hand.HandHistory(gameDescriptor)
            {
                DateOfHandUtc = handHistory.HandHistory.StartTime,
                HandId = handHistory.HandHistory.HandIdInfo.SequenceId,
                DealerButtonPosition = dealerPos,
                TableName = tableName,
                NumPlayersSeated = handHistory.HandHistory.Players.Count,
                TotalPot = ConvertAmount(handHistory.HandHistory.Summary.TotalPot),
                Rake = ConvertAmount(handHistory.HandHistory.Summary.Rake),
                HandActions = handActions,
                Players = playerList,
                CommunityCards = cards,
                Hero = winner
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="handHistory"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Hand.HandHistory ConvertTournamentHandHistory(History handHistory)
        {
            var tableName = $"{handHistory.TourneyBrandName} {handHistory.NameInfo.Suffix}";

            // PokerFormat pokerFormat = GetHandHistoryFormat((Enums.HandIdPlayType)handHistory.CashGameHandHistory.HandIdInfo.PlayType);

            var gameType = ConvertGameType((GameType)handHistory.GameType, (GameLimitType)handHistory.LimitType);

            var limit = ConvertLimit(ConvertAmount(handHistory.SmallBlind), ConvertAmount(handHistory.BigBlind),
                handHistory.Ante);

            var tableType = ConverterUtils.GetTableType();
            var seatType = ConverterUtils.GetSeatType((HandIdPlayType)handHistory.HandIdInfo.PlayType);

            var gameDescriptor = new HandHistories.Objects.GameDescription.GameDescriptor(EnumPokerSites.GGN, gameType, limit, tableType, seatType, null);

            var dealerPos = ConverterUtils.GetDealerPos(handHistory.Players);

            var handActions = new List<HandHistories.Objects.Actions.HandAction>();

            handActions.AddRange(ConvertInitStage(handHistory.Players));
            handActions.AddRange(ConvertActions(handHistory.Players,
                handHistory.HandInformation.Sequences, handHistory.Pots));

            var playerList = ConvertPlayers(handHistory.Players);

            HandHistories.Objects.Cards.BoardCards cards = null;

            if (handHistory.Summary.Board != null && handHistory.Summary.Board.Count != 0)
            {
                cards = ConvertCards(handHistory.Summary.Board[0]);
            }

            var winner = ConverterUtils.GetWinner(handHistory.Players, handHistory.Pots);

            return new HandHistories.Objects.Hand.HandHistory(gameDescriptor)
            {
                DateOfHandUtc = handHistory.StartTime,
                HandId = handHistory.HandIdInfo.SequenceId,
                DealerButtonPosition = dealerPos,
                TableName = tableName,
                NumPlayersSeated = handHistory.Players.Count,
                TotalPot = ConvertAmount(handHistory.Summary.TotalPot),
                Rake = ConvertAmount(handHistory.Summary.Rake),
                HandActions = handActions,
                Players = playerList,
                CommunityCards = cards,
                Hero = winner
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="histories"></param>
        /// <returns></returns>
        public static List<HandHistories.Objects.Hand.HandHistory> ConvertTournamentHandHistories(HandHistoriesInformation histories)
        {
            return histories.Histories.Select(ConvertTournamentHandHistory).ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="handHistories"></param>
        /// <returns></returns>
        public static List<HandHistories.Objects.Hand.HandHistory> ConvertCashHandHistories(HandHistoriesInformation handHistories)
        {
            return handHistories.Histories.Select(ConvertCashHandHistory).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tournaments"></param>
        /// <returns></returns>
        public static List<HandHistories.Objects.GameDescription.TournamentDescriptor> ConvertTournamentDescriptors(IList<TournamentInformation> tournaments)
        {
            return tournaments.Select(ConvertTournamentDescriptor).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tournament"></param>
        /// <returns></returns>
        public static HandHistories.Objects.GameDescription.TournamentDescriptor ConvertTournamentDescriptor(TournamentInformation tournament)
        {
            return new HandHistories.Objects.GameDescription.TournamentDescriptor
            {
                TournamentId = tournament.Id,
                TournamentInGameId = tournament.Id,
                TournamentName = tournament.Name,
                BuyIn = HandHistories.Objects.GameDescription.Buyin.FromBuyinRake(0, ConvertAmount(tournament.PrizeSummary.TotalPrizeAmount),
                    HandHistories.Objects.GameDescription.Currency.USD),
                Bounty = ConvertAmount(tournament.BountyInformation.BountyAmount),
                Rebuy = ConvertAmount(tournament.RebuyAmount),
                Addon = ConvertAmount(tournament.AddOnAmount),
                Winning = ConvertAmount(tournament.PrizeSummary.TotalPrizeAmount),
                TotalPlayers = (short)tournament.RegisteredPlayers,
                StartDate = tournament.LifeCycleData.StartTime,
                Speed = HandHistories.Objects.GameDescription.TournamentSpeed.Regular
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="smallBlind"></param>
        /// <param name="bigBlind"></param>
        /// <param name="ante"></param>
        /// <returns></returns>
        private static HandHistories.Objects.GameDescription.Limit ConvertLimit(decimal smallBlind, decimal bigBlind, int ante)
        {
            return HandHistories.Objects.GameDescription.Limit.FromSmallBlindBigBlind(smallBlind, bigBlind,
                HandHistories.Objects.GameDescription.Currency.USD, true, ante);
        }

        /// <summary>
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="gameLimitType"></param>
        /// <returns></returns>
        public static HandHistories.Objects.GameDescription.GameType ConvertGameType(GameType gameType, GameLimitType gameLimitType)
        {
            try
            {
                switch (gameType)
                {
                    case GameType.Holdem:
                        switch (gameLimitType)
                        {
                            case GameLimitType.Unknown:
                                break;
                            case GameLimitType.Limit:
                                return HandHistories.Objects.GameDescription.GameType.FixedLimitHoldem;
                            case GameLimitType.NoLimit:
                                return HandHistories.Objects.GameDescription.GameType.NoLimitHoldem;
                            case GameLimitType.PotLimit:
                                return HandHistories.Objects.GameDescription.GameType.PotLimitHoldem;
                            case GameLimitType.Half:
                            case GameLimitType.Quarter:
                                break;
                        }

                        break;
                    case GameType.Omaha:
                        switch (gameLimitType)
                        {
                            case GameLimitType.Unknown:
                                break;
                            case GameLimitType.Limit:
                                return HandHistories.Objects.GameDescription.GameType.FixedLimitOmaha;
                            case GameLimitType.NoLimit:
                                return HandHistories.Objects.GameDescription.GameType.NoLimitOmaha;
                            case GameLimitType.PotLimit:
                                return HandHistories.Objects.GameDescription.GameType.PotLimitOmaha;
                            case GameLimitType.Half:
                                break;
                            case GameLimitType.Quarter:
                                break;
                        }
                        break;
                }

                return HandHistories.Objects.GameDescription.GameType.Unknown;
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Could not convert game type."), e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Actions.HandActionType ConvertActionType(ActionType type)
        {
            switch (type)
            {
                case ActionType.Check:
                    {
                        return HandHistories.Objects.Actions.HandActionType.CHECK;
                    }
                case ActionType.Call:
                    {
                        return HandHistories.Objects.Actions.HandActionType.CALL;
                    }
                case ActionType.Bet:
                    {
                        return HandHistories.Objects.Actions.HandActionType.BET;
                    }
                case ActionType.Raise:
                    {
                        return HandHistories.Objects.Actions.HandActionType.RAISE;
                    }
                case ActionType.Fold:
                    {
                        return HandHistories.Objects.Actions.HandActionType.FOLD;
                    }
            }

            return HandHistories.Objects.Actions.HandActionType.UNKNOWN;
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Cards.Street ConvertState(RoundType type)
        {
            try
            {
                switch (type)
                {
                    case RoundType.PreFlop:
                        return HandHistories.Objects.Cards.Street.Preflop;
                    case RoundType.Flop:
                        return HandHistories.Objects.Cards.Street.Flop;
                    case RoundType.Turn:
                        return HandHistories.Objects.Cards.Street.Turn;
                    case RoundType.River:
                        return HandHistories.Objects.Cards.Street.River;
                    case RoundType.ShowDown:
                        return HandHistories.Objects.Cards.Street.Showdown;
                    case RoundType.End:
                        return HandHistories.Objects.Cards.Street.Summary;
                }

                return HandHistories.Objects.Cards.Street.Null;
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Could not convert state."), e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="sidePot"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Actions.HandAction ConvertHandAction(string nickName, Model.Action action, int state, bool sidePot = false)
        {
            try
            {
                var actionType = HandHistories.Objects.Actions.HandActionType.UNKNOWN;

                switch ((TableActionType)action.TableActionType)
                {
                    case TableActionType.Unknown:
                        switch ((AfterAction)action.AfterAction)
                        {
                            case AfterAction.Unknown:
                                actionType = HandHistories.Objects.Actions.HandActionType.UNKNOWN;
                                break;
                            case AfterAction.UncalledBet:
                                actionType = HandHistories.Objects.Actions.HandActionType.UNCALLED_BET;
                                break;
                            case AfterAction.Show:
                                actionType = HandHistories.Objects.Actions.HandActionType.SHOW;
                                break;
                            case AfterAction.Collect:
                                actionType = sidePot ?
                                    HandHistories.Objects.Actions.HandActionType.WINS_SIDE_POT :
                                    HandHistories.Objects.Actions.HandActionType.WINS;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case TableActionType.Turn:
                        {
                            actionType = action.IsAllIn
                                ? HandHistories.Objects.Actions.HandActionType.ALL_IN
                                : ConvertActionType((ActionType)action.TurnActionType);
                            break;
                        }
                    default:
                        break;
                }

                var street = ConvertState((RoundType)state);

                if (actionType == HandHistories.Objects.Actions.HandActionType.UNKNOWN)
                {
                    return null;
                }

                return new HandHistories.Objects.Actions.HandAction(nickName, actionType, ConvertAmount(action.ActionAmount),
                    street, action.IsAllIn);
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Could not convert hand actions."), e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="players"></param>
        /// <param name="sequences"></param>
        /// <param name="pots"></param>
        /// <returns></returns>
        public static List<HandHistories.Objects.Actions.HandAction> ConvertActions(IList<Player> players, IList<Sequence> sequences, IList<Pot> pots)
        {
            try
            {
                var actions = new List<HandHistories.Objects.Actions.HandAction>();

                foreach (var sequence in sequences)
                {
                    foreach (var action in sequence.Actions)
                    {
                        var player = players[action.PlayerIndex];

                        var winsSidePot = PlayerUtils.IsWinsSidePot(player, pots);

                        var handAction = ConvertHandAction(player?.NickName, action, sequence.State, winsSidePot);

                        if (handAction == null)
                        {
                            continue;
                        }

                        actions.Add(handAction);
                    }
                }

                return actions;
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Could not convert actions."), e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Cards.Card ConvertCard(CardInfo card)
        {
            var rank = card.Rank == "10" ? 'T' : card.Rank.ToCharArray()[0];
            var suit = ConverterUtils.GetCardSuitAsChar(card.Suit);

            return new HandHistories.Objects.Cards.Card(rank, suit);
        }

        /// <summary>
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Cards.BoardCards ConvertCards(Board board)
        {
            try
            {
                if (board?.Cards == null || board.Cards.Count == 0)
                {
                    return null;
                }

                var cards = new List<HandHistories.Objects.Cards.Card>();

                foreach (var card in board.Cards)
                {
                    if (card == null)
                    {
                        continue;
                    }

                    var cardInfo = ConverterUtils.GetCardInfoByOrdinal(card.OrdinalForSerializationOnly);

                    if (cardInfo == null)
                    {
                        continue;
                    }

                    cards.Add(ConvertCard(cardInfo));
                }

                return HandHistories.Objects.Cards.BoardCards.FromCards(cards.ToArray());
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Could not convert cards from board."), e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="holeCards"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Cards.HoleCards ConvertCards(IList<HoleCard> holeCards)
        {
            try
            {
                if (holeCards == null)
                    return null;

                var cards = new List<HandHistories.Objects.Cards.Card>();

                foreach (var card in holeCards)
                {
                    var cardInfo = ConverterUtils.GetCardInfoByOrdinal(card.OrdinalForSerializationOnly);

                    if (cardInfo == null) continue;

                    cards.Add(ConvertCard(cardInfo));
                }

                switch (cards.Count)
                {
                    case 2:
                        return HandHistories.Objects.Cards.HoleCards.ForHoldem(cards[0], cards[1]);
                    case 4:
                        return HandHistories.Objects.Cards.HoleCards.ForOmaha(cards[0], cards[1], cards[2], cards[3]);
                }
                return null;
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Could not convert cards from hole cards."), e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Players.Player ConvertPlayer(Player player)
        {
            var startingStack = ConvertAmount(player.InitialBalance);
            var seatNumber = player.SeatIndex;

            var winAmount = 0m;

            if (player.IsWinner)
            {
                winAmount = ConvertAmount(player.TotalEarnedAmount);
            }

            return new HandHistories.Objects.Players.Player
            {
                PlayerName = player.NickName,
                StartingStack = startingStack,
                SeatNumber = ++seatNumber,
                IsSittingOut = player.IsSittingOut,
                Win = winAmount,
                HoleCards = ConvertCards(player.HoleCards)
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Players.PlayerList ConvertPlayers(IList<Player> players)
        {
            return new HandHistories.Objects.Players.PlayerList(players.Select(ConvertPlayer).ToList());
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ConvertAmount(int value)
        {
            return Convert.ToDecimal(value) / 1000;
        }

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static HandHistories.Objects.Actions.HandAction ConvertInit2Action(Player player)
        {
            try
            {
                if (player.PostedSmallBlind > 0 && player.PostedBigBlind == 0)
                {
                    return new HandHistories.Objects.Actions.HandAction(player.NickName,
                        HandHistories.Objects.Actions.HandActionType.SMALL_BLIND, ConvertAmount(player.PostedSmallBlind),
                        HandHistories.Objects.Cards.Street.Preflop);
                }

                if (player.PostedBigBlind > 0 && player.PostedSmallBlind == 0)
                {
                    return new HandHistories.Objects.Actions.HandAction(player.NickName,
                        HandHistories.Objects.Actions.HandActionType.BIG_BLIND, ConvertAmount(player.PostedBigBlind),
                        HandHistories.Objects.Cards.Street.Preflop);
                }

                if (player.PostedSmallBlind > 0 && player.PostedBigBlind > 0)
                {
                    return new HandHistories.Objects.Actions.HandAction(player.NickName,
                        HandHistories.Objects.Actions.HandActionType.POSTS, ConvertAmount(player.PostedSmallBlind + player.PostedBigBlind),
                        HandHistories.Objects.Cards.Street.Preflop);
                }

                if (player.PostedAnte > 0)
                {
                    return new HandHistories.Objects.Actions.HandAction(player.NickName,
                        HandHistories.Objects.Actions.HandActionType.ANTE, ConvertAmount(player.PostedAnte),
                        HandHistories.Objects.Cards.Street.Preflop);
                }

                if (player.PostedStraddle > 0)
                {
                    return new HandHistories.Objects.Actions.HandAction(player.NickName,
                        HandHistories.Objects.Actions.HandActionType.RAISE, ConvertAmount(player.PostedStraddle),
                        HandHistories.Objects.Cards.Street.Preflop);
                }

                return null;
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Could not convert initial actions."), e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public static List<HandHistories.Objects.Actions.HandAction> ConvertInitStage(IList<Player> players)
        {
            try
            {
                var initStageActions = new List<HandHistories.Objects.Actions.HandAction>();

                foreach (var player in players)
                {
                    if (player == null)
                    {
                        continue;
                    }

                    var initStageAction = ConvertInit2Action(player);

                    if (initStageAction == null)
                    {
                        continue;
                    }

                    initStageActions.Add(initStageAction);
                }

                return initStageActions;
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Could not convert initial stage."), e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="playType"></param>
        /// <returns></returns>
        public static HandHistories.Objects.GameDescription.SeatType.SeatTypeEnum ConvertSeatType(HandIdPlayType playType)
        {
            try
            {
                switch (playType)
                {
                    case HandIdPlayType.Unknown:
                        return HandHistories.Objects.GameDescription.SeatType.SeatTypeEnum.Unknown;
                    case HandIdPlayType.Cash:
                        return HandHistories.Objects.GameDescription.SeatType.SeatTypeEnum._6Max;
                    case HandIdPlayType.SitAndGo:
                    case HandIdPlayType.Tournament:
                        return HandHistories.Objects.GameDescription.SeatType.SeatTypeEnum._FullRing_9Handed;
                    case HandIdPlayType.MegaSpin:
                    case HandIdPlayType.FortuneSpin:
                        break;
                }
                return HandHistories.Objects.GameDescription.SeatType.SeatTypeEnum.Unknown;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="playType"></param>
        /// <returns></returns>
        public static HandHistories.Objects.GameDescription.PokerFormat GetHandHistoryFormat(HandIdPlayType playType)
        {
            try
            {
                switch (playType)
                {
                    case HandIdPlayType.Cash:
                        return HandHistories.Objects.GameDescription.PokerFormat.CashGame;
                    case HandIdPlayType.SitAndGo:
                    case HandIdPlayType.Tournament:
                        return HandHistories.Objects.GameDescription.PokerFormat.Tournament;
                    case HandIdPlayType.MegaSpin:
                        break;
                    case HandIdPlayType.FortuneSpin:
                        break;
                }

                return HandHistories.Objects.GameDescription.PokerFormat.Unknown;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}