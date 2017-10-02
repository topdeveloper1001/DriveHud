//-----------------------------------------------------------------------
// <copyright file="SessionsLayoutCreator.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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
    public class SessionsLayoutCreator : ReportLayoutCreator
    {
        private readonly static string[] defaultColumns = new[] { nameof(Indicators.VPIP), nameof(Indicators.PFR), nameof(Indicators.ThreeBet), nameof(Indicators.ThreeBetCall),
            nameof(Indicators.WTSD), nameof(Indicators.WSSD), nameof(Indicators.AggPr), nameof(Indicators.Agg), nameof(Indicators.WSWSF), nameof(Indicators.FlopCBet),
            nameof(Indicators.Steal) };

        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_SessionStart", nameof(Indicators.SessionStart), GetColumnHeaderWidth(StringFormatter.GetDateTimeString(DateTime.Now)), new DateTimeToLocalizedDateTimeConverter()));
            gridView.Columns.Add(Add("Reports_Column_SessionLength", nameof(Indicators.SessionLength)));
            gridView.Columns.Add(Add("Reports_Column_GamesPlayed", nameof(Indicators.GameType), GetColumnHeaderWidth("0.00/0.00 NoLimitHoldem")));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Reports_Column_TotalWon", nameof(Indicators.TotalWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(Indicators.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));
            gridView.Columns.Add(AddFinancial("Reports_Column_StdDev", nameof(Indicators.StdDev), false));
            gridView.Columns.Add(Add("Reports_Column_StdDevBB", nameof(Indicators.StdDevBB), false, stringFormat: "{0:n2}"));
            gridView.Columns.Add(Add("Reports_Column_StdDevBB100", nameof(Indicators.StdDevBBPer100Hands), false, stringFormat: "{0:n2}"));
            gridView.Columns.Add(AddFinancial("Reports_Column_UsdPerHour", nameof(Indicators.NetWonPerHour), false));        

            AddDefaultStats(gridView, defaultColumns);

            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (i == 0 || i == 2)
                {
                    continue;
                }
                gridView.Columns[i].Width = GetColumnWidth(gridView.Columns[i]);
            }
        }
    }
}