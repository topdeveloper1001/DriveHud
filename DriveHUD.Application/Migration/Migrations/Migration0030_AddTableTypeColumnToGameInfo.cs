//-----------------------------------------------------------------------
// <copyright file="Migration0030_AddTableTypeColumnToGameInfo.cs" company="Ace Poker Solutions">
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
using FluentMigrator;
using System;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(30)]
    public class Migration0030_AddTableTypeColumnToGameInfo : Migration
    {
        private const string gameInfoTableName = "GameInfo";
        private const string tableTypeColumnName = "TableType";

        public override void Down()
        {          
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #30.");

            try
            {
                var gameInfoTable = Schema.Table(gameInfoTableName);

                if (!gameInfoTable.Exists())
                {
                    throw new InvalidOperationException($"Table {gameInfoTableName} doesn't exist.");
                }

                if (!gameInfoTable.Column(tableTypeColumnName).Exists())
                {
                    Create
                        .Column(tableTypeColumnName)
                        .OnTable(gameInfoTableName)
                        .AsInt64().NotNullable()
                        .WithDefaultValue(1);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #30 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #30 executed.");
        }
    }
}