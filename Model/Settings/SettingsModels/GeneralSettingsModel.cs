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
using System.Runtime.Serialization;

namespace Model.Settings
{
    [DataContract]
    public class GeneralSettingsModel : SettingsBase
    {
        [DataMember]
        public bool IsAutomaticallyDownloadUpdates { get; set; }

        [DataMember]
        public bool IsApplyFiltersToTournamentsAndCashGames { get; set; }

        [DataMember]
        public bool IsSaveFiltersOnExit { get; set; }

        [DataMember]
        public bool IsAdvancedLoggingEnabled { get; set; }

        [DataMember]
        public bool IsSQLiteEnabled { get; set; }

        [DataMember]
        public int TimeZoneOffset { get; set; }

        [DataMember]
        public DayOfWeek StartDayOfWeek { get; set; }

        [DataMember]
        public int HudViewMode { get; set; }

        [DataMember]
        public bool IsHudSavedAtFirstTime { get; set; }

        [DataMember]
        public bool RunSiteDetection { get; set; }

        [DataMember]
        public bool IsAPIEnabled { get; set; }

        [DataMember]
        public bool IsOpponentReportCacheSaved { get; set; }

        [DataMember]
        public DisplaySettings DisplaySettings { get; set; }

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