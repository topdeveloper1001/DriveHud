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

using DriveHUD.Common.Extensions;
using DriveHUD.Importers.PokerMaster;
using DriveHUD.Importers.PokerMaster.Model;
using Microsoft.QualityTools.Testing.Fakes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Fakes;
using System.IO;
using System.Linq;
using System.Net;

namespace PMCatcher.Tests
{
    [TestFixture]
    public class PacketManagerTests
    {
        private const string TestDataFolder = "PMTests\\TestData";

        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase(@"Packets\Packet1.txt", true)]
        public void PacketIsStartingPacketTest(string file, bool expected)
        {
            var bytes = ReadPacketFile(file);
            var packetManager = new PacketManager();

            var actual = PacketManager.IsStartingPacket(bytes);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(@"Packets\Packet1.txt")]
        public void TryParsePacketTest(string file)
        {
            var bytes = ReadPacketFile(file);
            var packetManager = new PacketManager();

            var capturedPacket = new CapturedPacket
            {
                Bytes = bytes,
                CreatedTimeStamp = DateTime.Parse("08/02/2018 12:28:28"),
                Destination = new IPEndPoint(IPAddress.Parse("192.168.0.105"), 27633),
                Source = new IPEndPoint(IPAddress.Parse("47.52.92.161"), 9188),
                SequenceNumber = 1962805251
            };

            var result = packetManager.TryParse(capturedPacket, out Package actual);

            Assert.IsTrue(result);
            Assert.IsNotNull(actual);
        }

        [TestCase(@"Packets\SCLoginBody.txt", "Peon")]
        public void DeserializationTest(string file, string username)
        {
            var body = ReadPacketFile(file);

            var bodyDecryptor = new BodyDecryptor();

            var bytes = bodyDecryptor.Decrypt(body);

            var scLoginRsp = SerializationHelper.Deserialize<SCLoginRsp>(bytes);

            Assert.IsNotNull(scLoginRsp);
            Assert.IsNotNull(scLoginRsp.UserInfo);
            Assert.That(scLoginRsp.UserInfo.Nick, Is.EqualTo(username));
        }

        [TestCase(@"Packets\119.28.109.172.9188-192.168.0.104.60251.txt", "OTU1MTI1NTY4Mzg0NTk3Ng==", @"Packets\119.28.109.172.9188-192.168.0.104.60251-cmd.txt", "dd/MM/yyyy HH:mm:ss")]
        [TestCase(@"Packets\119.28.109.172.9188-192.168.0.104.49082.txt", "OTQwMWNkNTAzZDQzMmJiMw==", @"Packets\119.28.109.172.9188-192.168.0.104.49082-cmd.txt", "dd/MM/yyyy HH:mm:ss")]
        [TestCase(@"Packets\119.28.109.172.9188-192.168.0.104.60235.txt", "NGNiMzZjMDFmZTAwOTFlOQ==", @"Packets\119.28.109.172.9188-192.168.0.104.60235-cmd.txt", "dd/MM/yyyy HH:mm:ss")]
        [TestCase(@"Packets\218.98.62.171.9188-10.0.0.81.3511.txt", "NWE4N2MxNjMyNWM2OWFlMA==", @"Packets\218.98.62.171.9188-10.0.0.81.3511-cmd.txt", "yyyy/M/dd H:mm:ss")]
        public void TryParseTest(string file, string decryptKey, string expectedCommandsFile, string dateFormat)
        {
            var packets = ReadCapturedPackets(file, dateFormat);
            var expectedCommands = GetCommandList(expectedCommandsFile);

            var packetManager = new PacketManager();

            var decryptKeyBytes = Convert.FromBase64String(decryptKey);

            var bodyDecryptor = new BodyDecryptor();

            var expectedCommandsIndex = 0;

            using (ShimsContext.Create())
            {
                foreach (var packet in packets)
                {
                    ShimDateTime.NowGet = () => packet.CreatedTimeStamp;

                    if (packetManager.TryParse(packet, out Package package))
                    {
                        Console.WriteLine(package.Cmd);

                        Assert.That(package.Cmd, Is.EqualTo(expectedCommands[expectedCommandsIndex++]));

                        if (package.Cmd == PackageCommand.Cmd_SCGameRoomStateChange)
                        {
                            var body = bodyDecryptor.Decrypt(package.Body, decryptKeyBytes, false);

                            if (!SerializationHelper.TryDeserialize(body, out SCGameRoomStateChange sCGameRoomStateChange))
                            {
                                Assert.Fail($"Packet {packet.SequenceNumber} was incorrectly combined with other packets. So result can't be deserialized.");
                            }
                        }
                    }
                }
            }
        }

        private List<PackageCommand> GetCommandList(string file)
        {
            file = Path.Combine(TestDataFolder, file);
            FileAssert.Exists(file);

            var commands = File.ReadAllLines(file)
                .Select(x => (PackageCommand)Enum.Parse(typeof(PackageCommand), x))
                .ToList();

            return commands;
        }

        private List<CapturedPacket> ReadCapturedPackets(string file, string dateFormat)
        {
            file = Path.Combine(TestDataFolder, file);
            FileAssert.Exists(file);

            var sourceDestination = ParseSourceDestinationFromFile(Path.GetFileNameWithoutExtension(file));

            var lines = File.ReadAllLines(file);

            var capturedPackets = new List<CapturedPacket>();

            CapturedPacket capturedPacket = null;

            bool isBody = false;

            foreach (var line in lines)
            {
                if (line.IndexOf("-begin-") > 0)
                {
                    capturedPacket = new CapturedPacket
                    {
                        Source = new IPEndPoint(IPAddress.Parse(sourceDestination.Item1), sourceDestination.Item2),
                        Destination = new IPEndPoint(IPAddress.Parse(sourceDestination.Item3), sourceDestination.Item4),
                    };

                    continue;
                }

                if (line.StartsWith("Date:", StringComparison.OrdinalIgnoreCase))
                {
                    var dateText = line.Substring(5).Trim();
                    capturedPacket.CreatedTimeStamp = DateTime.ParseExact(dateText, dateFormat, null);
                    continue;
                }

                if (line.StartsWith("SequenceNumber:", StringComparison.OrdinalIgnoreCase))
                {
                    var sequenceNumberText = line.Substring(16).Trim();
                    capturedPacket.SequenceNumber = uint.Parse(sequenceNumberText);
                    continue;
                }

                if (line.IndexOf("-body begin-") > 0)
                {
                    isBody = true;
                    continue;
                }

                if (line.IndexOf("-body end-") > 0)
                {
                    isBody = false;
                    continue;
                }

                if (isBody)
                {
                    capturedPacket.Bytes = line.FromHexStringToBytes();
                }

                if (line.IndexOf("-end-") > 0 && capturedPacket != null)
                {
                    capturedPackets.Add(capturedPacket);
                }
            }

            return capturedPackets;
        }

        private Tuple<string, int, string, int> ParseSourceDestinationFromFile(string file)
        {
            var fileSplitted = file.Split('-');

            var sourcePart = fileSplitted[0];
            var destinationPart = fileSplitted[1];

            var source = ParseIpPort(sourcePart);
            var destination = ParseIpPort(destinationPart);

            return Tuple.Create(source.Item1, source.Item2, destination.Item1, destination.Item2);
        }

        private Tuple<string, int> ParseIpPort(string ipPort)
        {
            var indexOfPort = ipPort.LastIndexOf('.');
            var ip = ipPort.Substring(0, indexOfPort - 1);
            var port = int.Parse(ipPort.Substring(indexOfPort + 1));
            return Tuple.Create(ip, port);
        }

        private byte[] ReadPacketFile(string file)
        {
            file = Path.Combine(TestDataFolder, file);

            Assert.IsTrue(File.Exists(file), $"{file} doesn't exist");

            var fileText = File.ReadAllText(file);

            Assert.IsNotEmpty(fileText, $"{file} is empty");

            var bytes = fileText.FromHexStringToBytes();
            return bytes;
        }
    }
}