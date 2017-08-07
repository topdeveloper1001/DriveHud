//-----------------------------------------------------------------------
// <copyright file="PositiontLayoutCreator.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Resources;
using Model.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class PositionLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(AddPosition("Reports_Column_Position", ReflectionHelper.GetPath<Indicators>(o => o.Source.PositionString), ReflectionHelper.GetPath<Indicators>(o => o.Source.Position)));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Reports_Column_TotalWon", nameof(Indicators.TotalWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(Indicators.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));
            gridView.Columns.Add(AddPercentile("Reports_Column_VPIP", nameof(Indicators.VPIP)));
            gridView.Columns.Add(AddPercentile("Reports_Column_PFR", nameof(Indicators.PFR)));
            gridView.Columns.Add(AddPercentile("Reports_Column_3Bet", nameof(Indicators.ThreeBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_3BetCall", nameof(Indicators.ThreeBetCall)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WTSD", nameof(Indicators.WTSD)));
            gridView.Columns.Add(AddPercentile("Reports_Column_AggPercent", nameof(Indicators.AggPr)));
            gridView.Columns.Add(AddPercentile("Reports_Column_AF", nameof(Indicators.Agg)));
            gridView.Columns.Add(AddPercentile("Reports_Column_WSWSF", nameof(Indicators.WSWSF)));
            gridView.Columns.Add(AddPercentile("Reports_Column_FlopToCBet", nameof(Indicators.FlopCBet)));
            gridView.Columns.Add(AddPercentile("Reports_Column_Steal", nameof(Indicators.Steal)));
            gridView.Columns.Add(AddPercentile("Reports_Column_4BetRange", nameof(Indicators.FourBetRange)));

            base.AddDefaultStats(gridView);

            foreach (var column in gridView.Columns)
            {
                column.Width = GetColumnWidth(column.Header as string) + 10;
            }
        }

        private GridViewDataColumn AddPosition(string resourceKey, string member, string dataMember)
        {
            return AddPosition(resourceKey, member, dataMember, new GridViewLength(0));
        }

        private GridViewDataColumn AddPosition(string resourceKey, string member, string dataMember, GridViewLength width)
        {
            var fef = new FrameworkElementFactory(typeof(TextBlock));
            var bindingText = new Binding(member);
            fef.SetBinding(TextBlock.TextProperty, bindingText);

            var template = new DataTemplate();
            template.VisualTree = fef;
            template.Seal();

            var column = new GridViewDataColumn
            {
                Header = CommonResourceManager.Instance.GetResourceString(resourceKey),
                DataMemberBinding = new Binding(dataMember),
                Width = width == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                CellTemplate = template,
                UniqueName = member,
            };

            return column;
        }
    }
}