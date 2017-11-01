//-----------------------------------------------------------------------
// <copyright file="RoundType.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.GGNetwork.Model
{
    public enum RoundType
    {
        Unknown = 0,
        PreFlop = 1,
        Flop = 2,
        Turn = 3,
        River = 4,
        ShowDown = 10,
        End = 99
    }
}