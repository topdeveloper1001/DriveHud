//-----------------------------------------------------------------------
// <copyright file="CashGraphSettingsService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using Model;
using Model.Enums;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;

namespace DriveHUD.Application.ViewModels.Graphs
{
    /// <summary>
    /// Service to perform base operations with the settings of cash graph 
    /// </summary>
    internal class CashGraphSettingsService : ICashGraphSettingsService
    {
        private const string SettingsFileName = "CashGraphSettings.data";

        private readonly string SettingsFile = Path.Combine(StringFormatter.GetAppDataFolderPath(), SettingsFileName);

        private Dictionary<CashChartType, CashGraphSettings> settings;

        /// <summary>
        /// Gets settings for the specified <see cref="CashChartType"/>
        /// </summary>
        /// <param name="cashChartType">Type of cash chart</param>
        /// <returns>Cash graph settings</returns>
        public CashGraphSettings GetSettings(CashChartType cashChartType)
        {
            if (!TryLoadSettings())
            {
                InitializeDefault();
            }

            if (settings.ContainsKey(cashChartType))
            {
                return settings[cashChartType];
            }

            return new CashGraphSettings { ChartDisplayRange = ChartDisplayRange.Month };
        }

        /// <summary>
        /// Saves settings for the specified array of cash graphs
        /// </summary>        
        public void SaveSettings()
        {
            if (settings == null)
            {
                return;
            }

            try
            {
                using (var fs = new FileStream(SettingsFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    Serializer.Serialize(fs, settings);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not save cash graph settings to '{SettingsFile}'", e);
            }
        }

        /// <summary>
        /// Load settings from <see cref="SettingsFile"/>
        /// </summary>
        /// <returns>True if settings are loaded, otherwise - false</returns>
        private bool TryLoadSettings()
        {
            if (settings != null)
            {
                return true;
            }

            if (!File.Exists(SettingsFile))
            {
                return false;
            }

            try
            {
                using (var fs = new FileStream(SettingsFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    settings = Serializer.Deserialize<Dictionary<CashChartType, CashGraphSettings>>(fs);

                    if (settings.Count == 0)
                    {
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not load cash graph settings from '{SettingsFile}'.", e);
            }

            return false;
        }

        /// <summary>
        /// Initialize settings with predefined values
        /// </summary>
        private void InitializeDefault()
        {
            settings = new Dictionary<CashChartType, CashGraphSettings>
            {
                [CashChartType.MoneyWon] = new CashGraphSettings
                {
                    ChartDisplayRange = ChartDisplayRange.Hands
                },
                [CashChartType.BB100] = new CashGraphSettings
                {
                    ChartDisplayRange = ChartDisplayRange.Month
                }
            };
        }
    }
}