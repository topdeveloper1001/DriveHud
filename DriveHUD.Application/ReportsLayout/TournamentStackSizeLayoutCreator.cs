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

using DriveHUD.Common.Resources;
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
        private readonly static string[] defaultColumns = new[] { nameof(Indicators.VPIP), nameof(Indicators.PFR), nameof(Indicators.ThreeBet),
             nameof(Indicators.AggPr), nameof(Indicators.Agg), nameof(Indicators.WTSD), nameof(Indicators.WSSD), nameof(Indicators.WSWSF) };

        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(GetMRatioColumn("Reports_Column_MRatio", nameof(MRatioReportRecord.MRatioZone), GetColumnHeaderWidth("M-Ratio") + 10));
            gridView.Columns.Add(Add("Reports_Column_TotalHands", nameof(MRatioReportRecord.TotalHands)));
            gridView.Columns.Add(AddFinancial("Reports_Column_NetWon", nameof(MRatioReportRecord.TotalWon)));
            gridView.Columns.Add(Add("Reports_Column_BB100", nameof(MRatioReportRecord.BB)));
            gridView.Columns.Add(Add("Reports_Column_EVBB100", nameof(Indicators.EVBB)));          

            AddDefaultStats(gridView, defaultColumns);
        }

        private GridViewDataColumn GetMRatioColumn(string resourceKey, string member, GridViewLength width)
        {
            var fef = new FrameworkElementFactory(typeof(TextBlock));

            var bindingText = new Binding(member);
            bindingText.Converter = new MRatioToTextConverter();

            fef.SetBinding(TextBlock.TextProperty, bindingText);
            fef.SetValue(FrameworkElement.WidthProperty, 40.0);
            fef.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);

            var rect = new FrameworkElementFactory(typeof(Rectangle));
            rect.SetValue(FrameworkElement.WidthProperty, 20.0);

            var backgroundColorBinding = new Binding(member);
            backgroundColorBinding.Converter = new MRatioToColorConverter();
            rect.SetBinding(Shape.FillProperty, backgroundColorBinding);

            var sp = new FrameworkElementFactory(typeof(StackPanel));
            sp.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            sp.AppendChild(fef);
            sp.AppendChild(rect);

            var template = new DataTemplate { VisualTree = sp };
            template.Seal();

            var column = new GridViewDataColumn
            {
                Header = CommonResourceManager.Instance.GetResourceString(resourceKey),
                DataMemberBinding = new Binding(member),
                Width = width == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                CellTemplate = template,
                UniqueName = member,
            };

            return column;
        }
    }
}