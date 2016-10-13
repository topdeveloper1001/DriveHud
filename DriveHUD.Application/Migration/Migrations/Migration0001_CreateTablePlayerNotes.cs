using DriveHUD.Common.Log;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(1)]
    public class Migration0001_CreateTablePlayerNotes : Migration
    {
        private const string tableName = "PlayerNotes";

        public override void Up()
        {
            LogProvider.Log.Info("Prepearing migration #1");

            if (!Schema.Table(tableName).Exists())
            {
                LogProvider.Log.Info($"Creating {tableName}");

                Create.Table(tableName)
                    .WithColumn("PlayerNoteId").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("PlayerId").AsInt32().NotNullable()
                    .WithColumn("Note").AsString()
                    .WithColumn("PokerSiteId").AsInt16().NotNullable();

                Create.ForeignKey("FK_PlayerNotes_PlayerId").FromTable(tableName).ForeignColumn("PlayerId").ToTable("Players").PrimaryColumn("PlayerId");
            }
            else
            {
                LogProvider.Log.Info($"Table {tableName} already exists.");
            }

            LogProvider.Log.Info("Migration #1 start executing");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_PlayerNotes_PlayerId").OnTable(tableName);
            Delete.Table(tableName);
        }
    }
}
