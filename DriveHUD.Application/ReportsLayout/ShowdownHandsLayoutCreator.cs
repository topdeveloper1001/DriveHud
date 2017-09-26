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
        private readonly static string[] defaultColumns = new[] { nameof(Indicators.VPIP), nameof(Indicators.PFR), nameof(Indicators.AggPr), nameof(Indicators.Agg),
            nameof(Indicators.ThreeBet), nameof(Indicators.WTSD), nameof(Indicators.WSSD), nameof(Indicators.WSWSF) };

        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_ShowdownHand", nameof(ShowdownHandsReportRecord.ShowdownHandString), GetColumnHeaderWidth("Straight Flush: With 2 Pocket Cards")));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WonPercent", nameof(ShowdownHandsReportRecord.WonHandProc)));
            gridView.Columns.Add(AddFinancial("Reports_Column_NetWon", nameof(Indicators.NetWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(Indicators.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));

            AddDefaultStats(gridView, defaultColumns);

            for (int i = 1; i < gridView.Columns.Count; i++)
            {
                gridView.Columns[i].Width = GetColumnWidth(gridView.Columns[i]) + 10;
            }
        }
    }
}