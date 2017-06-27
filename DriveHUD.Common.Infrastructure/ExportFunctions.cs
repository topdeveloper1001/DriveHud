//-----------------------------------------------------------------------
// <copyright file="ExportFunctions.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Extensions;
using Model.Importer;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DriveHUD.Common.Ifrastructure
{
    public static class ExportFunctions
    {
        private static Dictionary<int, Dictionary<EnumPosition, string>> positionTable = new Dictionary<int, Dictionary<EnumPosition, string>>
        {
            // 5-handed table
            [5] = new Dictionary<EnumPosition, string>
            {
                [EnumPosition.MP1] = "HJ"
            },
            // 6-handed table
            [6] = new Dictionary<EnumPosition, string>
            {
                [EnumPosition.UTG] = "UTG",
                [EnumPosition.MP1] = "HJ"
            },
            // 7-handed table
            [7] = new Dictionary<EnumPosition, string>
            {
                [EnumPosition.UTG_2] = "UTG",
                [EnumPosition.MP1] = "MP",
                [EnumPosition.MP2] = "HJ"
            },
            // 8-handed table
            [8] = new Dictionary<EnumPosition, string>
            {
                [EnumPosition.UTG_1] = "UTG",
                [EnumPosition.UTG_2] = "MP",
                [EnumPosition.MP1] = "MP",
                [EnumPosition.MP2] = "HJ"
            },
            // 9-handed table
            [9] = new Dictionary<EnumPosition, string>
            {
                [EnumPosition.UTG] = "UTG",
                [EnumPosition.UTG_1] = "EP",
                [EnumPosition.UTG_2] = "MP",
                [EnumPosition.MP1] = "MP",
                [EnumPosition.MP2] = "HJ"
            },
            // 10-handed table
            [10] = new Dictionary<EnumPosition, string>
            {
                [EnumPosition.UTG] = "UTG",
                [EnumPosition.UTG_1] = "EP",
                [EnumPosition.UTG_2] = "EP",
                [EnumPosition.MP1] = "MP",
                [EnumPosition.MP2] = "MP",
                [EnumPosition.MP3] = "HJ"
            }
        };

        #region Export Functions

        public static string ExportHand(long gamenumber, short pokerSiteId, EnumExportType exportType, bool isSetClipboard = false)
        {
            var resultString = string.Empty;

            try
            {
                var handHistory = ServiceLocator.Current.GetInstance<IDataService>().GetGame(gamenumber, pokerSiteId);

                switch (exportType)
                {
                    case EnumExportType.TwoPlusTwo:
                    case EnumExportType.CardsChat:
                    case EnumExportType.PokerStrategy:
                        resultString = ConvertHHToForumFormat(handHistory);
                        break;
                    case EnumExportType.PlainText:
                        resultString = ConvertHHToPlainText(handHistory);
                        break;
                    case EnumExportType.Raw:
                    default:
                        resultString = handHistory.FullHandHistoryText;
                        break;
                }

                if (isSetClipboard)
                {
                    Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(resultString));
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }

            return resultString;
        }

        public static string ConvertHHToForumFormat(HandHistory currentHandHistory)
        {
            try
            {
                if (currentHandHistory == null)
                {
                    LogProvider.Log.Info("ConvertHHToForumFormat: Cannot find handHistory");
                    return string.Empty;
                }

                HandHistories.Objects.Players.Player heroPlayer = null;
                StringBuilder res = new StringBuilder();
                String title = "NL Holdem $" + currentHandHistory.GameDescription.Limit.BigBlind + "(BB)";
                res.AppendLine("Hand History driven straight to this forum with DriveHUD [url=http://drivehud.com]Poker HUD & Database[/url]");
                res.Append(Environment.NewLine);
                res.AppendLine(title);

                if (currentHandHistory.Hero != null)
                {
                    heroPlayer = currentHandHistory.Hero;
                }

                //SHOW PLAYERS POSITIONS + STARTING STACK
                foreach (var player in currentHandHistory.Players)
                {
                    String playerName;
                    if (heroPlayer != null && player == heroPlayer)
                        playerName = "HERO";
                    else
                        playerName = GetPositionName(player.PlayerName, currentHandHistory);
                    res.AppendLine(String.Format("{0} (${1})",
                                SurroundWithColorIfInHand(playerName, currentHandHistory.HandActions.Any(x => x.Street == Street.Flop && x.PlayerName == player.PlayerName)),
                                player.StartingStack));
                }
                res.AppendLine("");

                //CARDS DEALT TO HERO
                var heroCards = (heroPlayer != null) && heroPlayer.hasHoleCards ? string.Join(" ", heroPlayer.HoleCards.Select(x => ConvertCardToForumFormat(x.ToString()))) : string.Empty;
                res.AppendLine($"Dealt to Hero {heroCards}");
                res.AppendLine("");

                decimal pot = currentHandHistory.HandActions.Where(x => x.IsBlinds).Sum(x => Math.Abs(x.Amount));
                Tuple<string, decimal> actionStringResult = GetActionString(heroPlayer?.PlayerName, currentHandHistory.PreFlop, currentHandHistory, true);
                res.AppendLine(actionStringResult.Item1);
                res.AppendLine();
                pot += actionStringResult.Item2;

                if (currentHandHistory.Flop.Any())
                {
                    res.AppendLine(GetStreetString(Street.Flop, pot, currentHandHistory));
                    actionStringResult = GetActionString(heroPlayer?.PlayerName, currentHandHistory.Flop, currentHandHistory);
                    res.AppendLine(actionStringResult.Item1);
                    res.AppendLine();
                    pot += actionStringResult.Item2;
                }
                else if (CardHelper.IsStreetAvailable(currentHandHistory.CommunityCards.ToString(), Street.Flop))
                {
                    res.AppendLine(GetStreetString(Street.Flop, pot, currentHandHistory));
                    res.AppendLine();
                }

                if (currentHandHistory.Turn.Any())
                {
                    res.AppendLine(GetStreetString(Street.Turn, pot, currentHandHistory));
                    actionStringResult = GetActionString(heroPlayer?.PlayerName, currentHandHistory.Turn, currentHandHistory);
                    res.AppendLine(actionStringResult.Item1);
                    res.AppendLine();
                    pot += actionStringResult.Item2;
                }
                else if (CardHelper.IsStreetAvailable(currentHandHistory.CommunityCards.ToString(), Street.Turn))
                {
                    res.AppendLine(GetStreetString(Street.Turn, pot, currentHandHistory));
                    res.AppendLine();
                }

                if (currentHandHistory.River.Any())
                {
                    res.AppendLine(GetStreetString(Street.River, pot, currentHandHistory));
                    actionStringResult = GetActionString(heroPlayer?.PlayerName, currentHandHistory.River, currentHandHistory);
                    res.AppendLine(actionStringResult.Item1);
                    res.AppendLine();
                    pot += actionStringResult.Item2;
                }
                else if (CardHelper.IsStreetAvailable(currentHandHistory.CommunityCards.ToString(), Street.River))
                {
                    res.AppendLine(GetStreetString(Street.River, pot, currentHandHistory));
                    res.AppendLine();
                }

                //PLAYER SHOWED CARDS
                foreach (var player in currentHandHistory.Players)
                {
                    if (player != null && player.hasHoleCards && player != heroPlayer)
                    {
                        var playerCards = string.Join(" ", player.HoleCards.Select(x => ConvertCardToForumFormat(x.ToString())));
                        var positionName = GetPositionName(player.PlayerName, currentHandHistory);
                        res.AppendLine($"{positionName} shows {playerCards}");
                        res.Append(Environment.NewLine);
                    }
                }

                //WINNERS
                foreach (var winAction in currentHandHistory.WinningActions)
                {
                    String playerName;
                    HandHistories.Objects.Players.Player player = currentHandHistory.Players.First(x => x.PlayerName == winAction.PlayerName);
                    if (heroPlayer != null && player == heroPlayer)
                    {
                        playerName = "HERO";
                    }
                    else
                    {
                        playerName = GetPositionName(player.PlayerName, currentHandHistory);
                    }
                    res.AppendLine(playerName + " wins $" + winAction.Amount);
                }

                return res.ToString();
            }
            catch (Exception exc)
            {
                LogProvider.Log.Error("ExportFunctions", String.Format("HandID: {0}", currentHandHistory.HandId), exc);
            }
            return "";
        }

        public static string ConvertHHToPlainText(HandHistory currentHandHistory)
        {
            try
            {
                if (currentHandHistory == null)
                {
                    LogProvider.Log.Info("ConvertHHToPlainText: Cannot find handHistory");
                    return string.Empty;
                }

                HandHistories.Objects.Players.Player heroPlayer = null;
                StringBuilder res = new StringBuilder();
                String title = "NL Holdem $" + currentHandHistory.GameDescription.Limit.BigBlind + "(BB)";
                res.AppendLine("Hand History driven straight to this forum by DriveHUD - http://drivehud.com");
                res.Append(Environment.NewLine);
                res.AppendLine(title);

                if (currentHandHistory.Hero != null)
                {
                    heroPlayer = currentHandHistory.Hero;
                }

                //SHOW PLAYERS POSITIONS + STARTING STACK
                foreach (var player in currentHandHistory.Players)
                {
                    String playerName;
                    if (heroPlayer != null && player == heroPlayer)
                        playerName = "HERO";
                    else
                        playerName = GetPositionName(player.PlayerName, currentHandHistory);
                    res.AppendLine($"{playerName} (${player.StartingStack})");
                }
                res.AppendLine("");

                //CARDS DEALT TO HERO
                var heroCards = (heroPlayer != null) && heroPlayer.hasHoleCards ? string.Join(" ", heroPlayer.HoleCards.Select(x => x.ToString())) : string.Empty;
                res.AppendLine($"Dealt to Hero {heroCards}");
                res.AppendLine("");

                decimal pot = currentHandHistory.HandActions.Where(x => x.IsBlinds).Sum(x => Math.Abs(x.Amount));
                Tuple<string, decimal> actionStringResult = GetPlainActionString(heroPlayer?.PlayerName, currentHandHistory.PreFlop, currentHandHistory, true);
                res.AppendLine(actionStringResult.Item1);
                res.AppendLine();
                pot += actionStringResult.Item2;

                if (currentHandHistory.Flop.Any())
                {
                    res.AppendLine(GetPlainStreetString(Street.Flop, pot, currentHandHistory));
                    actionStringResult = GetPlainActionString(heroPlayer?.PlayerName, currentHandHistory.Flop, currentHandHistory);
                    res.AppendLine(actionStringResult.Item1);
                    res.AppendLine();
                    pot += actionStringResult.Item2;
                }
                else if (CardHelper.IsStreetAvailable(currentHandHistory.CommunityCards.ToString(), Street.Flop))
                {
                    res.AppendLine(GetPlainStreetString(Street.Flop, pot, currentHandHistory));
                    res.AppendLine();
                }

                if (currentHandHistory.Turn.Any())
                {
                    res.AppendLine(GetPlainStreetString(Street.Turn, pot, currentHandHistory));
                    actionStringResult = GetPlainActionString(heroPlayer?.PlayerName, currentHandHistory.Turn, currentHandHistory);
                    res.AppendLine(actionStringResult.Item1);
                    res.AppendLine();
                    pot += actionStringResult.Item2;
                }
                else if (CardHelper.IsStreetAvailable(currentHandHistory.CommunityCards.ToString(), Street.Turn))
                {
                    res.AppendLine(GetPlainStreetString(Street.Turn, pot, currentHandHistory));
                    res.AppendLine();
                }

                if (currentHandHistory.River.Any())
                {
                    res.AppendLine(GetPlainStreetString(Street.River, pot, currentHandHistory));
                    actionStringResult = GetPlainActionString(heroPlayer?.PlayerName, currentHandHistory.River, currentHandHistory);
                    res.AppendLine(actionStringResult.Item1);
                    res.AppendLine();
                    pot += actionStringResult.Item2;
                }
                else if (CardHelper.IsStreetAvailable(currentHandHistory.CommunityCards.ToString(), Street.River))
                {
                    res.AppendLine(GetPlainStreetString(Street.River, pot, currentHandHistory));
                    res.AppendLine();
                }

                //PLAYER SHOWED CARDS
                foreach (var player in currentHandHistory.Players)
                {
                    if (player != null && player.hasHoleCards && player != heroPlayer)
                    {
                        var playerCards = string.Join(" ", player.HoleCards.Select(x => x.ToString()));
                        var positionName = GetPositionName(player.PlayerName, currentHandHistory);
                        res.AppendLine($"{positionName} shows {playerCards}");
                        res.Append(Environment.NewLine);
                    }
                }

                //WINNERS
                foreach (var winAction in currentHandHistory.WinningActions)
                {
                    String playerName;
                    HandHistories.Objects.Players.Player player = currentHandHistory.Players.First(x => x.PlayerName == winAction.PlayerName);
                    if (heroPlayer != null && player == heroPlayer)
                    {
                        playerName = "HERO";
                    }
                    else
                    {
                        playerName = GetPositionName(player.PlayerName, currentHandHistory);
                    }
                    res.AppendLine(playerName + " wins $" + winAction.Amount);
                }

                return res.ToString();
            }
            catch (Exception exc)
            {
                LogProvider.Log.Error("ExportFunctions", String.Format("HandID: {0}", currentHandHistory.HandId), exc);
            }
            return "";
        }

        private static String SurroundWithColorIfInHand(string player, bool isInHand)
        {
            if (isInHand) return "[color=red]" + player + "[/color]";
            return player;
        }

        public static string GetEquityDataToExport(string boardCards, IEnumerable<string> playersEquityStrings)
        {
            //Ace Poker Drills Equity Calculator (hyper link to: http://acepokerdrills.com):

            //Board: Ks Jc 9d

            //equity          win              tie          Hand Range 
            //Player 0:     64.662%      63.42%     01.24%        [ JJ, 97s ]
            //Player 1:     35.338%      34.10%     01.24%        [ T9s ]

            StringBuilder sb = new StringBuilder("This hand driven to you directly from DriveHUD [url=http://drivehud.com]Poker HUD & Database[/url]");

            sb.Append(Environment.NewLine);
            if (boardCards != null)
            {
                sb.AppendLine("Board: " + boardCards);
            }
            else
            {
                sb.AppendLine("Board: ");
            }
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.AppendLine(String.Format("{0,-10}        {1,-10}        {2,-10}        {3,-10}", "Equity", "Win", "Tie", "Hand Range"));
            sb.Append(Environment.NewLine);

            foreach (var equityString in playersEquityStrings)
            {
                sb.AppendLine(equityString);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        private static String ConvertCardToForumFormat(String card)
        {
            String suit = card[1] == 'h' ? "heart" : card[1] == 's' ? "spade" : card[1] == 'd' ? "diamond" : "club";
            return card[0] + ":" + suit + ":";
        }

        private static string GetPositionName(string playerName, HandHistory hand)
        {
            var position = Converter.ToPosition(hand, playerName);

            switch (position)
            {
                case EnumPosition.SB:
                    return "SB";
                case EnumPosition.BB:
                    return "BB";
                case EnumPosition.BTN:
                    return "BTN";
                case EnumPosition.CO:
                    return "CO";
                default:
                    var tableSize = hand.HandActions.Select(x => x.PlayerName).Distinct().Count();

                    if (positionTable.ContainsKey(tableSize) && positionTable[tableSize].ContainsKey(position))
                    {
                        return positionTable[tableSize][position];
                    }

                    return position.ToString();
            }
        }

        private static bool IsFolded(string playerName, IEnumerable<HandAction> actions)
        {
            return actions.Any(x => x.PlayerName == playerName && x.HandActionType == HandActionType.FOLD);
        }

        private static Tuple<string, decimal> GetActionString(string heroName, IEnumerable<HandAction> actions, HandHistory currentHandHistory, bool isPreflop = false)
        {
            decimal pot = 0;
            string resultString = string.Empty;

            foreach (var action in actions)
            {
                if (action.IsBlinds) continue;

                pot += Math.Abs(action.Amount);
                bool allIn = action.IsAllInAction;
                var actionPlayer = currentHandHistory.Players.First(x => x.PlayerName == action.PlayerName);
                string name = !string.IsNullOrWhiteSpace(heroName) && actionPlayer.PlayerName == heroName ? "HERO" : GetPositionName(actionPlayer.PlayerName, currentHandHistory);
                string playerActionString = GetPlayersActionString(action, GetRemainingStack(action.PlayerName, action, currentHandHistory));
                string composedString = String.Format("{0} {1}", name, playerActionString);
                bool isInHand = !IsFolded(actionPlayer.PlayerName, actions) && action.HandActionType != HandActionType.CHECK;
                resultString += SurroundWithColorIfInHand(composedString, isInHand) + ", ";
            }

            return new Tuple<string, decimal>(resultString.Trim().Trim(','), pot);
        }

        private static Tuple<string, decimal> GetPlainActionString(string heroName, IEnumerable<HandAction> actions, HandHistory currentHandHistory, bool isPreflop = false)
        {
            decimal pot = 0;
            string resultString = string.Empty;

            foreach (var action in actions)
            {
                if (action.IsBlinds) continue;

                pot += Math.Abs(action.Amount);
                bool allIn = action.IsAllInAction;
                var actionPlayer = currentHandHistory.Players.First(x => x.PlayerName == action.PlayerName);
                string name = !string.IsNullOrWhiteSpace(heroName) && actionPlayer.PlayerName == heroName ? "HERO" : GetPositionName(actionPlayer.PlayerName, currentHandHistory);
                string playerActionString = GetPlayersActionString(action, GetRemainingStack(action.PlayerName, action, currentHandHistory));
                string composedString = String.Format("{0} {1}", name, playerActionString);
                bool isInHand = !IsFolded(actionPlayer.PlayerName, actions) && action.HandActionType != HandActionType.CHECK;
                resultString += composedString + ", ";
            }

            return new Tuple<string, decimal>(resultString.Trim().Trim(','), pot);
        }

        private static string GetPlayersActionString(HandAction action, decimal remainingStack, bool isPreflop = false)
        {
            string resultString = string.Empty;
            switch (action.HandActionType)
            {
                case HandActionType.SMALL_BLIND:
                    resultString = "SB";
                    break;
                case HandActionType.BIG_BLIND:
                    resultString = "BB";
                    break;
                case HandActionType.FOLD:
                    return resultString = "Folds";
                case HandActionType.CHECK:
                    return resultString = "Checks";
                case HandActionType.CALL:
                    resultString = "Calls";
                    break;
                case HandActionType.RAISE:
                    resultString = "Raises To";
                    break;
                case HandActionType.BET:
                    resultString = "Bets";
                    break;
                case HandActionType.POSTS:
                    resultString = "Posts Ante";
                    break;
            }

            if (isPreflop)
            {
                resultString = String.Format("{0} $({1})", resultString, Math.Abs(action.Amount));
            }
            else
            {
                resultString = String.Format("{0} ${1}", resultString, Math.Abs(action.Amount));
            }
            if (action.IsAllInAction)
            {
                resultString = String.Format("{0} (allin)", resultString);
            }
            if (action.IsAggressiveAction || action.IsCall)
            {
                resultString = String.Format("{0} (Rem. Stack: {1})", resultString, remainingStack);
            }

            return resultString;
        }

        private static decimal GetRemainingStack(string playerName, HandAction action, HandHistory currentHandHistory)
        {
            var playerActions = currentHandHistory.HandActions.Take(currentHandHistory.HandActions.ToList().IndexOf(action) + 1).Where(x => x.PlayerName == playerName);
            var startingStack = currentHandHistory.Players.FirstOrDefault(x => x.PlayerName == playerName)?.StartingStack ?? 0;
            return startingStack == 0 ? 0 : startingStack - playerActions.Sum(x => Math.Abs(x.Amount));
        }

        private static string GetStreetString(Street street, decimal pot, HandHistory currentHandHistory)
        {
            string resultString = String.Format("{0} (${1}) ", street, pot);
            foreach (var card in currentHandHistory.CommunityCards.GetBoardOnStreet(street))
            {
                resultString += ConvertCardToForumFormat(card.CardStringValue) + " ";
            }

            return resultString.Trim();
        }

        private static string GetPlainStreetString(Street street, decimal pot, HandHistory currentHandHistory)
        {
            string resultString = String.Format("{0} (${1}) ", street, pot);
            foreach (var card in currentHandHistory.CommunityCards.GetBoardOnStreet(street))
            {
                resultString += card.CardStringValue + " ";
            }

            return resultString.Trim();
        }
        #endregion
    }
}
