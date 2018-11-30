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

using DriveHUD.Application.Licensing;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Importers;
using HandHistories.Objects.GameDescription;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Application.Security
{
    internal class UserSession : IUserSession
    {
        private readonly ICurrencyRatesService currencyRatesService;
        private CurrencyRates currencyRates;
        private readonly ReadOnlyDictionary<GameType, LicenseLimit> licenseLimits;

        public UserSession(ILicenseInfo[] registeredLicenses)
        {
            var limits = (from registeredLicense in registeredLicenses
                          from gameType in ConvertLicenseType(registeredLicense.LicenseType)
                          select new { GameType = gameType, registeredLicense.CashLimit, registeredLicense.TournamentLimit } into limit
                          group limit by limit.GameType into groupedLimit
                          let cashLimit = groupedLimit.MaxOrDefault(x => x.CashLimit)
                          let tournamentLimit = groupedLimit.MaxOrDefault(x => x.TournamentLimit)
                          select new { GameType = groupedLimit.Key, Limit = new LicenseLimit(cashLimit, tournamentLimit) }
                          ).ToDictionary(x => x.GameType, x => x.Limit);

            licenseLimits = new ReadOnlyDictionary<GameType, LicenseLimit>(limits);

            currencyRatesService = ServiceLocator.Current.GetInstance<ICurrencyRatesService>();
            currencyRatesService.RefreshCurrencyRates();
        }

        public bool IsMatch(GameMatchInfo gameInfo)
        {
            if (gameInfo == null || licenseLimits.Count == 0)
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

            if (!licenseLimits.TryGetValue(gameInfo.GameType, out LicenseLimit licenseLimit))
            {
                return false;
            }

            var cashBuyin = currencyRates != null && currencyRates.Rates != null && currencyRates.Rates.TryGetValue(gameInfo.Currency, out decimal cashRate) ?
                cashRate * gameInfo.CashBuyIn : gameInfo.CashBuyIn;

            var tournamentBuyIn = currencyRates != null && currencyRates.Rates != null && currencyRates.Rates.TryGetValue(gameInfo.TournamentCurrency, out decimal tourneyRate) ?
                tourneyRate * gameInfo.TournamentBuyIn : gameInfo.TournamentBuyIn;

            var match = cashBuyin <= licenseLimit.CashLimit && tournamentBuyIn <= licenseLimit.TournamentLimit;

            if (!match)
            {
                LogProvider.Log.Info($"GameType: {gameInfo.GameType}, GameCashBuyIn: {cashBuyin}, GameTournamentBuyIn: {tournamentBuyIn}, Curr: {gameInfo.Currency}, TournCurr: {gameInfo.TournamentCurrency}");
                LogProvider.Log.Info($"LicenseCashBuyIn: {licenseLimit.CashLimit}, LicenseTournamentBuyIn: {licenseLimit.TournamentLimit}");
            }

            return match;
        }

        private void InitializeCurrencyRates()
        {
            currencyRates = currencyRatesService.GetCurrencyRates();
        }

        private static IEnumerable<GameType> ConvertLicenseType(LicenseType licenseType)
        {
            var gameTypes = new List<GameType>();

            switch (licenseType)
            {
                case LicenseType.Holdem:
                    gameTypes.Add(GameType.CapNoLimitHoldem);
                    gameTypes.Add(GameType.FixedLimitHoldem);
                    gameTypes.Add(GameType.NoLimitHoldem);
                    gameTypes.Add(GameType.PotLimitHoldem);
                    gameTypes.Add(GameType.SpreadLimitHoldem);
                    break;
                case LicenseType.Omaha:
                    gameTypes.Add(GameType.CapPotLimitOmaha);
                    gameTypes.Add(GameType.FiveCardPotLimitOmaha);
                    gameTypes.Add(GameType.FiveCardPotLimitOmahaHiLo);
                    gameTypes.Add(GameType.FiveCardPotLimitOmaha);
                    gameTypes.Add(GameType.FixedLimitOmaha);
                    gameTypes.Add(GameType.FixedLimitOmahaHiLo);
                    gameTypes.Add(GameType.NoLimitOmaha);
                    gameTypes.Add(GameType.NoLimitOmahaHiLo);
                    gameTypes.Add(GameType.PotLimitOmaha);
                    gameTypes.Add(GameType.PotLimitOmahaHiLo);
                    break;
                case LicenseType.Combo:
                case LicenseType.Trial:
                    foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
                    {
                        gameTypes.Add(gameType);
                    }
                    break;
                default:
                    throw new DHInternalException(new NonLocalizableString("Not supported license type"));
            }

            return gameTypes;
        }

        private class LicenseLimit
        {
            public LicenseLimit(int cashLimit, decimal tournamentLimit)
            {
                CashLimit = cashLimit;
                TournamentLimit = tournamentLimit;
            }

            public int CashLimit { get; }

            public decimal TournamentLimit { get; }
        }
    }
}