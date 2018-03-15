//-----------------------------------------------------------------------
// <copyright file="Migration0032_AddTournamentIndexOnHandHistory.cs" company="Ace Poker Solutions">
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
    [Migration(32)]
    public class Migration0032_AddTournamentIndexOnHandHistory : Migration
    {
        private const string handHistoriesTableName = "HandHistories";
        private const string tournamentNumberPokerSiteIdIndexName = "HandHistoriesTournamentNumberPokerSiteId_IDX";

        public override void Down()
        {
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #32.");

            try
            {
                Create.Index(tournamentNumberPokerSiteIdIndexName)
                    .OnTable(handHistoriesTableName)
                    .OnColumn("TournamentNumber").Ascending()
                    .OnColumn("PokerSiteId").Ascending();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #32 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #32 executed.");
        }
    }
}