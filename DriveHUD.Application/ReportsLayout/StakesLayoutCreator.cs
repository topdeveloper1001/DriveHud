//-----------------------------------------------------------------------
// <copyright file="StakesLayoutCreator.cs" company="Ace Poker Solutions">
// Copyright � 2015 Ace Poker Solutions. All Rights Reserved.
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
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_GamesPlayed", ReflectionHelper.GetPath<Indicators>(o => o.Source.GameType), GetColumnWidth("0.00/0.00 NoLimitHoldem")));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Reports_Column_TotalWon", nameof(Indicators.TotalWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(Indicators.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));
            gridView.Columns.Add(AddPercentile("Reports_Column_VPIP", nameof(Indicators.VPIP)));
            gridView.Columns.Add(AddPercentile("Reports_Column_PFR", nameof(Indicators.PFR)));
            gridView.Columns.Add(AddPercentile("Reports_Column_3Bet", nameof(Indicators.ThreeBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_3BetCall", nameof(Indicators.ThreeBetCall)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WTSD", nameof(Indicators.WTSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSSD", nameof(Indicators.WSSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_AggPercent", nameof(Indicators.AggPr)));
            gridView.Columns.Add(AddPercentile("Reports_Column_AF", nameof(Indicators.Agg)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSWSF", nameof(Indicators.WSWSF)));
            gridView.Columns.Add(AddPercentile("Reports_Column_FlopToCBet", nameof(Indicators.FlopCBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_Steal", nameof(Indicators.Steal)));
            gridView.Columns.Add(AddPercentile("Reports_Column_4BetRange", nameof(Indicators.FourBetRange)));

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