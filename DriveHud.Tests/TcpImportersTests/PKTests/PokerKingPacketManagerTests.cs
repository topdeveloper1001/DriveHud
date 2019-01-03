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
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Fakes;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

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

        [TestCase(@"Packets\NoticePlayerActionTurnPacket.txt", 1)]
        [TestCase(@"Packets\MultiplePacket.txt", 2)]
        public void TryParsePacketTest(string file, int expected)
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

            var result = packetManager.TryParse(capturedPacket, out IList<PokerKingPackage> actualPackages);

            Assert.IsTrue(result);
            Assert.IsNotNull(actualPackages);
            Assert.That(actualPackages.Count, Is.EqualTo(expected));
        }

        [TestCase(@"Packets\NoticePlayerActionTurnPacket.txt", @"Packets\NoticePlayerActionTurnPacket.json")]
        [TestCase(@"Packets\NoticeGameSnapShotPacket.txt", @"Packets\NoticeGameSnapShotPacket.json")]
        [TestCase(@"Packets\NoticePlayerActionPacket.txt", @"Packets\NoticePlayerActionPacket.json")]
        [TestCase(@"Packets\RequestJoinRoomPacket.txt", @"Packets\RequestJoinRoomPacket.json")]
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

            var result = packetManager.TryParse(capturedPacket, out IList<PokerKingPackage> packages);

            foreach (var package in packages)
            {
                object actual = null;

                switch (package.PackageType)
                {
                    case PackageType.NoticeGameRoundEnd:
                        actual = SerializationHelper.Deserialize<NoticeGameRoundEnd>(package.Body);
                        break;
                    case PackageType.NoticePlayerActionTurn:
                        actual = SerializationHelper.Deserialize<NoticePlayerActionTurn>(package.Body);
                        break;
                    case PackageType.NoticeGameSnapShot:
                        actual = SerializationHelper.Deserialize<NoticeGameSnapShot>(package.Body);
                        break;
                    case PackageType.NoticePlayerAction:
                        actual = SerializationHelper.Deserialize<NoticePlayerAction>(package.Body);
                        break;
                    case PackageType.RequestJoinRoom:
                        actual = SerializationHelper.Deserialize<RequestJoinRoom>(package.Body);
                        break;
                }

                var jsonExpected = File.ReadAllText(Path.Combine(TestDataFolder, jsonFile));
                var jsonActual = JsonConvert.SerializeObject(actual, Formatting.Indented, new StringEnumConverter());

                Assert.That(jsonActual, Is.EqualTo(jsonExpected));
            }
        }    

        //[TestCase(@"Packets\170.33.8.75.31001-192.168.0.104.33644.txt", @"Packets\170.33.8.75.31001-192.168.0.104.33644-pkgt.txt", "dd/MM/yyyy HH:mm:ss")]
        //[TestCase(@"Packets\27.155.82.113.31001-192.168.1.4.55025.txt", @"Packets\27.155.82.113.31001-192.168.1.4.55025-pkgt.txt", "yyyy/MM/dd HH:mm:ss")]
        //[TestCase(@"Packets\170.33.8.75.31001-192.168.0.106.9535.txt", @"Packets\170.33.8.75.31001-192.168.0.106.9535-pkgt.txt", "dd/MM/yyyy HH:mm:ss")]
        //[TestCase(@"Packets\192.168.0.106.9535-170.33.8.75.31001.txt", @"Packets\192.168.0.106.9535-170.33.8.75.31001-pkgt.txt", "dd/MM/yyyy HH:mm:ss")]
        //[TestCase(@"Packets\218.65.131.23.31001-192.168.1.101.2495.txt", @"Packets\218.65.131.23.31001-192.168.1.101.2495-pkgt.txt", "dd/MM/yyyy HH:mm:ss")]
        //[TestCase(@"Packets\170.33.8.252.31001-192.168.0.108.2242.txt", @"Packets\170.33.8.252.31001-192.168.0.108.2242-pkgt.txt", "dd/MM/yyyy HH:mm:ss")]
        [TestCase(@"Packets\170.33.8.252.31001-192.168.0.109.51863.txt", @"Packets\170.33.8.252.31001-192.168.0.109.51863-pkgt.txt", "dd/MM/yyyy HH:mm:ss", "3631e0425ce3f883b9117ee154078b5a")]
        public void TryParseTest(string file, string expectedPackageTypesFile, string dateFormat, string token)
        {
            var packets = ReadCapturedPackets(file, null);

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

                    if (packetManager.TryParse(packet, out IList<PokerKingPackage> packages))
                    {
                        foreach (var package in packages)
                        {
                            Console.WriteLine(package.PackageType);

                            if (expectedPackageTypes.Count > 0)
                            {
                                Assert.That(package.PackageType, Is.EqualTo(expectedPackageTypes[expectedCommandsIndex++]));

                                var key = Encoding.ASCII.GetBytes(token);

                                var cipher = CipherUtilities.GetCipher("AES/ECB/PKCS5Padding");
                                cipher.Init(false, new KeyParameter(key));

                                var bytes = cipher.ProcessBytes(package.Body);
                                var final = cipher.DoFinal();

                                var decryptedData = bytes == null ? final :
                                    (final == null ? bytes : bytes.Concat(final).ToArray());

                                package.Body = decryptedData;
                            
                                AssertPackage(package, packet);
                            }
                        }
                    }
                }
            }
        }

        private void AssertPackage(PokerKingPackage package, CapturedPacket capturedPacket)
        {
            switch (package.PackageType)
            {
                case PackageType.RequestLeaveRoom:
                    AssertPackage<RequestLeaveRoom>(package, capturedPacket);
                    break;
                case PackageType.NoticeStartGame:
                    AssertPackage<NoticeStartGame>(package, capturedPacket);
                    break;
                case PackageType.NoticeResetGame:
                    AssertPackage<NoticeResetGame>(package, capturedPacket);
                    break;
                case PackageType.NoticeGamePost:
                    AssertPackage<NoticeGamePost>(package, capturedPacket);
                    break;
                case PackageType.NoticeGameAnte:
                    AssertPackage<NoticeGameAnte>(package, capturedPacket);
                    break;
                case PackageType.NoticeGameElectDealer:
                    AssertPackage<NoticeGameElectDealer>(package, capturedPacket);
                    break;
                case PackageType.NoticeGameBlind:
                    AssertPackage<NoticeGameBlind>(package, capturedPacket);
                    break;
                case PackageType.NoticeGameHoleCard:
                    AssertPackage<NoticeGameHoleCard>(package, capturedPacket);
                    break;
                case PackageType.NoticePlayerAction:
                    AssertPackage<NoticePlayerAction>(package, capturedPacket);
                    break;
                case PackageType.NoticeGameRoundEnd:
                    AssertPackage<NoticeGameRoundEnd>(package, capturedPacket);
                    break;
                case PackageType.NoticeGameCommunityCards:
                    AssertPackage<NoticeGameCommunityCards>(package, capturedPacket);
                    break;
                case PackageType.NoticeGameShowCard:
                    AssertPackage<NoticeGameShowCard>(package, capturedPacket);
                    break;
                case PackageType.NoticeGameSettlement:
                    AssertPackage<NoticeGameSettlement>(package, capturedPacket);
                    break;
                case PackageType.NoticeGameShowDown:
                    AssertPackage<NoticeGameShowDown>(package, capturedPacket);
                    break;
                case PackageType.NoticePlayerStayPosition:
                    AssertPackage<NoticePlayerStayPosition>(package, capturedPacket);
                    break;
                case PackageType.NoticePlayerShowCard:
                    AssertPackage<NoticePlayerShowCard>(package, capturedPacket);
                    break;
                case PackageType.NoticeBuyin:
                    AssertPackage<NoticeBuyin>(package, capturedPacket);
                    break;
                case PackageType.NoticeGameSnapShot:
                    AssertPackage<NoticeGameSnapShot>(package, capturedPacket);
                    break;
                case PackageType.RequestHeartBeat:
                    AssertPackage<RequestHeartBeat>(package, capturedPacket);
                    break;
            }
        }

        private void AssertPackage<T>(PokerKingPackage package, CapturedPacket capturedPacket)
        {
            Assert.IsTrue(SerializationHelper.TryDeserialize(package.Body, out T packageContent), $"Failed to deserialize {typeof(T)} package [ticks={capturedPacket.CreatedTimeStamp.Ticks}, userid={package.UserId}]");
        }
    }
}