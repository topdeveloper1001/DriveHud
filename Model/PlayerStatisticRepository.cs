//-----------------------------------------------------------------------
// <copyright file="PlayerStatisticRepository.cs" company="Ace Poker Solutions">
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
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using NHibernate.Linq;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Model
{
    public class PlayerStatisticRepository : IPlayerStatisticRepository
    {
        private const string PlayerStatisticExtension = ".stat";

        private const string PlayerStatisticBackupExtenstion = ".bak";

        private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        protected readonly string dataPath = StringFormatter.GetAppDataFolderPath();

        public PlayerStatisticRepository()
        {
            PlayersPath = StringFormatter.GetPlayerStatisticDataFolderPath();

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            CreatePlayersFolderIfNotExist();
        }

        protected virtual string PlayersPath
        {
            get;
            set;
        }

        #region Public methods

        public virtual void SetPlayerStatisticPath(string path)
        {
            PlayersPath = path;
            CreatePlayersFolderIfNotExist();
        }

        public virtual IEnumerable<Playerstatistic> GetAllPlayerStatistic(string playerName, short? pokersiteId)
        {
            try
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var player = session.Query<Players>()
                        .FirstOrDefault(x => x.Playername.Equals(playerName) && x.PokersiteId == pokersiteId);

                    if (player == null)
                    {
                        return new List<Playerstatistic>();
                    }

                    var files = GetPlayerFiles(player.PlayerId);

                    var result = GetAllPlayerStatisticFromFiles(files, pokersiteId);

                    return result;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Couldn't get player's {playerName} playerName.", e);
            }

            return new List<Playerstatistic>();
        }

        public virtual IEnumerable<Playerstatistic> GetPlayerStatisticFromFiles(IEnumerable<string> files)
        {
            if (files == null)
            {
                throw new ArgumentNullException(nameof(files));
            }

            rwLock.EnterReadLock();

            try
            {
                foreach (var file in files)
                {
                    RestoreBackupFile(file);

                    using (var sr = new StreamReaderWrapper(file))
                    {
                        string line = null;

                        while (sr != null && ((line = sr.ReadLine()) != null))
                        {
                            Playerstatistic stat = null;

                            try
                            {
                                if (string.IsNullOrWhiteSpace(line))
                                {
                                    LogProvider.Log.Warn(this, $"Empty line in {file}");
                                }

                                /* replace '-' and '_' characters in order to convert back from Modified Base64 (https://en.wikipedia.org/wiki/Base64#Implementations_and_history) */
                                byte[] byteAfter64 = Convert.FromBase64String(line.Replace('-', '+').Replace('_', '/').Trim());

                                using (var ms = new MemoryStream(byteAfter64))
                                {
                                    stat = Serializer.Deserialize<Playerstatistic>(ms);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogProvider.Log.Error($@"Could not process the file: {file}{Environment.NewLine}Error at line: {line}{Environment.NewLine}", ex);
                            }

                            if (stat != null)
                            {
                                yield return stat;
                            }
                        }
                    }
                }
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public virtual IEnumerable<Playerstatistic> GetPlayerStatisticFromFile(string file)
        {
            return GetPlayerStatisticFromFiles(new[] { file });
        }

        public virtual IEnumerable<Playerstatistic> GetPlayerStatistic(int playerId)
        {
            var files = GetPlayerFiles(playerId);
            return GetPlayerStatisticFromFiles(files);
        }

        public virtual IEnumerable<Playerstatistic> GetPlayerStatistic(string playerName, short? pokersiteId)
        {
            try
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var player = session.Query<Players>()
                        .FirstOrDefault(x => x.Playername.Equals(playerName) && x.PokersiteId == pokersiteId);

                    if (player == null)
                    {
                        return new List<Playerstatistic>();
                    }

                    var result = GetPlayerStatistic(player.PlayerId)
                        .Where(x => x.PokersiteId == pokersiteId);

                    return result;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Couldn't get player's {playerName} playerName.", e);
            }

            return new List<Playerstatistic>();
        }

        public virtual IDictionary<string, T> GetPlayersIndicators<T>(string[] playerNames, short? pokersiteId)
            where T : Indicators, IThreadSafeIndicators
        {
            try
            {
                Players[] players;

                // get players ids
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    players = session.Query<Players>()
                        .Where(x => playerNames.Contains(x.Playername) && x.PokersiteId == pokersiteId)
                        .ToArray();
                }

                var playersStatFiles = (from player in players
                                        from file in GetPlayerFiles(player.PlayerId)
                                        select new PlayerStatFile { Player = player, File = file }).ToArray();

                rwLock.EnterReadLock();

                try
                {
                    var maxThreads = Environment.ProcessorCount + 1;

                    var runningTasks = new List<Task>();

                    var fileQueue = GetPlayerStatFileQueue(playersStatFiles);

                    var playersIndicators = (from playersStatFile in playersStatFiles
                                             group playersStatFile by playersStatFile.Player.Playername into grouped
                                             where grouped.Any()
                                             let indicators = Activator.CreateInstance<T>()
                                             select new { grouped.Key, Indicators = indicators }).ToDictionary(x => x.Key, x => x.Indicators);


                    while (fileQueue.Count > 0)
                    {
                        while (runningTasks.Count < maxThreads && fileQueue.Count > 0)
                        {
                            var playerStatFile = fileQueue.Dequeue();

                            runningTasks.Add(Task.Run(() =>
                            {
                                RestoreBackupFile(playerStatFile.File);

                                using (var sr = new StreamReaderWrapper(playerStatFile.File))
                                {
                                    string line = null;

                                    while (sr != null && ((line = sr.ReadLine()) != null))
                                    {
                                        if (!TryParsePlayerStatistic(line, playerStatFile.File, out Playerstatistic stat) || stat == null)
                                        {
                                            continue;
                                        }

                                        var indicators = playersIndicators[playerStatFile.Player.Playername];

                                        indicators.AddStatistic(stat);
                                    }
                                }
                            }));
                        }

                        var completedTask = Task.WhenAny(runningTasks).Result;

                        runningTasks.Remove(completedTask);
                    }

                    Task.WhenAll(runningTasks).Wait();

                    return playersIndicators;
                }
                finally
                {
                    rwLock.ExitReadLock();
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not build indicators for {string.Join(", ", playerNames)} [{pokersiteId}]", e);
            }

            return null;
        }

        public virtual string[] GetPlayerFiles(int playerId)
        {
            try
            {
                var playerDirectory = Path.Combine(PlayersPath, playerId.ToString());

                if (!Directory.Exists(playerDirectory))
                {
                    return new string[0];
                }

                return Directory.GetFiles(playerDirectory, $"*{PlayerStatisticExtension}");
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not get player [{playerId}] files", e);
                return new string[0];
            }
        }

        public virtual void Store(Playerstatistic statistic)
        {
            rwLock.EnterWriteLock();

            var file = string.Empty;

            try
            {
                CreatePlayersFolderIfNotExist();

                file = GetPlayerstatisticFile(statistic, true);

                RestoreBackupFile(file);

                var data = string.Empty;

                using (var memoryStream = new MemoryStream())
                {
                    Serializer.Serialize(memoryStream, statistic);
                    data = Convert.ToBase64String(memoryStream.ToArray()).Trim();
                }

                File.AppendAllLines(file, new[] { data });

                UpdateStorageModel(statistic);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not save player statistic to the {file}.", e);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public virtual void Store(IEnumerable<Playerstatistic> statistic)
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
                CreatePlayersFolderIfNotExist();

                foreach (var stats in groupedStatistic)
                {
                    var fileName = GetPlayerstatisticFile(stats.PlayerId, stats.Playedyearandmonth.ToString(), true);

                    RestoreBackupFile(fileName);

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

                    UpdateStorageModel(stats.PlayerId, stats.Statistic);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not save player statistic.", e);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public virtual void DeletePlayerStatisticFromFile(Playerstatistic statistic)
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

        public virtual void DeletePlayerStatistic(Dictionary<int, List<Handhistory>> playersHands)
        {
            if (playersHands == null || playersHands.Count == 0)
            {
                return;
            }

            var fileHands = (from playerHands in playersHands
                             from hand in playerHands.Value
                             where hand.Handtimestamp.HasValue
                             let date = hand.Handtimestamp.Value.ToString("yyyyMM")
                             let playerStatisticFile = GetPlayerstatisticFile(playerHands.Key, date)
                             group new { File = playerStatisticFile, hand.Gamenumber } by playerStatisticFile
                             ).ToDictionary(x => x.Key, x => x.Select(y => y.Gamenumber).ToArray());

            rwLock.EnterWriteLock();

            try
            {
                Parallel.ForEach(fileHands, fh => DeletePlayerStatisticFromFile(fh.Key, new HashSet<long>(fh.Value.Distinct())));
            }
            catch
            {
                var players = string.Join(", ", playersHands.Select(x => x.Key).ToArray());
                LogProvider.Log.Error(this, $"Could not delete player statistic for players [{players}].");
                throw;
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        private void DeletePlayerStatisticFromFile(string file, HashSet<long> handNumbers)
        {
            if (!File.Exists(file))
            {
                return;
            }

            try
            {
                CreateBackupFile(file);

                var lines = new List<string>();

                using (var sr = new StreamReaderWrapper(file))
                {
                    string line = null;

                    while (sr != null && ((line = sr.ReadLine()) != null))
                    {
                        if (!TryParsePlayerStatistic(line, file, out Playerstatistic stat))
                        {
                            continue;
                        }

                        if (!handNumbers.Contains(stat.GameNumber))
                        {
                            lines.Add(line);
                        }
                    }
                }

                File.WriteAllLines(file, lines);

                DeleteBackupFile(file);
            }
            catch
            {
                LogProvider.Log.Error(this, $"Could not delete player statistic from file '{file}'");
                throw;
            }
        }

        private void CreateBackupFile(string file)
        {
            var backupFile = GetBackupFile(file);

            try
            {
                File.Copy(file, backupFile);
            }
            catch
            {
                LogProvider.Log.Error(this, $"Could not create backup file '{backupFile}'");
                throw;
            }
        }

        private void DeleteBackupFile(string file)
        {
            var backupFile = GetBackupFile(file);

            try
            {
                File.Delete(backupFile);
            }
            catch
            {
                LogProvider.Log.Error(this, $"Could not delete backup file '{backupFile}'");
                throw;
            }
        }

        private void RestoreBackupFile(string file)
        {
            var backupFile = GetBackupFile(file);

            if (!File.Exists(backupFile))
            {
                return;
            }

            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                File.Move(backupFile, file);

                LogProvider.Log.Info(this, $"Restored stat file '{file}' from '{backupFile}'.");
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not restore backup file '{backupFile}'", e);
            }
        }

        private static string GetBackupFile(string file)
        {
            return Path.ChangeExtension(file, PlayerStatisticBackupExtenstion);
        }

        protected virtual IEnumerable<Playerstatistic> GetAllPlayerStatisticFromFiles(IEnumerable<string> files, short? pokerSite)
        {
            if (files == null)
            {
                throw new ArgumentNullException(nameof(files));
            }

            rwLock.EnterReadLock();

            try
            {
                var maxThreads = Environment.ProcessorCount + 1;

                var runningTasks = new List<Task>();

                var fileQueue = new Queue<string>(files);

                var stats = new BlockingCollection<Playerstatistic>();

                while (fileQueue.Count > 0)
                {
                    while (runningTasks.Count < maxThreads && fileQueue.Count > 0)
                    {
                        var file = fileQueue.Dequeue();

                        runningTasks.Add(Task.Run(() =>
                        {
                            RestoreBackupFile(file);

                            using (var sr = new StreamReaderWrapper(file))
                            {
                                string line = null;

                                while (sr != null && ((line = sr.ReadLine()) != null))
                                {
                                    Playerstatistic stat = null;

                                    try
                                    {
                                        if (string.IsNullOrWhiteSpace(line))
                                        {
                                            LogProvider.Log.Warn(this, $"Empty line in {file}");
                                        }

                                        /* replace '-' and '_' characters in order to convert back from Modified Base64 (https://en.wikipedia.org/wiki/Base64#Implementations_and_history) */
                                        byte[] byteAfter64 = Convert.FromBase64String(line.Replace('-', '+').Replace('_', '/').Trim());

                                        using (var ms = new MemoryStream(byteAfter64))
                                        {
                                            stat = Serializer.Deserialize<Playerstatistic>(ms);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogProvider.Log.Error($@"Could not process the file: {file}{Environment.NewLine}Error at line: {line}{Environment.NewLine}", ex);
                                    }

                                    if (stat != null &&
                                        (pokerSite.HasValue && stat.PokersiteId == pokerSite || !pokerSite.HasValue))
                                    {
                                        stats.Add(stat);
                                    }
                                }
                            }
                        }));
                    }

                    var completedTask = Task.WhenAny(runningTasks).Result;

                    runningTasks.Remove(completedTask);
                }

                Task.WhenAll(runningTasks).Wait();

                return stats.ToList();
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        #endregion

        #region Infrastructure

        private bool TryParsePlayerStatistic(string line, string file, out Playerstatistic stat)
        {
            stat = null;

            try
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    LogProvider.Log.Warn(this, $"Empty line in {file}");
                    return false;
                }

                /* replace '-' and '_' characters in order to convert back from Modified Base64 (https://en.wikipedia.org/wiki/Base64#Implementations_and_history) */
                byte[] byteAfter64 = Convert.FromBase64String(line.Replace('-', '+').Replace('_', '/').Trim());

                using (var ms = new MemoryStream(byteAfter64))
                {
                    stat = Serializer.Deserialize<Playerstatistic>(ms);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error($@"Could not process the file: {file}{Environment.NewLine}Error at line: {line}{Environment.NewLine}", ex);
            }

            return false;
        }

        private string GetPlayerstatisticFile(Playerstatistic statistic, bool createPlayerDirectory = false)
        {
            return GetPlayerstatisticFile(statistic.PlayerId, statistic.Playedyearandmonth.ToString(), createPlayerDirectory);
        }

        private string GetPlayerstatisticFile(int playerId, string date, bool createPlayerDirectory = false)
        {
            var playerDirectory = Path.Combine(PlayersPath, playerId.ToString());

            if (createPlayerDirectory && !Directory.Exists(playerDirectory))
            {
                Directory.CreateDirectory(playerDirectory);
            }

            var playerStatisticFile = Path.Combine(playerDirectory, date);
            playerStatisticFile = Path.ChangeExtension(playerStatisticFile, PlayerStatisticExtension);

            return playerStatisticFile;
        }

        private void UpdateStorageModel(int playerId, IEnumerable<Playerstatistic> statistic)
        {
            var storageModel = ServiceLocator.Current.TryResolve<SingletonStorageModel>();

            if (storageModel != null && storageModel.PlayerSelectedItem != null &&
                (playerId == storageModel.PlayerSelectedItem.PlayerId || storageModel.PlayerSelectedItem.PlayerIds.Contains(playerId)))
            {
                storageModel.StatisticCollection.AddRange(statistic);
            }
        }

        private void UpdateStorageModel(Playerstatistic statistic)
        {
            UpdateStorageModel(statistic.PlayerId, new[] { statistic });
        }

        private void CreatePlayersFolderIfNotExist()
        {
            try
            {
                if (Directory.Exists(PlayersPath))
                {
                    return;
                }

                Directory.CreateDirectory(PlayersPath);
            }
            catch
            {
                LogProvider.Log.Error($"Could not create directory at '{PlayersPath}'.");
                throw;
            }
        }

        /// <summary>
        /// Gets <see cref="Queue{PlayerStatFile}"/> of <see cref="PlayerStatFile"/> in special order to increase the efficiency of multi-threading
        /// </summary>
        /// <param name="playersStatFiles"></param>
        /// <returns></returns>
        private Queue<PlayerStatFile> GetPlayerStatFileQueue(PlayerStatFile[] playersStatFiles)
        {
            var orderedPlayerStatFile = new List<PlayerStatFile>();

            var groupedPlayerStatFiles = playersStatFiles
                .GroupBy(x => x.Player.Playername, x => x)
                .Select(x => new
                {
                    x.Key,
                    PlayerStatFiles = new Queue<PlayerStatFile>(x)
                }).ToArray();

            while (groupedPlayerStatFiles.Any(x => x.PlayerStatFiles.Count > 0))
            {
                foreach (var groupedPlayerStatFile in groupedPlayerStatFiles)
                {
                    if (groupedPlayerStatFile.PlayerStatFiles.Count > 0)
                    {
                        orderedPlayerStatFile.Add(groupedPlayerStatFile.PlayerStatFiles.Dequeue());
                    }
                }
            }

            return new Queue<PlayerStatFile>(orderedPlayerStatFile);
        }

        #endregion

        #region Helpers

        private class StreamReaderWrapper : IDisposable
        {
            private StreamReader streamReader;

            public StreamReaderWrapper(string file)
            {
                try
                {
                    streamReader = new StreamReader(file);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"File '{file}' has not been processed.", e);
                }
            }

            public string ReadLine()
            {
                return streamReader?.ReadLine();
            }

            public void Dispose()
            {
                streamReader?.Dispose();
            }
        }

        private class PlayerStatFile
        {
            public Players Player { get; set; }

            public string File { get; set; }
        }

        #endregion
    }
}