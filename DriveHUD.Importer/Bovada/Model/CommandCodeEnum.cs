//-----------------------------------------------------------------------
// <copyright file="CommandCodeEnum.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Command codes
    /// </summary>
    [Serializable]
    public enum CommandCodeEnum
    {
        None = 0,
        HandNumberTableName = 1,
        PlayerAction = 2,
        SetStack = 3,
        HandPhaseV2 = 4,
        CardShown = 5,
        PocketCards = 6,
        PlayerAdded = 7,
        PlayerRemoved = 8,
        CommunityCard = 9,
        DealerSeat = 10,
        TourneyPlayerFinished = 11
    }
}