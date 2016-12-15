using DriveHUD.Entities;
using Model;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Diagnostics;
using System.Linq;

namespace DriveHUD.DBMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();

            using (var session = ModelEntities.OpenPostgreSQLSession())
            {
                TestSelection<Players>(session);
                TestSelection<HandHistoryRecord>(session);
            }

            using (var session = ModelEntities.OpenSession())
            {
                TestSelection<Players>(session);
                TestSelection<HandHistoryRecord>(session);
            }

            //Migrate();

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        static void TestSelection<T>(ISession session)
        {
            Console.WriteLine("Starting test");

            var sw = new Stopwatch();           
            long totalTime = 0;

            sw.Restart();
            var entities = session.Query<T>().Take(1000000).ToArray();
            sw.Stop();

            Console.WriteLine($"Read total {typeof(T).Name}: {entities.Length} [{sw.ElapsedMilliseconds} ms]");
            totalTime += sw.ElapsedMilliseconds;

            GC.Collect();    
        }

        static void Migrate()
        {
            var sw = new Stopwatch();
            long rows = 0;
            long totalTime = 0;

            Console.WriteLine("Starting migration");

            try
            {
                MigrateTable<Players>(sw, x => x.PlayerId, ref rows, ref totalTime);
                MigrateTable<Gametypes>(sw, x => x.GametypeId, ref rows, ref totalTime);
                MigrateTable<Handhistory>(sw, x => x.HandhistoryId, ref rows, ref totalTime);
                MigrateTable<Handnotes>(sw, x => x.HandNoteId, ref rows, ref totalTime);
                MigrateTable<HandHistoryRecord>(sw, x => x.Id, ref rows, ref totalTime);
                MigrateTable<Playernotes>(sw, x => x.PlayerNoteId, ref rows, ref totalTime);
                MigrateTable<Tournaments>(sw, x => x.TourneydataId, ref rows, ref totalTime);
                MigrateTable<Playerstatistic>(sw, x => x.CompiledplayerresultsId, ref rows, ref totalTime);
                Console.WriteLine($"Rows migrated: {rows} [{totalTime} ms]");
                Console.WriteLine("Migration completed");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Migration failed: {e.Message}");
            }
        }

        static void MigrateTable<T>(Stopwatch sw, Func<T, object> getId, ref long rows, ref long totalTime)
        {
            T[] entities = null;

            using (var session = ModelEntities.OpenPostgreSQLSession())
            {
                sw.Restart();
                entities = session.Query<T>().ToArray();
                sw.Stop();

                Console.WriteLine($"Read total {typeof(T).Name}: {entities.Length} [{sw.ElapsedMilliseconds} ms]");
                totalTime += sw.ElapsedMilliseconds;
            }

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

            GC.Collect();
        }
    }
}