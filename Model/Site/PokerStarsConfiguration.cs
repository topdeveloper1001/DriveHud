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
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Site
{
    public class PokerStarsConfiguration : ISiteConfiguration
    {
        private static readonly string[] PossibleFolders = new string[] { "PokerStars", "PokerStars.EU", "PokerStars.USNJ" };
        private static readonly string[] HandHistoryFolders = new string[] { "HandHistory", "TournSummary" };

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

        public EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.PokerStars;
            }
        }

        private readonly IEnumerable<EnumTableType> tableTypes;

        public IEnumerable<EnumTableType> TableTypes
        {
            get
            {
                return tableTypes;
            }
        }

        public string HeroName
        {
            get;
            set;
        }

        private readonly Dictionary<int, int> prefferedSeat;

        public Dictionary<int, int> PreferredSeats
        {
            get
            {
                return prefferedSeat;
            }
        }

        public TimeSpan TimeZoneOffset
        {
            get;
            set;
        }

        public string[] GetHandHistoryFolders()
        {
            var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var dirs = (from possibleFolder in PossibleFolders
                        from handHistoryFolder in HandHistoryFolders
                        let folder = Path.Combine(localApplicationData, possibleFolder)
                        let hhFolder = Path.Combine(folder, handHistoryFolder)
                        where Directory.Exists(folder)
                        select hhFolder).ToArray();

            return dirs;
        }

        /// <summary>
        /// Checks if PokerStar's save hand history settings are enabled and if correct locale is set
        /// </summary>
        public void ValidateSiteConfiguration()
        {
            const string iniSection = "PipeOption";
            const string iniKeyLocale = "Locale";
            const string iniKeyHHLocale = "HHLocale";
            const string iniKeyTSLocale = "TSLocale";
            const string iniKeySaveMyHands = "SaveMyHands";
            const string iniKeySaveMyTournSummaries = "SaveMyTournSummaries";

            var dirs = GetHandHistoryFolders().Select(x => (new DirectoryInfo(x)).Parent).GroupBy(x => x.FullName).Select(x => x.First()).ToArray();
            List<string> psClientsWithSettingsMismatch = new List<string>();

            foreach (var dir in dirs.Where(d => d.Exists))
            {
                try
                {
                    var settingsFile = Path.Combine(dir.FullName, "user.ini");
                    if (File.Exists(settingsFile))
                    {
                        LogProvider.Log.Info($"Getting PS settings from {settingsFile}");

                        var localeSetting = IniFileHelpers.ReadValue(iniSection, iniKeyLocale, settingsFile);
                        var hhLocaleSetting = IniFileHelpers.ReadValue(iniSection, iniKeyHHLocale, settingsFile);
                        var tsLocaleSetting = IniFileHelpers.ReadValue(iniSection, iniKeyTSLocale, settingsFile);
                        var hhEnabledSetting = IniFileHelpers.ReadValue(iniSection, iniKeySaveMyHands, settingsFile, "0");
                        var tsEnabledSetting = IniFileHelpers.ReadValue(iniSection, iniKeySaveMyTournSummaries, settingsFile, "0");

                        LogProvider.Log.Info($"Locale: {localeSetting}; HHLocale: {hhLocaleSetting}; TSLocale: {tsLocaleSetting}");

                        bool localeSettingIsCorrect = localeSetting == "0";

                        bool hhLanguageIsCorrect = string.IsNullOrWhiteSpace(hhLocaleSetting) ? localeSettingIsCorrect : hhLocaleSetting == "0";
                        bool tsLanguageIsCorrect = string.IsNullOrWhiteSpace(tsLocaleSetting) ? localeSettingIsCorrect : tsLocaleSetting == "0";
                        bool hhIsEnabled = hhEnabledSetting == "1";
                        bool tsIsEnabled = tsEnabledSetting == "1";

                        if (!hhLanguageIsCorrect || !tsLanguageIsCorrect || !hhIsEnabled || !tsIsEnabled)
                        {
                            psClientsWithSettingsMismatch.Add(dir.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(this, "Error during reading of the PS settings", ex);
                }
            }

            if (psClientsWithSettingsMismatch.Any())
            {
                var resultString = String.Format(CommonResourceManager.Instance.GetResourceString("Main_SiteSettingsMismatch_PokerStars"),
                    string.Join(", ", psClientsWithSettingsMismatch));

                ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<MainNotificationEvent>().Publish(new MainNotificationEventArgs("Settings", resultString));

                System.Diagnostics.Debug.WriteLine("resultString");
            }
        }
    }
}