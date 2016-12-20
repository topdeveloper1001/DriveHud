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

        static readonly object factorylock = new object();

        public static ISession OpenSession()
        {
            return CreateSessionFactory().OpenSession();
        }

        public static IStatelessSession OpenStatelessSession()
        {
            return CreateSessionFactory().OpenStatelessSession();
        }
    }
}