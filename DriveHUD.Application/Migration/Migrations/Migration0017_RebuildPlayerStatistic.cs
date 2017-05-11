//-----------------------------------------------------------------------
// <copyright file="Migration0017_RebuildPlayerStatistic.cs" company="Ace Poker Solutions">
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
    [Migration(17)]
    public class Migration0017_RebuildPlayerStatistic : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #17");

            var doMigration = false;

            App.SplashScreen.Dispatcher.Invoke(() =>
            {
                var notificationViewModel = new NotificationViewModel
                {
                    Title = CommonResourceManager.Instance.GetResourceString("Message_Migration0017_Title"),
                    Message = CommonResourceManager.Instance.GetResourceString("Message_Migration0017_Text"),
                    ConfirmButtonText = CommonResourceManager.Instance.GetResourceString("Message_Migration0017_Rebuild"),
                    ConfirmButtonAction = () =>
                    {
                        doMigration = true;
                        App.SplashScreen.DataContext.CloseNotification();
                    },
                    CancelButtonText = CommonResourceManager.Instance.GetResourceString("Message_Migration0017_Cancel"),
                    CancelButtonAction = () => App.SplashScreen.DataContext.CloseNotification()
                };

                App.SplashScreen.DataContext.ShowNotification(notificationViewModel);
            });

            if (doMigration)
            {
                try
                {
                    App.SplashScreen.Dispatcher.Invoke(() =>
                    {
                        App.SplashScreen.DataContext.Status = CommonResourceManager.Instance.GetResourceString("Message_Migration0017_Status");
                    });

                    var playerStatisticReImporter = ServiceLocator.Current.GetInstance<IPlayerStatisticReImporter>();
                    playerStatisticReImporter.ReImport();

                    LogProvider.Log.Info("Migration #17 executed.");
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Migration #17 failed.", e);
                }
            }
            else
            {
                LogProvider.Log.Info("Preparing migration #17 has been skipped by user.");
            }
        }
    }
}