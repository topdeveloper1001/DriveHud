//-----------------------------------------------------------------------
// <copyright file="BetOnlineCatcher.cs" company="Ace Poker Solutions">
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
using System;
using System.Linq;

namespace DriveHUD.Importers.BetOnline
{
    /// <summary>
    /// Class for inject catching DLL in BetOnline processes
    /// </summary>
    internal class BetOnlineCatcher : PokerCatcher, IBetOnlineCatcher
    {
        /// Dll to be injected
        /// </summary>
        private const string dllToInject = "CapPipedB.dll";

        /// <summary>
        /// Name of process in which dll will be injected
        /// </summary>
        private const string processName = "GameClient";

        /// <summary>
        /// Class of window in which dll will be injected
        /// </summary>
        private const string windowClassName = "Antares Game Client Window Class";

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
                return windowClassName;
            }
        }

        protected override ImporterIdentifier Identifier
        {
            get
            {
                return ImporterIdentifier.BetOnline;
            }
        }

        protected override ImporterIdentifier[] PipeIdentifiers
        {
            get
            {
                return new[] { ImporterIdentifier.BetOnline, ImporterIdentifier.BetOnlineTournament };
            }
        }

        #endregion 

        protected override bool IsEnabled()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

            var siteSettings = settings.SiteSettings.SitesModelList?.Where(x => x.PokerSite == EnumPokerSites.BetOnline ||
                x.PokerSite == EnumPokerSites.SportsBetting ||
                x.PokerSite == EnumPokerSites.TigerGaming);

            return siteSettings != null && siteSettings.Any(x => x.Enabled);
        }
    }
}