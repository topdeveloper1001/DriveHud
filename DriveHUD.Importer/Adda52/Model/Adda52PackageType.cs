//-----------------------------------------------------------------------
// <copyright file="Adda52PackageType.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.Adda52.Model
{
    internal enum Adda52PackageType
    {
        RoomData = 0,
        SeatInfo = 1,
        Ante = 2,
        Blinds = 3,
        Dealer = 4,
        UserAction = 5,
        GameStart = 6,
        RoundEnd = 7,
        CommunityCard = 8,
        Pot = 9,
        Winner = 9,
        UncalledBet = 10,
        HoleCard = 11,
        AccessToken = 12,
        MTTInfo = 13,
        MTTTables = 14,
        MTTPrizes = 15
    }
}