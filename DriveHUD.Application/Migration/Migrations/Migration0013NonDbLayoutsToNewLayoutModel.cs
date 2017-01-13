using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DriveHUD.Application.ViewModels;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using FluentMigrator;
using Model;
using Model.Enums;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(13)]
    public class Migration0013NonDbLayoutsToNewLayoutModel : Migration
    {
        private const string layoutsFileSettings = "Layouts.xml";

        public override void Up()
        {
            LogProvider.Log.Info("Running migration #13");
            var settingsFolder = StringFormatter.GetAppDataFolderPath();

            if (!Directory.Exists(settingsFolder))
            {
                LogProvider.Log.Info($"Folder {settingsFolder} not found.");

                return;
            }

            var layoutsFile = Path.Combine(settingsFolder, layoutsFileSettings);

            if (File.Exists(layoutsFile))
            {
                HudSavedLayouts hudLayouts;
                try
                {
                    using (var fs = File.Open(layoutsFile, FileMode.Open))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));

                        hudLayouts = xmlSerializer.Deserialize(fs) as HudSavedLayouts;
                    }

                    if (hudLayouts != null && hudLayouts.Layouts != null && hudLayouts.Layouts.Any())
                    {
                        foreach (var hudSavedLayout in hudLayouts.Layouts)
                        {
                            
                        }
                    }
                    else
                    {
                        LogProvider.Log.Info("Wasn't able to find any active layouts.");
                    }
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(ex);
                }
            }
            else
            {
                LogProvider.Log.Info($"File {layoutsFile} not found.");
            }

            LogProvider.Log.Info("Migration #13 executed.");
        }

        public override void Down()
        {
        }

    }
}