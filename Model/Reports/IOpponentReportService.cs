//-----------------------------------------------------------------------
// <copyright file="IOpponentReportService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model.Data;
using System.Collections.Generic;
using System.Threading;

namespace Model.Reports
{
    /// <summary>
    /// Exposes service to read/update/cache data for opponent report
    /// </summary>
    public interface IOpponentReportService
    {
        /// <summary>
        /// Gets the opponent report 
        /// </summary>
        /// <returns>The list of <see cref="Indicators"/></returns>
        IEnumerable<ReportIndicators> GetReport(CancellationToken cancellationToken);

        /// <summary>
        /// Updates stats in the opponents report
        /// </summary>
        /// <param name="stat">Stat to add to the report</param>
        void UpdateReport(Playerstatistic stat);

        /// <summary>
        /// Checks whenever player is presented in the report
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <returns>True is player exists, otherwise - false</returns>
        bool IsPlayerInReport(int playerId);

        /// <summary>
        /// Flush cached data to the file
        /// </summary>
        void Flush();

        /// <summary>
        /// Resets cached data
        /// </summary>
        void Reset();

        /// <summary>
        /// Loads only those hands in which hero was involved for the specified player
        /// </summary>
        /// <param name="playerId">Player to load hands</param>
        /// <param name="count">Amount of hands to load</param>
        /// <returns>Collection of hands</returns>
        IEnumerable<Playerstatistic> LoadPlayerHands(int playerId, int count);
    }
}