//-----------------------------------------------------------------------
// <copyright file="BovadaImporter.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Imports hands from Bovada/Bodog poker clients
    /// </summary>
    internal class BovadaImporter : PokerClientImporter, IBovadaImporter
    {
        private const uint bufferSize = 4096;

        private const int pipeReadingTimeout = 5000;

        private const string site = "Bovada";

        private const string pipeName = @"\\.\pipe\BCCServer";

        protected override EnumPokerSites Site
        {
            get { return EnumPokerSites.Ignition; }
        }

        protected override ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.Bovada;
            }
        }

        public override string SiteString
        {
            get
            {
                return site;
            }
        }

        protected override IPokerClientDataManager CreatePokerClientDataManager(IPokerClientEncryptedLogger logger)
        {
            var bovadaDataManagerInfo = new PokerClientDataManagerInfo
            {
                Logger = logger,
                Site = site
            };

            var bovadaDataManager = ServiceLocator.Current.GetInstance<IBovadaDataManager>();
            bovadaDataManager.Initialize(bovadaDataManagerInfo);

            return bovadaDataManager;
        }


        protected override PokerClientLoggerConfiguration CreatePokerClientLoggerConfiguration()
        {
            var logger = new PokerClientLoggerConfiguration
            {
                DateFormat = "yyy-MM-dd",
                DateTimeFormat = "HH:mm:ss",
                LogCleanupTemplate = "ign-games-*-*-*.log",
                LogDirectory = "Logs",
                LogTemplate = "ign-games-{0}.log",
                MessagesInBuffer = 30
            };

            return logger;
        }

        protected override uint BufferSize
        {
            get
            {
                return bufferSize;
            }
        }

        protected override int PipeReadingTimeout
        {
            get
            {
                return pipeReadingTimeout;
            }
        }

        protected override string PipeName
        {
            get
            {
                return pipeName;
            }
        }
    }
}