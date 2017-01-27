//-----------------------------------------------------------------------
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
using Npgsql;
using System;
using System.IO;
using System.Net.Sockets;

namespace DriveHUD.Application.Bootstrappers
{
    internal class SQLiteBootstrapper : ISQLiteBootstrapper
    {
        private const string databaseResource = "DriveHUD.Common.Resources.Database.drivehud.db";

        private ISettingsService settingsService;

        public void InitializeDatabase()
        {
            var appData = StringFormatter.GetAppDataFolderPath();

            var isNewInstallation = !Directory.Exists(appData);

            settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();

            if (isNewInstallation)
            {
                InitializeNewDatabase(appData);
                return;
            }

            var settings = settingsService.GetSettings();
            var databaseSetting = settings.DatabaseSettings;

            var dbFile = StringFormatter.GetSQLiteDbFilePath();

            try
            {
                LogProvider.Log.Debug("Run Migration Service");

                var connectionString = settings.GeneralSettings.IsSQLiteEnabled ?
                                            StringFormatter.GetSQLiteConnectionString() :
                                            StringFormatter.GetConnectionString(databaseSetting.Server, databaseSetting.Port, databaseSetting.Database, databaseSetting.User, databaseSetting.Password);

                var databaseType = settings.GeneralSettings.IsSQLiteEnabled ? DatabaseType.SQLite : DatabaseType.PostgreSQL;

                if (settings.GeneralSettings.IsSQLiteEnabled && !File.Exists(dbFile))
                {
                    CreateNewDatabase();
                }

                var migrationService = ServiceLocator.Current.GetInstance<IMigrationService>(databaseType.ToString());
                migrationService.MigrateToLatest(connectionString);

                LogProvider.Log.Debug("DH is up to date");

                var dbFileInfo = new FileInfo(dbFile);

                if (!dbFileInfo.Exists || dbFileInfo.Length == 0)
                {
                    LogProvider.Log.Debug("DB file not found or corrupted");
                    InitializeNewDatabase(appData);
                }
            }
            catch (NpgsqlException e)
            {
                if (!settings.GeneralSettings.IsSQLiteEnabled)
                {
                    LogProvider.Log.Info($"PostgreSQL DB isn't available or corrupted. Error: {e.Message}");

                    var messageBox = System.Windows.Forms.MessageBox.Show(owner: new System.Windows.Forms.Form { TopMost = true }, text: CommonResourceManager.Instance.GetResourceString("Error_PG_MigrationWarningMessage"),
                           caption: CommonResourceManager.Instance.GetResourceString("Error_PG_MigrationWarning"), buttons: System.Windows.Forms.MessageBoxButtons.YesNo);

                    if (messageBox == System.Windows.Forms.DialogResult.Yes)
                    {
                        LogProvider.Log.Info("New SQLite database will be initialized.");
                        InitializeNewDatabase(appData);
                        return;
                    }

                    throw;
                }
            }
            catch (Exception e)
            {
                if (!settings.GeneralSettings.IsSQLiteEnabled && (e as SocketException)?.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    LogProvider.Log.Info("PostgreSQL not found. Creating SQLite database and finish migration.");
                    InitializeNewDatabase(appData);
                    return;
                }

                LogProvider.Log.Error(this, "Couldn't complete migration actions.", e);
                throw;
            }
        }

        private void InitializeNewDatabase(string appData)
        {
            try
            {
                if (!Directory.Exists(appData))
                {
                    Directory.CreateDirectory(appData);
                }

                var dbFileInfo = new FileInfo(StringFormatter.GetSQLiteDbFilePath());

                var createNewDatabase = false;

                if (dbFileInfo.Exists && dbFileInfo.Length == 0)
                {
                    LogProvider.Log.Info("Deleting corrupted DB file");
                    dbFileInfo.Delete();
                    createNewDatabase = true;
                }
                else if (!dbFileInfo.Exists)
                {
                    createNewDatabase = true;
                }

                if (createNewDatabase)
                {
                    CreateNewDatabase();
                }

                var settings = settingsService.GetSettings();
                settings.GeneralSettings.IsSQLiteEnabled = true;
                settingsService.SaveSettings(settings);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't initialize new DB", e);
            }
        }

        public void CreateNewDatabase()
        {
            try
            {
                LogProvider.Log.Info("Creating new database");

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