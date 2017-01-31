//-----------------------------------------------------------------------
// <copyright file="Migration0015_FixBreakLineInLayouts.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(15)]
    public class Migration0015_FixBreakLineInLayouts : Migration
    {
        private static int MigrationNumber = 15;

        public override void Down()
        {
        }

        public override void Up()
        {
            try
            {
                LogProvider.Log.Info($"Preparing migration #{MigrationNumber}");

                var hudLayoutService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

                foreach (EnumTableType tableType in Enum.GetValues(typeof(EnumTableType)))
                {
                    foreach (var layout in hudLayoutService.GetAllLayouts(tableType))
                    {
                        for (var i = 0; i < layout.HudStats.Count; i++)
                        {
                            if (string.IsNullOrWhiteSpace(layout.HudStats[i].PropertyName) && !(layout.HudStats[i] is StatInfoBreak))
                            {
                                layout.HudStats[i] = new StatInfoBreak();
                            }
                        }

                        hudLayoutService.Save(layout);
                    }
                }

                LogProvider.Log.Info($"Migration #{MigrationNumber} executed.");
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Migration #{MigrationNumber} failed.", e);
            }
        }
    }
}