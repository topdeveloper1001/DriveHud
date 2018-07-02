//-----------------------------------------------------------------------
// <copyright file="ExportFunctions.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
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
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Data;
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
        private const string HeroName = "HERO";

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
                [EnumPosition.UTG_2] = "EP",
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
                    case EnumExportType.PlainText:
                        resultString = ConvertHHToForumFormat(handHistory, exportType);
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

        public static string ConvertHHToForumFormat(HandHistory handHistory, EnumExportType exportType, bool withStats = true)
        {
            if (handHistory == null || handHistory.GameDescription == null)
            {
                LogProvider.Log.Error(typeof(ExportFunctions), $"{nameof(ConvertHHToForumFormat)}: Could not export empty hand.");
                return string.Empty;
            }

            var result = new StringBuilder();

            try
            {
                IDictionary<string, ExportIndicators> playerIndicators = null;
                var playersNames = new Dictionary<string, string>();

                if (withStats)
                {
                    var playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
                    var playerNames = handHistory.Players.Select(x => x.PlayerName).ToArray();

                    playerIndicators = playerStatisticRepository.GetPlayersIndicators<ExportIndicators>(playerNames, (short)handHistory.GameDescription.Site);
                }

                result.AppendLine("Hand History driven straight to this forum with DriveHUD [url=http://drivehud.com/?t=hh]Poker Tracking[/url] Software");
                result.AppendLine();
                result.AppendLine($"[U][B]{ConvertGameType(handHistory)} ${handHistory.GameDescription.Limit.BigBlind}(BB)[/B][/U]");

                // players
                foreach (var player in handHistory.Players)
                {
                    var playerName = handHistory.Hero != null && handHistory.Hero == player ?
                        HeroName :
                        GetPositionName(player.PlayerName, handHistory);

                    if (!playersNames.ContainsKey(player.PlayerName))
                    {
                        playersNames.Add(player.PlayerName, playerName);
                    }

                    var isInPot = handHistory.HandActions.Any(x => x.Street == Street.Flop && x.PlayerName == player.PlayerName);

                    var beginFormatTag = string.Empty;
                    var endFormatTag = string.Empty;

                    if (isInPot)
                    {
                        beginFormatTag = "[color=blue][B]";
                        endFormatTag = "[/B][/color]";
                    }

                    result.Append($"{beginFormatTag}{playerName} (${player.StartingStack}){endFormatTag}");

                    if (playerIndicators != null && playerIndicators.ContainsKey(player.PlayerName))
                    {
                        var indicator = playerIndicators[player.PlayerName];

                        if (isInPot)
                        {
                            beginFormatTag = "[B]";
                            endFormatTag = "[/B]";
                        }

                        result.Append($" {beginFormatTag}[VPIP: {indicator.VPIP:0.#}% | PFR: {indicator.PFR:0.#}% | AGG: {indicator.AggPr:0.#}% | 3-Bet: {indicator.ThreeBet:0.#}% | Hands: {indicator.TotalHands}]{endFormatTag}");
                    }

                    result.Append(Environment.NewLine);
                }

                result.AppendLine();

                // dealt to 
                if (handHistory.Hero != null && handHistory.Hero.hasHoleCards)
                {
                    result.AppendLine($"[B]Dealt to Hero:[/B] {string.Join(" ", handHistory.Hero.HoleCards.Select(x => ConvertCardToForumFormat(x.ToString())))}");
                    result.AppendLine();
                }

                var pot = handHistory.HandActions.Where(x => x.IsBlinds).Sum(x => Math.Abs(x.Amount));

                // street headers and hand actions
                result.AppendLine(BuildActionLine(handHistory.PreFlop, handHistory, playersNames, ref pot));

                foreach (var street in new[] { Street.Flop, Street.Turn, Street.River })
                {
                    if (CardHelper.IsStreetAvailable(handHistory.CommunityCards, street))
                    {
                        result.AppendLine();

                        // if hero presented then build spr line
                        if (street == Street.Flop && handHistory.Hero != null && handHistory.Flop.Any(x => x.PlayerName == handHistory.Hero.PlayerName))
                        {
                            result.AppendLine(BuildSPROnFlopLine(handHistory, pot));
                        }

                        result.AppendLine(GetStreetString(street, pot, handHistory));

                        var streetActions = handHistory.HandActions.Where(x => x.Street == street);

                        if (streetActions.Any())
                        {
                            result.AppendLine(BuildActionLine(streetActions, handHistory, playersNames, ref pot));
                        }
                    }
                }

                result.AppendLine();

                var spoilTag = exportType == EnumExportType.CardsChat ? "SPOILER" : "spoil";

                result.AppendLine($"[{spoilTag}]");

                var doAppendLine = false;

                // show cards
                foreach (var player in handHistory.Players.Where(x => x.hasHoleCards && x != handHistory.Hero))
                {
                    if (!doAppendLine)
                    {
                        doAppendLine = true;
                    }

                    result.AppendLine($"{playersNames[player.PlayerName]} shows: {string.Join(" ", player.HoleCards.Select(x => ConvertCardToForumFormat(x.CardStringValue)))}");
                }

                if (doAppendLine)
                {
                    result.AppendLine();
                }

                // winners
                foreach (var winAction in handHistory.WinningActions)
                {
                    result.AppendLine($"{playersNames[winAction.PlayerName]} wins: ${winAction.Amount}");
                }

                result.AppendLine($"[/{spoilTag}]");
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(typeof(ExportFunctions), $"Could not export hand {handHistory.HandId} {handHistory.GameDescription.Site} in {exportType} format with stats = {withStats}", e);
            }

            var handHistoryText = result.ToString().Trim();

            return exportType == EnumExportType.PlainText ?
                ConvertToPlainText(handHistoryText) :
                handHistoryText;
        }

        private static string ConvertGameType(HandHistory handHistory)
        {
            switch (handHistory.GameDescription.GameType)
            {
                case GameType.FixedLimitHoldem:
                    return "FL Holdem";
                case GameType.FixedLimitOmaha:
                    return "FL Omaha";
                case GameType.FixedLimitOmahaHiLo:
                    return "FL Omaha HiLo";
                case GameType.NoLimitOmaha:
                    return "NL Omaha";
                case GameType.NoLimitOmahaHiLo:
                    return "NL Omaha HiLo";
                case GameType.PotLimitHoldem:
                    return "PL Holdem";
                case GameType.CapPotLimitOmaha:
                case GameType.FiveCardPotLimitOmaha:
                case GameType.PotLimitOmaha:
                    return "PL Omaha";
                case GameType.FiveCardPotLimitOmahaHiLo:
                case GameType.PotLimitOmahaHiLo:
                    return "PL Omaha HiLo";
                default:
                    return "NL Holdem";
            }
        }

        private static String SurroundWithTag(string input, bool condition, string beginTag, string endTag = null)
        {
            if (condition)
            {
                return $"[{beginTag}]{input}[/{endTag ?? beginTag}]";
            }

            return input;
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
                case EnumPosition.STRDL:
                    return "STRDL";
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
            return actions.Any(x => x.PlayerName == playerName && x.IsFold);
        }

        private static string BuildSPROnFlopLine(HandHistory handHistory, decimal pot)
        {
            var spr = 0m;

            var playersFoldedOnPreflop = handHistory.PreFlop.Where(x => x.IsFold).Select(x => x.PlayerName).ToArray();

            var smallestEffStackPLayer = handHistory.Players
                .Where(x => !playersFoldedOnPreflop.Contains(x.PlayerName))
                .OrderBy(x => x.StartingStack)
                .FirstOrDefault();

            if (smallestEffStackPLayer != null)
            {
                var playerPutInPreflop = Math.Abs(handHistory.PreFlop.Where(x => x.PlayerName == smallestEffStackPLayer.PlayerName).Sum(x => x.Amount));

                spr = pot != 0 ? (smallestEffStackPLayer.StartingStack - playerPutInPreflop) / pot : 0;
            }

            return $"[I][B]Hero SPR on Flop: [{spr:0.##} effective][/B][/I]";
        }

        private static string BuildActionLine(IEnumerable<HandAction> actions, HandHistory handHistory, Dictionary<string, string> playersNames, ref decimal pot)
        {
            var result = new StringBuilder();

            var putInThisStreet = new Dictionary<string, decimal>();

            var currentStreet = Street.Null;

            foreach (var action in actions)
            {
                if (currentStreet != action.Street)
                {
                    putInThisStreet.Clear();
                    currentStreet = action.Street;
                }

                if (!putInThisStreet.ContainsKey(action.PlayerName))
                {
                    putInThisStreet.Add(action.PlayerName, action.Amount);
                }
                else
                {
                    putInThisStreet[action.PlayerName] += action.Amount;
                }

                if (action.IsBlinds || action.HandActionType == HandActionType.UNCALLED_BET)
                {
                    continue;
                }

                pot += Math.Abs(action.Amount);

                if (!playersNames.TryGetValue(action.PlayerName, out string playerName))
                {
                    playerName = GetPositionName(action.PlayerName, handHistory);
                }

                var playerActionString = GetPlayersActionString(action, putInThisStreet[action.PlayerName]);

                var remainingStackString = action.Street != Street.Preflop && (action.IsAggressiveAction || action.IsCall) && !action.IsAllIn && !action.IsAllInAction ?
                     $" (Rem. Stack: {GetRemainingStack(action.PlayerName, action, handHistory)})" :
                     string.Empty;

                var isFolded = IsFolded(action.PlayerName, actions);
                var isInHand = !isFolded && !action.IsCheck;

                result.Append($"{SurroundWithTag($"{SurroundWithTag($"{playerName} {playerActionString}", isInHand, "color=red", "color")}{remainingStackString}", isFolded, "I")}, ");
            }

            return result.ToString().Trim(',', ' ');
        }

        private static string GetPlayersActionString(HandAction action, decimal putInThisStreet)
        {
            var resultString = string.Empty;

            var handActionType = action is AllInAction allInAction ?
                allInAction.SourceActionType :
                action.HandActionType;

            switch (handActionType)
            {
                case HandActionType.SMALL_BLIND:
                    resultString = "SB";
                    break;
                case HandActionType.BIG_BLIND:
                    resultString = "BB";
                    break;
                case HandActionType.FOLD:
                    return "Folds";
                case HandActionType.CHECK:
                    return "Checks";
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

            var amount = action.IsRaise() ? Math.Abs(putInThisStreet) : Math.Abs(action.Amount);
            var amountString = amount % 1 == 0 ? amount.ToString("F0") : amount.ToString("F2");

            resultString = $"{resultString} ${amountString}";

            if (action.IsAllInAction || action.IsAllIn)
            {
                return $"{resultString} (allin)";
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
            var cards = string.Join(" ",
                currentHandHistory.CommunityCards.GetBoardOnStreet(street).Select(x => ConvertCardToForumFormat(x.CardStringValue)).ToArray());

            var resultString = $"[B]{street} (${pot}):[/B] {cards}";

            return resultString.Trim();
        }

        private static string ConvertToPlainText(string handHistory)
        {
            return handHistory
                .Replace("[U]", string.Empty)
                .Replace("[/U]", string.Empty)
                .Replace("[B]", string.Empty)
                .Replace("[/B]", string.Empty)
                .Replace("[I]", string.Empty)
                .Replace("[/I]", string.Empty)
                .Replace("[color=red]", string.Empty)
                .Replace("[color=blue]", string.Empty)
                .Replace("[/color]", string.Empty)
                .Replace("[url=http://drivehud.com/?t=hh]Poker Tracking[/url] Software", "Poker Tracking Software - http://drivehud.com")
                .Replace("[/url]", string.Empty)
                .Replace($"[spoil]{Environment.NewLine}", string.Empty)
                .Replace($"{Environment.NewLine}[/spoil]", string.Empty)
                .Replace($"[SPOILER]{Environment.NewLine}", string.Empty)
                .Replace($"{Environment.NewLine}[/SPOILER]", string.Empty)
                .Replace($":diamond:", "d")
                .Replace($":spade:", "s")
                .Replace($":club:", "c")
                .Replace($":heart:", "h");
        }

        #endregion
    }
}