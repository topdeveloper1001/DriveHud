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
using Model.Data;
using System.Collections.Generic;
using System.IO;

namespace Model.Interfaces
{
    public interface IDataService
    {
        void SetPlayerStatisticPath(string path);

        IList<Playerstatistic> GetPlayerStatisticFromFile(int playerId, short? pokersiteId);

        IList<Playerstatistic> GetPlayerStatisticFromFile(string playerName, short? pokersiteId);

        Indicators GetPlayerIndicator(int playerId, short pokersiteId);

        IList<HandHistoryRecord> GetHandHistoryRecords();

        IList<HandHistoryRecord> GetPlayerHandRecords(string playerName, short pokersiteId);

        Players GetPlayer(string playerName, short pokersiteId);

        IList<Gametypes> GetPlayerGameTypes(string playerName, short pokersiteId);

        IList<Tournaments> GetPlayerTournaments(string playerName, short pokersiteId);

        IList<Tournaments> GetPlayerTournaments(IEnumerable<int> playerIds);

        Tournaments GetTournament(string tournamentId, string playerName, short pokersiteId);

        HandHistory GetGame(long gameNumber, short pokersiteId);

        IList<HandHistory> GetGames(IEnumerable<long> gameNumbers, short pokersiteId);

        Handnotes GetHandNote(long gameNumber, short pokersiteId);

        Playernotes GetPlayerNote(string playerName, short pokersiteId);

        IList<Handnotes> GetHandNotes(IEnumerable<long> gameNumbers, short pokersiteId);

        IList<Handnotes> GetHandNotes(short pokersiteId);

        Handhistory GetHandHistory(long gameNumber, short pokersiteId);

        void Purge();

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

        IPlayer GetActivePlayer();

        void SaveActivePlayer(string playerName, short? pokersiteId);

        #region Aliases

        Aliases GetAlias(string aliasName);

        void SaveAlias(AliasCollectionItem aliasToSave);

        void RemoveAlias(AliasCollectionItem aliasToRemove);

        IList<IPlayer> GetAliasesList();

        #endregion
    }
}