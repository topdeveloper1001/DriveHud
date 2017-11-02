//-----------------------------------------------------------------------
// <copyright file="IgnitionTableTitleTests.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.Bovada;
using NUnit.Framework;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class IgnitionTableTitleTests
    {
        [Test]
        [TestCase("Jackpot Sit & Go $2.00 10/20 No limit Hold'em  Tournament 19320964", true)]
        [TestCase("400/800, 80 Ante No limit Hold'em - $2,000 Guaranteed (Turbo SS) -  TBL #1", true)]
        [TestCase("#19318050 (Turbo)", false)]
        [TestCase("Poker", false)]
        [TestCase("Poker Table", false)]
        [TestCase("Ignition Poker", false)]
        [TestCase("Tournament #19308657 GSPO5 Mini Main Event Satellite 1 Seat Gtd info", false)]
        [TestCase("Ignition Casino - Poker Lobby", false)]
        [TestCase("$0.25/$0.50 No limit Hold'em", true)]
        public void TestIgnitionTableTitleIsValid(string title, bool expected)
        {
            var IgnitionTableTitle = new IgnitionTableTitle(title);
            Assert.That(IgnitionTableTitle.IsValid, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("Jackpot Sit & Go $2.00 10/20 No limit Hold'em  Tournament 19320964", true)]
        [TestCase("400/800, 80 Ante No limit Hold'em - $2,000 Guaranteed (Turbo SS) -  TBL #1", true)]
        [TestCase("$0.25/$0.50 No limit Hold'em", false)]
        public void TestIgnitionTableTitleIsTournament(string title, bool expected)
        {
            var IgnitionTableTitle = new IgnitionTableTitle(title);
            Assert.That(IgnitionTableTitle.IsTournament, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("Jackpot Sit & Go $2.00 10/20 No limit Hold'em  Tournament 19320964", "No limit Hold'em")]
        [TestCase("400/800, 80 Ante No limit Hold'em - $2,000 Guaranteed (Turbo SS) -  TBL #1", "No limit Hold'em")]
        [TestCase("$0.25/$0.50 No limit Hold'em", "No limit Hold'em")]
        public void TestIgnitionTableTitleGameType(string title, string gameType)
        {
            var IgnitionTableTitle = new IgnitionTableTitle(title);
            Assert.That(IgnitionTableTitle.GameType, Is.EqualTo(gameType));
        }

        [Test]
        [TestCase("Jackpot Sit & Go $2.00 10/20 No limit Hold'em  Tournament 19320964", "Jackpot Sit & Go $2.00")]
        [TestCase("400/800, 80 Ante No limit Hold'em - $2,000 Guaranteed (Turbo SS) -  TBL #1", "$2,000 Guaranteed (Turbo SS)")]
        [TestCase("50/100 No limit Hold'em - Satellite To Any $7 + $0.70 -  TBL #13", "Satellite To Any $7 + $0.70")]
        public void TestIgnitionTableTitleTableName(string title, string tableName)
        {
            var IgnitionTableTitle = new IgnitionTableTitle(title);
            Assert.That(IgnitionTableTitle.TableName, Is.EqualTo(tableName));
        }

        [Test]
        [TestCase("Jackpot Sit & Go $2.00 10/20 No limit Hold'em  Tournament 19320964", "10/20")]
        [TestCase("400/800, 80 Ante No limit Hold'em - $2,000 Guaranteed (Turbo SS) -  TBL #1", "400/800")]
        [TestCase("50/100 No limit Hold'em - Satellite To Any $7 + $0.70 -  TBL #13", "50/100")]
        [TestCase("$0.25/$0.50 No limit Hold'em", "$0.25/$0.50")]
        public void TestIgnitionTableTitleStacks(string title, string stacks)
        {
            var IgnitionTableTitle = new IgnitionTableTitle(title);
            Assert.That(IgnitionTableTitle.Stacks, Is.EqualTo(stacks));
        }

        [Test]
        [TestCase("Jackpot Sit & Go $2.00 10/20 No limit Hold'em  Tournament 19320964", "1")]
        [TestCase("400/800, 80 Ante No limit Hold'em - $2,000 Guaranteed (Turbo SS) -  TBL #1", "1")]
        [TestCase("50/100 No limit Hold'em - Satellite To Any $7 + $0.70 -  TBL #13", "13")]
        public void TestTableTitleTableId(string title, string tableId)
        {
            var IgnitionTableTitle = new IgnitionTableTitle(title);
            Assert.That(IgnitionTableTitle.TableId, Is.EqualTo(tableId));
        }

        [Test]
        [TestCase("Jackpot Sit & Go $2.00 10/20 No limit Hold'em  Tournament 19320964", "19320964")]
        [TestCase("400/800, 80 Ante No limit Hold'em - $2,000 Guaranteed (Turbo SS) -  TBL #1", null)]
        public void TestTableTitleTournamentId(string title, string tournamentId)
        {
            var IgnitionTableTitle = new IgnitionTableTitle(title);
            Assert.That(IgnitionTableTitle.TournamentId, Is.EqualTo(tournamentId));
        }

        [Test]
        [TestCase("Jackpot Sit & Go $2.00 10/20 No limit Hold'em  Tournament 19320964", null)]
        [TestCase("400/800, 80 Ante No limit Hold'em - $2,000 Guaranteed (Turbo SS) -  TBL #1", "80")]
        public void TestTableTitleAnte(string title, string ante)
        {
            var IgnitionTableTitle = new IgnitionTableTitle(title);
            Assert.That(IgnitionTableTitle.Ante, Is.EqualTo(ante));
        }

        [Test]
        [TestCase("Jackpot Sit & Go $2.00 10/20 No limit Hold'em  Tournament 19320964", true)]
        [TestCase("400/800, 80 Ante No limit Hold'em - $2,000 Guaranteed (Turbo SS) -  TBL #1", true)]
        [TestCase("$0.25/$0.50 No limit Hold'em", false)]
        public void TestTableTitleTournament(string title, bool expected)
        {
            var IgnitionTableTitle = new IgnitionTableTitle(title);
            Assert.That(IgnitionTableTitle.IsTournament, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("Jackpot Sit & Go $2.00 10/20 No limit Hold'em  Tournament 19320964", false)]
        [TestCase("400/800, 80 Ante No limit Hold'em - $2,000 Guaranteed (Turbo SS) -  TBL #1", false)]
        [TestCase("Play 2/4 No limit Hold'em", true)]
        public void TestTableTitlePlayMoney(string title, bool expected)
        {
            var IgnitionTableTitle = new IgnitionTableTitle(title);
            Assert.That(IgnitionTableTitle.IsPlayMoney, Is.EqualTo(expected));
        }
    }
}