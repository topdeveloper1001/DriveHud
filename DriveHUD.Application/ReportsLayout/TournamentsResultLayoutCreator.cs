//-----------------------------------------------------------------------
// <copyright file="TournamentsResultLayoutCreator.cs" company="Ace Poker Solutions">
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
    public class TournamentsResultLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(AddFinancial("Reports_Column_Buyin", nameof(TournamentReportRecord.BuyIn)));
            gridView.Columns.Add(Add("Reports_Column_TableType", nameof(TournamentReportRecord.TableType)));
            gridView.Columns.Add(Add("Reports_Column_GameType", nameof(TournamentReportRecord.GameType)));
            gridView.Columns.Add(Add("Reports_Column_Speed", nameof(TournamentReportRecord.TournamentSpeed)));
            gridView.Columns.Add(AddFinancial("Reports_Column_TotalWinnings", nameof(TournamentReportRecord.Won)));
            gridView.Columns.Add(AddFinancial("Reports_Column_NetWinnings", nameof(TournamentReportRecord.NetWon)));
            gridView.Columns.Add(AddPercentile("Reports_Column_ROI", nameof(TournamentReportRecord.ROI)));
            gridView.Columns.Add(AddPercentile("Reports_Column_ITM", nameof(TournamentReportRecord.ITM)));
            gridView.Columns.Add(Add("Reports_Column_FinalTables", nameof(TournamentReportRecord.FinalTables)));
        }
    }
}