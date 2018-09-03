//-----------------------------------------------------------------------
// <copyright file="PokerMasterPacketManager.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.PokerMaster.Model;
using System;

namespace DriveHUD.Importers.PokerMaster
{
    internal class PokerMasterPacketManager : PacketManager<PokerMasterPackage>
    {
        private static readonly byte[] startingPacketBytes = new byte[] { 254, 0 };

        private const int packetHeaderLength = 5;

        protected override int PacketHeaderLength
        {
            get
            {
                return packetHeaderLength;
            }
        }

        public override bool IsStartingPacket(byte[] bytes)
        {
            return StartsWith(bytes, startingPacketBytes);
        }

        public override int ReadPacketLength(byte[] bytes)
        {
            if (bytes.Length < PacketHeaderLength)
            {
                throw new ArgumentException(nameof(bytes), $"Packet must have more than {PacketHeaderLength} bytes");
            }

            var numArray = new byte[] { bytes[1], bytes[2], bytes[3], bytes[4] };

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(numArray);
            }

            return BitConverter.ToInt32(numArray, 0);
        }
    }
}