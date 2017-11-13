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
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using Prism.Events;
using System;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace DriveHUD.Importers.Bovada
{
    internal class IgnitionTable : BovadaTable, IPokerTable
    {
        private IImporterService importerService;
        private IIgnitionWindowCache ignitionWindowCache;
        private const string WindowClassName = "Chrome_WidgetWin_1";

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

            ignitionWindowCache.AddWindow(windowHandle);

            WindowHandle = windowHandle;
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

            var infoImporter = importerService.GetRunningImporter<IgnitionInfoImporter>();

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

                        if (IsWindowMatch(windowTitle, windowClassName) && !ignitionWindowCache.IsWindowCached(hWnd))
                        {
                            if (IsAdvancedLoggingEnabled)
                            {
                                LogProvider.Log.Info(this, $"Window [{windowTitle}, {windowClassName}] does match table [{TableName}, {TableId}, {TableIndex}]. [{Identifier}]");
                            }

                            windowHandle = hWnd;
                            return false;
                        }
                    }

                    return true;
                }, IntPtr.Zero);
            }

            return windowHandle;
        }

        private bool IsWindowMatch(string title, string className)
        {
            if (IsAdvancedLoggingEnabled)
            {
                LogProvider.Log.Info(this, $"Checking if window [{title}, {className}] matches table [{TableName}, {TableId}, {TableIndex}]. [{Identifier}]");
            }

            if (string.IsNullOrWhiteSpace(className) ||
                !className.Equals(WindowClassName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var tableTitleData = new IgnitionTableTitle(title);

            if (!tableTitleData.IsValid)
            {
                return false;
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
            uint tournamentId;

            if (!uint.TryParse(tableTitleData.TournamentId, out tournamentId))
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
            int tableId;

            var match = tableTitleData.TableName.Equals(TableName, StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(tableTitleData.TableId, out tableId) && (TableIndex == 0 || (TableIndex != 0 && tableId == TableIndex));

            if (IsAdvancedLoggingEnabled && match)
            {
                LogProvider.Log.Info(this, $"Tournament table is detected: {tableTitleData.TableName}, {tableTitleData.TableId} [{tableTitleData.OriginalTitle}] [{Identifier}]");
            }

            return match;
        }
    }
}