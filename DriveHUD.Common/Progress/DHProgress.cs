//-----------------------------------------------------------------------
// <copyright file="DHProgress.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using DriveHUD.Common.Resources;

namespace DriveHUD.Common.Progress
{
    public class DHProgress : Progress<ProgressItem>, IDHProgress
    {
        public virtual void Report(ILocalizableString message)
        {
            Report(message, 0);
        }

        public virtual void Report(ILocalizableString message, decimal progressValue)
        {
            var progressItem = new ProgressItem
            {
                Message = message,
                ProgressValue = progressValue
            };

            OnReport(progressItem);
        }
    }
}