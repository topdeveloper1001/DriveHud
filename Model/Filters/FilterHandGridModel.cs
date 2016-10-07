using DriveHUD.Entities;
using Model.Enums;
using System;
using System.Linq.Expressions;

namespace Model.Filters
{
    public class FilterHandGridModel : FilterBaseEntity, IFilterModel
    {
        private EnumFilterModelType _type = EnumFilterModelType.FilterHandGridModel;

        public EnumFilterModelType Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            return null;
        }

        public void LoadFilter(IFilterModel filter)
        {
        }

        public void ResetFilter()
        {
        }

        public void Initialize()
        {
        }
    }
}
