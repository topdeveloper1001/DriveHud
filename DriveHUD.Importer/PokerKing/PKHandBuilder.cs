//-----------------------------------------------------------------------
// <copyright file="PKHandBuilder.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using DriveHUD.Importers.PokerKing.Model;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Utils;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Importers.PokerKing
{
    internal class PKHandBuilder : IPKHandBuilder
    {
        private Dictionary<int, NoticeGameSnapShot> roomsData = new Dictionary<int, NoticeGameSnapShot>();

        private Dictionary<long, Dictionary<int, List<PokerKingPackage>>> userPackages = new Dictionary<long, Dictionary<int, List<PokerKingPackage>>>();

        public bool TryBuild(PokerKingPackage package, int identifier, out HandHistory handHistory)
        {
            handHistory = null;

            if (package == null || identifier == 0)
            {
                return false;
            }

            if (package.PackageType == PackageType.NoticeGameSnapShot)
            {
                ParsePackage<NoticeGameSnapShot>(package, x => ProcessNoticeGameSnapShot(x));
                return false;
            }

            if (!userPackages.TryGetValue(identifier, out Dictionary<int, List<PokerKingPackage>> roomPackages))
            {
                roomPackages = new Dictionary<int, List<PokerKingPackage>>();
                userPackages.Add(identifier, roomPackages);
            }

            if (!roomPackages.TryGetValue(package.RoomId, out List<PokerKingPackage> packages))
            {
                packages = new List<PokerKingPackage>();
                roomPackages.Add(package.RoomId, packages);
            }

            packages.Add(package);

            if (package.PackageType == PackageType.NoticeGameSettlement)
            {
                handHistory = BuildHand(packages, identifier, package.UserId);
            }

            return handHistory != null;
        }

        public bool IsRoomSnapShotAvailable(PokerKingPackage package)
        {
            if (package == null || package.RoomId == 0)
            {
                return false;
            }

            return roomsData.ContainsKey(package.RoomId);
        }

        public void CleanRoom(int identifier, int roomId)
        {
            if (userPackages.TryGetValue(identifier, out Dictionary<int, List<PokerKingPackage>> roomPackages) &&
                roomPackages.TryGetValue(roomId, out List<PokerKingPackage> packages))
            {
                packages.Clear();
            }
        }

        private bool ValidatePackages(List<PokerKingPackage> packages, uint userId)
        {
            var noticeGameSettlementPackage = packages.FirstOrDefault(x => x.PackageType == PackageType.NoticeGameSettlement);

            if (noticeGameSettlementPackage == null)
            {
                LogProvider.Log.Warn(CustomModulesNames.PKCatcher, $"Hand cannot be built because settlement data is missing. [{userId}]");
                return false;
            }

            var isValid = false;

            ParsePackage<NoticeGameSettlement>(noticeGameSettlementPackage, x =>
            {
                var noticeResetGameExists = packages.Any(p => p.PackageType == PackageType.NoticeResetGame);
                var noticeGameSnapshotExists = roomsData.ContainsKey(x.RoomId);

                isValid = noticeResetGameExists && noticeGameSnapshotExists;

                if (!isValid)
                {
                    if (!noticeGameSnapshotExists)
                    {
                        LogProvider.Log.Warn(CustomModulesNames.PKCatcher, $"Hand #{x.Gameuuid} of room #{x.RoomId} cannot be built because room snapshot is missing. [{userId}]");
                        return;
                    }

                    LogProvider.Log.Warn(CustomModulesNames.PKCatcher, $"Hand #{x.Gameuuid} of room #{x.RoomId} cannot be built because some data is missing. [{userId}]");
                }
            });

            return isValid;
        }

        private HandHistory BuildHand(List<PokerKingPackage> packages, int identifier, uint userId)
        {
            HandHistory handHistory = null;

            try
            {
                if (!ValidatePackages(packages, userId))
                {
                    packages.Clear();
                    return handHistory;
                }

                var isGameStarted = false;

                foreach (var package in packages)
                {
                    if (package.PackageType == PackageType.NoticeResetGame)
                    {
                        handHistory = new HandHistory
                        {
                            DateOfHandUtc = package.Timestamp.ToUniversalTime()
                        };

                        ParsePackage<NoticeResetGame>(package, x => ProcessNoticeResetGame(x, handHistory));
                        isGameStarted = true;
                        continue;
                    }

                    if (!isGameStarted)
                    {
                        continue;
                    }

                    switch (package.PackageType)
                    {
                        case PackageType.NoticeGameElectDealer:
                            ParsePackage<NoticeGameElectDealer>(package, x => ProcessNoticeGameElectDealer(x, handHistory));
                            break;
                        case PackageType.NoticeGameBlind:
                            ParsePackage<NoticeGameBlind>(package, x => ProcessNoticeGameBlind(x, handHistory));
                            break;
                        case PackageType.NoticeGameAnte:
                            ParsePackage<NoticeGameAnte>(package, x => ProcessNoticeGameAnte(x, handHistory));
                            break;
                        case PackageType.NoticeGameHoleCard:
                            ParsePackage<NoticeGameHoleCard>(package, x => ProcessNoticeGameHoleCard(x, handHistory, userId));
                            break;
                        case PackageType.NoticePlayerAction:
                            ParsePackage<NoticePlayerAction>(package, x => ProcessNoticePlayerAction(x, handHistory));
                            break;
                        case PackageType.NoticeGameCommunityCards:
                            ParsePackage<NoticeGameCommunityCards>(package, x => ProcessNoticeGameCommunityCards(x, handHistory));
                            break;
                        case PackageType.NoticeGameShowDown:
                            ParsePackage<NoticeGameShowDown>(package, x => ProcessNoticeGameShowDown(x, handHistory));
                            break;
                        case PackageType.NoticeGameSettlement:
                            ParsePackage<NoticeGameSettlement>(package, x => ProcessNoticeGameSettlement(x, handHistory));
                            break;
                    }
                }

                AdjustHandHistory(handHistory);

                return handHistory;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PKCatcher, $"Failed to build hand #{handHistory?.HandId ?? 0} room #{handHistory?.GameDescription.Identifier ?? 0} [{userId}] [{identifier}]", e);
                return null;
            }
            finally
            {
                packages.Clear();
            }
        }

        private void AdjustHandHistory(HandHistory handHistory)
        {
            if (handHistory == null)
            {
                return;
            }

            HandHistoryUtils.UpdateAllInActions(handHistory);
            HandHistoryUtils.CalculateBets(handHistory);
            HandHistoryUtils.CalculateUncalledBets(handHistory, true);
            HandHistoryUtils.CalculateTotalPot(handHistory);
            HandHistoryUtils.RemoveSittingOutPlayers(handHistory);

            if (!handHistory.GameDescription.IsTournament)
            {
                const decimal divider = 100m;

                handHistory.HandActions.ForEach(a => a.Amount = a.Amount / divider);

                handHistory.GameDescription.Limit.SmallBlind /= divider;
                handHistory.GameDescription.Limit.BigBlind /= divider;
                handHistory.GameDescription.Limit.Ante /= divider;

                handHistory.Players.ForEach(p =>
                {
                    p.Bet /= divider;
                    p.StartingStack /= divider;
                    p.Win /= divider;
                });

                handHistory.TotalPot /= divider;
            }
        }

        private void ParsePackage<T>(PokerKingPackage package, Action<T> action)
        {
            if (!SerializationHelper.TryDeserialize(package.Body, out T packageBody))
            {
                throw new DHInternalException(new NonLocalizableString($"Failed to deserialize package of {package.PackageType} type."));
            }

            action?.Invoke(packageBody);
        }

        private void ProcessNoticeGameSnapShot(NoticeGameSnapShot noticeGameSnapShot)
        {
            if (!roomsData.ContainsKey(noticeGameSnapShot.RoomId))
            {
                roomsData.Add(noticeGameSnapShot.RoomId, noticeGameSnapShot);
                LogProvider.Log.Info(CustomModulesNames.PKCatcher, $"Snapshot of room {noticeGameSnapShot.RoomId} has been stored.");
                return;
            }

            roomsData[noticeGameSnapShot.RoomId] = noticeGameSnapShot;

            LogProvider.Log.Info(CustomModulesNames.PKCatcher, $"Snapshot of room {noticeGameSnapShot.RoomId} has been updated.");
        }

        private void ProcessNoticeResetGame(NoticeResetGame noticeResetGame, HandHistory handHistory)
        {
            if (!long.TryParse(noticeResetGame.GameId, out long handId))
            {
                throw new DHInternalException(new NonLocalizableString($"Failed to parse hand id from '{noticeResetGame.GameId}'."));
            }

            var players = noticeResetGame.Players ?? throw new HandBuilderException(handId, "NoticeResetGame.Players must be not null.");

            handHistory.HandId = handId;

            foreach (var playerInfo in players)
            {
                var player = new Player
                {
                    PlayerName = playerInfo.Playerid.ToString(),
                    PlayerNick = playerInfo.Name,
                    StartingStack = playerInfo.Stake,
                    SeatNumber = playerInfo.Seatid + 1,
                    IsSittingOut = !playerInfo.InGame
                };

                handHistory.Players.Add(player);
            }
        }

        private void ProcessNoticeGameElectDealer(NoticeGameElectDealer noticeGameElectDealer, HandHistory handHistory)
        {
            handHistory.DealerButtonPosition = noticeGameElectDealer.DealerSeatId + 1;
        }

        private void ProcessNoticeGameAnte(NoticeGameAnte noticeGameAnte, HandHistory handHistory)
        {
            if (noticeGameAnte.SeatList == null ||
                noticeGameAnte.AmountList == null ||
                noticeGameAnte.SeatList.Length != noticeGameAnte.AmountList.Length)
            {
                return;
            }

            for (var i = 0; i < noticeGameAnte.SeatList.Length; i++)
            {
                var seat = noticeGameAnte.SeatList[i] + 1;
                var ante = noticeGameAnte.AmountList[i];

                var anteAction = new HandAction(GetPlayerName(handHistory, seat),
                    HandActionType.ANTE,
                    ante,
                    Street.Preflop);

                handHistory.HandActions.Add(anteAction);
            }
        }

        private void ProcessNoticeGameBlind(NoticeGameBlind noticeGameBlind, HandHistory handHistory)
        {
            if (!roomsData.TryGetValue(noticeGameBlind.RoomId, out NoticeGameSnapShot noticeGameSnapShot))
            {
                throw new HandBuilderException(handHistory.HandId, $"NoticeGameSnapShot has not been found for room #{noticeGameBlind.RoomId}.");
            }

            var gameRoomInfo = noticeGameSnapShot.Params ?? throw new HandBuilderException(handHistory.HandId, "NoticeGameSnapShot.Params must be not empty.");

            var ante = Math.Abs(handHistory.HandActions.Where(x => x.HandActionType == HandActionType.ANTE).MaxOrDefault(x => x.Amount));

            if (ante != 0)
            {
                ante = ante < gameRoomInfo.RuleAnteAmount ? gameRoomInfo.RuleAnteAmount : ante;
            }

            handHistory.GameDescription = new GameDescriptor(
                EnumPokerSites.PokerKing,
                GameType.NoLimitHoldem,
                Limit.FromSmallBlindBigBlind(noticeGameBlind.SBAmount, noticeGameBlind.BBAmount, Currency.YUAN, ante != 0, ante),
                TableType.FromTableTypeDescriptions(gameRoomInfo.GameMode == 3 ? TableTypeDescription.ShortDeck : TableTypeDescription.Regular),
                SeatType.FromMaxPlayers(gameRoomInfo.PlayerCountMax), null);

            handHistory.GameDescription.Identifier = noticeGameBlind.RoomId;

            handHistory.TableName = gameRoomInfo.GameName;

            if (noticeGameBlind.SBAmount > 0)
            {
                handHistory.HandActions.Add(new HandAction(GetPlayerName(handHistory, noticeGameBlind.SBSeatId + 1),
                        HandActionType.SMALL_BLIND,
                        noticeGameBlind.SBAmount,
                        Street.Preflop));
            }

            handHistory.HandActions.Add(new HandAction(GetPlayerName(handHistory, noticeGameBlind.BBSeatId + 1),
                 HandActionType.BIG_BLIND,
                 noticeGameBlind.BBAmount,
                 Street.Preflop));

            if (noticeGameBlind.StraddleSeatList != null && noticeGameBlind.StraddleSeatList.Length > 0 &&
                noticeGameBlind.StraddleAmountList != null && noticeGameBlind.StraddleAmountList.Length == noticeGameBlind.StraddleSeatList.Length)
            {
                for (var straddleIndex = 0; straddleIndex < noticeGameBlind.StraddleSeatList.Length; straddleIndex++)
                {
                    var straddleSeat = noticeGameBlind.StraddleSeatList[straddleIndex];
                    var straddleAmount = noticeGameBlind.StraddleAmountList[straddleIndex];

                    if (straddleSeat < 0 || straddleAmount <= 0)
                    {
                        continue;
                    }

                    handHistory.HandActions.Add(new HandAction(GetPlayerName(handHistory, straddleSeat + 1),
                        HandActionType.STRADDLE,
                        straddleAmount,
                        Street.Preflop));
                }
            }

            if (noticeGameBlind.PostSeatList != null)
            {
                foreach (var postSeat in noticeGameBlind.PostSeatList)
                {
                    handHistory.HandActions.Add(new HandAction(GetPlayerName(handHistory, postSeat + 1),
                        HandActionType.POSTS,
                        noticeGameBlind.BBAmount,
                        Street.Preflop));
                }
            }
        }

        private void ProcessNoticeGameHoleCard(NoticeGameHoleCard noticeGameHoleCard, HandHistory handHistory, uint userId)
        {
            if (noticeGameHoleCard.HoldCards == null)
            {
                return;
            }

            var cards = noticeGameHoleCard.HoldCards.Select(c => ConvertCardItem(c)).ToArray();

            var hero = handHistory.Players.FirstOrDefault(x => x.PlayerName == userId.ToString());

            if (hero == null)
            {
                LogProvider.Log.Warn($"Hero not found for hand {handHistory.HandId}, room {noticeGameHoleCard.RoomId}, user {userId}");
                return;
            }

            handHistory.Hero = hero;
            hero.HoleCards = HoleCards.FromCards(hero.PlayerName, cards);
        }

        private void ProcessNoticePlayerAction(NoticePlayerAction noticePlayerAction, HandHistory handHistory)
        {
            var actionType = ParseHandActionType(noticePlayerAction);

            if (actionType == HandActionType.UNKNOWN)
            {
                return;
            }


            HandAction action;

            if (actionType == HandActionType.ALL_IN)
            {
                var player = GetPlayer(handHistory, noticePlayerAction.LastActionSeatId + 1);

                var playerPutInPot = Math.Abs(handHistory.HandActions.Where(x => x.PlayerName == player.PlayerName).Sum(x => x.Amount));
                var allInAmount = player.StartingStack - playerPutInPot;

                action = new AllInAction(player.PlayerName,
                    allInAmount,
                    handHistory.CommunityCards.Street, false);
            }
            else
            {
                action = new HandAction(GetPlayerName(handHistory, noticePlayerAction.LastActionSeatId + 1),
                    actionType,
                    noticePlayerAction.Amount,
                    handHistory.CommunityCards.Street);
            }

            handHistory.HandActions.Add(action);
        }

        private void ProcessNoticeGameCommunityCards(NoticeGameCommunityCards noticeGameCommunityCards, HandHistory handHistory)
        {
            if (noticeGameCommunityCards.Cards == null)
            {
                LogProvider.Log.Warn(CustomModulesNames.PKCatcher, $"NoticeGameCommunityCards.Cards must be not null.");
                return;
            }

            foreach (var cardItem in noticeGameCommunityCards.Cards)
            {
                var card = ConvertCardItem(cardItem);
                handHistory.CommunityCards.AddCard(card);
            }
        }

        private void ProcessNoticeGameShowDown(NoticeGameShowDown noticeGameShowDown, HandHistory handHistory)
        {
            if (noticeGameShowDown.Shows == null)
            {
                return;
            }

            foreach (var show in noticeGameShowDown.Shows)
            {
                var player = GetPlayer(handHistory, show.SeatId + 1);

                if (handHistory.HandActions.Any(x => x.PlayerName == player.PlayerName && x.HandActionType == HandActionType.SHOW))
                {
                    continue;
                }

                var showAction = new HandAction(player.PlayerName,
                    HandActionType.SHOW,
                    0,
                    Street.Showdown);

                handHistory.HandActions.Add(showAction);

                if (player.hasHoleCards)
                {
                    continue;
                }

                if (show.Cards == null)
                {
                    LogProvider.Log.Warn(CustomModulesNames.PKCatcher, $"NoticeGameShowDown.Show.Cards must be not null.");
                    continue;
                }

                var cards = show.Cards.Select(c => ConvertCardItem(c)).ToArray();
                player.HoleCards = HoleCards.FromCards(player.PlayerName, cards);
            }
        }

        private void ProcessNoticeGameSettlement(NoticeGameSettlement noticeGameSettlement, HandHistory handHistory)
        {
            if (noticeGameSettlement.Winners == null)
            {
                throw new HandBuilderException(handHistory.HandId, $"NoticeGameSettlement.Winners must be not null.");
            }

            foreach (var winner in noticeGameSettlement.Winners)
            {
                var player = GetPlayer(handHistory, winner.SeatId + 1);

                var winningAction = new WinningsAction(player.PlayerName,
                     HandActionType.WINS, winner.Amount, 0);

                handHistory.HandActions.Add(winningAction);

                player.Win = winner.Amount;
            }
        }

        #region Static helpers

        private static string GetPlayerName(HandHistory handHistory, int seat)
        {
            var player = GetPlayer(handHistory, seat);
            return player.PlayerName;
        }

        private static Player GetPlayer(HandHistory handHistory, int seat)
        {
            var player = handHistory.Players.FirstOrDefault(x => x.SeatNumber == seat);

            if (player == null)
            {
                throw new HandBuilderException(handHistory.HandId, $"Player with seat={seat} not found.");
            }

            return player;
        }

        private static HandActionType ParseHandActionType(NoticePlayerAction noticePlayerAction)
        {
            switch (noticePlayerAction.ActionType)
            {
                case ActionType.Allin:
                    return HandActionType.ALL_IN;
                case ActionType.Bet:
                    return HandActionType.BET;
                case ActionType.Call:
                case ActionType.CallMuck:
                    return HandActionType.CALL;
                case ActionType.Check:
                    return HandActionType.CHECK;
                case ActionType.Fold:
                    return HandActionType.FOLD;
                case ActionType.Post:
                    return HandActionType.POSTS;
                case ActionType.Raise:
                    return HandActionType.RAISE;
                default:
                    return HandActionType.UNKNOWN;
            }
        }

        private static readonly ReadOnlyDictionary<int, int> suitConversionTable = new ReadOnlyDictionary<int, int>(new Dictionary<int, int>
        {
            [0] = 1,
            [1] = 0,
            [2] = 2,
            [3] = 3
        });

        private static Card ConvertCardItem(CardItem cardItem)
        {
            var card = suitConversionTable[cardItem.Suit] * 13 + cardItem.Rank;
            return Card.GetCardFromIntValue(card);
        }

        #endregion
    }
}