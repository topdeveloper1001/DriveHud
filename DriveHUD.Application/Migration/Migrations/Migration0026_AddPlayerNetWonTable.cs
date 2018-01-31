//-----------------------------------------------------------------------
// <copyright file="Migration0026_AddPlayerNetWonTable.cs" company="Ace Poker Solutions">
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
using System;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(26)]
    public class Migration0026_AddPlayerNetWonTable : Migration
    {
        private const string playerNetWonTableName = "PlayerNetWon";
        private const string playerNetWonPlayerIdIndexName = "PlayerNetWonPlayerId_IDX";

        private const string playerGameInfoTableName = "PlayerGameInfo";
        private const string playerGameInfoIdIndexName = "PlayerGameInfoPlayerId_IDX";

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #26.");

            try
            {
                CreatePlayerNetWonTable();
                CreatePlayerGameInfoTable();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #26 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #26 executed.");
        }

        public override void Down()
        {
            if (Schema.Table(playerNetWonTableName).Exists())
            {
                Delete.Table(playerNetWonTableName);
            }

            if (Schema.Table(playerGameInfoTableName).Exists())
            {
                Delete.Table(playerGameInfoTableName);
            }
        }

        private void CreatePlayerNetWonTable()
        {
            if (!Schema.Table(playerNetWonTableName).Exists())
            {
                LogProvider.Log.Info($"Creating \"{playerNetWonTableName}\" table.");

                Create.Table(playerNetWonTableName)
                    .WithColumn("PlayerNetWonId").AsInt32().NotNullable().PrimaryKey()
                    .WithColumn("PlayerId").AsInt32().NotNullable()
                    .WithColumn("Currency").AsInt32().NotNullable()
                    .WithColumn("NetWon").AsInt64().NotNullable().WithDefaultValue(0);

                Create.Index(playerNetWonPlayerIdIndexName)
                    .OnTable(playerNetWonTableName)
                    .OnColumn("PlayerId");
            }
            else
            {
                LogProvider.Log.Info($"Table \"{playerNetWonTableName}\" already exists.");
            }
        }

        private void CreatePlayerGameInfoTable()
        {
            if (!Schema.Table(playerGameInfoTableName).Exists())
            {
                LogProvider.Log.Info($"Creating \"{playerGameInfoTableName}\" table.");

                Create.Table(playerGameInfoTableName)
                    .WithColumn("PlayerGameInfoId").AsInt32().NotNullable().PrimaryKey()
                    .WithColumn("PlayerId").AsInt32().NotNullable()
                    .WithColumn("GameInfoId").AsInt32().NotNullable();

                Create.Index(playerGameInfoIdIndexName)
                    .OnTable(playerGameInfoTableName)
                    .OnColumn("PlayerId");
            }
            else
            {
                LogProvider.Log.Info($"Table \"{playerGameInfoTableName}\" already exists.");
            }
        }
    }
}