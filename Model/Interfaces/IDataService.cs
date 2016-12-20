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

using System.Collections.Generic;
using System.IO;
using HandHistories.Objects.Hand;
using Model.Data;
using DriveHUD.Entities;

namespace Model.Interfaces
{
    public interface IDataService
    {        
        IList<Playerstatistic> GetPlayerStatisticFromFile(int playerId, short? pokersiteId);

        IList<Playerstatistic> GetPlayerStatisticFromFile(string playerName, short? pokersiteId);

        Indicators GetPlayerIndicator(int playerId, short pokersiteId);

        IList<HandHistoryRecord> GetPlayerHandRecords(string playerName, short pokersiteId);

        Players GetPlayer(string playerName, short pokersiteId);

        IList<Gametypes> GetPlayerGameTypes(string playerName, short pokersiteId);

        IList<Tournaments> GetPlayerTournaments(string playerName, short pokersiteId);

        Tournaments GetTournament(string tournamentId, string playerName, short pokersiteId);

        HandHistory GetGame(long gameNumber, short pokersiteId);

        Handnotes GetHandNote(long gameNumber, short pokersiteId);

        Playernotes GetPlayerNote(string playerName, short pokersiteId);

        IList<Handnotes> GetHandNotes(IEnumerable<long> gameNumbers, short pokersiteId);

        IList<Handnotes> GetHandNotes(short pokersiteId);

        Handhistory GetHandHistory(long gameNumber, short pokersiteId);

        void Purge();

        void Store(Playerstatistic statistic);

        void Store(Handnotes handNote);

        void Store(Playernotes playernotes);

        void Store(Tournaments tournament);

        Stream OpenStorageStream(string filename, FileMode mode);

        IList<PlayerCollectionItem> GetPlayersList();

        void AddPlayerToList(PlayerCollectionItem playerItem);

        void AddPlayerRangeToList(IEnumerable<PlayerCollectionItem> playerItems);

        void RemoveAppData();

        /// <summary>
        /// Deletes specific player's statistic from file storage
        /// </summary>
        /// <param name="statistic">Statistic to delete</param>
        void DeletePlayerStatisticFromFile(Playerstatistic statistic);

        PlayerCollectionItem GetActivePlayer();

        void SaveActivePlayer(string playerName, short pokersiteId);

    }
}
