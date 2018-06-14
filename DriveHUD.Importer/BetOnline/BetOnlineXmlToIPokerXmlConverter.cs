//-----------------------------------------------------------------------
// <copyright file="BetOnlineXmlToIPokerXmlConverter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.Importers.Builders.iPoker;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using Model.Site;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace DriveHUD.Importers.BetOnline
{
    /// <summary>
    /// Class to convert BetOnline xml to iPoker xml
    /// </summary>
    internal class BetOnlineXmlToIPokerXmlConverter : IBetOnlineXmlConverter
    {
        private XElement tableDetails;
        private XElement gameState;
        private XElement changes;
        private XElement relocationData;

        private int actionNo = 1;

        private Dictionary<int, Player> playersOnTable;
        private Dictionary<int, string> showCards;
        private Dictionary<int, decimal> uncalledBets;

        private bool isTournament;

        private readonly ICardsConverter cardsConverter;
        private readonly ISiteConfiguration configuration;

        private readonly IBetOnlineTableService tableService;

        private GameInfo gameInfo;

        private int maxPlayers = 10;
        private string sessionCode = string.Empty;
        private const ImporterIdentifier Identifier = ImporterIdentifier.BetOnline;

        public BetOnlineXmlToIPokerXmlConverter()
        {
            cardsConverter = ServiceLocator.Current.GetInstance<ICardsConverter>();
            showCards = new Dictionary<int, string>();
            uncalledBets = new Dictionary<int, decimal>();

            var configurationService = ServiceLocator.Current.GetInstance<ISiteConfigurationService>();
            configuration = configurationService.Get(EnumPokerSites.BetOnline);

            tableService = ServiceLocator.Current.GetInstance<IBetOnlineTableService>();
        }

        /// <summary>
        /// Gets hand number
        /// </summary>
        public string HandNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets whenever converter is initialized
        /// </summary>
        public bool IsInitialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes converter  
        /// </summary>
        /// <param name="xml"></param>
        public void Initialize(string xml)
        {
            try
            {
                var xDocument = XDocument.Parse(xml);

                tableDetails = xDocument.GetFirstElement("TableDetails");
                gameState = xDocument.GetFirstElement("GameState");
                changes = xDocument.GetFirstElement("Changes");

                HandNumber = gameState.Attribute("hand").Value;

                isTournament = tableDetails != null &&
                    tableDetails.Attribute("type") != null &&
                    tableDetails.Attribute("type").Value.Equals("TOURNAMENT_TABLE", StringComparison.InvariantCultureIgnoreCase);

                if (!IsFullHand())
                {
                    var tableName = isTournament ? GetTournamentTableName() : GetTableName();

                    LogProvider.Log.Info(this, string.Format("Hand {0} from '{1}' has been skipped, because it isn't full. [{2}]", HandNumber, tableName, Identifier));
                    return;
                }

                IsInitialized = true;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Converter has not been initialized. Data could not be parsed", e);
            }
        }

        /// <summary>
        /// Adds relocation data to converter
        /// </summary>
        /// <param name="relocationData"><see cref="XElement"/> of relocation data </param>
        public void AddRelocationData(XElement relocationData)
        {
            this.relocationData = relocationData;
        }

        /// <summary>
        /// Converts betonline hand history xml to iPoker xml
        /// </summary>        
        /// <returns>The result of conversion</returns>
        public ConvertedResult Convert()
        {
            if (!IsInitialized)
            {
                return null;
            }

            try
            {
                gameInfo = new GameInfo
                {
                    PokerSite = EnumPokerSites.BetOnline
                };

                sessionCode = BuildSessionCode();

                var handHistory = BuildHandHistory();

                var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
                var siteSettings = settings.SiteSettings.SitesModelList?.FirstOrDefault(x => x.PokerSite == gameInfo.PokerSite);

                if (siteSettings != null && !siteSettings.Enabled)
                {
                    return null;
                }

                AdjustHandHistory(handHistory);

                var convertedXml = SerializationHelper.SerializeObject(handHistory);

                var result = new ConvertedResult
                {
                    ConvertedXml = convertedXml,
                    TableName = isTournament ? string.Format("{0}, {1}", GetTableName(), GetTournamentName()) : handHistory.General.TableName,
                    TableId = tableDetails.Attribute("id").Value,
                    HandNumber = HandNumber,
                    Players = playersOnTable.Values.ToList(),
                    GameInfo = gameInfo
                };

                return result;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Data could not be parsed. [{Identifier}]", e);
                return null;
            }
        }

        /// <summary>
        /// Set winnings, bets and cards
        /// </summary>
        /// <param name="handHistory">Hand history</param>
        private void AdjustHandHistory(HandHistory handHistory)
        {
            // cards
            var game = handHistory.Games.FirstOrDefault();
            var preflop = game.Rounds.FirstOrDefault(x => x.No == 1);

            foreach (var card in preflop.Cards)
            {
                if (showCards.ContainsKey(card.Seat))
                {
                    card.Value = showCards[card.Seat];
                }
            }

            // winning
            foreach (XElement winner in changes.Descendants("Winner"))
            {
                var seat = int.Parse(winner.Attribute("seat").Value);
                var win = decimal.Parse(winner.Attribute("amount").Value, NumberStyles.Currency, CultureInfo.InvariantCulture);

                if (playersOnTable.ContainsKey(seat))
                {
                    playersOnTable[seat].Win += win;
                }
            }

            // uncalled bets
            foreach (var uncalledBet in uncalledBets)
            {
                if (playersOnTable.ContainsKey(uncalledBet.Key))
                {
                    playersOnTable[uncalledBet.Key].Win += uncalledBet.Value;
                }
            }

            // bets 
            var actionsBySeat = game.Rounds.SelectMany(x => x.Actions).GroupBy(x => x.SeatNumber)
                .Select(x => new { Seat = x.Key, Actions = x.ToArray() })
                .ToArray();

            foreach (var actions in actionsBySeat)
            {
                var bet = actions.Actions.Sum(x => x.Sum);
                playersOnTable[actions.Seat].Bet = bet;
            }

            // in case of 4-max table
            if (maxPlayers == 4)
            {
                foreach (var player in game.General.Players)
                {
                    player.Seat = player.Seat * 2;
                }

                RelocateSeats(game.General.Players, 8);
            }
            else
            {
                RelocateSeats(game.General.Players, maxPlayers);
            }

            // seats and players
            foreach (var player in game.General.Players)
            {
                if (player.IsHero)
                {
                    handHistory.General.Nickname = player.Name;
                }

                player.Seat++;
            }

            // game type
            if (!isTournament)
            {
                var parameters = tableDetails.GetFirstElement("Parameters");

                var smallBlind = parameters.Attribute("stakes-low").Value;
                var bigBlind = parameters.Attribute("stakes-high").Value;

                handHistory.General.GameType = string.Format(CultureInfo.InvariantCulture, "{0} ${1}/${2}", handHistory.General.GameType, smallBlind, bigBlind);
            }

            AdjustRaises(game.Rounds);
        }

        /// <summary>
        /// Check if hand is full or partial
        /// </summary>
        /// <returns>Returns true if hand is full</returns>
        private bool IsFullHand()
        {
            var hasNewHand = false;
            var hasEndHand = false;

            foreach (XElement element in changes.Nodes())
            {
                if (element.Name.LocalName.Equals("NewHand"))
                {
                    hasNewHand = true;
                }

                if (element.Name.LocalName.Equals("EndHand"))
                {
                    hasEndHand = true;
                }
            }

            return hasNewHand && hasEndHand;
        }

        private string BuildSessionCode()
        {
            var sessionIdentifier = string.Empty;

            if (isTournament)
            {
                var entryIdNode = gameState.Attribute("historyId");

                if (entryIdNode != null)
                {
                    sessionIdentifier = entryIdNode.Value.Length < 17 ?
                                            entryIdNode.Value :
                                            entryIdNode.Value.Substring(entryIdNode.Value.Length - 16);
                }
            }

            if (string.IsNullOrEmpty(sessionIdentifier))
            {
                sessionIdentifier = tableDetails.Attribute("id").Value;
            }

            var identifierCleaned = sessionIdentifier.Replace("-", string.Empty);

            long sessionCode;

            if (long.TryParse(identifierCleaned, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out sessionCode))
            {
                return sessionCode.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Builds hand history object based on base xml 
        /// </summary>
        /// <returns>Hand history object</returns>
        private HandHistory BuildHandHistory()
        {
            var general = BuildGeneral();
            var games = new List<Game> { BuildGame() };

            general.MaxPlayers = maxPlayers;

            var handNumber = games[0].GameCode;

            EnumPokerSites site;

            var windowHandle = tableService.GetWindowHandle(handNumber, out site);

            if (string.IsNullOrEmpty(sessionCode))
            {
                sessionCode = windowHandle.ToString();
            }

            gameInfo.Session = sessionCode;
            gameInfo.WindowHandle = windowHandle;
            gameInfo.GameNumber = (long)handNumber;

            if (site != EnumPokerSites.Unknown)
            {
                gameInfo.PokerSite = site;
            }

            general.Site = gameInfo.PokerSite;

            var handHistory = new HandHistory
            {
                SessionCode = sessionCode,
                General = general,
                Games = games
            };

            return handHistory;
        }

        /// <summary>
        /// Build general hand history object based on base xml
        /// </summary>
        /// <returns>General hand history object</returns>
        private General BuildGeneral()
        {
            var buyIn = PokerConfiguration.DefaultBuyIn;
            var totalBuyIn = PokerConfiguration.DefaultBuyIn;

            var general = new General
            {
                Mode = PokerConfiguration.DefaultMode,
                GameType = BuildGameType(),
                TableName = isTournament ? GetTournamentTableName() : GetTableName(),
                StartDate = GetTableTime(),
                Duration = PokerConfiguration.DefaultDuration,
                Nickname = configuration.HeroName,
                BuyIn = buyIn,
                Currency = Currency.USD,
                TotalBuyIn = totalBuyIn
            };

            if (isTournament)
            {
                var tournamentName = GetTournamentName();

                var tournamentTableNode = tableDetails.Descendants("TournamentTable").First();

                if (tournamentTableNode.Attribute("buyIn") != null && tournamentTableNode.Attribute("fee") != null)
                {
                    var feeText = tournamentTableNode.Attribute("fee").Value;
                    var buyInValue = tournamentTableNode.Attribute("buyIn").Value;

                    decimal buyInNumber = 0m;
                    decimal.TryParse(buyInValue, NumberStyles.Currency, CultureInfo.InvariantCulture, out buyInNumber);

                    decimal fee = 0m;
                    decimal.TryParse(feeText, NumberStyles.Currency, CultureInfo.InvariantCulture, out fee);

                    var prizeFee = buyInNumber - fee;

                    buyIn = string.Format(CultureInfo.InvariantCulture, PokerConfiguration.BuyInFormat, prizeFee, fee);
                    totalBuyIn = string.Format(CultureInfo.InvariantCulture, PokerConfiguration.TotalBuyInFormat, buyInNumber);
                }
                else if (!string.IsNullOrEmpty(tournamentName))
                {
                    var tournamentCacheService = ServiceLocator.Current.GetInstance<ITournamentsCacheService>();
                    var tournamentInfo = tournamentCacheService.GetTournamentInfo(tournamentName);

                    if (tournamentInfo != null)
                    {
                        var prizeFee = tournamentInfo.Attribute("prizeFee") != null ? tournamentInfo.Attribute("prizeFee").Value : PokerConfiguration.DefaultBuyIn;
                        var fee = tournamentInfo.Attribute("fee") != null ? tournamentInfo.Attribute("fee").Value : PokerConfiguration.DefaultBuyIn;
                        var buyInValue = tournamentInfo.Attribute("buyIn") != null ? tournamentInfo.Attribute("buyIn").Value : PokerConfiguration.DefaultBuyIn;

                        decimal buyInNumber = 0m;

                        // only numbers could be here
                        if (!decimal.TryParse(buyInValue, NumberStyles.Currency, CultureInfo.InvariantCulture, out buyInNumber))
                        {
                            buyInValue = PokerConfiguration.DefaultBuyIn;
                        }

                        buyIn = string.Format(CultureInfo.InvariantCulture, PokerConfiguration.BuyInFormat, prizeFee, fee);
                        totalBuyIn = string.Format(CultureInfo.InvariantCulture, PokerConfiguration.TotalBuyInFormat, buyInValue);
                    }
                }

                general.TournamentName = tournamentName;
                general.BuyIn = buyIn;
                general.TotalBuyIn = totalBuyIn;

                var prizeInfoNode = gameState.GetFirstElementOrDefault("PrizeInfo");

                if (prizeInfoNode != null && prizeInfoNode.Attribute("prizePool") != null)
                {
                    general.TotalPrizePool = string.Format(CultureInfo.InvariantCulture, PokerConfiguration.TotalBuyInFormat, prizeInfoNode.Attribute("prizePool").Value);
                }
            }

            return general;
        }

        /// <summary>
        /// Build game hand history object based on base xml
        /// </summary>
        /// <returns>Game hand history object</returns>
        private Game BuildGame()
        {
            ulong handNumber;

            if (!ulong.TryParse(gameState.Attribute("hand").Value, out handNumber))
            {
                throw new InvalidOperationException(string.Format("Hand number {0} could not be parsed. [{1}]", gameState.Attribute("hand").Value, Identifier));
            }

            var game = new Game
            {
                GameCode = handNumber,
                General = BuildGameGeneral(),
                Rounds = BuildRounds()
            };

            return game;
        }

        /// <summary>
        /// Build game general hand history object based on base xml
        /// </summary>
        /// <returns>Game hand history object</returns>
        private GameGeneral BuildGameGeneral()
        {
            var startDateText = gameState.Attribute("serverTime").Value;

            long startDateInUnixFormat;

            if (!long.TryParse(startDateText, out startDateInUnixFormat))
            {
                throw new InvalidOperationException(string.Format("Hand time {0} could not be parsed. [{1}]", startDateText, Identifier));
            }

            var startDate = DateTimeHelper.UnixTimeInMilisecondsToDateTime(startDateInUnixFormat);

            var players = BuildPlayers();

            var gameGeneral = new GameGeneral
            {
                StartDate = GetCorrectedTime(startDate),
                Players = players
            };

            return gameGeneral;
        }

        /// <summary>
        /// Build game hand history object based on base xml
        /// </summary>
        /// <returns>Game hand history object</returns>
        private List<Round> BuildRounds()
        {
            var rounds = new List<Round>();
            var roundNumber = 0;

            var round = new Round
            {
                No = roundNumber++,
                Actions = new List<Builders.iPoker.Action>()
            };

            rounds.Add(round);

            foreach (XElement changeElement in changes.Nodes())
            {
                // start preflop
                if (changeElement.Name.LocalName.Equals("DealingCards"))
                {
                    var cards = (from seatElement in changeElement.Descendants("Seat")
                                 let seat = int.Parse(seatElement.Attribute("id").Value)
                                 let cardsText = string.Join(" ", seatElement.Descendants("Card").Select(x => x.Value).ToArray())
                                 orderby seat
                                 select new Cards
                                 {
                                     Player = playersOnTable[seat].Name,
                                     Seat = seat,
                                     Type = CardsType.Pocket,
                                     Value = cardsConverter.Convert(cardsText).Replace("XX", PokerConfiguration.UnknownCard)
                                 }).ToList();

                    round = new Round
                    {
                        No = roundNumber++,
                        Cards = cards,
                        Actions = new List<Builders.iPoker.Action>()
                    };

                    rounds.Add(round);

                    continue;
                }

                // start flop
                if (changeElement.Name.LocalName.Equals("DealingFlop"))
                {
                    round = new Round
                    {
                        No = roundNumber++,
                        Cards = new List<Cards>
                        {
                            GetRoundCards(changeElement, CardsType.Flop)
                        },
                        Actions = new List<Builders.iPoker.Action>()
                    };

                    rounds.Add(round);

                    continue;

                }

                // start turn
                if (changeElement.Name.LocalName.Equals("DealingTurn"))
                {
                    round = new Round
                    {
                        No = roundNumber++,
                        Cards = new List<Cards>
                        {
                            GetRoundCards(changeElement, CardsType.Turn)
                        },
                        Actions = new List<Builders.iPoker.Action>()
                    };

                    rounds.Add(round);

                    continue;
                }

                // start river
                if (changeElement.Name.LocalName.Equals("DealingRiver"))
                {
                    round = new Round
                    {
                        No = roundNumber++,
                        Cards = new List<Cards>
                        {
                            GetRoundCards(changeElement, CardsType.River)
                        },
                        Actions = new List<Builders.iPoker.Action>()
                    };

                    rounds.Add(round);

                    continue;
                }

                var action = ConvertElementToAction(changeElement);

                if (action == null)
                {
                    continue;
                }

                round.Actions.Add(action);
            }

            return rounds;
        }

        /// <summary>
        /// Builds players 
        /// </summary>
        /// <returns></returns>
        private List<Player> BuildPlayers()
        {
            var seatsNode = gameState.GetFirstElement("Seats");

            maxPlayers = int.Parse(seatsNode.Attribute("number").Value);

            gameInfo.TableType = maxPlayers == 4 ? EnumTableType.Eight : (EnumTableType)maxPlayers;

            var dealer = int.Parse(seatsNode.Attribute("dealer").Value);

            var heroSeat = seatsNode.Attribute("me") != null ? int.Parse(seatsNode.Attribute("me").Value) : -1;

            var players = (from seatNode in seatsNode.Descendants("Seat")
                           join activeSeat in changes.GetFirstElement("ActiveSeats").Descendants("Seat") on seatNode.Attribute("id").Value equals activeSeat.Attribute("id").Value into gj
                           let seat = int.Parse(seatNode.Attribute("id").Value)
                           where seatNode.HasElements
                           select new Player
                           {
                               Seat = seat,
                               Name = seatNode.GetFirstElement("PlayerInfo").Attribute("nickname") != null ?
                                          RemoveIllegalCharacters(seatNode.GetFirstElement("PlayerInfo").Attribute("nickname").Value) :
                                          tableService.GetRandomPlayerName(sessionCode, seat + 1),
                               Chips = decimal.Parse(seatNode.GetFirstElement("Chips").Attribute("stack-size").Value, NumberStyles.Currency, CultureInfo.InvariantCulture),
                               Dealer = seat == dealer,
                               IsHero = heroSeat == seat
                           }).ToList();

            playersOnTable = players.ToDictionary(x => x.Seat);

            return players;
        }

        /// <summary>
        /// Get table name based from base xml depends on type of game
        /// </summary>
        /// <returns>Table name</returns>
        private string GetTableName()
        {
            return tableDetails.Attribute("name").Value;
        }

        /// <summary>
        /// Get tournament table name based from base xml depends on type of game
        /// </summary>
        /// <returns>Table name</returns>
        private string GetTournamentTableName()
        {
            return string.Format("{0}, {1}", GetTableName(), GetTournamentName());
        }

        /// <summary>
        /// Get table name based from base xml depends on type of game
        /// </summary>
        /// <returns>Table name</returns>
        private string GetTournamentName()
        {
            return isTournament ? tableDetails.Descendants("TournamentTable").First().Attribute("tournamentName").Value : null;
        }

        /// <summary>
        /// Remove illegal characters
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        private string RemoveIllegalCharacters(string playerName)
        {
            return playerName.Replace("|", "-").Trim();
        }

        /// <summary>
        /// Build game type in iPoker format
        /// </summary>
        /// <returns>Game type in iPoker format</returns>
        private string BuildGameType()
        {
            var tableNode = isTournament ?
                                tableDetails.Descendants("TournamentTable").First() :
                                tableDetails.Descendants("SingleTable").First();

            var gameType = GetGameType(tableNode.Attribute("game").Value);

            var gameLimit = GetGameLimit(tableNode.Attribute("limit").Value);

            gameInfo.GameType = GetGameInfoGameType(tableNode.Attribute("game").Value);
            gameInfo.GameFormat = GetGameFormat();

            var result = string.Format("{0} {1}", gameType, gameLimit);

            return result;
        }

        private string GetGameType(string text)
        {
            var gameType = string.Empty;

            switch (text)
            {
                case "TEXAS_HOLDEM":
                    gameType = "Holdem";
                    break;
                case "OMAHA":
                    gameType = "Omaha";
                    break;
                case "OMAHA_HIGH_LOW":
                    gameType = "Omaha Hi-Lo";
                    break;
                default:
                    LogProvider.Log.Warn(string.Format("Detected unknown game type: {0}", gameType));
                    gameType = "Holdem";
                    break;
            }

            return gameType;
        }

        private Bovada.GameType GetGameInfoGameType(string text)
        {
            switch (text)
            {
                case "TEXAS_HOLDEM":
                    return Bovada.GameType.Holdem;
                case "OMAHA":
                    return Bovada.GameType.Omaha;
                case "OMAHA_HIGH_LOW":
                    return Bovada.GameType.OmahaHiLo;
                default:
                    return Bovada.GameType.Holdem;
            }
        }

        private static string GetGameLimit(string text)
        {
            var gameLimit = string.Empty;

            switch (text)
            {
                case "NO_LIMIT":
                    gameLimit = "NL";
                    break;
                case "POT_LIMIT":
                    gameLimit = "PL";
                    break;
                default:
                    LogProvider.Log.Warn(string.Format("Detected unknown limit type: {0}", gameLimit));
                    gameLimit = "Limit";
                    break;
            }

            return gameLimit;
        }

        private GameFormat GetGameFormat()
        {
            if (!isTournament)
            {
                return GameFormat.Cash;
            }

            var tournamentType = tableDetails.GetFirstElement("TournamentTable").Attribute("type").Value;

            if (tournamentType.Equals("SITANDGO_TOURNAMENT", StringComparison.InvariantCultureIgnoreCase) ||
                tournamentType.Equals("WINDFALL_TOURNAMENT", StringComparison.InvariantCultureIgnoreCase))
            {
                return GameFormat.SnG;
            }

            return GameFormat.MTT;
        }

        private DateTime GetTableTime()
        {
            var tableTimeUnix = long.Parse(gameState.Attribute("serverTime").Value);

            var tableTime = DateTimeHelper.UnixTimeInMilisecondsToDateTime(tableTimeUnix);

            return GetCorrectedTime(tableTime);
        }

        private DateTime GetCorrectedTime(DateTime utcDateTime)
        {
            var timeZoneOffset = configuration.TimeZoneOffset;

            if (timeZoneOffset != null)
            {
                utcDateTime = utcDateTime.Add(timeZoneOffset);
            }

            return utcDateTime;
        }

        private Builders.iPoker.Action ConvertElementToAction(XElement element)
        {
            if (!element.Name.LocalName.Equals("PlayerAction"))
            {
                return null;
            }

            var actionNode = element.FirstNode as XElement;

            if (actionNode == null)
            {
                return null;
            }

            var seat = int.Parse(element.Attribute("seat").Value);

            var actionText = actionNode.Name.LocalName;

            // remember cards
            if (actionText.Equals("Show") || (actionText.Equals("Muck") && actionNode.HasElements))
            {
                var cardsText = string.Join(" ", element.Descendants("Card").Select(x => x.Value).ToArray());

                if (!string.IsNullOrEmpty(cardsText))
                {
                    var cards = cardsConverter.Convert(cardsText);

                    if (!showCards.ContainsKey(seat))
                    {
                        showCards.Add(seat, cards);
                    }
                }
            }

            // handle uncalled bets
            if (actionText.Equals("UncalledBet"))
            {
                var uncalledBet = decimal.Parse(actionNode.Attribute("amount").Value, NumberStyles.Currency, CultureInfo.InvariantCulture);

                if (!uncalledBets.ContainsKey(seat))
                {
                    uncalledBets.Add(seat, uncalledBet);
                }
                else
                {
                    uncalledBets[seat] += uncalledBet;
                }
            }

            var actionType = ConvertToActionType(actionText);

            if (actionType == ActionType.Undefined)
            {
                return null;
            }

            if (!playersOnTable.ContainsKey(seat))
            {
                LogProvider.Log.Error(string.Format("Could not get player on seat {0}. [{1}]", seat, Identifier));
            }

            var sum = 0m;

            if (actionNode.Attribute("amount") != null)
            {
                sum = decimal.Parse(actionNode.Attribute("amount").Value, NumberStyles.Currency, CultureInfo.InvariantCulture);
            }

            if (actionNode.Attribute("dead") != null)
            {
                sum += decimal.Parse(actionNode.Attribute("dead").Value, NumberStyles.Currency, CultureInfo.InvariantCulture);
            }

            var action = new Builders.iPoker.Action
            {
                No = actionNo++,
                Type = actionType,
                SeatNumber = seat,
                Player = playersOnTable[seat].Name,
                Sum = sum
            };

            return action;
        }

        private ActionType ConvertToActionType(string actionText)
        {
            switch (actionText)
            {
                case "Call":
                    return ActionType.Call;
                case "Check":
                    return ActionType.Check;
                case "Fold":
                    return ActionType.Fold;
                case "PostAnte":
                    return ActionType.Ante;
                case "PostSmallBlind":
                    return ActionType.SB;
                case "PostBigBlind":
                    return ActionType.BB;
                case "Raise":
                    return ActionType.RaiseTo;
                case "Bet":
                    return ActionType.Bet;
                default:
                    return ActionType.Undefined;
            }
        }

        private Cards GetRoundCards(XElement roundNode, CardsType cardsType)
        {
            var cardsText = string.Join(" ", roundNode.Descendants("Card").Select(x => x.Value).ToArray());

            var cards = new Cards
            {
                Type = cardsType,
                Player = string.Empty,
                Value = cardsConverter.Convert(cardsText)
            };

            return cards;
        }

        private void AdjustRaises(IEnumerable<Round> rounds)
        {
            var zeroRound = rounds.FirstOrDefault(x => x.No == 0);

            if (zeroRound == null)
            {
                return;
            }

            var zeroRoundActions = zeroRound.Actions.Where(x => x.Type != ActionType.Ante).GroupBy(x => x.SeatNumber).ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var round in rounds)
            {
                if (round.No == 0)
                {
                    continue;
                }

                var playerActions = round.Actions.GroupBy(x => x.SeatNumber).Select(x => new { SeatNumber = x.Key, Actions = x.ToArray() });

                var playerActionsRaiseLevel = new Dictionary<int, decimal>();

                foreach (var playerAction in playerActions)
                {
                    if (!playerActionsRaiseLevel.ContainsKey(playerAction.SeatNumber))
                    {
                        var baseRaiseLevel = (round.No == 1 && zeroRoundActions.ContainsKey(playerAction.SeatNumber) && zeroRoundActions[playerAction.SeatNumber].Any()) ?
                                                zeroRoundActions[playerAction.SeatNumber].Last().Sum :
                                                0;

                        playerActionsRaiseLevel.Add(playerAction.SeatNumber, baseRaiseLevel);
                    }

                    foreach (var action in playerAction.Actions)
                    {
                        var preActionRaiseLevel = playerActionsRaiseLevel[playerAction.SeatNumber];
                        playerActionsRaiseLevel[playerAction.SeatNumber] += action.Sum;

                        if (action.Type == ActionType.RaiseTo)
                        {
                            action.Sum += preActionRaiseLevel;
                        }
                    }
                }
            }
        }

        private void RelocateSeats(List<Player> players, int maxPlayers)
        {
            if (relocationData == null)
            {
                return;
            }

            var heroSeat = players.FirstOrDefault(x => x.IsHero)?.Seat ?? -1;

            // real position
            var me = heroSeat < 0 ? int.Parse(relocationData.Attribute("me").Value) : heroSeat;

            // position on table
            var pivot = int.Parse(relocationData.Attribute("pivot").Value);

            var shift = (pivot - me) % maxPlayers;

            foreach (var player in players)
            {
                player.Seat = (player.Seat + shift + maxPlayers) % maxPlayers;
            }
        }
    }
}