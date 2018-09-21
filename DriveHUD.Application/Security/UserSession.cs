//-----------------------------------------------------------------------
// <copyright file="UserSession.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Linq;

using DriveHUD.Importers;
using HandHistories.Objects.GameDescription;
using DriveHUD.Common.Log;

namespace DriveHUD.Application.Security
{
    internal class UserSession : IUserSession
    {
        public GameType[] AllowedGameTypes
        {
            get;
            set;
        }

        public decimal TournamentLimit
        {
            get;
            set;
        }

        public int CashLimit
        {
            get;
            set;
        }

        public bool IsMatch(GameMatchInfo gameInfo)
        {
            if (gameInfo == null)
            {
                return false;
            }

            if ((gameInfo.Currency == Currency.PlayMoney && gameInfo.TournamentBuyIn == 0) ||
                (gameInfo.TournamentCurrency == Currency.PlayMoney && gameInfo.CashBuyIn == 0))
            {
                return true;
            }

            decimal cashBuyin = gameInfo.CashBuyIn;

            if (gameInfo.Currency == Currency.INR)
            {
                cashBuyin *= 0.014m;
            }

            decimal tournamentBuyIn = gameInfo.TournamentBuyIn;

            if (gameInfo.TournamentCurrency == Currency.INR)
            {
                tournamentBuyIn *= 0.014m;
            }

            var match = AllowedGameTypes.Contains(gameInfo.GameType) &&
                            cashBuyin <= CashLimit &&
                                tournamentBuyIn <= TournamentLimit;

            if (!match)
            {
                LogProvider.Log.Info($"GameType: {gameInfo.GameType}, GameCashBuyIn: {cashBuyin}, GameTournamentBuyIn: {tournamentBuyIn}, Curr: {gameInfo.Currency}, TournCurr: {gameInfo.TournamentCurrency}");
                LogProvider.Log.Info($"LicenseCashBuyIn: {CashLimit}, LicenseTournamentBuyIn: {TournamentLimit}");
            }

            return match;
        }
    }
}