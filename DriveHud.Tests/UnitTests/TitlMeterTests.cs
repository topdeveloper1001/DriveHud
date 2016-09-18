//-----------------------------------------------------------------------
// <copyright file="TitlMeterTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using NUnit.Framework;
using System.Linq;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class TitlMeterTests
    {
        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void TestTiltMeterValueIsIncreasedIfPlayerSawShowdownButLost(bool usePlusOperator)
        {
            var playerstatistic1 = new Playerstatistic();

            var playerstatistic2 = new Playerstatistic
            {
                Sawshowdown = 1,
                Wonhand = 0,
                Wonshowdown = 0
            };

            if (usePlusOperator)
            {
                playerstatistic1 += playerstatistic2;
            }
            else
            {
                playerstatistic1.Add(playerstatistic2);
            }

            Assert.That(playerstatistic1.TiltMeterPermanent, Is.EqualTo(1));
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void TestTiltMeterValueIsIncreasedIfPlayerSawFlopButFolded(bool usePlusOperator)
        {
            var playerstatistic1 = new Playerstatistic();

            var playerstatistic2 = new Playerstatistic
            {
                Sawflop = 1,
                PlayerFolded = true
            };

            if (usePlusOperator)
            {
                playerstatistic1 += playerstatistic2;
            }
            else
            {
                playerstatistic1.Add(playerstatistic2);
            }

            var tiltMeter = playerstatistic1.TiltMeterTemporaryHistory.Dequeue();

            Assert.That(tiltMeter, Is.EqualTo(1));
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void TestTiltMeterValueIsIncreasedWhenPotWasBetween50and100BBPlayerLost(bool usePlusOperator)
        {
            var playerstatistic1 = new Playerstatistic();

            var playerstatistic2 = new Playerstatistic
            {
                Pot = 1000,
                BigBlind = 20,
                PlayerFolded = true,
                Wonhand = 0
            };

            if (usePlusOperator)
            {
                playerstatistic1 += playerstatistic2;
            }
            else
            {
                playerstatistic1.Add(playerstatistic2);
            }

            var tiltMeter = playerstatistic1.TiltMeterTemporaryHistory.Dequeue();

            Assert.That(tiltMeter, Is.EqualTo(1));
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void TestTiltMeterValueIsIncreasedWhenPotWasMoreThan100BBPlayerLost(bool usePlusOperator)
        {
            var playerstatistic1 = new Playerstatistic();

            var playerstatistic2 = new Playerstatistic
            {
                Pot = 1000,
                BigBlind = 5,
                PlayerFolded = true,
                Wonhand = 0
            };

            if (usePlusOperator)
            {
                playerstatistic1 += playerstatistic2;
            }
            else
            {
                playerstatistic1.Add(playerstatistic2);
            }

            var tiltMeter = playerstatistic1.TiltMeterTemporaryHistory.Dequeue();

            Assert.That(tiltMeter, Is.EqualTo(2));
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void TestTiltMeterValueIsIncreasedWhenFour3BetInRow(bool usePlusOperator)
        {
            var playerstatistic1 = new Playerstatistic();

            for (var i = 0; i < 4; i++)
            {
                var playerstatistic2 = new Playerstatistic
                {
                    Didthreebet = 1
                };

                if (usePlusOperator)
                {
                    playerstatistic1 += playerstatistic2;
                }
                else
                {
                    playerstatistic1.Add(playerstatistic2);
                }
            }

            var tiltMeter = playerstatistic1.TiltMeterTemporaryHistory.Last();

            Assert.That(tiltMeter, Is.EqualTo(2));
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void TestTiltMeterValueIsIncreasedWhenFivePFRInRow(bool usePlusOperator)
        {
            var playerstatistic1 = new Playerstatistic();

            for (var i = 0; i < 5; i++)
            {
                var playerstatistic2 = new Playerstatistic
                {
                    Pfrhands = 1
                };

                if (usePlusOperator)
                {
                    playerstatistic1 += playerstatistic2;
                }
                else
                {
                    playerstatistic1.Add(playerstatistic2);
                }
            }

            var tiltMeter = playerstatistic1.TiltMeterTemporaryHistory.Last();

            Assert.That(tiltMeter, Is.EqualTo(2));
        }       
    }
}