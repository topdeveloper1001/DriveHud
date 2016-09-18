//-----------------------------------------------------------------------
// <copyright file="PokerBuilder.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using DriveHUD.Importers.Bovada;
using DriveHUD.Common;
using Model.Enums;
using Model.Site;
using DriveHUD.Entities;

namespace DriveHUD.Importers.Builders.iPoker
{
    /// <summary>
    /// iPoker hand history builder
    /// </summary>
    internal class PokerBuilder : IHandHistoryBuilder
    {
        private HandModel2 handModel;
        private IPokerTable tableModel;
        private int actionNo;
        private ICardsConverter converter;
        private ISiteConfiguration configuration;

        public PokerBuilder()
        {
            converter = new PokerCardsConverter();
        }

        public string Build(HandModel2 handModel, IPokerTable tableModel, ISiteConfiguration configuration, out Game game)
        {
            Check.ArgumentNotNull(() => handModel);
            Check.ArgumentNotNull(() => tableModel);
            Check.Require(configuration != null, "Configuration must be set.");

            actionNo = 1;

            this.handModel = handModel;
            this.tableModel = tableModel;
            this.configuration = configuration;

            var rounds = new List<Round>
            {
                BuildZeroRound(),
                BuildPreflopRound(),
                BuildPostflopRound(),
                BuildTurnRound(),
                BuildRiverRound()
            };

            rounds = rounds.Where(x => x != null).ToList();

            var gameGeneral = BuildGameGeneral(rounds);

            game = new Game
            {
                GameCode = handModel.HandNumber,
                General = gameGeneral,
                Rounds = rounds
            };

            AdjustRaises(rounds);

            var handHistory = CreateHandHistory(game);
            var handHistoryXmlDocument = GetHandHistoryXmlDocument(handHistory);

            // set place in tournament if hero finished tournament
            if (handModel.CashOrTournament == CashOrTournament.Tournament)
            {
                SetTournamentPlace(handHistoryXmlDocument);
            }

            // do not append existing hand
            if (!HandExists(handHistoryXmlDocument))
            {
                var gameXmlNode = GetGameXmlNode(game);

                var gameXmlNodeImported = handHistoryXmlDocument.ImportNode(gameXmlNode, true);

                handHistoryXmlDocument.DocumentElement.AppendChild(gameXmlNodeImported);
            }

            var handHistoryXml = handHistoryXmlDocument.InnerXml;

            return handHistoryXml;
        }

        #region IO operations

        /// <summary>
        /// Serialize an object into an XML string
        /// </summary>
        /// <typeparam name="T">Type of object to serialize.</typeparam>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="enc">Encoding of the serialized output.</param>
        /// <returns>Serialized (xml) object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private static string SerializeObject<T>(T obj, Encoding enc)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            using (var ms = new MemoryStream())
            {
                var xmlWriterSettings = new XmlWriterSettings()
                {
                    // If set to true XmlWriter would close MemoryStream automatically and using would then do double dispose
                    // Code analysis does not understand that. That's why there is a suppress message.
                    CloseOutput = false,
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = false,
                    Indent = true
                };

                using (var xw = XmlWriter.Create(ms, xmlWriterSettings))
                {
                    var s = new XmlSerializer(typeof(T));
                    s.Serialize(xw, obj, new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) }));
                }

                var result = enc.GetString(ms.ToArray());

                var _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

                if (result.StartsWith(_byteOrderMarkUtf8))
                {
                    result = result.Remove(0, _byteOrderMarkUtf8.Length);
                }

                return result;
            }
        }

        private XmlNode GetGameXmlNode(Game game)
        {
            var gameXml = SerializeObject(game, Encoding.UTF8);

            var gameXmlDocument = new XmlDocument();
            gameXmlDocument.PreserveWhitespace = true;
            gameXmlDocument.LoadXml(gameXml);

            return gameXmlDocument.DocumentElement;
        }

        private XmlDocument GetHandHistoryXmlDocument(HandHistory handHistory)
        {
            var handHistoryXml = SerializeObject(handHistory, Encoding.UTF8);

            var handHistoryXmlDocument = new XmlDocument();
            handHistoryXmlDocument.PreserveWhitespace = true;
            handHistoryXmlDocument.LoadXml(handHistoryXml);

            return handHistoryXmlDocument;
        }

        #endregion

        #region Rounds

        /// <summary>
        /// Build zero round (ante & sb & bb)
        /// </summary>
        /// <returns>Zero round</returns>
        private Round BuildZeroRound()
        {
            // zero round logic:
            // order: ante -> sb -> bb
            // player has enough chips: ante -> sb or bb
            // player has not enough chips (< ante): ante -> no blinds
            // player has not enough chips (< sb): full ante -> partial sb
            // player has not enough chips (< bb): full ante -> partial bb

            var actions = GetAnteActions();

            Action<int, ActionType, decimal> addBlindAction = (seat, actionType, blind) =>
            {
                if (!tableModel.PlayersOnTable.ContainsKey(seat) || !handModel.StacksBeforeHand.ContainsKey(seat))
                {
                    return;
                }

                var initialStack = handModel.StacksBeforeHand[seat];

                // if SB/BB initial stack more than Ante then SB/BB is posted
                if (initialStack > handModel.Ante)
                {
                    var stackAfterAnte = initialStack - handModel.Ante;
                    var sum = (stackAfterAnte - blind > 0) ? blind : stackAfterAnte;

                    var sbAction = new Action
                    {
                        No = actionNo++,
                        Type = actionType,
                        Player = tableModel.PlayersOnTable[seat],
                        SeatNumber = seat,
                        Sum = sum,
                        Cards = PokerConfiguration.ZeroRoundActionCards
                    };

                    actions.Add(sbAction);
                }
            };

            addBlindAction(handModel.SmallBlindSeat, ActionType.SB, handModel.SmallBlind);
            addBlindAction(handModel.BigBlindSeat, ActionType.BB, handModel.BigBlind);

            foreach (var item in handModel.DeadBlinds)
            {
                var deadBlindActionType = item.Value == handModel.SmallBlind ? ActionType.SB : ActionType.BB;
                addBlindAction(item.Key, deadBlindActionType, item.Value);
            }

            var zeroRound = new Round
            {
                No = 0,
                Actions = actions
            };

            return zeroRound;
        }

        private Round BuildPreflopRound()
        {
            if (handModel.PreflopCommands == null)
            {
                throw new Exception(string.Format("No preflops commands. Possible MTT table changing round. [HandNumber = {0}]", handModel.HandNumber));
            }

            var shownCards = handModel.Commands.FilterCommands<CardShown>(CommandCodeEnum.CardShown);

            // take all shown cards and group them by seat
            var shownCardsGrouped = (from shownCard in shownCards
                                     group shownCard by shownCard.SeatNumber into shownCardGrouped
                                     select new
                                     {
                                         SeatNumber = shownCardGrouped.Key,
                                         Cards = shownCardGrouped.Select(x => x.Card.Replace("Error Error", PokerConfiguration.UnknownCard)).Distinct().ToArray()
                                     }).ToDictionary(x => x.SeatNumber, x => x.Cards);

            var pocketCards = handModel.PreflopCommands.FilterCommands<PocketCards>(CommandCodeEnum.PocketCards);

            // take all pocket cards, and group them by seat, and merge with shown cards
            var pocketCardsGrouped = (from pocketCard in pocketCards
                                      group pocketCard by pocketCard.SeatNumber into pockerCardGrouped
                                      let seatCards = pockerCardGrouped
                                                           .Select(x => string.IsNullOrWhiteSpace(x.Card) ? PokerConfiguration.UnknownCard : x.Card)
                                                           .ToArray()
                                      let mergedSeatCards = shownCardsGrouped.ContainsKey(pockerCardGrouped.Key) ?
                                                                MergeCards(seatCards, shownCardsGrouped[pockerCardGrouped.Key]).ToArray() :
                                                                seatCards
                                      select new
                                      {
                                          SeatNumber = pockerCardGrouped.Key,
                                          Cards = string.Join(PokerConfiguration.CardSeparator, mergedSeatCards)
                                      }).ToDictionary(x => x.SeatNumber, x => x.Cards);

            var invalidCards = pocketCardsGrouped.FirstOrDefault(x => !tableModel.PlayersOnTable.ContainsKey(x.Key));

            if (invalidCards.Value != null)
            {
                throw new Exception(string.Format("Invalid preflop commands [Cards]. Couldn't find player's name on seat. [HandNumber = {0}, Seat = {1}]",
                                        handModel.HandNumber, invalidCards.Key));
            }

            // take all set stack 
            var setStacks = handModel.PreflopCommands.FilterCommands<Stack>(CommandCodeEnum.SetStack);

            // get delta (n2-n1) and group it by seat
            var roundDeltaStack = (from setStack in setStacks
                                   group setStack by setStack.SeatNumber into setStacksGrouped
                                   select new
                                   {
                                       SeatNumber = setStacksGrouped.Key,
                                       DeltaStacks = new Queue<decimal>(GetDeltaCollection(setStacksGrouped.Select(x => x.StackValue)))
                                   }).ToDictionary(x => x.SeatNumber, x => x.DeltaStacks);

            var actions = GetRoundActions(handModel.PreflopCommands, roundDeltaStack, HandPhaseEnum.Preflop);

            var cards = (from card in pocketCardsGrouped
                         select new Cards
                         {
                             Type = CardsType.Pocket,
                             Seat = card.Key,
                             Player = tableModel.PlayersOnTable[card.Key],
                             Value = converter.Convert(card.Value)
                         }).ToList();

            var round = new Round
            {
                No = 1,
                Cards = cards,
                Actions = actions
            };

            return round;
        }

        private Round BuildPostflopRound()
        {
            if (handModel.PostflopCommands == null || handModel.PostflopCommands.Count < 1)
            {
                return null;
            }

            var roundDeltaStack = GetRoundSeatDeltaStack(handModel.PostflopCommands, handModel.PreflopCommands);
            var actions = GetRoundActions(handModel.PostflopCommands, roundDeltaStack, HandPhaseEnum.PostFlop);

            var round = new Round
            {
                No = 2,
                Cards = new List<Cards>
                {
                    new Cards
                    {
                        Player = string.Empty,
                        Type = CardsType.Flop,
                        Value = GetRoundCards(handModel.PostflopCommands, HandPhaseEnum.PostFlop)
                    }
                },
                Actions = actions
            };

            return round;
        }

        private Round BuildTurnRound()
        {
            if (handModel.TurnCommands == null || handModel.TurnCommands.Count < 1)
            {
                return null;
            }

            var roundDeltaStack = GetRoundSeatDeltaStack(handModel.TurnCommands, handModel.PostflopCommands);
            var actions = GetRoundActions(handModel.TurnCommands, roundDeltaStack, HandPhaseEnum.Turn);

            var round = new Round
            {
                No = 3,
                Cards = new List<Cards>
                {
                    new Cards
                    {
                        Player = string.Empty,
                        Type = CardsType.Turn,
                        Value = GetRoundCards(handModel.TurnCommands, HandPhaseEnum.Turn)
                    }
                },
                Actions = actions
            };

            return round;
        }

        private Round BuildRiverRound()
        {
            if (handModel.RiverCommands == null || handModel.RiverCommands.Count < 1)
            {
                return null;
            }

            var roundDeltaStack = GetRoundSeatDeltaStack(handModel.RiverCommands, handModel.TurnCommands);
            var actions = GetRoundActions(handModel.RiverCommands, roundDeltaStack, HandPhaseEnum.Turn);

            var round = new Round
            {
                No = 4,
                Cards = new List<Cards>
                {
                    new Cards
                    {
                        Player = string.Empty,
                        Type = CardsType.River,
                        Value = GetRoundCards(handModel.RiverCommands, HandPhaseEnum.River)
                    }
                },
                Actions = actions
            };

            return round;
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

        #endregion

        #region General

        private GameGeneral BuildGameGeneral(IEnumerable<Round> rounds)
        {
            if (rounds == null)
            {
                throw new ArgumentNullException("rounds");
            }

            var players = new List<Player>();

            var winners = GetWinners(rounds);

            foreach (var seat in handModel.SeatsDealt.OrderBy(x => x))
            {
                if (!tableModel.PlayersOnTable.ContainsKey(seat))
                {
                    throw new Exception(string.Format("Couldn't find player's name on #{0} seat [HandNumber = {1}]", seat, handModel.HandNumber));
                }

                if (!handModel.StacksBeforeHand.ContainsKey(seat))
                {
                    throw new Exception(string.Format("Couldn't find player's stack before hand on #{0} seat [HandNumber = {1}]", seat, handModel.HandNumber));
                }

                var playerActions = rounds.SelectMany(x => x.Actions).Where(x => x.SeatNumber == seat).ToArray();

                var playerBet = playerActions.Sum(x => x.Sum);

                // if no stacks after the hand, then player was removed 
                var stacksAfterHand = handModel.StacksAfterHand.ContainsKey(seat) ? handModel.StacksAfterHand[seat] : 0;

                var playerFolded = handModel.Commands.FilterCommands<PlayerAction>(CommandCodeEnum.PlayerAction)
                                        .Any(x => x.SeatNumber == seat &&
                                            (x.PlayerActionEnum == PlayerActionEnum.Fold || x.PlayerActionEnum == PlayerActionEnum.FoldShow));

                var win = !playerFolded ? stacksAfterHand - handModel.StacksBeforeHand[seat] + playerBet : 0;

                if (playerFolded)
                {
                    winners.Remove(seat);
                }

                var player = new Player
                {
                    Seat = seat,
                    Name = tableModel.PlayersOnTable[seat],
                    Chips = handModel.StacksBeforeHand[seat],
                    Dealer = handModel.DealerSeat == seat,
                    Win = win > 0 ? win : 0,
                    Bet = playerBet
                };

                players.Add(player);
            }

            int shift = AdjustPlayerSeats(players);

            var gameGeneral = new GameGeneral
            {
                StartDate = GetCurrentTimeUTC(),
                Players = players,
                PlayersSeatShift = shift,
            };

            return gameGeneral;
        }

        private Dictionary<int, Dictionary<int, int>> seatMap = new Dictionary<int, Dictionary<int, int>>
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
            }
        };

        private int ConvertSeat(int seat)
        {
            if (!seatMap.ContainsKey(handModel.TableType) || !seatMap[handModel.TableType].ContainsKey(seat))
            {
                return seat;
            }

            return seatMap[handModel.TableType][seat];
        }

        /// <summary>
        /// Adjusts players seats based on Preferred Seat setting
        /// </summary>
        /// <param name="players">collection of players to adjust seats for</param>
        /// <returns>Seat shift</returns>
        private int AdjustPlayerSeats(IEnumerable<Player> players)
        {
            var preferredSeats = configuration.PreferredSeats;

            if (!preferredSeats.ContainsKey(handModel.TableType))
            {
                return 0;
            }

            return RotatePlayerSeats(players, preferredSeats[handModel.TableType]);

            //foreach (var player in players)
            //{
            //    player.Seat = ConvertSeat(player.Seat);
            //}
        }

        /// <summary>
        /// Rotates player seats regarding preferred seat 
        /// </summary>
        /// <param name="players">collection of players to adjust seats for</param>
        /// <param name="prefferedSeat">hero's preferred seat</param>
        /// <returns>Seat shift</returns>
        private int RotatePlayerSeats(IEnumerable<Player> players, int prefferedSeat)
        {
            var heroSeat = tableModel.HeroSeat > 0 ? tableModel.HeroSeat : handModel.HeroSeat;

            if (heroSeat < 1 || heroSeat > 9)
            {
                return 0;
            }

            var shift = (prefferedSeat - heroSeat) % handModel.TableType;

            foreach (var player in players)
            {
                player.Seat = Helpers.GeneralHelpers.ShiftPlayerSeat(player.Seat, shift, handModel.TableType);
            }

            return shift;
        }

        #endregion

        #region Hand history

        private HandHistory CreateHandHistory(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }

            var general = new General
            {
                Mode = PokerConfiguration.DefaultMode,
                GameType = GetGameType(),
                TableName = GetTableName(),
                StartDate = GetTableTimeUTC(),
                Duration = PokerConfiguration.DefaultDuration,
                Nickname = configuration.HeroName,
                BuyIn = GetBuyIn(),
                Currency = Currency.USD,
                TotalBuyIn = GetTotalBuyIn()
            };

            if (handModel.CashOrTournament == CashOrTournament.Tournament)
            {
                general.TournamentName = GetTournamentName();
                general.Place = GetHeroPlace();
            }

            var handHistory = new HandHistory
            {
                SessionCode = handModel.Handle.ToInt32(),
                General = general
            };

            return handHistory;
        }

        private string GetGameType()
        {
            var gameTableType = GetGameTypeString(handModel.GameType, handModel.GameLimit);

            if (handModel.CashOrTournament != CashOrTournament.Tournament)
            {
                //if (handModel.GameType == GameType.Omaha)
                //{
                //    return GetOmahaGameTypeString(gameTableType);
                //}

                return GetHoldemGameTypeString(gameTableType);
            }

            return gameTableType;
        }

        private int GetHeroPlace()
        {
            // check if hero really finished tournament
            var heroFinishedCommand = handModel.PlayersFinishedTournament.FirstOrDefault(x => x.SeatNumber == handModel.HeroSeat);

            if (heroFinishedCommand != null)
            {
                return heroFinishedCommand.Rank;
            }

            // if no, then possible hero place is 
            if (handModel.LastNotHeroPlayerFinishedPlace != 0)
            {
                return handModel.LastNotHeroPlayerFinishedPlace == 1 ? 2 : handModel.LastNotHeroPlayerFinishedPlace - 1;
            }

            if (handModel.GameFormat == GameFormat.SnG)
            {
                return handModel.TableType;
            }

            return PokerConfiguration.DefaultTournamentPlace;
        }

        private string GetHoldemGameTypeString(string gameTableType)
        {
            decimal smallBlindToWrite = handModel.SmallBlind;
            decimal bigBlindToWrite = handModel.BigBlind;

            if (smallBlindToWrite <= 0 && bigBlindToWrite > 0)
            {
                smallBlindToWrite = BovadaConverters.ConvertBBtoSB(bigBlindToWrite);
            }

            var smallBlind = (handModel.GameLimit != GameLimit.NL) ?
                                     BovadaConverters.ConvertDecimalToString(GetLimitStake(smallBlindToWrite)) :
                                     BovadaConverters.ConvertDecimalToString(smallBlindToWrite);

            var bigBlind = (handModel.GameLimit != GameLimit.NL) ?
                                BovadaConverters.ConvertDecimalToString(GetLimitStake(bigBlindToWrite)) :
                                BovadaConverters.ConvertDecimalToString(bigBlindToWrite);

            var gameType = string.Format("{0} ${1}/${2}", gameTableType, smallBlind, bigBlind);

            return gameType;
        }

        private string GetOmahaGameTypeString(string gameTableType)
        {
            var regex = new Regex(@"(\$[\d\.]+\/\$[\d\.]+)");

            var match = regex.Match(tableModel.InitialTableTitle);

            if (!match.Success)
            {
                return GetHoldemGameTypeString(gameTableType);
            }

            var gameType = string.Format("{0} {1}", gameTableType, match.Value);
            return gameType;
        }

        private static string GetGameTypeString(GameType gameType, GameLimit gameLimit)
        {
            switch (gameType)
            {
                case GameType.Omaha:
                    return "Omaha PL";

                case GameType.OmahaHiLo:
                    if (gameLimit == GameLimit.NL)
                    {
                        return "Omaha Hi-Lo NL";
                    }
                    else if (gameLimit == GameLimit.PL)
                    {
                        return "Omaha Hi-Lo PL";
                    }

                    return "Omaha Hi-Lo Limit";

                default:
                    if (gameLimit == GameLimit.NL)
                    {
                        return "Holdem NL";
                    }

                    return "Holdem Limit";
            }
        }

        private string GetTableName()
        {
            if (handModel.IsZonePoker)
            {
                return string.Format(PokerConfiguration.ZonePokerTableTemplate, handModel.TableName);
            }

            if (handModel.CashOrTournament == CashOrTournament.Tournament)
            {
                return string.Format(PokerConfiguration.TournamentTableTitleTemplate, handModel.TableName, handModel.TournamentNumber);
            }

            return handModel.TableName;
        }

        private string GetTournamentName()
        {
            return string.Format(PokerConfiguration.TournamentTableTitleTemplate, handModel.TableName, handModel.TournamentNumber);
        }

        private string GetBuyIn()
        {
            return string.Format("${0}+${1}", BovadaConverters.ConvertDecimalToString(handModel.TournamentBuyIn), BovadaConverters.ConvertDecimalToString(handModel.TournamentRake));
        }
        private string GetTotalBuyIn()
        {
            return string.Format("${0}", BovadaConverters.ConvertDecimalToString(handModel.TournamentBuyIn + handModel.TournamentRake));
        }

        private decimal GetLimitStake(decimal smallBlind)
        {
            if (smallBlind == 0.02m)
            {
                return 0.05m;
            }
            else if (smallBlind == 0.1m)
            {
                return 0.25m;
            }
            return smallBlind * 2;
        }

        private void SetTournamentPlace(XmlDocument handHistory)
        {
            var placeNode = handHistory.SelectSingleNode("//session/general/place");

            if (placeNode == null)
            {
                return;
            }

            var place = GetHeroPlace();

            placeNode.InnerText = place.ToString();
        }

        private bool HandExists(XmlDocument handHistory)
        {
            var xpath = string.Format("//session/game[@gamecode='{0}']", handModel.HandNumber);

            var gameNode = handHistory.SelectSingleNode(xpath);

            return gameNode != null;
        }

        #endregion

        #region Infrastructure

        private List<Action> GetAnteActions()
        {
            var actions = new List<Action>();

            if (handModel.Ante > 0)
            {
                var playersPostedAnte = new HashSet<int>(handModel.Commands.FilterCommands<PlayerAction>(CommandCodeEnum.PlayerAction)
                                                        .Where(x => x.PlayerActionEnum == PlayerActionEnum.Ante)
                                                        .Select(x => x.SeatNumber));

                foreach (var seat in playersPostedAnte)
                {
                    if (!tableModel.PlayersOnTable.ContainsKey(seat))
                    {
                        throw new Exception(string.Format("Couldn't find player's name on #{0} seat [HandNumber = {1}]", seat, handModel.HandNumber));
                    }

                    if (!handModel.StacksBeforeHand.ContainsKey(seat))
                    {
                        throw new Exception(string.Format("Couldn't find player's stack before hand on #{0} seat [HandNumber = {1}]", seat, handModel.HandNumber));
                    }

                    var sum = handModel.Ante > handModel.StacksBeforeHand[seat] ? handModel.StacksBeforeHand[seat] : handModel.Ante;

                    var anteAction = new Action
                    {
                        No = actionNo++,
                        Type = ActionType.Ante,
                        Player = tableModel.PlayersOnTable[seat],
                        Sum = sum,
                        SeatNumber = seat,
                        Cards = PokerConfiguration.ZeroRoundActionCards
                    };

                    actions.Add(anteAction);
                }
            }

            return actions;
        }

        private static IEnumerable<decimal> GetDeltaCollection(IEnumerable<decimal> collection)
        {
            var array = collection.ToArray();
            var deltaCollection = new List<decimal>();

            if (array.Length > 1)
            {
                for (var i = 1; i < array.Length; i++)
                {
                    var delta = array[i] - array[i - 1];
                    deltaCollection.Add(delta);
                }
            }

            return deltaCollection;
        }

        private ActionType GetActionType(PlayerActionEnum playerActionType, decimal sum)
        {
            switch (playerActionType)
            {
                case PlayerActionEnum.Fold:
                case PlayerActionEnum.FoldShow:
                    return ActionType.Fold;
                case PlayerActionEnum.Call:
                    return ActionType.Call;
                case PlayerActionEnum.Check:
                    return ActionType.Check;
                case PlayerActionEnum.Bet:
                    return ActionType.Bet;
                case PlayerActionEnum.RaiseTo:
                    return ActionType.RaiseTo;
                case PlayerActionEnum.Allin:
                    return ActionType.AllIn;
                case PlayerActionEnum.AllinRaise:
                    return ActionType.AllIn;
                case PlayerActionEnum.PostSB:
                    return ActionType.SB;
                case PlayerActionEnum.PostBB:
                    return ActionType.BB;
                default:
                    throw new Exception(string.Format("Unexpected player action [HandNumber = {0}, Action = {1}]", handModel.HandNumber, playerActionType));
            }
        }

        /// <summary>
        /// Get round actions
        /// </summary>
        /// <param name="roundCommands">Round commands</param>
        /// <param name="roundDeltaStack">Round delta stacks</param>
        /// <param name="handPhase">Phase of hand</param>
        /// <returns>Round actions</returns>
        private List<Action> GetRoundActions(IEnumerable<Command> roundCommands, Dictionary<int, Queue<decimal>> roundDeltaStack, HandPhaseEnum handPhase)
        {
            if (roundCommands == null)
            {
                throw new ArgumentNullException("roundCommands");
            }

            if (roundDeltaStack == null)
            {
                throw new ArgumentNullException("roundDeltaStack");
            }

            var actions = new List<Action>();

            // take all player action
            var playerActions = roundCommands.FilterCommands<PlayerAction>(CommandCodeEnum.PlayerAction);

            var invalidPlayerAction = playerActions.FirstOrDefault(x => !tableModel.PlayersOnTable.ContainsKey(x.SeatNumber));

            if (invalidPlayerAction != null)
            {
                throw new Exception(string.Format("Invalid {0} commands [Action]. Couldn't find player's name on seat. [HandNumber = {1}, Seat = {2}]",
                                        handPhase.ToString().ToLower(), handModel.HandNumber, invalidPlayerAction.SeatNumber));
            }

            foreach (var playerAction in playerActions)
            {
                if (!roundDeltaStack.ContainsKey(playerAction.SeatNumber) || roundDeltaStack[playerAction.SeatNumber].Count < 1)
                {
                    throw new Exception(string.Format("Invalid {0} commands [Action, Stacks]. Couldn't find stack for player action [HandNumber = {1}, Seat = {2}, Action = {3}]",
                                      handPhase.ToString().ToLower(), handModel.HandNumber, playerAction.SeatNumber, playerAction.PlayerActionEnum));
                }

                var sum = -((handModel.CashOrTournament == CashOrTournament.Tournament) ?
                            roundDeltaStack[playerAction.SeatNumber].Dequeue() * 100 :
                            roundDeltaStack[playerAction.SeatNumber].Dequeue());
                var actionType = GetActionType(playerAction.PlayerActionEnum, sum);

                var action = new Action
                {
                    No = actionNo++,
                    Player = tableModel.PlayersOnTable[playerAction.SeatNumber],
                    Sum = sum,
                    SeatNumber = playerAction.SeatNumber,
                    Type = actionType
                };

                actions.Add(action);
            }

            return actions;
        }

        /// <summary>
        /// Get round deltas
        /// </summary>
        /// <param name="currentRoundCommands">Current round commands</param>
        /// <param name="previousRoundCommands">Previous round commands</param>
        /// <returns></returns>
        private Dictionary<int, Queue<decimal>> GetRoundSeatDeltaStack(IEnumerable<Command> currentRoundCommands, IEnumerable<Command> previousRoundCommands)
        {
            var previousRoundStacks = previousRoundCommands.FilterCommands<Stack>(CommandCodeEnum.SetStack);

            var previousRoundLastStacks = (from previousRoundStack in previousRoundStacks
                                           group previousRoundStack by previousRoundStack.SeatNumber into previousRoundStackGrouped
                                           select previousRoundStackGrouped.LastOrDefault()).ToArray();

            var roundStacks = previousRoundLastStacks.Concat(currentRoundCommands.FilterCommands<Stack>(CommandCodeEnum.SetStack));

            // get delta (n2-n1) and group it by seat
            var deltaStacksGrouped = (from roundStack in roundStacks
                                      group roundStack by roundStack.SeatNumber into roundStacksGrouped
                                      select new
                                      {
                                          SeatNumber = roundStacksGrouped.Key,
                                          DeltaStacks = new Queue<decimal>(GetDeltaCollection(roundStacksGrouped.Select(x => x.StackValue)))
                                      }).ToDictionary(x => x.SeatNumber, x => x.DeltaStacks);

            return deltaStacksGrouped;
        }

        private string GetRoundCards(IEnumerable<Command> roundCommands, HandPhaseEnum handPhase)
        {
            var communityCards = roundCommands.FilterCommands<CommunityCard>(CommandCodeEnum.CommunityCard).ToArray();

            var expectedCardsAmount = (handPhase == HandPhaseEnum.PostFlop) ? 3 : 1;

            if (communityCards.Length != expectedCardsAmount)
            {
                throw new Exception(string.Format("Invalid community cards. Expected amount of cards: {0}, but was {1} [HandNumber = {2}, Phase = {3}]",
                                      expectedCardsAmount, communityCards.Length, handModel.HandNumber, handPhase));
            }

            var cards = string.Join(PokerConfiguration.CardSeparator, communityCards.Select(x => x.Card).ToArray());
            var convertedCards = converter.Convert(cards);

            return convertedCards;
        }

        /// <summary>
        /// Merge pocket and shown cards
        /// </summary>
        /// <param name="pocketCards">Pocket cards</param>
        /// <param name="shownCards">Shown cards</param>
        /// <returns>Merged cards</returns>
        private IEnumerable<string> MergeCards(IEnumerable<string> pocketCards, IEnumerable<string> shownCards)
        {
            if (pocketCards == null || shownCards == null || !shownCards.Any())
            {
                return pocketCards;
            }

            // get valuable cards only (remove empty, X, Error)
            var valueableCards = shownCards.Where(x => !string.IsNullOrWhiteSpace(x) && !x.Equals("Error") && !x.Equals(PokerConfiguration.UnknownCard)).ToArray();

            if (valueableCards.Length < 1)
            {
                return pocketCards;
            }

            var num = pocketCards.Count();

            var mergedCards = pocketCards
                                .Concat(valueableCards)
                                .Distinct()
                                .Where(x => !x.Equals(PokerConfiguration.UnknownCard))
                                .ToList();

            // fill missed cards with X
            for (var i = 0; i < num - mergedCards.Count; i++)
            {
                mergedCards.Add(PokerConfiguration.UnknownCard);
            }

            return mergedCards;
        }

        private DateTime GetCurrentTime()
        {
            return GetCorrectedTime(DateTime.Now);
        }

        private DateTime GetCurrentTimeUTC()
        {
            return GetUTCTime(DateTime.Now);
        }

        private DateTime GetTableTime()
        {
            return GetCorrectedTime(tableModel.BeginDate);
        }

        private DateTime GetTableTimeUTC()
        {
            return GetUTCTime(tableModel.BeginDate);
        }

        private DateTime GetCorrectedTime(DateTime dateTime)
        {
            var currentUTCTime = dateTime.ToUniversalTime();

            var timeZoneOffset = configuration.TimeZoneOffset;

            if (timeZoneOffset != null)
            {
                currentUTCTime = currentUTCTime.Add(timeZoneOffset);
            }

            return currentUTCTime;
        }

        private DateTime GetUTCTime(DateTime dateTime)
        {
            return dateTime.ToUniversalTime();
        }

        private IPokerEvaluator GetEvaluator()
        {
            switch (handModel.GameType)
            {
                case GameType.Omaha:
                case GameType.OmahaHiLo:
                    return new OmahaEvaluator();
                case GameType.Holdem:
                    return new HoldemEvaluator();
                default:
                    return null;
            }
        }

        private IList<int> GetWinners(IEnumerable<Round> rounds)
        {
            var evaluator = GetEvaluator();

            if (evaluator == null)
            {
                return new List<int>();
            }

            var roundCards = rounds.Where(x => x.Cards != null).SelectMany(x => x.Cards).ToArray();
            var boardCards = roundCards.Where(x => x.Type != CardsType.Pocket).Select(x => x.Value).ToArray();
            var boardCardsString = string.Join(" ", boardCards);

            evaluator.SetCardsOnTable(boardCardsString);

            var playersCards = roundCards.Where(x => x.Type == CardsType.Pocket && !x.Value.Contains(PokerConfiguration.UnknownCard)).ToArray();

            if (playersCards.Length < 1)
            {
                return new List<int>();
            }

            foreach (var playerCards in playersCards)
            {
                evaluator.SetPlayerCards(playerCards.Seat, playerCards.Value);
            }

            var winners = evaluator.GetWinners().ToList();

            return winners;
        }

        #endregion
    }
}