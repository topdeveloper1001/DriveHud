//-----------------------------------------------------------------------
// <copyright file="XRHReg.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DeployLX.Licensing.v5;
using DriveHUD.Common.Security;

namespace DriveHUD.PlayerXRay.Security
{
    internal class XRHReg : ILicenseManager
    {
        private readonly XRHRegistration.XRHReg licenseManager = new XRHRegistration.XRHReg();

        public void ResetCacheForLicense(SecureLicense license)
        {
            licenseManager.ResetCacheForLicense(license);
        }

        public SecureLicense Validate(LicenseValidationRequestInfo requestInfo)
        {
            return licenseManager.Validate(requestInfo);
        }
    }
}