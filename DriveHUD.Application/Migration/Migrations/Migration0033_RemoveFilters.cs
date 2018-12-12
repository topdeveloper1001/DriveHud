//-----------------------------------------------------------------------
// <copyright file="Migration0033_AddTournamentIndexOnHandHistory.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using FluentMigrator;
using Model;
using System;
using System.IO;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(33)]
    public class Migration0033_RemoveFilters : Migration
    {
        private static readonly string[] filters = new[] { "Cash_filter.df", "Tournament_filter.df" };

        public override void Down()
        {
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #33.");

            try
            {
                var appFolder = StringFormatter.GetAppDataFolderPath();

                foreach (var filter in filters)
                {
                    var filterFile = Path.Combine(appFolder, filter);

                    if (File.Exists(filterFile))
                    {
                        File.Delete(filterFile);
                    }
                }

            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #33 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #33 executed.");
        }
    }
}
