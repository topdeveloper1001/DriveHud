//-----------------------------------------------------------------------
// <copyright file="TournamentStackSizeLayoutCreator.cs" company="Ace Poker Solutions">
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
using Model.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class TournamentStackSizeLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(GetMRatioColumn("M-Ratio", nameof(MRatioReportRecord.MRatio), GetColumnWidth("M-Ratio") + 10));
            gridView.Columns.Add(Add("Total Hands", nameof(MRatioReportRecord.TotalHands)));
            gridView.Columns.Add(AddFinancial("Net Won", nameof(MRatioReportRecord.TotalWon)));
            gridView.Columns.Add(Add("bb/100", nameof(MRatioReportRecord.BB)));
            gridView.Columns.Add(AddPercentile("VPIP", nameof(MRatioReportRecord.VPIP)));
            gridView.Columns.Add(AddPercentile("PFR", nameof(MRatioReportRecord.PFR)));
            gridView.Columns.Add(AddPercentile("Agg", nameof(MRatioReportRecord.Agg)));
            gridView.Columns.Add(AddPercentile("Agg%", nameof(MRatioReportRecord.AggPr)));
            gridView.Columns.Add(AddPercentile("3-Bet%", nameof(MRatioReportRecord.ThreeBet)));
            gridView.Columns.Add(AddPercentile("WTSD%", nameof(MRatioReportRecord.WTSD)));
            gridView.Columns.Add(AddPercentile("W$SD", nameof(MRatioReportRecord.WSSD)));
            gridView.Columns.Add(AddPercentile("W$WSF", nameof(MRatioReportRecord.WSWSF)));

            base.AddDefaultStats(gridView);
        }

        private GridViewDataColumn GetMRatioColumn(string name, string member, GridViewLength width)
        {
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));

            var bindingText = new Binding(member);
            bindingText.Converter = new MRatioToTextConverter();
            fef.SetBinding(TextBlock.TextProperty, bindingText);
            fef.SetValue(TextBlock.WidthProperty, 40.0);
            fef.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);

            FrameworkElementFactory rect = new FrameworkElementFactory(typeof(Rectangle));
            rect.SetValue(Rectangle.WidthProperty, 20.0);
            var backgroundColorBinding = new Binding(member);
            backgroundColorBinding.Converter = new MRatioToColorConverter();
            rect.SetBinding(Rectangle.FillProperty, backgroundColorBinding);

            FrameworkElementFactory sp = new FrameworkElementFactory(typeof(StackPanel));
            sp.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            sp.AppendChild(fef);
            sp.AppendChild(rect);

            DataTemplate template = new DataTemplate { VisualTree = sp };
            template.Seal();

            GridViewDataColumn column = new GridViewDataColumn
            {
                Header = name,
                DataMemberBinding = new Binding(member),
                Width = width == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                CellTemplate = template,
                UniqueName = member,
            };

            return column;
        }
    }
}