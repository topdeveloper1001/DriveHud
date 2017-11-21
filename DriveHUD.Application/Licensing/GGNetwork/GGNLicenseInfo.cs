﻿//-----------------------------------------------------------------------
// <copyright file="GGNLicenseInfo.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Utils;
using System;
using System.Linq;

namespace DriveHUD.Application.Licensing.GGNetwork
{
    /// <summary>
    /// License information class
    /// </summary>
    internal class GGNLicenseInfo : IGGNLicenseInfo
    {
        private readonly SecureLicense license;

        private readonly GGNLicenseType licenseType;

        public GGNLicenseInfo(SecureLicense license, GGNLicenseType licenseType)
        {
            this.license = license;
            this.licenseType = licenseType;

            if (IsTrial)
            {
                var timeMonitor = license.GetTimeMonitor();
                ExpiryDate = DateTime.Now + timeMonitor.TimeRemaining;
            }
        }

        public GGNLicenseType LicenseType
        {
            get
            {
                return licenseType;
            }
        }

        public SecureLicense License
        {
            get
            {
                return license;
            }
        }

        public bool IsRegistered
        {
            get
            {
                return license != null && !license.IsTrial;
            }
        }

        public bool IsTrial
        {
            get
            {
                return license != null && license.SerialNumber != null && (license.SerialNumber.StartsWith("GGNT", StringComparison.InvariantCulture) || licenseType == GGNLicenseType.GGNTrial);
            }
        }

        public bool IsTrialExpired
        {
            get
            {
                var hasExpiredErrorCode = ValidationException != null ?
                    ValidationException.Message.Contains("LCS_EXP") || Utils.GetErrorCodes(ValidationException).Any(x => x.Equals("E_TimeExpired")) :
                    false;

                return license != null && license.IsTrial && hasExpiredErrorCode;
            }
        }

        private string serial = string.Empty;

        public string Serial
        {
            get
            {
                return license != null ? license.SerialNumber : serial;
            }
            set
            {
                serial = value;
            }
        }

        public TimeSpan TimeRemaining
        {
            get
            {
                var currentTime = DateTime.Now;
                var timeRemaining = ExpiryDate > currentTime ? ExpiryDate - currentTime : TimeSpan.Zero;
                return timeRemaining;
            }
        }

        public DateTime ExpiryDate
        {
            get;
            set;
        }

        public bool IsExpiringSoon
        {
            get
            {
                return IsRegistered && !IsTrial && TimeRemaining < TimeSpan.FromDays(30);
            }
        }

        public bool IsExpired
        {
            get;
            set;
        }

        public NoLicenseException ValidationException
        {
            get;
            set;
        }     
    }
}