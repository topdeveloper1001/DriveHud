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
using System.Text;

namespace PMCatcher.Tests
{
    [TestFixture]
    class PokerMasterPacketManagerTests : PacketManagerTest
    {
        protected override string TestDataFolder => "TcpImportersTests\\PMTests\\TestData";

        [TestCase(@"Packets\Packet1.txt", true)]
        public void PacketIsStartingPacketTest(string file, bool expected)
        {
            var bytes = ReadPacketFile(file);
            var packetManager = new PokerMasterPacketManager();

            var actual = packetManager.IsStartingPacket(bytes);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(@"Packets\Packet1.txt")]
        public void TryParsePacketTest(string file)
        {
            var bytes = ReadPacketFile(file);
            var packetManager = new PokerMasterPacketManager();

            var capturedPacket = new CapturedPacket
            {
                Bytes = bytes,
                CreatedTimeStamp = DateTime.Parse("08/02/2018 12:28:28"),
                Destination = new IPEndPoint(IPAddress.Parse("192.168.0.105"), 27633),
                Source = new IPEndPoint(IPAddress.Parse("47.52.92.161"), 9188),
                SequenceNumber = 1962805251
            };

            var dumpFiles = Directory.GetFiles(@"d:\Temp\Dump\", "*.*", SearchOption.TopDirectoryOnly);

            var sqNo = 1u;

            foreach (var dumpFile in dumpFiles)
            {


                var dumpCaptPack = new CapturedPacket
                {
                    Bytes = File.ReadAllBytes(dumpFile).Skip(25).ToArray(),
                    CreatedTimeStamp = DateTime.Parse("08/02/2018 12:28:28"),
                    Destination = new IPEndPoint(IPAddress.Parse("192.168.0.105"), 27633),
                    Source = new IPEndPoint(IPAddress.Parse("47.52.92.161"), 9188),
                    SequenceNumber = sqNo++
                };

                if (packetManager.TryParse(dumpCaptPack, out IList<PokerMasterPackage> dumpPacks))
                {
                    var dmpPack = dumpPacks.FirstOrDefault();

                    if (dmpPack.Cmd == PackageCommand.Cmd_SCLoginRsp)
                    {

                        var bodyDecryptor = new BodyDecryptor();


                        var key = Encoding.UTF8.GetBytes("116ff58c0b178429");

                        var bodyBytes = bodyDecryptor.Decrypt(dmpPack.Body, key, false);

                        File.WriteAllBytes(@"d:\dmp.bin", bodyBytes);
                    }
                }
            }

            var result = packetManager.TryParse(capturedPacket, out IList<PokerMasterPackage> actualPackages);

            Assert.IsTrue(result);
            Assert.IsNotNull(actualPackages);
            CollectionAssert.IsNotEmpty(actualPackages);
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

        //[TestCase(@"Packets\119.28.109.172.9188-192.168.0.104.60251.txt", "OTU1MTI1NTY4Mzg0NTk3Ng==", @"Packets\119.28.109.172.9188-192.168.0.104.60251-cmd.txt", "dd/MM/yyyy HH:mm:ss")]
        //[TestCase(@"Packets\119.28.109.172.9188-192.168.0.104.49082.txt", "OTQwMWNkNTAzZDQzMmJiMw==", @"Packets\119.28.109.172.9188-192.168.0.104.49082-cmd.txt", "dd/MM/yyyy HH:mm:ss")]
        //[TestCase(@"Packets\119.28.109.172.9188-192.168.0.104.60235.txt", "NGNiMzZjMDFmZTAwOTFlOQ==", @"Packets\119.28.109.172.9188-192.168.0.104.60235-cmd.txt", "dd/MM/yyyy HH:mm:ss")]
        [TestCase(@"Packets\218.98.62.171.9188-10.0.0.81.3511.txt", "NWE4N2MxNjMyNWM2OWFlMA==", @"Packets\218.98.62.171.9188-10.0.0.81.3511-cmd.txt", "yyyy/M/dd H:mm:ss")]
        //[TestCase(@"Packets\218.98.62.171.9188-192.168.0.102.60431.txt", "NGQ0ODUwZTBiZjRhMGZhMQ==", @"Packets\218.98.62.171.9188-192.168.0.102.60431-cmd.txt", "yyyy/M/dd H:mm:ss")]
        //[TestCase(@"Packets\218.98.62.171.9188-192.168.0.102.60462.txt", "YWMzYjJhYjg0MjljMmQxZA==", @"Packets\218.98.62.171.9188-192.168.0.102.60462-cmd.txt", "yyyy/M/dd H:mm:ss")]
        public void TryParseTest(string file, string decryptKey, string expectedCommandsFile, string dateFormat)
        {
            var packets = ReadCapturedPackets(file, dateFormat);
            var expectedCommands = !string.IsNullOrEmpty(expectedCommandsFile) ?
                    GetPackageTypeList<PackageCommand>(expectedCommandsFile) :
                    new List<PackageCommand>();

            var packetManager = new PokerMasterPacketManager();

            var decryptKeyBytes = Convert.FromBase64String(decryptKey);

            var bodyDecryptor = new BodyDecryptor();

            var expectedCommandsIndex = 0;

            using (ShimsContext.Create())
            {
                foreach (var packet in packets)
                {
                    ShimDateTime.NowGet = () => packet.CreatedTimeStamp;

                    if (packetManager.TryParse(packet, out IList<PokerMasterPackage> packages))
                    {
                        foreach (var package in packages)
                        {
                            Console.WriteLine(package.Cmd);

                            if (expectedCommands.Count > 0)
                            {
                                Assert.That(package.Cmd, Is.EqualTo(expectedCommands[expectedCommandsIndex++]));
                            }

                            if (package.Cmd == PackageCommand.Cmd_SCLoginRsp)
                            {
                                var body = bodyDecryptor.Decrypt(package.Body);

                                File.WriteAllBytes(@"d:\dta.bin", body);

                                if (!SerializationHelper.TryDeserialize(body, out SCLoginRsp sCLoginRsp))
                                {
                                    Assert.Fail($"Packet {packet.SequenceNumber} was incorrectly combined with other packets. So result can't be deserialized.");
                                }
                            }

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
        }
    }
}