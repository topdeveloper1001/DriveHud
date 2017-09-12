//-----------------------------------------------------------------------
// <copyright file="AmericasCardroomConfiguration.cs" company="Ace Poker Solutions">
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
using Microsoft.Win32;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Model.Site
{
    public class AmericasCardroomConfiguration : ISiteConfiguration
    {
        private const string DisplayNameKeyValue = "DisplayName";
        private const string InstallLocationKeyValue = "InstallLocation";
        private const string SettingsFile = "pref.xml";
        private const string ProfilesFolder = "profiles";
        private const string ProfileHandHistoryId = "3";
        private const string ProfileSaveHandHistoryId = "1";
        private const string CorrectSaveHandHistoryTag = "1";

        public AmericasCardroomConfiguration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,
                EnumTableType.Three,
                EnumTableType.Four,
                EnumTableType.Six,
                EnumTableType.Eight,
                EnumTableType.Nine
            };
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

        public virtual EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.AmericasCardroom;
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

        public TimeSpan TimeZoneOffset
        {
            get;
            set;
        }

        public bool IsHandHistoryLocationRequired
        {
            get
            {
                return true;
            }
        }

        public bool IsPrefferedSeatsAllowed
        {
            get
            {
                return true;
            }
        }

        public virtual string LogoSource
        {
            get
            {
                return "/DriveHUD.Common.Resources;Component/images/SiteLogos/wpn_logo.png";
            }
        }

        protected virtual string RegistryDisplayName
        {
            get
            {
                return "AmericasCardroom";
            }
        }

        public virtual string[] GetHandHistoryFolders()
        {
            var handHistoryFolders = new List<string>();

            var path = GetInstalledPath(RegistryDisplayName);

            var profilesDirectoryPath = Path.Combine(path, ProfilesFolder);

            try
            {
                var profilesDirectory = new DirectoryInfo(profilesDirectoryPath);

                if (!profilesDirectory.Exists)
                {
                    return handHistoryFolders.ToArray();
                }

                foreach (var settingsFile in profilesDirectory.GetFiles(SettingsFile, SearchOption.AllDirectories))
                {
                    try
                    {
                        var xmlValues = ReadXmlValues(settingsFile.FullName, new[] { ProfileHandHistoryId });

                        if (xmlValues.ContainsKey(ProfileHandHistoryId) && !string.IsNullOrWhiteSpace(xmlValues[ProfileHandHistoryId]))
                        {
                            handHistoryFolders.Add(xmlValues[ProfileHandHistoryId]);
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
                LogProvider.Log.Error(this, $"Could not read data from {profilesDirectoryPath}", e);
            }

            return handHistoryFolders.Distinct().ToArray();
        }

        protected string GetInstalledPath(string softwareName)
        {
            try
            {
                var uninstallKeys = new[] {  Registry.LocalMachine.OpenSubKey(RegistryUtils.UninstallRegistryPath32Bit),
                          Registry.LocalMachine.OpenSubKey(RegistryUtils.UninstallRegistryPath64Bit) };

                foreach (var uninstallKey in uninstallKeys)
                {
                    if (uninstallKey == null)
                    {
                        continue;
                    }

                    foreach (var subkey in uninstallKey.GetSubKeyNames().Select(keyName => uninstallKey.OpenSubKey(keyName)))
                    {
                        if (string.Equals(subkey?.GetValue(DisplayNameKeyValue) as string, softwareName, StringComparison.OrdinalIgnoreCase))
                        {
                            return subkey?.GetValue(InstallLocationKeyValue) as string;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could read data from registry for {softwareName}. [{Site}] ", e);
            }

            return string.Empty;
        }

        public ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            var installedPath = GetInstalledPath(RegistryDisplayName);

            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                IsEnabled = siteModel.Enabled,
                IsDetected = !string.IsNullOrEmpty(installedPath)
            };

            if (!validationResult.IsDetected)
            {
                return validationResult;
            }

            var profilesDirectoryPath = Path.Combine(installedPath, ProfilesFolder);

            try
            {
                var profilesDirectory = new DirectoryInfo(profilesDirectoryPath);

                if (!profilesDirectory.Exists)
                {
                    return validationResult;
                }

                foreach (var settingsFile in profilesDirectory.GetFiles(SettingsFile, SearchOption.AllDirectories))
                {
                    try
                    {
                        var xmlValues = ReadXmlValues(settingsFile.FullName, new[] { ProfileHandHistoryId, ProfileSaveHandHistoryId });

                        if (xmlValues.ContainsKey(ProfileHandHistoryId) && !string.IsNullOrWhiteSpace(xmlValues[ProfileHandHistoryId]))
                        {
                            validationResult.HandHistoryLocations.Add(xmlValues[ProfileHandHistoryId]);
                        }

                        if (xmlValues.ContainsKey(ProfileSaveHandHistoryId) && (!string.IsNullOrEmpty(xmlValues[ProfileSaveHandHistoryId]) && xmlValues[ProfileSaveHandHistoryId] != CorrectSaveHandHistoryTag))
                        {
                            var issue = string.Format(CommonResourceManager.Instance.GetResourceString("Error_WPN_Validation_SaveHandHistory"), RegistryDisplayName, settingsFile.Directory?.Name);
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
                LogProvider.Log.Error(this, $"Could not read data from {profilesDirectoryPath}", e);
            }

            return validationResult;
        }

        /// <summary>
        /// Reads xml tag value from text
        /// </summary>
        /// <param name="text">Xml</param>
        /// <returns>Result</returns>
        private Dictionary<string, string> ReadXmlValues(string file, string[] ids)
        {
            var result = ids.GroupBy(x => x).ToDictionary(x => x.Key, x => string.Empty);

            try
            {
                using (var xmlReader = XmlReader.Create(file))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.HasAttributes)
                        {
                            foreach (var id in ids)
                            {
                                var attribute = xmlReader.GetAttribute("id");

                                if (!string.IsNullOrEmpty(attribute) && attribute.Equals(id, StringComparison.OrdinalIgnoreCase))
                                {
                                    result[id] = xmlReader.ReadElementContentAsString();

                                    if (result.Values.All(x => !string.IsNullOrEmpty(x)))
                                    {
                                        return result;
                                    }
                                }
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