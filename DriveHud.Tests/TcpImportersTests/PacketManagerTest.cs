//-----------------------------------------------------------------------
// <copyright file="PacketManagerTest.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.PokerKing;
using DriveHUD.Importers.PokerKing.Model;
using DriveHUD.Importers.PokerMaster;
using DriveHUD.Importers.PokerMaster.Model;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using PMCatcher.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace DriveHud.Tests.TcpImportersTests
{
    abstract class PacketManagerTest
    {
        protected abstract string TestDataFolder { get; }

        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            var unityContainer = new UnityContainer();

            unityContainer.RegisterType<IPackageBuilder<PokerKingPackage>, PokerKingPackageBuilder>();
            unityContainer.RegisterType<IPackageBuilder<PokerMasterPackage>, PokerMasterPackageBuilder>();

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);
        }

        protected List<CapturedPacket> ReadCapturedPackets(string file, string dateFormat)
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

                if (!string.IsNullOrEmpty(dateFormat) && line.StartsWith("Date:", StringComparison.OrdinalIgnoreCase))
                {
                    var dateText = line.Substring(5).Trim();
                    capturedPacket.CreatedTimeStamp = DateTime.ParseExact(dateText, dateFormat, null);
                    continue;
                }

                if (line.StartsWith("Date Now (ticks):", StringComparison.OrdinalIgnoreCase))
                {
                    var ticksText = line.Substring(17).Trim();
                    var ticks = long.Parse(ticksText);
                    capturedPacket.CreatedTimeStamp = new DateTime(ticks);
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

            var packetsToUpdate = capturedPackets.GroupBy(x => x.CreatedTimeStamp)
                .Select(x => new { Date = x.Key, Packets = x.ToList() })
                .Where(x => x.Packets.Count > 1)
                .ToArray();

            packetsToUpdate.ForEach(x =>
            {
                var delta = 0;
                x.Packets.ForEach(y => y.CreatedTimeStamp = y.CreatedTimeStamp.AddTicks(delta++));
            });

            return capturedPackets;
        }

        protected Tuple<string, int, string, int> ParseSourceDestinationFromFile(string file)
        {
            var fileSplitted = file.Split('-');

            var sourcePart = fileSplitted[0];
            var destinationPart = fileSplitted[1];

            var source = ParseIpPort(sourcePart);
            var destination = ParseIpPort(destinationPart);

            return Tuple.Create(source.Item1, source.Item2, destination.Item1, destination.Item2);
        }

        protected Tuple<string, int> ParseIpPort(string ipPort)
        {
            var indexOfPort = ipPort.LastIndexOf('.');
            var ip = ipPort.Substring(0, indexOfPort);
            var port = int.Parse(ipPort.Substring(indexOfPort + 1));
            return Tuple.Create(ip, port);
        }

        protected byte[] ReadPacketFile(string file)
        {
            file = Path.Combine(TestDataFolder, file);

            Assert.IsTrue(File.Exists(file), $"{file} doesn't exist");

            var fileText = File.ReadAllText(file);

            Assert.IsNotEmpty(fileText, $"{file} is empty");

            var bytes = fileText.FromHexStringToBytes();
            return bytes;
        }

        protected List<T> GetPackageTypeList<T>(string file)
        {
            file = Path.Combine(TestDataFolder, file);

            FileAssert.Exists(file);

            var commands = File.ReadAllLines(file)
                .Select(x => (T)Enum.Parse(typeof(T), x))
                .ToList();

            return commands;
        }
    }
}