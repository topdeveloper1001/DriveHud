//-----------------------------------------------------------------------
// <copyright file="BetOnlineImporter.cs" company="Ace Poker Solutions">
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
using Microsoft.Practices.ServiceLocation;
using DriveHUD.Entities;
using Model.Settings;
using System.Linq;

namespace DriveHUD.Importers.BetOnline
{
    /// <summary>
    /// Imports hands from BetOnline poker clients
    /// </summary>
    internal class BetOnlineImporter : PokerClientImporter, IBetOnlineImporter
    {
        private const string site = "BetOnline";

        private const uint bufferSize = 65536;

        private const int pipeReadingTimeout = 5000;

        private const string pipeName = @"\\.\pipe\BOCServer";

        protected override EnumPokerSites Site
        {
            get { return EnumPokerSites.BetOnline; }
        }

        protected override ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.BetOnline;
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
            var betOnlineDataManagerInfo = new PokerClientDataManagerInfo
            {
                Logger = logger,
                Site = site
            };

            var betOnlineDataManager = ServiceLocator.Current.GetInstance<IBetOnlineDataManager>();
            betOnlineDataManager.Initialize(betOnlineDataManagerInfo);

            return betOnlineDataManager;
        }

        protected override PokerClientLoggerConfiguration CreatePokerClientLoggerConfiguration()
        {
            var logger = new PokerClientLoggerConfiguration
            {
                DateFormat = "yyy-MM-dd",
                DateTimeFormat = "HH:mm:ss",
                LogCleanupTemplate = "bol-games-*-*-*.log",
                LogDirectory = "Logs",
                LogTemplate = "bol-games-{0}.log",
                MessagesInBuffer = 0
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

        protected override bool IsDisabled()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
            var siteSettings = settings.SiteSettings.SitesModelList?.Where(x => x.PokerSite == EnumPokerSites.BetOnline || x.PokerSite == EnumPokerSites.SportsBetting || x.PokerSite == EnumPokerSites.TigerGaming);

            return siteSettings != null && siteSettings.All(x => !x.Enabled);
        }
    }
}