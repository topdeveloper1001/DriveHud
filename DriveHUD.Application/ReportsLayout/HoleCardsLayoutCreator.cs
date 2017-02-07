using DriveHUD.Common.Reflection;
using Model.Data;
using System.Threading;

using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class HoleCardsLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Down Cards", nameof(HoleCardsReportRecord.Cards), GridViewLength.Auto));
            gridView.Columns.Add(Add("Total Hands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Total Won", nameof(Indicators.TotalWon)));
            gridView.Columns.Add(Add("bb/100", nameof(Indicators.BB)));
            gridView.Columns.Add(AddPercentile("Winning%", nameof(HoleCardsReportRecord.WonHandProc)));
            gridView.Columns.Add(AddPercentile("VPIP", nameof(Indicators.VPIP)));
            gridView.Columns.Add(AddPercentile("PFR", nameof(Indicators.PFR)));
            gridView.Columns.Add(AddPercentile("3Bet%", nameof(Indicators.ThreeBet)));
            gridView.Columns.Add(AddPercentile("3-Bet Call%", nameof(Indicators.ThreeBetCall)));
            gridView.Columns.Add(AddPercentile("WTSD%", nameof(Indicators.WTSD)));
            gridView.Columns.Add(AddPercentile("Agg%", nameof(Indicators.AggPr)));
            gridView.Columns.Add(AddPercentile("AF", nameof(Indicators.Agg)));
            gridView.Columns.Add(AddPercentile("W$WSF", nameof(Indicators.WSWSF)));
            gridView.Columns.Add(AddPercentile("Flop C-Bet%", nameof(Indicators.FlopCBet)));
            gridView.Columns.Add(AddPercentile("Steal%", nameof(Indicators.Steal)));

            base.AddDefaultStats(gridView);

            foreach (var column in gridView.Columns)
            {
                column.Width = GetColumnWidth(column.Header as string) + 10;
            }
        }
    }
}