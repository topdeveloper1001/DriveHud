//-----------------------------------------------------------------------
// <copyright file="IPokerConfiguration.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Model.Site
{
    public class IPokerConfiguration : BaseSiteConfiguration, ISiteConfiguration
    {
        private static readonly string[] SiteNames = new string[]
        {
            "Poker at bet365",
            "Betfair Poker",
            "Ladbrokes Poker",
            "Titan Poker",
            "William Hill Poker",
            "Coral Poker",
            "Paddy Power Poker",
            "Betfred Poker",
            "BoyleSports Poker",
            "NetBet Poker",
            "Everest Poker",
            "Winner Poker",
            "Dafa Poker",
            "Expekt Poker",
            "Betclic Poker",
            // the following clients allow to select installation path, so it will be stored in registry            
            "Betlive Poker",
            "Poker at Sports Interaction",
            "MansionPoker"
        };

        private static readonly Lazy<Regex> handHistoryLocationPattern = new Lazy<Regex>(() => new Regex(@"data\\(.*)\\History\\Data$"));

        private const string PredefinedInstallFolder = @"c:\\Poker";

        private const string RegistryInstallFolderKey = "homedir";

        private const string SoftwareRegistryKey = "SOFTWARE";

        public IPokerConfiguration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,
                EnumTableType.Three,
                EnumTableType.Four,
                EnumTableType.Six,
                EnumTableType.Nine,
                EnumTableType.Ten
            };
        }

        public override bool IsHandHistoryLocationRequired
        {
            get
            {
                return true;
            }
        }

        public override bool IsAutoCenterAllowed
        {
            get
            {
                return true;
            }
        }

        public override string LogoSource
        {
            get
            {
                return "/DriveHUD.Common.Resources;Component/images/SiteLogos/ipoker_logo.png";
            }
        }

        private readonly Dictionary<int, int> prefferedSeat;

        public override Dictionary<int, int> PreferredSeats
        {
            get
            {
                return prefferedSeat;
            }
        }

        public override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.IPoker;
            }
        }

        private readonly IEnumerable<EnumTableType> tableTypes;

        public override IEnumerable<EnumTableType> TableTypes
        {
            get
            {
                return tableTypes;
            }
        }

        public override string[] GetHandHistoryFolders()
        {
            var handHistoryFolders = new List<string>();

            foreach (var siteName in SiteNames)
            {
                var installPath = GetSiteInstallPath(siteName);

                if (string.IsNullOrEmpty(installPath))
                {
                    continue;
                }

                var hhDirs = Directory.EnumerateDirectories(installPath, "*", SearchOption.AllDirectories)
                    .Where(x => handHistoryLocationPattern.Value.IsMatch(x)).ToArray();

                handHistoryFolders.AddRange(hhDirs);
            }

            return handHistoryFolders.Distinct().ToArray();
        }

        public override ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            if (siteModel == null)
            {
                return null;
            }

            // since we don't show HandHistoryLocations on site setup form, we don't need to spend time on reading dirs
            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                HandHistoryLocations = new List<string>(),
                IsEnabled = siteModel.Enabled,
                IsAutoCenter = true
            };

            var settingsModel = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

            foreach (var siteName in SiteNames)
            {
                var installPath = GetSiteInstallPath(siteName);

                if (!string.IsNullOrEmpty(installPath))
                {
                    if (settingsModel.GeneralSettings.IsAdvancedLoggingEnabled)
                    {
                        LogProvider.Log.Info($"Site detection: IPoker[{siteName}]: {installPath}");
                    }

                    try
                    {
                        var hhDirs = Directory.EnumerateDirectories(installPath, "*", SearchOption.AllDirectories)
                            .Where(x => handHistoryLocationPattern.Value.IsMatch(x)).ToArray();

                        validationResult.HandHistoryLocations.AddRange(hhDirs);
                    }
                    catch (Exception e)
                    {
                        LogProvider.Log.Error(this, $"Couldn't get hand history locations for {siteName} at {installPath}", e);
                    }

                    validationResult.IsDetected = true;
                }
            }

            return validationResult;
        }

        private string GetSiteInstallPath(string siteName)
        {
            try
            {
                // check local app folder
                var localAppInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), siteName);

                if (Directory.Exists(localAppInstallPath))
                {
                    return localAppInstallPath;
                }

                // check registry for homedir
                var siteRegistryKeyPath = Path.Combine(SoftwareRegistryKey, siteName);

                var softwareRegistryKey = Registry.CurrentUser.OpenSubKey(siteRegistryKeyPath);

                if (softwareRegistryKey != null)
                {
                    var registryInstallPath = softwareRegistryKey.GetValue(RegistryInstallFolderKey) as string;

                    if (!string.IsNullOrEmpty(registryInstallPath) && Directory.Exists(registryInstallPath))
                    {
                        return registryInstallPath;
                    }
                }

                // check predefined install path
                var predefinedInstallPath = Path.Combine(PredefinedInstallFolder, siteName);

                if (Directory.Exists(predefinedInstallPath))
                {
                    return localAppInstallPath;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Couldn't find install path for {siteName}", e);
            }

            return null;
        }
    }
}