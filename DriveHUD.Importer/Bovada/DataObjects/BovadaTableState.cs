//-----------------------------------------------------------------------
// <copyright file="BovadaTableState.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Bovada table states (based on bovada stream)
    /// </summary>
    internal enum BovadaTableState
    {
        Unknown = 0,
        Preparing = 1,
        Initializing = 2,
        PostingBlinds = 4,
        Preflop = 8,
        PostFlop = 16,
        Turn = 32,
        River = 64,
        TournamentShowdown = 4096,
        TournamentShowStacks = 8192,
        ShowDown = 32768,
        ShowStacks = 65536
    }
}