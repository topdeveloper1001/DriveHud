using DriveHUD.Entities;
using Model.Enums;
using System;
using System.Linq.Expressions;

namespace Model.Filters
{
    public interface IFilterModel : ICloneable
    {
        Expression<Func<Playerstatistic, bool>> GetFilterPredicate();

        void ResetFilter();

        void LoadFilter(IFilterModel filter);

        EnumFilterModelType Type { get; set; }
    }
}