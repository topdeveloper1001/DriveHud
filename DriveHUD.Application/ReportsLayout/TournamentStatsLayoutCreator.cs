//-----------------------------------------------------------------------
// <copyright file="TournamentStatsLayoutCreator.cs" company="Ace Poker Solutions">
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
    public class TournamentStatsLayoutCreator : ReportLayoutCreator
    {
        private readonly static string[] defaultColumns = new[] { nameof(Indicators.VPIP), nameof(Indicators.PFR), nameof(Indicators.ThreeBet),
            nameof(Indicators.WTSD), nameof(Indicators.WSSD), nameof(Indicators.WSWSF), nameof(Indicators.AggPr), nameof(Indicators.Agg),  nameof(Indicators.FlopCBet),
            nameof(Indicators.FoldCBet), nameof(Indicators.Steal), nameof(Indicators.BlindsReraiseSteal), nameof(Indicators.BlindsFoldSteal), nameof(Indicators.FourBet),
            nameof(Indicators.UO_PFR_EP), nameof(Indicators.UO_PFR_MP), nameof(Indicators.UO_PFR_CO), nameof(Indicators.UO_PFR_BN), nameof(Indicators.UO_PFR_SB) };

        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_TableType", nameof(TournamentReportRecord.TableType)));
            gridView.Columns.Add(Add("Reports_Column_GameType", nameof(TournamentReportRecord.GameType)));
            gridView.Columns.Add(Add("Reports_Column_Speed", nameof(TournamentReportRecord.TournamentSpeed)));
            gridView.Columns.Add(AddFinancial("Reports_Column_Buyin", nameof(TournamentReportRecord.BuyIn)));

            gridView.Columns.Add(AddPercentile("Reports_Column_VPIP", nameof(TournamentReportRecord.VPIP)));
            gridView.Columns.Add(AddPercentile("Reports_Column_PFR", nameof(TournamentReportRecord.PFR)));
            gridView.Columns.Add(AddPercentile("Reports_Column_AggPercent", nameof(TournamentReportRecord.AggPr)));
            gridView.Columns.Add(AddPercentile("Reports_Column_Agg", nameof(TournamentReportRecord.Agg)));
            gridView.Columns.Add(AddPercentile("Reports_Column_3Bet", nameof(TournamentReportRecord.ThreeBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WTSD", nameof(TournamentReportRecord.WTSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSSD", nameof(TournamentReportRecord.WSSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSWSF", nameof(TournamentReportRecord.WSWSF)));
            gridView.Columns.Add(AddPercentile("Reports_Column_FlopToCBet", nameof(TournamentReportRecord.FlopCBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_FoldToFlopCBet", nameof(TournamentReportRecord.FoldCBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_Steal", nameof(TournamentReportRecord.Steal)));
            gridView.Columns.Add(AddPercentile("Reports_Column_BlindsReRaiseSteal", nameof(TournamentReportRecord.BlindsReraiseSteal)));
            gridView.Columns.Add(AddPercentile("Reports_Column_BlindsFoldToSteal", nameof(TournamentReportRecord.BlindsFoldSteal)));
            gridView.Columns.Add(AddPercentile("Reports_Column_4Bet", nameof(TournamentReportRecord.FourBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_UO_PFR_EP", nameof(TournamentReportRecord.UO_PFR_EP)));
            gridView.Columns.Add(AddPercentile("Reports_Column_UO_PFR_MP", nameof(TournamentReportRecord.UO_PFR_MP)));
            gridView.Columns.Add(AddPercentile("Reports_Column_UO_PFR_CO", nameof(TournamentReportRecord.UO_PFR_CO)));
            gridView.Columns.Add(AddPercentile("Reports_Column_UO_PFR_BTN", nameof(TournamentReportRecord.UO_PFR_BN)));
            gridView.Columns.Add(AddPercentile("Reports_Column_UO_PFR_SB", nameof(TournamentReportRecord.UO_PFR_SB)));

            AddDefaultStats(gridView, defaultColumns);

            foreach (var column in gridView.Columns)
            {
                column.Width = GetColumnWidth(column);
            }
        }
    }
}