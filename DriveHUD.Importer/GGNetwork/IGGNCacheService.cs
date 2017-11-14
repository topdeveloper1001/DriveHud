//-----------------------------------------------------------------------
// <copyright file="IGGNCacheService.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.GGNetwork.Model;

namespace DriveHUD.Importers.GGNetwork
{
    /// <summary>
    /// Exposes GGN Cache service
    /// </summary>
    internal interface IGGNCacheService
    {
        /// <summary>
        /// Refreshes cache data
        /// </summary>
        void Refresh();

        /// <summary>
        /// Gets <see cref="TournamentInformation"/> for the specified tournament id
        /// </summary>
        /// <param name="tournamentId">Tournament id</param>
        /// <returns><see cref="TournamentInformation"/> if tournament is in cache, otherwise null</returns>
        TournamentInformation GetTournament(string tournamentId);

        /// <summary>
        /// Clears cache data
        /// </summary>
        void Clear();
    }
}