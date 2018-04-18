//-----------------------------------------------------------------------
// <copyright file="PacketManager.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.PokerMaster.Model;
using System;
using System.Collections.Generic;

namespace DriveHUD.Importers.PokerMaster
{
    internal class PacketManager : IPacketManager
    {
        private static readonly int packetHeaderLength = 5;

        private static readonly byte[] startingPacketBytes = new byte[] { 254, 0 };

        private Dictionary<SourceDestination, PacketsSet<Package>> packetsBytes = new Dictionary<SourceDestination, PacketsSet<Package>>();

        public static bool IsStartingPacket(byte[] bytes)
        {
            return StartsWith(bytes, startingPacketBytes);
        }

        public bool TryParse(CapturedPacket packet, out Package package)
        {
            var packetsSet = GetPacketsSet(packet);

            var subPacket = !IsStartingPacket(packet.Bytes) ?
              packetsSet.AddSubPacket(packet.Bytes, packet.CreatedTimeStamp, packet.SequenceNumber) :
              packetsSet.AddStartingPacket(packet.Bytes, ReadPacketLength(packet.Bytes), packet.CreatedTimeStamp, packet.SequenceNumber);

            if (subPacket == null || !subPacket.TryParse(packetHeaderLength, out package))
            {
                packetsSet.RemoveExpiredPackets();
                package = null;
                return false;
            }

            return true;
        }

        private PacketsSet<Package> GetPacketsSet(CapturedPacket packet)
        {
            var sourceDestination = new SourceDestination
            {
                IPSource = packet.IPSource,
                PortSource = packet.Source.Port,
                IPDestination = packet.IPDestination,
                PortDestination = packet.Destination.Port
            };

            if (packetsBytes.TryGetValue(sourceDestination, out PacketsSet<Package> packetsSet))
            {
                return packetsSet;
            }

            packetsSet = new PacketsSet<Package>();

            packetsBytes.Add(sourceDestination, packetsSet);

            return packetsSet;
        }

        private static bool StartsWith(byte[] bytes, byte[] value)
        {
            if (bytes == null || value == null || bytes.Length < value.Length)
            {
                return false;
            }

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != bytes[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static int ReadPacketLength(byte[] bytes)
        {
            if (bytes.Length < packetHeaderLength)
            {
                throw new ArgumentException(nameof(bytes), $"Packet must have more than {packetHeaderLength} bytes");
            }

            var numArray = new byte[] { bytes[1], bytes[2], bytes[3], bytes[4] };

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(numArray);
            }

            return BitConverter.ToInt32(numArray, 0);
        }

        private class SourceDestination
        {
            public string IPSource { get; set; }

            public int PortSource { get; set; }

            public string IPDestination { get; set; }

            public int PortDestination { get; set; }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + IPSource.GetHashCode();
                    hashcode = (hashcode * 31) + PortSource;
                    hashcode = (hashcode * 31) + IPDestination.GetHashCode();
                    hashcode = (hashcode * 31) + PortDestination;
                    return hashcode;
                }
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SourceDestination);
            }

            public bool Equals(SourceDestination obj)
            {
                return obj != null && IPSource == obj.IPSource && PortSource == obj.PortSource &&
                    IPDestination == obj.IPDestination && PortDestination == obj.PortDestination;
            }
        }
    }
}