using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using Model;
using NHibernate;
using NHibernate.Linq;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DriveHUD.DBMigration
{
    class Program
    {
        static int Main(string[] args)
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();

            try
            {
                Migrate();
                PrepareStatisticMigration();
                MigratePlayerStatistic();
            }
            catch (Exception e)
            {
                WriteException(e);
                Console.WriteLine("Failed");
                return 1;
            }

            Console.WriteLine("Done");


            return 0;
        }

        static void WriteException(Exception e)
        {
            Console.WriteLine($"Migration failed: {e}");
        }

        static void PrepareStatisticMigration()
        {
            Console.WriteLine("Preparing to migration");
            Console.WriteLine("Removing inconsistent");

            using (var session = ModelEntities.OpenStatelessSession())
            {
                using (var transation = session.BeginTransaction())
                {
                    var players = session.Query<Players>().ToArray();
                    var playersGrouped = players.GroupBy(x => new { x.Playername, x.PokersiteId }).Select(x => new { PlayerKey = x.Key, Players = x.ToArray() }).ToArray();

                    var inconsistentPlayes = playersGrouped.Where(x => x.Players.Length > 1).ToArray();

                    inconsistentPlayes.ForEach(x =>
                    {
                        var playersToDelete = x.Players.Skip(1).ToArray();

                        playersToDelete.ForEach(player =>
                        {
                            session.Delete(player);
                        });
                    });

                    transation.Commit();
                }
            }
        }

        static void Migrate()
        {
            var sw = new Stopwatch();
            long rows = 0;
            long totalTime = 0;

            Console.WriteLine("Starting migration");

            MigrateTable<Players>(sw, x => x.PlayerId, ref rows, ref totalTime);
            MigrateTable<Gametypes>(sw, x => x.GametypeId, ref rows, ref totalTime);
            MigrateTable<Handhistory>(sw, x => x.HandhistoryId, ref rows, ref totalTime);
            MigrateTable<Handnotes>(sw, x => x.HandNoteId, ref rows, ref totalTime);
            MigrateTable<HandHistoryRecord>(sw, x => x.Id, ref rows, ref totalTime);
            MigrateTable<Playernotes>(sw, x => x.PlayerNoteId, ref rows, ref totalTime);
            MigrateTable<Tournaments>(sw, x => x.TourneydataId, ref rows, ref totalTime);
            Console.WriteLine($"Rows migrated: {rows} [{totalTime} ms]");
            Console.WriteLine("DB Migration completed");
        }

        static void MigrateTable<T>(Stopwatch sw, Expression<Func<T, object>> getIdExpression, ref long rows, ref long totalTime)
        {
            using (var session = ModelEntities.OpenPostgreSQLSession())
            {
                var entitiesCount = session.Query<T>().Count();

                var rowsPerCycle = 200000;

                var numOfCycles = (int)Math.Ceiling((double)entitiesCount / rowsPerCycle);

                for (var i = 0; i < numOfCycles; i++)
                {
                    var limitFrom = i * rowsPerCycle;

                    MigrateTablePartial(sw, getIdExpression, ref rows, ref totalTime, session, limitFrom, rowsPerCycle);

                    session.Clear();
                    GC.Collect();
                }
            }
        }

        static void MigrateTablePartial<T>(Stopwatch sw, Expression<Func<T, object>> getIdExpression, ref long rows, ref long totalTime, ISession pgSession, int limitFrom, int rowsToSelect)
        {
            T[] entities = null;

            sw.Restart();
            entities = pgSession.Query<T>().OrderBy(getIdExpression).Skip(limitFrom).Take(rowsToSelect).ToArray();
            sw.Stop();

            var getId = getIdExpression.Compile();

            Console.WriteLine($"Read total {typeof(T).Name}: {entities.Length} [{sw.ElapsedMilliseconds} ms]");
            totalTime += sw.ElapsedMilliseconds;

            using (var session = ModelEntities.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    sw.Restart();

                    for (var i = 0; i < entities.Length; i++)
                    {
                        session.Save(entities[i], getId(entities[i]));
                    }

                    transaction.Commit();

                    sw.Stop();
                }

                Console.WriteLine($"Migrated rows of {typeof(T).Name}: {entities.Length} [{sw.ElapsedMilliseconds} ms]");
                rows += entities.Length;
                totalTime += sw.ElapsedMilliseconds;
            }
        }

        static void MigratePlayerStatistic()
        {
            var players = ReadPlayers();

            var playersDataFolder = StringFormatter.GetPlayersDataFolderPath();

            if (!Directory.Exists(playersDataFolder))
            {
                Console.WriteLine($"Directory \"{playersDataFolder}\" not found. No data to be migrated.");
                return;
            }

            var statFiles = Directory.GetFiles(playersDataFolder, "*.stat", SearchOption.AllDirectories);

            Console.WriteLine($"Total files found: {statFiles.Length}");
            Console.WriteLine("Reading data...");

            var playerStatistic = new List<Playerstatistic>();

            var maxBuffer = 25000;
            var totalMigrated = 0;
            long totalTime = 0;

            var sw = new Stopwatch();
            sw.Start();

            foreach (var file in statFiles)
            {
                try
                {
                    using (var sr = new StreamReader(file, new UTF8Encoding(false, true)))
                    {
                        while (sr.Peek() >= 0)
                        {
                            if (playerStatistic.Count >= maxBuffer)
                            {
                                sw.Stop();
                                Console.WriteLine($"Read {playerStatistic.Count} rows [{sw.ElapsedMilliseconds}]");
                                InsertPlayerStatistic(playerStatistic, ref totalMigrated, ref totalTime);
                                sw.Restart();
                            }

                            try
                            {
                                var line = sr.ReadLine().Replace("\0", string.Empty);

                                if (string.IsNullOrWhiteSpace(line))
                                {
                                    continue;
                                }

                                var byteAfter64 = Convert.FromBase64String(line.Replace('-', '+').Replace('_', '/'));

                                using (MemoryStream afterStream = new MemoryStream(byteAfter64))
                                {
                                    var stat = Serializer.Deserialize<Playerstatistic>(afterStream);

                                    var playerKey = new PlayersKey { PlayerName = stat.PlayerName, PokersiteId = stat.PokersiteId };

                                    if (!players.ContainsKey(playerKey))
                                    {
                                        Console.WriteLine($"Player \"{stat.PlayerName}\" [PlayerName={stat.PlayerName};PokerSiteId={stat.PokersiteId}] not found. Skipped.");
                                        continue;
                                    }

                                    stat.PlayerId = players[playerKey].PlayerId;

                                    playerStatistic.Add(stat);
                                }
                            }
                            catch (Exception e)
                            {
                                WriteException(e);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    WriteException(e);
                }
            }

            sw.Stop();
            Console.WriteLine($"Read {playerStatistic.Count} rows [{sw.ElapsedMilliseconds}]");
            InsertPlayerStatistic(playerStatistic, ref totalMigrated, ref totalTime);

            Console.WriteLine($"Migrated rows of PlayerStatistic: {totalMigrated} [{totalTime} ms]");
        }

        static void InsertPlayerStatistic(List<Playerstatistic> playerStatistic, ref int totalMigrated, ref long totalTime)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                var dataMigrationService = new DataMigrationService();
                dataMigrationService.Store(playerStatistic);

                totalTime += sw.ElapsedMilliseconds;
                Console.WriteLine($"Migrated rows of PlayerStatistic: {playerStatistic.Count} [{sw.ElapsedMilliseconds} ms]");
            }
            catch (Exception e)
            {
                WriteException(e);
                throw new OperationCanceledException("Migration aborted.");
            }

            playerStatistic.Clear();

            GC.Collect();
        }

        static Dictionary<PlayersKey, Players> ReadPlayers()
        {
            using (var session = ModelEntities.OpenSession())
            {
                return session.Query<Players>()
                        .ToDictionary(x => new PlayersKey { PlayerName = x.Playername, PokersiteId = x.PokersiteId },
                                               new LambdaComparer<PlayersKey>((x, y) => x.PlayerName == y.PlayerName && x.PokersiteId == y.PokersiteId));
            }
        }

        private class PlayersKey
        {
            public string PlayerName { get; set; }

            public int PokersiteId { get; set; }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + PlayerName.GetHashCode();
                    hashcode = (hashcode * 31) + PokersiteId;
                    return hashcode;
                }
            }

            public override bool Equals(object obj)
            {
                var playerKey = obj as PlayersKey;

                return Equals(playerKey);
            }

            public bool Equals(PlayersKey obj)
            {
                if (obj == null)
                {
                    return false;
                }

                return PlayerName == obj.PlayerName && PokersiteId == obj.PokersiteId;
            }
        }

        private class DataMigrationService : DataService
        {
            protected override string playersPath
            {
                get
                {
                    return StringFormatter.GetPlayerStatisticDataFolderPath();
                }
            }
        }
    }
}