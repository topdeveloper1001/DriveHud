//-----------------------------------------------------------------------
// <copyright file="ImporterService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Importer service
    /// </summary>
    internal class ImporterService : IImporterInternalService
    {
        private List<IBackgroundProcess> importers = new List<IBackgroundProcess>();

        private object locker = new object();

        public bool IsStarted
        {
            get;
            private set;
        }

        public event EventHandler ImportingStopped;

        /// <summary>
        /// Register importer 
        /// </summary>
        /// <typeparam name="T">Interface of importer</typeparam>
        public IImporterService Register<T>() where T : IBackgroundProcess
        {
            Check.Require(importers.All(x => x.GetType() != typeof(T)), "Importer has been already registered");

            var importer = ServiceLocator.Current.GetInstance<T>();

            Check.Require(importer != null, "Importer wasn't found");

            importer.ProcessStopped += OnImporterProcessStopped;
            importers.Add(importer);

            return this;
        }

        /// <summary>
        /// Start import
        /// </summary>
        public void StartImport()
        {
            foreach (var importer in importers)
            {
                importer.Start();
            }

            IsStarted = true;
        }

        /// <summary>
        /// Stop import
        /// </summary>
        public void StopImport()
        {
            foreach (var importer in importers)
            {
                importer.Stop();
            }

            IsStarted = false;
        }

        /// <summary>
        /// Raise importing stopped event
        /// </summary>
        private void RaiseImportingStopped()
        {
            var handler = ImportingStopped;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raise importing stopped if all process has been stopped
        /// </summary>                
        private void OnImporterProcessStopped(object sender, EventArgs e)
        {
            lock(locker)
            {
                System.Diagnostics.Debug.WriteLine($"{sender.ToString()} has exited");

                var isRunning = importers.Any(x => x.IsRunning);

                if (!isRunning)
                {
                    RaiseImportingStopped();
                }
            }
        }
    }
}