﻿//-----------------------------------------------------------------------
// <copyright file="TournamentChartSeries.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Data;
using Model.Enums;
using ReactiveUI;
using System;

namespace DriveHUD.ViewModels
{
    [Serializable]
    public class TournamentChartSeries : BaseChartSeries
    {
        private ChartTournamentSeriesType seriesType;

        public ChartTournamentSeriesType SeriesType
        {
            get
            {
                return seriesType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref seriesType, value);
            }
        }

        public Action<ChartSeriesItem, ChartSeriesItem, TournamentReportRecord> UpdateChartSeriesItem
        {
            get;
            set;
        }
    }
}