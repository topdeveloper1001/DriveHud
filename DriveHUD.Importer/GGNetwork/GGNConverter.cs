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
using HandHistories.Parser.Utils.FastParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using HandHistories.Objects.Hand;

namespace DriveHUD.Importers.GGNetwork
{
    /// <summary>
    /// Defines methods to convert GGN hand histories into DH hand histories
    /// </summary>
    internal class GGNConverter
    {
        public static HandHistories.Objects.Hand.HandHistory ConvertHandHistory(GGNHandHistory ggnHandHistory, IGGNCacheService cacheService)
        {
            HandHistories.Objects.GameDescription.TournamentDescriptor tournamentDescriptor = null;

            if (ggnHandHistory.IsTournament)
            {
                var tournamentInformation = cacheService.GetTournament(ggnHandHistory.TourneyId);

                if (tournamentInformation == null)
                {
                    cacheService.RefreshAsync().Wait();

                    tournamentInformation = cacheService.GetTournament(ggnHandHistory.TourneyId);

                    if (tournamentInformation == null)
                    {
                        // create descriptor using only existing data (something might be missed, but hand history will be built)
                        tournamentDescriptor = new HandHistories.Objects.GameDescription.TournamentDescriptor
                        {
                            TournamentId = ggnHandHistory.TourneyId,
                            TournamentInGameId = ggnHandHistory.TourneyId,
                            TournamentName = GGNUtils.PurgeTournamentName(ggnHandHistory.TourneyBrandName),
                            BuyIn = HandHistories.Objects.GameDescription.Buyin.FromBuyinRake(ConvertAmount(ggnHandHistory.TourneyBuyIn), 0, HandHistories.Objects.GameDescription.Currency.USD),
                            StartDate = ggnHandHistory.StartTime,
                            Speed = ParserUtils.ParseTournamentSpeed(ggnHandHistory.TourneyBrandName)
                        };
                    }
                    else
                    {
                        tournamentDescriptor = ConvertTournamentDescriptor(tournamentInformation);
                    }
                }
                else
                {
                    tournamentDescriptor = ConvertTournamentDescriptor(tournamentInformation);
                }
            }

            var pokerFormat = ggnHandHistory.IsTournament ?
                HandHistories.Objects.GameDescription.PokerFormat.Tournament :
                HandHistories.Objects.GameDescription.PokerFormat.CashGame;

            var gameType = ConvertGameType((GameType)ggnHandHistory.GameType, (GameLimitType)ggnHandHistory.LimitType);

            // small & bing blinds for cash are stored in 0.1 cents, but for tourney are stored in chips
            var smallBlind = ggnHandHistory.IsTournament ? ggnHandHistory.SmallBlind : ConvertAmount(ggnHandHistory.SmallBlind);
            var bigBlind = ggnHandHistory.IsTournament ? ggnHandHistory.BigBlind : ConvertAmount(ggnHandHistory.BigBlind);

            var limit = ConvertLimit(smallBlind, bigBlind, ggnHandHistory.Ante);

            var tableType = ConverterUtils.GetTableType();

            var seatType = HandHistories.Objects.GameDescription.SeatType.FromMaxPlayers(ggnHandHistory.MaxPlayer);

            var gameDescriptor = new HandHistories.Objects.GameDescription.GameDescriptor(pokerFormat, EnumPokerSites.GGN, gameType, limit,
                tableType, seatType, tournamentDescriptor);

            var handHistory = new HandHistories.Objects.Hand.HandHistory(gameDescriptor)
            {
                DateOfHandUtc = ggnHandHistory.StartTime,
                HandId = ggnHandHistory.HandIdInfo.SequenceId,
                DealerButtonPosition = ConverterUtils.GetDealerPosition(ggnHandHistory.Players),
                NumPlayersSeated = ggnHandHistory.Players.Count,
                TotalPot = ggnHandHistory.IsTournament ? ggnHandHistory.Summary.TotalPot : ConvertAmount(ggnHandHistory.Summary.TotalPot),
                Rake = ConvertAmount(ggnHandHistory.Summary.Rake)
            };

            // convert table name
            if (ggnHandHistory.IsTournament)
            {
                handHistory.TableName = $"{handHistory.GameDescription.Tournament.TournamentName} - Table {ggnHandHistory.NameInfo.Suffix}";
            }
            else
            {
                var tableNumber = ggnHandHistory.NameInfo.Suffix < 10 ?
                    $"0{ggnHandHistory.NameInfo.Suffix}" :
                    ggnHandHistory.NameInfo.Suffix.ToString();

                handHistory.TableName = $"{ggnHandHistory.NameInfo.Id.Remove(0, 9)} {tableNumber}";
            }

            // convert actions
            var handActions = new List<HandHistories.Objects.Actions.HandAction>();
            handActions.AddRange(ConvertInitStage(ggnHandHistory.Players, ggnHandHistory.IsTournament));
            handActions.AddRange(ConvertActions(ggnHandHistory.Players, ggnHandHistory.HandInformation.Sequences, ggnHandHistory.Pots, ggnHandHistory.IsTournament));
            handHistory.HandActions = handActions;

            // convert players
            handHistory.Players = ConvertPlayers(ggnHandHistory.Players, ggnHandHistory.IsTournament);

            // convert community cards
            if (ggnHandHistory.Summary.Board != null && ggnHandHistory.Summary.Board.Count != 0)
            {
                handHistory.CommunityCards = ConvertCards(ggnHandHistory.Summary.Board[0]);
            }

            // calculate and update some data which aren't presented in original format
            AdjustHandHistory(handHistory);

            return handHistory;
        }

        private static void AdjustHandHistory(HandHistory handHistory)
        {
            // adjust player bets
            foreach (var player in handHistory.Players)
            {
                var playerActions = handHistory.HandActions.Where(x => x.PlayerName.Equals(player.PlayerName));

                if (playerActions != null && playerActions.Any())
                {
                    player.Bet = Math.Abs(playerActions.Where(x => x.Amount < 0).Sum(x => x.Amount));
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="handHistories"></param>
        /// <returns></returns>
        public static List<HandHistories.Objects.Hand.HandHistory> ConvertHandHistories(HandHistoriesInformation handHistories, IGGNCacheService cacheService)
        {
            return handHistories.Histories.Select(x => ConvertHandHistory(x, cacheService)).ToList();
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
                TournamentName = GGNUtils.PurgeTournamentName(tournament.Name),
                BuyIn = HandHistories.Objects.GameDescription.Buyin.FromBuyinRake(ConvertAmount(tournament.BuyIn + tournament.BountyInformation.BountyAmount),
                    ConvertAmount(tournament.EntranceFee),
                    HandHistories.Objects.GameDescription.Currency.USD),
                Bounty = ConvertAmount(tournament.BountyInformation.BountyAmount),
                Rebuy = ConvertAmount(tournament.RebuyAmount),
                Addon = ConvertAmount(tournament.AddOnAmount),
                TotalPlayers = (short)tournament.RegisteredPlayers,
                StartDate = tournament.LifeCycleData.StartTime,
                Speed = ParserUtils.ParseTournamentSpeed(tournament.Name)
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
        public static HandHistories.Objects.Actions.HandAction ConvertHandAction(string nickName, Model.Action action, int state, bool sidePot, bool isTournament)
        {
            try
            {
                if (action.TableActionType != (int)TableActionType.Turn)
                {
                    return null;
                }

                var street = ConvertState((RoundType)state);

                var actionType = HandHistories.Objects.Actions.HandActionType.UNKNOWN;

                // normal action
                if (action.TurnActionType != (int)ActionType.Unknown)
                {
                    actionType = ConvertActionType((ActionType)action.TurnActionType);

                    if (action.IsAllIn)
                    {
                        return new HandHistories.Objects.Actions.AllInAction(nickName,
                            isTournament ? action.ActionAmount : ConvertAmount(action.ActionAmount),
                            street,
                            actionType == HandHistories.Objects.Actions.HandActionType.RAISE,
                            actionType);
                    }
                }
                else
                {
                    switch ((AfterAction)action.AfterAction)
                    {
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
                            actionType = HandHistories.Objects.Actions.HandActionType.UNKNOWN;
                            break;
                    }
                }

                if (actionType == HandHistories.Objects.Actions.HandActionType.UNKNOWN)
                {
                    return null;
                }

                return new HandHistories.Objects.Actions.HandAction(nickName, actionType,
                    isTournament ? action.ActionAmount : ConvertAmount(action.ActionAmount),
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
        public static List<HandHistories.Objects.Actions.HandAction> ConvertActions(IList<Player> players, IList<Sequence> sequences, IList<Pot> pots, bool isTournament)
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

                        var handAction = ConvertHandAction(player?.NickName, action, sequence.State, winsSidePot, isTournament);

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
        public static HandHistories.Objects.Players.Player ConvertPlayer(Player player, bool isTournament)
        {
            var startingStack = isTournament ? player.InitialBalance : ConvertAmount(player.InitialBalance);
            var seatNumber = player.SeatIndex;

            var winAmount = 0m;

            if (player.IsWinner)
            {
                winAmount = isTournament ?
                    (player.TotalEarnedAmount + player.ContributedPot) :
                    ConvertAmount(player.TotalEarnedAmount + player.ContributedPot);
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
        public static HandHistories.Objects.Players.PlayerList ConvertPlayers(IList<Player> players, bool isTournament)
        {
            return new HandHistories.Objects.Players.PlayerList(players.Select(x => ConvertPlayer(x, isTournament)).ToList());
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
        /// <param name="players"></param>
        /// <returns></returns>
        public static List<HandHistories.Objects.Actions.HandAction> ConvertInitStage(IList<Player> players, bool isTournament)
        {
            try
            {
                var anteActions = new List<HandHistories.Objects.Actions.HandAction>();
                var blindActions = new List<HandHistories.Objects.Actions.HandAction>();
                var straddleActions = new List<HandHistories.Objects.Actions.HandAction>();

                foreach (var player in players)
                {
                    if (player == null)
                    {
                        continue;
                    }

                    if (player.PostedSmallBlind > 0 && player.PostedBigBlind == 0)
                    {
                        blindActions.Insert(0, new HandHistories.Objects.Actions.HandAction(player.NickName,
                            HandHistories.Objects.Actions.HandActionType.SMALL_BLIND,
                            isTournament ? player.PostedSmallBlind : ConvertAmount(player.PostedSmallBlind),
                            HandHistories.Objects.Cards.Street.Preflop));
                    }

                    if (player.PostedBigBlind > 0 && player.PostedSmallBlind == 0)
                    {
                        blindActions.Add(new HandHistories.Objects.Actions.HandAction(player.NickName,
                            HandHistories.Objects.Actions.HandActionType.BIG_BLIND,
                            isTournament ? player.PostedBigBlind : ConvertAmount(player.PostedBigBlind),
                            HandHistories.Objects.Cards.Street.Preflop));
                    }

                    if (player.PostedSmallBlind > 0 && player.PostedBigBlind > 0)
                    {
                        blindActions.Add(new HandHistories.Objects.Actions.HandAction(player.NickName,
                            HandHistories.Objects.Actions.HandActionType.POSTS,
                            isTournament ? (player.PostedSmallBlind + player.PostedBigBlind) : ConvertAmount(player.PostedSmallBlind + player.PostedBigBlind),
                            HandHistories.Objects.Cards.Street.Preflop));
                    }

                    if (player.PostedAnte > 0)
                    {
                        anteActions.Add(new HandHistories.Objects.Actions.HandAction(player.NickName,
                            HandHistories.Objects.Actions.HandActionType.ANTE,
                            isTournament ? player.PostedAnte : ConvertAmount(player.PostedAnte),
                            HandHistories.Objects.Cards.Street.Preflop));
                    }

                    if (player.PostedStraddle > 0)
                    {
                        straddleActions.Add(new HandHistories.Objects.Actions.HandAction(player.NickName,
                            HandHistories.Objects.Actions.HandActionType.RAISE,
                            isTournament ? player.PostedStraddle : ConvertAmount(player.PostedStraddle),
                            HandHistories.Objects.Cards.Street.Preflop));
                    }
                }

                return anteActions.Concat(blindActions).Concat(straddleActions).ToList();
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Could not convert initial stage."), e);
            }
        }
    }
}