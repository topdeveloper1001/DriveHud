//-----------------------------------------------------------------------
// <copyright file="Migration0018_MigrationToHUD_V2.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Log;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using Model;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(18)]
    public class Migration0018_MigrationToHUD_V2 : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #18");

            try
            {
                var layoutsDirectoryPath = StringFormatter.GetLayoutsFolderPath();
                var layoutsDirectory = new DirectoryInfo(layoutsDirectoryPath);
                var layoutsTempDirectoryPath = $"{StringFormatter.GetLayoutsFolderPath()}-temp";
                var layoutsBackupDirectoryPath = $"{StringFormatter.GetLayoutsFolderPath()}-backup";
                var layoutsExt = StringFormatter.GetLayoutsExtension();
                var mappingsFile = $"{StringFormatter.GetLayoutsMappings()}{layoutsExt}";

                var searchPattern = $"*{layoutsExt}";

                if (Directory.Exists(layoutsTempDirectoryPath))
                {
                    Directory.Delete(layoutsTempDirectoryPath, true);
                }

                Directory.CreateDirectory(layoutsTempDirectoryPath);

                var layoutsFiles = layoutsDirectory.GetFiles(searchPattern).Where(x => x.Name != mappingsFile).ToArray();

                var layoutMigrator = ServiceLocator.Current.GetInstance<Migrators.ILayoutMigrator>();

                foreach (var layoutFile in layoutsFiles)
                {
                    try
                    {
                        var layout = ReadLayoutInfo(layoutFile.FullName);

                        var migratedLayout = layoutMigrator.Migrate(layout);
                        var migratedLayoutFile = Path.Combine(layoutsTempDirectoryPath, layoutFile.Name);

                        SaveLayoutInfo(migratedLayoutFile, migratedLayout);
                    }
                    catch (Exception e)
                    {
                        LogProvider.Log.Error(this, $"{layoutFile.FullName} has not been migrated.", e);
                    }
                }

                var mappingsFilePath = Path.Combine(layoutsDirectoryPath, mappingsFile);

                if (File.Exists(mappingsFilePath))
                {
                    var mappingsFileTempPath = Path.Combine(layoutsTempDirectoryPath, mappingsFile);
                    File.Copy(mappingsFilePath, mappingsFileTempPath);
                }

                layoutsDirectory.MoveTo(layoutsBackupDirectoryPath);
                Directory.Move(layoutsTempDirectoryPath, layoutsDirectoryPath);

                LogProvider.Log.Info("Migration #18 executed.");
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Migration #18 failed.", ex);
            }
        }

        /// <summary>
        /// Reads layout v1 from the specified file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private HudLayoutInfo ReadLayoutInfo(string file)
        {
            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfo));
                var hudLayoutInfo = xmlSerializer.Deserialize(fs) as HudLayoutInfo;
                return hudLayoutInfo;
            }
        }

        /// <summary>
        /// Saves layout v2 to the specified file
        /// </summary>
        /// <param name="layoutFile"></param>
        /// <param name="layoutInfo"></param>
        private void SaveLayoutInfo(string layoutFile, HudLayoutInfoV2 layoutInfo)
        {
            using (var fs = File.Open(layoutFile, FileMode.Create))
            {
                var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfoV2));
                xmlSerializer.Serialize(fs, layoutInfo);
            }
        }
    }
}