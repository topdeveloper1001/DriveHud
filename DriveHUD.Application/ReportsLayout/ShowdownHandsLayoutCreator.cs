using DriveHUD.Common.Reflection;
using Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class ShowdownHandsLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Showdown Hand", nameof(ShowdownHandsReportRecord.ShowdownHandString), GetColumnWidth("Straight Flush: With 2 Pocket Cards")));
            gridView.Columns.Add(Add("Total Hands", nameof(ShowdownHandsReportRecord.TotalHands)));
            gridView.Columns.Add(AddPercentile("Won%", nameof(ShowdownHandsReportRecord.WonHandProc)));
            gridView.Columns.Add(AddFinancial("Net Won", ReflectionHelper.GetPath<ShowdownHandsReportRecord>(o => o.Source.NetWon)));
            gridView.Columns.Add(Add("bb/100", nameof(ShowdownHandsReportRecord.BB)));
            gridView.Columns.Add(AddPercentile("VPIP", nameof(ShowdownHandsReportRecord.VPIP)));
            gridView.Columns.Add(AddPercentile("PFR", nameof(ShowdownHandsReportRecord.PFR)));
            gridView.Columns.Add(AddPercentile("Agg%", nameof(ShowdownHandsReportRecord.AggPr)));
            gridView.Columns.Add(AddPercentile("Agg", nameof(ShowdownHandsReportRecord.Agg)));
            gridView.Columns.Add(AddPercentile("3Bet%", nameof(ShowdownHandsReportRecord.ThreeBet)));
            gridView.Columns.Add(AddPercentile("WTSD%", nameof(ShowdownHandsReportRecord.WTSD)));
            gridView.Columns.Add(AddPercentile("W$SD", nameof(ShowdownHandsReportRecord.WSSD)));
            gridView.Columns.Add(AddPercentile("W$WSF", nameof(ShowdownHandsReportRecord.WSWSF)));

            for (int i = 1; i < gridView.Columns.Count; i++)
            {
                gridView.Columns[i].Width = GetColumnWidth(gridView.Columns[i].Header as string) + 10;
            }
        }
    }
}
