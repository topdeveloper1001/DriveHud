//-----------------------------------------------------------------------
// <copyright file="Migration0021_AddImportedFilesTable.cs" company="Ace Poker Solutions">
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
    [Migration(21)]
    public class Migration0021_AddImportedFilesTable : Migration
    {
        private const string importedFilesTableName = "ImportedFiles";
        private const string importedFilesFileNameIndexName = "ImportedFilesFileName_IDX";

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #21.");

            try
            {
                if (!Schema.Table(importedFilesTableName).Exists())
                {
                    LogProvider.Log.Info($"Creating \"{importedFilesTableName}\" table.");

                    Create.Table(importedFilesTableName)
                        .WithColumn("ImportedFileId").AsInt32().NotNullable().PrimaryKey()
                        .WithColumn("FileName").AsString().NotNullable()
                        .WithColumn("FileSize").AsInt64().NotNullable()
                        .WithColumn("LastWriteTime").AsDateTime().NotNullable();

                    Create.Index(importedFilesFileNameIndexName)
                        .OnTable(importedFilesTableName)
                        .OnColumn("FileName");
                }
                else
                {
                    LogProvider.Log.Info($"Table \"{importedFilesTableName}\" already exists.");
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #21 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #21 executed.");
        }

        public override void Down()
        {
            if (Schema.Table(importedFilesTableName).Exists())
            {
                Delete.Table(importedFilesTableName);
            }
        }
    }
}