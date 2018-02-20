//-----------------------------------------------------------------------
// <copyright file="IDataService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using HandHistories.Objects.Hand;
using NHibernate;
using System;
using System.Collections.Generic;
using System.IO;

namespace Model.Interfaces
{
    public interface IDataService
    {
        void SetPlayerStatisticPath(string path);

        IList<Playerstatistic> GetPlayerStatisticFromFile(int playerId, short? pokersiteId);

        IList<Playerstatistic> GetPlayerStatisticFromFile(int playerId, Func<Playerstatistic, bool> filter);

        IList<Playerstatistic> GetPlayerStatisticFromFile(string playerName, short? pokersiteId);

        void ActOnPlayerStatisticFromFile(int playerId, Func<Playerstatistic, bool> predicate, Action<Playerstatistic> action);

        void ActOnPlayerStatisticFromFile(string playerName, short? pokerSiteId, Func<Playerstatistic, bool> predicate, Action<Playerstatistic> action);

        /// <summary>
        /// Executes action on each player statistic in the specified file if it matches the specified predicate. 
        /// <para>*This method is not thread-safe.*</para>        
        /// </summary>
        /// <param name="file">File with stats</param>
        /// <param name="predicate">Predicate to filter data</param>
        /// <param name="action">Action to execute on each statistic</param>
        void ActOnPlayerStatisticFromFile(string file, Func<Playerstatistic, bool> predicate, Action<Playerstatistic> action);

        Players GetPlayer(string playerName, short pokersiteId);

        IList<Gametypes> GetPlayerGameTypes(IEnumerable<int> playerIds);

        IList<Tournaments> GetPlayerTournaments(IEnumerable<int> playerIds);

        Tournaments GetTournament(string tournamentId, string playerName, short pokersiteId);

        HandHistory GetGame(long gameNumber, short pokersiteId);

        IList<HandHistory> GetGames(IEnumerable<long> gameNumbers, short pokersiteId);

        Handnotes GetHandNote(long gameNumber, short pokersiteId);

        IEnumerable<Playernotes> GetPlayerNotes(string playerName, short pokersiteId);

        IEnumerable<Playernotes> GetPlayerNotes(int playerId);

        void DeletePlayerNotes(IEnumerable<Playernotes> playernotes);

        IList<Handnotes> GetHandNotes(short pokersiteId);

        Handhistory GetHandHistory(long gameNumber, short pokersiteId);

        IEnumerable<ImportedFile> GetImportedFiles(IEnumerable<string> fileNames, ISession session);

        IEnumerable<ImportedFile> GetImportedFiles(IEnumerable<string> fileNames);

        void Store(Playerstatistic statistic);

        void Store(IEnumerable<Playerstatistic> statistic);

        void Store(Handnotes handNote);

        void Store(Playernotes playernotes);

        void Store(Tournaments tournament);

        Stream OpenStorageStream(string filename, FileMode mode);

        IList<IPlayer> GetPlayersList();

        void AddPlayerToList(IPlayer playerItem);

        void AddPlayerRangeToList(IEnumerable<IPlayer> playerItems);

        void RemoveAppData();

        /// <summary>
        /// Deletes specific player's statistic from file storage
        /// </summary>
        /// <param name="statistic">Statistic to delete</param>
        void DeletePlayerStatisticFromFile(Playerstatistic statistic);

        void DeleteHandHistory(long handNumber, int pokerSiteId);

        IPlayer GetActivePlayer();

        void SaveActivePlayer(string playerName, short? pokersiteId);

        void VacuumDatabase();

        IEnumerable<PlayerNetWon> GetTopPlayersByNetWon(int top, IEnumerable<int> playersToExclude);

        #region Aliases

        Aliases GetAlias(string aliasName);

        void SaveAlias(AliasCollectionItem aliasToSave);

        void RemoveAlias(AliasCollectionItem aliasToRemove);

        IList<IPlayer> GetAliasesList();

        #endregion

        string[] GetPlayerFiles(int playerId);
    }
}