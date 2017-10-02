//-----------------------------------------------------------------------
// <copyright file="PokerStarsZoomImporter.cs" company="Ace Poker Solutions">
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
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System.Linq;

namespace DriveHUD.Importers.PokerStars
{
    internal class PokerStarsZoomImporter : PokerClientImporter, IPokerStarsZoomImporter
    {
        private const uint bufferSize = 4096;

        private const int pipeReadingTimeout = 5000;

        private const string site = "PokerStarsZoom";

        private const string pipeName = @"\\.\pipe\APSPokerStars";

        protected override EnumPokerSites Site
        {
            get { return EnumPokerSites.PokerStars; }
        }

        protected override ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.PokerStarsZoom;
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

            var dataManager = ServiceLocator.Current.GetInstance<IPokerStarsZoomDataManager>();
            dataManager.Initialize(dataManagerInfo);

            return dataManager;
        }

        protected override PokerClientLoggerConfiguration CreatePokerClientLoggerConfiguration()
        {
            return null;
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

        protected override bool IsDisabled()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
            var siteSettings = settings.SiteSettings.SitesModelList?.FirstOrDefault(x => x.PokerSite == Site);

            return siteSettings != null && (!siteSettings.Enabled || !siteSettings.FastPokerEnabled);
        }
    }
}