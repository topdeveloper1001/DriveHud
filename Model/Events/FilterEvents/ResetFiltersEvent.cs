using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class ResetFiltersEventArgs : EventArgs
    {
        public FilterSectionItem FilterSection { get; set; }

        public ResetFiltersEventArgs() : this(null)
        {

        }

        public ResetFiltersEventArgs(FilterSectionItem param)
        {
            this.FilterSection = param;
        }
    }

    public class ResetFiltersEvent : Prism.Events.PubSubEvent<ResetFiltersEventArgs>
    {
    }
}
