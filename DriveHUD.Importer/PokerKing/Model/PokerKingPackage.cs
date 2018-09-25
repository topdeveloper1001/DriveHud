//-----------------------------------------------------------------------
// <copyright file="PokerKingPackage.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ProtoBuf;
using System;

namespace DriveHUD.Importers.PokerKing.Model
{
    [ProtoContract]
    internal class PokerKingPackage
    {
        [ProtoMember(1)]
        public DateTime Timestamp { get; set; }

        [ProtoMember(2)]
        public PackageType PackageType { get; set; }

        [ProtoMember(3)]
        public uint UserId { get; set; }

        [ProtoMember(4)]
        public int RoomId { get; set; }

        [ProtoMember(5)]
        public byte[] Body { get; set; }

#if DEBUG
        public override string ToString()
        {
            return $"{PackageType}: {Timestamp}; BodyLength: {Body.Length}";
        }
#endif
    }
}