﻿//-----------------------------------------------------------------------
// <copyright file="IPokerTable.cs" company="Ace Poker Solutions">
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
using System.Collections.Generic;
using System.Xml;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Interface of poker client table 
    /// </summary>
    internal interface IPokerTable
    {
        uint Uid { get; }

        Dictionary<int, string> PlayersOnTable { get; }

        int HeroSeat { get; }

        DateTime BeginDate { get; }

        string InitialTableTitle { get; }

        IntPtr WindowHandle { get; }

        void ProcessCommand<TObject>(TObject dataObject);
    }
}