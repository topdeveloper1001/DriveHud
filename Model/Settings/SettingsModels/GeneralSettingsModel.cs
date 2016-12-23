using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Settings
{
    [Serializable]
    public class GeneralSettingsModel : SettingsBase
    {
        public bool IsAutomaticallyDownloadUpdates { get; set; }
        public bool IsApplyFiltersToTournamentsAndCashGames { get; set; }
        public bool IsSaveFiltersOnExit { get; set; }
        public bool IsAdvancedLoggingEnabled { get; set; }

        public bool IsSQLiteEnabled { get; set; }

        public int TimeZoneOffset { get; set; }

        public DayOfWeek StartDayOfWeek { get; set; }

        public GeneralSettingsModel()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            IsAutomaticallyDownloadUpdates = true;
            IsApplyFiltersToTournamentsAndCashGames = true;
            IsSaveFiltersOnExit = true;
            IsSQLiteEnabled = true;
            IsAdvancedLoggingEnabled = false;
            TimeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).Hours;

            StartDayOfWeek = DayOfWeek.Monday;
        }
    }
}
