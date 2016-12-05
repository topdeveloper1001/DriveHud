using DriveHUD.Common.Log;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(7)]
    public class Migration0007_NonDB_ChangeDefaultCustomProcessedDataLocation : Migration
    {
        public override void Up()
        {
            LogProvider.Log.Info("Running migration #7");

            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

            settings.SiteSettings.ProcessedDataLocation = Path.Combine(StringFormatter.GetAppDataFolderPath(), "ProcessedData");

            ServiceLocator.Current.GetInstance<ISettingsService>().SaveSettings(settings);

            LogProvider.Log.Info("Migration #7 executed.");
        }

        public override void Down()
        {
        }
    }
}
