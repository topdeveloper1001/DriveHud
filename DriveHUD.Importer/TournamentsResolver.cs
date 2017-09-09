//-----------------------------------------------------------------------
// <copyright file="TournamentsResolver.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.GameDescription;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Class to get some tournament data based on specific tournaments parameters
    /// </summary>
    internal class TournamentsResolver
    {
        /// <summary>
        /// Get initial stack size based on tournament name
        /// </summary>
        /// <param name="tournamentName">Tournament name</param>
        /// <returns>Stack size</returns>
        public static int GetInitialStackSizeByName(string tournamentName)
        {
            var stackSize = 0;

            if (string.IsNullOrEmpty(tournamentName))
            {
                return stackSize;
            }

            return stackSize;
        }

        /// <summary>
        /// Get possible tournament initial stacks
        /// </summary>
        /// <returns>List of possible stacks</returns>
        public static decimal[] GetPossibleInitialeStacksSizes()
        {
            return new decimal[] { 250m, 300m, 500m, 800m, 1000m, 1500m, 2000m, 3000m, 5000m, 10000m, 30000m, 50000m };
        }

        /// <summary>
        /// Gets predefined tournament prizes data
        /// </summary>
        /// <returns>tournament prizes data or null if there are no such predefined tournament</returns>
        public static TournamentResultModel GetPredefinedTournament(string tournamentName, int buyinInCents, int totalPlayers, Currency currency)
        {
            var tournaments = TournamentResultModel.GetPredefinedTournaments();

            return tournaments.FirstOrDefault(x => x.Name == tournamentName
                                        && x.BuyinInCents == buyinInCents
                                        && x.TotalPlayers == totalPlayers
                                        && x.Currency == currency);
        }
    }
}