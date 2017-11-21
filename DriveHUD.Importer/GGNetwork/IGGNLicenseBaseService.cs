//-----------------------------------------------------------------------
// <copyright file="IGGNLicenseService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.Hand;

namespace DriveHUD.Importers.GGNetwork
{
    public interface IGGNLicenseBaseService
    {
        /// <summary>
        /// Validate application license
        /// </summary>
        bool Validate();

        /// <summary>
        /// Validates if the specified <see cref="HandHistory"/> satisfies installed licenses
        /// </summary>
        /// <param name="handHistory"><see cref="HandHistory"/> to validate</param>
        /// <returns>True if valid, otherwise - false</returns>
        bool IsMatch(HandHistory handHistory);

        /// <summary>
        /// If any of license is registered
        /// </summary>
        bool IsRegistered { get; }
    }
}