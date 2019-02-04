//-----------------------------------------------------------------------
// <copyright file="HandHistoryToIPokerConverter.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Linq;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using Model.Export;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal class HandHistoryToIPokerConverter : IHandHistoryToIPokerConverter
    {
        private static readonly Dictionary<HandActionType, ActionType> ActionMapping = new Dictionary<HandActionType, ActionType>
        {
            { HandActionType.ANTE, ActionType.Ante },
            { HandActionType.SMALL_BLIND, ActionType.SB },
            { HandActionType.BIG_BLIND, ActionType.BB },
            { HandActionType.POSTS, ActionType.BB },
            { HandActionType.FOLD, ActionType.Fold },
            { HandActionType.CHECK, ActionType.Check },
            { HandActionType.CALL, ActionType.Call },
            { HandActionType.BET, ActionType.Bet },
            { HandActionType.RAISE, ActionType.RaiseTo },
            { HandActionType.ALL_IN, ActionType.AllIn },
            { HandActionType.STRADDLE, ActionType.RaiseTo }
        };

        private static readonly HashSet<Street> ValidStreets = new HashSet<Street>
        {
            Street.Preflop,
            Street.Flop,
            Street.Turn,
            Street.River,
        };

        private static readonly HashSet<HandActionType> InitActions = new HashSet<HandActionType>
        {
            HandActionType.ANTE,
            HandActionType.SMALL_BLIND,
            HandActionType.BIG_BLIND,
            HandActionType.POSTS,
        };

        private static readonly Dictionary<Street, CardsType> StreetCardsTypeMapping = new Dictionary<Street, CardsType>
        {
            { Street.Flop, CardsType.Flop },
            { Street.Turn, CardsType.Turn },
            { Street.River, CardsType.River },
        };

        private Currency GetCurrency(HandHistories.Objects.Hand.HandHistory history)
        {
            return Currency.USD;
        }

        private string GetBuyInString(HandHistories.Objects.Hand.HandHistory history)
        {
            // Most of this code was taken from HandHistories.Objects.GameDescription.Buyin.ToString()
            // because it's not possible to replace currency symbol using just ToString() method
            var currency = history.GameDescription.Tournament.BuyIn.GetCurrencySymbol();
            var separator = " + ";

            var format = CultureInfo.InvariantCulture;
            var prizePoolValue = history.GameDescription.Tournament.BuyIn.PrizePoolValue;
            var rake = history.GameDescription.Tournament.BuyIn.Rake;

            var prizePoolString = (prizePoolValue != Math.Round(prizePoolValue)) ? prizePoolValue.ToString("N2", format) : prizePoolValue.ToString("N0", format);
            var rakeString = (rake != Math.Round(rake)) ? rake.ToString("N2", format) : rake.ToString("N0", format);

            if (history.GameDescription.Tournament.BuyIn.IsKnockout)
            {
                var KnockoutValue = history.GameDescription.Tournament.BuyIn.KnockoutValue;

                string knockoutString = (KnockoutValue != Math.Round(KnockoutValue)) ? KnockoutValue.ToString("N2", format) : KnockoutValue.ToString("N0", format);

                return string.Format("{0}{1}{4}{0}{3}{4}{0}{2}", currency, prizePoolString, rakeString, knockoutString, separator);
            }

            return string.Format("{0}{1}{3}{0}{2}", currency, prizePoolString, rakeString, separator);
        }

        private string GetTotalBuyInString(HandHistories.Objects.Hand.HandHistory history)
        {
            var buyIn = history.GameDescription.Tournament.BuyIn.TotalBuyin().ToString(CultureInfo.InvariantCulture);
            var currency = history.GameDescription.Tournament.BuyIn.GetCurrencySymbol();

            return $"{currency}{buyIn}";
        }

        private string GetGameType(HandHistories.Objects.Hand.HandHistory history)
        {
            switch (history.GameDescription.GameType)
            {
                case GameType.NoLimitHoldem:
                    return "Holdem NL";
                case GameType.PotLimitOmaha:
                    return "Omaha PL";
                case GameType.PotLimitHoldem:
                    return "Holdem PL";
                case GameType.NoLimitOmahaHiLo:
                    return "Omaha Hi-Lo NL";
                case GameType.PotLimitOmahaHiLo:
                    return "Omaha Hi-Lo PL";
                default:
                    return string.Empty;
            }
        }

        private General CreateGeneral(HandHistories.Objects.Hand.HandHistory history)
        {
            var result = new General
            {
                Mode = "real",
                Chipsin = 0,
                Chipsout = 0,
                GameCount = 1,
                Awardpoints = 0,
                StatusPoints = 0,
                Ipoints = 0,
                IsAsian = "0",
                StartDate = history.DateOfHandUtc,
                Duration = "N/A",
                TableName = history.TableName,
                Nickname = history.Hero != null ? history.Hero.PlayerName :
                   !string.IsNullOrEmpty(history.HeroName) ? history.HeroName : string.Empty,
                GameType = GetGameType(history)           
            };

            if (history.GameDescription.IsTournament)
            {
                history.GameDescription.Tournament.BuyIn.Currency = GetCurrency(history);
                result.TournamentName = history.GameDescription.Tournament.TournamentName;
                result.TournamentCurrency = history.GameDescription.Tournament.BuyIn.Currency.ToString();
                result.Currency = history.GameDescription.Tournament.BuyIn.Currency;
                result.BuyIn = GetBuyInString(history);           // These two functions add $ currency symbol to buyins with empty currency (Gold, Club Chips, etc.)
                result.TotalBuyIn = GetTotalBuyInString(history); // Otherwise HM2 will consider it's euros and convert to $ anyway
                // For some reason HM2 doesn't want to import tournament hand without place
                // If place is unknown we put number of players involved in the hand there
                result.Place = history.GameDescription.Tournament.FinishPosition > 0 ? history.GameDescription.Tournament.FinishPosition : history.Players.Count;
                result.Win = history.GameDescription.Tournament.TotalPrize;
            }
            else
            {
                history.GameDescription.Limit.Currency = GetCurrency(history);

                result.Currency = history.GameDescription.Limit.Currency;

                var blinds = history.GameDescription.Limit.ToString(CultureInfo.InvariantCulture, false, true, "/");
                result.GameType = $"{result.GameType} {blinds}";
                result.Bets = 0;
                result.Wins = 0;
            }

            return result;
        }

        private List<Player> CreateGameGeneralPlayers(HandHistories.Objects.Hand.HandHistory history)
        {
            var result = new List<Player>();

            seatMap.TryGetValue(history.GameDescription.SeatType.MaxPlayers, out Dictionary<int, int> seats);

            foreach (var player in history.Players)
            {
                var uncalledBet = history.HandActions
                    .Where(a => a.PlayerName == player.PlayerName && a.HandActionType == HandActionType.UNCALLED_BET)
                    .Select(a => a.Amount)
                    .DefaultIfEmpty(0)
                    .Sum();

                result.Add(
                    new Player
                    {
                        Name = player.PlayerName,
                        Chips = player.StartingStack,
                        Bet = player.Bet,
                        Win = player.Win + uncalledBet,
                        Seat = seats != null && seats.ContainsKey(player.SeatNumber) ? seats[player.SeatNumber] : player.SeatNumber,
                        Dealer = history.DealerButtonPosition == player.SeatNumber,
                    }
                );
            }

            return result;
        }

        private GameGeneral CreateGameGeneral(HandHistories.Objects.Hand.HandHistory history)
        {
            var result = new GameGeneral
            {
                StartDate = history.DateOfHandUtc,
                Players = CreateGameGeneralPlayers(history),
            };

            return result;
        }

        private int GetRoundNumber(HandAction action)
        {
            if (action.Street == Street.Preflop)
            {
                return InitActions.Contains(action.HandActionType) ? 0 : 1;
            }

            return (int)action.Street;
        }

        private string CardsToText(IEnumerable<HandHistories.Objects.Cards.Card> cards)
        {
            return string.Join(" ", cards.Select(c => $"{c.Suit}{c.Rank}".ToUpper()));
        }

        private string CreateGameRoundsRoundCardsTextForPlayer(HandHistories.Objects.Hand.HandHistory history, HandHistories.Objects.Players.Player player)
        {
            if (player.hasHoleCards)
            {
                return CardsToText(player.HoleCards);
            }

            int totalCards = history.GameDescription.GameType == GameType.PotLimitOmaha ? 4 : 2;
            return string.Join(" ", Enumerable.Repeat("X", totalCards));
        }

        private IEnumerable<HandHistories.Objects.Cards.Card> GetCardsDealtOnStreet(IEnumerable<HandHistories.Objects.Cards.Card> cards, Street street)
        {
            switch (street)
            {
                case Street.Flop:
                    return cards.Take(3);
                case Street.Turn:
                    return cards.Skip(3).Take(1);
                case Street.River:
                    return cards.Skip(4).Take(1);
                default:
                    return Enumerable.Empty<HandHistories.Objects.Cards.Card>();
            }
        }

        private List<Cards> CreateGameRoundsRoundCards(HandHistories.Objects.Hand.HandHistory history, Street street)
        {
            var result = new List<Cards>();

            if (street == Street.Preflop)
            {
                foreach (var player in history.Players)
                {
                    result.Add(
                        new Cards
                        {
                            Player = player.PlayerName,
                            Type = CardsType.Pocket,
                            Value = CreateGameRoundsRoundCardsTextForPlayer(history, player),
                        }
                    );
                }
            }
            else if (street >= Street.Flop && street <= Street.River)
            {
                result.Add(
                    new Cards
                    {
                        Type = StreetCardsTypeMapping[street],
                        Value = CardsToText(GetCardsDealtOnStreet(history.CommunityCards, street))
                    }
                );
            }

            return result;
        }

        private List<Action> CreateGameRoundsRoundActions(HandHistories.Objects.Hand.HandHistory history, Dictionary<int, List<HandAction>> roundActions,
            Street street, ref int currentActionNumber)
        {
            var result = new List<Action>();

            int roundIndex = (int)street;

            if (!roundActions.ContainsKey(roundIndex))
            {
                return result;
            }

            var investments = history.Players.ToDictionary(p => p.PlayerName, p => 0m);

            if (street == Street.Preflop)
            {
                var actionAmountMapping = new[]
                {
                     HandActionType.SMALL_BLIND,
                     HandActionType.BIG_BLIND,
                     HandActionType.POSTS
                };

                var players = history.Players.ToList();

                var actions = history.HandActions.Where(a => actionAmountMapping.Contains(a.HandActionType));

                actions.ForEach(action =>
                {
                    investments[action.PlayerName] = Math.Abs(action.Amount);
                });
            }

            foreach (var action in roundActions[roundIndex])
            {
                var amount = Math.Abs(action.Amount);
                investments[action.PlayerName] += amount;

                var sum = action.HandActionType == HandActionType.RAISE ? investments[action.PlayerName] : amount;

                result.Add(
                    new Action
                    {
                        No = currentActionNumber++,
                        Type = ActionMapping[action.HandActionType],
                        Player = action.PlayerName,
                        Sum = sum,
                    }
                );
            }

            return result;
        }

        private List<Round> CreateGameRounds(HandHistories.Objects.Hand.HandHistory history)
        {
            var result = new List<Round>();

            var actionGroups = history.HandActions
                .Where(a => ValidStreets.Contains(a.Street) && ActionMapping.ContainsKey(a.HandActionType))
                .GroupBy(GetRoundNumber)
                .ToDictionary(g => g.Key, g => g.ToList());

            int currentActionNumber = 0;

            for (Street street = Street.Init; street <= history.CommunityCards.Street; street++)
            {
                result.Add(
                    new Round
                    {
                        No = (int)street,
                        Cards = CreateGameRoundsRoundCards(history, street),
                        Actions = CreateGameRoundsRoundActions(history, actionGroups, street, ref currentActionNumber),
                    }
                );
            }

            return result;
        }

        private Game CreateGame(HandHistories.Objects.Hand.HandHistory history)
        {
            var result = new Game
            {
                GameCode = (ulong)history.HandId,
                General = CreateGameGeneral(history),
                Rounds = CreateGameRounds(history),
            };

            return result;
        }

        public string Convert(HandHistories.Objects.Hand.HandHistory history)
        {
            try
            {
                var target = new HandHistory
                {
                    SessionCode = history.GameDescription.Identifier.ToString(),
                    General = CreateGeneral(history),
                    Games = new List<Game> { CreateGame(history) },
                };

                var handHistoryXml = SerializationHelper.SerializeObject(target, true);

                return handHistoryXml;
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Failed to convert handhistory to IPoker format."), e);
            }
        }

        private static readonly Dictionary<int, Dictionary<int, int>> seatMap = new Dictionary<int, Dictionary<int, int>>
        {
            { 2, new Dictionary<int, int>
                 {
                    { 1, 3 },
                    { 2, 8 }
                 }
            },
            { 4, new Dictionary<int, int>
                {
                    {1, 2},
                    {2, 4},
                    {3, 7},
                    {4, 9}
                }
            },
            { 6, new Dictionary<int, int>
                 {
                    { 1, 1 },
                    { 2, 3 },
                    { 3, 5 },
                    { 4, 6 },
                    { 5, 8 },
                    { 6, 10 }
                 }
            },
            { 8, new Dictionary<int, int>
                 {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 4 },
                    { 4, 5 },
                    { 5, 6 },
                    { 6, 7 },
                    { 7, 9 },
                    { 8, 10 }
                 }
            },
            { 9, new Dictionary<int, int>
                 {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 6, 6 },
                    { 7, 8 },
                    { 8, 9 },
                    { 9, 10 },
                 }
            }
        };
    }
}