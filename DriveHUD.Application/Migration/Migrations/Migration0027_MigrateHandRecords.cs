//-----------------------------------------------------------------------
// <copyright file="Migration0027_MigrateHandRecords.cs" company="Ace Poker Solutions">
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
using HandHistories.Objects.GameDescription;
using Model;
using System;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(27)]
    public class Migration0027_MigrateHandRecords : Migration
    {
        private const string playerNetWonTableName = "PlayerNetWon";
        private const string playerGameInfoTableName = "PlayerGameInfo";
        private const string handRecordsTableName = "HandRecords";

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #27.");

            try
            {
                TruncateTable(playerNetWonTableName);
                TruncateTable(playerGameInfoTableName);
                FillPlayerNetWonTable();
                FillPlayerGameInfoTable();
                DeleteHandRecordsTable();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #27 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #27 executed.");
        }

        public override void Down()
        {
            try
            {
                TruncateTable(playerNetWonTableName);
                TruncateTable(playerGameInfoTableName);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Rollback of Migration #27 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Rollback of Migration #27 executed.");
        }

        private void FillPlayerNetWonTable()
        {
            if (!Schema.Table(handRecordsTableName).Exists())
            {
                return;
            }

            using (var session = ModelEntities.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var currencyId = (int)Currency.USD;

                    var sqlQuery = session
                        .CreateSQLQuery($@"insert into {playerNetWonTableName} (PlayerId, Currency, NetWon) 
                                select h.PlayerId, {currencyId}, sum(h.NetWon) 
                                from {handRecordsTableName} h 
                                join GameInfo g on h.GameInfoId = g.GameInfoId 
                                where g.IsTournament = 0 and NetWon <> 0 
                                group by PlayerId");

                    var result = sqlQuery.ExecuteUpdate();

                    LogProvider.Log.Info($"Inserted {result} rows into '{playerNetWonTableName}'");

                    transaction.Commit();
                }
            }
        }

        private void FillPlayerGameInfoTable()
        {
            if (!Schema.Table(handRecordsTableName).Exists())
            {
                return;
            }

            using (var session = ModelEntities.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var sqlQuery = session
                        .CreateSQLQuery($@"insert into {playerGameInfoTableName} (PlayerId, GameInfoId) 
                                select PlayerId, GameInfoId 
                                from {handRecordsTableName}                                                               
                                group by PlayerId, GameInfoId");

                    var result = sqlQuery.ExecuteUpdate();

                    LogProvider.Log.Info($"Inserted {result} rows into '{playerGameInfoTableName}'");

                    transaction.Commit();
                }
            }
        }

        private void TruncateTable(string tableName)
        {
            LogProvider.Log.Info($"Truncating {tableName}");

            using (var session = ModelEntities.OpenStatelessSession())
            {
                var sqlQuery = session
                           .CreateSQLQuery($"delete from {tableName}");

                sqlQuery.ExecuteUpdate();
            }
        }

        private void DeleteHandRecordsTable()
        {
            if (!Schema.Table(handRecordsTableName).Exists())
            {
                return;
            }

            LogProvider.Log.Info($"Deleting {handRecordsTableName} table");
            Delete.Table(handRecordsTableName);
            LogProvider.Log.Info($"{handRecordsTableName} table has been successfully deleted.");
        }
    }
}