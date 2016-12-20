﻿//-----------------------------------------------------------------------
// <copyright file="SQLiteBoostrapper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.MigrationService;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using System;
using System.IO;

namespace DriveHUD.Application.Bootstrappers
{
    internal class SQLiteBootstrapper : ISQLiteBootstrapper
    {
        private const string databaseResource = "DriveHUD.Common.Resources.Database.drivehud.db";

        public void InitializeDatabase()
        {
            var appData = StringFormatter.GetAppDataFolderPath();

            var isNewInstallation = !Directory.Exists(appData);

            if (isNewInstallation)
            {
                InitializeNewDatabase(appData);
                return;
            }

            var dbFile = StringFormatter.GetSQLiteDbFilePath();

            try
            {
                if (!File.Exists(dbFile))
                {
                    CreateNewDatabase();
                }

                var settingsModel = ServiceLocator.Current.GetInstance<ISettingsService>();
                var settings = settingsModel.GetSettings();
                var databaseSetting = settings.DatabaseSettings;

                LogProvider.Log.Debug("Run Migration Service");

                var connectionString = settings.GeneralSettings.IsSQLiteEnabled ?
                                            StringFormatter.GetSQLiteConnectionString() :
                                            StringFormatter.GetConnectionString(databaseSetting.Server, databaseSetting.Port, databaseSetting.Database, databaseSetting.User, databaseSetting.Password);

                var databaseType = settings.GeneralSettings.IsSQLiteEnabled ? DatabaseType.SQLite : DatabaseType.PostgreSQL;

                var migrationService = ServiceLocator.Current.GetInstance<IMigrationService>(databaseType.ToString());
                migrationService.MigrateToLatest(connectionString);

                LogProvider.Log.Debug("DH is up to date");
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't initialize DB", e);
            }
        }

        private void InitializeNewDatabase(string appData)
        {
            try
            {
                Directory.CreateDirectory(appData);
                CreateNewDatabase();                
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't initialize new DB", e);
            }
        }

        private void CreateNewDatabase()
        {
            try
            {
                var resourcesAssembly = typeof(ResourceRegistrator).Assembly;

                using (var stream = resourcesAssembly.GetManifestResourceStream(databaseResource))
                {
                    using (var fs = File.OpenWrite(StringFormatter.GetSQLiteDbFilePath()))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(fs);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't create DB", e);
            }
        }    
    }
}