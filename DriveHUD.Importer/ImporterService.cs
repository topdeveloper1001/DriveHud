﻿//-----------------------------------------------------------------------
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
using DriveHUD.Common.Infrastructure.Events;
using DriveHUD.Common.Linq;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.Bovada;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
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

        public ImporterService()
        {
            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<StartTcpImporterEvent>().Subscribe(arg => StartTcpImporters());
        }

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
        }

        /// <summary>
        /// Gets importer
        /// </summary>
        /// <typeparam name="T">Importer interface</typeparam>
        /// <returns>Registered importer or null</returns>
        public T GetImporter<T>()
        {
            var importer = importers.OfType<T>().FirstOrDefault();
            return importer;
        }

        /// <summary>
        /// Gets running importer
        /// </summary>
        /// <typeparam name="T">Importer interface</typeparam>
        /// <returns>Registered importer or null</returns>
        public T GetRunningImporter<T>()
        {
            var importer = importers.OfType<T>().FirstOrDefault(x =>
            {
                var backgroundProcess = x as IBackgroundProcess;
                return backgroundProcess != null && backgroundProcess.IsRunning;
            });

            return importer;
        }

        /// <summary>
        /// Raise importing stopped event
        /// </summary>
        private void RaiseImportingStopped()
        {
            ImportingStopped?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raise importing stopped if all process has been stopped
        /// </summary>                
        private void OnImporterProcessStopped(object sender, EventArgs e)
        {
            lock (locker)
            {
                System.Diagnostics.Debug.WriteLine($"{sender.ToString()} has exited");

                var isRunning = importers.Any(x => x.IsRunning);

                if (!isRunning)
                {
                    System.Diagnostics.Debug.WriteLine($"All importers have exited");

                    IsStarted = false;
                    RaiseImportingStopped();

                    var ignitionWindowsCache = ServiceLocator.Current.GetInstance<IIgnitionWindowCache>();
                    ignitionWindowsCache.Clear();
                }
            }
        }

        private void StartTcpImporters()
        {
            var tcpImporter = importers.OfType<ITcpImporter>().FirstOrDefault();

            if (tcpImporter != null && !tcpImporter.IsRunning)
            {
                tcpImporter.Start();
            }

            importers
                .OfType<ITcpPacketImporter>()
                .Where(x => !x.IsRunning && !x.IsDisabled())
                .ForEach(x => x.Start());
        }
    }
}