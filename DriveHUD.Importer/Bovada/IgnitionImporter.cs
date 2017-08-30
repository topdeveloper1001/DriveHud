//-----------------------------------------------------------------------
// <copyright file="IgnitionImporter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Microsoft.Practices.ServiceLocation;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Imports hands from Ignition/Bodog poker clients (v.4.50+)
    /// </summary>
    internal class IgnitionImporter : BovadaImporter, IIgnitionImporter
    {
        private const string site = "Ignition";

        private const string pipeName = @"\\.\pipe\BodogServer";

        protected override ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.Ignition;
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

            var bovadaDataManager = ServiceLocator.Current.GetInstance<IIgnitionDataManager>();
            bovadaDataManager.Initialize(bovadaDataManagerInfo);

            return bovadaDataManager;
        }


        protected override PokerClientLoggerConfiguration CreatePokerClientLoggerConfiguration()
        {
            var logger = new PokerClientLoggerConfiguration
            {
                DateFormat = "yyy-MM-dd",
                DateTimeFormat = "HH:mm:ss",
                LogCleanupTemplate = "ign-v2-games-*-*-*.log",
                LogDirectory = "Logs",
                LogTemplate = "ign-v2-games-{0}.log",
                MessagesInBuffer = 30
            };

            return logger;
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