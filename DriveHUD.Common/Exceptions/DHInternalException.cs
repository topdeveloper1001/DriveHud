//-----------------------------------------------------------------------
// <copyright file="DHInternalException.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Common.Exceptions
{
    /// <summary>
    /// Class for internal DH exceptions
    /// </summary>
    public class DHInternalException : LocalizableException
    {
        public DHInternalException(ILocalizableString message)
            : base(message, null)
        {
        }

        public DHInternalException(ILocalizableString message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}