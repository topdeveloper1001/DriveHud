//-----------------------------------------------------------------------
// <copyright file="DataService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Model.Interfaces;
using Model.Reports;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace Model
{
    /// <summary>
    /// The data service.
    /// </summary>
    public class DataService : IDataService
    {
        protected readonly string dataPath = StringFormatter.GetAppDataFolderPath();

        protected virtual string PlayersPath
        {
            get;
            set;
        }

        private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public DataService()
        {
            PlayersPath = StringFormatter.GetPlayerStatisticDataFolderPath();

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            if (!Directory.Exists(PlayersPath))
            {
                Directory.CreateDirectory(PlayersPath);
            }
        }

        public void SetPlayerStatisticPath(string path)
        {
            PlayersPath = path;

            if (!Directory.Exists(PlayersPath))
            {
                Directory.CreateDirectory(PlayersPath);
            }
        }

        public IList<Playerstatistic> GetPlayerStatisticFromFile(int playerId, short? pokersiteId)
        {
            var result = new List<Playerstatistic>();

            ActOnPlayerStatisticFromFile(playerId,
                stat => !pokersiteId.HasValue || (stat.PokersiteId == pokersiteId),
                stat => result.Add(stat));

            return result;
        }

        public IList<Playerstatistic> GetPlayerStatisticFromFile(int playerId, Func<Playerstatistic, bool> filter)
        {
            var result = new List<Playerstatistic>();

            ActOnPlayerStatisticFromFile(playerId, filter, stat => result.Add(stat));

            return result;
        }

        public IList<Playerstatistic> GetPlayerStatisticFromFile(string playerName, short? pokersiteId)
        {
            try
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var player = session.Query<Players>().FirstOrDefault(x => x.Playername.Equals(playerName) && x.PokersiteId == pokersiteId);

                    if (player == null)
                    {
                        return new List<Playerstatistic>();
                    }

                    return GetPlayerStatisticFromFile(player.PlayerId, pokersiteId);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't get player", e);
            }

            return new List<Playerstatistic>();
        }

        /// <summary>
        /// Reads player statistic for the specified player name, invoke action for the filtered by predicate statistic 
        /// </summary>      
        public void ActOnPlayerStatisticFromFile(string playerName, short? pokerSiteId, Func<Playerstatistic, bool> predicate, Action<Playerstatistic> action)
        {
            try
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var player = session.Query<Players>().FirstOrDefault(x => x.Playername.Equals(playerName) && x.PokersiteId == pokerSiteId);

                    if (player == null)
                    {
                        return;
                    }

                    ActOnPlayerStatisticFromFile(player.PlayerId, predicate, action);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't get player", e);
            }
        }

        /// <summary>
        /// Reads player statistic for the specified player id, invoke action for the filtered by predicate statistic 
        /// </summary>      
        public void ActOnPlayerStatisticFromFile(int playerId, Func<Playerstatistic, bool> predicate, Action<Playerstatistic> action)
        {
            if (action == null)
            {
                return;
            }

            var files = GetPlayerFiles(playerId);

            if (files == null || !files.Any())
            {
                return;
            }

            rwLock.EnterReadLock();

            try
            {
                foreach (var file in files)
                {
                    ActOnPlayerStatisticFromFile(file, predicate, action);
                }
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public void ActOnPlayerStatisticFromFile(string file, Func<Playerstatistic, bool> predicate, Action<Playerstatistic> action)
        {
            try
            {
                using (var sr = new StreamReader(file))
                {
                    string line = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                LogProvider.Log.Warn(this, $"Empty line in {file}");
                                continue;
                            }

                            /* replace '-' and '_' characters in order to convert back from Modified Base64 (https://en.wikipedia.org/wiki/Base64#Implementations_and_history) */
                            byte[] byteAfter64 = Convert.FromBase64String(line.Replace('-', '+').Replace('_', '/').Trim());

                            using (var ms = new MemoryStream(byteAfter64))
                            {
                                var stat = Serializer.Deserialize<Playerstatistic>(ms);

                                if (predicate == null || predicate(stat))
                                {
                                    action(stat);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogProvider.Log.Error($@"Could not process the file: {file}{Environment.NewLine}Error at line: {line}{Environment.NewLine}", ex);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"File '{file}' has not been processed.", e);
                return;
            }
        }

        public Players GetPlayer(string playerName, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Players>().FirstOrDefault(x => x.Playername == playerName && x.PokersiteId == pokersiteId);
            }
        }

        #region Aliases

        public Aliases GetAlias(string aliasName)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Aliases>().FirstOrDefault(x => x.AliasName == aliasName);
            }
        }

        public void SaveAlias(AliasCollectionItem aliasToSave)
        {
            using (var session = ModelEntities.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var playersIds = aliasToSave.PlayersInAlias.Select(x => x.PlayerId).ToArray();

                    var players = session.Query<Players>().Where(x => playersIds.Contains(x.PlayerId)).ToArray();

                    var alias = new Aliases()
                    {
                        AliasId = aliasToSave.PlayerId,
                        AliasName = aliasToSave.Name,
                        Players = players
                    };

                    session.SaveOrUpdate(alias);

                    transaction.Commit();

                    if (aliasToSave.PlayerId == 0)
                    {
                        aliasToSave.PlayerId = alias.AliasId;
                    }
                }
            }
        }

        public void RemoveAlias(AliasCollectionItem aliasToRemove)
        {
            using (var session = ModelEntities.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var aliasEntity = session.Load<Aliases>(aliasToRemove.PlayerId);

                    session.Delete(aliasEntity);

                    transaction.Commit();
                }
            }
        }

        #endregion

        public IList<Gametypes> GetPlayerGameTypes(IEnumerable<int> playerIds)
        {
            using (var session = ModelEntities.OpenStatelessSession())
            {
                return session.Query<PlayerGameInfo>()
                    .Where(x => playerIds.Contains(x.PlayerId))
                    .Select(x => x.GameInfo)
                    .Distinct()
                    .ToList();
            }
        }

        /// <summary>
        /// Gets tournaments list for the specified ids of players 
        /// </summary>
        /// <param name="playerIds"></param>
        /// <returns></returns>
        public IList<Tournaments> GetPlayerTournaments(IEnumerable<int> playerIds)
        {
            if (playerIds.IsNullOrEmpty())
            {
                return new List<Tournaments>();
            }

            try
            {
                using (var session = ModelEntities.OpenSession())
                {
                    return session.Query<Tournaments>().Where(x => playerIds.Contains(x.Player.PlayerId)).Fetch(x => x.Player).ToList();
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Could not read tournaments", ex);
            }

            return new List<Tournaments>();
        }

        public Tournaments GetTournament(string tournamentId, string playerName, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Tournaments>().FirstOrDefault(x => x.Tourneynumber == tournamentId && x.Player.Playername == playerName && x.SiteId == pokersiteId);
            }
        }

        public HandHistory GetGame(long gameNumber, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                var hh = session.Query<Handhistory>().FirstOrDefault(x => x.Gamenumber == gameNumber && x.PokersiteId == pokersiteId);

                if (hh == null)
                {
                    return null;
                }

                var handHistoryParserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();

                var handHistoryParser = handHistoryParserFactory.GetFullHandHistoryParser((EnumPokerSites)pokersiteId, hh.HandhistoryVal);

                var result = handHistoryParser.ParseFullHandHistory(hh.HandhistoryVal);

                if (result == null)
                {
                    return null;
                }

                return result;
            }
        }

        public IList<HandHistory> GetGames(IEnumerable<long> gameNumbers, short pokersiteId)
        {
            var handHistoryParserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();

            var handHistoryParser = handHistoryParserFactory.GetFullHandHistoryParser((EnumPokerSites)pokersiteId);

            using (var session = ModelEntities.OpenSession())
            {
                List<HandHistory> historyList = new List<HandHistory>();

                Disjunction restriction = Restrictions.Disjunction();
                restriction.Add(Restrictions.Conjunction()
                     .Add(Restrictions.On<Handhistory>(x => x.Gamenumber).IsIn(gameNumbers.ToList()))
                     .Add(Restrictions.Where<Handhistory>(x => x.PokersiteId == pokersiteId)));

                var list = session.QueryOver<Handhistory>()
                    .Where(restriction)
                    .List();

                foreach (var history in list)
                {
                    var result = handHistoryParser.ParseFullHandHistory(history.HandhistoryVal);

                    if (result == null)
                    {
                        continue;
                    }

                    historyList.Add(result);
                }

                return historyList;
            }
        }

        public IEnumerable<Playernotes> GetPlayerNotes(string playerName, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                var playerNotes = session.Query<Playernotes>()
                    .Where(x => x.Player.Playername == playerName && x.PokersiteId == pokersiteId)
                    .ToArray();

                return playerNotes;
            }
        }

        public IEnumerable<Playernotes> GetPlayerNotes(int playerId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                var playerNotes = session.Query<Playernotes>()
                    .Where(x => x.PlayerId == playerId)
                    .ToArray();

                return playerNotes;
            }
        }

        public void DeletePlayerNotes(IEnumerable<Playernotes> playernotes)
        {
            if (playernotes == null || playernotes.Count() == 0)
            {
                return;
            }

            try
            {
                using (var session = ModelEntities.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var playernotesIds = playernotes.Select(x => x.PlayerNoteId).Distinct().ToArray();

                        var playernotesToDelete = session.Query<Playernotes>()
                             .Where(x => playernotesIds.Contains(x.PlayerNoteId))
                             .ToArray();

                        playernotesToDelete.ForEach(x => session.Delete(x));

                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not delete player notes", e);
            }
        }

        public Handnotes GetHandNote(long gameNumber, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                var hn = session.Query<Handnotes>().FirstOrDefault(x => x.Gamenumber == gameNumber && x.PokersiteId == pokersiteId);

                return hn;
            }
        }

        public IList<Handnotes> GetHandNotes(short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Handnotes>().Where(x => x.PokersiteId == pokersiteId).ToList();
            }
        }

        public Handhistory GetHandHistory(long gameNumber, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                var hh = session.Query<Handhistory>().FirstOrDefault(x => x.Gamenumber == gameNumber && x.PokersiteId == pokersiteId);

                return hh ?? null;
            }
        }

        public IEnumerable<ImportedFile> GetImportedFiles(IEnumerable<string> fileNames, ISession session)
        {
            Check.Require(fileNames != null, "fileNames must be not null");
            Check.Require(session != null, "session must be not null");

            var importedFiles = new List<ImportedFile>();

            var importedFilesPerQuery = 100;

            var importedFilesCount = fileNames.Count();

            var numOfQueries = (int)Math.Ceiling((double)importedFilesCount / importedFilesPerQuery);

            for (var i = 0; i < numOfQueries; i++)
            {
                var numOfRowToStartQuery = i * importedFilesPerQuery;

                var tempFileNames = fileNames.Skip(numOfRowToStartQuery).Take(importedFilesPerQuery);

                var tempImportedFiles = session.Query<ImportedFile>()
                    .Where(x => tempFileNames.Contains(x.FileName))
                    .ToList();

                importedFiles.AddRange(tempImportedFiles);
            }

            return importedFiles;
        }

        public IEnumerable<ImportedFile> GetImportedFiles(IEnumerable<string> fileNames)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return GetImportedFiles(fileNames, session);
            }
        }

        public void Store(Handnotes handnotes)
        {
            try
            {
                using (var session = ModelEntities.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var existingHandNote = session
                            .Query<Handnotes>()
                            .FirstOrDefault(x => x.PokersiteId == handnotes.PokersiteId && x.Gamenumber == handnotes.Gamenumber);

                        if (existingHandNote != null)
                        {
                            existingHandNote.HandTag = handnotes.HandTag;
                            existingHandNote.Note = handnotes.Note;
                        }
                        else
                        {
                            existingHandNote = handnotes;
                        }

                        session.SaveOrUpdate(existingHandNote);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't save hand notes", e);
            }
        }

        public void Store(Playernotes playernotes)
        {
            try
            {
                using (var session = ModelEntities.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(playernotes);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't save player notes", e);
            }
        }

        public void Store(Tournaments tournament)
        {
            try
            {
                using (var session = ModelEntities.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(tournament);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't save tournament", e);
            }
        }

        public void Store(Playerstatistic statistic)
        {
            rwLock.EnterWriteLock();

            try
            {
                if (!Directory.Exists(PlayersPath))
                {
                    Directory.CreateDirectory(PlayersPath);
                }

                var playerDirectory = Path.Combine(PlayersPath, statistic.PlayerId.ToString());

                if (!Directory.Exists(playerDirectory))
                {
                    Directory.CreateDirectory(playerDirectory);
                }

                var fileName = Path.Combine(playerDirectory, statistic.Playedyearandmonth.ToString()) + ".stat";

                var data = string.Empty;

                using (var msTestString = new MemoryStream())
                {
                    Serializer.Serialize(msTestString, statistic);
                    data = Convert.ToBase64String(msTestString.ToArray()).Trim();
                }

                File.AppendAllLines(fileName, new[] { data });

                var storageModel = ServiceLocator.Current.TryResolve<SingletonStorageModel>();

                if (storageModel.PlayerSelectedItem != null &&
                    (statistic.PlayerId == storageModel.PlayerSelectedItem.PlayerId || storageModel.PlayerSelectedItem.PlayerIds.Contains(statistic.PlayerId)))
                {
                    storageModel.StatisticCollection.Add(statistic);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public void Store(IEnumerable<Playerstatistic> statistic)
        {
            if (statistic == null || !statistic.Any())
            {
                return;
            }

            var groupedStatistic = (from stat in statistic
                                    group stat by new { stat.PlayerId, stat.Playedyearandmonth } into grouped
                                    select new
                                    {
                                        grouped.Key.PlayerId,
                                        grouped.Key.Playedyearandmonth,
                                        Statistic = grouped.OrderBy(x => x.Playedyearandmonth).ToArray()
                                    }).ToArray();

            rwLock.EnterWriteLock();

            try
            {
                if (!Directory.Exists(PlayersPath))
                {
                    Directory.CreateDirectory(PlayersPath);
                }

                foreach (var stats in groupedStatistic)
                {
                    var playerDirectory = Path.Combine(PlayersPath, stats.PlayerId.ToString());

                    if (!Directory.Exists(playerDirectory))
                    {
                        Directory.CreateDirectory(playerDirectory);
                    }

                    var fileName = Path.Combine(playerDirectory, stats.Playedyearandmonth.ToString()) + ".stat";

                    var statisticStringsToAppend = new List<string>();

                    foreach (var stat in stats.Statistic)
                    {
                        using (var msTestString = new MemoryStream())
                        {
                            Serializer.Serialize(msTestString, stat);
                            var data = Convert.ToBase64String(msTestString.ToArray()).Trim();

                            statisticStringsToAppend.Add(data);
                        }
                    }

                    File.AppendAllLines(fileName, statisticStringsToAppend);

                    var storageModel = ServiceLocator.Current.TryResolve<SingletonStorageModel>();

                    if (storageModel.PlayerSelectedItem != null && stats.PlayerId == storageModel.PlayerSelectedItem.PlayerId)
                    {
                        storageModel.StatisticCollection.AddRange(stats.Statistic);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't save player statistic", e);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public Stream OpenStorageStream(string filename, FileMode mode)
        {
            if (File.Exists(Path.Combine(dataPath, filename)))
            {
                return File.Open(Path.Combine(dataPath, filename), mode);
            }
            else
            {
                if (mode == FileMode.Create)
                {
                    return File.Open(Path.Combine(dataPath, filename), mode);
                }
                else
                {
                    return null;
                }
            }
        }

        #region workaround for players collection (need to organize it better)

        private List<IPlayer> playerInternalCollection = null;
        private List<IPlayer> aliasesInternalCollection = null;

        private ReaderWriterLockSlim playerCollectionLock = new ReaderWriterLockSlim();
        private ReaderWriterLockSlim aliasCollectionLock = new ReaderWriterLockSlim();

        public void AddPlayerToList(IPlayer playerItem)
        {
            if (playerItem == null)
            {
                return;
            }

            playerCollectionLock.EnterWriteLock();

            try
            {
                if (playerInternalCollection == null)
                {
                    playerInternalCollection = new List<IPlayer>();
                }

                playerInternalCollection.Add(playerItem);
            }
            finally
            {
                playerCollectionLock.ExitWriteLock();
            }
        }

        public void AddPlayerRangeToList(IEnumerable<IPlayer> playerItems)
        {
            if (playerItems == null)
            {
                return;
            }

            playerCollectionLock.EnterWriteLock();

            try
            {
                if (playerInternalCollection == null)
                {
                    playerInternalCollection = new List<IPlayer>();
                }

                playerInternalCollection.AddRange(playerItems);
            }
            finally
            {
                playerCollectionLock.ExitWriteLock();
            }
        }

        public IList<IPlayer> GetPlayersList()
        {
            if (playerInternalCollection == null)
            {
                playerCollectionLock.EnterWriteLock();

                if (playerInternalCollection == null)
                {
                    try
                    {
                        playerInternalCollection = GetPlayersListInternal();
                    }
                    finally
                    {
                        playerCollectionLock.ExitWriteLock();
                    }
                }
            }

            playerCollectionLock.EnterReadLock();

            try
            {
                return new List<IPlayer>(playerInternalCollection.OrderBy(x => x.Name));
            }
            finally
            {
                playerCollectionLock.ExitReadLock();
            }
        }

        private List<IPlayer> GetPlayersListInternal()
        {
            try
            {
                if (!Directory.Exists(PlayersPath))
                {
                    Directory.CreateDirectory(PlayersPath);
                }

                List<IPlayer> players = new List<IPlayer>();

                using (var session = ModelEntities.OpenSession())
                {
                    players.AddRange(session.Query<Players>().ToArray().Select(x => new PlayerCollectionItem
                    {
                        PlayerId = x.PlayerId,
                        PokerSite = (EnumPokerSites)x.PokersiteId,
                        Name = x.Playername
                    }));

                    return players;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't get player list", e);
            }

            return new List<IPlayer>();
        }

        public IList<IPlayer> GetAliasesList()
        {
            if (aliasesInternalCollection == null)
            {
                aliasCollectionLock.EnterWriteLock();

                if (aliasesInternalCollection == null)
                {
                    try
                    {
                        aliasesInternalCollection = GetAliasesListInternal();
                    }
                    finally
                    {
                        aliasCollectionLock.ExitWriteLock();
                    }
                }
            }

            aliasCollectionLock.EnterReadLock();

            try
            {
                return new List<IPlayer>(aliasesInternalCollection.OrderBy(x => x.Name));
            }
            finally
            {
                aliasCollectionLock.ExitReadLock();
            }
        }

        private List<IPlayer> GetAliasesListInternal()
        {
            try
            {
                using (var session = ModelEntities.OpenSession())
                {
                    var aliasesEntities = session.Query<Aliases>().Fetch(x => x.Players).ToArray();

                    var aliases = aliasesEntities.Select(x => new AliasCollectionItem
                    {
                        PlayerId = x.AliasId,
                        Name = x.AliasName,
                        PlayersInAlias = new ObservableCollection<PlayerCollectionItem>(x.Players.Select(p =>
                        new PlayerCollectionItem
                        {
                            PlayerId = p.PlayerId,
                            PokerSite = (EnumPokerSites)p.PokersiteId,
                            Name = p.Playername
                        }))
                    }).OfType<IPlayer>().ToList();

                    return aliases;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't get aliases list", e);
            }

            return new List<IPlayer>();
        }

        #endregion

        public void RemoveAppData()
        {
            try
            {
                if (Directory.Exists(dataPath))
                {
                    Directory.Delete(dataPath, true);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Couldn't delete directory '{dataPath}'", e);
            }
        }

        /// <summary>
        /// Deletes specific player's statistic from file storage
        /// </summary>
        /// <param name="statistic">Statistic to delete</param>
        public void DeletePlayerStatisticFromFile(Playerstatistic statistic)
        {
            var files = GetPlayerFiles(statistic.PlayerId);

            if (files == null || !files.Any())
            {
                return;
            }

            string convertedStatistic = string.Empty;

            using (var msTestString = new MemoryStream())
            {
                Serializer.Serialize(msTestString, statistic);
                convertedStatistic = Convert.ToBase64String(msTestString.ToArray());
            }

            rwLock.EnterWriteLock();

            try
            {
                foreach (var file in files)
                {
                    string[] allLines = null;
                    allLines = File.ReadAllLines(file);

                    if (allLines.Any(x => x.Equals(convertedStatistic, StringComparison.Ordinal)))
                    {
                        var newLines = new List<string>(allLines.Count());

                        foreach (var line in allLines)
                        {
                            if (!line.Equals(convertedStatistic, StringComparison.Ordinal))
                            {
                                newLines.Add(line);
                            }
                        }

                        File.WriteAllLines(file, newLines);

                        return;
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }

        }

        public void DeleteHandHistory(long handNumber, int pokerSiteId)
        {
            try
            {
                var handHistory = GetGame(handNumber, (short)pokerSiteId);

                if (handHistory == null)
                {
                    LogProvider.Log.Warn(this, $"Hand {handNumber} has not been found in db. So it can't be deleted.");
                    return;
                }

                var allPlayers = GetPlayersList().Where(x => x.PokerSite == (EnumPokerSites)pokerSiteId);

                var players = (from player in allPlayers
                               join hhPlayer in handHistory.Players on player.Name equals hhPlayer.PlayerName
                               select player).ToArray();

                var playerStatisticToDelete = new List<Playerstatistic>();

                var opponentReportService = ServiceLocator.Current.GetInstance<IOpponentReportService>();
                var opponentReportResetRequired = false;

                players.ForEach(x =>
                {
                    if (!opponentReportResetRequired && opponentReportService.IsPlayerInReport(x.PlayerId))
                    {
                        opponentReportResetRequired = true;
                    }

                    ActOnPlayerStatisticFromFile(x.PlayerId, s => s.GameNumber == handNumber && s.PokersiteId == pokerSiteId, s => playerStatisticToDelete.Add(s));
                });

                playerStatisticToDelete.ForEach(x => DeletePlayerStatisticFromFile(x));

                using (var session = ModelEntities.OpenStatelessSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var playerIds = playerStatisticToDelete.Select(x => x.PlayerId).ToArray();

                        // update players summary
                        if (playerIds.Length > 0)
                        {
                            var playersEntities = session.Query<Players>().Where(x => playerIds.Contains(x.PlayerId)).ToArray();

                            playersEntities.ForEach(p =>
                            {
                                if (handHistory.GameDescription.IsTournament)
                                {
                                    p.Tourneyhands--;
                                }
                                else
                                {
                                    p.Cashhands--;
                                }

                                session.Update(p);
                            });

                            var netWons = session.Query<PlayerNetWon>().Where(x => playerIds.Contains(x.PlayerId)).ToArray();

                            netWons.ForEach(n =>
                            {
                                var statistic = playerStatisticToDelete.FirstOrDefault(x => x.PlayerId == n.PlayerId && x.CurrencyId == n.Currency);

                                if (statistic != null)
                                {
                                    n.NetWon -= Utils.ConvertToCents(statistic.NetWon);
                                    session.Update(n);
                                }
                            });
                        }

                        var hh = session.Query<Handhistory>().FirstOrDefault(x => x.Gamenumber == handNumber && x.PokersiteId == pokerSiteId);

                        if (hh != null)
                        {
                            session.Delete(hh);
                        }

                        transaction.Commit();
                    }
                }

                if (opponentReportResetRequired)
                {
                    opponentReportService.Reset();
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Could not delete hand {handNumber} {(EnumPokerSites)pokerSiteId}", ex);
            }
        }

        public IPlayer GetActivePlayer()
        {
            IPlayer activePlayer = new PlayerCollectionItem();

            string dataPath = StringFormatter.GetActivePlayerFilePath();

            if (File.Exists(dataPath))
            {
                var splittedResult = File.ReadAllText(dataPath).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                if (splittedResult.Length < 2)
                {
                    try
                    {
                        using (var session = ModelEntities.OpenSession())
                        {
                            var alias = session.Query<Aliases>().Fetch(x => x.Players).FirstOrDefault(x => x.AliasName.Equals(splittedResult[0]));

                            if (alias != null)
                            {
                                activePlayer = new AliasCollectionItem
                                {
                                    PlayerId = alias.AliasId,
                                    Name = alias.AliasName,
                                    PlayersInAlias = new ObservableCollection<PlayerCollectionItem>(alias.Players.Select(p =>
                                        new PlayerCollectionItem
                                        {
                                            PlayerId = p.PlayerId,
                                            PokerSite = (EnumPokerSites)p.PokersiteId,
                                            Name = p.Playername
                                        }))
                                };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogProvider.Log.Error(this, "Couldn't get active alias", e);
                    }

                    return activePlayer;
                }

                if (!short.TryParse(splittedResult[1], out short pokerSiteId))
                {
                    return activePlayer;
                }

                try
                {
                    using (var session = ModelEntities.OpenSession())
                    {
                        var player = session.Query<Players>().FirstOrDefault(x => x.Playername.Equals(splittedResult[0]) && x.PokersiteId == pokerSiteId);

                        if (player != null)
                        {
                            activePlayer = new PlayerCollectionItem { PlayerId = player.PlayerId, Name = player.Playername, PokerSite = (EnumPokerSites)player.PokersiteId };
                        }
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Couldn't get active player", e);
                }
            }

            return activePlayer;
        }

        public void SaveActivePlayer(string playerName, short? pokersiteId)
        {
            try
            {
                string dataPath = StringFormatter.GetActivePlayerFilePath();

                if (pokersiteId.HasValue)
                {
                    File.WriteAllText(dataPath, $"{playerName}{Environment.NewLine}{pokersiteId}");
                }
                else
                {
                    File.WriteAllText(dataPath, playerName);
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }
        }

        public void VacuumDatabase()
        {
            try
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    session.CreateSQLQuery("vacuum").ExecuteUpdate();
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Vacuuming failed.", ex);
            }
        }

        public IEnumerable<PlayerNetWon> GetTopPlayersByNetWon(int top, IEnumerable<int> playersToExclude)
        {
            if (top < 1)
            {
                return new List<PlayerNetWon>();
            }

            try
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var playersToExcludeQuery = string.Join(",", playersToExclude);

                    var query = session.CreateSQLQuery($@"select hpt2.PlayerId, hpt2.Currency, sum(hpt2.NetWon) as NetWon from HandsPlayers hpt1 
                        join HandsPlayers hpt2 on hpt1.HandId = hpt2.HandId
                        where hpt1.PlayerId in ({playersToExcludeQuery})
                        group by hpt2.PlayerId, hpt2.Currency
                        order by NetWon desc limit 0, {top}");

                    query.SetResultTransformer(PlayerNetWonQuery.Transformer);

                    var result = query.List<PlayerNetWon>();

                    return result;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not read top players by net won [{top}, {string.Join(";", playersToExclude)}]", e);
            }

            return new List<PlayerNetWon>();
        }

        public string[] GetPlayerFiles(int playerId)
        {
            try
            {
                string playerDirectory = Path.Combine(PlayersPath, playerId.ToString());

                if (!Directory.Exists(playerDirectory))
                {
                    return new string[0];
                }

                return Directory.GetFiles(playerDirectory, "*.stat");
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not get player [{playerId}] files", e);
                return new string[0];
            }
        }

        private sealed class PlayerNetWonQuery : IResultTransformer
        {
            public static readonly PlayerNetWonQuery Transformer = new PlayerNetWonQuery();

            private PlayerNetWonQuery()
            {
            }

            public IList TransformList(IList collection)
            {
                return collection;
            }

            public object TransformTuple(object[] tuple, string[] aliases)
            {
                return new PlayerNetWon
                {
                    PlayerId = Convert.ToInt32(tuple[0]),
                    Currency = Convert.ToInt32(tuple[1]),
                    NetWon = Convert.ToInt64(tuple[2])
                };
            }
        }
    }
}