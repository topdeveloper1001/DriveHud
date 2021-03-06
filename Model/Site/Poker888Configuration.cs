﻿//-----------------------------------------------------------------------
// <copyright file="Poker888Configuration.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using Microsoft.Win32;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Model.Site
{
    public class Poker888Configuration : BaseSiteConfiguration, ISiteConfiguration
    {
        private static readonly string[] PossibleFolders = new string[] { "888poker", "DelawareParkPoker" };

        private static readonly string[] LanguageRegistryKeys = new[] { @"SOFTWARE\pacificpoker\poker\INIT", @"SOFTWARE\DelawareParkPoker\poker\INIT" };
        private const string LanguageRegistryKeyValue = "CURRENT_LANG_ID";

        private const string HandHistoryPathTag = "HistoryPath";
        private const string IsEnableHistoryTag = "IsEnableHistory";
        private const string CorrectIsEnableHistorySetting = "1";

        private const string settingsFileName = "UserSettings.xml";

        public Poker888Configuration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,
                EnumTableType.Three,
                EnumTableType.Four,
                EnumTableType.Five,
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
                return EnumPokerSites.Poker888;
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

        public override string LogoSource
        {
            get
            {
                return "/DriveHUD.Common.Resources;Component/images/SiteLogos/888_logo.png";
            }
        }

        public override string[] GetHandHistoryFolders()
        {
            var handHistoryFolders = new List<string>();

            var configurationDirectories = GetConfigurationDirectories();

            foreach (var configurationDirectory in configurationDirectories)
            {
                try
                {
                    var settingsFiles = configurationDirectory.GetFiles(settingsFileName, SearchOption.AllDirectories);

                    foreach (var settingsFile in settingsFiles)
                    {
                        try
                        {
                            if (settingsFile.Directory.Parent.FullName != configurationDirectory.FullName)
                            {
                                continue;
                            }

                            var xmlValues = ReadXmlValues(settingsFile.FullName, new[] { HandHistoryPathTag });

                            if (xmlValues.ContainsKey(HandHistoryPathTag) && !string.IsNullOrWhiteSpace(xmlValues[HandHistoryPathTag]))
                            {
                                handHistoryFolders.Add(xmlValues[HandHistoryPathTag]);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogProvider.Log.Error(this, $"Error occurred during reading {settingsFile.FullName}", ex);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Could not read data from {configurationDirectory.FullName}", e);
                }
            }

            return handHistoryFolders.Distinct().ToArray();
        }

        public override ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                IsDetected = IsInstalled(),
                IsEnabled = siteModel.Enabled
            };

            var configurationDirectories = GetConfigurationDirectories();

            foreach (var configurationDirectory in configurationDirectories)
            {
                try
                {
                    var settingsFiles = configurationDirectory.GetFiles(settingsFileName, SearchOption.AllDirectories);

                    foreach (var settingsFile in settingsFiles)
                    {
                        try
                        {
                            if (settingsFile.Directory.Parent.FullName != configurationDirectory.FullName)
                            {
                                continue;
                            }

                            var xmlValues = ReadXmlValues(settingsFile.FullName, new[] { HandHistoryPathTag, IsEnableHistoryTag });

                            if (xmlValues.ContainsKey(HandHistoryPathTag) && !string.IsNullOrWhiteSpace(xmlValues[HandHistoryPathTag]))
                            {
                                validationResult.HandHistoryLocations.Add(xmlValues[HandHistoryPathTag]);
                            }

                            if (xmlValues.ContainsKey(IsEnableHistoryTag) && (xmlValues[IsEnableHistoryTag] != CorrectIsEnableHistorySetting))
                            {
                                var issue = string.Format(CommonResourceManager.Instance.GetResourceString("Error_888_Validation_IsEnableHistory"),
                                    settingsFile.Directory.Parent.Name);

                                validationResult.Issues.Add(issue);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogProvider.Log.Error(this, $"Error occurred during reading {settingsFile.FullName}", ex);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Could not read data from {configurationDirectory.FullName}", e);
                }
            }

            try
            {
                foreach (var registryKey in LanguageRegistryKeys)
                {
                    var languageRegistryKey = Registry.CurrentUser.OpenSubKey(registryKey);

                    if (languageRegistryKey != null)
                    {
                        var language = Convert.ToUInt32(languageRegistryKey.GetValue(LanguageRegistryKeyValue));

                        if (language != 0)
                        {
                            var issue = string.Format(CommonResourceManager.Instance.GetResourceString("Error_888_Validation_HHLanguage"));
                            validationResult.Issues.Add(issue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Could not read '{LanguageRegistryKeys}'", ex);
            }

            return validationResult;
        }

        /// <summary>
        /// Determines whenever poker client is installed
        /// </summary>
        /// <returns>True - if installed, otherwise - false</returns>
        private bool IsInstalled()
        {
            try
            {
                var isInstalled = LanguageRegistryKeys.Any(key => Registry.CurrentUser.OpenSubKey(key) != null);

                return isInstalled;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Couldn't detect if 888 poker is installed.", e);
            }

            return false;
        }

        /// <summary>
        /// Gets the array of directories with user 888 configuration
        /// </summary>
        /// <returns>The array of directories with user 888 configuration</returns>
        private IList<DirectoryInfo> GetConfigurationDirectories()
        {
            var directories = new List<DirectoryInfo>();

            try
            {
                var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

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
        /// Reads xml tag value from text
        /// </summary>
        /// <param name="text">Xml</param>
        /// <returns>Result</returns>
        private Dictionary<string, string> ReadXmlValues(string file, string[] tags)
        {
            var result = tags.GroupBy(x => x).ToDictionary(x => x.Key, x => string.Empty);

            try
            {
                using (var xmlReader = XmlReader.Create(file))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && result.ContainsKey(xmlReader.Name))
                        {
                            result[xmlReader.Name] = xmlReader.ReadElementContentAsString();

                            if (result.Values.All(x => !string.IsNullOrEmpty(x)))
                            {
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Could not read xml data from {file}", ex);
            }

            return result;
        }
    }
}