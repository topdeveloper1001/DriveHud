//-----------------------------------------------------------------------
// <copyright file="BovadaConverters.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using HandHistories.Parser.Utils.FastParsing;
using Model.Enums;
using System;
using System.Globalization;
using System.Linq;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Converters for Bovada
    /// </summary>
    internal class BovadaConverters
    {
        public static int ConvertHexStringToInt32(string input)
        {
            input = PrepareHexStringToCoversion(input);

            int result;

            if (!int.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
            {
                return 0;
            }

            return result;
        }

        public static uint ConvertHexStringToUint32(string input)
        {
            input = PrepareHexStringToCoversion(input);

            uint result;

            if (!uint.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
            {
                return 0;
            }

            return result;
        }

        private static string PrepareHexStringToCoversion(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (input.StartsWith("0x"))
            {
                input = input.Substring(2, input.Length - 2);
            }

            return input;
        }

        public static string ConvertCardIntToString(int cardInt)
        {
            if (cardInt > 52)
            {
                return string.Empty;
            }

            var suit = cardInt / 13;
            var rank = cardInt % 13;

            var rankString = string.Empty;
            var suitString = string.Empty;

            switch (rank)
            {
                case 0:
                    rankString = "A";
                    break;
                case 1:
                    rankString = "2";
                    break;
                case 2:
                    rankString = "3";
                    break;
                case 3:
                    rankString = "4";
                    break;
                case 4:
                    rankString = "5";
                    break;
                case 5:
                    rankString = "6";
                    break;
                case 6:
                    rankString = "7";
                    break;
                case 7:
                    rankString = "8";
                    break;
                case 8:
                    rankString = "9";
                    break;
                case 9:
                    rankString = "T";
                    break;
                case 10:
                    rankString = "J";
                    break;
                case 11:
                    rankString = "Q";
                    break;
                case 12:
                    rankString = "K";
                    break;
                default:
                    rankString = "";
                    break;
            }

            switch (suit)
            {
                case 0:
                    suitString = "c";
                    break;
                case 1:
                    suitString = "d";
                    break;
                case 2:
                    suitString = "h";
                    break;
                case 3:
                    suitString = "s";
                    break;
                default:
                    suitString = "";
                    break;
            }

            return rankString + suitString;
        }

        public static PlayerActionEnum ConvertActionTypeToActionEnum(BovadaPlayerActionType actionType)
        {
            switch (actionType)
            {
                case BovadaPlayerActionType.AllIn:
                    return PlayerActionEnum.Allin;
                case BovadaPlayerActionType.AllInRaise:
                    return PlayerActionEnum.AllinRaise;
                case BovadaPlayerActionType.Bet:
                    return PlayerActionEnum.Bet;
                case BovadaPlayerActionType.Call:
                    return PlayerActionEnum.Call;
                case BovadaPlayerActionType.Check:
                    return PlayerActionEnum.Check;
                case BovadaPlayerActionType.Fold:
                    return PlayerActionEnum.Fold;
                case BovadaPlayerActionType.FoldShow:
                    return PlayerActionEnum.FoldShow;
                case BovadaPlayerActionType.Raise:
                    return PlayerActionEnum.RaiseTo;
                case BovadaPlayerActionType.Ante:
                    return PlayerActionEnum.Ante;
                default:
                    throw new DHInternalException(new NonLocalizableString("Unknown action type: " + (int)actionType));
            }
        }

        public static GameType ConvertGameType(string gameType)
        {
            Check.ArgumentNotNull(() => gameType);

            gameType = gameType.ToLowerInvariant();

            switch (gameType)
            {
                case "omaha":
                    return GameType.Omaha;
                case "omahahl":
                    return GameType.OmahaHiLo;
                default:
                    return GameType.Holdem;
            }
        }

        public static GameFormat ConvertGameFormat(string gameFormat)
        {
            Check.ArgumentNotNull(() => gameFormat);

            gameFormat = gameFormat.ToLowerInvariant();

            switch (gameFormat)
            {
                case "sng":
                    return GameFormat.SnG;
                case "scheduled":
                    return GameFormat.MTT;
                case "zone":
                    return GameFormat.Zone;
                default:
                    return GameFormat.Cash;
            }
        }

        public static GameLimit ConvertGameLimit(string gameLimit)
        {
            Check.ArgumentNotNull(() => gameLimit);

            gameLimit = gameLimit.ToLowerInvariant();

            switch (gameLimit)
            {
                case "pl":
                    return GameLimit.PL;
                case "fl":
                    return GameLimit.FL;
                default:
                    return GameLimit.NL;
            }
        }

        public static EnumCurrency ConvertCurrency(string currency)
        {
            Check.ArgumentNotNull(() => currency);

            currency = currency.ToLowerInvariant();

            switch (currency)
            {
                case "eur":
                    return EnumCurrency.EUR;
                case "can":
                    return EnumCurrency.CAN;
                case "cny":
                    return EnumCurrency.CNY;
                default:
                    return EnumCurrency.USD;
            }
        }

        public static int ConvertTableState(int tableState)
        {
            if (tableState == 4096)
            {
                return (int)BovadaTableState.ShowDown;
            }
            else if (tableState == 8192)
            {
                return (int)BovadaTableState.ShowStacks;
            }

            return tableState;
        }

        public static decimal ConvertBBtoSB(decimal bigBlind)
        {
            if (bigBlind == 0.05m)
            {
                return 0.02m;
            }

            if (bigBlind == 0.25m)
            {
                return 0.10m;
            }

            return bigBlind / 2;
        }

        public static string ConvertDecimalToString(decimal number)
        {
            var s = string.Format(CultureInfo.InvariantCulture, "{0:0.00}", number);

            if (s.EndsWith("00"))
            {
                return ((int)number).ToString();
            }
            else
            {
                return s;
            }
        }

        public static decimal ConvertPrizeTextToDecimal(string prizeText)
        {
            if (string.IsNullOrEmpty(prizeText))
            {
                return 0;
            }

            int prizeStartIndex = -1;
            int prizeEndIndex = -1;

            if (prizeText.IndexOf("Tournament Ticket:", StringComparison.OrdinalIgnoreCase) > 0 &&
                (prizeStartIndex = prizeText.IndexOf("Any", StringComparison.OrdinalIgnoreCase)) > 0)
            {
                prizeEndIndex = prizeText.LastIndexOf("<", StringComparison.Ordinal);

                if (prizeEndIndex < prizeStartIndex)
                {
                    prizeEndIndex = prizeText.Length;
                }

                prizeText = prizeText.Substring(prizeStartIndex, prizeEndIndex - prizeStartIndex);

                var ticketBuyInAndRake = prizeText
                    .Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => ParserUtils.ParseMoney(x.Trim()))
                    .ToArray();

                var result = ticketBuyInAndRake.Sum();
                return result;
            }

            prizeStartIndex = prizeText.IndexOf("Cash:", StringComparison.OrdinalIgnoreCase);

            if (prizeStartIndex <= 0)
            {
                prizeStartIndex = prizeText.IndexOf("Play Money:", StringComparison.OrdinalIgnoreCase);

                if (prizeStartIndex <= 0)
                {
                    return 0;
                }

                prizeStartIndex += 11;
            }
            else
            {
                prizeStartIndex += 6;
            }

            prizeEndIndex = prizeText.LastIndexOf("<", StringComparison.Ordinal);

            if (prizeEndIndex <= 0)
            {
                return 0;
            }

            prizeText = prizeText.Substring(prizeStartIndex, prizeEndIndex - prizeStartIndex);

            var prize = ParserUtils.ParseMoney(prizeText);
            return prize;
        }
    }
}