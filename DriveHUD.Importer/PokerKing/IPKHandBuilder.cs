﻿//-----------------------------------------------------------------------
// <copyright file="IPKHandBuilder.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.PokerKing.Model;
using HandHistories.Objects.Hand;
using System.Collections.Generic;

namespace DriveHUD.Importers.PokerKing
{
    internal interface IPKHandBuilder
    {
        bool TryBuild(PokerKingPackage package, int identifier, out HandHistory handHistory);

        bool IsRoomSnapShotAvailable(PokerKingPackage package);

        NoticeGameSnapShot GetNoticeRoomSnapShot(PokerKingPackage package);

        int GetFastFoldRoomByUser(uint userId);

        void CleanRoom(int identifier, int roomId);

        void CleanFastFoldRooms(PokerKingPackage package, int identifier, out List<HandHistory> handHistories);
    }
}