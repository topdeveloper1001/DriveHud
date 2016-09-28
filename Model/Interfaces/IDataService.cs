using System.Collections.Generic;
using System.IO;
using HandHistories.Objects.Hand;
using Model.Data;
using DriveHUD.Entities;

namespace Model.Interfaces
{
    public interface IDataService
    {
        IList<Playerstatistic> GetPlayerStatistic(string playerName);

        IList<Playerstatistic> GetPlayerStatisticFromFile(string playerName);

        Indicators GetPlayerIndicator(string playerName);

        IList<HandHistoryRecord> GetPlayerHandRecords(string playerName);

        Players GetPlayer(string playerName, short pokersiteId);

        IList<Gametypes> GetPlayerGameTypes(string playerName);

        IList<Tournaments> GetPlayerTournaments(string playerName);

        Tournaments GetTournament(string tournamentId, string playerName);

        HandHistory GetGame(long gameNumber);

        Handnotes GetHandNote(long gameNumber);

        Playernotes GetPlayerNote(string playerName, short pokersiteId);

        IList<Handnotes> GetHandNotes(IEnumerable<long> gameNumbers);

        IList<Handnotes> GetHandNotes();

        Handhistory GetHandHistory(long gameNumber);

        void Purge();

        void Store(Playerstatistic statistic);

        void Store(Handnotes handNote);

        void Store(Playernotes playernotes);

        void Store(Tournaments tournament);

        Stream OpenStorageStream(string filename,FileMode mode);

        IList<string> GetPlayersList();

        void RemoveAppData();

        /// <summary>
        /// Deletes specific player's statistic from file storage
        /// </summary>
        /// <param name="statistic">Statistic to delete</param>
        void DeletePlayerStatisticFromFile(Playerstatistic statistic);

        string GetActivePlayer();

        void SaveActivePlayer(string playerName);

    }
}
