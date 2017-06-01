//-----------------------------------------------------------------------
// <copyright file="LogCleaner.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using Model;
using System;
using System.IO;
using System.Linq;

namespace DriveHUD.Application.Services
{
    /// <summary>
    /// Defines methods to clear logs 
    /// </summary>
    internal class LogCleaner
    {
        private const int maxSizeLogBackups = 10;

        private static readonly string[] logsPatterns = new string[] { "drivehud*", "hud*", "NHibernate*", "ign-games*", "bol-games*" };

        /// <summary>
        /// Clear folder with logs
        /// </summary>
        public static void ClearLogsFolder()
        {
            var logsFolder = StringFormatter.GetLogsFolderPath();

            try
            {
                LogProvider.Log.Info($"Clearing '{logsFolder}'");

                var logsDirectory = new DirectoryInfo(logsFolder);

                if (!logsDirectory.Exists)
                {
                    return;
                }

                foreach (var logPattern in logsPatterns)
                {
                    var logFilesToDelete = logsDirectory.GetFiles(logPattern)
                        .GroupBy(x => x.LastWriteTime.Date)
                        .Select(x => new { Day = x.Key, Files = x.ToArray() })
                        .OrderByDescending(x => x.Day)
                        .Skip(maxSizeLogBackups)
                        .SelectMany(x => x.Files)
                        .ToArray();

                    logFilesToDelete.ForEach(x =>
                    {
                        try
                        {
                            x.Delete();
                        }
                        catch (Exception ex)
                        {
                            LogProvider.Log.Error(nameof(LogCleaner), $"Failed to delete log file: {x.FullName}", ex);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(nameof(LogCleaner), $"Failed to clear log folder: {logsFolder}", e);
            }
        }
    }
}