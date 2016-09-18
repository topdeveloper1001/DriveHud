using System.Configuration;
using DriveHUD.Inserter.Domain;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace DriveHUD.Inserter
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
                        sessionFactory = Fluently.Configure()
                            .Database(PostgreSQLConfiguration.Standard
                                .ConnectionString(
                                    c => c.Is(ConfigurationManager.ConnectionStrings["DriveHudDB"].ConnectionString)))
                            .Mappings(x => x.FluentMappings.AddFromAssemblyOf<Hands>())
                            .BuildSessionFactory();
                    }

                }
            }
            return sessionFactory;
        }

        static readonly object factorylock = new object();

        public static ISession OpenSession()
        {
            return CreateSessionFactory().OpenSession();
        }
    }


}
