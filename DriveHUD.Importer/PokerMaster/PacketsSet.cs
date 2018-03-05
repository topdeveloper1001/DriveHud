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
        private List<SubPacket<T>> packets = new List<SubPacket<T>>();

        public SubPacket<T> AddSubPacket(byte[] bytes, DateTime createdDate)
        {
            if (packets.Count == 0)
            {
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

        public SubPacket<T> AddStartingPacket(byte[] body, int expectedLength, DateTime dateCreated)
        {
            var subPacket = new SubPacket<T>(body, expectedLength, dateCreated);
            packets.Add(subPacket);
            return subPacket;
        }

        public void Remove(SubPacket<T> packet)
        {
            packets.Remove(packet);
        }

        public void RemoveExpiredPackets(int expirationPeriod = 3500)
        {
            packets.RemoveByCondition(x => x.IsExpired(expirationPeriod));
        }
    }
}