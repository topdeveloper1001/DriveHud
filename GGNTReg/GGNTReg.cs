﻿//-----------------------------------------------------------------------
// <copyright file="GGNTReg.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DeployLX.Licensing.v5;
using System.ComponentModel;

namespace GGNTRegistration
{
    /// <summary>
    /// Combo license manager
    /// </summary>
    [LicenseProvider(typeof(SecureLicenseManager))]
    public class GGNTReg
    {
        public void ResetCacheForLicense(SecureLicense license)
        {
            SecureLicenseManager.ResetCacheForLicense(license);
        }

        public SecureLicense Validate(LicenseValidationRequestInfo requestInfo)
        {
            var license = SecureLicenseManager.Validate(this, typeof(GGNTReg), requestInfo);
            return license;
        }
    }
}