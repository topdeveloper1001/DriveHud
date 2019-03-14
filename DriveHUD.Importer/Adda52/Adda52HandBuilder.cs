//-----------------------------------------------------------------------
// <copyright file="Adda52HandBuilder.cs" company="Ace Poker Solutions">
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
using DriveHUD.Importers.Adda52.Model;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DriveHUD.Importers.Adda52
{
    internal class Adda52HandBuilder : IAdda52HandBuilder
    {
        private Dictionary<int, List<Adda52JsonPackage>> roomPackages = new Dictionary<int, List<Adda52JsonPackage>>();
        private Dictionary<int, RoomData> roomsData = new Dictionary<int, RoomData>();
        private Dictionary<string, MTTCombinedData> mttData = new Dictionary<string, MTTCombinedData>();

        private static readonly Currency currency = Currency.INR;
        private static string heroName;
        private static readonly string loggerName = EnumPokerSites.Adda52.ToString();

        public bool TryBuild(Adda52JsonPackage package, out HandHistory handHistory)
        {
            handHistory = null;

            if (package == null)
            {
                return false;
            }

            if (package.PackageType == Adda52PackageType.RoomData)
            {
                ParsePackage<RoomData>(package, x => ProcessRoomData(package.RoomId, x));
                return false;
            }

            if (package.PackageType == Adda52PackageType.AccessToken)
            {
                ParsePackage<AccessToken>(package, x => ProcessAccessToken(x));
                return false;
            }

            if (package.PackageType == Adda52PackageType.MTTInfo)
            {
                ParsePackage<MTTInfo>(package, x => ProcessMTTInfo(x));
                return false;
            }

            if (package.PackageType == Adda52PackageType.MTTTables)
            {
                ParsePackage<MTTTables>(package, x => ProcessMTTTables(x));
                return false;
            }

            if (package.PackageType == Adda52PackageType.MTTPrizes)
            {
                ParsePackage<MTTPrizes>(package, x => ProcessMTTPrizes(x));
                return false;
            }

            if (!roomPackages.TryGetValue(package.RoomId, out List<Adda52JsonPackage> packages))
            {
                packages = new List<Adda52JsonPackage>();
                roomPackages.Add(package.RoomId, packages);
            }

            packages.Add(package);

            if (package.PackageType == Adda52PackageType.RoundEnd)
            {
                handHistory = BuildHand(packages);
            }

            return handHistory != null && handHistory.Players.Count > 0;
        }

        private HandHistory BuildHand(List<Adda52JsonPackage> packages)
        {
            HandHistory handHistory = null;

            try
            {
                if (!Validate(packages))
                {
                    packages.Clear();
                    return handHistory;
                }

                var isGameStarted = false;

                Ante ante = null;

                foreach (var package in packages)
                {
                    if (package.PackageType == Adda52PackageType.GameStart)
                    {
                        handHistory = new HandHistory
                        {
                            DateOfHandUtc = package.TimestampUtc
                        };

                        ParsePackage<GameStart>(package, x => ProcessGameStart(x, handHistory));

                        isGameStarted = true;
                        continue;
                    }

                    if (!isGameStarted)
                    {
                        continue;
                    }

                    switch (package.PackageType)
                    {
                        case Adda52PackageType.Ante:
                            ParsePackage<Ante>(package, x => ante = x);
                            break;
                        case Adda52PackageType.Blinds:
                            ParsePackage<Blinds>(package, x => ProcessBlinds(package.RoomId, x, handHistory));
                            break;
                        case Adda52PackageType.CommunityCard:
                            ParsePackage<CommunityCardInfo>(package, x => ProcessCommunityCards(x, handHistory));
                            break;
                        case Adda52PackageType.Dealer:
                            ParsePackage<Dealer>(package, x => ProcessDealer(x, handHistory));
                            break;
                        case Adda52PackageType.UserAction:
                            ParsePackage<UserAction>(package, x => ProcessUserAction(x, handHistory));
                            break;
                        case Adda52PackageType.UncalledBet:
                            ParsePackage<UncalledBet>(package, x => ProcessUncalledBet(x, handHistory));
                            break;
                        case Adda52PackageType.SeatInfo:
                            ParsePackage<RoomSeatInfo>(package, x =>
                            {
                                ProcessSeatInfo(x, handHistory);

                                if (ante != null)
                                {
                                    ProcessAnte(ante, handHistory);
                                    ante = null;
                                }
                            });
                            break;
                        case Adda52PackageType.Winner:
                            ParsePackage<Winner>(package, x => ProcessWinner(x, handHistory));
                            break;
                        case Adda52PackageType.HoleCard:
                            ParsePackage<HoleCard>(package, x => ProcessHoleCard(x, handHistory));
                            break;
                    }
                }

                AdjustHandHistory(handHistory);

                return handHistory;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to build hand #{handHistory?.HandId ?? 0} room #{handHistory?.GameDescription.Identifier ?? 0}. [{loggerName}]", e);
                return null;
            }
            finally
            {
                packages.Clear();
            }
        }

        private bool Validate(List<Adda52JsonPackage> packages)
        {
            return packages.Any(x => x.PackageType == Adda52PackageType.GameStart);
        }

        private void ProcessAccessToken(AccessToken accessToken)
        {
            LogProvider.Log.Info(this, $"User {accessToken.UserId} has logged in. [{loggerName}]");
            heroName = accessToken.UserId.ToString();
        }

        private void ProcessMTTInfo(MTTInfo mttInfo)
        {
            if (!mttData.TryGetValue(mttInfo.RoomName, out MTTCombinedData mttCombinedData))
            {
                mttCombinedData = new MTTCombinedData
                {
                    MTTInfo = mttInfo
                };

                mttData.Add(mttInfo.RoomName, mttCombinedData);

                LogProvider.Log.Info(this, $"MTT info {mttInfo.RoomName} has been stored. [{loggerName}]");
                return;
            }

            mttCombinedData.MTTInfo = mttInfo;

            LogProvider.Log.Info(this, $"MTT info {mttInfo.RoomName} has been updated. [{loggerName}]");
        }

        private void ProcessMTTTables(MTTTables mttTables)
        {
            if (!mttData.TryGetValue(mttTables.RoomName, out MTTCombinedData mttCombinedData))
            {
                mttCombinedData = new MTTCombinedData
                {
                    MTTTables = mttTables
                };

                mttData.Add(mttTables.RoomName, mttCombinedData);

                LogProvider.Log.Info(this, $"MTT tables info {mttTables.RoomName} has been stored. [{loggerName}]");
                return;
            }

            mttCombinedData.MTTTables = mttTables;

            LogProvider.Log.Info(this, $"MTT tables info {mttTables.RoomName} has been updated. [{loggerName}]");
        }

        private void ProcessMTTPrizes(MTTPrizes mttPrizes)
        {
            if (!mttData.TryGetValue(mttPrizes.RoomName, out MTTCombinedData mttCombinedData))
            {
                mttCombinedData = new MTTCombinedData
                {
                    MTTPrizes = mttPrizes
                };

                mttData.Add(mttPrizes.RoomName, mttCombinedData);

                LogProvider.Log.Info(this, $"MTT prizes info {mttPrizes.RoomName} has been stored. [{loggerName}]");
                return;
            }

            mttCombinedData.MTTPrizes = mttPrizes;

            LogProvider.Log.Info(this, $"MTT prizes info {mttPrizes.RoomName} has been updated. [{loggerName}]");
        }

        private void ProcessGameStart(GameStart gameStart, HandHistory handHistory)
        {
            var hashIndex = gameStart.RoomName.IndexOf('#');

            if (hashIndex <= 0)
            {
                throw new HandBuilderException($"Failed to parse hand id from {gameStart.RoomName} in GameStart.");
            }

            var handNumberText = gameStart.RoomName.Substring(hashIndex + 1);

            if (!long.TryParse(handNumberText, out long handNumber))
            {
                throw new HandBuilderException($"Failed to parse hand id from {gameStart.RoomName} in GameStart.");
            }

            handNumber = handNumber * 100000 + gameStart.RoundId;

            handHistory.HandId = handNumber;
        }

        private void ProcessAnte(Ante ante, HandHistory handHistory)
        {
            var anteAmount = (decimal)ante.Amount / handHistory.Players.Count;

            handHistory.GameDescription.Limit.Ante = anteAmount;
            handHistory.GameDescription.Limit.IsAnteTable = true;

            var sbAction = handHistory.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND);

            IEnumerable<Player> players;

            if (sbAction != null)
            {
                players = handHistory.Players
                    .SkipWhile(x => x.PlayerName != sbAction.PlayerName)
                    .Concat(handHistory.Players.TakeWhile(x => x.PlayerName != sbAction.PlayerName))
                    .Reverse();
            }
            else
            {
                players = handHistory.Players.Reverse();
            }

            foreach (var player in players)
            {
                var anteAction = new HandAction(player.PlayerName,
                    HandActionType.ANTE,
                    anteAmount,
                    Street.Preflop);

                handHistory.HandActions.Insert(0, anteAction);
            }
        }

        private void ProcessBlinds(int roomId, Blinds blinds, HandHistory handHistory)
        {
            if (!roomsData.TryGetValue(roomId, out RoomData roomData))
            {
                throw new HandBuilderException(handHistory.HandId, $"RoomData has not been found for room #{roomId}.");
            }

            TournamentDescriptor tournament = null;

            var isSTT = roomData.GameType.Equals("STT", StringComparison.OrdinalIgnoreCase);
            var isMTT = !isSTT && roomData.GameType.Equals("MTT", StringComparison.OrdinalIgnoreCase);

            if (isSTT || isMTT)
            {
                tournament = new TournamentDescriptor
                {
                    Speed = TournamentSpeed.Regular
                };

                if (isSTT)
                {
                    ProcessSTT(tournament, blinds.RoomName, roomData);
                    tournament.StartDate = handHistory.DateOfHandUtc;
                }
                else
                {
                    ProcessMTT(tournament, blinds.RoomName);
                }
            }

            var tableType = roomData.TurnTime != 0 && roomData.TurnTime <= 15 ? TableTypeDescription.Speed : TableTypeDescription.Regular;

            if (roomData.IsAnonymousTable)
            {
                tableType = TableTypeDescription.Anonymous;
            }

            var limit = Limit.FromSmallBlindBigBlind(blinds.SmallBlind, blinds.BigBlind,
                tournament != null ? Currency.Chips : (roomData.IsFreeroll ? Currency.PlayMoney : currency));

            handHistory.GameDescription = new GameDescriptor(
                tournament != null ? PokerFormat.Tournament : PokerFormat.CashGame,
                EnumPokerSites.Adda52,
                GetGameType(roomData),
                limit,
                TableType.FromTableTypeDescriptions(tableType),
                SeatType.FromMaxPlayers(roomData.MaxPlayers), tournament);

            handHistory.GameDescription.Identifier = roomId;
            handHistory.TableName = blinds.RoomName;

            if (tournament == null)
            {
                handHistory.GameDescription.CashBuyInHigh = roomData.BuyinHigh;
            }
        }

        private void ProcessMTT(TournamentDescriptor tournament, string roomName)
        {
            tournament.TournamentsTags = TournamentsTags.MTT;

            var tournamentData = mttData.Values.FirstOrDefault(x =>
                    x.MTTTables != null && x.MTTTables.MTTTableInfo != null &&
                    x.MTTTables.MTTTableInfo.MTTTables != null &&
                    x.MTTTables.MTTTableInfo.MTTTables.Any(m => m.TableName.Equals(roomName)));

            if (tournamentData == null)
            {
                tournament.TournamentId = roomName;
                LogProvider.Log.Warn(this, $"Failed to find MTT info for {roomName}. [{loggerName}]");
            }
            else
            {
                tournament.TournamentId = tournamentData.MTTTables.RoomName.Substring(tournamentData.MTTTables.RoomName.IndexOf('#') + 1);
                tournament.TournamentName = tournamentData.MTTPrizes?.MTTPrizeInfo?.GameDetail;

                if (DateTime.TryParseExact(tournamentData.MTTInfo?.MTTDetailedInfo?.StartDateText, "MMM dd, yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
                {
                    var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    tournament.StartDate = TimeZoneInfo.ConvertTimeToUtc(startDate, timeZoneInfo);
                }
                else
                {
                    tournament.StartDate = DateTime.UtcNow;
                }

                var entryChips = tournamentData.MTTInfo?.MTTDetailedInfo?.EntryChipInfo?.EntryChips?.FirstOrDefault(x => x.IsReal);

                if (entryChips != null)
                {
                    tournament.StartingStack = entryChips.InitialStakes;

                    var comissionRate = tournamentData.MTTInfo.MTTDetailedInfo.ComissionRate;

                    var fee = Math.Round(entryChips.Amount * comissionRate / 100, 0);
                    // it isn't correct to include bounty in prize, but currently DH doesn't show bounty as standalone field
                    var prizePool = entryChips.Amount - fee + tournamentData.MTTInfo.MTTDetailedInfo.BountyAmount;

                    tournament.BuyIn = Buyin.FromBuyinRake(prizePool, fee, currency);
                }
                else
                {
                    LogProvider.Log.Warn(this, $"Failed to find buyin info for {tournamentData.MTTTables.RoomName}. [{loggerName}]");
                }
            }
        }

        private void ProcessSTT(TournamentDescriptor tournament, string roomName, RoomData roomData)
        {
            tournament.TournamentsTags = TournamentsTags.STT;
            tournament.TournamentId = roomName.Substring(roomName.IndexOf('#') + 1);
            tournament.TournamentName = roomName;

            if (string.IsNullOrEmpty(roomData.BuyinFees))
            {
                return;
            }

            var buyinFeeStrings = roomData.BuyinFees.Split('#');

            if (!int.TryParse(buyinFeeStrings[0], out int prizePool))
            {
                LogProvider.Log.Warn(this, $"Failed to parse prize pool of buyinFee {roomData.BuyinFees} of STT {roomName}. [{loggerName}]");
            }

            var fee = 0;

            if (buyinFeeStrings.Length > 1 && !string.IsNullOrEmpty(buyinFeeStrings[1]) && !int.TryParse(buyinFeeStrings[1], out fee))
            {
                LogProvider.Log.Warn(this, $"Failed to parse fee of buyinFee {roomData.BuyinFees} of STT {roomName}. [{loggerName}]");
            }

            tournament.BuyIn = Buyin.FromBuyinRake(prizePool, fee, currency);
        }

        private void ProcessDealer(Dealer dealer, HandHistory handHistory)
        {
            handHistory.DealerButtonPosition = dealer.DealerSeat;
        }

        private void ProcessUserAction(UserAction userAction, HandHistory handHistory)
        {
            var street = handHistory.CommunityCards.Street;

            var playerName = userAction.PlayerId.ToString();

            var actionType = GetHandActionType(userAction, handHistory, playerName, street);

            HandAction action;

            if (actionType == HandActionType.ALL_IN)
            {
                var player = handHistory.Players.FirstOrDefault(x => x.PlayerName == playerName);

                if (player == null)
                {
                    throw new HandBuilderException($"Failed to find player {playerName} for allin action.");
                }

                var playerPutInPot = Math.Abs(handHistory.HandActions.Where(x => x.PlayerName == player.PlayerName).Sum(x => x.Amount));
                var allInAmount = player.StartingStack - playerPutInPot;

                action = new AllInAction(player.PlayerName,
                    allInAmount,
                    street, false);
            }
            else
            {
                action = new HandAction(playerName,
                    actionType,
                    userAction.Amount,
                    street);
            }

            handHistory.HandActions.Add(action);
        }

        private void ProcessUncalledBet(UncalledBet uncalledBet, HandHistory handHistory)
        {
            var player = handHistory.Players.FirstOrDefault(x => x.SeatNumber == uncalledBet.SeatId);

            if (player == null)
            {
                throw new HandBuilderException($"Failed to find player for uncalled bet action on seat {uncalledBet.SeatId}.");
            }

            var action = new HandAction(player.PlayerName,
                HandActionType.UNCALLED_BET,
                uncalledBet.Amount,
                handHistory.CommunityCards.Street);

            handHistory.HandActions.Add(action);
        }

        private void ProcessHoleCard(HoleCard holeCard, HandHistory handHistory)
        {
            var holeCards = string.Join(string.Empty, holeCard.HoleCardsInfo.HoleCards.Where(x => x != null).Select(x => x.ToString()));

            if (string.IsNullOrEmpty(holeCards))
            {
                return;
            }

            var hero = handHistory.Players.FirstOrDefault(x => x.PlayerName == heroName);

            if (hero == null)
            {
                LogProvider.Log.Warn(this, $"Hero {heroName} wasn't found in hand. [{loggerName}]");
                return;
            }

            hero.HoleCards = HoleCards.FromCards(holeCards);
            handHistory.Hero = hero;
        }

        private void ProcessSeatInfo(RoomSeatInfo seatInfo, HandHistory handHistory)
        {
            if (handHistory.Players.Count > 0)
            {
                return;
            }

            if (seatInfo.SeatInfo == null || seatInfo.SeatInfo.Seats == null)
            {
                throw new HandBuilderException("RoomSeatInfo.SeatInfo must be not null");
            }

            foreach (var seat in seatInfo.SeatInfo.Seats)
            {
                if (seat.PlayerId <= 0)
                {
                    continue;
                }

                var player = new Player
                {
                    PlayerName = seat.PlayerId.ToString(),
                    PlayerNick = seat.PlayerName,
                    SeatNumber = seat.SeatId,
                    StartingStack = seat.Chips
                };

                handHistory.Players.Add(player);
            }
        }

        private void ProcessWinner(Winner winner, HandHistory handHistory)
        {
            if (handHistory == null)
            {
                return;
            }

            foreach (var playerWinner in winner.WinnerInfo.Winners)
            {
                var playerName = playerWinner.PlayerId.ToString();

                var winningAction = new WinningsAction(playerName,
                    HandActionType.WINS,
                    playerWinner.Amount,
                    playerWinner.PotId);

                handHistory.HandActions.Add(winningAction);

                var player = handHistory.Players.FirstOrDefault(x => x.PlayerName == playerName);

                if (player == null)
                {
                    throw new HandBuilderException($"Failed to find player {playerName} for set win.");
                }

                player.Win = playerWinner.Amount;
            }

            foreach (var playerRank in winner.WinnerInfo.PlayerRanks)
            {
                if (playerRank.HoleCards == null ||
                    playerRank.HoleCards.HoleCards == null ||
                    playerRank.HoleCards.HoleCards.Length == 0)
                {
                    continue;
                }

                var playerName = playerRank.PlayerId.ToString();
                var player = handHistory.Players.FirstOrDefault(x => x.PlayerName == playerName);

                if (player == null)
                {
                    throw new HandBuilderException($"Failed to find player {playerRank.PlayerId} for set hole cards.");
                }

                if (player.hasHoleCards)
                {
                    continue;
                }

                var holeCards = string.Join(string.Empty, playerRank.HoleCards.HoleCards.Where(x => x != null).Select(x => x.ToString()));

                if (!string.IsNullOrEmpty(holeCards) && (holeCards.Length == 4 || holeCards.Length == 8))
                {
                    player.HoleCards = HoleCards.FromCards(holeCards);
                }
                else
                {
                    var possibleHoleCards = new List<string>();

                    if (playerRank.KickerCards != null && playerRank.KickerCards.Length > 0)
                    {
                        possibleHoleCards.AddRange(playerRank.KickerCards.Where(x => x != null).Select(x => x.ToString()));
                    }

                    if (playerRank.RankCards != null && playerRank.RankCards.Length > 0)
                    {
                        possibleHoleCards.AddRange(playerRank.RankCards.Where(x => x != null).Select(x => x.ToString()));
                    }

                    var playerHoleCards = string.Join(string.Empty, possibleHoleCards
                        .Where(x => !handHistory.CommunityCardsString.ContainsIgnoreCase(x)).Distinct());

                    if (!string.IsNullOrEmpty(playerHoleCards) && (playerHoleCards.Length == 4 || playerHoleCards.Length == 8))
                    {
                        player.HoleCards = HoleCards.FromCards(playerHoleCards);
                    }
                }
            }
        }

        private void ProcessCommunityCards(CommunityCardInfo communityCardInfo, HandHistory handHistory)
        {
            if (communityCardInfo.CommunityCard == null || communityCardInfo.CommunityCard.CommunityCards == null)
            {
                throw new HandBuilderException("Community cards must be not null.");
            }

            var boardCards = string.Join(string.Empty, communityCardInfo.CommunityCard.CommunityCards.Select(x => x.ToString()));
            handHistory.CommunityCards = BoardCards.FromCards(boardCards);
        }

        private void ProcessRoomData(int roomId, RoomData roomData)
        {
            if (!roomsData.ContainsKey(roomId))
            {
                roomsData.Add(roomId, roomData);
                LogProvider.Log.Info(this, $"RoomData of room {roomId} has been stored. [{loggerName}]");
                return;
            }

            roomsData[roomId] = roomData;

            LogProvider.Log.Info(this, $"RoomData of room {roomId} has been updated. [{loggerName}]");
        }

        private void AdjustHandHistory(HandHistory handHistory)
        {
            if (handHistory == null)
            {
                return;
            }

            HandHistoryUtils.UpdateAllInActions(handHistory);
            HandHistoryUtils.CalculateBets(handHistory);
            HandHistoryUtils.CalculateTotalPot(handHistory);
            HandHistoryUtils.RemoveSittingOutPlayers(handHistory);

            foreach (var player in handHistory.Players)
            {
                handHistory.HandActions
                    .Where(x => x.PlayerName == player.PlayerName)
                    .ForEach(x => x.PlayerName = player.PlayerNick);

                if (handHistory.HeroName == player.PlayerName)
                {
                    handHistory.HeroName = player.PlayerNick;
                }

                player.PlayerName = player.PlayerNick;
                player.PlayerNick = null;
            }
        }

        private void ParsePackage<T>(Adda52JsonPackage package, Action<T> action) where T : class
        {
            Adda52Message<T> message;

            try
            {
                message = JsonConvert.DeserializeObject<Adda52Message<T>>(package.JsonData);
            }
            catch
            {
                throw new DHInternalException(new NonLocalizableString($"Failed to deserialize package of {package.PackageType} type."));
            }

            action?.Invoke(message.Body.Data);
        }

        private GameType GetGameType(RoomData roomData)
        {
            if (roomData.RingVariant == null || roomData.RingVariant.Equals("HOLDEM", StringComparison.OrdinalIgnoreCase))
            {
                if (roomData.BettingRule.Equals("NL", StringComparison.OrdinalIgnoreCase))
                {
                    return GameType.NoLimitHoldem;
                }
                else if (roomData.BettingRule.Equals("PLO", StringComparison.OrdinalIgnoreCase))
                {
                    return GameType.PotLimitHoldem;
                }
            }
            else if (roomData.RingVariant.Equals("OMAHA", StringComparison.OrdinalIgnoreCase))
            {
                if (roomData.BettingRule.Equals("NL", StringComparison.OrdinalIgnoreCase))
                {
                    return GameType.NoLimitOmaha;
                }
                else if (roomData.BettingRule.Equals("PLO", StringComparison.OrdinalIgnoreCase))
                {
                    return GameType.PotLimitOmaha;
                }
            }
            else if (roomData.RingVariant.Equals("OMAHA_HL", StringComparison.OrdinalIgnoreCase))
            {
                if (roomData.BettingRule.Equals("NL", StringComparison.OrdinalIgnoreCase))
                {
                    return GameType.NoLimitOmahaHiLo;
                }
                else if (roomData.BettingRule.Equals("PLO", StringComparison.OrdinalIgnoreCase))
                {
                    return GameType.PotLimitOmahaHiLo;
                }
            }

            throw new HandBuilderException($"Unsupported game type: {roomData.RingVariant} {roomData.BettingRule}");
        }

        private HandActionType GetHandActionType(UserAction userAction, HandHistory handHistory, string playerName, Street street)
        {
            switch (userAction.Action)
            {
                case UserActionType.Fold:
                    return HandActionType.FOLD;
                case UserActionType.SmallBlind:
                    return HandActionType.SMALL_BLIND;
                case UserActionType.BigBlind:
                    return HandActionType.BIG_BLIND;
                case UserActionType.AllIn:
                    return HandActionType.ALL_IN;
                case UserActionType.BetOrRaise:
                    {
                        if (street == Street.Preflop)
                        {
                            return HandActionType.RAISE;
                        }

                        var streetActions = handHistory.HandActions.Where(x => x.Street == street);

                        if (streetActions.Any(x => x.Amount < 0))
                        {
                            return HandActionType.RAISE;
                        }

                        return HandActionType.BET;
                    }
                case UserActionType.CheckOrCall:
                    return userAction.Amount == 0 ? HandActionType.CHECK : HandActionType.CALL;
                default:
                    throw new HandBuilderException($"Unknown action type: {userAction.Action}");
            }
        }

        /// <summary>
        /// Represents combined info about mtt and mtt tables
        /// </summary>
        private class MTTCombinedData
        {
            public MTTInfo MTTInfo { get; set; }

            public MTTTables MTTTables { get; set; }

            public MTTPrizes MTTPrizes { get; set; }
        }
    }
}