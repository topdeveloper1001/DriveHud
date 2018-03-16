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
        private const int ProcessedPacketsMaxSize = 50;

        private SortedSet<uint> processedPackets = new SortedSet<uint>();

        private List<SubPacket<T>> packets = new List<SubPacket<T>>();

        private List<SubPacket<T>> secondPackets = new List<SubPacket<T>>();

        public SubPacket<T> AddSubPacket(byte[] bytes, DateTime createdDate, uint sequenceNumber)
        {
            if (processedPackets.Contains(sequenceNumber))
            {
                return null;
            }

            processedPackets.Add(sequenceNumber);

            if (packets.Count == 0)
            {
                // add packet without heading packet to the special collection
                var secondPacket = new SubPacket<T>(bytes, 0, createdDate, sequenceNumber);
                secondPackets.Add(secondPacket);               
                return null;
            }

            if (packets.Count == 1 && packets[0].CanAddSubPacket(bytes, createdDate))
            {
                packets[0].Add(bytes);
                return packets[0];
            }

            var subPacket = packets.FirstOrDefault(x => x.CanCompleteBySubPacket(bytes, createdDate));

            if (subPacket != null)
            {
                subPacket.Add(bytes);
                return subPacket;
            }

            var packetCanBeCompleted = packets
                .LastOrDefault(x => x.CanAddSubPacket(bytes, createdDate));

            if (packetCanBeCompleted != null)
            {
                packetCanBeCompleted.Add(bytes);
                return packetCanBeCompleted;
            }

            return null;
        }

        public SubPacket<T> AddStartingPacket(byte[] body, int expectedLength, DateTime dateCreated, uint sequenceNumber)
        {
            if (processedPackets.Contains(sequenceNumber))
            {
                return null;
            }

            var subPacket = new SubPacket<T>(body, expectedLength, dateCreated, sequenceNumber);

            if (secondPackets.Count > 0)
            {
                var secondPacket = secondPackets.FirstOrDefault(x => subPacket.CanCompleteBySubPacket(x.Bytes.ToArray(), x.SequenceNumber));

                if (secondPacket != null)
                {
                    subPacket.Add(secondPacket.Bytes.ToArray());
                    secondPackets.Remove(secondPacket);
                }
            }

            packets.Add(subPacket);

            processedPackets.Add(sequenceNumber);

            return subPacket;
        }

        public void Remove(SubPacket<T> packet)
        {
            packets.Remove(packet);
        }

        public void RemoveExpiredPackets(int expirationPeriod = 3500)
        {
            packets.RemoveByCondition(p => p.IsExpired(expirationPeriod));
            secondPackets.RemoveByCondition(p => p.IsExpired(expirationPeriod));

            if (processedPackets.Count > ProcessedPacketsMaxSize)
            {
                var packetsToRemove = processedPackets.Take(processedPackets.Count - ProcessedPacketsMaxSize).ToArray();
                processedPackets.RemoveRange(packetsToRemove);
            }
        }
    }
}