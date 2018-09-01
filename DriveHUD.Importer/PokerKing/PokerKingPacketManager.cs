//-----------------------------------------------------------------------
// <copyright file="PokerKingPacketManager.cs" company="Ace Poker Solutions">
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
using DriveHUD.Importers.PokerKing.Model;
using System;

namespace DriveHUD.Importers.PokerKing
{
    internal class PokerKingPacketManager : PacketManager<PokerKingPackage>
    {
        private const int packetHeaderLength = 2;

        protected override int PacketHeaderLength
        {
            get
            {
                return packetHeaderLength;
            }
        }

        public override bool IsStartingPacket(byte[] bytes)
        {
            return bytes.Length > 5 &&
              bytes[2] == 0x27 &&
              bytes[4] == 0x00;
        }

        public override int ReadPacketLength(byte[] bytes)
        {
            if (bytes.Length < packetHeaderLength)
            {
                throw new ArgumentException(nameof(bytes), $"Packet must have more than {packetHeaderLength} bytes");
            }

            var numArray = new byte[] { bytes[0], bytes[1] };

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(numArray);
            }

            return BitConverter.ToUInt16(numArray, 0);
        }
    }
}