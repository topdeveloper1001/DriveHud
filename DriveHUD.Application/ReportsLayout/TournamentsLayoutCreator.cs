using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Helpers;
using Model;
using Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class TournamentsLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Started", nameof(TournamentReportRecord.StartedString), GetColumnWidth(StringFormatter.GetDateTimeString(DateTime.Now))));
            gridView.Columns.Add(Add("Table Type", nameof(TournamentReportRecord.TableType)));
            gridView.Columns.Add(Add("Game Type", nameof(TournamentReportRecord.GameType), GetColumnWidth("NoLimitHoldem")));
            gridView.Columns.Add(Add("Speed", nameof(TournamentReportRecord.TournamentSpeed)));
            gridView.Columns.Add(AddFinancial("Buy-in", nameof(TournamentReportRecord.BuyIn)));
            gridView.Columns.Add(AddFinancial("Re-buy", nameof(TournamentReportRecord.Rebuy)));
            gridView.Columns.Add(AddFinancial("Rake", nameof(TournamentReportRecord.Rake)));
            gridView.Columns.Add(Add("Place", nameof(TournamentReportRecord.FinishPosition)));
            gridView.Columns.Add(AddFinancial("Won Amount", nameof(TournamentReportRecord.Won)));
            gridView.Columns.Add(AddFinancial("Net Won", nameof(TournamentReportRecord.NetWon)));
            gridView.Columns.Add(Add("Tournament Length", nameof(TournamentReportRecord.TournamentLength)));
            gridView.Columns.Add(Add("Table Size", nameof(TournamentReportRecord.TableSize)));

            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (i == 0 || i == 2)
                {
                    continue;
                }
                gridView.Columns[i].Width = GetColumnWidth(gridView.Columns[i].Header as string) + 18;
            }
        }
    }
}
