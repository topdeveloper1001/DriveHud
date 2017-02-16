using DriveHUD.Common.Wpf.Helpers;
using Telerik.Windows.Controls;
using System.Linq;
using System;
using HandHistories.Objects.GameDescription;
using DriveHUD.Common.Resources;
using Model;
using Model.Data;

namespace DriveHUD.Application.ReportsLayout
{
    public class SessionsLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Session Start", nameof(Indicators.SessionStart), GetColumnWidth(StringFormatter.GetDateTimeString(DateTime.Now))));
            gridView.Columns.Add(Add("Session Length", nameof(Indicators.SessionLength)));
            gridView.Columns.Add(Add("Games Played", nameof(Indicators.GameType), GetColumnWidth("0.00/0.00 NoLimitHoldem")));
            gridView.Columns.Add(Add("Total Hands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Total Won", nameof(Indicators.TotalWon)));
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

            base.AddDefaultStats(gridView);

            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (i == 0 || i == 2)
                {
                    continue;
                }
                gridView.Columns[i].Width = GetColumnWidth(gridView.Columns[i].Header as string);
            }
        }
    }
}