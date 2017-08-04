﻿//-----------------------------------------------------------------------
// <copyright file="OpponentAnalysisLayoutCreator.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Reflection;
using Model.Data;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class OpponentAnalysisLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_PlayerName", ReflectionHelper.GetPath<Indicators>(o => o.Source.PlayerName)));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Reports_Column_TotalWon", nameof(Indicators.TotalWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(Indicators.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));
            gridView.Columns.Add(AddPercentile("Reports_Column_VPIP", nameof(Indicators.VPIP)));
            gridView.Columns.Add(AddPercentile("Reports_Column_PFR", nameof(Indicators.PFR)));
            gridView.Columns.Add(AddPercentile("Reports_Column_3Bet", nameof(Indicators.ThreeBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_3BetCall", nameof(Indicators.ThreeBetCall)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WTSD", nameof(Indicators.WTSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSSD", nameof(Indicators.WSSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_AggPercent", nameof(Indicators.AggPr)));
            gridView.Columns.Add(AddPercentile("Reports_Column_AF", nameof(Indicators.Agg)));

            gridView.Columns.Add(AddPercentile("Reports_Column_WSWSF", nameof(Indicators.WSWSF)));
            gridView.Columns.Add(AddPercentile("Reports_Column_FlopToCBet", nameof(Indicators.FlopCBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_Steal", nameof(Indicators.Steal)));
            gridView.Columns.Add(AddPercentile("Reports_Column_BlindsReRaiseSteal", nameof(Indicators.BlindsReraiseSteal)));
            gridView.Columns.Add(AddPercentile("Reports_Column_BlindsFoldToSteal", nameof(Indicators.BlindsFoldSteal)));
            gridView.Columns.Add(AddPercentile("Reports_Column_4BetRange", nameof(Indicators.FourBetRange)));
            gridView.Columns.Add(AddPercentile("Reports_Column_UO_PFR_EP", nameof(Indicators.UO_PFR_EP)));
            gridView.Columns.Add(AddPercentile("Reports_Column_UO_PFR_MP", nameof(Indicators.UO_PFR_MP)));
            gridView.Columns.Add(AddPercentile("Reports_Column_UO_PFR_CO", nameof(Indicators.UO_PFR_CO)));
            gridView.Columns.Add(AddPercentile("Reports_Column_UO_PFR_BTN", nameof(Indicators.UO_PFR_BN)));
            gridView.Columns.Add(AddPercentile("Reports_Column_UO_PFR_SB", nameof(Indicators.UO_PFR_SB)));

            base.AddDefaultStats(gridView);

            foreach (var column in gridView.Columns)
            {
                column.Width = GetColumnWidth(column.Header as string);
            }
        }
    }
}