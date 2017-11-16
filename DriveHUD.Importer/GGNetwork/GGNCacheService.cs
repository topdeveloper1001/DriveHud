//-----------------------------------------------------------------------
// <copyright file="GGNCacheService.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Importers.GGNetwork.Model;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.Importers.GGNetwork
{
    internal class GGNCacheService : IGGNCacheService
    {
        private readonly Dictionary<string, TournamentInformation> tournamentsCacheInfo = new Dictionary<string, TournamentInformation>();

        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Refreshes cache data
        /// </summary>
        public async Task RefreshAsync()
        {
            try
            {
                var tournamentsReader = ServiceLocator.Current.GetInstance<IGGNTournamentReader>();

                var tournaments = await tournamentsReader.ReadAllTournamentsAsync();

                rwLock.EnterWriteLock();

                try
                {
                    foreach (var tournament in tournaments)
                    {
                        if (!tournamentsCacheInfo.ContainsKey(tournament.Id))
                        {
                            tournamentsCacheInfo.Add(tournament.Id, tournament);
                            continue;
                        }

                        tournamentsCacheInfo[tournament.Id] = tournament;
                    }
                }
                finally
                {
                    rwLock.ExitWriteLock();
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could refresh GGN cache data.", e);
            }
        }

        /// <summary>
        /// Gets <see cref="TournamentInformation"/> for the specified tournament id
        /// </summary>
        /// <param name="tournamentId">Tournament id</param>
        /// <returns><see cref="TournamentInformation"/> if tournament is in cache, otherwise null</returns>
        public TournamentInformation GetTournament(string tournamentId)
        {
            if (string.IsNullOrEmpty(tournamentId))
            {
                return null;
            }

            rwLock.EnterReadLock();

            try
            {
                if (tournamentsCacheInfo.ContainsKey(tournamentId))
                {
                    return tournamentsCacheInfo[tournamentId];
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not read GGN tournament info from cache data.", e);
            }
            finally
            {
                rwLock.ExitReadLock();
            }

            return null;
        }

        /// <summary>
        /// Clears cache data
        /// </summary>
        public void Clear()
        {
            rwLock.EnterWriteLock();

            try
            {
                tournamentsCacheInfo.Clear();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not clear GGN cache data.", e);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }
    }
}