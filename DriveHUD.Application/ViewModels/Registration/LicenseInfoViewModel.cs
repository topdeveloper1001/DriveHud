﻿//-----------------------------------------------------------------------
// <copyright file="LicenseInfoViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common;
using DriveHUD.Common.Wpf.Mvvm;

namespace DriveHUD.Application.ViewModels.Registration
{
    public class LicenseInfoViewModel : ViewModelBase
    {
        private ILicenseInfo licenseInfo;

        internal LicenseInfoViewModel(ILicenseInfo licenseInfo)
        {
            Check.ArgumentNotNull(() => licenseInfo);

            this.licenseInfo = licenseInfo;
        }

        public string Serial
        {
            get
            {
                return licenseInfo.Serial;
            }
        }

        public int DaysLeft
        {
            get
            {
                return licenseInfo.TimeRemaining.Days;
            }
        }
    }
}