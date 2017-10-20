//-----------------------------------------------------------------------
// <copyright file="Migration0024_AddAutoNoteColumnToPlayerNotes.cs" company="Ace Poker Solutions">
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
    [Migration(24)]
    public class Migration0024_AddAutoNoteColumnToPlayerNotes : Migration
    {
        private const string playerNotesTableName = "PlayerNotes";
        private const string playerNotesTempTableName = "PlayerNotesTemp";
        private const string playerNotesIndexName = "PlayerNotesPlayerId_IDX";
        private const string autoNoteColumnName = "AutoNote";
        private const string noteColumnName = "Note";

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #24.");

            try
            {
                if (Schema.Table(playerNotesTempTableName).Exists())
                {
                    Delete.Table(playerNotesTempTableName);
                }

                if (Schema.Table(playerNotesTableName).Exists())
                {
                    if (Schema.Table(playerNotesTableName).Index(playerNotesIndexName).Exists())
                    {
                        Delete.Index(playerNotesIndexName).OnTable(playerNotesTableName);
                    }

                    Rename.Table(playerNotesTableName).To(playerNotesTempTableName);
                }

                Create.Table(playerNotesTableName)
                    .WithColumn("PlayerNoteId").AsInt32().PrimaryKey().NotNullable()
                    .WithColumn("PlayerId").AsInt32().NotNullable()
                    .WithColumn("Note").AsString().NotNullable()
                    .WithColumn("CardRange").AsString().Nullable()
                    .WithColumn("Timestamp").AsDateTime().Nullable()
                    .WithColumn("IsAutoNote").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("GameNumber").AsInt64().Nullable()
                    .WithColumn("PokerSiteId").AsInt32().NotNullable();

                Create.Index(playerNotesIndexName).OnTable(playerNotesTableName).OnColumn("PlayerId");

                Execute.Sql($"INSERT INTO {playerNotesTableName} (PlayerNoteId, PlayerId, Note, PokerSiteId) SELECT PlayerNoteId, PlayerId, Note, PokerSiteId FROM {playerNotesTempTableName} WHERE Note is not null");
                Delete.Table(playerNotesTempTableName);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #24 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #24 executed.");
        }

        public override void Down()
        {
            LogProvider.Log.Info("Rolling back migration #24.");

            try
            {
                if (Schema.Table(playerNotesTempTableName).Exists())
                {
                    if (Schema.Table(playerNotesTableName).Exists())
                    {
                        LogProvider.Log.Info($"Deleting \"{playerNotesTableName}\" table.");
                        Delete.Table(playerNotesTableName);
                        LogProvider.Log.Info($"Renaming \"{playerNotesTempTableName}\" table to \"{playerNotesTableName}\".");
                        Rename.Table(playerNotesTempTableName).To(playerNotesTableName);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #24 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #24 rolled back.");
        }
    }
}
