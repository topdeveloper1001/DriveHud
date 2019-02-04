//-----------------------------------------------------------------------
// <copyright file="LightIndicatorsTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Tests.UnitTests.Helpers;
using DriveHUD.Common.Extensions;
using DriveHUD.Entities;
using Model.Data;
using NUnit.Framework;
using ProtoBuf;
using System.Collections.Generic;
using System.IO;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class LightIndicatorsTests : BaseIndicatorsTests
    {
        [TestCase(@"..\..\UnitTests\TestData\testdata.stat")]
        public void TestLightIndicators(string file)
        {
            var playerStatistic = DataServiceHelper.GetPlayerStatisticFromFile(file);

            var indicators = new TestIndicators(playerStatistic);
            var lightIndicators = new LightIndicators(playerStatistic);

            Assert.Multiple(() =>
            {
                Assert.That(lightIndicators.BB, Is.EqualTo(indicators.BB), nameof(HudLightIndicators.BB));
                Assert.That(lightIndicators.EVBB, Is.EqualTo(indicators.EVBB), nameof(HudLightIndicators.EVBB));
                Assert.That(lightIndicators.SessionStart, Is.EqualTo(indicators.SessionStart), nameof(HudLightIndicators.SessionStart));
                Assert.That(lightIndicators.SessionLength, Is.EqualTo(indicators.SessionLength), nameof(HudLightIndicators.SessionLength));
                Assert.That(lightIndicators.UO_PFR_EP, Is.EqualTo(indicators.UO_PFR_EP), nameof(HudLightIndicators.UO_PFR_EP));
                Assert.That(lightIndicators.UO_PFR_MP, Is.EqualTo(indicators.UO_PFR_MP), nameof(HudLightIndicators.UO_PFR_MP));
                Assert.That(lightIndicators.UO_PFR_CO, Is.EqualTo(indicators.UO_PFR_CO), nameof(HudLightIndicators.UO_PFR_CO));
                Assert.That(lightIndicators.UO_PFR_BN, Is.EqualTo(indicators.UO_PFR_BN), nameof(HudLightIndicators.UO_PFR_BN));
                Assert.That(lightIndicators.UO_PFR_SB, Is.EqualTo(indicators.UO_PFR_SB), nameof(HudLightIndicators.UO_PFR_SB));
                Assert.That(lightIndicators.UO_PFR_BB, Is.EqualTo(indicators.UO_PFR_BB), nameof(HudLightIndicators.UO_PFR_BB));
                Assert.That(lightIndicators.ThreeBet_BB, Is.EqualTo(indicators.ThreeBet_BB), nameof(HudLightIndicators.ThreeBet_BB));
                Assert.That(lightIndicators.ThreeBet_SB, Is.EqualTo(indicators.ThreeBet_SB), nameof(HudLightIndicators.ThreeBet_SB));
                Assert.That(lightIndicators.ThreeBet_EP, Is.EqualTo(indicators.ThreeBet_EP), nameof(HudLightIndicators.ThreeBet_EP));
                Assert.That(lightIndicators.ThreeBet_MP, Is.EqualTo(indicators.ThreeBet_MP), nameof(HudLightIndicators.ThreeBet_MP));
                Assert.That(lightIndicators.ThreeBet_CO, Is.EqualTo(indicators.ThreeBet_CO), nameof(HudLightIndicators.ThreeBet_CO));
                Assert.That(lightIndicators.ThreeBet_BN, Is.EqualTo(indicators.ThreeBet_BN), nameof(HudLightIndicators.ThreeBet_BN));
                Assert.That(lightIndicators.FoldToThreeBet, Is.EqualTo(indicators.FoldToThreeBet), nameof(HudLightIndicators.FoldToThreeBet));
                Assert.That(lightIndicators.FourBet, Is.EqualTo(indicators.FourBet), nameof(HudLightIndicators.FourBet));
                Assert.That(lightIndicators.FourBetInBB, Is.EqualTo(indicators.FourBetInBB), nameof(HudLightIndicators.FourBetInBB));
                Assert.That(lightIndicators.FourBetInSB, Is.EqualTo(indicators.FourBetInSB), nameof(HudLightIndicators.FourBetInSB));
                Assert.That(lightIndicators.FourBetInEP, Is.EqualTo(indicators.FourBetInEP), nameof(HudLightIndicators.FourBetInEP));
                Assert.That(lightIndicators.FourBetInMP, Is.EqualTo(indicators.FourBetInMP), nameof(HudLightIndicators.FourBetInMP));
                Assert.That(lightIndicators.FourBetInCO, Is.EqualTo(indicators.FourBetInCO), nameof(HudLightIndicators.FourBetInCO));
                Assert.That(lightIndicators.FourBetInBTN, Is.EqualTo(indicators.FourBetInBTN), nameof(HudLightIndicators.FourBetInBTN));
                Assert.That(lightIndicators.VPIP_BB, Is.EqualTo(indicators.VPIP_BB), nameof(HudLightIndicators.VPIP_BB));
                Assert.That(lightIndicators.VPIP_SB, Is.EqualTo(indicators.VPIP_SB), nameof(HudLightIndicators.VPIP_SB));
                Assert.That(lightIndicators.VPIP_EP, Is.EqualTo(indicators.VPIP_EP), nameof(HudLightIndicators.VPIP_EP));
                Assert.That(lightIndicators.VPIP_MP, Is.EqualTo(indicators.VPIP_MP), nameof(HudLightIndicators.VPIP_MP));
                Assert.That(lightIndicators.VPIP_CO, Is.EqualTo(indicators.VPIP_CO), nameof(HudLightIndicators.VPIP_CO));
                Assert.That(lightIndicators.VPIP_BN, Is.EqualTo(indicators.VPIP_BN), nameof(HudLightIndicators.VPIP_BN));
                Assert.That(lightIndicators.ColdCall_BB, Is.EqualTo(indicators.ColdCall_BB), nameof(HudLightIndicators.ColdCall_BB));
                Assert.That(lightIndicators.ColdCall_SB, Is.EqualTo(indicators.ColdCall_SB), nameof(HudLightIndicators.ColdCall_SB));
                Assert.That(lightIndicators.ColdCall_EP, Is.EqualTo(indicators.ColdCall_EP), nameof(HudLightIndicators.ColdCall_EP));
                Assert.That(lightIndicators.ColdCall_MP, Is.EqualTo(indicators.ColdCall_MP), nameof(HudLightIndicators.ColdCall_MP));
                Assert.That(lightIndicators.ColdCall_CO, Is.EqualTo(indicators.ColdCall_CO), nameof(HudLightIndicators.ColdCall_CO));
                Assert.That(lightIndicators.ColdCall_BN, Is.EqualTo(indicators.ColdCall_BN), nameof(HudLightIndicators.ColdCall_BN));

                Assert.That(lightIndicators.HourOfHand, Is.EqualTo(indicators.HourOfHand), nameof(HudLightIndicators.HourOfHand));
            });
        }

        [TestCase(@"..\..\UnitTests\TestData\testdata.stat")]
        public void TestReportIndicators(string file)
        {
            var playerStatistic = DataServiceHelper.GetPlayerStatisticFromFile(file);

            var indicators = new TestIndicators(playerStatistic);
            var reportIndicator = new ReportIndicators(playerStatistic);

            Assert.That(reportIndicator.BB, Is.EqualTo(indicators.BB), nameof(HudLightIndicators.BB));
            Assert.That(reportIndicator.EVBB, Is.EqualTo(indicators.EVBB), nameof(HudLightIndicators.EVBB));
            Assert.That(reportIndicator.SessionStart, Is.EqualTo(indicators.SessionStart), nameof(HudLightIndicators.SessionStart));
            Assert.That(reportIndicator.SessionLength, Is.EqualTo(indicators.SessionLength), nameof(HudLightIndicators.SessionLength));
            Assert.That(reportIndicator.UO_PFR_EP, Is.EqualTo(indicators.UO_PFR_EP), nameof(HudLightIndicators.UO_PFR_EP));
            Assert.That(reportIndicator.UO_PFR_MP, Is.EqualTo(indicators.UO_PFR_MP), nameof(HudLightIndicators.UO_PFR_MP));
            Assert.That(reportIndicator.UO_PFR_CO, Is.EqualTo(indicators.UO_PFR_CO), nameof(HudLightIndicators.UO_PFR_CO));
            Assert.That(reportIndicator.UO_PFR_BN, Is.EqualTo(indicators.UO_PFR_BN), nameof(HudLightIndicators.UO_PFR_BN));
            Assert.That(reportIndicator.UO_PFR_SB, Is.EqualTo(indicators.UO_PFR_SB), nameof(HudLightIndicators.UO_PFR_SB));
            Assert.That(reportIndicator.UO_PFR_BB, Is.EqualTo(indicators.UO_PFR_BB), nameof(HudLightIndicators.UO_PFR_BB));
            Assert.That(reportIndicator.HourOfHand, Is.EqualTo(indicators.HourOfHand), nameof(HudLightIndicators.HourOfHand));
            Assert.That(reportIndicator.Statistics.Count, Is.EqualTo(playerStatistic.Count), "Count");
        }

        [TestCase(@"..\..\UnitTests\TestData\testdata.stat")]
        public void TestLightIndicatorProtobufSerialization(string file)
        {
            var playerStatistic = DataServiceHelper.GetPlayerStatisticFromFile(file);
            var lightIndicators = new LightIndicators(playerStatistic);

            Assert.DoesNotThrow(() =>
            {
                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, lightIndicators);
                }
            }, "LightIndicator must be serializable");
        }       

        private class TestIndicators : Indicators
        {
            public TestIndicators(IEnumerable<Playerstatistic> playerStatistic) : base(playerStatistic)
            {
            }

            public override decimal ThreeBetMPvsEP => throw new System.NotImplementedException();

            public override decimal ThreeBetCOvsEP => throw new System.NotImplementedException();

            public override decimal ThreeBetCOvsMP => throw new System.NotImplementedException();

            public override decimal ThreeBetBTNvsEP => throw new System.NotImplementedException();

            public override decimal ThreeBetBTNvsMP => throw new System.NotImplementedException();

            public override decimal ThreeBetBTNvsCO => throw new System.NotImplementedException();

            public override decimal ThreeBetSBvsEP => throw new System.NotImplementedException();

            public override decimal ThreeBetSBvsMP => throw new System.NotImplementedException();

            public override decimal ThreeBetSBvsCO => throw new System.NotImplementedException();

            public override decimal ThreeBetSBvsBTN => throw new System.NotImplementedException();

            public override decimal ThreeBetBBvsEP => throw new System.NotImplementedException();

            public override decimal ThreeBetBBvsMP => throw new System.NotImplementedException();

            public override decimal ThreeBetBBvsCO => throw new System.NotImplementedException();

            public override decimal ThreeBetBBvsBTN => throw new System.NotImplementedException();

            public override decimal ThreeBetBBvsSB => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInEPvs3BetMP => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInEPvs3BetCO => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInEPvs3BetBTN => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInEPvs3BetSB => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInEPvs3BetBB => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInMPvs3BetCO => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInMPvs3BetBTN => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInMPvs3BetSB => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInMPvs3BetBB => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInCOvs3BetBTN => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInCOvs3BetSB => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInCOvs3BetBB => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInBTNvs3BetSB => throw new System.NotImplementedException();

            public override decimal FoldTo3BetInBTNvs3BetBB => throw new System.NotImplementedException();

            public override decimal CheckRaiseFlopAsPFR => throw new System.NotImplementedException();
        }
    }
}