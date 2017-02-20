//-----------------------------------------------------------------------
// <copyright file="FileImporterTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Tests.IntegrationTests.Base;
using DriveHUD.Common.Progress;
using DriveHUD.Entities;
using DriveHUD.Importers;
using Model;
using NHibernate.Linq;
using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace DriveHud.Tests.IntegrationTests.Importers
{
    /// <summary>
    /// Importer integration tests
    /// </summary>
    [TestFixture]
    public class FileImporterTests : BaseDatabaseTest
    {
        /// <summary>
        /// Initialize environment for test
        /// </summary>
        [TestFixtureSetUp]
        public void SetUp()
        {
            Initalize();
            FillDatabase();
        }

        /// <summary>
        /// Freeing resources
        /// </summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
            RemoveDatabase();
        }

        [Test]
        [TestCase("6995792", 9)]
        public void TournamentsAreImportedForEachPlayer(string tournamentNumber, int expectedCount)
        {
            FillDatabase();

            using (var perfScope = new PerformanceMonitor("TournamentsAreImportedForEachPlayer"))
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var tournaments = session.Query<Tournaments>()
                        .Where(x => x.Tourneynumber.Equals(tournamentNumber, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    Assert.That(tournaments.Count, Is.EqualTo(9));
                }
            }
        }

        /// <summary>
        /// Fill database with data from test hand history files
        /// </summary>
        protected virtual void FillDatabase()
        {
            using (var perfScope = new PerformanceMonitor("FillDatabase"))
            {
                var progress = Substitute.For<IDHProgress>();
                var fileImporter = new FileImporter();

                foreach (var handHistoryFile in TestCaseDataSet)
                {
                    var handHistoryFileInfo = new FileInfo(handHistoryFile);

                    Assert.That(handHistoryFileInfo.Exists, $"{handHistoryFile} doesn't exists. Please check.");

                    fileImporter.Import(handHistoryFileInfo, progress);
                }
            }
        }

        /// <summary>
        /// Set of hh files to fill DB
        /// </summary>
        private string[] TestCaseDataSet = new[]
        {
            @"d:\Git\Temp\HH20170216 T6995792-G39795657-Full-Sum.txt"
        };
    }
}