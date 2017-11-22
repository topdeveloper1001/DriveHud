//-----------------------------------------------------------------------
// <copyright file="GGNTournamentReaderTests.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Common.Log;
using DriveHUD.Importers.GGNetwork;
using NUnit.Framework;
using System.Linq;
using System.Threading;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class GGNTournamentReaderTests : BaseIndicatorsTests
    {
        [Test]
        public void TournamentReaderReadAllTournaments()
        {
            using (var pm = new PerformanceMonitor(nameof(TournamentReaderReadAllTournaments)))
            {
                var tournamentReader = new GGNTournamentReader();

                var readAllTournamentsTask = tournamentReader.ReadAllTournamentsAsync(CancellationToken.None);
                readAllTournamentsTask.Wait();

                var tournaments = readAllTournamentsTask.Result;

                Assert.That(tournaments.Count(), Is.GreaterThan(0));
            }
        }
    }
}