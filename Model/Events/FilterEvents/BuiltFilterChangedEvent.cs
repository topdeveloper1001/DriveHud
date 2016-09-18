using DriveHUD.Entities;
using Model.Filters;
using System;
using System.Linq.Expressions;

namespace Model.Events
{
    public class BuiltFilterChangedEventArgs : EventArgs
    {
        public BuiltFilterModel BuiltFilter { get; set; }
        public Expression<Func<Playerstatistic, bool>> Predicate { get; set; }


        public BuiltFilterChangedEventArgs(BuiltFilterModel builtFilter, Expression<Func<Playerstatistic, bool>> predicate)
        {
            this.BuiltFilter = builtFilter;
            this.Predicate = predicate;
        }

    }

    public class BuiltFilterChangedEvent : Prism.Events.PubSubEvent<BuiltFilterChangedEventArgs>
    { }
}
