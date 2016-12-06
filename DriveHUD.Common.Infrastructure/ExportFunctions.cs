using DriveHUD.Common.Log;
using HandHistories.Objects.Hand;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using Model.Extensions;
using DriveHUD.Common.Linq;
using Model.Enums;
using Microsoft.Practices.ServiceLocation;
using Model.Interfaces;
using System.Windows;

namespace DriveHUD.Common.Ifrastructure
{
    public static class ExportFunctions
    {
        private static List<String[]> positionNames = new List<string[]>();

        private static void InitPositionNames()
        {
            positionNames.Add(new String[] { }); // 0-handed table
            positionNames.Add(new String[] { }); // 1-handed table
            positionNames.Add(new String[] { "SB", "BB" }); // 2-handed table
            positionNames.Add(new String[] { "SB", "BB", "BTN" }); // 3-handed table
            positionNames.Add(new String[] { "SB", "BB", "CO", "BTN" }); // 4-handed table
            positionNames.Add(new String[] { "SB", "BB", "HJ", "CO", "BTN" }); // 5-handed table
            positionNames.Add(new String[] { "SB", "BB", "UTG", "HJ", "CO", "BTN" }); // 6-handed table
            positionNames.Add(new String[] { "SB", "BB", "UTG", "MP", "HJ", "CO", "BTN" }); // 7-handed table
            positionNames.Add(new String[] { "SB", "BB", "UTG", "MP", "MP", "HJ", "CO", "BTN" }); // 8-handed table
            positionNames.Add(new String[] { "SB", "BB", "UTG", "EP", "MP", "MP", "HJ", "CO", "BTN" }); // 9-handed table
            positionNames.Add(new String[] { "SB", "BB", "UTG", "EP", "EP", "MP", "MP", "HJ", "CO", "BTN" }); // 10-handed table
            positionNames.Add(new String[] { "SB", "BB", "UTG", "EP", "EP", "EP", "MP", "MP", "HJ", "CO", "BTN" }); // 11-handed table (should not exists in online poker)
        }

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
                    case EnumExportType.Raw:
                    default:
                        resultString = handHistory.FullHandHistoryText;
                        break;
                }

                if (isSetClipboard)
                {
                    Clipboard.SetText(handHistory.FullHandHistoryText);
                }
            }
            catch(Exception ex)
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

            StringBuilder sb = new StringBuilder("Ace Poker Drills [url=http://acepokerdrills.com]Poker Equity Calculator[/url]");



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

        private static string GetPositionName(string playerName, HandHistory currentHandHistory)
        {
            if (positionNames.Count == 0) InitPositionNames();
            //int posToCheck = (PositionIndex == -1) ? 0 : PositionIndex;

            var playersList = currentHandHistory.PreFlop.Select(x => x.PlayerName).Distinct();
            if (!playersList.Contains(playerName))
                return string.Empty;

            int positionIndex = playersList.ToList().IndexOf(playersList.First(x => x == playerName));

            int posToCheck = positionIndex;
            int nbPlayers = currentHandHistory.Players.Count;
            if (currentHandHistory.PreFlop.Count() > 2 && currentHandHistory.PreFlop.First().HandActionType == HandActionType.BIG_BLIND)
            {
                posToCheck++;
                nbPlayers++;
            }
            return positionNames[nbPlayers][posToCheck];
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
        #endregion
    }
}
