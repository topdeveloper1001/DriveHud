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

using System;
using System.Collections.Generic;
using System.Threading;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Represents simple cache for handles of windows
    /// </summary>
    internal class IgnitionWindowCache : IIgnitionWindowCache
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        private HashSet<IntPtr> windowsHandles = new HashSet<IntPtr>();

        /// <summary>
        /// Adds the specified handle to the cache
        /// </summary>
        /// <param name="hWnd">The handle to add to the cache</param>
        public void AddWindow(IntPtr hWnd)
        {
            cacheLock.EnterUpgradeableReadLock();

            try
            {
                if (windowsHandles.Contains(hWnd))
                {
                    return;
                }

                cacheLock.EnterWriteLock();

                try
                {
                    if (!windowsHandles.Contains(hWnd))
                    {
                        windowsHandles.Add(hWnd);
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
        /// <returns>Returns true if the handle exists in the cache, otherwise - false</returns>
        public bool IsWindowCached(IntPtr hWnd)
        {
            cacheLock.EnterReadLock();

            try
            {
                return windowsHandles.Contains(hWnd);
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Removes the specified window from the cache
        /// </summary>
        /// <param name="hWnd">The handle to remove from the cache</param>
        public void RemoveWindow(IntPtr hWnd)
        {
            cacheLock.EnterUpgradeableReadLock();

            try
            {
                if (windowsHandles.Contains(hWnd))
                {
                    return;
                }

                cacheLock.EnterWriteLock();

                try
                {
                    if (!windowsHandles.Contains(hWnd))
                    {
                        windowsHandles.Remove(hWnd);
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