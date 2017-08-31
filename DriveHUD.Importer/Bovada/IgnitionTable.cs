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
        private bool isConnectedInfoParsed;

        public IgnitionTable(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            importerService = ServiceLocator.Current.GetInstance<IImporterService>();
            ignitionWindowCache = ServiceLocator.Current.GetInstance<IIgnitionWindowCache>();
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
            if (WindowHandle != IntPtr.Zero)
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

                LogProvider.Log.Info($"Table info has been set: {tableData} [{Identifier}]");
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
                LogProvider.Log.Warn($"Table data hasn't been detected from stream. Use table title. [{Identifier}]");

                var title = WinApi.GetWindowText(WindowHandle);

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
                    var sb = new StringBuilder(256);

                    if (WinApi.GetClassName(hWnd, sb, sb.Capacity) != 0)
                    {
                        var windowTitle = WinApi.GetWindowText(hWnd);
                        var windowClassName = sb.ToString();

                        if (IsWindowMatch(windowTitle, windowClassName) && !ignitionWindowCache.IsWindowCached(hWnd))
                        {
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
            var match = !string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(className) &&
                className.Equals(WindowClassName, StringComparison.OrdinalIgnoreCase) &&
                !title.StartsWith("#") && !title.Equals("Poker", StringComparison.OrdinalIgnoreCase) && title.IndexOf("Lobby", StringComparison.OrdinalIgnoreCase) < 0;

            return match;
        }
    }
}