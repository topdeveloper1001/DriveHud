﻿//-----------------------------------------------------------------------
// <copyright file="SCEnterGameRoomRsp.cs" company="Ace Poker Solutions">
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
    internal class SCEnterGameRoomRsp
    {
        [ProtoMember(1)]
        public ErrorCodeType ErrCode { get; set; }

        [ProtoMember(2)]
        public GameRoomInfo GameRoomInfo { get; set; }

        [ProtoMember(3)]
        public long RoomId { get; set; }

        [ProtoMember(4)]
        public long ClubId { get; set; }

        [ProtoMember(5)]
        public string ClubName { get; set; }

        [ProtoMember(6)]
        public string CryptCode { get; set; }
    }
}