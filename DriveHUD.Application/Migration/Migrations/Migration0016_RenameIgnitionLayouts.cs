//-----------------------------------------------------------------------
// <copyright file="Migration0016_RenameIgnitionLayouts.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Common.Extensions;
using DriveHUD.Application.ViewModels.Hud;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(16)]
    public class Migration0016_RenameIgnitionLayouts : Migration
    {
        private static int MigrationNumber = 16;

        private const string Suffix = " - Ignition/Bodog";

        public override void Down()
        {
        }

        public override void Up()
        {
            //try
            //{
            //    LogProvider.Log.Info($"Preparing migration #{MigrationNumber}");

            //    var hudLayoutService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            //    // update layouts
            //    foreach (EnumTableType tableType in Enum.GetValues(typeof(EnumTableType)))
            //    {
            //        foreach (var layout in hudLayoutService.GetAllLayouts(tableType))
            //        {
            //            if (layout.HudViewType == HudViewType.Plain || !layout.IsDefault || layout.Name.Contains(Suffix))
            //            {
            //                continue;
            //            }

            //            // mark old layout as non-default to be able to delete it
            //            layout.IsDefault = false;
            //            hudLayoutService.Save(layout);

            //            var oldLayoutName = layout.Name;
            //            layout.Name = UpdateLayoutName(layout.Name);
            //            layout.IsDefault = true;

            //            hudLayoutService.Save(layout);
            //            hudLayoutService.Delete(oldLayoutName);
            //        }
            //    }

            //    LogProvider.Log.Info($"Migration #{MigrationNumber} executed.");
            //}
            //catch (Exception e)
            //{
            //    LogProvider.Log.Error(this, $"Migration #{MigrationNumber} failed.", e);
            //}
        }

        private string UpdateLayoutName(string name)
        {
            return $"{name}{Suffix}";
        }
    }
}