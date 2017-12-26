//-----------------------------------------------------------------------
// <copyright file="StringFormatter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using Microsoft.Practices.ServiceLocation;
using Model.Importer;
using Model.Settings;
using System;
using System.IO;

namespace Model
{
    public static class StringFormatter
    {
        private static ISettingsService SettingsService
        {
            get { return ServiceLocator.Current.GetInstance<ISettingsService>(); }
        }

        public static string GetAppDataFolderPath()
        {
            var appdata = CommonResourceManager.Instance.GetResourceString(ResourceStrings.AppDataFolder);
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appdata);
        }

        public static string GetLayoutsV2FolderPath()
        {
            return Path.Combine(GetAppDataFolderPath(), CommonResourceManager.Instance.GetResourceString(ResourceStrings.LayoutsV2Folder));
        }

        public static string GetLayoutsFolderPath()
        {
            return Path.Combine(GetAppDataFolderPath(), CommonResourceManager.Instance.GetResourceString(ResourceStrings.LayoutsFolder));
        }

        public static string GetLayoutsExtension()
        {
            return CommonResourceManager.Instance.GetResourceString(ResourceStrings.LayoutsExtension);
        }

        public static string GetLayoutsMappings()
        {
            return CommonResourceManager.Instance.GetResourceString(ResourceStrings.LayoutsMappings);
        }

        public static string GetProcessedDataFolderPath()
        {
            return Path.Combine(SettingsService.GetSettings().SiteSettings.ProcessedDataLocation);
        }

        public static string GetPlayersDataFolderPath()
        {
            return Path.Combine(GetAppDataFolderPath(), CommonResourceManager.Instance.GetResourceString(ResourceStrings.DefaultPlayersFolderName));
        }

        public static string GetPlayerStatisticDataFolderPath()
        {
            return Path.Combine(GetAppDataFolderPath(), CommonResourceManager.Instance.GetResourceString(ResourceStrings.DefaultPlayerStatisticFolderName));
        }

        public static string GetPlayerStatisticDataTempFolderPath()
        {
            return Path.Combine(GetAppDataFolderPath(), CommonResourceManager.Instance.GetResourceString(ResourceStrings.PlayerStatisticTempFolderName));
        }

        public static string GetPlayerStatisticDataBackupFolderPath()
        {
            return Path.Combine(GetAppDataFolderPath(), CommonResourceManager.Instance.GetResourceString(ResourceStrings.PlayerStatisticBackupFolderName));
        }

        public static string GetPlayerStatisticDataOldFolderPath()
        {
            return Path.Combine(GetAppDataFolderPath(), CommonResourceManager.Instance.GetResourceString(ResourceStrings.PlayerStatisticOldFolderName));
        }

        public static string GetPlayerStatisticExtension()
        {
            return CommonResourceManager.Instance.GetResourceString(ResourceStrings.DefaultPlayerStatisticExtension);
        }

        public static string GetActivePlayerFilePath()
        {
            return Path.Combine(GetAppDataFolderPath(), CommonResourceManager.Instance.GetResourceString(ResourceStrings.ActivePlayerFileName));
        }

        public static string ActionLineSeparator = "-";

        public static string GetDateString(DateTime utcDateTime)
        {
            return Converter.ToLocalizedDateTime(utcDateTime).ToString("MM/dd/yyyy");
        }

        public static string GetDateTimeString(DateTime utcDateTime)
        {
            return Converter.ToLocalizedDateTime(utcDateTime).ToString("MM/dd/yyyy HH.mm tt");
        }

        public static string GetReplayerHeaderString(string stakeLevel, DateTime handDate)
        {
            var header = CommonResourceManager.Instance.GetResourceString(ResourceStrings.ReplayerHeaderResourceString);
            return string.Format(header, stakeLevel, GetDateTimeString(handDate));
        }

        /// <summary>
        /// Gets the path to the folder with logs
        /// </summary>
        /// <returns>Path to the folder with logs</returns>
        public static string GetLogsFolderPath()
        {
            var logsFolder = CommonResourceManager.Instance.GetResourceString(ResourceStrings.LogsFolder);
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logsFolder);
        }

        /// <summary>
        /// Gets the path to the folder with modules
        /// </summary>
        /// <returns>Path to the folder with modules</returns>
        public static string GetModulesFolderPath()
        {
            var modulesFolder = CommonResourceManager.Instance.GetResourceString(ResourceStrings.ModulesFolder);
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, modulesFolder);
        }

        public static string GetImporterPipeAddress()
        {
            return CommonResourceManager.Instance.GetResourceString(ResourceStrings.ImporterPipeAddress);
        }

        #region DB strings

        public static string GetConnectionString(string server, string port, string database, string user, string password)
        {
            return string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};Persist Security Info=true;Convert Infinity DateTime=true", server, port, database, user, password);
        }

        public static string GetSQLiteDbFilePath()
        {
            var appData = GetAppDataFolderPath();
            var dbFileName = CommonResourceManager.Instance.GetResourceString(ResourceStrings.DbFileName);

            return Path.Combine(appData, dbFileName);
        }

        public static string GetSQLiteConnectionString()
        {
            var dbFile = GetSQLiteDbFilePath();
            return $"Data Source={dbFile};Version=3;foreign keys=true;journal_mode=WAL;";
        }

        #endregion

        #region App store strings

        public static string GetAppStoreDataFolder()
        {
            var appStoreData = CommonResourceManager.Instance.GetResourceString(ResourceStrings.AppStoreDataFolder);
            return Path.Combine(GetAppDataFolderPath(), appStoreData);
        }

        public static string GetAppStoreLocalProductRepo()
        {
            var data = CommonResourceManager.Instance.GetResourceString(ResourceStrings.AppStoreLocalProductRepo);
            return Path.Combine(GetAppStoreDataFolder(), data);
        }

        public static string GetAppStoreLocalTrainingProductRepo()
        {
            var data = CommonResourceManager.Instance.GetResourceString(ResourceStrings.AppStoreLocalTrainingRepo);
            return Path.Combine(GetAppStoreDataFolder(), data);
        }

        public static string GetAppStoreLocalTempProductRepo()
        {
            var data = CommonResourceManager.Instance.GetResourceString(ResourceStrings.AppStoreLocalTempProductRepo);
            return Path.Combine(GetAppStoreDataFolder(), data);
        }

        public static string GetAppStoreLocalTempTrainingProductRepo()
        {
            var data = CommonResourceManager.Instance.GetResourceString(ResourceStrings.AppStoreLocalTempTrainingProductRepo);
            return Path.Combine(GetAppStoreDataFolder(), data);
        }

        public static string GetAppStoreRemoteProductRepo()
        {
            var data = CommonResourceManager.Instance.GetResourceString(ResourceStrings.AppStoreRemoteProductRepo);
            return data;
        }

        public static string GetAppStoreRemoteTrainingProductRepo()
        {
            var data = CommonResourceManager.Instance.GetResourceString(ResourceStrings.AppStoreRemoteTrainingProductRepo);
            return data;
        }

        public static string GetAppStoreRemoteProductHash()
        {
            var data = CommonResourceManager.Instance.GetResourceString(ResourceStrings.AppStoreRemoteProductHash);
            return data;
        }

        public static string GetAppStoreRemoteTrainingProductHash()
        {
            var data = CommonResourceManager.Instance.GetResourceString(ResourceStrings.AppStoreRemoteTrainingProductHash);
            return data;
        }     

        #endregion
    }
}