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
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-positions.xml", "P2_127068GC", 0, HandActionType.FOLD, Street.Preflop, 1, 3)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-positions.xml", "Hero", 0, HandActionType.FOLD, Street.Preflop, 1, 4)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-positions.xml", "P4_753926TU", 0, HandActionType.FOLD, Street.Preflop, 1, 5)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-positions.xml", "P5_530485GH", 0, HandActionType.FOLD, Street.Preflop, 1, 6)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-2-players.xml", "Hero", -9.5, HandActionType.RAISE, Street.Preflop, 1, 3)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-2-players.xml", "P1-243595JX", -14, HandActionType.RAISE, Street.Preflop, 1, 4)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-2-players.xml", "P1-243595JX", 0, HandActionType.CHECK, Street.Flop, 1, 7)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-2-players.xml", "Hero", -40, HandActionType.BET, Street.Flop, 1, 8)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-zone-2-players.xml", "P1-243595JX", -40, HandActionType.CALL, Street.Flop, 1, 9)]
        public void ZoneActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, int numberOfActions, int actionNo)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions
                .Select((x, index) => new { Action = x, Number = index + 1 })
                .Where(x => x.Action.Street == street && x.Action.PlayerName.Equals(playerName) &&
                    x.Action.HandActionType == handActionType && x.Action.Amount == amount && x.Number == actionNo).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ign-sb-allin-uncalled-bet.xml", "Hero", -6320, HandActionType.ALL_IN, HandActionType.RAISE, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\BOL-NHL-6-max-25NL-AllIn-Call.xml", "ChiTownMCA", -15.8, HandActionType.ALL_IN, HandActionType.CALL, Street.River, 1)]
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
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\BOL-NHL-6-max-25NL-AllIn-Call.xml", "punisher1127", 0, HandActionType.WINS, false, Street.Summary, 0)]
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