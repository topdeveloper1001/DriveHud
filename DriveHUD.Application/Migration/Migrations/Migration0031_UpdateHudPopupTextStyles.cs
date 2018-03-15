//-----------------------------------------------------------------------
// <copyright file="Migration0031_UpdateHudPopupTextStyles.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using Model.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(31)]
    public class Migration0031_UpdateHudPopupTextStyles : Migration
    {
        private const int FontSize = 12;

        private readonly Color FontColor = Color.FromRgb(255, 255, 255);

        public override void Down()
        {
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #31");

            try
            {
                var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

                var layouts = new List<HudLayoutInfoV2>();

                foreach (EnumTableType tableType in Enum.GetValues(typeof(EnumTableType)))
                {
                    layouts.AddRange(hudLayoutsService.GetAllLayouts(tableType));
                }

                foreach (var layout in layouts)
                {
                    layout.LayoutTools
                        .OfType<HudLayoutGaugeIndicator>()
                        .SelectMany(x => x.Stats)
                        .ForEach(s => UpdateStatInfo(s));

                    hudLayoutsService.Save(layout);
                }

                LogProvider.Log.Info($"Migration #31 completed. {layouts.Count} layouts were processed.");
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #31 failed.", e);
            }
        }

        private void UpdateStatInfo(StatInfo statInfo)
        {
            if (statInfo == null)
            {
                return;
            }

            statInfo.Initialize();
            statInfo.SettingsAppearanceFontSize = FontSize;
            statInfo.SettingsAppearanceFontBold_IsChecked = true;
            statInfo.SettingsAppearanceValueRangeCollection.ForEach(x => x.Color = FontColor);
        }
    }
}