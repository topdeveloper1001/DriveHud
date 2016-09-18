using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Persistence.Services;
using DriveHUD.Common.Log;

namespace DriveHUD.Application.Controls
{
    public class GridViewCustomPropertyProvider : ICustomPropertyProvider
    {
        public CustomPropertyInfo[] GetCustomProperties()
        {
            // Create three custom properties to persist the Columns, Sorting and Group descriptors using proxy objects
            return new CustomPropertyInfo[]
            {
                new CustomPropertyInfo("Columns", typeof(List<ColumnProxy>)),
                new CustomPropertyInfo("SortDescriptors", typeof(List<SortDescriptorProxy>)),
                new CustomPropertyInfo("GroupDescriptors", typeof(List<GroupDescriptorProxy>)),
                new CustomPropertyInfo("FilterDescriptors", typeof(List<FilterSetting>)),
            };
        }

        public void InitializeObject(object context)
        {
            if (context is RadGridView)
            {
                RadGridView gridView = context as RadGridView;
                gridView.SortDescriptors.Clear();
                gridView.GroupDescriptors.Clear();
                gridView.Columns
                    .OfType<GridViewColumn>()
                    .Where(c => c.ColumnFilterDescriptor.IsActive)
                    .ToList().ForEach(c => c.ClearFilters());
            }
        }

        public object InitializeValue(CustomPropertyInfo customPropertyInfo, object context)
        {
            return null;
        }

        public object ProvideValue(CustomPropertyInfo customPropertyInfo, object context)
        {
            RadGridView gridView = context as RadGridView;
            try
            {
                switch (customPropertyInfo.Name)
                {
                    case "Columns":
                        {
                            List<ColumnProxy> columnProxies = new List<ColumnProxy>();

                            foreach (GridViewColumn column in gridView.Columns)
                            {
                                columnProxies.Add(new ColumnProxy()
                                {
                                    UniqueName = column.UniqueName,
                                    Header = column.Header.ToString(),
                                    DisplayOrder = column.DisplayIndex,
                                    Width = column.Width,
                                    Visisble = column.IsVisible,
                                });
                            }

                            return columnProxies;
                        }

                    case "SortDescriptors":
                        {
                            List<SortDescriptorProxy> sortDescriptorProxies = new List<SortDescriptorProxy>();

                            foreach (ColumnSortDescriptor descriptor in gridView.SortDescriptors)
                            {
                                if (descriptor.Column == null)
                                {
                                    continue;
                                }

                                sortDescriptorProxies.Add(new SortDescriptorProxy()
                                {
                                    ColumnUniqueName = descriptor.Column.UniqueName,
                                    SortDirection = descriptor.SortDirection,
                                });
                            }

                            return sortDescriptorProxies;
                        }

                    case "GroupDescriptors":
                        {
                            List<GroupDescriptorProxy> groupDescriptorProxies = new List<GroupDescriptorProxy>();

                            foreach (ColumnGroupDescriptor descriotor in gridView.GroupDescriptors)
                            {
                                groupDescriptorProxies.Add(new GroupDescriptorProxy()
                                {
                                    ColumnUniqueName = descriotor.Column.UniqueName,
                                    SortDirection = descriotor.SortDirection,
                                });
                            }

                            return groupDescriptorProxies;
                        }

                    case "FilterDescriptors":
                        {
                            List<FilterSetting> filterSettings = new List<FilterSetting>();

                            /*foreach (IColumnFilterDescriptor columnFilter in gridView.FilterDescriptors)
                            {
                                FilterSetting columnFilterSetting = new FilterSetting();

                                columnFilterSetting.ColumnUniqueName = columnFilter.Column.UniqueName;

                                columnFilterSetting.SelectedDistinctValues.AddRange(columnFilter.DistinctFilter.DistinctValues);

                                if (columnFilter.FieldFilter.Filter1.IsActive)
                                {
                                    columnFilterSetting.Filter1 = new FilterDescriptorProxy();
                                    columnFilterSetting.Filter1.Operator = columnFilter.FieldFilter.Filter1.Operator;
                                    columnFilterSetting.Filter1.Value = columnFilter.FieldFilter.Filter1.Value;
                                    columnFilterSetting.Filter1.IsCaseSensitive = columnFilter.FieldFilter.Filter1.IsCaseSensitive;
                                }

                                columnFilterSetting.FieldFilterLogicalOperator = columnFilter.FieldFilter.LogicalOperator;

                                if (columnFilter.FieldFilter.Filter2.IsActive)
                                {
                                    columnFilterSetting.Filter2 = new FilterDescriptorProxy();
                                    columnFilterSetting.Filter2.Operator = columnFilter.FieldFilter.Filter2.Operator;
                                    columnFilterSetting.Filter2.Value = columnFilter.FieldFilter.Filter2.Value;
                                    columnFilterSetting.Filter2.IsCaseSensitive = columnFilter.FieldFilter.Filter2.IsCaseSensitive;
                                }

                                filterSettings.Add(columnFilterSetting);
                            }*/

                            return filterSettings;
                        }
                }
            }
            catch
            {
                LogProvider.Log.Warn(this, "RadGridViewSettings.ProvideValue: Cannot get grid setting");
            }


            return null;
        }

        public void RestoreValue(CustomPropertyInfo customPropertyInfo, object context, object value)
        {
            RadGridView gridView = context as RadGridView;

            switch (customPropertyInfo.Name)
            {
                case "Columns":
                    {
                        List<ColumnProxy> columnProxies = value as List<ColumnProxy>;

                        foreach (ColumnProxy proxy in columnProxies)
                        {
                            try
                            {
                                if (proxy == null || proxy.UniqueName == null)
                                    continue;
                                GridViewColumn column = gridView.Columns[proxy.UniqueName];
                                if (column == null)
                                    continue;
                                if (proxy.DisplayOrder != -1)
                                {
                                    column.DisplayIndex = proxy.DisplayOrder;
                                }
                                column.Header = proxy.Header;
                                column.Width = proxy.Width;
                                column.IsVisible = proxy.Visisble;
                            }
                            catch (Exception ex)
                            {
                                LogProvider.Log.Error(this, "RadGridViewSettings RestoreValue Columns Error", ex);
                            }
                        }
                    }
                    break;

                case "SortDescriptors":
                    {
                        gridView.SortDescriptors.SuspendNotifications();

                        gridView.SortDescriptors.Clear();

                        List<SortDescriptorProxy> sortDescriptoProxies = value as List<SortDescriptorProxy>;

                        foreach (SortDescriptorProxy proxy in sortDescriptoProxies)
                        {
                            GridViewColumn column = gridView.Columns[proxy.ColumnUniqueName];
                            gridView.SortDescriptors.Add(new ColumnSortDescriptor() { Column = column, SortDirection = proxy.SortDirection });
                        }

                        gridView.SortDescriptors.ResumeNotifications();
                    }
                    break;

                case "GroupDescriptors":
                    {
                        gridView.GroupDescriptors.SuspendNotifications();

                        gridView.GroupDescriptors.Clear();

                        List<GroupDescriptorProxy> groupDescriptorProxies = value as List<GroupDescriptorProxy>;

                        foreach (GroupDescriptorProxy proxy in groupDescriptorProxies)
                        {
                            GridViewColumn column = gridView.Columns[proxy.ColumnUniqueName];
                            gridView.GroupDescriptors.Add(new ColumnGroupDescriptor() { Column = column, SortDirection = proxy.SortDirection });
                        }

                        gridView.GroupDescriptors.ResumeNotifications();
                    }
                    break;

                case "FilterDescriptors":
                    {
                        List<FilterSetting> filterSettings = value as List<FilterSetting>;
                        /*gridView.FilterDescriptors.SuspendNotifications();

                        foreach (var c in gridView.Columns)
                        {
                            if (c.ColumnFilterDescriptor.IsActive)
                            {
                                c.ClearFilters();
                            }
                        }

                        List<FilterSetting> filterSettings = value as List<FilterSetting>;

                        foreach (FilterSetting setting in filterSettings)
                        {
                            Telerik.Windows.Controls.GridViewColumn column = gridView.Columns[setting.ColumnUniqueName];

                            Telerik.Windows.Controls.GridView.IColumnFilterDescriptor columnFilter = column.ColumnFilterDescriptor;

                            foreach (object distinctValue in setting.SelectedDistinctValues)
                            {
                                columnFilter.DistinctFilter.AddDistinctValue(distinctValue);
                            }

                            if (setting.Filter1 != null)
                            {
                                columnFilter.FieldFilter.Filter1.Operator = setting.Filter1.Operator;
                                columnFilter.FieldFilter.Filter1.Value = setting.Filter1.Value;
                                columnFilter.FieldFilter.Filter1.IsCaseSensitive = setting.Filter1.IsCaseSensitive;
                            }

                            columnFilter.FieldFilter.LogicalOperator = setting.FieldFilterLogicalOperator;

                            if (setting.Filter2 != null)
                            {
                                columnFilter.FieldFilter.Filter2.Operator = setting.Filter2.Operator;
                                columnFilter.FieldFilter.Filter2.Value = setting.Filter2.Value;
                                columnFilter.FieldFilter.Filter2.IsCaseSensitive = setting.Filter2.IsCaseSensitive;
                            }
                        }

                        gridView.FilterDescriptors.ResumeNotifications();*/
                    }
                    break;
            }
        }
    }

    public class ColumnProxy
    {
        public string UniqueName { get; set; }
        public int DisplayOrder { get; set; }
        public string Header { get; set; }
        public GridViewLength Width { get; set; }

        public bool Visisble { get; set; }
    }

    public class SortDescriptorProxy
    {
        public string ColumnUniqueName { get; set; }
        public ListSortDirection SortDirection { get; set; }
    }

    public class GroupDescriptorProxy
    {
        public string ColumnUniqueName { get; set; }
        public ListSortDirection? SortDirection { get; set; }
    }

    public class FilterDescriptorProxy
    {
        public Telerik.Windows.Data.FilterOperator Operator { get; set; }
        public object Value { get; set; }
        public bool IsCaseSensitive { get; set; }
    }

    public class FilterSetting
    {
        public string ColumnUniqueName { get; set; }

        private List<object> selectedDistinctValue;
        public List<object> SelectedDistinctValues
        {
            get
            {
                if (this.selectedDistinctValue == null)
                {
                    this.selectedDistinctValue = new List<object>();
                }

                return this.selectedDistinctValue;
            }
        }

        public FilterDescriptorProxy Filter1 { get; set; }
        public Telerik.Windows.Data.FilterCompositionLogicalOperator FieldFilterLogicalOperator { get; set; }
        public FilterDescriptorProxy Filter2 { get; set; }
    }
}
