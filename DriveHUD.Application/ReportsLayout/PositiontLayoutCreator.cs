using DriveHUD.Common.Reflection;
using DriveHUD.Common.Wpf.Helpers;
using Model.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ReportsLayout
{
    public class PositiontLayoutCreator : ReportLayoutCreator
    {
        public override void Create(RadGridView gridView)
        {
            gridView.Columns.Clear();

            gridView.Columns.Add(AddPosition("Position", ReflectionHelper.GetPath<Indicators>(o => o.Source.PositionString), ReflectionHelper.GetPath<Indicators>(o => o.Source.Position)));
            gridView.Columns.Add(Add("Total Hands", nameof(Indicators.TotalHands)));
            gridView.Columns.Add(AddFinancial("Total Won", nameof(Indicators.TotalWon)));
            gridView.Columns.Add(Add("bb/100", nameof(Indicators.BB)));
            gridView.Columns.Add(AddPercentile("VPIP", nameof(Indicators.VPIP)));
            gridView.Columns.Add(AddPercentile("PFR", nameof(Indicators.PFR)));
            gridView.Columns.Add(AddPercentile("3Bet%", nameof(Indicators.ThreeBet)));
            gridView.Columns.Add(AddPercentile("3-Bet Call%", nameof(Indicators.ThreeBetCall)));
            gridView.Columns.Add(AddPercentile("WTSD%", nameof(Indicators.WTSD)));
            gridView.Columns.Add(AddPercentile("Agg%", nameof(Indicators.AggPr)));
            gridView.Columns.Add(AddPercentile("AF", nameof(Indicators.Agg)));
            gridView.Columns.Add(AddPercentile("W$WSF", nameof(Indicators.WSWSF)));
            gridView.Columns.Add(AddPercentile("Flop C-Bet%", nameof(Indicators.FlopCBet)));
            gridView.Columns.Add(AddPercentile("Steal%", nameof(Indicators.Steal)));
            gridView.Columns.Add(AddPercentile("4Bet Range", nameof(Indicators.FourBetRange)));

            base.AddDefaultStats(gridView);

            foreach (var column in gridView.Columns)
            {
                column.Width = GetColumnWidth(column.Header as string) + 10;
            }
        }

        private GridViewDataColumn AddPosition(string name, string member, string dataMember)
        {
            return AddPosition(name, member, dataMember, new GridViewLength(0));
        }

        private GridViewDataColumn AddPosition(string name, string member, string dataMember, GridViewLength width)
        {
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
            var bindingText = new Binding(member);
            fef.SetBinding(TextBlock.TextProperty, bindingText);

            DataTemplate template = new DataTemplate();
            template.VisualTree = fef;
            template.Seal();

            GridViewDataColumn column = new GridViewDataColumn
            {
                Header = name,
                DataMemberBinding = new Binding(dataMember),
                Width = width == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                CellTemplate = template,
                UniqueName = member,
            };

            return column;
        }
    }
}