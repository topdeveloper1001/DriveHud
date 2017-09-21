//-----------------------------------------------------------------------
// <copyright file="Migration0022_AddMaxPlayersToPlayerstatistic.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using FluentMigrator;
using System;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(22)]
    public class Migration0022_AddMaxPlayersToPlayerstatistic : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #22");

            var doMigration = false;

            App.SplashScreen.Dispatcher.Invoke(() =>
            {
                var notificationViewModel = new NotificationViewModel
                {
                    Title = CommonResourceManager.Instance.GetResourceString("Message_Migration0022_Title"),
                    Message = CommonResourceManager.Instance.GetResourceString("Message_Migration0022_Text"),
                    Button2Text = CommonResourceManager.Instance.GetResourceString("Message_Migration0022_Run"),
                    Button2Action = () =>
                    {
                        doMigration = true;
                        App.SplashScreen.DataContext.CloseNotification();
                    },
                    Button3Text = CommonResourceManager.Instance.GetResourceString("Message_Migration0022_Cancel"),
                    Button3Action = () => App.SplashScreen.DataContext.CloseNotification()
                };

                App.SplashScreen.DataContext.ShowNotification(notificationViewModel);
            });

            if (doMigration)
            {
                try
                {
                    App.SplashScreen.Dispatcher.Invoke(() =>
                    {
                        App.SplashScreen.DataContext.Status = CommonResourceManager.Instance.GetResourceString("Message_Migration0022_Status");
                    });

                    var migrator = new PlayerStatisticMaxPlayersMigrator();
                    migrator.Update();

                    LogProvider.Log.Info("Migration #22 executed.");
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Migration #22 failed.", e);
                }
            }
            else
            {
                LogProvider.Log.Info("Migration #22 has been skipped by user.");
            }      
        }
    }
}