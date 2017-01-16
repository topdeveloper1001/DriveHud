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
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.Importers.BetOnline
{
    internal class BetOnlineTableService : IBetOnlineTableService
    {
        private Process pokerClientProcess;
        protected bool isRunning = false;
        protected CancellationTokenSource cancellationTokenSource;

        private const string ProcessName = "GameClient";

        private ReaderWriterLockSlim lockObject;

        private const int ProcessSearchingTimeout = 1500;

        private readonly Dictionary<string, Dictionary<int, string>> players;

        private Dictionary<IntPtr, HashSet<ulong>> handlesHandHistoryId;

        private EnumPokerSites site;

        public BetOnlineTableService()
        {
            players = new Dictionary<string, Dictionary<int, string>>();
            lockObject = new ReaderWriterLockSlim();
            handlesHandHistoryId = new Dictionary<IntPtr, HashSet<ulong>>();
        }

        #region IBackgroundProcess implementation

        /// <summary>
        /// Process has been stopped
        /// </summary>
        public event EventHandler ProcessStopped;

        /// <summary>
        /// True if process is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }

        /// <summary>
        /// Starts background process
        /// </summary>    
        public void Start()
        {
            if (isRunning)
            {
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
          
            site = EnumPokerSites.Unknown;

            // start main job
            Task.Run(() =>
            {
                isRunning = true;
                ProcessTableWindows();
            }, cancellationTokenSource.Token);
        }

        /// <summary>
        /// Stop background process
        /// </summary>     
        public void Stop()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        public void Dispose()
        {
        }

        #endregion

        public int GetWindowHandle(ulong handId, out EnumPokerSites site)
        {
            lockObject.EnterReadLock();

            try
            {
                site = this.site;

                foreach (KeyValuePair<IntPtr, HashSet<ulong>> handleHandIds in handlesHandHistoryId)
                {
                    if (handleHandIds.Value != null && handleHandIds.Value.Contains(handId))
                    {
                        return handleHandIds.Key.ToInt32();
                    }
                }
            }
            finally
            {
                lockObject.ExitReadLock();
            }

            return 0;
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
      
        #region Infrastructure

        private void ProcessTableWindows()
        {
            try
            {
                while (true)
                {
                    if (pokerClientProcess == null || pokerClientProcess.HasExited)
                    {
                        // clear resources
                        if (pokerClientProcess != null && pokerClientProcess.HasExited)
                        {
                            Clear();
                        }

                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            Clear();
                            RaiseProcessStopped();
                            return;
                        }

                        pokerClientProcess = GetPokerClientProcess();

                        if (pokerClientProcess == null)
                        {
                            Task.Delay(ProcessSearchingTimeout).Wait();
                            continue;
                        }
                    }

                    var handles = new List<IntPtr>();

                    foreach (ProcessThread thread in pokerClientProcess.Threads)
                    {
                        WinApi.EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                        {
                            handles.Add(hWnd);
                            return true;
                        }, IntPtr.Zero);
                    }

                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        Clear();
                        RaiseProcessStopped();
                        return;
                    }

                    lockObject.EnterWriteLock();

                    try
                    {
                        ProcessHandles(handles);
                    }
                    finally
                    {
                        lockObject.ExitWriteLock();
                    }

                    Task.Delay(ProcessSearchingTimeout).Wait();
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Overlaying is failed", e);
            }
        }

        private void RemoveClosedHandles(List<IntPtr> handles)
        {
            var handlesToRemove = (from existingHandle in handlesHandHistoryId.Keys
                                   join handle in handles on existingHandle equals handle into gj
                                   from joinedHandle in gj.DefaultIfEmpty()
                                   where joinedHandle == IntPtr.Zero
                                   select existingHandle).ToArray();

            foreach (var handle in handlesToRemove)
            {
                handlesHandHistoryId.Remove(handle);
            }
        }

        private void ProcessHandles(List<IntPtr> handles)
        {
            RemoveClosedHandles(handles);

            foreach (var handle in handles)
            {
                var title = WinApi.GetWindowText(handle);

                if (site == EnumPokerSites.Unknown)
                {
                    site = GetPokerSite(title);
                }

                if (Match(title))
                {
                    var titles = title.Split('|');

                    if (titles.Length < 2)
                    {
                        continue;
                    }

                    var handIdText = titles[1].Trim();
                    handIdText = handIdText.Substring(6, handIdText.Length - 6);

                    ulong handId;

                    if (!ulong.TryParse(handIdText, out handId))
                    {
                        continue;
                    }

                    AddHandHistoryIdToHandle(handle, handId);
                }
            }
        }

        private void AddHandHistoryIdToHandle(IntPtr handle, ulong handHistoryId)
        {
            HashSet<ulong> hands;

            if (!handlesHandHistoryId.ContainsKey(handle))
            {
                hands = new HashSet<ulong>();
                handlesHandHistoryId.Add(handle, hands);
            }
            else
            {
                hands = handlesHandHistoryId[handle];
            }

            if (!hands.Contains(handHistoryId))
            {
                hands.Add(handHistoryId);
            }
        }

        private EnumPokerSites GetPokerSite(string title)
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

        private bool Match(string title)
        {
            if (title.StartsWith("Tournament", StringComparison.OrdinalIgnoreCase) || title.StartsWith("BetOnline", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (title.IndexOf("| Hand", StringComparison.OrdinalIgnoreCase) > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get client process
        /// </summary>
        /// <returns>Client process if exist, otherwise - null</returns>
        private Process GetPokerClientProcess()
        {
            var processes = Process.GetProcesses();

            var pokerClientProcess = processes.FirstOrDefault(x => x.ProcessName.Equals(ProcessName));

            return pokerClientProcess;
        }

        /// <summary>
        /// Get client process
        /// </summary>
        /// <returns>Client process if exist, otherwise - null</returns>
        protected virtual Process[] GetPokerClientProcesses()
        {
            var processes = Process.GetProcesses();

            var pokerClientProcesses = processes.Where(x => x.ProcessName.Equals(ProcessName)).ToArray();

            return pokerClientProcesses;
        }

        /// Raise process stopped event
        /// </summary>
        private void RaiseProcessStopped()
        {
            isRunning = false;

            var handler = ProcessStopped;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Clear resources
        /// </summary>
        private void Clear()
        {
            lockObject.EnterWriteLock();

            try
            {
                handlesHandHistoryId.Clear();
                players.Clear();
                site = EnumPokerSites.Unknown;
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }

        #endregion
    }
}