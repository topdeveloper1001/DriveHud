//-----------------------------------------------------------------------
// <copyright file="IPKCatcherService.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.CustomServices;
using DriveHUD.PKCatcher.Licensing;
using DriveHUD.PKCatcher.Settings;
using HandHistories.Objects.Hand;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace DriveHUD.PKCatcher.Services
{
    internal class PKCatcherService : IPKCatcherService
    {
        private readonly IPKSettingsService settingsService;

        private readonly ILicenseService licenseService;

        public PKCatcherService()
        {
            licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();
            settingsService = ServiceLocator.Current.GetInstance<IPKSettingsService>();
        }       

        public bool IsEnabled()
        {
            return licenseService != null && licenseService.IsRegistered && (settingsService.GetSettings()?.Enabled ?? false);
        }
     
        public bool CheckHand(HandHistory handHistory)
        {
            var registeredLicenses = licenseService.LicenseInfos.Where(x => x.IsRegistered).ToArray();

            // if any license is not trial
            if (registeredLicenses.Any(x => !x.IsTrial))
            {
                registeredLicenses = registeredLicenses.Where(x => !x.IsTrial).ToArray();
            }

            if (registeredLicenses.Length == 0)
            {
                return false;
            }

            if (handHistory.GameDescription.IsTournament)
            {
                var tournamentLimit = registeredLicenses.Max(x => x.TournamentLimit);
                return handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue <= tournamentLimit;
            }

            var cashLimit = registeredLicenses.Max(x => x.CashLimit);

            var limit = handHistory.GameDescription.Limit.BigBlind;       

            return limit <= cashLimit;
        }
    }
}