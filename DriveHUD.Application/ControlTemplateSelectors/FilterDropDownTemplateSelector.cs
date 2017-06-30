using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ControlTemplateSelectors
{
    public class FilterDropDownTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var fvm = item as FilterDropDownModel;

            if (fvm == null)
            {
                return CommonTemplate;
            }

            if (fvm.FilterType == Model.Enums.EnumFilterDropDown.FilterCustomDates)
            {
                return CustomDateRangeTemplate;
            }

            return CommonTemplate;
        }

        public DataTemplate CommonTemplate { get; set; }

        public DataTemplate CustomDateRangeTemplate { get; set; }
    }
}
