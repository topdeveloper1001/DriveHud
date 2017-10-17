//-----------------------------------------------------------------------
// <copyright file="Migration0023_AddAutoNoteColumnToPlayerNotes.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(23)]
    public class Migration0023_AddAutoNoteColumnToPlayerNotes : Migration
    {
        private const string playerNotesTableName = "PlayerNotes";
        private const string playerNotesTempTableName = "PlayerNotesTemp";
        private const string playerNotesIndexName = "PlayerNotesPlayerId_IDX";
        private const string autoNoteColumnName = "AutoNote";
        private const string noteColumnName = "Note";

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #23.");

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
                    .WithColumn("Note").AsString().Nullable()
                    .WithColumn("AutoNote").AsString().Nullable()
                    .WithColumn("PokerSiteId").AsInt32().NotNullable();

                Create.Index(playerNotesIndexName).OnTable(playerNotesTableName).OnColumn("PlayerId");

                if (Schema.Table(playerNotesTempTableName).Exists())
                {
                    Execute.Sql($"INSERT INTO {playerNotesTableName} SELECT PlayerNoteId, PlayerId, Note, null, PokerSiteId FROM {playerNotesTempTableName}");
                    Delete.Table(playerNotesTempTableName);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #23 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #23 executed.");
        }

        public override void Down()
        {
            LogProvider.Log.Info("Rolling back migration #23.");

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
                LogProvider.Log.Error(this, "Migration #23 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #23 rolled back.");
        }
    }
}
