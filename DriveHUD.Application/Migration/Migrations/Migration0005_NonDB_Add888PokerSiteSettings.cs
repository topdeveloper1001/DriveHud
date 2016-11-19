//-----------------------------------------------------------------------
// <copyright file="Migration0005_NonDB_Add888PokerSiteSettings.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(5)]
    public class Migration0005_NonDB_Add888PokerSiteSettings : Migration
    {
        public override void Up()
        {
            LogProvider.Log.Info("Running migration #5");

            try
            {
                var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
                var settingsModel = settingsService.GetSettings();

                var poker888SettingsExist = settingsModel.SiteSettings.SitesModelList.Any(x => x.PokerSite == EnumPokerSites.Poker888);

                if (poker888SettingsExist)
                {
                    return;
                }

                var poker888SiteModel = new SiteModel
                {
                    HandHistoryLocationList = new System.Collections.ObjectModel.ObservableCollection<string>(),
                    PokerSite = EnumPokerSites.Poker888
                };

                settingsModel.SiteSettings.SitesModelList = settingsModel.SiteSettings.SitesModelList.Concat(new[] { poker888SiteModel }).ToArray();

                settingsService.SaveSettings(settingsModel);
            }
            catch(Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            LogProvider.Log.Info("Migration #5 executed.");
        }

        public override void Down()
        {
        }
    }
}
