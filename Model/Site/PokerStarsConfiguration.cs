//-----------------------------------------------------------------------
// <copyright file="PokerStarsConfiguration.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model.Site
{
    public class PokerStarsConfiguration : BaseSiteConfiguration, ISiteConfiguration
    {
        private static readonly string[] PossibleFolders = new string[] { "PokerStars", "PokerStars.EU", "PokerStars.USNJ", "PokerStars.PT", "PokerStars.UK", "PokerStars.FR",
            "PokerStars.DK", "PokerStars.BG", "PokerStars.IT", "PokerStars.ES", "PokerStars.CZ", "PokerStars.SH" };

        private const string defaultHandHistoryFolder = "HandHistory";
        private const string defaultTourneySummaryFolder = "TournSummary";
        private const string iniSection = "PipeOption";
        private const string iniKeyLocale = "Locale";
        private const string iniKeyHHLocale = "HHLocale";
        private const string iniKeyTSLocale = "TSLocale";
        private const string iniKeySaveMyHands = "SaveMyHands";
        private const string iniKeySaveMyTournSummaries = "SaveMyTournSummaries";
        private const string iniSaveMyHandsPath = "SaveMyHandsPath";
        private const string iniSaveMyTournSummariesPath = "SaveMyTournSummariesPath";
        private const string settingsFileName = "user.ini";
        private const string correctLanguageSetting = "0";

        public PokerStarsConfiguration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,
                EnumTableType.Four,
                EnumTableType.Six,
                EnumTableType.Eight,
                EnumTableType.Nine,
                EnumTableType.Ten
            };
        }

        public override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.PokerStars;
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

        private readonly Dictionary<int, int> prefferedSeat;

        public override Dictionary<int, int> PreferredSeats
        {
            get
            {
                return prefferedSeat;
            }
        }

        public override bool IsHandHistoryLocationRequired
        {
            get
            {
                return true;
            }
        }

        public override bool IsPrefferedSeatsAllowed
        {
            get
            {
                return true;
            }
        }

        public override bool FastPokerAllowed
        {
            get
            {
                return true;
            }
        }

        public override string FastPokerModeName
        {
            get
            {
                return CommonResourceManager.Instance.GetResourceString("Settings_FastPoker_PokerStarsName");
            }
        }

        public override string LogoSource
        {
            get
            {
                return "/DriveHUD.Common.Resources;Component/images/SiteLogos/pokerstars_logo.png";
            }
        }

        /// <summary>
        /// Gets the paths to hand histories folders
        /// </summary>
        /// <returns>The array of paths to hand histories folders</returns>
        public override string[] GetHandHistoryFolders()
        {
            var handHistoryFolders = new List<string>();

            var configurationDirectories = GetConfigurationDirectories();

            foreach (var configurationDirectory in configurationDirectories)
            {
                var settingsFilePath = Path.Combine(configurationDirectory.FullName, settingsFileName);

                if (!File.Exists(settingsFilePath))
                {
                    continue;
                }

                if (!ReadAndAddPathFromIniSettings(iniSaveMyHandsPath, handHistoryFolders, settingsFilePath))
                {
                    var handHistoryFolder = Path.Combine(configurationDirectory.FullName, defaultHandHistoryFolder);
                    handHistoryFolders.Add(handHistoryFolder);
                }

                if (!ReadAndAddPathFromIniSettings(iniSaveMyTournSummariesPath, handHistoryFolders, settingsFilePath))
                {
                    var tourneySummaryFolder = Path.Combine(configurationDirectory.FullName, defaultTourneySummaryFolder);
                    handHistoryFolders.Add(tourneySummaryFolder);
                }
            }

            return handHistoryFolders.Distinct().ToArray();
        }

        /// <summary>
        /// Validates site settings in both client and DH 
        /// </summary>
        /// <param name="siteModel">Model with site settings</param>
        /// <returns>The result of validation</returns>
        public override ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            if (siteModel == null)
            {
                return null;
            }

            var configurationDirectories = GetConfigurationDirectories();

            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                HandHistoryLocations = GetHandHistoryFolders().ToList(),
                IsDetected = configurationDirectories.Count > 0,
                IsEnabled = siteModel.Enabled,
                FastPokerEnabled = true
            };

            foreach (var configurationDirectory in configurationDirectories)
            {
                try
                {
                    var settingsFile = Path.Combine(configurationDirectory.FullName, settingsFileName);

                    if (!File.Exists(settingsFile))
                    {
                        continue;
                    }

                    LogProvider.Log.Info($"Reading PS settings from {settingsFile}");

                    var localeSetting = IniFileHelpers.ReadValue(iniSection, iniKeyLocale, settingsFile);
                    var hhLocaleSetting = IniFileHelpers.ReadValue(iniSection, iniKeyHHLocale, settingsFile);
                    var tsLocaleSetting = IniFileHelpers.ReadValue(iniSection, iniKeyTSLocale, settingsFile);
                    var hhEnabledSetting = IniFileHelpers.ReadValue(iniSection, iniKeySaveMyHands, settingsFile, "0");
                    var tsEnabledSetting = IniFileHelpers.ReadValue(iniSection, iniKeySaveMyTournSummaries, settingsFile, "0");

                    LogProvider.Log.Info($"PS Settings: Locale: {localeSetting}; HHLocale: {hhLocaleSetting}; TSLocale: {tsLocaleSetting}");

                    var isLocaleSettingCorrect = localeSetting == correctLanguageSetting;

                    bool isHHLanguageCorrect = string.IsNullOrWhiteSpace(hhLocaleSetting) ? isLocaleSettingCorrect : hhLocaleSetting == correctLanguageSetting;
                    bool isTSLanguageCorrect = string.IsNullOrWhiteSpace(tsLocaleSetting) ? isLocaleSettingCorrect : tsLocaleSetting == correctLanguageSetting;
                    bool isHHEnabled = hhEnabledSetting == "1";
                    bool isTSEnabled = tsEnabledSetting == "1";

                    if (!isHHLanguageCorrect || !isTSLanguageCorrect || !isHHEnabled || !isTSEnabled)
                    {
                        if (!isHHEnabled)
                        {
                            var issue = string.Format(CommonResourceManager.Instance.GetResourceString("Error_PS_Validation_SaveMyHandHistory"),
                                configurationDirectory.Name);

                            validationResult.Issues.Add(issue);
                        }

                        if (!isTSEnabled)
                        {
                            var issue = string.Format(CommonResourceManager.Instance.GetResourceString("Error_PS_Validation_SaveTournamentSummaries"),
                                configurationDirectory.Name);

                            validationResult.Issues.Add(issue);
                        }

                        if (!isHHLanguageCorrect)
                        {
                            var issue = string.Format(CommonResourceManager.Instance.GetResourceString("Error_PS_Validation_HHLanguage"),
                                configurationDirectory.Name);

                            validationResult.Issues.Add(issue);
                        }

                        if (!isTSLanguageCorrect)
                        {
                            var issue = string.Format(CommonResourceManager.Instance.GetResourceString("Error_PS_Validation_TSLanguage"),
                                configurationDirectory.Name);

                            validationResult.Issues.Add(issue);
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(this, "Error during reading of the PS settings", ex);
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Gets the array of directories with user PS configuration
        /// </summary>
        /// <returns>The array of directories with user PS configuration</returns>
        private IList<DirectoryInfo> GetConfigurationDirectories()
        {
            var directories = new List<DirectoryInfo>();

            try
            {
                var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                foreach (var possibleFolder in PossibleFolders)
                {
                    var path = Path.Combine(localApplicationData, possibleFolder);

                    try
                    {
                        var directory = new DirectoryInfo(path);

                        if (directory.Exists)
                        {
                            directories.Add(directory);
                        }
                    }
                    catch (Exception e)
                    {
                        LogProvider.Log.Error(this, $"Could not get info about {path}", e);
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Could not get path to local application data folder", ex);
            }

            return directories;
        }

        /// <summary>
        /// Reads the value for the specified key for the specified path to ini file and add value to the specified list
        /// </summary>
        /// <param name="iniKey">Key to read</param>
        /// <param name="handHistoryFolders">List to add value</param>
        /// <param name="settingsFilePath">Path to the ini file</param>
        private bool ReadAndAddPathFromIniSettings(string iniKey, IList<string> handHistoryFolders, string settingsFilePath)
        {
            try
            {
                var handHistoriesFolder = IniFileHelpers.ReadValue(iniSection, iniKey, settingsFilePath);

                if (!string.IsNullOrEmpty(handHistoriesFolder))
                {
                    handHistoryFolders.Add(handHistoriesFolder);
                    return true;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not read '{iniKey}'", e);
            }

            return false;
        }
    }
}