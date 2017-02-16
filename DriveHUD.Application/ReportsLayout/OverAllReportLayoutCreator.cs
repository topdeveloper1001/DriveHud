using DriveHUD.Common.Reflection;
using DriveHUD.Common.Wpf.Helpers;
using Model.Data;
using System.Collections.Generic;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class OverAllReportLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Total Hands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Total Won", ReflectionHelper.GetPath<Indicators>(o => o.Source.NetWon)));
            gridView.Columns.Add(Add("bb/100", nameof(Indicators.BB)));
            gridView.Columns.Add(AddPercentile("VPIP", nameof(Indicators.VPIP)));
            gridView.Columns.Add(AddPercentile("PFR", nameof(Indicators.PFR)));
            gridView.Columns.Add(AddPercentile("3-Bet%", nameof(Indicators.ThreeBet)));
            gridView.Columns.Add(AddPercentile("3-Bet Call%", nameof(Indicators.ThreeBetCall)));
            gridView.Columns.Add(AddPercentile("WTSD%", nameof(Indicators.WTSD)));
            gridView.Columns.Add(AddPercentile("W$SD", nameof(Indicators.WSSD)));
            gridView.Columns.Add(AddPercentile("Agg%", nameof(Indicators.AggPr)));
            gridView.Columns.Add(AddPercentile("AF", nameof(Indicators.Agg)));
            gridView.Columns.Add(AddFinancial("$EV Diff", nameof(Indicators.EVDiff)));

            gridView.Columns.Add(AddPercentile("W$WSF", nameof(Indicators.WSWSF)));
            gridView.Columns.Add(AddPercentile("Flop C-Bet%", nameof(Indicators.FlopCBet)));
            gridView.Columns.Add(AddPercentile("Steal%", nameof(Indicators.Steal)));
            gridView.Columns.Add(AddPercentile("Blinds Re-raise Steal", nameof(Indicators.BlindsReraiseSteal)));
            gridView.Columns.Add(AddPercentile("Blinds Fold to Steal", nameof(Indicators.BlindsFoldSteal)));
            gridView.Columns.Add(AddPercentile("4Bet Range", nameof(Indicators.FourBetRange)));
            gridView.Columns.Add(AddPercentile("UO-PFR EP%", nameof(Indicators.UO_PFR_EP)));
            gridView.Columns.Add(AddPercentile("UO-PFR MP%", nameof(Indicators.UO_PFR_MP)));
            gridView.Columns.Add(AddPercentile("UO-PFR CO%", nameof(Indicators.UO_PFR_CO)));
            gridView.Columns.Add(AddPercentile("UO-PFR BN%", nameof(Indicators.UO_PFR_BN)));
            gridView.Columns.Add(AddPercentile("UO-PFR SB%", nameof(Indicators.UO_PFR_SB)));

            base.AddDefaultStats(gridView, nameof(Indicators.EVDiff));

            foreach (var column in gridView.Columns)
            {
                column.Width = GetColumnWidth(column.Header as string);
            }
        }
    }
}