using System.Collections.Generic;
using System.IO;
using HandHistories.Objects.Hand;
using Model.Data;
using DriveHUD.Entities;

namespace Model.Interfaces
{
    public interface IDataService
    {
        IList<Playerstatistic> GetPlayerStatistic(string playerName, short pokersiteId);

        IList<Playerstatistic> GetPlayerStatisticFromFile(string playerName, short? pokersiteId);

        Indicators GetPlayerIndicator(string playerName, short pokersiteId);
        IList<HandHistoryRecord> GetHandHistoryRecords();
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