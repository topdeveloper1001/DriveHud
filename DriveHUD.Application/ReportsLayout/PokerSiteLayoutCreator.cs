//-----------------------------------------------------------------------
// <copyright file="PokerSiteLayoutCreator.cs" company="Ace Poker Solutions">
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
    public class PokerSiteLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_PokerSite", nameof(Indicators.PokerSite)));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Reports_Column_TotalWon", nameof(Indicators.TotalWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(Indicators.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));
            gridView.Columns.Add(AddPercentile("Reports_Column_VPIP", nameof(Indicators.VPIP)));
            gridView.Columns.Add(AddPercentile("Reports_Column_PFR", nameof(Indicators.PFR)));
            gridView.Columns.Add(AddPercentile("Reports_Column_Agg", nameof(Indicators.Agg)));
            gridView.Columns.Add(AddPercentile("Reports_Column_AggPercent", nameof(Indicators.AggPr)));
            gridView.Columns.Add(AddPercentile("Reports_Column_3Bet", nameof(Indicators.ThreeBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WTSD", nameof(Indicators.WTSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSSD", nameof(Indicators.WSSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSWSF", nameof(Indicators.WSWSF)));

            base.AddDefaultStats(gridView);

            for (int i = 1; i < gridView.Columns.Count; i++)
            {
                gridView.Columns[i].Width = GetColumnWidth(gridView.Columns[i].Header as string) + 10;
            }
        }
    }
}