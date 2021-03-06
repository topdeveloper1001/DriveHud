﻿//-----------------------------------------------------------------------
// <copyright file="IIgnitionWindowCache.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Represents simple cache for handles of windows
    /// </summary>
    internal interface IIgnitionWindowCache
    {
        /// <summary>
        /// Checks whenever the specified handle exists in the cache
        /// </summary>
        /// <param name="hWnd">The handle to check if exists in the cache</param>
        /// <returns>Returns table associated with the handle if the handle exists in the cache, otherwise - null</returns>
        BovadaTable GetCachedTable(IntPtr hWnd);

        /// <summary>
        /// Adds the specified handle to the cache
        /// </summary>
        /// <param name="hWnd">The handle to add to the cache</param>
        /// <param name="bovadaTable">The table associated with the handle</param>
        void AddWindow(IntPtr hWnd, BovadaTable bovadaTable);

        /// <summary>
        /// Removes the specified window from the cache
        /// </summary>
        /// <param name="hWnd">The handle to remove from the cache</param>
        /// <param name="cacheAsPreviousTable">Determines whenever removed table must be cached as previous table</param>
        void RemoveWindow(IntPtr hWnd, bool cacheAsPreviousTable = false);

        /// <summary>
        /// Clears the cache
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the handle of previous table
        /// </summary>
        /// <param name="tableId">Table id</param>
        /// <returns>Handle of previous table</returns>
        IntPtr GetPreviousHandle(uint tableId);
    }
}