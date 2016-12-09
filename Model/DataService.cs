using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Factory;
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
            get { return StringFormatter.GetPlayersDataFolderPath(); }
        }

        private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public DataService()
        {
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            if (!Directory.Exists(playersPath))
                Directory.CreateDirectory(playersPath);
        }

        public IList<Playerstatistic> GetPlayerStatistic(string playerName, short pokersiteId)
        {
            using (var session = ModelEntities.OpenSession())
            {
                var player = session.Query<Players>().FirstOrDefault(x => x.Playername == playerName && x.PokersiteId == pokersiteId);
                if (player == null)
                    return null;
                return session.Query<Playerstatistic>().Where(x => x.PlayerId == player.PlayerId).ToList();
            }
        }

        public Indicators GetPlayerIndicator(string playerName, short pokersiteId)
        {
            var indicator = new Indicators();
            var statistics = GetPlayerStatisticFromFile(playerName, pokersiteId);
            indicator.UpdateSource(statistics);

            return indicator;
        }

        public IList<Playerstatistic> GetPlayerStatisticFromFile(string playerName, short? pokersiteId)
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
                var hh = session.Query<Handhistory>().Where(x => gameNumbers.Any(g => g == x.Gamenumber));
                if (hh == null || hh.Count() == 0)
                    return null;

                foreach (var history in hh)
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

                if (statistic.PlayerName == storageModel.PlayerSelectedItem.Name && (EnumPokerSites)statistic.PokersiteId == storageModel.PlayerSelectedItem.PokerSite)
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

        public IList<PlayerCollectionItem> GetPlayersList()
        {
            if (!Directory.Exists(playersPath))
                Directory.CreateDirectory(playersPath);

            var names = Directory.GetDirectories(playersPath).Select(x => new DirectoryInfo(x).Name).ToList();
            var result = new List<PlayerCollectionItem>();

            using (var session = ModelEntities.OpenSession())
            {
                var hh = session.Query<Players>().Where(x => names.Contains(x.Playername));

                if (hh != null)
                {
                    var group = hh.ToList().GroupBy(x => x.Playername);

                    foreach (var pg in group)
                    {
                        if (!pg.Any())
                        {
                            continue;
                        }

                        if (pg.Count() == 1)
                        {
                            result.Add(new PlayerCollectionItem { Name = pg.Key, PokerSite = (EnumPokerSites)pg.FirstOrDefault()?.PokersiteId });
                        }
                        else
                        {
                            var stats = GetPlayerStatisticFromFile(pg.Key, null);
                            stats.Select(s => s.PokersiteId).Distinct().ForEach(s => result.Add(new PlayerCollectionItem { Name = pg.Key, PokerSite = (EnumPokerSites)s }));
                        }
                    }
                }
            }

            return result;
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

        public PlayerCollectionItem GetActivePlayer()
        {
            PlayerCollectionItem result = new PlayerCollectionItem();
            string dataPath = StringFormatter.GetActivePlayerFilePath();
            if (File.Exists(dataPath))
            {
                var splittedResult = File.ReadAllText(dataPath).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                if (splittedResult != null && splittedResult.Count() == 2)
                {
                    result = new PlayerCollectionItem { Name = splittedResult[0], PokerSite = (EnumPokerSites)Enum.Parse(typeof(EnumPokerSites), splittedResult[1]) };
                }
            }

            return result;
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