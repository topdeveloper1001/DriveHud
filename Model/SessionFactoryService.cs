//-----------------------------------------------------------------------
// <copyright file="SessionFactoryService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Practices.ServiceLocation;
using Model.Mapping;
using Model.Settings;
using NHibernate;

namespace Model
{
    internal class SessionFactoryService : ISessionFactoryService
    {
        private static readonly object factorylock = new object();

        private ISessionFactory sessionFactory;

        public ISessionFactory CreateSessionFactory()
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

        private ISessionFactory sqliteSessionFactory;

        public ISessionFactory CreateSQLiteSessionFactory()
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