using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Telerik.Windows.Controls;

using DriveHUD.Application.ValueConverters;
using DriveHUD.Common.Wpf.Helpers;

namespace DriveHUD.Application.ReportsLayout
{
    public abstract class ReportLayoutCreator
    {
        public virtual void Create(RadGridView gridView)
        {

        }


        protected virtual GridViewDataColumn Add(string name, string member)
        {
            return Add(name, member, new GridViewLength(0));
        }

        protected virtual GridViewDataColumn Add(string name, string member, GridViewLength width)
        {
            GridViewDataColumn column = new GridViewDataColumn
            {
                Header = name,
                DataMemberBinding = new Binding(member),
                Width = width.Value == 0 ? new GridViewLength(1, GridViewLengthUnitType.Star) : width,
                UniqueName = member,
            };

            return column;
        }

        protected virtual GridViewDataColumn AddFinancial(string name, string member)
        {
            return AddFinancial(name, member, new GridViewLength(0));
        }

        protected virtual GridViewDataColumn AddFinancial(string name, string member, GridViewLength width)
        {
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
            var bindingText = new Binding(member);
            bindingText.StringFormat = "{0:c2}";
            fef.SetBinding(TextBlock.TextProperty, bindingText);

            var bindingForeground = new Binding(member);
            bindingForeground.Converter = new ValueToColorConverter();
            fef.SetBinding(TextBlock.ForegroundProperty, bindingForeground);

            DataTemplate template = new DataTemplate();
            template.VisualTree = fef;
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

        protected virtual GridViewDataColumn AddPercentile(string name, string member)
        {
            return AddPercentile(name, member, new GridViewLength(0));
        }

        protected virtual GridViewDataColumn AddPercentile(string name, string member, GridViewLength width)
        {
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
            var bindingText = new Binding(member) { StringFormat = "{0:n1}"};
            fef.SetBinding(TextBlock.TextProperty, bindingText);

            DataTemplate template = new DataTemplate { VisualTree = fef };
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

        public static double GetColumnWidth(string  text)
        {
            double minWidth = TextMeasurer.MesureString(text);
            if(text.Length <= 2)
            {
                minWidth += 40;
            }
            else if (text.Length <= 5)
            {
                minWidth += 20;
            }
            else if (text.Length > 15)
            {
                minWidth -= 20;
            }
            return minWidth;
        }
    }
}