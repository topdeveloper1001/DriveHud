//-----------------------------------------------------------------------
// <copyright file="SessionService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Licensing;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Resources;
using DriveHUD.Importers;
using HandHistories.Objects.GameDescription;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Application.Security
{
    internal class SessionService : ISessionService
    {
        private IUserSession userSession;
        private readonly ILicenseService licenseService;
        private bool isInitialized;

        public SessionService()
        {
            licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();
        }

        public IUserSession GetUserSession()
        {
            if (!isInitialized)
            {
                throw new DHInternalException(new NonLocalizableString("Session has not been initialized"));
            }

            return userSession;
        }

        public void Initialize()
        {
            isInitialized = true;

            var registeredLicenses = licenseService.LicenseInfos.Where(x => x.IsRegistered).ToArray();

            // if any license is not trial
            if (registeredLicenses.Any(x => !x.IsTrial))
            {
                registeredLicenses = registeredLicenses.Where(x => !x.IsTrial).ToArray();
            }

            userSession = new UserSession(registeredLicenses);
        }        
    }
}