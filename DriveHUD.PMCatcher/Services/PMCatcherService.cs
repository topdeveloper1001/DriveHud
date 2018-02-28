//-----------------------------------------------------------------------
// <copyright file="IPMCatcherService.cs" company="Ace Poker Solutions">
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
using DriveHUD.PMCatcher.Licensing;
using DriveHUD.PMCatcher.Settings;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;

namespace DriveHUD.PMCatcher.Services
{
    internal class PMCatcherService : IPMCatcherService
    {
        private readonly IPMSettingsService settingsService;

        private readonly ILicenseService licenseService;

        public PMCatcherService()
        {
            licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();
            settingsService = ServiceLocator.Current.GetInstance<IPMSettingsService>();
        }

        public Dictionary<long, string> GetHeroes()
        {
            return settingsService.GetSettings()?.Heroes ?? new Dictionary<long, string>();
        }

        public bool IsEnabled()
        {
            return licenseService != null && licenseService.IsRegistered && (settingsService.GetSettings()?.Enabled ?? false);
        }

        public void SaveHeroes(Dictionary<long, string> heroes)
        {
            if (heroes == null)
            {
                return;
            }

            var settingsModel = settingsService.GetSettings();
            settingsModel.Heroes = heroes;
            settingsService.SaveSettings(settingsModel);
        }
    }
}