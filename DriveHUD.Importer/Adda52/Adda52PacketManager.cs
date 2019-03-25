//-----------------------------------------------------------------------
// <copyright file="Adda52PacketManager.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.AndroidBase;
using System;

namespace DriveHUD.Importers.Adda52
{
    internal class Adda52PacketManager : PacketManager<Adda52Package>
    {
        // header length can be 2 or 4 bytes, so it will handled in package builder
        protected override int PacketHeaderLength => 0;

        public override bool IsStartingPacket(byte[] bytes)
        {
            return bytes != null && bytes.Length > 0 && (bytes[0] == 0x82 || bytes[0] == 0x80 || bytes[0] == 0x81);
        }

        public override int ReadPacketLength(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
            {
                return 0;
            }

            if (bytes[1] == 0x7E && (bytes[0] == 0x82 || bytes[0] == 0x80 || bytes[0] == 0x81))
            {
                var numArray = new byte[] { bytes[2], bytes[3] };

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(numArray);
                }

                return BitConverter.ToUInt16(numArray, 0) + 4;
            }

            return bytes[1] + 2;
        }
    }
}