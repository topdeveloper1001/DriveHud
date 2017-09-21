//-----------------------------------------------------------------------
// <copyright file="PokerStarsZoomCatcher.cs" company="Ace Poker Solutions">
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
    /// <summary>
    /// Class for inject catching DLL in PS process
    /// </summary>
    internal class PokerStarsZoomCatcher : PokerCatcher, IPokerStarsZoomCatcher
    {
        /// <summary>
        /// Dll to be injected
        /// </summary>
        private const string dllToInject = "CapPipedP.dll";

        /// <summary>
        /// Name of process in which dll will be injected
        /// </summary>
        private const string processName = "PokerStars";

        #region PokerCatcher members

        protected override string DllToInject
        {
            get
            {
                return dllToInject;
            }
        }

        protected override string ProcessName
        {
            get
            {
                return processName;
            }
        }

        protected override string WindowClassName
        {
            get
            {
                return string.Empty;
            }
        }

        protected override ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.PokerStarsZoom;
            }
        }

        protected override ImporterIdentifier[] PipeIdentifiers
        {
            get
            {
                return new[] { ImporterIdentifier.PokerStarsZoom };
            }
        }

        protected override bool IsEnabled()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
            var siteSettings = settings.SiteSettings.SitesModelList?.FirstOrDefault(x => x.PokerSite == EnumPokerSites.PokerStars);

            if (siteSettings != null && !siteSettings.Enabled)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}