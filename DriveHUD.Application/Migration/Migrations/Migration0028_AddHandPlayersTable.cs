//-----------------------------------------------------------------------
// <copyright file="Migration0028_AddHandPlayersTable.cs" company="Ace Poker Solutions">
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
    [Migration(28)]
    public class Migration0028_AddHandPlayersTable : Migration
    {
        private const string HandsPlayersTable = "HandsPlayers";
        private const string HandsPlayersHandIdIndex = "HandsPlayersHandId_IDX";
        private const string HandsPlayersPlayerIdIndex = "HandsPlayersPlayerId_IDX";

        public override void Down()
        {
            try
            {
                if (!Schema.Table(HandsPlayersTable).Exists())
                {
                    return;
                }

                Delete.Table(HandsPlayersTable);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error("Rollback of Migration #28 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Rollback of Migration #28 executed.");
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #28.");

            try
            {
                CreateHandPlayersTable();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #28 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #28 executed.");
        }

        private void CreateHandPlayersTable()
        {
            if (!Schema.Table(HandsPlayersTable).Exists())
            {
                LogProvider.Log.Info($"Creating \"{HandsPlayersTable}\" table.");

                Execute.Sql($@"CREATE TABLE `{HandsPlayersTable}` (
	                `HandPlayerId`	INTEGER,
	                `HandId`	INTEGER NOT NULL,
	                `PlayerId`	INTEGER NOT NULL,
                    `Currency` INTEGER NOT NULL DEFAULT 1, 
                    `NetWon` INTEGER NOT NULL DEFAULT 0, 
                    FOREIGN KEY(`PlayerId`) REFERENCES `Players`(`PlayerId`) ON DELETE CASCADE,
	                PRIMARY KEY(`HandPlayerId`),
	                FOREIGN KEY(`HandId`) REFERENCES `HandHistories`(`HandHistoryId`) ON DELETE CASCADE)
                ;");

                Create.Index(HandsPlayersPlayerIdIndex)
                    .OnTable(HandsPlayersTable)
                    .OnColumn("PlayerId");

                Create.Index(HandsPlayersHandIdIndex)
                    .OnTable(HandsPlayersTable)
                    .OnColumn("HandId");

                return;
            }

            LogProvider.Log.Info($"Table \"{HandsPlayersTable}\" already exists.");
        }
    }
}