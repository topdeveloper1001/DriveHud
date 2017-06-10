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

using DriveHUD.Application.MigrationService.Migrators;
using DriveHUD.Application.SplashScreen;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using FluentMigrator;
using Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(19)]
    public class Migration0019_MoveLayoutsToLayoutV2 : Migration
    {
        private readonly string layoutsV2Folder;
        private readonly string layoutsFolder;
        private readonly string mappingsFile;
        private readonly string layoutsExt;
        private readonly string layoutsBackupDirectoryPath;

        public Migration0019_MoveLayoutsToLayoutV2()
        {
            layoutsV2Folder = StringFormatter.GetLayoutsV2FolderPath();
            layoutsFolder = StringFormatter.GetLayoutsFolderPath();
            layoutsExt = StringFormatter.GetLayoutsExtension();
            layoutsBackupDirectoryPath = $"{StringFormatter.GetLayoutsFolderPath()}-backup";
            mappingsFile = $"{StringFormatter.GetLayoutsMappings()}{layoutsExt}";
        }

        public override void Down()
        {
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #19");

            var skipMigration = false;
            var shutdown = false;

            while (!skipMigration)
            {
                if (shutdown)
                {
                    continue;
                }

                try
                {
                    if (!Directory.Exists(layoutsV2Folder))
                    {
                        Directory.CreateDirectory(layoutsV2Folder);
                    }

                    var layoutDirectoryInfo = new DirectoryInfo(layoutsFolder);

                    if (!layoutDirectoryInfo.Exists)
                    {
                        LogProvider.Log.Error($"Layout directory '{layoutsFolder}' doesn't exist. Default layouts will be used. Migration #19 has been skipped.");
                        return;
                    }

                    var layoutFiles = layoutDirectoryInfo.GetFiles($"*{layoutsExt}");

                    var filesToDelete = new List<FileInfo>();

                    var isMapping = false;

                    // copy layouts to new folder
                    foreach (var layoutFile in layoutFiles)
                    {
                        if (IsHudLayoutInfoV2(layoutFile.FullName) || (isMapping = IsMappingsFile(layoutFile.FullName)))
                        {
                            var destination = Path.Combine(layoutsV2Folder, layoutFile.Name);

                            layoutFile.CopyTo(destination, true);

                            LogProvider.Log.Info($"'{layoutFile.FullName}' has been copied to '{destination}'");

                            if (!isMapping)
                            {
                                filesToDelete.Add(layoutFile);
                            }
                        }
                    }

                    // delete copied files
                    filesToDelete.ForEach(x =>
                    {
                        x.Delete();
                        LogProvider.Log.Info($"'{x.FullName}' has been deleted");
                    });

                    var layoutsBackupDirectoryInfo = new DirectoryInfo(layoutsBackupDirectoryPath);

                    // copy backup files to old LayoutFolder
                    if (layoutsBackupDirectoryInfo.Exists)
                    {
                        var backupFiles = layoutsBackupDirectoryInfo.GetFiles($"*{layoutsExt}");

                        foreach (var backupFile in backupFiles)
                        {
                            if (IsHudLayoutInfo(backupFile.FullName) || IsMappingsFile(backupFile.FullName))
                            {
                                var destination = Path.Combine(layoutsFolder, backupFile.Name);

                                backupFile.CopyTo(destination, true);

                                LogProvider.Log.Info($"'{backupFile.FullName}' has been copied to '{destination}'");
                            }
                        }
                    }

                    LogProvider.Log.Info("Migration #19 executed.");
                    return;
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error("Migration", $"Migration #19 failed.", ex);

                    App.SplashScreen.Dispatcher.Invoke(() =>
                    {
                        var notificationViewModel = new NotificationViewModel
                        {
                            Title = CommonResourceManager.Instance.GetResourceString("Message_Migration0019_Title"),
                            Message = CommonResourceManager.Instance.GetResourceString("Message_Migration0019_Text"),
                            Button1Text = CommonResourceManager.Instance.GetResourceString("Message_Migration0019_Retry"),
                            Button1Action = () => App.SplashScreen.DataContext.CloseNotification(),
                            Button2Text = CommonResourceManager.Instance.GetResourceString("Message_Migration0019_Skip"),
                            Button2Action = () =>
                            {
                                skipMigration = true;
                                LogProvider.Log.Info("Migration #19 has been skipped.");
                                App.SplashScreen.DataContext.CloseNotification();
                            },
                            Button3Text = CommonResourceManager.Instance.GetResourceString("Message_Migration0019_Exit"),
                            Button3Action = () =>
                            {
                                try
                                {
                                    shutdown = true;
                                    App.SplashScreen.DataContext.CloseNotification();
                                    App.SplashScreen.CloseSplashScreen();
                                    LogProvider.Log.Info("DriveHUD exited.");
                                    Environment.Exit(0);
                                }
                                catch (Exception e)
                                {
                                    LogProvider.Log.Error(this, e);
                                }
                            }
                        };

                        App.SplashScreen.DataContext.ShowNotification(notificationViewModel);
                    });
                }
            }
        }

        /// <summary>
        /// Determines whenever the specified file is a serialized <see cref="HudLayoutInfoV2"/> 
        /// </summary>
        /// <param name="fileName">Path to the specified file</param>
        /// <returns>True if file is a serialized <see cref="HudLayoutInfoV2"/>, otherwise - false</returns>
        private bool IsHudLayoutInfoV2(string fileName)
        {
            try
            {
                var canDeserialize = MigrationUtils.CanDeserialize<HudLayoutInfoV2>(fileName);
                return canDeserialize;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error("Migration", $"Layout (v2) has not been loaded on the '{fileName}' path.", e);
            }

            return false;
        }

        /// <summary>
        /// Determines whenever the specified file is a serialized <see cref="HudLayoutInfo"/> 
        /// </summary>
        /// <param name="fileName">Path to the specified file</param>
        /// <returns>True if file is a serialized <see cref="HudLayoutInfo"/>, otherwise - false</returns>
        private bool IsHudLayoutInfo(string fileName)
        {
            try
            {
                var canDeserialize = MigrationUtils.CanDeserialize<HudLayoutInfo>(fileName);
                return canDeserialize;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error("Migration", $"Layout (v1) has not been loaded on the '{fileName}' path.", e);
            }

            return false;
        }

        /// <summary>
        /// Determines whenever the specified file is a serialized <see cref="HudLayoutMappings"/> 
        /// </summary>
        /// <param name="fileName">Path to the specified file</param>
        /// <returns>True if file is a serialized <see cref="HudLayoutMappings"/>, otherwise - false</returns>
        private bool IsMappingsFile(string fileName)
        {
            if (!Path.GetFileName(fileName).Equals(mappingsFile, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                var canDeserialize = MigrationUtils.CanDeserialize<HudLayoutMappings>(fileName);
                return canDeserialize;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error("Migration", $"Mappings has not been loaded on the '{fileName}' path.", e);
            }

            return false;
        }
    }
}