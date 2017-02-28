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
using Model;
using NHibernate;
using System;

namespace DriveHud.Tests.IntegrationTests.Base
{
    public class SessionFactoryTestService : ISessionFactoryService
    {
        private static readonly object factorylock = new object();

        public ISessionFactory CreateSessionFactory()
        {
            throw new NotSupportedException("Only SQLite is supported in tests.");
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
                            .Database(SQLiteConfiguration.Standard.ConnectionString(DatabaseHelper.GetConnectionString()))
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