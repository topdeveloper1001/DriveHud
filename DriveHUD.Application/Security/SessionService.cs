//-----------------------------------------------------------------------
// <copyright file="SessionService.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Resources;
using DriveHUD.Importers;
using HandHistories.Objects.GameDescription;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Application.Security
{
    internal class SessionService : ISessionService
    {
        private IUserSession userSession;
        private readonly ILicenseService licenseService;
        private bool isInitialized;

        public SessionService()
        {
            licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();
        }

        public IUserSession GetUserSession()
        {
            if (!isInitialized)
            {
                throw new DHInternalException(new NonLocalizableString("Session has not been initialized"));
            }

            return userSession;
        }

        public void Initialize()
        {
            isInitialized = true;

            var registeredLicenses = licenseService.LicenseInfos.Where(x => x.IsRegistered).ToArray();

            // if any license is not trial
            if (registeredLicenses.Any(x => !x.IsTrial))
            {
                registeredLicenses = registeredLicenses.Where(x => !x.IsTrial).ToArray();
            }

            var gameTypes = registeredLicenses.SelectMany(x => ConvertLicenseType(x.LicenseType)).Distinct().ToArray();
            var cashLimit = registeredLicenses.Length > 0 ? registeredLicenses.Max(x => x.CashLimit) : 0;
            var tournamentLimit = registeredLicenses.Length > 0 ? registeredLicenses.Max(x => x.TournamentLimit) : 0;

            userSession = new UserSession(cashLimit, tournamentLimit, gameTypes);
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
    }
}