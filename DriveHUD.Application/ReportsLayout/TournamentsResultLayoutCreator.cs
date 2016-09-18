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
    public class TournamentsResultLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(AddFinancial("Buy-in", nameof(TournamentReportRecord.BuyIn)));
            gridView.Columns.Add(Add("Table Type", nameof(TournamentReportRecord.TableType)));
            gridView.Columns.Add(Add("Game Type", nameof(TournamentReportRecord.GameType)));
            gridView.Columns.Add(Add("Speed", nameof(TournamentReportRecord.TournamentSpeed)));
            gridView.Columns.Add(AddFinancial("Total Winnings", nameof(TournamentReportRecord.Won)));
            gridView.Columns.Add(AddFinancial("Net Winnings", nameof(TournamentReportRecord.NetWon)));
            gridView.Columns.Add(AddPercentile("ROI%", nameof(TournamentReportRecord.ROI)));
            gridView.Columns.Add(AddPercentile("ITM%", nameof(TournamentReportRecord.ITM)));
            gridView.Columns.Add(Add("Final Tables", nameof(TournamentReportRecord.FinalTables)));
        }
    }
}
