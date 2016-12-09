//-----------------------------------------------------------------------
// <copyright file="BetOnlineTournamentImporter.cs" company="Ace Poker Solutions">
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
using DriveHUD.Importers.Loggers;
using Microsoft.Practices.ServiceLocation;

namespace DriveHUD.Importers.BetOnline
{
    internal class BetOnlineTournamentImporter : PokerClientImporter, IBetOnlineTournamentImporter
    {
        private const string site = "BetOnline";

        private const uint bufferSize = 262144;

        private const int pipeReadingTimeout = 5000;

        private const string pipeName = @"\\.\pipe\BOCServerInfo";

        protected override EnumPokerSites Site
        {
            get { return EnumPokerSites.BetOnline; }
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
            var betOnlineDataManagerInfo = new PokerClientDataManagerInfo
            {
                Logger = logger,
                Site = site
            };

            var betOnlineTournamentDataManager = ServiceLocator.Current.GetInstance<IBetOnlineTournamentManager>();
            betOnlineTournamentDataManager.Initialize(betOnlineDataManagerInfo);

            return betOnlineTournamentDataManager;
        }

        protected override PokerClientLoggerConfiguration CreatePokerClientLoggerConfiguration()
        {
            return null;
        }

        protected override void InitializeLogger()
        {
            var pokerClientLoggerConfiguration = CreatePokerClientLoggerConfiguration();

            pokerClientLogger = ServiceLocator.Current.GetInstance<IPokerClientEncryptedLogger>(LogServices.BetOnlineTournament.ToString());
            pokerClientLogger.Initialize(pokerClientLoggerConfiguration);
        }

        protected override string PipeName
        {
            get
            {
                return pipeName;
            }
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
    }
}