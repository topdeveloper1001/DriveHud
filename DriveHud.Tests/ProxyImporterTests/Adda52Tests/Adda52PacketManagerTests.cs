//-----------------------------------------------------------------------
// <copyright file="Adda52PacketManagerTests.cs" company="Ace Poker Solutions">
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
using DriveHUD.Importers.Adda52;
using Microsoft.QualityTools.Testing.Fakes;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Fakes;
using System.IO;
using System.Linq;
using System.Text;

namespace DriveHud.Tests.ProxyImporterTests.Adda52Tests
{
    [TestFixture]
    class Adda52PacketManagerTests : PacketManagerTest
    {
        protected override string TestDataFolder => "ProxyImporterTests\\Adda52Tests\\TestData";

        [TestCase(@"Packets\0.0.0.0.8893-127.0.0.1.55025.txt", @"Packets\LongPacket.json")]
        [TestCase(@"Packets\0.0.0.0.8893-127.0.0.1.12031.txt", @"Packets\SinglePacket.json")]
        [TestCase(@"Packets\0.0.0.0.8893-127.0.0.1.12032.txt", @"Packets\NotJsonOnlyPacket.json")]
        public void TryParseTest(string file, string expectedResultFile)
        {
            var packets = ReadCapturedPackets(file, null);

            var packetManager = new Adda52PacketManager();

            var actualJson = string.Empty;
            var expectedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(TestDataFolder, expectedResultFile))), Formatting.Indented);

            var sequenceNumber = 1u;

            using (ShimsContext.Create())
            {
                foreach (var packet in packets)
                {
                    ShimDateTime.NowGet = () => packet.CreatedTimeStamp;

                    packet.SequenceNumber = sequenceNumber++;

                    if (packetManager.TryParse(packet, out IList<Adda52Package> packages))
                    {
                        var package = packages.FirstOrDefault();

                        Assert.IsNotNull(package);

                        var jsonText = Encoding.UTF8.GetString(package.Bytes);
                        var dynamicObject = JsonConvert.DeserializeObject(jsonText);

                        actualJson = JsonConvert.SerializeObject(dynamicObject, Formatting.Indented);
                    }
                }
            }

            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }      
    }
}