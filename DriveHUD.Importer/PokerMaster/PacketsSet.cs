//-----------------------------------------------------------------------
// <copyright file="PackageSet.cs" company="Ace Poker Solutions">
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.PokerMaster
{
    internal class PacketsSet<T> where T : class
    {
        private const int ProcessedPacketsMaxSize = 150;

        private SortedSet<uint> processedPackets = new SortedSet<uint>();

        private SortedDictionary<uint, SubPacket<T>> packets = new SortedDictionary<uint, SubPacket<T>>();

        public SubPacket<T> AddSubPacket(byte[] body, DateTime createdDate, uint sequenceNumber)
        {
            return AddPacket(body, 0, createdDate, sequenceNumber, false);
        }

        public SubPacket<T> AddStartingPacket(byte[] body, int expectedLength, DateTime createdDate, uint sequenceNumber)
        {
            return AddPacket(body, expectedLength, createdDate, sequenceNumber, true);
        }

        private SubPacket<T> AddPacket(byte[] body, int expectedLength, DateTime createdDate, uint sequenceNumber, bool isStarting)
        {
            if (processedPackets.Contains(sequenceNumber))
            {
                return null;
            }

            var subPacket = new SubPacket<T>(body, expectedLength, createdDate, sequenceNumber, isStarting);

            packets.Add(sequenceNumber, subPacket);

            processedPackets.Add(sequenceNumber);

            if (TryAssemblyPacket(out SubPacket<T> reassembliedPacket))
            {
                return reassembliedPacket;
            }

            return subPacket;
        }

        private bool TryAssemblyPacket(out SubPacket<T> subPacket)
        {
            subPacket = null;

            SubPacket<T> startingPacket = null;

            var packetsToRemove = new List<uint>();

            foreach (var packet in packets.Values.ToArray())
            {
                if (packet.IsStarting)
                {
                    if (packet.IsCompleted)
                    {
                        subPacket = packet;
                        Remove(packet);
                        return true;
                    }

                    packetsToRemove.Clear();
                    packetsToRemove.Add(packet.SequenceNumber);

                    startingPacket = packet.Clone();

                    continue;
                }

                if (startingPacket == null)
                {
                    continue;
                }

                var packetBytes = packet.Bytes.ToArray();

                if (startingPacket.CanAddSubPacket(packetBytes, packet.CreatedDate))
                {
                    packetsToRemove.Add(packet.SequenceNumber);

                    if (startingPacket.CanCompleteBySubPacket(packetBytes, packet.SequenceNumber))
                    {
                        startingPacket.Add(packetBytes);
                        subPacket = startingPacket;
                        packetsToRemove.ForEach(x => packets.Remove(x));
                        return true;
                    }

                    startingPacket.Add(packetBytes);
                }
            }

            return false;
        }

        public void Remove(SubPacket<T> packet)
        {
            packets.Remove(packet.SequenceNumber);
        }

        public void RemoveExpiredPackets(int expirationPeriod = 3500)
        {
            packets.RemoveByCondition(p => p.Value.IsExpired(expirationPeriod));

            if (processedPackets.Count > ProcessedPacketsMaxSize)
            {
                var packetsToRemove = processedPackets.Take(processedPackets.Count - ProcessedPacketsMaxSize).ToArray();
                processedPackets.RemoveRange(packetsToRemove);
            }
        }
    }
}