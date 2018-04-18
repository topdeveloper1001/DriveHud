//-----------------------------------------------------------------------
// <copyright file="Package.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Importers.PokerMaster.Model
{
    [ProtoContract]
    internal class Package
    {
        [ProtoMember(1)]
        public long Uuid { get; set; }

        [ProtoMember(2)]
        public byte[] Head { get; set; }

        [ProtoMember(3)]
        public long MisSystemTime { get; set; }

        [ProtoMember(4)]
        public byte[] Body { get; set; }

        [ProtoMember(5)]
        public long CurrentSystemTime { get; set; }

        [ProtoMember(6)]
        public long SeqNo { get; set; }

        [ProtoMember(7)]
        public PackageCommand Cmd { get; set; }

        [ProtoMember(8)]
        public int EncodeType { get; set; }

        [ProtoMember(9)]
        public int Version { get; set; }
    }
}