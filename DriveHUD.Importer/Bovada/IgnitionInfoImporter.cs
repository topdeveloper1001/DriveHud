//-----------------------------------------------------------------------
// <copyright file="IgntionInfoImporter.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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
using Model.Settings;
using System.Linq;

namespace DriveHUD.Importers.Bovada
{
    internal class IgnitionInfoImporter : PokerClientImporter, IIgnitionInfoImporter
    {
        private const string site = "IgntionInfoImporter";

        private const uint bufferSize = 65531;

        private const int pipeReadingTimeout = 2000;

        private const string pipeName = @"\\.\pipe\BodogServerInfo";

        public override string SiteString
        {
            get
            {
                return site;
            }
        }

        protected override ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.IgnitionInfo;
            }
        }

        protected override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.Ignition;
            }
        }

        public IIgnitionInfoDataManager InfoDataManager
        {
            get;
            private set;
        }

        protected override void InitializeLogger()
        {
        }

        protected override IPokerClientDataManager CreatePokerClientDataManager(IPokerClientEncryptedLogger logger)
        {
            var bovadaDataManagerInfo = new PokerClientDataManagerInfo
            {
                Logger = logger,
                Site = site
            };

            var infoDataManager = ServiceLocator.Current.GetInstance<IIgnitionInfoDataManager>();
            infoDataManager.Initialize(bovadaDataManagerInfo);

            InfoDataManager = infoDataManager;

            return infoDataManager;
        }

        protected override PokerClientLoggerConfiguration CreatePokerClientLoggerConfiguration()
        {
            return null;
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

        public override bool IsDisabled()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
            var siteSettings = settings.SiteSettings.SitesModelList?.FirstOrDefault(x => x.PokerSite == EnumPokerSites.Ignition);

            return siteSettings != null && !siteSettings.Enabled;
        }
    }
}