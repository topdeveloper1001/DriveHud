using DriveHUD.Common.Log;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(11)]
    public class Migration0011_RemoveHandHistoryHandNumberUniqueKey : Migration
    {
        const string hhTable = "HandHistories";
        const string hhConstraint = "HandHistories_HandNumber_UK";

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #11");

            if (Schema.Table(hhTable).Constraint(hhConstraint).Exists())
            {
                Delete.UniqueConstraint(hhConstraint).FromTable(hhTable);
            }

            LogProvider.Log.Info("Migration #11 start executing");
        }

        public override void Down()
        {
        }
    }
}
