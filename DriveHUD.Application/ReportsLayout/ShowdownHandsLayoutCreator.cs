//-----------------------------------------------------------------------
// <copyright file="ShowdownHandsLayoutCreator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Data;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class ShowdownHandsLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_ShowdownHand", nameof(ShowdownHandsReportRecord.ShowdownHandString), GetColumnHeaderWidth("Straight Flush: With 2 Pocket Cards")));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(ShowdownHandsReportRecord.TotalHands)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WonPercent", nameof(ShowdownHandsReportRecord.WonHandProc)));
            gridView.Columns.Add(AddFinancial("Reports_Column_NetWon", nameof(ShowdownHandsReportRecord.NetWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(ShowdownHandsReportRecord.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));
            gridView.Columns.Add(AddPercentile("Reports_Column_VPIP", nameof(ShowdownHandsReportRecord.VPIP)));
            gridView.Columns.Add(AddPercentile("Reports_Column_PFR", nameof(ShowdownHandsReportRecord.PFR)));
            gridView.Columns.Add(AddPercentile("Reports_Column_AggPercent", nameof(ShowdownHandsReportRecord.AggPr)));
            gridView.Columns.Add(AddPercentile("Reports_Column_Agg", nameof(ShowdownHandsReportRecord.Agg)));
            gridView.Columns.Add(AddPercentile("Reports_Column_3Bet", nameof(ShowdownHandsReportRecord.ThreeBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WTSD", nameof(ShowdownHandsReportRecord.WTSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSSD", nameof(ShowdownHandsReportRecord.WSSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSWSF", nameof(ShowdownHandsReportRecord.WSWSF)));

            base.AddDefaultStats(gridView);

            for (int i = 1; i < gridView.Columns.Count; i++)
            {
                gridView.Columns[i].Width = GetColumnWidth(gridView.Columns[i]) + 10;
            }
        }
    }
}