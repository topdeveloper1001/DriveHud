using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using DriveHUD.Common.Log;

namespace DriveHUD.Application.Migration.Migrations
{
    [Migration(20)]
    public class Migration0020_AddAliasTable : FluentMigrator.Migration
    {
        public Migration0020_AddAliasTable()
        {

        }

        public override void Down()
        {
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #19");

            LogProvider.Log.Info("Check table \"Aliases\"");
            if (!Schema.Table("Aliases").Exists())
            {
                Create.Table("Aliases")
                    .WithColumn("AliasId").AsInt32().NotNullable().PrimaryKey()
                    .WithColumn("AliasName").AsString().NotNullable();
            }

            LogProvider.Log.Info("Check table \"AliasPlayers\"");
            if (!Schema.Table("AliasPlayers").Exists())
            {
                Create.Table("AliasPlayers")
                    .WithColumn("PlayerId").AsInt32().NotNullable().PrimaryKey().ReferencedBy("Players", "PlayerId")
                    .WithColumn("AliasId").AsInt32().NotNullable().PrimaryKey().ReferencedBy("Aliases", "AliasId");
            }
        }

    }
}
