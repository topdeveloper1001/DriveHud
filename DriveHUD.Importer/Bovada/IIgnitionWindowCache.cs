//-----------------------------------------------------------------------
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
        /// <returns>Returns true if the handle exists in the cache, otherwise - false</returns>
        bool IsWindowCached(IntPtr hWnd);

        /// <summary>
        /// Adds the specified handle to the cache
        /// </summary>
        /// <param name="hWnd">The handle to add to the cache</param>
        void AddWindow(IntPtr hWnd);

        /// <summary>
        /// Removes the specified window from the cache
        /// </summary>
        /// <param name="hWnd">The handle to remove from the cache</param>
        void RemoveWindow(IntPtr hWnd);

        /// <summary>
        /// Clears the cache
        /// </summary>
        void Clear();
    }
}