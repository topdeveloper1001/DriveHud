using Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class TournamentPokerSiteLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Poker Site", nameof(TournamentReportRecord.PokerSite)));
            gridView.Columns.Add(Add("Total Hands", nameof(TournamentReportRecord.TotalHands)));
            gridView.Columns.Add(AddFinancial("Total Won", nameof(TournamentReportRecord.Won)));
            gridView.Columns.Add(Add("bb/100", nameof(TournamentReportRecord.BB)));
            gridView.Columns.Add(AddPercentile("VPIP", nameof(TournamentReportRecord.VPIP)));
            gridView.Columns.Add(AddPercentile("PFR", nameof(TournamentReportRecord.PFR)));
            gridView.Columns.Add(AddPercentile("Agg", nameof(TournamentReportRecord.Agg)));
            gridView.Columns.Add(AddPercentile("Agg%", nameof(TournamentReportRecord.AggPr)));
            gridView.Columns.Add(AddPercentile("3-Bet%", nameof(TournamentReportRecord.ThreeBet)));
            gridView.Columns.Add(AddPercentile("WTSD%", nameof(TournamentReportRecord.WTSD)));
            gridView.Columns.Add(AddPercentile("W$SD", nameof(TournamentReportRecord.WSSD)));
            gridView.Columns.Add(AddPercentile("W$WSF", nameof(TournamentReportRecord.WSWSF)));

            base.AddDefaultStats(gridView);
        }
    }
}
