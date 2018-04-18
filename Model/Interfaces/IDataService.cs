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
using System.Collections.Generic;
using System.IO;

namespace Model.Interfaces
{
    /// <summary>
    /// Exposes service to operate with stored data
    /// </summary>
    public interface IDataService
    {
        #region Notes

        Handnotes GetHandNote(long gameNumber, short pokersiteId);

        IList<Handnotes> GetHandNotes(short pokersiteId);

        IEnumerable<Playernotes> GetPlayerNotes(string playerName, short pokersiteId);

        IEnumerable<Playernotes> GetPlayerNotes(int playerId);

        void DeletePlayerNotes(IEnumerable<Playernotes> playernotes);

        void Store(Handnotes handNote);

        void Store(Playernotes playernotes);

        #endregion

        #region Imported files

        IEnumerable<ImportedFile> GetImportedFiles(IEnumerable<string> fileNames, ISession session);

        IEnumerable<ImportedFile> GetImportedFiles(IEnumerable<string> fileNames);

        #endregion 

        #region Players

        Players GetPlayer(string playerName, short pokersiteId);

        Players GetPlayer(int playerId);

        IList<Gametypes> GetPlayerGameTypes(IEnumerable<int> playerIds);
     
        IList<IPlayer> GetPlayersList();

        void AddPlayerToList(IPlayer playerItem);

        void AddPlayerRangeToList(IEnumerable<IPlayer> playerItems);

        IPlayer GetActivePlayer();

        void SaveActivePlayer(string playerName, short? pokersiteId);

        IEnumerable<PlayerNetWon> GetTopPlayersByNetWon(int top, IEnumerable<int> playersToExclude);

        #endregion

        #region HandHistory/Tournaments

        HandHistory GetGame(long gameNumber, short pokersiteId);

        IList<HandHistory> GetGames(IEnumerable<long> gameNumbers, short pokersiteId);

        Handhistory GetHandHistory(long gameNumber, short pokersiteId);

        Tournaments GetTournament(string tournamentId, string playerName, short pokersiteId);

        IList<Tournaments> GetPlayerTournaments(IEnumerable<int> playerIds);

        void DeleteHandHistory(long handNumber, int pokerSiteId);

        void DeleteTournament(string tournamentId, int pokerSiteId);

        void Store(Tournaments tournament);

        #endregion

        #region System 

        void VacuumDatabase();

        void RemoveAppData();

        Stream OpenStorageStream(string filename, FileMode mode);

        #endregion

        #region Aliases

        Aliases GetAlias(string aliasName);

        void SaveAlias(AliasCollectionItem aliasToSave);

        void RemoveAlias(AliasCollectionItem aliasToRemove);

        IList<IPlayer> GetAliasesList();

        #endregion    
    }
}