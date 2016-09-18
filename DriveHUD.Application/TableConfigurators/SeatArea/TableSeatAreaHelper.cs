using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.TableConfigurators
{
    public static class TableSeatAreaHelpers
    {
        private static ISettingsService settingsService
        {
            get { return ServiceLocator.Current.GetInstance<ISettingsService>(); }
        }

        public static PreferredSeatModel GetSeatSetting(EnumTableType tableType, EnumPokerSites pokerSite)
        {
            var settings = settingsService.GetSettings();
            var preferredSettings = settings.PreferredSeatSettings;

            var currentSeatSetting = preferredSettings.PrefferedSeats.FirstOrDefault(x => x.PokerSite == pokerSite
                                                                                        && x.TableType == tableType);
            if (currentSeatSetting == null)
            {
                return new PreferredSeatModel() { IsPreferredSeatEnabled = false, PreferredSeat = -1, PokerSite = pokerSite, TableType = tableType };
            }

            return currentSeatSetting;
        }

        public static void SetPrefferedSeatSetting(int seat, EnumTableType tableType, EnumPokerSites pokerSite)
        {
            var settings = settingsService.GetSettings();
            var preferredSettings = settings.PreferredSeatSettings;

            var currentSeatSetting = preferredSettings.PrefferedSeats.FirstOrDefault(x => x.PokerSite == pokerSite
                                                                                       && x.TableType == tableType);
            if (currentSeatSetting == null)
            {
                currentSeatSetting = new PreferredSeatModel() { IsPreferredSeatEnabled = false, PreferredSeat = -1, PokerSite = pokerSite, TableType = tableType };
                preferredSettings.PrefferedSeats.Add(currentSeatSetting);
            }

            currentSeatSetting.PreferredSeat = seat;
            settingsService.SaveSettings(settings);
        }
    }
}
