using DriveHUD.Entities;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Practices.ServiceLocation;
using Model.Mapping;
using Model.Settings;
using NHibernate;

namespace Model
{
    public static class ModelEntities
    {
        private static ISessionFactory sessionFactory;

        public static ISessionFactory CreateSessionFactory()
        {
            if (sessionFactory == null || sessionFactory.IsClosed)
            {
                lock (factorylock)
                {
                    if (sessionFactory == null || sessionFactory.IsClosed)
                    {
                        var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().DatabaseSettings;

                        sessionFactory = Fluently.Configure()
                            .Database(PostgreSQLConfiguration.Standard
                                .ConnectionString(
                                    c => c.Is(StringFormatter.GetConnectionString(settings.Server, settings.Port, settings.Database, settings.User, settings.Password))))
                            .Mappings(x => x.FluentMappings.AddFromAssemblyOf<HandhistoryMap>())
                            .BuildSessionFactory();
                    }

                }
            }

            return sessionFactory;
        }

        private static ISessionFactory sqliteSessionFactory;

        public static ISessionFactory CreateSQLiteSessionFactory()
        {
            if (sqliteSessionFactory == null || sqliteSessionFactory.IsClosed)
            {
                lock (factorylock)
                {
                    if (sqliteSessionFactory == null || sqliteSessionFactory.IsClosed)
                    {
                        sqliteSessionFactory = Fluently.Configure()
                            .Database(SQLiteConfiguration.Standard.ConnectionString(StringFormatter.GetSQLiteConnectionString()))
                            .Mappings(x => x.FluentMappings.AddFromAssemblyOf<DriveHUD.Entities.Mapping.TournamentsMap>())
                            .BuildSessionFactory();
                    }

                }
            }

            return sqliteSessionFactory;
        }

        static readonly object factorylock = new object();

        public static ISession OpenPostgreSQLSession()
        {
            return CreateSessionFactory().OpenSession();
        }

        public static ISession OpenSession()
        {
            return CreateSQLiteSessionFactory().OpenSession();
        }

        public static IStatelessSession OpenStatelessSession()
        {
            return CreateSQLiteSessionFactory().OpenStatelessSession();
        }
    }
}