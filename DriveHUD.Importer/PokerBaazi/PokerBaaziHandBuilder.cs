//-----------------------------------------------------------------------
// <copyright file="PokerBaaziHandBuilder.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
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
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.Importers.PokerBaazi.Model;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.PokerBaazi
{
    internal class PokerBaaziHandBuilder : IPokerBaaziHandBuilder
    {
        private Dictionary<uint, List<PokerBaaziPackage>> roomPackages = new Dictionary<uint, List<PokerBaaziPackage>>();
        private Dictionary<uint, PokerBaaziInitResponse> roomsInitResponses = new Dictionary<uint, PokerBaaziInitResponse>();
        private Dictionary<long, PokerBaaziTournamentDetailsResponse> tournamentDetails = new Dictionary<long, PokerBaaziTournamentDetailsResponse>();

        private static readonly Currency currency = Currency.INR;
        private static readonly string loggerName = EnumPokerSites.PokerBaazi.ToString();
        private static readonly EnumPokerSites PokerSite = EnumPokerSites.PokerBaazi;

        /// <summary>
        /// Finds the <see cref="PokerBaaziInitResponse"/> of the specified room
        /// </summary>
        /// <param name="roomId">Room id</param>
        /// <returns><see cref="PokerBaaziInitResponse"/> of the specified room if found; otherwise - null</returns>
        public PokerBaaziInitResponse FindInitResponse(uint roomId)
        {
            roomsInitResponses.TryGetValue(roomId, out PokerBaaziInitResponse response);
            return response;
        }

        /// <summary>
        /// Tries to build hand history by using buffered packages or buffer the specified package for further using
        /// </summary>
        /// <param name="package">Package to buffer</param>
        /// <param name="handHistory">Hand history</param>
        /// <returns>True if hand can be built; otherwise - false</returns>
        public bool TryBuild(PokerBaaziPackage package, out HandHistory handHistory)
        {
            handHistory = null;

            if (package == null)
            {
                return false;
            }

            if (package.PackageType == PokerBaaziPackageType.InitResponse)
            {
                ParsePackage<PokerBaaziInitResponse>(package, x => ProcessInitResponse(x));
                return false;
            }
            else if (package.PackageType == PokerBaaziPackageType.TournamentDetailsResponse)
            {
                ParsePackage<PokerBaaziTournamentDetailsResponse>(package, x => ProcessTournamentDetailsResponse(x));
                return false;
            }

            if (!roomPackages.TryGetValue(package.RoomId, out List<PokerBaaziPackage> packages))
            {
                packages = new List<PokerBaaziPackage>();
                roomPackages.Add(package.RoomId, packages);
            }

            packages.Add(package);

            if (package.PackageType == PokerBaaziPackageType.WinnerResponse)
            {
                handHistory = BuildHand(packages);
            }

            return handHistory != null && handHistory.Players.Count > 0;
        }

        /// <summary>
        /// Builds hand history from the specified packages
        /// </summary>
        /// <param name="packages">Packages</param>
        /// <returns><see cref="HandHistory"/> or null if it can't be built</returns>
        private HandHistory BuildHand(List<PokerBaaziPackage> packages)
        {
            HandHistory handHistory = null;

            try
            {
                if (!Validate(packages))
                {
                    packages.Clear();
                    return handHistory;
                }

                foreach (var package in packages)
                {
                    switch (package.PackageType)
                    {
                        case PokerBaaziPackageType.StartGameResponse:
                            handHistory = new HandHistory();
                            ParsePackage<PokerBaaziStartGameResponse>(package, (response, timestamp) => ProcessSpectatorResponse(response, timestamp, handHistory));
                            break;
                        case PokerBaaziPackageType.UserButtonActionResponse:
                            ParsePackage<PokerBaaziUserActionResponse>(package, response => ProcessUserActionResponse(response, handHistory));
                            break;
                        case PokerBaaziPackageType.RoundResponse:
                            ParsePackage<PokerBaaziRoundResponse>(package, response => ProcessRoundResponse(response, handHistory));
                            break;
                        case PokerBaaziPackageType.WinnerResponse:
                            ParsePackage<PokerBaaziWinnerResponse>(package, response => ProcessWinnerResponse(response, handHistory));
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

        /// <summary>
        /// Validates the specified packages
        /// </summary>
        /// <param name="packages">Packages to validate</param>
        /// <returns>True if packages are valid; otherwise - false</returns>
        private bool Validate(List<PokerBaaziPackage> packages)
        {
            return packages.Any(x => x.PackageType == PokerBaaziPackageType.StartGameResponse);
        }

        /// <summary>
        /// Processes the specified <see cref="PokerBaaziInitResponse"/>
        /// </summary>
        /// <param name="response">Response to process</param>
        private void ProcessInitResponse(PokerBaaziInitResponse response)
        {
            if (!roomsInitResponses.ContainsKey(response.RoomId))
            {
                roomsInitResponses.Add(response.RoomId, response);
                LogProvider.Log.Info(this, $"Init data of room {response.RoomId} has been stored. [{loggerName}]");
                return;
            }

            roomsInitResponses[response.RoomId] = response;

            LogProvider.Log.Info(this, $"Init data of room {response.RoomId} has been updated. [{loggerName}]");
        }

        /// <summary>
        /// Processes the specified <see cref="PokerBaaziTournamentDetails"/>
        /// </summary>
        /// <param name="response">Response to process</param>
        private void ProcessTournamentDetailsResponse(PokerBaaziTournamentDetailsResponse response)
        {
            if (!tournamentDetails.ContainsKey(response.TournamentId))
            {
                tournamentDetails.Add(response.TournamentId, response);
                LogProvider.Log.Info(this, $"Tournament data of tournament #{response.TournamentId} '{response.Details?.TournamentName}' has been stored. [{loggerName}]");
                return;
            }

            tournamentDetails[response.TournamentId] = response;

            LogProvider.Log.Info(this, $"Tournament data of tournament #{response.TournamentId} '{response.Details?.TournamentName}' has been updated. [{loggerName}]");
        }

        /// <summary>
        /// Processes the specified <see cref="PokerBaaziStartGameResponse"/>
        /// </summary>
        /// <param name="startGameResponse">Response to process</param>
        /// <param name="handHistory">Hand history</param>
        private void ProcessSpectatorResponse(PokerBaaziStartGameResponse startGameResponse, long timestamp, HandHistory handHistory)
        {
            if (!roomsInitResponses.TryGetValue(startGameResponse.RoomId, out PokerBaaziInitResponse initResponse))
            {
                throw new HandBuilderException(startGameResponse.HandId, $"InitResponse has not been found for room #{startGameResponse.RoomId}");
            }

            var isTournament = startGameResponse.TournamentId != 0;

            handHistory.HandId = startGameResponse.HandId;
            handHistory.DateOfHandUtc = DateTimeHelper.UnixTimeInMilisecondsToDateTime(timestamp);

            handHistory.TableName = isTournament && !string.IsNullOrEmpty(initResponse.TournamentTableName) ?
                initResponse.TournamentTableName : initResponse.TournamentName;

            handHistory.GameDescription = isTournament ?
                CreateTournamentGameDescriptor(initResponse, startGameResponse) : CreateCashGameDescriptor(initResponse, startGameResponse);

            handHistory.GameDescription.Identifier = startGameResponse.RoomId;

            if (startGameResponse.Players == null)
            {
                throw new HandBuilderException(startGameResponse.HandId, $"SpectatorResponse.Players isn't set for room #{startGameResponse.RoomId}");
            }

            // add players
            foreach (var playerInfo in startGameResponse.Players.Values)
            {
                if (playerInfo.PlayerId <= 0)
                {
                    continue;
                }

                var player = new Player
                {
                    PlayerName = playerInfo.PlayerName,
                    SeatNumber = playerInfo.Seat + 1,
                    StartingStack = playerInfo.Chips + playerInfo.BetAmount
                };

                if (initResponse.UserId == playerInfo.PlayerId)
                {
                    handHistory.Hero = player;
                }

                if (PokerBaaziUtils.TryParseCards(playerInfo.Cards, out Card[] holeCards))
                {
                    player.HoleCards = HoleCards.FromCards(playerInfo.PlayerName, holeCards);
                }

                handHistory.Players.Add(player);

                if (playerInfo.IsDealer)
                {
                    handHistory.DealerButtonPosition = player.SeatNumber;
                }

                if (playerInfo.BetAmount > 0)
                {
                    var actionType = playerInfo.IsSmallBlind ? HandActionType.SMALL_BLIND :
                        (playerInfo.IsBigBlind ? HandActionType.BIG_BLIND : HandActionType.POSTS);

                    handHistory.HandActions.Add(
                        new HandAction(playerInfo.PlayerName,
                            actionType,
                            playerInfo.BetAmount,
                            Street.Preflop));
                }

                if (isTournament && startGameResponse.Ante != 0)
                {
                    handHistory.HandActions.Add(
                        new HandAction(playerInfo.PlayerName,
                            HandActionType.ANTE,
                            startGameResponse.Ante,
                            Street.Preflop));

                    player.StartingStack += startGameResponse.Ante;
                }
            }

            HandHistoryUtils.SortHandActions(handHistory);
        }

        /// <summary>
        /// Creates <see cref="GameDescriptor"/> for cash games
        /// </summary>
        /// <param name="initResponse">Init response</param>
        /// <param name="response">Start game response</param>
        /// <returns>GameDescriptor</returns>
        private GameDescriptor CreateCashGameDescriptor(PokerBaaziInitResponse initResponse, PokerBaaziStartGameResponse response)
        {
            return new GameDescriptor(
                PokerFormat.CashGame,
                PokerSite,
                ParseGameType(initResponse),
                Limit.FromSmallBlindBigBlind(initResponse.SmallBlind, initResponse.BigBlind, currency),
                TableType.FromTableTypeDescriptions(TableTypeDescription.Regular),
                SeatType.FromMaxPlayers(initResponse.MaxPlayers),
                null);
        }

        /// <summary>
        /// Creates <see cref="GameDescriptor"/> for tournaments games
        /// </summary>
        /// <param name="initResponse">Init response</param>
        /// <param name="startGameResponse">Start game response</param>
        /// <returns>GameDescriptor</returns>
        private GameDescriptor CreateTournamentGameDescriptor(PokerBaaziInitResponse initResponse, PokerBaaziStartGameResponse startGameResponse)
        {
            if (!tournamentDetails.TryGetValue(startGameResponse.TournamentId, out PokerBaaziTournamentDetailsResponse tournamentDetailsResponse))
            {
                throw new HandBuilderException(startGameResponse.HandId, $"TournamentDetailsResponse has not been found for tournament #{startGameResponse.TournamentId}");
            }

            if (tournamentDetailsResponse.Details == null)
            {
                throw new HandBuilderException(startGameResponse.HandId, $"TournamentDetailsResponse.Details are empty for tournament #{startGameResponse.TournamentId}");
            }

            var tournamentDescriptor = new TournamentDescriptor
            {
                StartDate = tournamentDetailsResponse.Details.TournamentStartDate,
                TournamentName = tournamentDetailsResponse.Details.TournamentName,
                StartingStack = (int)tournamentDetailsResponse.Details.StartingStake,
                TotalPlayers = tournamentDetailsResponse.Details.TotalPlayers,
                TournamentsTags = tournamentDetailsResponse.Details.MaxEntries > 9 ? TournamentsTags.MTT : TournamentsTags.STT,
                Speed = TournamentSpeed.Regular,
                TournamentId = tournamentDetailsResponse.TournamentId.ToString(),
                BuyIn = Buyin.FromBuyinRake(tournamentDetailsResponse.Details.BuyIn, tournamentDetailsResponse.Details.EntryFee, currency)
            };

            return new GameDescriptor(
                PokerFormat.Tournament,
                PokerSite,
                ParseGameType(initResponse),
                Limit.FromSmallBlindBigBlind(startGameResponse.SmallBlind, startGameResponse.BigBlind, Currency.Chips, true, startGameResponse.Ante),
                TableType.FromTableTypeDescriptions(TableTypeDescription.Regular),
                SeatType.FromMaxPlayers(initResponse.MaxPlayers),
                tournamentDescriptor);
        }

        /// <summary>
        /// Processes the specified <see cref="PokerBaaziUserActionResponse"/>
        /// </summary>
        /// <param name="response">Response to process</param>
        /// <param name="handHistory">Hand history</param>
        private void ProcessUserActionResponse(PokerBaaziUserActionResponse response, HandHistory handHistory)
        {
            var street = handHistory.CommunityCards.Street;

            var actionType = ParseHandActionType(response.Action);

            var action = new HandAction(response.PlayerName,
                   actionType,
                   response.Amount,
                   street);

            handHistory.HandActions.Add(action);
        }

        /// <summary>
        /// Processes the specified <see cref="PokerBaaziUserActionResponse"/>
        /// </summary>
        /// <param name="response">Response to process</param>
        /// <param name="handHistory">Hand history</param>
        private void ProcessRoundResponse(PokerBaaziRoundResponse response, HandHistory handHistory)
        {
            if (!PokerBaaziUtils.TryParseCards(response.CommunitryCards, out Card[] cards))
            {
                throw new HandBuilderException(response.HandId, $"Failed to parse community cards: {response.CommunitryCards}");
            }

            handHistory.CommunityCards = BoardCards.FromCards(cards);
        }

        /// <summary>
        /// Processes the specified <see cref="PokerBaaziWinnerResponse"/>
        /// </summary>
        /// <param name="response">Response to process</param>
        /// <param name="handHistory">Hand history</param>
        private void ProcessWinnerResponse(PokerBaaziWinnerResponse response, HandHistory handHistory)
        {
            if (response.Winners == null)
            {
                throw new HandBuilderException(response.HandId, $"Failed to read winners from winner response");
            }

            foreach (var winnerInfo in response.Winners.Values)
            {
                var player = handHistory.Players.FirstOrDefault(x => x.PlayerName == winnerInfo.PlayerName);

                if (player == null)
                {
                    LogProvider.Log.Warn(this, $"Player {winnerInfo.PlayerName} from winner response not found in hand history players [{loggerName}]");
                }

                if (player != null && PokerBaaziUtils.TryParseCards(winnerInfo.HoleCards, out Card[] holeCards))
                {
                    player.HoleCards = HoleCards.FromCards(player.PlayerName, holeCards);
                }

                if (winnerInfo.WinAmount > 0)
                {
                    handHistory.HandActions.Add(
                        new WinningsAction(
                            winnerInfo.PlayerName,
                            HandActionType.WINS,
                            winnerInfo.WinAmount, 0));

                    player.Win = winnerInfo.WinAmount;
                }
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
            HandHistoryUtils.CalculateUncalledBets(handHistory, false);
            HandHistoryUtils.CalculateTotalPot(handHistory);
            HandHistoryUtils.RemoveSittingOutPlayers(handHistory);

            var totalWin = handHistory.WinningActions.Sum(x => x.Amount);

            var rake = handHistory.TotalPot - totalWin;

            if (rake > 0)
            {
                handHistory.Rake = rake;
            }
        }

        /// <summary>
        /// Parses <see cref="PokerBaaziPackage"/> into response object, then performs the specified action on that response object
        /// </summary>
        /// <typeparam name="T">Type of response object</typeparam>
        /// <param name="package">Package to parse</param>
        /// <param name="action">Action to perform on response object</param>
        private void ParsePackage<T>(PokerBaaziPackage package, Action<T, long> action) where T : class
        {
            PokerBaaziResponse<T> response;

            try
            {
                response = JsonConvert.DeserializeObject<PokerBaaziResponse<T>>(package.JsonData);
            }
            catch
            {
                throw new DHInternalException(new NonLocalizableString($"Failed to deserialize package of {package.PackageType} type."));
            }

            action?.Invoke(response.ClassObj, response.Timestamp);
        }

        /// <summary>
        /// Parses <see cref="PokerBaaziPackage"/> into response object, then performs the specified action on that response object
        /// </summary>
        /// <typeparam name="T">Type of response object</typeparam>
        /// <param name="package">Package to parse</param>
        /// <param name="action">Action to perform on response object</param>
        private void ParsePackage<T>(PokerBaaziPackage package, Action<T> action) where T : class
        {
            ParsePackage<T>(package, (x, t) => action?.Invoke(x));
        }

        /// <summary>
        /// Parses hand action type from the specified string
        /// </summary>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private static HandActionType ParseHandActionType(string actionType)
        {
            switch (actionType)
            {
                case "Raise":
                    return HandActionType.RAISE;
                case "Bet":
                    return HandActionType.BET;
                case "Fold":
                    return HandActionType.FOLD;
                case "Call":
                    return HandActionType.CALL;
                case "Check":
                    return HandActionType.CHECK;
                case "AllIn":
                    return HandActionType.ALL_IN;
                default:
                    throw new HandBuilderException($"Unknown action type: {actionType}");
            }
        }

        /// <summary>
        /// Parses game type from the specified <see cref="PokerBaaziInitResponse"/>
        /// </summary>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private GameType ParseGameType(PokerBaaziInitResponse initResponse)
        {
            if (string.IsNullOrEmpty(initResponse.GameType) || string.IsNullOrEmpty(initResponse.GameLimit))
            {
                LogProvider.Log.Warn(this, $"Empty game type/limit found in response '{initResponse.GameType}' '{initResponse.GameLimit}'. Use default type. [{loggerName}]");
                return GameType.NoLimitHoldem;
            }

            if (initResponse.GameType.ContainsIgnoreCase("Hold'em") &&
                initResponse.GameLimit.ContainsIgnoreCase("no limit"))
            {
                return GameType.NoLimitHoldem;
            }

            if (initResponse.GameType.ContainsIgnoreCase("Omaha") &&
               initResponse.GameLimit.ContainsIgnoreCase("pot limit"))
            {
                return GameType.PotLimitOmaha;
            }

            LogProvider.Log.Warn(this, $"Unknown game type/limit parsed from response '{initResponse.GameType}' '{initResponse.GameLimit}'. Use default type. [{loggerName}]");

            return GameType.NoLimitHoldem;
        }
    }
}