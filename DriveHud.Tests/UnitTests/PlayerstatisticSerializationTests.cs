//-----------------------------------------------------------------------
// <copyright file="PlayerstatisticSerializationTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Entities;
using NUnit.Framework;
using ProtoBuf;
using System.IO;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class PlayerstatisticSerializationTests
    {
        [TestCase(EnumPosition.SB)]
        [TestCase(EnumPosition.BB)]
        [TestCase(EnumPosition.BTN)]
        [TestCase(EnumPosition.CO)]
        [TestCase(EnumPosition.EP)]
        [TestCase(EnumPosition.MP)]
        [TestCase(EnumPosition.MP1)]
        [TestCase(EnumPosition.MP2)]
        [TestCase(EnumPosition.MP3)]
        [TestCase(EnumPosition.STRDL)]
        [TestCase(EnumPosition.Undefined)]
        [TestCase(EnumPosition.UTG)]
        [TestCase(EnumPosition.UTG_1)]
        [TestCase(EnumPosition.UTG_2)]
        public void PlayerstatisticPositionFieldCanBeSerializedAndDeserialized(EnumPosition position)
        {
            var playerStatistic = new Playerstatistic
            {
                FirstRaiserPosition = position,
                ThreeBettorPosition = position,
                FourBettorPosition = position,
            };

            byte[] bytes = null;

            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, playerStatistic);
                bytes = memoryStream.ToArray();
            }

            var actualStat = SerializationHelper.Deserialize<Playerstatistic>(bytes);

            Assert.Multiple(() =>
            {
                Assert.That(actualStat.ThreeBettorPosition, Is.EqualTo(playerStatistic.ThreeBettorPosition));
                Assert.That(actualStat.FirstRaiserPosition, Is.EqualTo(playerStatistic.FirstRaiserPosition));
                Assert.That(actualStat.FourBettorPosition, Is.EqualTo(playerStatistic.FourBettorPosition));
            });
        }
    }
}