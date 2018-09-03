//-----------------------------------------------------------------------
// <copyright file="PKHandBuilderTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.PokerKing;
using DriveHUD.Importers.PokerKing.Model;
using HandHistories.Objects.Hand;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DriveHud.Tests.TcpImportersTests.PKTests
{
    [TestFixture]
    class PKHandBuilderTests : PacketManagerTest
    {
        private const string SourceJsonFile = "Source.json";
        private const string ExpectedResultFile = "Result.xml";

        private const int identifier = 7777;

        protected override string TestDataFolder => "TcpImportersTests\\PKTests\\TestData\\HandsRawData";

        [TestCase("8-max-regular-no-hero", 105772u)]
        [TestCase("6-max-short-dbl-ante-no-hero", 105772u)]
        [TestCase("8-max-regular-hero", 105772u)]
        public void TryBuildTest(string testFolder, uint heroId)
        {
            var packages = ReadPackages(testFolder);

            CollectionAssert.IsNotEmpty(packages, $"Packages collection must be not empty for {testFolder}");

            var handBuilder = new PKHandBuilder();

            HandHistory actual = null;

            foreach (var package in packages)
            {
                package.UserId = heroId;

                if (handBuilder.TryBuild(package, identifier, out actual))
                {
                    break;
                }
            }

            Assert.IsNotNull(actual, $"Actual HandHistory must be not null for {testFolder}");

            var expected = ReadExpectedHandHistory(testFolder);

            Assert.IsNotNull(expected, $"Expected HandHistory must be not null for {testFolder}");

            AssertionUtils.AssertHandHistory(actual, expected);
        }

        [TestCase("multiple-accounts-raw-1", 31001)]
        public void MultipleTryBuildTest(string folder, int destinationPort)
        {
            var testFolder = Path.Combine(TestDataFolder, folder);

            DirectoryAssert.Exists(testFolder);

            var logFiles = Directory.GetFiles(testFolder, "*.txt.");

            var packages = new List<CapturedPacket>();

            foreach (var logFile in logFiles)
            {
                packages.AddRange(TcpImporterTestUtils.ReadCapturedPackets(logFile, null));
            }

            packages = packages.OrderBy(x => x.CreatedTimeStamp).ToList();

            var handHistories = new List<HandHistory>();

            var packetManager = new PokerKingPacketManager();
            var handBuilder = new PKHandBuilder();

            foreach (var capturedPacket in packages)
            {
                if (!packetManager.TryParse(capturedPacket, out PokerKingPackage package, true) || !PKImporterStub.IsAllowedPackage(package))
                {
                    continue;
                }

                var port = capturedPacket.Destination.Port != destinationPort ? capturedPacket.Destination.Port : capturedPacket.Source.Port;

                if (handBuilder.TryBuild(package, port, out HandHistory handHistory))
                {
                    handHistories.Add(handHistory);
                }
            }

            Assert.IsTrue(handHistories.Count > 0);
        }

        private HandHistory ReadExpectedHandHistory(string folder)
        {
            var xmlFile = Path.Combine(TestDataFolder, folder, ExpectedResultFile);

            FileAssert.Exists(xmlFile);

            try
            {
                var handHistoryText = File.ReadAllText(xmlFile);
                return SerializationHelper.DeserializeObject<HandHistory>(handHistoryText);
            }
            catch (Exception e)
            {
                Assert.Fail($"{ExpectedResultFile} in {folder} has not been deserialized: {e}");
            }

            return null;
        }

        private IEnumerable<PokerKingPackage> ReadPackages(string folder)
        {
            var packages = new List<PokerKingPackage>();

            var jsonFile = Path.Combine(TestDataFolder, folder, SourceJsonFile);

            FileAssert.Exists(jsonFile);

            PKTestSourceObject testSourceObject = null;

            try
            {
                var jsonFileContent = File.ReadAllText(jsonFile);
                testSourceObject = JsonConvert.DeserializeObject<PKTestSourceObject>(jsonFileContent, new StringEnumConverter());
            }
            catch (Exception e)
            {
                Assert.Fail($"{ExpectedResultFile} in {folder} has not been deserialized: {e}");
            }

            Assert.IsNotNull(testSourceObject);

            foreach (var packet in testSourceObject.Packets)
            {
                PokerKingPackage package = null;

                switch (packet.PackageType)
                {
                    case PackageType.RequestLeaveRoom:
                        package = BuildPackage<RequestLeaveRoom>(packet);
                        break;
                    case PackageType.NoticeStartGame:
                        package = BuildPackage<NoticeStartGame>(packet);
                        break;
                    case PackageType.NoticeResetGame:
                        package = BuildPackage<NoticeResetGame>(packet);
                        break;
                    case PackageType.NoticeGamePost:
                        package = BuildPackage<NoticeGamePost>(packet);
                        break;
                    case PackageType.NoticeGameAnte:
                        package = BuildPackage<NoticeGameAnte>(packet);
                        break;
                    case PackageType.NoticeGameElectDealer:
                        package = BuildPackage<NoticeGameElectDealer>(packet);
                        break;
                    case PackageType.NoticeGameBlind:
                        package = BuildPackage<NoticeGameBlind>(packet);
                        break;
                    case PackageType.NoticeGameHoleCard:
                        package = BuildPackage<NoticeGameHoleCard>(packet);
                        break;
                    case PackageType.NoticePlayerAction:
                        package = BuildPackage<NoticePlayerAction>(packet);
                        break;
                    case PackageType.NoticeGameRoundEnd:
                        package = BuildPackage<NoticeGameRoundEnd>(packet);
                        break;
                    case PackageType.NoticeGameCommunityCards:
                        package = BuildPackage<NoticeGameCommunityCards>(packet);
                        break;
                    case PackageType.NoticeGameShowCard:
                        package = BuildPackage<NoticeGameShowCard>(packet);
                        break;
                    case PackageType.NoticeGameSettlement:
                        package = BuildPackage<NoticeGameSettlement>(packet);
                        break;
                    case PackageType.NoticeGameShowDown:
                        package = BuildPackage<NoticeGameShowDown>(packet);
                        break;
                    case PackageType.NoticePlayerStayPosition:
                        package = BuildPackage<NoticePlayerStayPosition>(packet);
                        break;
                    case PackageType.NoticePlayerShowCard:
                        package = BuildPackage<NoticePlayerShowCard>(packet);
                        break;
                    case PackageType.NoticeBuyin:
                        package = BuildPackage<NoticeBuyin>(packet);
                        break;
                    case PackageType.NoticeGameSnapShot:
                        package = BuildPackage<NoticeGameSnapShot>(packet);
                        break;
                    case PackageType.RequestHeartBeat:
                        package = BuildPackage<RequestHeartBeat>(packet);
                        break;
                    default:
                        Assert.Fail($"Unsupported package type: {packet.PackageType}");
                        break;
                }

                Assert.IsNotNull(package);

                packages.Add(package);
            }

            return packages;
        }

        private PokerKingPackage BuildPackage<T>(PKTestSourcePacket packet)
        {
            var contentObject = JsonConvert.DeserializeObject<T>(packet.Content.ToString(), new StringEnumConverter());

            Assert.IsNotNull(contentObject, $"Content object of {typeof(T)} must be not null.");

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, contentObject);
                var bytes = ms.ToArray();

                var package = new PokerKingPackage
                {
                    PackageType = packet.PackageType,
                    Body = bytes,
                    Timestamp = packet.Time
                };

                return package;
            }
        }

        private class PKTestSourceObject
        {
            public IEnumerable<PKTestSourcePacket> Packets { get; set; }
        }

        private class PKTestSourcePacket
        {
            public PackageType PackageType { get; set; }

            public DateTime Time { get; set; }

            public object Content { get; set; }
        }

        private class PKImporterStub : PKImporter
        {
            public new static bool IsAllowedPackage(PokerKingPackage package)
            {
                return PKImporter.IsAllowedPackage(package);
            }
        }
    }
}