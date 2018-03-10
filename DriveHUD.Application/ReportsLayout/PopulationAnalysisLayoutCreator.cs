//-----------------------------------------------------------------------
// <copyright file="PopulationAnalysisLayoutCreator.cs" company="Ace Poker Solutions">
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
    public class PopulationAnalysisLayoutCreator : ReportLayoutCreator
    {
        private readonly static string[] defaultColumns = new[] { nameof(Indicators.VPIP), nameof(Indicators.PFR), nameof(Indicators.AggPr), nameof(Indicators.ThreeBet),
            nameof(Indicators.FourBet), nameof(Indicators.ThreeBetVsSteal), nameof(Indicators.FoldCBet),  nameof(Indicators.FoldToThreeBet), nameof(Indicators.FoldToFourBet),
            nameof(Indicators.CheckRaisedFlopCBet), nameof(Indicators.RaiseFlop), nameof(Indicators.RaiseTurn), nameof(Indicators.RaiseRiver),
            nameof(Indicators.FoldToTurnCBet), nameof(Indicators.FoldToFlopRaise),  nameof(Indicators.CheckRiverAfterBBLine),  nameof(Indicators.CheckRiverOnBXLine),
            nameof(Indicators.DidDelayedTurnCBet), nameof(Indicators.BetWhenCheckedTo), nameof(Indicators.RiverBetSizeMoreThanOne),
            nameof(Indicators.WTSDAfterCalling3Bet), nameof(Indicators.WTSDAfterSeeingTurn), nameof(Indicators.WTSDAsPF3Bettor) };

        public override void Create(RadGridView gridView)
        {
            gridView.FrozenColumnCount = 1;            
            gridView.Columns.Clear();

            gridView.Columns.Add(AddPlayerType("Reports_Columns_PlayerType", ReflectionHelper.GetPath<PopulationReportIndicators>(o => o.PlayerType)));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Reports_Column_TotalWon", nameof(Indicators.TotalWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(Indicators.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));

            AddDefaultStats(gridView, defaultColumns);

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