//-----------------------------------------------------------------------
// <copyright file="PacketManagerTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Tests.TcpImportersTests;
using DriveHUD.Common.Extensions;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.PokerKing;
using DriveHUD.Importers.PokerKing.Model;
using Microsoft.QualityTools.Testing.Fakes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Fakes;
using System.IO;
using System.Linq;
using System.Net;

namespace DriveHud.Tests.PKTests
{
    [TestFixture]
    class PokerKingPacketManagerTests : PacketManagerTest
    {
        protected override string TestDataFolder => "TcpImportersTests\\PKTests\\TestData";

        [TestCase(@"Packets\NoticePlayerActionTurnPacket.txt", true)]
        public void PacketIsStartingPacketTest(string file, bool expected)
        {
            var bytes = ReadPacketFile(file);
            var packetManager = new PokerKingPacketManager();

            var actual = packetManager.IsStartingPacket(bytes);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(@"Packets\NoticePlayerActionTurnPacket.txt")]
        public void TryParsePacketTest(string file)
        {
            var bytes = ReadPacketFile(file);
            var packetManager = new PokerKingPacketManager();

            var capturedPacket = new CapturedPacket
            {
                Bytes = bytes,
                CreatedTimeStamp = DateTime.Parse("08/02/2018 12:28:28"),
                Destination = new IPEndPoint(IPAddress.Parse("192.168.0.105"), 27633),
                Source = new IPEndPoint(IPAddress.Parse("47.52.92.161"), 9188),
                SequenceNumber = 1962805251
            };

            var result = packetManager.TryParse(capturedPacket, out PokerKingPackage actual);

            Assert.IsTrue(result);
            Assert.IsNotNull(actual);
        }

        [TestCase(@"Packets\NoticePlayerActionTurnPacket.txt", @"Packets\NoticePlayerActionTurnPacket.json")]
        public void DeserializationTest(string file, string jsonFile)
        {
            var bytes = ReadPacketFile(file);
            var packetManager = new PokerKingPacketManager();

            var capturedPacket = new CapturedPacket
            {
                Bytes = bytes,
                CreatedTimeStamp = DateTime.Parse("08/02/2018 12:28:28"),
                Destination = new IPEndPoint(IPAddress.Parse("192.168.0.105"), 27633),
                Source = new IPEndPoint(IPAddress.Parse("47.52.92.161"), 9188),
                SequenceNumber = 1962805251
            };

            var result = packetManager.TryParse(capturedPacket, out PokerKingPackage package);

            var noticePlayerActionTurnActual = SerializationHelper.Deserialize<NoticePlayerActionTurn>(package.Body);

            var jsonExpected = File.ReadAllText(Path.Combine(TestDataFolder, jsonFile));
            var jsonActual = JsonConvert.SerializeObject(noticePlayerActionTurnActual, Formatting.Indented, new StringEnumConverter());

            Assert.That(jsonActual, Is.EqualTo(jsonExpected));
        }

        [TestCase(@"Packets\170.33.8.75.31001-192.168.0.104.33644.txt", @"Packets\170.33.8.75.31001-192.168.0.104.33644-pkgt.txt", "dd/MM/yyyy HH:mm:ss")]
        public void TryParseTest(string file, string expectedPackageTypesFile, string dateFormat)
        {
            var packets = ReadCapturedPackets(file, dateFormat);

            var expectedPackageTypes = !string.IsNullOrEmpty(expectedPackageTypesFile) ?
                    GetPackageTypeList<PackageType>(expectedPackageTypesFile) :
                    new List<PackageType>();

            var packetManager = new PokerKingPacketManager();

            var expectedCommandsIndex = 0;

            using (ShimsContext.Create())
            {
                foreach (var packet in packets)
                {
                    ShimDateTime.NowGet = () => packet.CreatedTimeStamp;

                    if (packetManager.TryParse(packet, out PokerKingPackage package))
                    {
                        Console.WriteLine(package.PackageType);

                        if (expectedPackageTypes.Count > 0)
                        {
                            Assert.That(package.PackageType, Is.EqualTo(expectedPackageTypes[expectedCommandsIndex++]));
                        }
                    }
                }
            }
        }
    }
}