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
    [Migration(3)]
    public class Migration0003_NonDB_PokersiteSettingsChangeBovadaToIgnition : Migration
    {
        public override void Up()
        {
            LogProvider.Log.Info("Running migration #3");

            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
            var bovada = settings.SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == Entities.EnumPokerSites.Bovada);

            if (bovada != null)
            {
                bovada.PokerSite = Entities.EnumPokerSites.Ignition;
                ServiceLocator.Current.GetInstance<ISettingsService>().SaveSettings(settings);
            }

            LogProvider.Log.Info("Migration #3 executed.");
        }

        public override void Down()
        {
        }
    }
}
