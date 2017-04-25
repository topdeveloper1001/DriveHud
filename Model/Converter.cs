using System;
using System.Linq;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HoldemHand;
using Model.Enums;
using System.Collections.Generic;
using DriveHUD.Common.Log;
using Model.Settings;
using Microsoft.Practices.ServiceLocation;
using DriveHUD.Entities;
using Model.Extensions;
using DriveHUD.EquityCalculator.Base.OmahaCalculations;
using HandHistories.Objects.Players;
using Card = HandHistories.Objects.Cards.Card;

namespace Model.Importer
{
    public static class Converter
    {
        private static List<EnumPosition[]> PositionList = new List<EnumPosition[]>() {
            new EnumPosition[2] { EnumPosition.SB, EnumPosition.BB }, // 2-handed table
            new EnumPosition[3] { EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB }, // 3-handed table
            new EnumPosition[4] { EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB,}, // 4-handed table
            new EnumPosition[5] { EnumPosition.MP1, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB}, // 5-handed table
            new EnumPosition[6] { EnumPosition.UTG, EnumPosition.MP1, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB}, // 6-handed table
            new EnumPosition[7] { EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB }, // 7-handed table
            new EnumPosition[8] { EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB }, // 8-handed table
            new EnumPosition[9] { EnumPosition.UTG, EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB }, // 9-handed table
            new EnumPosition[10] { EnumPosition.UTG, EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.MP3, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB }, // 10-handed table
        };

        public static string ToHoleCards(HoleCards holeCards)
        {
            if (holeCards == null)
                return string.Empty;

            var mask = Hand.ParseHand(holeCards.ToString());
            var cards = Hand.Cards(mask).ToList();
            cards.Sort(new CardComparer());

            return string.Join(string.Empty, cards);
        }

        public static HandHistoryRecord ToHandHistoryRecord(HandHistory hand, Playerstatistic stat)
        {
            var player = hand.Players.FirstOrDefault(x => string.Equals(x.PlayerName, stat.PlayerName, StringComparison.OrdinalIgnoreCase));

            if (player == null)
            {
                return null;
            }

            var record = new HandHistoryRecord
            {
                Time = hand.DateOfHandUtc,
                Cards = player.HoleCards == null ? string.Empty : player.HoleCards.ToString(),
                Line = hand.HandActions.Where(x => x.PlayerName == stat.PlayerName || x is StreetAction)
                    .Select(ActionToString)
                    .Aggregate((x, y) => x + y).Trim(','),

                Board = hand.CommunityCards.ToString(),
                NetWonInCents = stat.Totalamountwonincents,
                BBinCents = stat.Totalamountwonincents / stat.BigBlind,
                Pos = ToPositionString(ToPosition(hand, stat)),
                Allin = ToAllin(hand, stat),
                Action = ToAction(stat),
                Equity = stat.Equity
            };

            if (record.Equity != 0 && hand.TotalPot != null)
            {
                if (stat.Wonhand == 1)
                    record.EquityDiff = -(decimal)(hand.TotalPot * (1 - record.Equity));
                else
                    record.EquityDiff = (decimal)((hand.TotalPot ?? 0) * record.Equity);
            }

            return record;
        }

        public static string ActionToString(HandAction action)
        {
            if (action is StreetAction)
                return ",";

            if (action.IsRaise())
                return "R";

            if (action.IsCall())
                return "C";

            if (action.IsBet())
                return "B";

            if (action.HandActionType == HandActionType.FOLD)
                return "F";

            if (action.HandActionType == HandActionType.CHECK)
                return "X";

            return string.Empty;
        }

        public static EnumPosition ToPosition(HandHistory hand, Playerstatistic stat)
        {
            if (stat.IsDealer)
            {
                return EnumPosition.BTN;
            }
            else if (stat.IsSmallBlind)
            {
                return EnumPosition.SB;
            }
            else if (stat.IsBigBlind)
            {
                return EnumPosition.BB;
            }
            else if (stat.IsCutoff)
            {
                return EnumPosition.CO;
            }

            var firstPlayerAction = hand.HandActions.FirstOrDefault(x => x.PlayerName == stat.PlayerName);
            if (firstPlayerAction != null)
            {
                int firstPlayerActionIndex = hand.HandActions.ToList().IndexOf(firstPlayerAction) - hand.HandActions.Where(x => x.HandActionType == HandActionType.SMALL_BLIND).Take(1).Count() - hand.HandActions.Where(x => x.HandActionType == HandActionType.BIG_BLIND).Take(1).Count();

                /* Conver size in case if there are not 2 blind actions (only bb, multiple bb etc.) */
                var blinds = hand.HandActions.Take(2);
                int blindSize = (hand.HandActions.Any(x => x.HandActionType == HandActionType.SMALL_BLIND && blinds.Any(b => b.PlayerName == x.PlayerName)) ? 1 : 0)
                    + (hand.HandActions.Any(x => x.HandActionType == HandActionType.BIG_BLIND && blinds.Any(b => b.PlayerName == x.PlayerName)) ? 1 : 0);

                int tableSize = hand.Players.Where(p => !p.IsSittingOut).Count() - blindSize + 2; // PositionList contains 2 blind positions
                var table = PositionList.FirstOrDefault(x => x.Count() == tableSize);

                if (table != null && firstPlayerActionIndex >= 0 && firstPlayerActionIndex < table.Count())
                {
                    return table[firstPlayerActionIndex];
                }
            }

            // determine position based on distance from dealer

            return EnumPosition.Undefined;
        }

        public static string ToPositionString(EnumPosition position)
        {
            switch (position)
            {
                case EnumPosition.BTN:
                    return "BTN";
                case EnumPosition.SB:
                    return "SB";
                case EnumPosition.BB:
                    return "BB";
                case EnumPosition.CO:
                    return "CO";
                case EnumPosition.MP3:
                case EnumPosition.MP2:
                case EnumPosition.MP1:
                case EnumPosition.MP:
                    return "MP";
                case EnumPosition.UTG:
                case EnumPosition.UTG_1:
                case EnumPosition.UTG_2:
                case EnumPosition.EP:
                    return "EP";
                case EnumPosition.Undefined:
                default:
                    return "Undefined";
            }

        }

        public static string ToAction(Playerstatistic stat)
        {
            if (stat.Vpiphands > 0)
                return "VPIP";

            if (stat.Pfrhands > 0)
                return "PFR";

            return string.Empty;
        }

        public static string ToAllin(HandHistory hand, Playerstatistic stat)
        {
            var wasAllinAction = hand.HandActions.OfType<AllInAction>().FirstOrDefault();
            if (wasAllinAction == null)
                return string.Empty;

            return wasAllinAction.Street.ToString();
        }

        public static string CurrentBoardCards { get; set; }
        public static HandHistories.Objects.Cards.Card[] CurrentBoard { get; set; }

        public static Player[] ActivePlayerHasHoleCard { get; set;}

        public static decimal CalculateEquity(HandAction action, HandHistory hand, string name)
        {
            
            if (action.IsFold || !hand.Players.FirstOrDefault(x => x.PlayerName == name).hasHoleCards)     //todo make for dead cards, for exmaple when guy folds with known hole cards
                return -1;
            if (ActivePlayerHasHoleCard == null)
                ActivePlayerHasHoleCard = hand.Players.Where(player => player.hasHoleCards).ToArray();

            HoleCards[] cards = ActivePlayerHasHoleCard.Select(x => x.HoleCards).ToArray();
            Player currentPlayer = ActivePlayerHasHoleCard.FirstOrDefault(x => x.PlayerName == name);

            int count = cards.Count();
            decimal equity = 0;
            int targetPlayerIndex = cards.ToList().IndexOf(currentPlayer?.HoleCards);

            switch (action.Street)
            {
                case Street.Preflop:
                    CurrentBoard = new HandHistories.Objects.Cards.Card[] {};
                    CurrentBoardCards = string.Empty;
                    break;
                case Street.Flop:
                    CurrentBoard = hand.CommunityCards.Take(3).ToArray();
                    CurrentBoardCards = new string(hand.CommunityCardsString.Take(6).ToArray());
                    break;
                case Street.Turn:
                    CurrentBoard = hand.CommunityCards.Take(4).ToArray();
                    CurrentBoardCards = new string(hand.CommunityCardsString.Take(8).ToArray());
                    break;
                case Street.River:
                    CurrentBoard = hand.CommunityCards.ToArray();
                    CurrentBoardCards = hand.CommunityCardsString;
                    break;
                case Street.Showdown:
                    CurrentBoard = hand.CommunityCards.ToArray();
                    CurrentBoardCards = hand.CommunityCardsString;
                    break;
                case Street.Summary:
                    CurrentBoard = hand.CommunityCards.ToArray();//todo the_weeknd 9J268
                    CurrentBoardCards = hand.CommunityCardsString;
                    break;
            }  
            try
            {
                GeneralGameTypeEnum gameType = new GeneralGameTypeEnum().ParseGameType(hand.GameDescription.GameType);
                
                if (gameType == GeneralGameTypeEnum.Holdem && currentPlayer?.HoleCards?.Count > 0)
                {
                    long[] wins = new long[count];
                    long[] losses = new long[count];
                    long[] ties = new long[count];
                    long totalhands = 0;

                    List<string> strList = new List<string>();
                    foreach (var card in cards.Where(card => card?.Count > 0))
                        strList.AddRange(CardHelper.SplitTwoCards(card.ToString()));
                    Hand.HandWinOdds(strList.ToArray(), CurrentBoardCards, string.Empty, wins, ties, losses, ref totalhands);
                    if (totalhands != 0)
                    {
                        equity = Math.Round((decimal)(wins[targetPlayerIndex] * 100) / totalhands, 2) ;
                    }
                }
                //gameType = GeneralGameTypeEnum.Omaha;
                //cards[0] = HoleCards.ForOmaha(HandHistories.Objects.Cards.Card.Parse("Qd"), 
                //                              HandHistories.Objects.Cards.Card.Parse("9d"), 
                //                              HandHistories.Objects.Cards.Card.Parse("9s"), 
                //                              HandHistories.Objects.Cards.Card.Parse("7s")); 
                //cards[1] = HoleCards.ForOmaha(HandHistories.Objects.Cards.Card.Parse("5h"),
                //                              HandHistories.Objects.Cards.Card.Parse("Tc"),
                //                              HandHistories.Objects.Cards.Card.Parse("Qc"),
                //                              HandHistories.Objects.Cards.Card.Parse("8d"));   
                //string[][] t = cards.Select(x => x.Select(c => c.ToString()).ToArray()).ToArray();
                else if ((gameType == GeneralGameTypeEnum.Omaha || gameType == GeneralGameTypeEnum.OmahaHiLo) && currentPlayer?.HoleCards?.Count > 0)
                {
                    OmahaEquityCalculatorMain calc = new OmahaEquityCalculatorMain(true, gameType == GeneralGameTypeEnum.OmahaHiLo);
                    MEquity[] eq = calc.Equity(CurrentBoard.Select(x => x.ToString()).ToArray(),
                                         cards.Select(x => x.Select(c => c.ToString()).ToArray()).ToArray(),
                                         new string[] { },
                                         0);

                    equity = Math.Round((decimal)eq[targetPlayerIndex].TotalEq, 2);
                }

            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }

            return equity;
        }

        public static decimal CalculateAllInEquity(HandHistory hand, Playerstatistic stat)
        {
            // last hero street action
            var lastHeroStreetAction = hand.HandActions
                .LastOrDefault(x => x.PlayerName == stat.PlayerName
                                    && x.Street >= Street.Preflop
                                    && x.Street <= Street.River);

            if (lastHeroStreetAction == null || lastHeroStreetAction.IsFold || lastHeroStreetAction.Street == Street.River)
            {
                return 0m;
            }

            if (!hand.HandActions.Where(x => x.Street == lastHeroStreetAction.Street).Any(x => x.IsAllInAction))
            {
                return 0m;
            }

            var currentPlayer = hand.Players.FirstOrDefault(x => x.PlayerName == stat.PlayerName);
            if (currentPlayer == null || !currentPlayer.hasHoleCards)
            {
                return 0m;
            }

            var activePlayers = hand.Players.Where(p => hand.HandActions.Any(x => p.PlayerName == x.PlayerName))
                .Where(p => !hand.HandActions.Any(h => h.PlayerName == p.PlayerName && h.IsFold)).Where(x => x.hasHoleCards).ToList();

            if (activePlayers.Count() < 2)
            {
                return 0m;
            }

            var cards = activePlayers.Select(x => x.HoleCards).ToArray();

            int count = cards.Count();
            int targetPlayerIndex = cards.ToList().IndexOf(currentPlayer.HoleCards);
            // something went wrong
            if (count == 0 || targetPlayerIndex < 0)
            {
                LogProvider.Log.Debug($"Wasn't able to calculate AllIn equity for hand {hand.HandId}. Hand date: {hand.DateOfHandUtc}. HandHistory:{Environment.NewLine}{hand.FullHandHistoryText}");
                return 0;
            }

            decimal equity = 0;

            try
            {
                var gameType = new GeneralGameTypeEnum().ParseGameType(hand.GameDescription.GameType);

                // holdem
                var boardCards = CardHelper.IsStreetAvailable(hand.CommunityCards.ToString(), lastHeroStreetAction.Street)
                      ? hand.CommunityCards.GetBoardOnStreet(lastHeroStreetAction.Street)
                      : hand.CommunityCards;

                if (gameType == GeneralGameTypeEnum.Holdem && cards.All(x => x.Count <= 2))
                {
                    long[] wins = new long[count];
                    long[] losses = new long[count];
                    long[] ties = new long[count];
                    long totalhands = 0;


                    Hand.HandWinOdds(cards.Select(x => x.ToString()).ToArray(), boardCards.ToString(), string.Empty, wins, ties, losses, ref totalhands);

                    if (totalhands != 0)
                    {
                        equity = (decimal)(wins[targetPlayerIndex] + ties[targetPlayerIndex] / 2.0) / totalhands;
                    }
                }
                else if ((gameType == GeneralGameTypeEnum.Omaha || gameType == GeneralGameTypeEnum.OmahaHiLo) && cards.All(x => x.Count >= 2 && x.Count < 5))
                {
                    OmahaEquityCalculatorMain calc = new OmahaEquityCalculatorMain(true, gameType == GeneralGameTypeEnum.OmahaHiLo);
                    var eq = calc.Equity(boardCards.Select(x => x.ToString()).ToArray(), cards.Select(x => x.Select(c => c.ToString()).ToArray()).ToArray(), new string[] { }, 0);

                    equity = (decimal)eq[targetPlayerIndex].TotalEq / 100;
                }

            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }

            return equity;
        }

        public static string ActionToString(HandActionType type)
        {
            switch (type)
            {
                case HandActionType.FOLD:
                    return "Folds";
                case HandActionType.CALL:
                    return "Calls";
                case HandActionType.CHECK:
                    return "Checks";
                case HandActionType.RAISE:
                    return "Raises";
                case HandActionType.BET:
                    return "Bets";
                case HandActionType.SMALL_BLIND:
                    return "Small Blind";
                case HandActionType.BIG_BLIND:
                    return "Big Blind";
                case HandActionType.ALL_IN:
                    return "All in";
                case HandActionType.POSTS:
                    return "Posts";
                case HandActionType.WINS:
                case HandActionType.WINS_SIDE_POT:
                case HandActionType.WINS_THE_LOW:
                case HandActionType.WINS_TOURNAMENT:
                    return "Wins";
            }

            return null;
        }

        public static EnumFacingPreflop ToFacingPreflop(IEnumerable<HandAction> preflopHandActions, string playerName)
        {
            HandAction firstPlayerAction = preflopHandActions.FirstOrDefault(x => x.PlayerName == playerName
                                                                && x.HandActionType != HandActionType.SMALL_BLIND
                                                                && x.HandActionType != HandActionType.BIG_BLIND);
            if (firstPlayerAction == null)
            {
                return EnumFacingPreflop.None;
            }

            int indexOfFirstPlayerAction = preflopHandActions.ToList().IndexOf(firstPlayerAction);

            IEnumerable<HandAction> actions = preflopHandActions
                                                    .Take(indexOfFirstPlayerAction)
                                                    .Where(x => x.HandActionType != HandActionType.SMALL_BLIND
                                                            && x.HandActionType != HandActionType.BIG_BLIND);

            if (actions.Any(x => x.IsRaise()))
            {
                switch (actions.Count(x => x.IsRaise()))
                {
                    case 1:
                        int indexOfRaiseAction = actions.ToList().IndexOf(actions.First(x => x.IsRaise()));
                        var postRaiseActions = actions.Skip(indexOfRaiseAction);
                        if (postRaiseActions.Any(x => x.IsCall()))
                        {
                            if (postRaiseActions.Count(x => x.IsCall()) > 1)
                            {
                                return EnumFacingPreflop.MultipleCallers;
                            }
                            else
                            {
                                return EnumFacingPreflop.RaiserAndCaller;
                            }
                        }
                        else
                        {
                            return EnumFacingPreflop.Raiser;
                        }
                    case 2:
                        return EnumFacingPreflop.ThreeBet;
                    case 3:
                        return EnumFacingPreflop.FourBet;
                    case 4:
                        return EnumFacingPreflop.FiveBet;
                }
            }

            int limpersNumber = actions.Where(x => x.HandActionType == HandActionType.CHECK
                                                || x.HandActionType == HandActionType.CALL).Count();
            if (limpersNumber > 0)
            {
                if (limpersNumber > 1)
                {
                    return EnumFacingPreflop.MultipleLimpers;
                }
                else
                {
                    return EnumFacingPreflop.Limper;
                }
            }

            return EnumFacingPreflop.Unopened;
        }

        public static STTTypes ToSitNGoType(string tournamentName)
        {
            if (tournamentName.IndexOf("Beginner", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return STTTypes.Beginner;
            }

            if (tournamentName.IndexOf("Double-Up", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return STTTypes.DoubleUp;
            }

            if (tournamentName.IndexOf("Triple-Up", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return STTTypes.TripleUp;
            }

            if (tournamentName.IndexOf("1-Up", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return STTTypes.DoubleUp;
            }

            return STTTypes.Normal;
        }

        /// <summary>
        /// Converts the provided utc date time value to local time using <see cref="GeneralSettingsModel.TimeZoneOffset"/> setting
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <returns></returns>
        public static DateTime ToLocalizedDateTime(DateTime utcDateTime)
        {
            var offset = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings.TimeZoneOffset;
            return utcDateTime.AddHours(offset);
        }
    }
}