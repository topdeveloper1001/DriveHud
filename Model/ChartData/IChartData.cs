﻿//-----------------------------------------------------------------------
// <copyright file="IChartData.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model.Data;
using Model.Enums;
using System;
using System.Collections.Generic;

namespace Model.ChartData
{
    public interface ITournamentChartData
    {
        DateTime GetFirstDate(DateTime maxDateTime);

        IEnumerable<TournamentReportRecord> Create(IList<Tournaments> tournaments, TournamentChartFilterType tournamentChartFilterType);
    }
}