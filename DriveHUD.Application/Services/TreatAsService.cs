//-----------------------------------------------------------------------
// <copyright file="TreatAsService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DriveHUD.Application.Services
{
    internal class TreatAsService : ITreatAsService
    {
        private Dictionary<IntPtr, EnumTableType> teatedWindows = new Dictionary<IntPtr, EnumTableType>();

        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public void AddOrUpdate(IntPtr handle, EnumTableType tableType)
        {
            if (handle == IntPtr.Zero)
            {
                return;
            }

            rwLock.EnterWriteLock();

            try
            {
                if (!teatedWindows.ContainsKey(handle))
                {
                    teatedWindows.Add(handle, tableType);
                    return;
                }

                teatedWindows[handle] = tableType;
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public EnumTableType? GetTableType(IntPtr handle)
        {
            rwLock.EnterReadLock();

            try
            {
                if (!teatedWindows.ContainsKey(handle))
                {
                    return null;
                }

                return teatedWindows[handle];
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }
    }
}