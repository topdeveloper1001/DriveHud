using DriveHUD.Common.Wpf.Helpers;
using Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class TournamentStatsLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Table Type", nameof(TournamentReportRecord.TableType)));
            gridView.Columns.Add(Add("Game Type", nameof(TournamentReportRecord.GameType)));
            gridView.Columns.Add(Add("Speed", nameof(TournamentReportRecord.TournamentSpeed)));
            gridView.Columns.Add(AddFinancial("Buy-in", nameof(TournamentReportRecord.BuyIn)));
            gridView.Columns.Add(AddPercentile("VPIP", nameof(TournamentReportRecord.VPIP)));
            gridView.Columns.Add(AddPercentile("PFR", nameof(TournamentReportRecord.PFR)));
            gridView.Columns.Add(AddPercentile("Agg%", nameof(TournamentReportRecord.AggPr)));
            gridView.Columns.Add(AddPercentile("Agg", nameof(TournamentReportRecord.Agg)));
            gridView.Columns.Add(AddPercentile("3-Bet%", nameof(TournamentReportRecord.ThreeBet)));
            gridView.Columns.Add(AddPercentile("WTSD", nameof(TournamentReportRecord.WTSD)));
            gridView.Columns.Add(AddPercentile("W$SD", nameof(TournamentReportRecord.WSSD)));
            gridView.Columns.Add(AddPercentile("WWSF", nameof(TournamentReportRecord.WSWSF)));
            gridView.Columns.Add(AddPercentile("Flop c-bet%", nameof(TournamentReportRecord.FlopCBet)));
            gridView.Columns.Add(AddPercentile("Fold to Flop C-bet%", nameof(TournamentReportRecord.FoldCBet)));
            gridView.Columns.Add(AddPercentile("Steal %", nameof(TournamentReportRecord.Steal)));
            gridView.Columns.Add(AddPercentile("Blinds re-raise steal", nameof(TournamentReportRecord.BlindsReraiseSteal)));
            gridView.Columns.Add(AddPercentile("Blinds fold to steal", nameof(TournamentReportRecord.BlindsFoldSteal)));
            gridView.Columns.Add(AddPercentile("4-bet%", nameof(TournamentReportRecord.FourBet)));
            gridView.Columns.Add(AddPercentile("UO-PFR EP%", nameof(TournamentReportRecord.UO_PFR_EP)));
            gridView.Columns.Add(AddPercentile("UO-PFR MP%", nameof(TournamentReportRecord.UO_PFR_MP)));
            gridView.Columns.Add(AddPercentile("UO-PFR CO%", nameof(TournamentReportRecord.UO_PFR_CO)));
            gridView.Columns.Add(AddPercentile("UO-PFR BN%", nameof(TournamentReportRecord.UO_PFR_BN)));
            gridView.Columns.Add(AddPercentile("UO-PFR SB%", nameof(TournamentReportRecord.UO_PFR_SB)));

            base.AddDefaultStats(gridView);

            foreach (var column in gridView.Columns)
            {
                column.Width = GetColumnWidth(column.Header as string);
            }
        }
    }
}
