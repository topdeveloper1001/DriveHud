//-----------------------------------------------------------------------
// <copyright file="BetOnlineTableService.cs" company="Ace Poker Solutions">
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
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using DriveHUD.Common.WinApi;
using System.Collections.Generic;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;

namespace DriveHUD.Importers.BetOnline
{
    public class BetOnlineTableService : IBetOnlineTableService
    {
        private Process pokerClientProcess;

        private const string ProcessName = "GameClient";

        private ReaderWriterLockSlim lockObject;

        private readonly Dictionary<string, Dictionary<int, string>> players;

        public BetOnlineTableService()
        {
            players = new Dictionary<string, Dictionary<int, string>>();
            lockObject = new ReaderWriterLockSlim();
        }

        public int GetSessionCode(string tableId, out EnumPokerSites site)
        {
            var tableInfo = GetTableInformation(tableId);

            var hwnd = tableInfo.Item1;
            site = tableInfo.Item2;

            if (hwnd == IntPtr.Zero)
            {
                var random = new Random();
                var sessionCode = random.Next(1000000, 9999999);
                return sessionCode;
            }

            return hwnd.ToInt32();
        }

        public string GetRandomPlayerName(string sessionCode, int seat)
        {
            lockObject.EnterUpgradeableReadLock();

            try
            {
                Dictionary<int, string> playerNameBySeat;

                string playerName;

                if (!players.ContainsKey(sessionCode))
                {
                    lockObject.EnterWriteLock();

                    try
                    {
                        playerName = Utils.GenerateRandomPlayerName(seat);

                        playerNameBySeat = new Dictionary<int, string>();
                        playerNameBySeat.Add(seat, playerName);

                        players.Add(sessionCode, playerNameBySeat);

                        return playerName;
                    }
                    finally
                    {
                        lockObject.ExitWriteLock();
                    }
                }

                playerNameBySeat = players[sessionCode];

                if (playerNameBySeat.ContainsKey(seat))
                {
                    return playerNameBySeat[seat];
                }

                lockObject.EnterWriteLock();

                try
                {
                    playerName = Utils.GenerateRandomPlayerName(seat);

                    playerNameBySeat.Add(seat, playerName);

                    return playerName;
                }
                finally
                {
                    lockObject.ExitWriteLock();
                }
            }
            finally
            {
                lockObject.ExitUpgradeableReadLock();
            }
        }

        public void Reset()
        {
            lockObject.EnterWriteLock();

            try
            {
                players.Clear();
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }

        #region Infrastructure

        private IEnumerable<IntPtr> GetProcessWindowsHandles()
        {
            var handles = new List<IntPtr>();

            if (pokerClientProcess == null || pokerClientProcess.HasExited)
            {
                pokerClientProcess = GetPokerClientProcess();

                if (pokerClientProcess == null)
                {
                    return handles;
                }
            }

            foreach (ProcessThread thread in pokerClientProcess.Threads)
            {
                WinApi.EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                {
                    handles.Add(hWnd);
                    return true;
                }, IntPtr.Zero);
            }

            return handles;
        }

        private Tuple<IntPtr, EnumPokerSites> GetTableInformation(string tableId)
        {
            try
            {
                var handles = GetProcessWindowsHandles();

                var site = EnumPokerSites.Unknown;
                var matchedHandle = IntPtr.Zero;

                foreach (var handle in handles)
                {
                    var title = WinApi.GetWindowText(handle);

                    if (site == EnumPokerSites.Unknown)
                    {
                        site = GetPokerSite(title);
                    }

                    if ((matchedHandle == IntPtr.Zero) && Match(title, tableId))
                    {
                        matchedHandle = handle;
                    }

                    if (matchedHandle != IntPtr.Zero && site != EnumPokerSites.Unknown)
                    {
                        return Tuple.Create(matchedHandle, site);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not find betonline table", e);
            }

            return Tuple.Create(IntPtr.Zero, EnumPokerSites.Unknown);
        }

        protected EnumPokerSites GetPokerSite(string title)
        {
            if (title.StartsWith("BetOnline", StringComparison.InvariantCultureIgnoreCase))
            {
                return EnumPokerSites.BetOnline;
            }

            if (title.StartsWith("TigerGaming", StringComparison.InvariantCultureIgnoreCase))
            {
                return EnumPokerSites.TigerGaming;
            }

            if (title.StartsWith("SportsBetting", StringComparison.InvariantCultureIgnoreCase))
            {
                return EnumPokerSites.SportsBetting;
            }

            return EnumPokerSites.Unknown;
        }

        protected bool Match(string title, string tableId)
        {
            if (title.StartsWith("Tournament", StringComparison.InvariantCultureIgnoreCase) || title.StartsWith("BetOnline", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (title.IndexOf("Hand", StringComparison.InvariantCultureIgnoreCase) > 0 && title.IndexOf(tableId, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get client process
        /// </summary>
        /// <returns>Client process if exist, otherwise - null</returns>
        protected virtual Process GetPokerClientProcess()
        {
            var processes = Process.GetProcesses();

            var pokerClientProcess = processes.FirstOrDefault(x => x.ProcessName.Equals(ProcessName));

            return pokerClientProcess;
        }

        #endregion
    }
}