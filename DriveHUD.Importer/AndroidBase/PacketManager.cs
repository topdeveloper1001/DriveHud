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

using System.Collections.Generic;

namespace DriveHUD.Importers.AndroidBase
{
    internal abstract class PacketManager<T> : IPacketManager<T> where T : class
    {
        protected Dictionary<SourceDestination, PacketsSet<T>> packetsBytes = new Dictionary<SourceDestination, PacketsSet<T>>();

        protected abstract int PacketHeaderLength
        {
            get;
        }

        public abstract bool IsStartingPacket(byte[] bytes);

        public virtual bool TryParse(CapturedPacket packet, out T package, bool takeExpectedLength = false)
        {
            var packetsSet = GetPacketsSet(packet);

            var subPacket = !IsStartingPacket(packet.Bytes) ?
              packetsSet.AddSubPacket(packet.Bytes, packet.CreatedTimeStamp, packet.SequenceNumber) :
              packetsSet.AddStartingPacket(packet.Bytes, ReadPacketLength(packet.Bytes), packet.CreatedTimeStamp, packet.SequenceNumber);

            if (subPacket == null || !subPacket.TryParse(PacketHeaderLength, out package, takeExpectedLength))
            {
                packetsSet.RemoveExpiredPackets();
                package = null;
                return false;
            }

            return true;
        }

        public abstract int ReadPacketLength(byte[] bytes);

        protected virtual PacketsSet<T> GetPacketsSet(CapturedPacket packet)
        {
            var sourceDestination = new SourceDestination
            {
                IPSource = packet.IPSource,
                PortSource = packet.Source.Port,
                IPDestination = packet.IPDestination,
                PortDestination = packet.Destination.Port
            };

            if (packetsBytes.TryGetValue(sourceDestination, out PacketsSet<T> packetsSet))
            {
                return packetsSet;
            }

            packetsSet = new PacketsSet<T>();

            packetsBytes.Add(sourceDestination, packetsSet);

            return packetsSet;
        }

        protected static bool StartsWith(byte[] bytes, byte[] value)
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
    
        protected class SourceDestination
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