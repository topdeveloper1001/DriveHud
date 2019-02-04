//-----------------------------------------------------------------------
// <copyright file="ChartSeries.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model.Enums;
using ReactiveUI;
using System;

namespace DriveHUD.ViewModels
{
    [Serializable]
    public class ChartSeries : BaseChartSeries
    {
        #region Properties

        private ChartCashSeriesWinningType chartCashSeriesWinningType;

        public ChartCashSeriesWinningType ChartCashSeriesWinningType
        {
            get
            {
                return chartCashSeriesWinningType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref chartCashSeriesWinningType, value);
            }
        }

        private ChartCashSeriesValueType chartCashSeriesValueType;

        public ChartCashSeriesValueType ChartCashSeriesValueType
        {
            get
            {
                return chartCashSeriesValueType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref chartCashSeriesValueType, value);
            }
        }

        public Action<ChartSeriesItem, ChartSeriesItem, Playerstatistic, int, int> UpdateChartSeriesItem
        {
            get;
            set;
        }

        #endregion
    }
}