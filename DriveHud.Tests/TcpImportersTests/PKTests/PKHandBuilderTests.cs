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
using DriveHUD.Common.Log;
using DriveHUD.Importers;
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
        private const int destinationPort = 31001;

        private const int identifier = 7777;

        private IDHLog testLogger;

        public override void SetUp()
        {
            base.SetUp();

            testLogger = new TestLogger();
            LogProvider.SetCustomLogger(testLogger);
        }

        protected override string TestDataFolder => "TcpImportersTests\\PKTests\\TestData\\HandsRawData";

        [TestCase("8-max-regular-no-hero", 105772u)]
        [TestCase("6-max-short-dbl-ante-no-hero", 105772u)]
        [TestCase("8-max-regular-hero", 105772u)]
        [TestCase("6-max-short-all-in", 105772u)]
        [TestCase("8-max-regular-hero-wrong-order", 105772u)]
        [TestCase("8-max-regular-no-hero-bet-on-preflop", 105772u)]
        [TestCase("8-max-hero-raises-from-sb", 105772u)]
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

        [TestCase("multiple-accounts-raw-1")]
        [TestCase("multiple-accounts-raw-2")]
        [TestCase("multiple-accounts-raw-3")]
        [TestCase("multiple-accounts-raw-4")]
        [TestCase("multiple-accounts-raw-5")]
        [TestCase("multiple-accounts-raw-6")]
        [TestCase("multiple-accounts-raw-7")]
        public void MultipleTryBuildTest(string folder)
        {
            var testFolder = Path.Combine(TestDataFolder, folder);

            DirectoryAssert.Exists(testFolder);

            var logFiles = Directory.GetFiles(testFolder, "*.txt.");

            var capturedPackets = new List<CapturedPacket>();

            foreach (var logFile in logFiles)
            {
                capturedPackets.AddRange(TcpImporterTestUtils.ReadCapturedPackets(logFile, null));
            }

            capturedPackets = capturedPackets.OrderBy(x => x.CreatedTimeStamp).ToList();

            var handHistories = new List<HandHistory>();

            var packetManager = new PokerKingPacketManager();
            var handBuilder = new PKHandBuilder();
            var debugPKImporter = new DebugPKImporter();

            foreach (var capturedPacket in capturedPackets)
            {
                if (!packetManager.TryParse(capturedPacket, out IList<PokerKingPackage> packages))
                {
                    continue;
                }

                foreach (var package in packages)
                {
                    if (!PKImporterStub.IsAllowedPackage(package))
                    {
                        continue;
                    }

                    package.Timestamp = capturedPacket.CreatedTimeStamp;

                    debugPKImporter.LogPackage(capturedPacket, package);

                    if (package.PackageType == PackageType.RequestJoinRoom)
                    {
                        debugPKImporter.ParsePackage<RequestJoinRoom>(package,
                          body => LogProvider.Log.Info($"User {package.UserId} entered room {body.RoomId}."),
                          () => LogProvider.Log.Info($"User {package.UserId} entered room."));

                        continue;
                    }

                    var port = capturedPacket.Destination.Port != destinationPort ? capturedPacket.Destination.Port : capturedPacket.Source.Port;

                    if (package.PackageType == PackageType.RequestLeaveRoom)
                    {
                        debugPKImporter.ParsePackage<RequestLeaveRoom>(package,
                            body =>
                            {
                                LogProvider.Log.Info($"User {package.UserId} left room {body.RoomId}.");
                                handBuilder.CleanRoom(port, body.RoomId);
                            },
                            () => LogProvider.Log.Info($"User {package.UserId} left room {package.RoomId}."));

                        continue;
                    }

                    if (handBuilder.TryBuild(package, port, out HandHistory handHistory))
                    {
                        handHistories.Add(handHistory);
                        LogProvider.Log.Info($"Hand #{handHistory.HandId} has been sent. [{package.UserId}]");
                    }
                }
            }

            WriteHandHistoriesToFile(handHistories);

            Assert.IsTrue(handHistories.Count > 0);
        }

        private void WriteHandHistoriesToFile(IEnumerable<HandHistory> handHistories)
        {
            var groupedHandHistories = handHistories.GroupBy(x => x.GameDescription.Identifier).ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var handHistoriesByIdentifier in groupedHandHistories)
            {
                var file = $"{handHistoriesByIdentifier.Key}.xml";

                var xml = SerializationHelper.SerializeObject(handHistoriesByIdentifier.Value);

                File.WriteAllText(file, xml);
            }
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

        private class TestLogger : IDHLog
        {
            public bool IsAdvanced { get; set; }

            public void Log(Type senderType, object message, LogMessageType logMessageType)
            {
            }

            public void Log(Type senderType, object message, Exception exception, LogMessageType logMessageType)
            {
            }

            public void Log(string loggerName, object message, LogMessageType logMessageType)
            {
                Console.WriteLine(message);
            }

            public void Log(string loggerName, object message, Exception exception, LogMessageType logMessageType)
            {
                Console.WriteLine($"{message}: {exception}");
            }
        }

        private class DebugPKImporter : PKImporter
        {
            private Dictionary<int, DebugPKLogger> loggers = new Dictionary<int, DebugPKLogger>();

            public DebugPKImporter()
            {
            }

            public void LogPackage(CapturedPacket capturedPacket, PokerKingPackage package)
            {
                var port = capturedPacket.Destination.Port != destinationPort ? capturedPacket.Destination.Port : capturedPacket.Source.Port;

                if (!loggers.TryGetValue(port, out DebugPKLogger logger))
                {
                    logger = new DebugPKLogger(port);
                    loggers.Add(port, logger);
                }

                protectedLogger = logger;

                base.LogPackage(package);
            }

            public new void ParsePackage<T>(PokerKingPackage package, Action<T> onSuccess, Action onFail)
            {
                base.ParsePackage(package, onSuccess, onFail);
            }

            private class DebugPKLogger : IPokerClientEncryptedLogger
            {
                private const string logFilePattern = "Json-raw-{0}.json";

                private string logFile;

                public DebugPKLogger(int port)
                {
                    logFile = string.Format(logFilePattern, port);

                    if (File.Exists(logFile))
                    {
                        File.Delete(logFile);
                    }
                }

                public void CleanLogs()
                {
                }

                public void Initialize(PokerClientLoggerConfiguration configuration)
                {
                }

                public void Log(string message)
                {
                    File.AppendAllText(logFile, message);
                }

                public void StartLogging()
                {
                }

                public void StopLogging()
                {
                }
            }
        }
    }
}