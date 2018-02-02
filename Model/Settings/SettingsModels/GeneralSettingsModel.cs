//-----------------------------------------------------------------------
// <copyright file="GeneralSettingsModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System;

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

        public int HudViewMode { get; set; }

        public bool IsHudSavedAtFirstTime { get; set; }

        public bool RunSiteDetection { get; set; }

        public bool IsAPIEnabled { get; set; }

        public bool IsOpponentReportCacheSaved { get; set; }

        public GeneralSettingsModel()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            IsAutomaticallyDownloadUpdates = true;
            IsApplyFiltersToTournamentsAndCashGames = true;
            IsSaveFiltersOnExit = true;
            IsAdvancedLoggingEnabled = false;
            IsHudSavedAtFirstTime = true;
            IsSQLiteEnabled = true;
            TimeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).Hours;
            HudViewMode = (int)HudViewType.Vertical_1;
            RunSiteDetection = true;
            IsAPIEnabled = false;

            StartDayOfWeek = DayOfWeek.Monday;
        }
    }
}