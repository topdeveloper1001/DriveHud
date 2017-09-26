//-----------------------------------------------------------------------
// <copyright file="TournamentPokerSiteLayoutCreator.cs" company="Ace Poker Solutions">
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
    public class TournamentPokerSiteLayoutCreator : ReportLayoutCreator
    {
        private readonly static string[] defaultColumns = new[] { nameof(Indicators.VPIP), nameof(Indicators.PFR), nameof(Indicators.ThreeBet),
             nameof(Indicators.AggPr), nameof(Indicators.Agg), nameof(Indicators.WTSD), nameof(Indicators.WSSD), nameof(Indicators.WSWSF) };

        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_PokerSite", nameof(TournamentReportRecord.PokerSite)));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(TournamentReportRecord.TotalHands)));
            gridView.Columns.Add(AddFinancial("Reports_Column_TotalWon", nameof(TournamentReportRecord.Won)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(TournamentReportRecord.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));

            gridView.Columns.Add(AddPercentile("Reports_Column_VPIP", nameof(TournamentReportRecord.VPIP)));
            gridView.Columns.Add(AddPercentile("Reports_Column_PFR", nameof(TournamentReportRecord.PFR)));
            gridView.Columns.Add(AddPercentile("Reports_Column_Agg", nameof(TournamentReportRecord.Agg)));
            gridView.Columns.Add(AddPercentile("Reports_Column_AggPercent", nameof(TournamentReportRecord.AggPr)));
            gridView.Columns.Add(AddPercentile("Reports_Column_3Bet", nameof(TournamentReportRecord.ThreeBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WTSD", nameof(TournamentReportRecord.WTSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSSD", nameof(TournamentReportRecord.WSSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSWSF", nameof(TournamentReportRecord.WSWSF)));

            AddDefaultStats(gridView, defaultColumns);
        }
    }
}