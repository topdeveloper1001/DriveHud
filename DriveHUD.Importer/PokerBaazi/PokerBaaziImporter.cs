//-----------------------------------------------------------------------
// <copyright file="PokerBaaziCatcher.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;

namespace DriveHUD.Importers.PokerBaazi
{
    internal class PokerBaaziImporter : PokerClientImporter, IPokerBaaziImporter
    {
        private const string site = "PokerBaazi";

        private const string pipeName = @"\\.\pipe\PBServer";

        protected override ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.PokerBaazi;
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
            var dataManagerInfo = new PokerClientDataManagerInfo
            {
                Logger = logger,
                Site = site
            };

            var dataManager = ServiceLocator.Current.GetInstance<IPokerBaaziDataManager>();
            dataManager.Initialize(dataManagerInfo);

            return dataManager;
        }


        protected override PokerClientLoggerConfiguration CreatePokerClientLoggerConfiguration()
        {
            var logger = new PokerClientLoggerConfiguration
            {
                DateFormat = "yyy-MM-dd",
                DateTimeFormat = "HH:mm:ss",
                LogCleanupTemplate = "pbz-games-*-*-*.log",
                LogDirectory = "Logs",
                LogTemplate = "pbz-games-{0}.log",
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

        public override bool IsDisabled()
        {
            return false;
        }

        protected override uint BufferSize => 8192;

        protected override int PipeReadingTimeout => 2500;

        protected override EnumPokerSites Site => EnumPokerSites.PokerBaazi;
    }
}