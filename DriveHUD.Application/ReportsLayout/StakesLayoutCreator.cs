//-----------------------------------------------------------------------
// <copyright file="StakesLayoutCreator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Reflection;
using Model.Data;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class StakesLayoutCreator : ReportLayoutCreator
    {
        private readonly static string[] defaultColumns = new[] { nameof(Indicators.VPIP), nameof(Indicators.PFR), nameof(Indicators.ThreeBet), nameof(Indicators.ThreeBetCall),
            nameof(Indicators.WTSD), nameof(Indicators.WSSD), nameof(Indicators.AggPr), nameof(Indicators.Agg), nameof(Indicators.WSWSF), nameof(Indicators.FlopCBet),
            nameof(Indicators.Steal), nameof(Indicators.FourBetRange) };

        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_GamesPlayed", ReflectionHelper.GetPath<Indicators>(o => o.Source.GameType), GetColumnHeaderWidth("0.00/0.00 NoLimitHoldem")));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Reports_Column_TotalWon", nameof(Indicators.TotalWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(Indicators.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));

            base.AddDefaultStats(gridView, defaultColumns);

            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                gridView.Columns[i].Width = GetColumnWidth(gridView.Columns[i]);
            }
        }
    }
}