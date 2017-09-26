//-----------------------------------------------------------------------
// <copyright file="HoleCardsLayoutCreator.cs" company="Ace Poker Solutions">
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
    public class HoleCardsLayoutCreator : ReportLayoutCreator
    {
        private readonly static string[] defaultColumns = new[] { nameof(Indicators.VPIP), nameof(Indicators.PFR), nameof(Indicators.ThreeBet), nameof(Indicators.ThreeBetCall),
            nameof(Indicators.WTSD), nameof(Indicators.AggPr), nameof(Indicators.Agg), nameof(Indicators.WSWSF), nameof(Indicators.FlopCBet), nameof(Indicators.Steal) };

        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_DownCards", nameof(HoleCardsReportRecord.Cards), GridViewLength.Auto));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Reports_Column_TotalWon", nameof(Indicators.TotalWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(Indicators.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));
            gridView.Columns.Add(AddPercentile("Reports_Column_Winning", nameof(HoleCardsReportRecord.WonHandProc)));

            AddDefaultStats(gridView, defaultColumns);

            foreach (var column in gridView.Columns)
            {
                column.Width = GetColumnWidth(column) + 10;
            }
        }
    }
}