//-----------------------------------------------------------------------
// <copyright file="ILicenseService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;

namespace DriveHUD.PlayerXRay.Licensing
{
    /// <summary>
    /// Interface of Licensing service
    /// </summary>
    internal interface ILicenseService
    {
        /// <summary>
        /// Validate application license
        /// </summary>
        bool Validate();

        /// <summary>
        /// Register with specified serial number
        /// </summary>
        /// <param name="serial">Serial number</param>
        bool Register(string serial, string email);

        /// <summary>
        /// Encrypts the specified email
        /// </summary>
        /// <param name="email">Email to encrypt</param>
        /// <returns>Encrypted email</returns>
        string EncryptEmail(string email);

        /// <summary>
        /// Current licenses information
        /// </summary>
        IEnumerable<ILicenseInfo> LicenseInfos { get; }

        /// <summary>
        /// If any of license is registered
        /// </summary>
        bool IsRegistered { get; }

        /// <summary>
        /// Only one license is registered and it's a trial
        /// </summary>
        bool IsTrial { get; }

        /// <summary>
        /// Trial expired
        /// </summary>
        bool IsTrialExpired { get; }

        /// <summary>
        /// True if one of existing licenses is expiring soon
        /// </summary>
        bool IsExpiringSoon { get; }

        /// <summary>
        /// Determine when any of licenses is expired
        /// </summary>
        bool IsExpired { get; }

        /// <summary>
        /// Determines if user can upgrade his license
        /// </summary>
        bool IsUpgradable { get; }

        /// <summary>
        /// Gets the type of license from serial number
        /// </summary>
        /// <param name="serial">Serial number</param>
        /// <returns>The type of the license</returns>
        LicenseType? GetTypeFromSerial(string serial);
    }
}