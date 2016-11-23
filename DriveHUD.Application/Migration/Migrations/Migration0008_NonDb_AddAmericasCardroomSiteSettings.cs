using DriveHUD.Common.Log;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(8)]
    public class Migration0008_NonDb_AddAmericasCardroomSiteSettings : Migration
    {
        public override void Up()
        {
            LogProvider.Log.Info("Running migration #8");

            try
            {
                var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
                var settingsModel = settingsService.GetSettings();

                var americasCardroomSettingsExist = settingsModel.SiteSettings.SitesModelList.Any(x => x.PokerSite == Entities.EnumPokerSites.AmericasCardroom);

                if (americasCardroomSettingsExist)
                {
                    return;
                }

                var americasCardroomSiteModel = new SiteModel
                {
                    HandHistoryLocationList = new System.Collections.ObjectModel.ObservableCollection<string>(),
                    PokerSite = Entities.EnumPokerSites.AmericasCardroom
                };

                settingsModel.SiteSettings.SitesModelList = settingsModel.SiteSettings.SitesModelList.Concat(new[] { americasCardroomSiteModel }).ToArray();

                settingsService.SaveSettings(settingsModel);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            LogProvider.Log.Info("Migration #8 executed.");
        }

        public override void Down()
        {
        }
    }
}
