//-----------------------------------------------------------------------
// <copyright file="BovadaCatcher.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Diagnostics;
using System.Linq;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Class for inject catching DLL in Bovada or Bodog processes
    /// </summary>
    internal class BovadaCatcher : PokerCatcher, IBovadaCatcher
    {
        /// <summary>
        /// Dll to be injected
        /// </summary>
        private const string dllToInject = "CapPipedI.dll";

        /// <summary>
        /// Name of process in which dll will be injected
        /// </summary>
        private const string processName = "Lobby";

        /// <summary>
        /// Class of window in which dll will be injected
        /// </summary>
        private const string windowClassName = "Qt5QWindowIcon";

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
                return ImporterIdentifier.Bovada;
            }
        }

        protected override ImporterIdentifier[] PipeIdentifiers
        {
            get
            {
                return new[] { ImporterIdentifier.Bovada };
            }
        }

        protected override bool IsEnabled()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
            var siteSettings = settings.SiteSettings.SitesModelList?.FirstOrDefault(x => x.PokerSite == EnumPokerSites.Ignition);

            if (siteSettings != null && !siteSettings.Enabled)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Indicates whether the specified process matches conditions of the current catcher
        /// </summary>
        /// <param name="process">Process to check</param>
        /// <returns>True if process matches; otherwise - false</returns>
        protected override bool IsProcessMatch(Process process)
        {
            return process.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase) &&
                process.GetParentProcess() == null;
        }

        #endregion
    }
}