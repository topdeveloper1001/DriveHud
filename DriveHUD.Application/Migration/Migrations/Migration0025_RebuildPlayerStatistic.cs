//-----------------------------------------------------------------------
// <copyright file="Migration0025_RebuildPlayerStatistic.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.SplashScreen;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Importers;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using System;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(25)]
    public class Migration0025_RebuildPlayerStatistic : Migration
    {
        private static readonly bool skipMigration = false;

        public override void Down()
        {
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #25");

            if (skipMigration)
            {
                LogProvider.Log.Info("Migration #25 has been skipped by system.");
                return;
            }

            var doMigration = false;

            App.SplashScreen.Dispatcher.Invoke(() =>
            {
                var notificationViewModel = new NotificationViewModel
                {
                    Title = CommonResourceManager.Instance.GetResourceString("Message_Migration0025_Title"),
                    Message = CommonResourceManager.Instance.GetResourceString("Message_Migration0025_Text"),
                    Button2Text = CommonResourceManager.Instance.GetResourceString("Message_Migration0025_Rebuild"),
                    Button2Action = () =>
                    {
                        doMigration = true;
                        App.SplashScreen.DataContext.CloseNotification();
                    },
                    Button3Text = CommonResourceManager.Instance.GetResourceString("Message_Migration0025_Cancel"),
                    Button3Action = () => App.SplashScreen.DataContext.CloseNotification()
                };

                App.SplashScreen.DataContext.ShowNotification(notificationViewModel);
            });

            if (doMigration)
            {
                try
                {
                    var playerStatisticReImporter = ServiceLocator.Current.GetInstance<IPlayerStatisticReImporter>();

                    App.SplashScreen.Dispatcher.Invoke(() =>
                    {
                        App.SplashScreen.DataContext.Status = CommonResourceManager.Instance.GetResourceString("Message_Migration0025_Status");
                        playerStatisticReImporter.InitializeProgress(App.SplashScreen.DataContext.Progress);
                    });
                                      
                    playerStatisticReImporter.ReImport();

                    LogProvider.Log.Info("Migration #25 executed.");
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Migration #25 failed.", e);
                }
            }
            else
            {
                LogProvider.Log.Info("Migration #25 has been skipped by user.");
            }
        }
    }
}