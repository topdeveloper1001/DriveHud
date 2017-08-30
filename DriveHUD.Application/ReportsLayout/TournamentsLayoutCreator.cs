//-----------------------------------------------------------------------
// <copyright file="TournamentPokerSiteLayoutCreator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Converters;
using Model;
using Model.Data;
using System;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class TournamentsLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_Started", nameof(TournamentReportRecord.Started), GetColumnWidth(StringFormatter.GetDateTimeString(DateTime.Now)), new DateTimeToLocalizedDateTimeConverter()));
            gridView.Columns.Add(Add("Reports_Column_TableType", nameof(TournamentReportRecord.TableType)));
            gridView.Columns.Add(Add("Reports_Column_GameType", nameof(TournamentReportRecord.GameType), GetColumnWidth("NoLimitHoldem")));
            gridView.Columns.Add(Add("Reports_Column_Speed", nameof(TournamentReportRecord.TournamentSpeed)));
            gridView.Columns.Add(AddFinancial("Reports_Column_Buyin", nameof(TournamentReportRecord.BuyIn)));
            gridView.Columns.Add(AddFinancial("Reports_Column_Rebuy", nameof(TournamentReportRecord.Rebuy)));
            gridView.Columns.Add(AddFinancial("Reports_Column_Rake", nameof(TournamentReportRecord.Rake)));
            gridView.Columns.Add(Add("Reports_Column_Place", nameof(TournamentReportRecord.FinishPosition)));
            gridView.Columns.Add(AddFinancial("Reports_Column_WonAmount", nameof(TournamentReportRecord.Won)));
            gridView.Columns.Add(AddFinancial("Reports_Column_NetWon", nameof(TournamentReportRecord.NetWon)));
            gridView.Columns.Add(Add("Reports_Column_TournamentLength", nameof(TournamentReportRecord.TournamentLength)));
            gridView.Columns.Add(Add("Reports_Column_TableSize", nameof(TournamentReportRecord.TableSize)));

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