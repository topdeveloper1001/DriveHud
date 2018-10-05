//-----------------------------------------------------------------------
// <copyright file="BovadaTable.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace DriveHUD.Importers.Bovada
{
    internal class IgnitionTable : BovadaTable, IPokerTable
    {
        private IImporterService importerService;
        private IIgnitionWindowCache ignitionWindowCache;
        private const string WindowClassName = "Chrome_WidgetWin";

        public IgnitionTable(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            importerService = ServiceLocator.Current.GetInstance<IImporterService>();
            ignitionWindowCache = ServiceLocator.Current.GetInstance<IIgnitionWindowCache>();

            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
            IsAdvancedLoggingEnabled = settings.GeneralSettings.IsAdvancedLoggingEnabled;
        }

        private bool IsAdvancedLoggingEnabled
        {
            get;
            set;
        }

        public override ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.Ignition;
            }
        }

        /// <summary>
        /// Process command data object
        /// </summary>
        /// <param name="cmdObj">Command data object</param>
        protected override void ProcessCmdObject(BovadaCommandDataObject cmdObj)
        {
            if (cmdObj.gid != null)
            {
                ParseGidInfo(cmdObj);
                return;
            }

            if (cmdObj.pid != null &&
                cmdObj.pid.Equals("PING", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            SetWindowHandle();
            base.ProcessCmdObject(cmdObj);
            SetBaseTableData();
            SetTableCashName();
        }

        /// <summary>
        /// Sets window handle of poker table
        /// </summary>
        private void SetWindowHandle()
        {
            if (WindowHandle != IntPtr.Zero || (!IsTournament && !playerHasSeat) || IsZonePokerTable)
            {
                return;
            }

            var pokerCatcher = importerService.GetImporter<IIgnitionCatcher>();

            if (pokerCatcher == null || !pokerCatcher.IsRunning)
            {
                return;
            }

            var pokerClientProcess = pokerCatcher.PokerClientProcess;

            if (pokerClientProcess == null || pokerClientProcess.HasExited)
            {
                return;
            }

            var windowHandle = GetWindowHandle(pokerClientProcess);

            if (windowHandle == IntPtr.Zero)
            {
                return;
            }

            ignitionWindowCache.AddWindow(windowHandle, this);

            WindowHandle = windowHandle;

            SendPreImporedData();
        }

        private void SendPreImporedData()
        {
            if (IsZonePokerTable)
            {
                return;
            }

            var gameInfo = new GameInfo
            {
                PokerSite = EnumPokerSites.Ignition,
                TableType = (EnumTableType)MaxSeat,
                WindowHandle = WindowHandle.ToInt32()
            };

            var loadingText = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_PreLoadingText_Ignition");

            var eventArgs = new PreImportedDataEventArgs(gameInfo, loadingText);
            eventAggregator.GetEvent<PreImportedDataEvent>().Publish(eventArgs);
        }

        /// <summary>
        /// Sets base table data such as <see cref="GameFormat"/>, <see cref="GameType"/>, <see cref="GameLimit"/>
        /// </summary>
        private void SetBaseTableData()
        {
            if (isConnectedInfoParsed)
            {
                return;
            }

            var infoImporter = importerService.GetRunningImporter<IIgnitionInfoImporter>();

            if (infoImporter != null && infoImporter.InfoDataManager != null)
            {
                IgnitionTableData tableData = null;

                tableData = infoImporter.InfoDataManager.GetTableData(TableId);

                if (tableData == null)
                {
                    return;
                }

                TableId = tableData.Id;
                MaxSeat = tableData.TableSize;
                GameFormat = tableData.GameFormat;
                GameLimit = tableData.GameLimit;
                GameType = tableData.GameType;
                TableName = HttpUtility.UrlDecode(tableData.TableName);

                isConnectedInfoParsed = true;

                LogProvider.Log.Info(this, $"Table info has been set: {tableData} [{Identifier}]");
            }
        }

        /// <summary>
        /// Sets the table name for cash game
        /// </summary>
        private void SetTableCashName()
        {
            if (WindowHandle != IntPtr.Zero && string.IsNullOrEmpty(TableName) && !IsTournament && TableId != 0)
            {
                TableName = $"Table{TableId}";
            }
        }

        /// <summary>
        /// Parses CO_OPTION_INFO command
        /// </summary>
        /// <param name="cmdObj">Command data object</param>
        protected override void ParseOptionInfo(BovadaCommandDataObject cmdObj)
        {
        }

        protected override void ParseConnectInfo(BovadaCommandDataObject cmdObj)
        {
            var tableNumber = cmdObj.tourNo != 0 ? cmdObj.tourNo :
                  cmdObj.tableNo;

            TableId = tableNumber;
        }

        protected override void ParseGidInfo(BovadaCommandDataObject cmdObj)
        {
            if (!cmdObj.gid.Equals("Unjoined", StringComparison.OrdinalIgnoreCase) &&
                !cmdObj.gid.Equals("disconnecting", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (WindowHandle != IntPtr.Zero || !string.IsNullOrEmpty(TableName))
            {
                LogProvider.Log.Info($"Unjoined table [{TableName}, {TableId}, {TableIndex}, {WindowHandle}]. [{Identifier}]");
            }

            ignitionWindowCache.RemoveWindow(WindowHandle, true);

            WindowHandle = IntPtr.Zero;
            TableName = string.Empty;
            isConnectedInfoParsed = false;
        }

        protected override void ParsePlayBuyinQs2(BovadaCommandDataObject cmdObj)
        {
            if (cmdObj.prevTableId == 0)
            {
                return;
            }

            var previousHandle = ignitionWindowCache.GetPreviousHandle(cmdObj.prevTableId);

            if (previousHandle == IntPtr.Zero || !WinApi.IsWindow(previousHandle) || previousHandle == WindowHandle)
            {
                return;
            }

            var cachedTable = ignitionWindowCache.GetCachedTable(previousHandle);

            if (cachedTable != null)
            {
                ignitionWindowCache.RemoveWindow(previousHandle);
                cachedTable.WindowHandle = IntPtr.Zero;
                cachedTable.TableName = string.Empty;
            }

            WindowHandle = previousHandle;
            ignitionWindowCache.AddWindow(previousHandle, this);
        }

        protected override void PreImportChecks()
        {
            if (WindowHandle == IntPtr.Zero || WinApi.IsWindow(WindowHandle))
            {
                return;
            }

            LogProvider.Log.Info($"Window [{WindowHandle}] for table [{TableName}, {TableId}, {TableIndex}] doesn't exist. [{Identifier}]");

            ignitionWindowCache.RemoveWindow(WindowHandle);
            WindowHandle = IntPtr.Zero;
            TableName = string.Empty;
        }

        protected override void UpdateHandNumberCommand()
        {
            // if info didn't come for some reason trying to use title of window            
            if (!isConnectedInfoParsed)
            {
                var title = WinApi.GetWindowText(WindowHandle);

                LogProvider.Log.Warn($"Table data hasn't been detected from stream. Use table title '{title}' [{WindowHandle}] [{Identifier}]");

                GameType = BovadaConverters.ConvertGameTypeFromTitle(title);
                GameFormat = BovadaConverters.ConvertGameFormatFromTitle(title);
                GameLimit = BovadaConverters.ConvertGameLimitFromTitle(title);
                MaxSeat = MaxSeat == 0 ? 9 : MaxSeat;

                if (GameFormat == GameFormat.SnG || GameFormat == GameFormat.MTT)
                {
                    TableName = BovadaConverters.ParseTableNameFromTitle(title);
                }

                TableId = (uint)RandomProvider.GetThreadRandom().Next(12000001, 19999999);
            }

            base.UpdateHandNumberCommand();
        }

        private IntPtr GetWindowHandle(Process pokerClientProcess)
        {
            var windowHandle = IntPtr.Zero;

            var possibleWindowHandles = new List<IntPtr>();

            foreach (ProcessThread thread in pokerClientProcess.Threads)
            {
                WinApi.EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                {
                    var parentHWnd = WinApi.GetParent(hWnd);

                    if (parentHWnd != IntPtr.Zero)
                    {
                        return true;
                    }

                    var sb = new StringBuilder(256);

                    if (WinApi.GetClassName(hWnd, sb, sb.Capacity) != 0)
                    {
                        var windowTitle = WinApi.GetWindowText(hWnd);
                        var windowClassName = sb.ToString();

                        if (IsAdvancedLoggingEnabled)
                        {
                            LogProvider.Log.Info(this, $"Checking if window [{windowTitle}, {windowClassName}, {hWnd}] matches table [{TableName}, {TableId}, {TableIndex}]. [{Identifier}]");
                        }

                        if (IsWindowMatch(windowTitle, windowClassName))
                        {
                            // if window is already cached we need to check maybe d/c happened
                            var cachedTable = ignitionWindowCache.GetCachedTable(hWnd);

                            if (cachedTable != null)
                            {
                                var match = TableId != 0 && cachedTable.TableId == TableId &&
                                    MaxSeat != 0 && cachedTable.MaxSeat == MaxSeat &&
                                    cachedTable.GameFormat == GameFormat && cachedTable.GameLimit == GameLimit &&
                                    cachedTable.GameType == GameType &&
                                    (IsTournament && cachedTable.TableName == TableName &&
                                    (cachedTable.TableIndex == TableIndex || HeroSeat != 0 && cachedTable.HeroSeat != 0) || !IsTournament);

                                if (match)
                                {
                                    if (IsAdvancedLoggingEnabled)
                                    {
                                        LogProvider.Log.Info(this, $"Window [{windowTitle}, {windowClassName}, {hWnd}] was found in the cache, and it does match table [{TableName}, {TableId}, {TableIndex}, {HeroSeat}]. [{Identifier}]");
                                    }

                                    windowHandle = hWnd;
                                    return false;
                                }

                                return true;
                            }

                            if (IsAdvancedLoggingEnabled)
                            {
                                LogProvider.Log.Info(this, $"Window [{windowTitle}, {windowClassName}, {hWnd}] does match table [{TableName}, {TableId}, {TableIndex}]. [{Identifier}]");
                            }

                            possibleWindowHandles.Add(hWnd);

                            return true;
                        }
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

        private bool IsWindowMatch(string title, string className)
        {
            if (string.IsNullOrWhiteSpace(className) ||
                className.IndexOf(WindowClassName, StringComparison.OrdinalIgnoreCase) < 0)
            {
                return false;
            }

            var tableTitleData = new IgnitionTableTitle(title);

            if (!tableTitleData.IsValid)
            {
                return false;
            }

            if (IsZonePokerTable)
            {
                return ZonePokerTableMatch(tableTitleData);
            }

            if (IsJackpotTable)
            {
                return JackpotTableMatch(tableTitleData);
            }

            if (IsTournament)
            {
                return TournamentTableMatch(tableTitleData);
            }

            if (IsAdvancedLoggingEnabled)
            {
                LogProvider.Log.Info(this, $"Cash table is detected. [{tableTitleData.OriginalTitle}]");
            }

            return true;
        }

        private bool JackpotTableMatch(IgnitionTableTitle tableTitleData)
        {
            if (!uint.TryParse(tableTitleData.TournamentId, out uint tournamentId))
            {
                return false;
            }

            if (TournamentId != tournamentId)
            {
                return false;
            }

            TableName = tableTitleData.TableName;

            if (IsAdvancedLoggingEnabled)
            {
                LogProvider.Log.Info(this, $"Jackpot table is detected: {tableTitleData.TableName}, {tableTitleData.TournamentId} [{tableTitleData.OriginalTitle}] [{Identifier}]");
            }

            return true;
        }

        private bool TournamentTableMatch(IgnitionTableTitle tableTitleData)
        {
            var match = tableTitleData.TableName.Equals(TableName, StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(tableTitleData.TableId, out int tableId) && (TableIndex == 0 || (TableIndex != 0 && tableId == TableIndex));

            if (IsAdvancedLoggingEnabled && match)
            {
                LogProvider.Log.Info(this, $"Tournament table is detected: {tableTitleData.TableName}, {tableTitleData.TableId} [{tableTitleData.OriginalTitle}] [{Identifier}]");
            }

            return match;
        }

        private bool ZonePokerTableMatch(IgnitionTableTitle tableTitleData)
        {
            return false;
        }
    }
}