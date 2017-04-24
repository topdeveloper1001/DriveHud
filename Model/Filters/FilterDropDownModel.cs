using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Filters
{
    public class FilterDropDownModel : INotifyPropertyChanged
    {
        public EnumFilterDropDown FilterType { get; set; }
        public string Name { get; set; }

        public FilterDropDownModel() { }

        public FilterDropDownModel(string name, EnumFilterDropDown filterType)
        {
            this.Name = name;
            this.FilterType = filterType;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static ObservableCollection<FilterDropDownModel> GetDefaultFilterTypeList()
        {
            return new ObservableCollection<FilterDropDownModel>()
            {
                new FilterDropDownModel("Create Filter", EnumFilterDropDown.FilterCreate),
                new FilterDropDownModel("Filter Today", EnumFilterDropDown.FilterToday),
                new FilterDropDownModel("Filter This Week", EnumFilterDropDown.FilterThisWeek),
                new FilterDropDownModel("Filter This Month", EnumFilterDropDown.FilterThisMonth),
                new FilterDropDownModel("Filter Last Month", EnumFilterDropDown.FilterLastMonth),
                new FilterDropDownModel("Filter This Year", EnumFilterDropDown.FilterThisYear),
                new FilterDropDownModel("Custom Date Range", EnumFilterDropDown.FilterCustomDateRange),
                new FilterDropDownModel("Clear All Filters", EnumFilterDropDown.FilterAllStats)
            };
        }
    }
}