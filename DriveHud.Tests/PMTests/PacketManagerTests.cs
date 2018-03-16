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
using NUnit.Framework;
using System;
using System.Collections.Generic;
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

        [Test]
        [TestCase(@"Packets\Packet1.txt", true)]
        public void PacketIsStartingPacketTest(string file, bool expected)
        {
            var bytes = ReadPacketFile(file);
            var packetManager = new PacketManager();

            var actual = packetManager.IsStartingPacket(bytes);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
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

        [Test]
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

        [TestCase(@"Packets\119.28.109.172.9188-192.168.0.104.60251.txt", "OTU1MTI1NTY4Mzg0NTk3Ng==")]
        [TestCase(@"Packets\119.28.109.172.9188-192.168.0.104.49082.txt", "OTQwMWNkNTAzZDQzMmJiMw==")]
        [TestCase(@"Packets\119.28.109.172.9188-192.168.0.104.60235.txt", "NGNiMzZjMDFmZTAwOTFlOQ==")]
        public void TryParseTest(string file, string decryptKey)
        {
            var packets = ReadCapturedPackets(file);

            var packetManager = new PacketManager();

            var decryptKeyBytes = Convert.FromBase64String(decryptKey);

            var bodyDecryptor = new BodyDecryptor();

            foreach (var packet in packets)
            {
                if (packetManager.TryParse(packet, out Package package))
                {                    
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

        private List<CapturedPacket> ReadCapturedPackets(string file)
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
                    capturedPacket.CreatedTimeStamp = DateTime.ParseExact(dateText, "dd/MM/yyyy HH:mm:ss", null);
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