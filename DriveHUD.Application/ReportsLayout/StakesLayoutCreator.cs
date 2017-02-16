using DriveHUD.Common.Reflection;
using DriveHUD.Common.Wpf.Helpers;
using Model.Data;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class StakesLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Game Played", ReflectionHelper.GetPath<Indicators>(o => o.Source.GameType), GetColumnWidth("0.00/0.00 NoLimitHoldem")));
            gridView.Columns.Add(Add("Total Hands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Total Won", ReflectionHelper.GetPath<Indicators>(o => o.Source.NetWon)));
            gridView.Columns.Add(Add("bb/100", nameof(Indicators.BB)));
            gridView.Columns.Add(AddPercentile("VPIP", nameof(Indicators.VPIP)));
            gridView.Columns.Add(AddPercentile("PFR", nameof(Indicators.PFR)));
            gridView.Columns.Add(AddPercentile("3Bet%", nameof(Indicators.ThreeBet)));
            gridView.Columns.Add(AddPercentile("3-Bet Call%", nameof(Indicators.ThreeBetCall)));
            gridView.Columns.Add(AddPercentile("WTSD%", nameof(Indicators.WTSD)));
            gridView.Columns.Add(AddPercentile("W$SD", nameof(Indicators.WSSD)));
            gridView.Columns.Add(AddPercentile("Agg%", nameof(Indicators.AggPr)));
            gridView.Columns.Add(AddPercentile("AF", nameof(Indicators.Agg)));
            gridView.Columns.Add(AddPercentile("W$WSF", nameof(Indicators.WSWSF)));
            gridView.Columns.Add(AddPercentile("Flop C-Bet%", nameof(Indicators.FlopCBet)));
            gridView.Columns.Add(AddPercentile("Steal%", nameof(Indicators.Steal)));
            gridView.Columns.Add(AddPercentile("4Bet Range", nameof(Indicators.FourBetRange)));

            base.AddDefaultStats(gridView);

            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (i == 0)
                    continue;
                gridView.Columns[i].Width = GetColumnWidth(gridView.Columns[i].Header as string);
            }
        }
    }
}