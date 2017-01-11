//-----------------------------------------------------------------------
// <copyright file="PipeManager.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.WinApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace DriveHUD.Importers
{
    internal class PipeManager : IPipeManager
    {
        private Dictionary<ImporterIdentifier, IntPtr> pipes = new Dictionary<ImporterIdentifier, IntPtr>();
        private ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public void AddHandle(ImporterIdentifier importer, IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return;
            }

            try
            {
                locker.EnterWriteLock();

                if (!pipes.ContainsKey(importer))
                {
                    pipes.Add(importer, handle);
                    return;
                }

                pipes[importer] = handle;
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public void RemoveHandle(ImporterIdentifier importer)
        {
            try
            {
                locker.EnterWriteLock();

                if (pipes.ContainsKey(importer))
                {
                    var handle = pipes[importer];
                    ClosePipe(handle);
                    pipes.Remove(importer);
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public IntPtr GetHandle(ImporterIdentifier importer)
        {
            try
            {
                locker.EnterReadLock();

                if (pipes.ContainsKey(importer))
                {
                    return pipes[importer];
                }

                return IntPtr.Zero;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        /// <summary>
        /// Close pipe server
        /// </summary>
        /// <param name="pipeHandle">Handle to pipe</param>
        private void ClosePipe(IntPtr pipeHandle)
        {
            if (pipeHandle == IntPtr.Zero)
            {
                return;
            }

            if (!WinApi.FlushFileBuffers(pipeHandle))
            {
                LogProvider.Log.Warn(this, string.Format(CultureInfo.InvariantCulture, "Flushing pipe buffer failed with code 0x{0:X}.", WinApi.GetLastError()));
            }

            if (!WinApi.DisconnectNamedPipe(pipeHandle))
            {
                LogProvider.Log.Warn(this, string.Format(CultureInfo.InvariantCulture, "Disconnecting pipe failed with code 0x{0:X}.", WinApi.GetLastError()));
            }

            if (!WinApi.CloseHandle(pipeHandle))
            {
                LogProvider.Log.Warn(this, string.Format(CultureInfo.InvariantCulture, "Closing pipe handle failed with code 0x{0:X}.", WinApi.GetLastError()));
            }            
        }
    }
}