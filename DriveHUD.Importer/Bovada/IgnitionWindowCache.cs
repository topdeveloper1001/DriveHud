//-----------------------------------------------------------------------
// <copyright file="IgnitionWindowCache.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.WinApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Represents simple cache for handles of windows
    /// </summary>
    internal class IgnitionWindowCache : IIgnitionWindowCache
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        private Dictionary<IntPtr, BovadaTable> windowsHandles = new Dictionary<IntPtr, BovadaTable>();
        private Dictionary<uint, IntPtr> previousTables = new Dictionary<uint, IntPtr>();

        /// <summary>
        /// Adds the specified handle to the cache
        /// </summary>
        /// <param name="hWnd">The handle to add to the cache</param>
        public void AddWindow(IntPtr hWnd, BovadaTable table)
        {
            cacheLock.EnterUpgradeableReadLock();

            try
            {
                if (windowsHandles.ContainsKey(hWnd))
                {
                    return;
                }

                cacheLock.EnterWriteLock();

                try
                {
                    if (!windowsHandles.ContainsKey(hWnd))
                    {
                        windowsHandles.Add(hWnd, table);
                    }
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Clears the cache
        /// </summary>
        public void Clear()
        {
            cacheLock.EnterWriteLock();

            try
            {
                windowsHandles.Clear();
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Checks whenever the specified handle exists in the cache
        /// </summary>
        /// <param name="hWnd">The handle to check if exists in the cache</param>
        /// <returns>Returns table associated with the handle if the handle exists in the cache, otherwise - null</returns>
        public BovadaTable GetCachedTable(IntPtr hWnd)
        {
            cacheLock.EnterUpgradeableReadLock();

            try
            {
                var closedWindows = windowsHandles.Keys.Where(wh => !WinApi.IsWindow(wh)).ToArray();

                if (closedWindows.Length > 0)
                {
                    cacheLock.EnterWriteLock();

                    try
                    {
                        closedWindows.ForEach(wh => windowsHandles.Remove(wh));
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                }

                if (windowsHandles.ContainsKey(hWnd))
                {
                    return windowsHandles[hWnd];
                }

                return null;
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
        }


        /// <summary>
        /// Gets the handle of previous table
        /// </summary>
        /// <param name="tableId">Table id</param>
        /// <returns>Handle of previous table</returns>
        public IntPtr GetPreviousHandle(uint tableId)
        {
            cacheLock.EnterUpgradeableReadLock();

            try
            {
                var closedWindows = previousTables.Where(pt => !WinApi.IsWindow(pt.Value)).ToArray();

                if (closedWindows.Length > 0)
                {
                    cacheLock.EnterWriteLock();

                    try
                    {
                        closedWindows.ForEach(pt => previousTables.Remove(pt.Key));
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                }

                if (previousTables.TryGetValue(tableId, out IntPtr previousTableHandle))
                {
                    return previousTableHandle;
                }

                return IntPtr.Zero;
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Removes the specified window from the cache
        /// </summary>
        /// <param name="hWnd">The handle to remove from the cache</param>
        public void RemoveWindow(IntPtr hWnd, bool cacheAsPreviousTable = false)
        {
            cacheLock.EnterUpgradeableReadLock();

            try
            {
                if (!windowsHandles.ContainsKey(hWnd))
                {
                    return;
                }

                cacheLock.EnterWriteLock();

                try
                {
                    if (windowsHandles.TryGetValue(hWnd, out BovadaTable table))
                    {
                        windowsHandles.Remove(hWnd);

                        if (cacheAsPreviousTable && !table.IsTournament &&
                            !table.IsZonePokerTable && WinApi.IsWindow(hWnd) && !previousTables.ContainsKey(table.TableId))
                        {
                            previousTables.Add(table.TableId, hWnd);
                        }
                        else if (!cacheAsPreviousTable && previousTables.ContainsKey(table.TableId))
                        {
                            previousTables.Remove(table.TableId);
                        }
                    }
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
        }
    }
}