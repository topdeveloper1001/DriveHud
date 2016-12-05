using DriveHUD.Common.Linq;
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
    [Migration(10)]
    public class Migration0010_NonDb_RemovePreferredSeatingSportsbettingTigerGaming : Migration
    {
        public override void Up()
        {
            LogProvider.Log.Info("Running migration #10");

            try
            {
                var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
                var settingsModel = settingsService.GetSettings();

                var sites = settingsModel.SiteSettings.SitesModelList
                    .Where(x => x.PokerSite == Entities.EnumPokerSites.SportsBetting
                        || x.PokerSite == Entities.EnumPokerSites.TigerGaming);

                foreach (var site in sites)
                {
                    foreach (var seat in site.PrefferedSeats)
                    {
                        if (seat != null)
                        {
                            seat.IsPreferredSeatEnabled = false;
                            seat.PreferredSeat = -1;
                        }
                    }
                }

                settingsService.SaveSettings(settingsModel);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            LogProvider.Log.Info("Migration #10 executed.");
        }

        public override void Down()
        {
        }
    }
}
