//-----------------------------------------------------------------------
// <copyright file="IImporterService.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Importers
{
    /// <summary>
    /// Importer service is service responsible for importing
    /// </summary>
    public interface IImporterService
    {
        /// <summary>
        /// Importing has been stopped
        /// </summary>
        event EventHandler ImportingStopped;

        /// <summary>
        /// Start import
        /// </summary>
        void StartImport();

        /// <summary>
        /// Stop import
        /// </summary>
        void StopImport();     
    }
}