//-----------------------------------------------------------------------
// <copyright file="PokerBaaziImporter.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using DriveHUD.Importers.PokerBaazi.Model;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveHUD.Importers.PokerBaazi
{
    internal class PokerBaaziImporter : GenericImporter, IPokerBaaziImporter
    {
        private const int NoDataDelay = 200;

        private readonly BlockingCollection<string> packetBuffer = new BlockingCollection<string>();

        protected readonly ISettingsService settings;
        protected readonly IImporterService importerService;

        public PokerBaaziImporter()
        {
            settings = ServiceLocator.Current.GetInstance<ISettingsService>();
            importerService = ServiceLocator.Current.GetInstance<IImporterService>();
        }

        #region IPokerBaaziImporter implementation

        public void AddPackage(string data)
        {
            packetBuffer.Add(data);
        }

        #endregion

        protected override string[] ProcessNames => new string[0];

        protected override EnumPokerSites Site => EnumPokerSites.PokerBaazi;

        protected override bool IsAdvancedLogEnabled
        {
            get
            {
                return settings.GetSettings()?.GeneralSettings.IsAdvancedLoggingEnabled ?? false;
            }
        }

        protected override void DoImport()
        {
            try
            {
                ProcessBuffer();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to process imported data. [{SiteString}]", e);
            }

            packetBuffer.Clear();

            RaiseProcessStopped();
        }

        protected override IntPtr FindWindow(ParsingResult parsingResult)
        {
            return IntPtr.Zero;
        }

        protected override bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult)
        {
            return false;
        }

        /// <summary>
        /// Processes buffer data
        /// </summary>
        protected virtual void ProcessBuffer()
        {
            var handBuilder = ServiceLocator.Current.GetInstance<IPokerBaaziHandBuilder>();
            var windows = new Dictionary<uint, IntPtr>();

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (!packetBuffer.TryTake(out string capturedData))
                    {
                        Task.Delay(NoDataDelay).Wait();
                        continue;
                    }

                    if (!PokerBaaziPackage.TryParse(capturedData, out PokerBaaziPackage package))
                    {
                        continue;
                    }

                    var windowHandle = IntPtr.Zero;

                    if (package.PackageType == PokerBaaziPackageType.InitResponse)
                    {
                        var windowsToRemove = windows.Where(x => !WinApi.IsWindow(x.Value)).ToArray();
                        windowsToRemove.ForEach(x => windows.Remove(x.Key));
                    }

                    // Get window related to the package
                    if (package.RoomId != 0 && !windows.TryGetValue(package.RoomId, out windowHandle))
                    {
                        var windowsToRemove = windows.Where(x => !WinApi.IsWindow(x.Value)).ToArray();
                        windowsToRemove.ForEach(x => windows.Remove(x.Key));

                        var initResponse = handBuilder.FindInitResponse(package.RoomId);

                        if (initResponse != null)
                        {
                            var existingWindows = new HashSet<IntPtr>(windows.Values);

                            windowHandle = FindWindow(initResponse, existingWindows);

                            if (windowHandle != IntPtr.Zero)
                            {
                                windows.Add(package.RoomId, windowHandle);
                                SendPreImporedData("Notifications_HudLayout_PreLoadingText_PB", windowHandle);
                            }
                        }
                    }

                    if (!handBuilder.TryBuild(package, out HandHistory handHistory))
                    {
                        continue;
                    }

                    var handHistoryText = SerializationHelper.SerializeObject(handHistory);

#if DEBUG
                    if (!Directory.Exists("Hands"))
                    {
                        Directory.CreateDirectory("Hands");
                    }

                    File.WriteAllText($"Hands\\pokerbaazi_hand_exported_{handHistory.HandId}.xml", handHistoryText);
#endif

                    var gameInfo = new GameInfo
                    {
                        WindowHandle = windowHandle.ToInt32(),
                        PokerSite = EnumPokerSites.PokerBaazi,
                        GameNumber = handHistory.HandId,
                        Session = windowHandle.ToString()
                    };

                    ProcessHand(handHistoryText, gameInfo);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Could not process captured data. [{SiteString}]", e);
                }
            }
        }

        /// <summary>
        /// Finds window related to the specified <see cref="PokerBaaziInitResponse"/>
        /// </summary>
        /// <param name="initResponse">Init response</param>
        /// <returns>Handle of window if found; otherwise empty handle</returns>
        protected virtual IntPtr FindWindow(PokerBaaziInitResponse initResponse, HashSet<IntPtr> existingWindows)
        {
            var catcher = importerService.GetImporter<IPokerBaaziCatcher>();

            var pokerClientProcess = catcher.PokerClientProcess;

            try
            {
                if (pokerClientProcess == null || pokerClientProcess.HasExited)
                {
                    return IntPtr.Zero;
                }

                var windowHandle = IntPtr.Zero;

                var possibleWindowHandles = new List<IntPtr>();

                foreach (ProcessThread thread in pokerClientProcess.Threads)
                {
                    WinApi.EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                    {
                        if (existingWindows.Contains(hWnd) ||
                            WinApi.GetParent(hWnd) != IntPtr.Zero)
                        {
                            return true;
                        }

                        var windowTitle = WinApi.GetWindowText(hWnd);

                        if (IsAdvancedLogEnabled)
                        {
                            LogProvider.Log.Info(this, $"Checking if window [{windowTitle}, {hWnd}] matches table [{initResponse.TournamentName}, {initResponse.TournamentTableName}, {initResponse.RoomId}]. [{SiteString}]");
                        }

                        if (windowTitle.ContainsIgnoreCase(initResponse.TournamentName) &&
                            (string.IsNullOrEmpty(initResponse.TournamentTableName) ||
                                !string.IsNullOrEmpty(initResponse.TournamentTableName) && windowTitle.ContainsIgnoreCase(initResponse.TournamentTableName)))
                        {
                            if (IsAdvancedLogEnabled)
                            {
                                LogProvider.Log.Info(this, $"Window [{windowTitle}, {hWnd}] does match table [{initResponse.TournamentName}, {initResponse.TournamentTableName}, {initResponse.RoomId}]. [{SiteString}]");
                            }

                            possibleWindowHandles.Add(hWnd);
                        }

                        return true;
                    }, IntPtr.Zero);
                }

                if (windowHandle == IntPtr.Zero)
                {
                    windowHandle = possibleWindowHandles.FirstOrDefault();
                }

                return windowHandle;

            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to find window for room {initResponse.RoomId}, '{initResponse.TournamentName}' [{SiteString}]", e);
            }

            return IntPtr.Zero;
        }
    }
}