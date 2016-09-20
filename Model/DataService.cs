using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Interfaces;
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
        private readonly string dataPath = StringFormatter.GetAppDataFolderPath();
        private string playersPath
        {
            get { return StringFormatter.GetProcessedDataFolderPath(); }
        }

        private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public DataService()
        {
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            if (!Directory.Exists(playersPath))
                Directory.CreateDirectory(playersPath);
        }

        public IList<Playerstatistic> GetPlayerStatistic(string playerName)
        {
            using (var session = ModelEntities.OpenSession())
            {
                var player = session.Query<Players>().FirstOrDefault(x => x.Playername == playerName);
                if (player == null)
                    return null;
                return session.Query<Playerstatistic>().Where(x => x.PlayerId == player.PlayerId).ToList();
            }
        }

        public Indicators GetPlayerIndicator(string playerName)
        {
            var indicator = new Indicators();
            var statistics = GetPlayerStatisticFromFile(playerName);
            indicator.UpdateSource(statistics);

            return indicator;
        }

        public IList<Playerstatistic> GetPlayerStatisticFromFile(string playerName)
        {
            List<Playerstatistic> result = new List<Playerstatistic>();

            var files = GetPlayerFiles(playerName);

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
                            result.Add(stat);
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

        public IList<HandHistoryRecord> GetPlayerHandRecords(string playerName)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<HandHistoryRecord>().Where(x => x.Player.Playername == playerName).ToList();
            }
        }

        public Players GetPlayer(string playerName, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Players>().FirstOrDefault(x => x.Playername == playerName && x.PokersiteId == pokersiteId);
            }
        }

        public IList<Gametypes> GetPlayerGameTypes(string playerName)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<HandHistoryRecord>().Where(x => x.Player.Playername == playerName).Select(x => x.GameType).Distinct().ToList();
            }
        }

        public IList<Tournaments> GetPlayerTournaments(string playerName)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Tournaments>().Where(x => x.Player.Playername == playerName).Fetch(x => x.Player).ToList();
            }
        }

        public Tournaments GetTournament(string tournamentId, string playerName)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Tournaments>().FirstOrDefault(x => x.Tourneynumber == tournamentId && x.Player.Playername == playerName);
            }
        }

        public HandHistory GetGame(long gameNumber)
        {
            using (var session = ModelEntities.OpenSession())
            {
                var hh = session.Query<Handhistory>().FirstOrDefault(x => x.Gamenumber == gameNumber);
                if (hh == null)
                    return null;
                var result = ServiceLocator.Current.GetInstance<IParser>().ParseGame(hh.HandhistoryVal);
                if (result == null)
                    return null;

                return result.Source;
            }
        }

        public IList<HandHistory> GetGames(IEnumerable<long> gameNumbers)
        {
            using (var session = ModelEntities.OpenSession())
            {
                List<HandHistory> historyList = new List<HandHistory>();
                var hh = session.Query<Handhistory>().Where(x => gameNumbers.Any(g => g == x.Gamenumber));
                if (hh == null || hh.Count() == 0)
                    return null;

                foreach (var history in hh)
                {
                    var result = ServiceLocator.Current.GetInstance<IParser>().ParseGame(history.HandhistoryVal);
                    if (result == null)
                        continue;

                    historyList.Add(result.Source);
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

        public Handnotes GetHandNote(long gameNumber)
        {
            using (var session = ModelEntities.OpenSession())
            {
                var hn = session.Query<Handnotes>().FirstOrDefault(x => x.Gamenumber == gameNumber);

                return hn;
            }
        }

        /// <summary>
        /// DO NOT USE - BAD PERFORMANCE
        /// </summary>
        /// <param name="gameNumbers"></param>
        /// <returns></returns>
        public IList<Handnotes> GetHandNotes(IEnumerable<long> gameNumbers)
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Handnotes>().Where(x => gameNumbers.Contains(x.Gamenumber)).ToList();
            }
        }

        public IList<Handnotes> GetHandNotes()
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Handnotes>().ToList();
            }
        }

        public Handhistory GetHandHistory(long gameNumber)
        {
            using (var session = ModelEntities.OpenSession())
            {
                var hh = session.Query<Handhistory>().FirstOrDefault(x => x.Gamenumber == gameNumber);

                return hh ?? null;
            }
        }

        public void Purge()
        {
            using (var session = ModelEntities.OpenSession())
            {
                session.CreateSQLQuery("TRUNCATE \"PlayerStatistic\", \"HandNotes\", \"HandHistories\", \"HandRecords\", \"Players\" CASCADE").ExecuteUpdate();
            }

            if (Directory.Exists(playersPath))
            {
                Directory.Delete(playersPath, true);
            }
        }

        public void Store(Handnotes handnotes)
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

        public void Store(Playernotes playernotes)
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

        public void Store(Tournaments tournament)
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

        public void Store(Playerstatistic statistic)
        {
            if (!Directory.Exists(playersPath))
            {
                Directory.CreateDirectory(playersPath);
            }

            var playerDirectory = Path.Combine(playersPath, statistic.PlayerName);

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

            rwLock.EnterWriteLock();

            try
            {
                File.AppendAllLines(fileName, new[] { data });

                var storageModel = ServiceLocator.Current.TryResolve<SingletonStorageModel>();

                if (statistic.PlayerName == storageModel.PlayerSelectedItem)
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

        public IList<string> GetPlayersList()
        {
            if (!Directory.Exists(playersPath))
                Directory.CreateDirectory(playersPath);
            return Directory.GetDirectories(playersPath).Select(x => new DirectoryInfo(x).Name).ToList();
        }

        public void RemoveAppData()
        {
            if (Directory.Exists(dataPath))
            {
                Directory.Delete(dataPath, true);
            }
        }

        /// <summary>
        /// Deletes specific player's statistic from file storage
        /// </summary>
        /// <param name="statistic">Statistic to delete</param>
        public void DeletePlayerStatisticFromFile(Playerstatistic statistic)
        {
            var files = GetPlayerFiles(statistic.PlayerName);

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

        public string GetActivePlayer()
        {
            string result = string.Empty;
            string dataPath = StringFormatter.GetActivePlayerFilePath();
            if (File.Exists(dataPath))
            {
                result = File.ReadAllText(dataPath);
            }

            return result;
        }

        public void SaveActivePlayer(string playerName)
        {
            try
            {
                string dataPath = StringFormatter.GetActivePlayerFilePath();
             
                File.WriteAllText(dataPath, playerName);
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }
        }

        private string[] GetPlayerFiles(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                return new string[] { };
            }

            string playerDirectory = Path.Combine(playersPath, playerName);

            if (!Directory.Exists(playerDirectory))
            {
                return new string[] { };
            }

            return Directory.GetFiles(playerDirectory, "*.stat");
        }

    }
}