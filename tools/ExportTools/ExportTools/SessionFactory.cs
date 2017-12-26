using DriveHUD.Entities.Mapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace ExportTools
{
    internal class SessionFactory
    {
        private static readonly object factorylock = new object();

        private ISessionFactory sessionFactory;

        private ISessionFactory sqliteSessionFactory;

        private static SessionFactory instance;

        public static SessionFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SessionFactory();
                }

                return instance;
            }
        }

        public ISessionFactory CreateSQLiteSessionFactory()
        {
            if (sqliteSessionFactory == null || sqliteSessionFactory.IsClosed)
            {
                lock (factorylock)
                {
                    if (sqliteSessionFactory == null || sqliteSessionFactory.IsClosed)
                    {
                        sqliteSessionFactory = Fluently.Configure()
                            .Database(SQLiteConfiguration.Standard.ConnectionString(Globals.GetSQLiteConnectionString()))
                            .Mappings(x => x.FluentMappings.AddFromAssemblyOf<TournamentsMap>())
                            .BuildSessionFactory();
                    }

                }
            }

            return sqliteSessionFactory;
        }

        public ISession OpenSession()
        {
            return CreateSQLiteSessionFactory().OpenSession();
        }

        public IStatelessSession OpenStatelessSession()
        {
            return CreateSQLiteSessionFactory().OpenStatelessSession();
        }
    }
}