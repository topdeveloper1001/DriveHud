//-----------------------------------------------------------------------
// <copyright file="HudLightIndicatorTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Data;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DriveHud.Tests.IntegrationTests.Importers
{
    /// <summary>
    /// HudLightIndicators integration tests
    /// </summary>
    [TestFixture]
    class HudLightIndicatorsTests : PlayerStatisticBaseTests
    {
        protected override string TestDataFolder
        {
            get
            {
                return @"..\..\IntegrationTests\Importers\TestData\Indicators";
            }
        }

        [TestCase(@"Hero-RaisedLimp-1.txt", EnumPokerSites.PokerStars, "Hero", 100, 1, 1)]
        [TestCase(@"Hero-RaisedLimp-1.txt", EnumPokerSites.PokerStars, "FrAnWaN", 0, 0, 0)]
        [TestCase(@"Hero-RaisedLimp-2.txt", EnumPokerSites.PokerStars, "Hero", 0, 0, 1)]
        public void RaiseLimpersStatTest(string fileName, EnumPokerSites pokerSite, string playerName, decimal expected, int occurredExpected, int couldOccurredExpected)
        {
            AssertThatIndicatorIsCalculated(x => x.RaiseLimpersObject, fileName, pokerSite, playerName, expected, occurredExpected, couldOccurredExpected);
        }

        [TestCase(@"Hero-RaisedLimp-1.txt", EnumPokerSites.PokerStars, "Hero", 100, 1, 1)]
        public void RaiseLimpersInBNStatTest(string fileName, EnumPokerSites pokerSite, string playerName, decimal expected, int occurredExpected, int couldOccurredExpected)
        {
            AssertThatIndicatorIsCalculated(x => x.RaiseLimpersInBNObject, fileName, pokerSite, playerName, expected, occurredExpected, couldOccurredExpected);
        }

        [TestCase(@"Hero-RaisedLimp-2.txt", EnumPokerSites.PokerStars, "Hero", 0, 0, 1)]
        public void RaiseLimpersInSBStatTest(string fileName, EnumPokerSites pokerSite, string playerName, decimal expected, int occurredExpected, int couldOccurredExpected)
        {
            AssertThatIndicatorIsCalculated(x => x.RaiseLimpersInSBObject, fileName, pokerSite, playerName, expected, occurredExpected, couldOccurredExpected);
        }

        [TestCase(@"Hero-FoldedTo3BetInBTNvs3BetSB-1.txt", EnumPokerSites.PokerStars, "Hero", 100, 1, 1)]
        public void FoldTo3BetInBTNvs3BetSBObjectStatTest(string fileName, EnumPokerSites pokerSite, string playerName, decimal expected, int occurredExpected, int couldOccurredExpected)
        {
            AssertThatIndicatorIsCalculated(x => x.FoldTo3BetInBTNvs3BetSBObject, fileName, pokerSite, playerName, expected, occurredExpected, couldOccurredExpected);
        }

        protected virtual void AssertThatIndicatorIsCalculated(Expression<Func<HudLightIndicators, StatDto>> expression, string fileName, EnumPokerSites pokerSite, string playerName, decimal expected, int occurredExpected, int couldOccurredExpected, [CallerMemberName] string method = "UnknownMethod")
        {
            using (var perfScope = new PerformanceMonitor(method))
            {
                var indicator = new HudLightIndicators();

                Playerstatistic playerstatistic = null;

                var playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
                playerStatisticRepository.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");

                indicator.AddStatistic(playerstatistic);

                var getStat = expression.Compile();

                var actualStatDto = getStat(indicator);
                var expectedStatDto = new StatDto
                {
                    Value = expected,
                    Occurred = occurredExpected,
                    CouldOccurred = couldOccurredExpected
                };

                AssertionUtils.AssertStatDto(actualStatDto, expectedStatDto);
            }
        }
    }
}