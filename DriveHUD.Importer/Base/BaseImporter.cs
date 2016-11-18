//-----------------------------------------------------------------------
// <copyright file="PokerStarsImporter.cs" company="Ace Poker Solutions">
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
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.Importers
{
    internal abstract class BaseImporter : IBackgroundProcess
    {
        protected bool isRunning;

        protected CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Importer site (PS, iPoker, Bovada, etc.)
        /// </summary>
        public abstract string Site { get; }

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }

        #region IBackgroundProcess implementation

        public event EventHandler ProcessStopped;

        /// <summary>
        /// Starts importing hands from poker client
        /// </summary>
        /// <param name="configuration"></param>
        public virtual void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();

            if (isRunning)
            {
                return;
            }

            LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "Starting importing \"{0}\" data", Site));

            isRunning = true;

            // start main job
            Task.Run(() => DoImport(), cancellationTokenSource.Token);
        }

        /// <summary>
        /// Stop importing, send cancellation request to stop internal thread
        /// </summary>
        public virtual void Stop()
        {
            LogProvider.Log.Info(this, string.Format(CultureInfo.InvariantCulture, "Stopping importing \"{0}\" data", Site));

            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        #endregion

        #region Infrastructure      

        /// <summary>
        /// Imports data from different sources
        /// </summary>
        protected abstract void DoImport();

        /// <summary>
        /// Raise process stopped event
        /// </summary>
        protected void RaiseProcessStopped()
        {
            isRunning = false;

            var handler = ProcessStopped;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IDisposable implementation

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here.                    
                    Disposing();
                }

                // Clear unmanaged resources here
                disposed = true;
            }
        }

        protected virtual void Disposing()
        {
        }

        ~BaseImporter()
        {
            Dispose(false);
        }

        #endregion
    }
}
