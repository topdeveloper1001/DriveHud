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

using DriveHUD.Common.Log;
using DriveHUD.Importers;
using HandHistories.Objects.GameDescription;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace DriveHUD.Application.Security
{
    internal class UserSession : IUserSession
    {
        private readonly ICurrencyRatesService currencyRatesService;

        private readonly GameType[] allowedGameTypes;
        private readonly decimal cashLimit;
        private readonly decimal tournamentLimit;
        private CurrencyRates currencyRates;

        public UserSession(decimal cashLimit, decimal tournamentLimit, GameType[] allowedGameTypes)
        {
            this.cashLimit = cashLimit;
            this.tournamentLimit = tournamentLimit;
            this.allowedGameTypes = allowedGameTypes;

            currencyRatesService = ServiceLocator.Current.GetInstance<ICurrencyRatesService>();
            currencyRatesService.RefreshCurrencyRates();
        }

        public bool IsMatch(GameMatchInfo gameInfo)
        {
            if (gameInfo == null || cashLimit == 0 || tournamentLimit == 0)
            {
                return false;
            }

            if (currencyRates == null)
            {
                InitializeCurrencyRates();
            }

            if ((gameInfo.Currency == Currency.PlayMoney && gameInfo.TournamentBuyIn == 0) ||
                (gameInfo.TournamentCurrency == Currency.PlayMoney && gameInfo.CashBuyIn == 0))
            {
                return true;
            }

            var cashBuyin = currencyRates != null && currencyRates.Rates != null && currencyRates.Rates.TryGetValue(gameInfo.Currency, out decimal cashRate) ?
                cashRate * gameInfo.CashBuyIn : gameInfo.CashBuyIn;

            var tournamentBuyIn = currencyRates != null && currencyRates.Rates != null && currencyRates.Rates.TryGetValue(gameInfo.Currency, out decimal tourneyRate) ?
                tourneyRate * gameInfo.TournamentBuyIn : gameInfo.TournamentBuyIn;

            var match = allowedGameTypes.Contains(gameInfo.GameType) &&
                            cashBuyin <= cashLimit &&
                                tournamentBuyIn <= tournamentLimit;

            if (!match)
            {
                LogProvider.Log.Info($"GameType: {gameInfo.GameType}, GameCashBuyIn: {cashBuyin}, GameTournamentBuyIn: {tournamentBuyIn}, Curr: {gameInfo.Currency}, TournCurr: {gameInfo.TournamentCurrency}");
                LogProvider.Log.Info($"LicenseCashBuyIn: {cashLimit}, LicenseTournamentBuyIn: {tournamentLimit}");
            }

            return match;
        }

        private void InitializeCurrencyRates()
        {
            currencyRates = currencyRatesService.GetCurrencyRates();
        }
    }
}