﻿//-----------------------------------------------------------------------
// <copyright file="IDHProgress.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using System;
using System.Threading;

namespace DriveHUD.Common.Progress
{
    /// <summary>
    /// Base progress interface
    /// </summary>
    public interface IDHProgress : IProgress<ProgressItem>
    {
        /// <summary>
        /// Report progress
        /// </summary>
        /// <param name="message">Progress message</param>
        /// <param name="progressValue">Progress value</param>
        void Report(ILocalizableString message, decimal progressValue);

        /// <summary>
        /// Report progress
        /// </summary>
        /// <param name="message">Progress message</param>
        void Report(ILocalizableString message);

        CancellationToken CancellationToken { get; }

        void Reset(CancellationToken cancellationToken);
    }
}