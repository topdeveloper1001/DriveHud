//-----------------------------------------------------------------------
// <copyright file="Migration0020_AddAliasTable.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using FluentMigrator;
using DriveHUD.Common.Log;
using System.Data;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(20)]
    public class Migration0020_AddAliasTable : Migration
    {
        private const string aliasesTableName = "Aliases";
        private const string aliasesPlayersTableName = "AliasPlayers";
        private const string aliasesPlayersPlayerFK = "FK_AliasPlayers_PlayerId";
        private const string aliasesPlayersAliasFK = "FK_AliasPlayers_AliasId";
     
        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #20");

            LogProvider.Log.Info("Check if table \"Aliases\" exists");

            if (!Schema.Table(aliasesTableName).Exists())
            {
                Create.Table(aliasesTableName)
                    .WithColumn("AliasId").AsInt32().NotNullable().PrimaryKey()
                    .WithColumn("AliasName").AsString().NotNullable();
            }

            LogProvider.Log.Info("Check if table \"AliasPlayers\" exists");

            if (!Schema.Table(aliasesPlayersTableName).Exists())
            {
                Create.Table(aliasesPlayersTableName)
                    .WithColumn("PlayerId").AsInt32().NotNullable().PrimaryKey()
                    .WithColumn("AliasId").AsInt32().NotNullable().PrimaryKey();

                Create.ForeignKey(aliasesPlayersPlayerFK)
                    .FromTable(aliasesPlayersTableName)
                    .ForeignColumn("PlayerId")
                    .ToTable("Players")
                    .PrimaryColumn("PlayerId")
                    .OnDelete(Rule.Cascade);

                Create.ForeignKey(aliasesPlayersAliasFK)
                    .FromTable(aliasesPlayersTableName)
                    .ForeignColumn("AliasId")
                    .ToTable(aliasesTableName)
                    .PrimaryColumn("AliasId")
                    .OnDelete(Rule.Cascade);
            }
        }

        public override void Down()
        {
            if (Schema.Table(aliasesPlayersTableName).Exists())
            {
                Delete.ForeignKey(aliasesPlayersPlayerFK).OnTable(aliasesPlayersTableName);
                Delete.ForeignKey(aliasesPlayersAliasFK).OnTable(aliasesPlayersTableName);
                Delete.Table(aliasesPlayersTableName);
            }

            if (Schema.Table(aliasesTableName).Exists())
            {
                Delete.Table(aliasesTableName);
            }
        }
    }
}