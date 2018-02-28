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
using System.IO;
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

        private byte[] ReadPacketFile(string file)
        {
            file = Path.Combine(TestDataFolder, file);

            Assert.IsTrue(File.Exists(file), $"{file} doesn't exist");

            var fileText = File.ReadAllText(file);

            Assert.IsNotEmpty(fileText, $"{file} is empty");

            var bytes = fileText.FromHextStringToBytes();
            return bytes;
        }
    }
}