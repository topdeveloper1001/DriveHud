//-----------------------------------------------------------------------
// <copyright file="IPokerBovadaFastParserTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.FastParser.IPoker;
using NUnit.Framework;
using System.Linq;

namespace DriveHud.Tests.IntegrationTests.Parsers.IPoker
{
    [TestFixture]
    class IPokerBovadaFastParserTests : IPokerBaseTests
    {
        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-positions.xml", "P2_127068GC", 0, HandActionType.FOLD, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-positions.xml", "Hero", 0, HandActionType.FOLD, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-positions.xml", "P4_753926TU", 0, HandActionType.FOLD, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-positions.xml", "P5_530485GH", 0, HandActionType.FOLD, Street.Preflop, 1)]
        public void ZoneActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, int numberOfActions)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions.Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.HandActionType == handActionType && x.Amount == amount).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-sb-allin-uncalled-bet.xml", "Hero", -6320, HandActionType.ALL_IN, HandActionType.RAISE, Street.Preflop, 1)]
        public void AllInActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType,
            HandActionType sourceHandActionType, Street street, int numberOfActions)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions
                .OfType<AllInAction>()
                .Where(x => x.Street == street &&
                    x.PlayerName.Equals(playerName) &&
                    x.HandActionType == handActionType &&
                    x.SourceActionType == sourceHandActionType &&
                    x.Amount == amount).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-sb-allin-uncalled-bet.xml", "P6_391468UT", -15, HandActionType.SMALL_BLIND, true, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-sb-allin-uncalled-bet.xml", "Hero", 6120, HandActionType.UNCALLED_BET, false, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-sb-allin-uncalled-bet.xml", "Hero", 370, HandActionType.WINS, false, Street.Summary, 1)]
        public void ActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType,
          bool isAllIn, Street street, int numberOfActions)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions
                .Where(x => x.Street == street &&
                    x.PlayerName.Equals(playerName) &&
                    x.HandActionType == handActionType &&
                    x.IsAllIn == isAllIn &&
                    x.Amount == amount).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions));
        }

        protected override IHandHistoryParser CreateParser()
        {
            return new IPokerBovadaFastParserImpl();
        }
    }
}