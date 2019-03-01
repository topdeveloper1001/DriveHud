//-----------------------------------------------------------------------
// <copyright file="Converter.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HoldemHand;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Importer
{
    public static class Converter
    {
        private static List<EnumPosition[]> PositionList = new List<EnumPosition[]>
        {
            new EnumPosition[2] { EnumPosition.SB, EnumPosition.BB },
            new EnumPosition[3] { EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB },
            new EnumPosition[4] { EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB,},
            new EnumPosition[5] { EnumPosition.MP1, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB},
            new EnumPosition[6] { EnumPosition.UTG, EnumPosition.MP1, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB},
            new EnumPosition[7] { EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB },
            new EnumPosition[8] { EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB },
            new EnumPosition[9] { EnumPosition.UTG, EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB },
            new EnumPosition[10] { EnumPosition.UTG, EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.MP3, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB }
        };

        private static List<EnumPosition[]> PKShortDeckPositionList = new List<EnumPosition[]>
        {
            new EnumPosition[2] { EnumPosition.CO, EnumPosition.BTN },
            new EnumPosition[3] { EnumPosition.MP1, EnumPosition.CO, EnumPosition.BTN },
            new EnumPosition[4] { EnumPosition.UTG, EnumPosition.MP1, EnumPosition.CO, EnumPosition.BTN, },
            new EnumPosition[5] { EnumPosition.UTG, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN},
            new EnumPosition[6] { EnumPosition.UTG, EnumPosition.UTG_1, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN},
            new EnumPosition[7] { EnumPosition.UTG, EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN },
            new EnumPosition[8] { EnumPosition.UTG, EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.MP3, EnumPosition.CO, EnumPosition.BTN },
            new EnumPosition[9] { EnumPosition.UTG, EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.MP3, EnumPosition.CO, EnumPosition.BTN },
            new EnumPosition[10] { EnumPosition.UTG, EnumPosition.UTG, EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.MP3, EnumPosition.CO, EnumPosition.BTN }
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
            return ToPosition(hand, stat?.PlayerName, stat);
        }

        public static EnumPosition ToPosition(HandHistory hand, string playerName)
        {
            return ToPosition(hand, playerName, null);
        }

        public static EnumPosition ToPosition(HandHistory hand, string playerName, Playerstatistic stat = null)
        {
            if (stat != null && stat.IsDealer)
            {
                return EnumPosition.BTN;
            }
            else if (stat != null && stat.IsSmallBlind)
            {
                return EnumPosition.SB;
            }
            else if (stat != null && stat.IsBigBlind)
            {
                return EnumPosition.BB;
            }
            else if (stat != null && stat.IsStraddle)
            {
                return EnumPosition.STRDL;
            }

            var tableSize = hand.HandActions.Select(x => x.PlayerName).Distinct().Count();

            var table = PositionList.FirstOrDefault(x => x.Count() == tableSize);

            var handActions = hand.HandActions
                .Where(x => x.HandActionType != HandActionType.ANTE).ToList();

            var firstPlayerAction = handActions
                .Where(x => x.HandActionType != HandActionType.SMALL_BLIND && x.HandActionType != HandActionType.BIG_BLIND && x.HandActionType != HandActionType.POSTS)
                .FirstOrDefault(x => x.PlayerName == playerName);

            int firstPlayerActionIndex;

            if (firstPlayerAction != null)
            {
                if (firstPlayerAction.HandActionType == HandActionType.STRADDLE)
                {
                    return EnumPosition.STRDL;
                }

                var blindActionsCount = handActions
                    .Where(x => x.HandActionType == HandActionType.SMALL_BLIND ||
                        x.HandActionType == HandActionType.BIG_BLIND ||
                        x.HandActionType == HandActionType.POSTS)
                    .Count();

                firstPlayerActionIndex = handActions.IndexOf(firstPlayerAction) - blindActionsCount;

                if (table != null && firstPlayerActionIndex >= 0 && firstPlayerActionIndex < table.Count())
                {
                    return table[firstPlayerActionIndex];
                }
            }

            // get position using button place
            var playersBeforeDealerPosition = new List<Player>();
            var playersAfterDealerPosition = new List<Player>();

            for (var i = 0; i < hand.Players.Count; i++)
            {
                if (i <= hand.DealerButtonPosition - 1)
                {
                    playersBeforeDealerPosition.Add(hand.Players[i]);
                }
                else
                {
                    playersAfterDealerPosition.Add(hand.Players[i]);
                }
            }

            var playersOrderedByPosition = playersAfterDealerPosition.Concat(playersAfterDealerPosition);

            firstPlayerActionIndex = playersOrderedByPosition.FindIndex(x => x.PlayerName == playerName) - 2;

            if (table != null && firstPlayerActionIndex >= 0 && firstPlayerActionIndex < table.Count())
            {
                return table[firstPlayerActionIndex];
            }

            // check ante
            var handAnteActions = hand.HandActions.Where(x => x.HandActionType == HandActionType.ANTE).ToList();

            firstPlayerAction = handAnteActions.FirstOrDefault(x => x.PlayerName == playerName);

            firstPlayerActionIndex = handAnteActions.IndexOf(firstPlayerAction) - handActions.Where(x => x.HandActionType == HandActionType.SMALL_BLIND).Take(1).Count() - handActions.Where(x => x.HandActionType == HandActionType.BIG_BLIND).Take(1).Count();

            if (table != null && firstPlayerActionIndex >= 0 && firstPlayerActionIndex < table.Count())
            {
                return table[firstPlayerActionIndex];
            }

            return EnumPosition.Undefined;
        }

        public static EnumPosition ToPKShortDeckPosition(HandHistory hand, string playerName, Playerstatistic stat = null)
        {
            if (stat != null && stat.IsDealer)
            {
                return EnumPosition.BTN;
            }
            else if (stat != null && stat.IsStraddle)
            {
                return EnumPosition.STRDL;
            }

            var tableSize = hand.HandActions.Select(x => x.PlayerName).Distinct().Count();

            var table = PKShortDeckPositionList.FirstOrDefault(x => x.Count() == tableSize);

            var handActions = hand.HandActions
                .Where(x => x.HandActionType != HandActionType.ANTE).ToList();

            var firstPlayerAction = handActions
                .Where(x => x.HandActionType != HandActionType.SMALL_BLIND && x.HandActionType != HandActionType.BIG_BLIND && x.HandActionType != HandActionType.POSTS)
                .FirstOrDefault(x => x.PlayerName == playerName);

            int firstPlayerActionIndex;

            if (firstPlayerAction != null)
            {
                if (firstPlayerAction.HandActionType == HandActionType.STRADDLE)
                {
                    return EnumPosition.STRDL;
                }

                var blindActionsCount = handActions
                    .Where(x => x.HandActionType == HandActionType.SMALL_BLIND ||
                        x.HandActionType == HandActionType.BIG_BLIND ||
                        x.HandActionType == HandActionType.POSTS)
                    .Count();

                firstPlayerActionIndex = handActions.IndexOf(firstPlayerAction) - blindActionsCount;

                if (table != null && firstPlayerActionIndex >= 0 && firstPlayerActionIndex < table.Count())
                {
                    return table[firstPlayerActionIndex];
                }
            }

            // get position using button place
            var playersBeforeDealerPosition = new List<Player>();
            var playersAfterDealerPosition = new List<Player>();

            for (var i = 0; i < hand.Players.Count; i++)
            {
                if (i <= hand.DealerButtonPosition - 1)
                {
                    playersBeforeDealerPosition.Add(hand.Players[i]);
                }
                else
                {
                    playersAfterDealerPosition.Add(hand.Players[i]);
                }
            }

            var playersOrderedByPosition = playersAfterDealerPosition.Concat(playersAfterDealerPosition);

            firstPlayerActionIndex = playersOrderedByPosition.FindIndex(x => x.PlayerName == playerName) - 1;

            if (table != null && firstPlayerActionIndex >= 0 && firstPlayerActionIndex < table.Count())
            {
                return table[firstPlayerActionIndex];
            }

            // check ante
            var handAnteActions = hand.HandActions.Where(x => x.HandActionType == HandActionType.ANTE).ToList();

            firstPlayerAction = handAnteActions.FirstOrDefault(x => x.PlayerName == playerName);

            firstPlayerActionIndex = handAnteActions.IndexOf(firstPlayerAction) - handActions.Where(x => x.HandActionType == HandActionType.BIG_BLIND).Take(1).Count();

            if (table != null && firstPlayerActionIndex >= 0 && firstPlayerActionIndex < table.Count())
            {
                return table[firstPlayerActionIndex];
            }

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
                case EnumPosition.STRDL:
                    return "STRDL";
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
            var wasAllinAction = hand.HandActions.FirstOrDefault(x => x.IsAllIn || x.IsAllInAction);

            if (wasAllinAction == null)
            {
                return string.Empty;
            }

            return wasAllinAction.Street.ToString();
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
                case HandActionType.STRADDLE:
                    return "Straddle";
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
            HandAction firstPlayerAction = preflopHandActions.FirstOrDefault(x => x.PlayerName == playerName && !x.IsBlinds);

            if (firstPlayerAction == null)
            {
                return EnumFacingPreflop.None;
            }

            int indexOfFirstPlayerAction = preflopHandActions.ToList().IndexOf(firstPlayerAction);

            IEnumerable<HandAction> actions = preflopHandActions
                                                    .Take(indexOfFirstPlayerAction)
                                                    .Where(x => !x.IsBlinds);

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
            if (string.IsNullOrEmpty(tournamentName))
            {
                return STTTypes.Normal;
            }

            if (tournamentName.IndexOf("Beginner", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return STTTypes.Beginner;
            }

            if (tournamentName.IndexOf("Double-Up", StringComparison.OrdinalIgnoreCase) >= 0 ||
                tournamentName.IndexOf("1-Up", StringComparison.OrdinalIgnoreCase) >= 0 ||
                tournamentName.IndexOf("One-Up", StringComparison.OrdinalIgnoreCase) >= 0 ||
                tournamentName.IndexOf("Double Or Nothing", StringComparison.OrdinalIgnoreCase) >= 0 ||
                tournamentName.IndexOf("DoubleUp", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return STTTypes.DoubleUp;
            }

            if (tournamentName.IndexOf("Triple-Up", StringComparison.OrdinalIgnoreCase) >= 0 ||
                tournamentName.IndexOf("TripleUp", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return STTTypes.TripleUp;
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