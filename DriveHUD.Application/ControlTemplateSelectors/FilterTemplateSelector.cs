using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DriveHUD.Common.Wpf.DataTemplateSelectors;
using Model.Enums;
using Model.Filters;

namespace DriveHUD.Application.ControlTemplateSelectors
{
	public class FilterTemplateSelector : DataTemplateSelector
	{
		public TemplateCollection Templates { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			FilterDropDownModel itemFilterDropDownModel = item as FilterDropDownModel;

			if (itemFilterDropDownModel?.FilterType == EnumFilterDropDown.FilterCustomDateRange)
				return Templates.FirstOrDefault(x => x.Value.Equals(EnumFilterDropDown.FilterCustomDateRange)).DataTemplate;


			return Templates.FirstOrDefault(x => x.Value.Equals("standardTemplate")).DataTemplate;
		}


	}
}

