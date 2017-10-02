//-----------------------------------------------------------------------
// <copyright file="OpponentAnalysisLayoutCreator.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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
    public class OpponentAnalysisLayoutCreator : ReportLayoutCreator
    {
        private readonly static string[] defaultColumns = new[] { nameof(Indicators.VPIP), nameof(Indicators.PFR), nameof(Indicators.ThreeBet), nameof(Indicators.ThreeBetCall),
            nameof(Indicators.WTSD), nameof(Indicators.WSSD), nameof(Indicators.AggPr), nameof(Indicators.Agg), nameof(Indicators.WSWSF), nameof(Indicators.FlopCBet),
            nameof(Indicators.Steal), nameof(Indicators.BlindsReraiseSteal), nameof(Indicators.BlindsFoldSteal), nameof(Indicators.FourBetRange), nameof(Indicators.UO_PFR_EP),
            nameof(Indicators.UO_PFR_MP), nameof(Indicators.UO_PFR_CO), nameof(Indicators.UO_PFR_BN), nameof(Indicators.UO_PFR_SB) };

        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(Add("Reports_Column_PlayerName", ReflectionHelper.GetPath<Indicators>(o => o.Source.PlayerName)));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Reports_Column_TotalWon", nameof(Indicators.TotalWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(Indicators.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));

            AddDefaultStats(gridView, defaultColumns);

            foreach (var column in gridView.Columns)
            {
                column.Width = GetColumnWidth(column);
            }
        }
    }
}