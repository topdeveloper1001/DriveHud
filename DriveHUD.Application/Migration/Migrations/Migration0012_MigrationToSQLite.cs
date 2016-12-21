//-----------------------------------------------------------------------
// <copyright file="Migration0012_MigrationToSQLite.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Linq;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using System;
using System.Diagnostics;
using System.IO;
using DriveHUD.Application.Bootstrappers;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(12)]
    public class Migration0012_MigrationToSQLite : Migration
    {
        private const string migrationProcessFile = "DriveHUD.DBMigration.exe";

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #12");

            if (!File.Exists(migrationProcessFile))
            {
                throw new FileNotFoundException("Migration isn't possible. Migration tool has not been found.", migrationProcessFile);
            }

            LogProvider.Log.Info("Migration #12 start executing");

            RollbackMigration();

            var sqliteBootstrapper = ServiceLocator.Current.GetInstance<ISQLiteBootstrapper>();
            sqliteBootstrapper.CreateNewDatabase();

            var migrationProcess = new Process();
            migrationProcess.StartInfo.FileName = migrationProcessFile;
            migrationProcess.StartInfo.UseShellExecute = false;
            migrationProcess.StartInfo.RedirectStandardOutput = true;
            migrationProcess.StartInfo.CreateNoWindow = true;

            migrationProcess.Start();

            string output;

            while ((output = migrationProcess.StandardOutput.ReadLine()) != null)
            {
                App.SplashScreen.Dispatcher.Invoke(() =>
                {
                    App.SplashScreen.DataContext.Status = output;
                });

                LogProvider.Log.Info(output);
            }

            migrationProcess.WaitForExit();

            if (migrationProcess.ExitCode != 0)
            {
                LogProvider.Log.Info("Migration #12 failed.");
                LogProvider.Log.Info("Rollback changes");

                RollbackMigration();

                throw new Exception($"Migration activity #12 failed");
            }

            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();

            var settings = settingsService.GetSettings();
            settings.GeneralSettings.IsSQLiteEnabled = true;
            settingsService.SaveSettings(settings);

            LogProvider.Log.Info("Migration #12 executed.");
        }

        public override void Down()
        {
        }

        private void RollbackMigration()
        {
            try
            {
                var processes = Process.GetProcessesByName("DriveHUD.DBMigration");
                processes.ForEach(x => x.Kill());

                var dbFile = StringFormatter.GetSQLiteDbFilePath();

                if (File.Exists(dbFile))
                {
                    File.Delete(dbFile);
                }

                var dbFolder = StringFormatter.GetPlayerStatisticDataFolderPath();

                if (Directory.Exists(dbFolder))
                {
                    Directory.Delete(dbFolder, true);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Rollback failed", e);
            }
        }
    }
}