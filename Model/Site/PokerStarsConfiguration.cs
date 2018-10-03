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

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Microsoft.Win32;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Model.Site
{
    public class PokerStarsConfiguration : BaseSiteConfiguration, ISiteConfiguration
    {
        private static readonly string[] PossibleFolders = new string[] { "PokerStars", "PokerStars.EU", "PokerStars.USNJ", "PokerStars.PT", "PokerStars.UK", "PokerStars.FR",
            "PokerStars.DK", "PokerStars.BE", "PokerStars.BG", "PokerStars.IT", "PokerStars.ES", "PokerStars.EE", "PokerStars.CZ", "PokerStars.SH", "PokerStars.RO", "PokerStars.RUSO" };

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
        private const string psClientLaunchFile = "PokerStars.exe";
        private const string displayNameKey = "DisplayName";
        private const string displayNameKeyValuePattern = "PokerStars";
        private const string installLocationKey = "InstallLocation";
        private const string auditIni = "audit-files.ini";

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

            InstallAuditIni(validationResult);

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
        /// Gets the list of directories where PS clients are installed
        /// </summary>
        /// <returns></returns>
        private IList<DirectoryInfo> GetInstallPaths()
        {
            var directories = new List<DirectoryInfo>();

            try
            {
                var programFileFolders = new[] {
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                };

                // Check possible folders
                foreach (var programFileFolder in programFileFolders)
                {
                    foreach (var possibleFolder in PossibleFolders)
                    {
                        var launchFile = Path.Combine(programFileFolder, possibleFolder, psClientLaunchFile);

                        if (!File.Exists(launchFile))
                        {
                            continue;
                        }

                        var installationDirectory = Path.GetDirectoryName(launchFile);

                        try
                        {
                            var directory = new DirectoryInfo(installationDirectory);
                            directories.Add(directory);
                        }
                        catch (Exception e)
                        {
                            LogProvider.Log.Error(this, $"Could not get info about {installationDirectory}", e);
                        }
                    }
                }

                // Check registry for custom installations
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
                        var displayNameKeyValue = subkey?.GetValue(displayNameKey) as string;

                        if (string.IsNullOrEmpty(displayNameKeyValue) ||
                            displayNameKeyValue.IndexOf(displayNameKeyValuePattern, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            continue;
                        }

                        var installLocationKeyValue = subkey?.GetValue(installLocationKey) as string;

                        if (string.IsNullOrEmpty(installLocationKeyValue))
                        {
                            continue;
                        }

                        var launchFile = Path.Combine(installLocationKeyValue, psClientLaunchFile);

                        if (!File.Exists(launchFile))
                        {
                            continue;
                        }

                        try
                        {
                            var directory = new DirectoryInfo(installLocationKeyValue);
                            directories.Add(directory);
                        }
                        catch (Exception e)
                        {
                            LogProvider.Log.Error(this, $"Could not get info about {installLocationKeyValue}", e);
                        }
                    }
                }

                directories = directories
                    .Distinct(new LambdaComparer<DirectoryInfo>((x1, x2) => x1.FullName.Equals(x2.FullName, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Could not get installation folder of PS", ex);
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

        #region Audit methods

        private const string AuditSection = "Audit";
        private const string EnabledKey = "Enabled";
        private const string PathKey = "Path";
        private const string ProcessName = "PokerStars";

        public static string GetAuditPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PokerStars", "Audit");
        }

        public static string GetAuditPath(Process pokerStarsProcess)
        {
            if (pokerStarsProcess == null)
            {
                return GetAuditPath();
            }

            try
            {
                var pokerStarsFolder = Path.GetDirectoryName(pokerStarsProcess.MainModule.FileName);
                var auditIniFile = Path.Combine(pokerStarsFolder, auditIni);

                if (!File.Exists(auditIniFile))
                {
                    return GetAuditPath();
                }

                var path = IniFileHelpers.ReadValue(AuditSection, PathKey, auditIniFile);

                return !string.IsNullOrEmpty(path) ? path : GetAuditPath();
            }
            catch
            {
                return GetAuditPath();
            }
        }

        private void InstallAuditIni(SiteValidationResult validationResult)
        {
            try
            {
                var installPaths = GetInstallPaths();

                var isPokerStarsRunning = IsPokerStarsRunning();

                var auditPath = GetAuditPath();

                if (!Directory.Exists(auditPath))
                {
                    Directory.CreateDirectory(auditPath);
                    LogProvider.Log.Info($"Created '{auditPath}' folder");
                }

                foreach (var installPath in installPaths)
                {
                    var launchFile = Path.Combine(installPath.FullName, psClientLaunchFile);
                    TryLogPSLaunchFileVersion(launchFile);

                    var auditIniFile = Path.Combine(installPath.FullName, auditIni);

                    if (File.Exists(auditIniFile))
                    {
                        ValidateAuditInitFile(auditIniFile, auditPath);
                        continue;
                    }

                    CreateAuditInitFile(auditIniFile, auditPath);

                    if (isPokerStarsRunning)
                    {
                        var issue = string.Format(CommonResourceManager.Instance.GetResourceString("Error_PS_Validation_ZoomSupport"),
                                    installPath.Name);

                        validationResult.Issues.Add(issue);
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Could not enable zoom support for PS", ex);
            }
        }

        /// <summary>
        /// Validates audit ini file at the specified path
        /// </summary>
        /// <param name="auditIniFile">Path to ini file</param>
        /// <param name="auditPath">Path to folder where audit files will be stored</param>
        private void ValidateAuditInitFile(string auditIniFile, string auditPath)
        {
            try
            {
                var wasModified = false;
                var auditFileLines = File.ReadAllLines(auditIniFile);

                for (var i = 0; i < auditFileLines.Length; i++)
                {
                    if (auditFileLines[i].StartsWith(EnabledKey, StringComparison.Ordinal))
                    {
                        var enabledValue = auditFileLines[i].Substring(auditFileLines[i].IndexOf('=') + 1);

                        if (enabledValue != "1")
                        {
                            auditFileLines[i] = $"{EnabledKey}=1";
                            wasModified = true;
                        }
                    }

                    if (auditFileLines[i].StartsWith(PathKey, StringComparison.Ordinal))
                    {
                        var path = auditFileLines[i].Substring(auditFileLines[i].IndexOf('=') + 1);

                        if (!path.Equals(auditPath, StringComparison.OrdinalIgnoreCase))
                        {
                            LogProvider.Log.Warn($"PS audit path '{path}' isn't equal to default '{auditPath}' in '{auditIniFile}'");
                        }
                    }
                }

                if (wasModified)
                {
                    File.WriteAllLines(auditIniFile, auditFileLines, new UTF8Encoding(true));
                    LogProvider.Log.Info($"PS audit file '{auditIniFile}' was updated.");
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Could not validate {auditIniFile} file", ex);
            }
        }

        /// <summary>
        /// Creates audit ini file at the specified path
        /// </summary>
        /// <param name="auditIniFile">Path to ini file</param>
        /// <param name="auditPath">Path to folder where audit files will be stored</param>
        private void CreateAuditInitFile(string auditIniFile, string auditPath)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"[{AuditSection}]");
                sb.AppendLine($"{EnabledKey}=1");
                sb.AppendLine($"{PathKey}={auditPath}");

                File.WriteAllText(auditIniFile, sb.ToString(), new UTF8Encoding(true));
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Could not create {auditIniFile} file", ex);
                return;
            }

            LogProvider.Log.Info($"Created audit file at {auditIniFile}");
        }

        /// <summary>
        /// Writes to log the version of the specified PS client launch file
        /// </summary>
        /// <param name="launchFile">Launch file to write version to the log</param>
        /// <returns>True if version was logged; otherwise - false</returns>
        private bool TryLogPSLaunchFileVersion(string launchFile)
        {
            if (!File.Exists(launchFile))
            {
                return false;
            }

            try
            {
                var fvi = FileVersionInfo.GetVersionInfo(launchFile);

                if (!Version.TryParse(fvi.FileVersion, out Version fv) && fvi.FileMajorPart != 0)
                {
                    fv = new Version(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
                }

                if (fv != null)
                {
                    LogProvider.Log.Info($"PS client v.{fv} found at {launchFile}");
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Could not log version of PS launch file: {launchFile}", ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Detects whenever PS process is running
        /// </summary>
        /// <returns>True if process is running or can't be detected; otherwise - false</returns>
        private bool IsPokerStarsRunning()
        {
            try
            {
                var processes = Process.GetProcesses();
                return processes.Any(x => x.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Could not detect if PS client is running", ex);
                return true;
            }
        }

        #endregion
    }
}