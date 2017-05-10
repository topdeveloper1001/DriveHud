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

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Interfaces;
using NHibernate.Criterion;
using NHibernate.Linq;
using ProtoBuf;
using System;
using System.Collections.Generic;
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

        protected virtual string playersPath
        {
            get;
            set;
        }

        private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public DataService()
        {
            playersPath = StringFormatter.GetPlayerStatisticDataFolderPath();

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            if (!Directory.Exists(playersPath))
            {
                Directory.CreateDirectory(playersPath);
            }
        }

        public void SetPlayerStatisticPath(string path)
        {
            playersPath = path;

            if (!Directory.Exists(playersPath))
            {
                Directory.CreateDirectory(playersPath);
            }
        }

        public Indicators GetPlayerIndicator(int playerId, short pokersiteId)
        {
            var indicator = new Indicators();
            var statistics = GetPlayerStatisticFromFile(playerId, pokersiteId);
            indicator.UpdateSource(statistics);

            return indicator;
        }

        public IList<Playerstatistic> GetPlayerStatisticFromFile(int playerId, short? pokersiteId)
        {
            List<Playerstatistic> result = new List<Playerstatistic>();

            var files = GetPlayerFiles(playerId);

            if (files == null || !files.Any())
            {
                return result;
            }

            foreach (var file in files)
            {
                rwLock.EnterReadLock();

                string[] allLines = null;

                try
                {
                    allLines = File.ReadAllLines(file);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, e);
                    return result;
                }
                finally
                {
                    rwLock.ExitReadLock();
                }

                foreach (var line in allLines)
                {
                    try
                    {
                        /* replace '-' and '_' characters in order to convert back from Modified Base64 (https://en.wikipedia.org/wiki/Base64#Implementations_and_history) */
                        byte[] byteAfter64 = Convert.FromBase64String(line.Replace('-', '+').Replace('_', '/'));

                        using (MemoryStream afterStream = new MemoryStream(byteAfter64))
                        {
                            var stat = Serializer.Deserialize<Playerstatistic>(afterStream);
                            if (!pokersiteId.HasValue || (stat.PokersiteId == pokersiteId))
                            {
                                result.Add(stat);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogProvider.Log.Error($@"Cannot import the file: {file}{Environment.NewLine}Error at line: {line}{Environment.NewLine}", ex);
                    }
                }
            }

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

        public IList<HandHistoryRecord> GetPlayerHandRecords(string playerName, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<HandHistoryRecord>().Where(x => x.Player.Playername == playerName && x.Player.PokersiteId == pokersiteId).ToList();
            }
        }

        public Players GetPlayer(string playerName, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Players>().FirstOrDefault(x => x.Playername == playerName && x.PokersiteId == pokersiteId);
            }
        }

        public IList<HandHistoryRecord> GetHandHistoryRecords()
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<HandHistoryRecord>().Fetch(x => x.Player).ToList();
            }
        }

        public IList<Gametypes> GetPlayerGameTypes(string playerName, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<HandHistoryRecord>().Where(x => x.Player.Playername == playerName && x.Player.PokersiteId == pokersiteId).Select(x => x.GameType).Distinct().ToList();
            }
        }

        public IList<Tournaments> GetPlayerTournaments(string playerName, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Tournaments>().Where(x => x.Player.Playername == playerName && x.SiteId == pokersiteId).Fetch(x => x.Player).ToList();
            }
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

                var handHistoryParser = handHistoryParserFactory.GetFullHandHistoryParser((EnumPokerSites)pokersiteId);

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

                var list = session.QueryOver<Handhistory>().Where(restriction)
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

        public Playernotes GetPlayerNote(string playerName, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                var pn = session.Query<Playernotes>().FirstOrDefault(x => x.Player.Playername == playerName && x.PokersiteId == pokersiteId);
                return pn;
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

        /// <summary>
        /// DO NOT USE - BAD PERFORMANCE
        /// </summary>
        /// <param name="gameNumbers"></param>
        /// <returns></returns>
        public IList<Handnotes> GetHandNotes(IEnumerable<long> gameNumbers, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Handnotes>().Where(x => gameNumbers.Contains(x.Gamenumber) && x.PokersiteId == pokersiteId).ToList();
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

        public void Purge()
        {
            using (var session = ModelEntities.OpenStatelessSession())
            {
                session.CreateSQLQuery("TRUNCATE HandNotes, HandHistories, HandRecords, Players CASCADE").ExecuteUpdate();
            }

            if (Directory.Exists(playersPath))
            {
                Directory.Delete(playersPath, true);
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
                        session.SaveOrUpdate(handnotes);
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
                if (!Directory.Exists(playersPath))
                {
                    Directory.CreateDirectory(playersPath);
                }

                var playerDirectory = Path.Combine(playersPath, statistic.PlayerId.ToString());

                if (!Directory.Exists(playerDirectory))
                {
                    Directory.CreateDirectory(playerDirectory);
                }

                var fileName = Path.Combine(playerDirectory, statistic.Playedyearandmonth.ToString()) + ".stat";

                var data = string.Empty;

                using (var msTestString = new MemoryStream())
                {
                    Serializer.Serialize(msTestString, statistic);
                    data = Convert.ToBase64String(msTestString.ToArray());
                }

                File.AppendAllLines(fileName, new[] { data });

                var storageModel = ServiceLocator.Current.TryResolve<SingletonStorageModel>();

                if (statistic.PlayerId == storageModel.PlayerSelectedItem.PlayerId)
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
                                        PlayerId = grouped.Key.PlayerId,
                                        Playedyearandmonth = grouped.Key.Playedyearandmonth,
                                        Statistic = grouped.OrderBy(x => x.Playedyearandmonth).ToArray()
                                    }).ToArray();

            rwLock.EnterWriteLock();

            try
            {
                if (!Directory.Exists(playersPath))
                {
                    Directory.CreateDirectory(playersPath);
                }

                foreach (var stats in groupedStatistic)
                {
                    var playerDirectory = Path.Combine(playersPath, stats.PlayerId.ToString());

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
                            var data = Convert.ToBase64String(msTestString.ToArray());

                            statisticStringsToAppend.Add(data);
                        }
                    }

                    File.AppendAllLines(fileName, statisticStringsToAppend);

                    var storageModel = ServiceLocator.Current.TryResolve<SingletonStorageModel>();

                    if (stats.PlayerId == storageModel.PlayerSelectedItem.PlayerId)
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

        private List<PlayerCollectionItem> playerInternalCollection = null;

        private ReaderWriterLockSlim playerCollectionLock = new ReaderWriterLockSlim();

        public void AddPlayerToList(PlayerCollectionItem playerItem)
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
                    playerInternalCollection = new List<PlayerCollectionItem>();
                }

                playerInternalCollection.Add(playerItem);
            }
            finally
            {
                playerCollectionLock.ExitWriteLock();
            }
        }

        public void AddPlayerRangeToList(IEnumerable<PlayerCollectionItem> playerItems)
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
                    playerInternalCollection = new List<PlayerCollectionItem>();
                }

                playerInternalCollection.AddRange(playerItems);
            }
            finally
            {
                playerCollectionLock.ExitWriteLock();
            }
        }

        public IList<PlayerCollectionItem> GetPlayersList()
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
                return new List<PlayerCollectionItem>(playerInternalCollection.OrderBy(x => x.Name));
            }
            finally
            {
                playerCollectionLock.ExitReadLock();
            }
        }

        private List<PlayerCollectionItem> GetPlayersListInternal()
        {
            try
            {
                if (!Directory.Exists(playersPath))
                {
                    Directory.CreateDirectory(playersPath);
                }

                using (var session = ModelEntities.OpenSession())
                {
                    var players = session.Query<Players>().ToArray().Select(x => new PlayerCollectionItem
                    {
                        PlayerId = x.PlayerId,
                        PokerSite = (EnumPokerSites)x.PokersiteId,
                        Name = x.Playername
                    }).ToList();

                    return players;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't get player list", e);
            }

            return new List<PlayerCollectionItem>();
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
                return;

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

        public PlayerCollectionItem GetActivePlayer()
        {
            var activePlayer = new PlayerCollectionItem();

            string dataPath = StringFormatter.GetActivePlayerFilePath();

            if (File.Exists(dataPath))
            {
                var splittedResult = File.ReadAllText(dataPath).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                if (splittedResult.Length < 2)
                {
                    return activePlayer;
                }

                short pokerSiteId = 0;

                if (!short.TryParse(splittedResult[1], out pokerSiteId))
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

        public void SaveActivePlayer(string playerName, short pokersiteId)
        {
            try
            {
                string dataPath = StringFormatter.GetActivePlayerFilePath();

                File.WriteAllText(dataPath, $"{playerName}{Environment.NewLine}{pokersiteId}");
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }
        }

        private string[] GetPlayerFiles(int playerId)
        {
            string playerDirectory = Path.Combine(playersPath, playerId.ToString());

            if (!Directory.Exists(playerDirectory))
            {
                return new string[] { };
            }

            return Directory.GetFiles(playerDirectory, "*.stat");
        }
    }
}