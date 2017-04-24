using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Entities;
using Model.Enums;
using DriveHUD.Common.Utils;
using DriveHUD.Common.Extensions;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;

namespace Model.Filters
{
    [Serializable]
    public class FilterDateModel : FilterBaseEntity, IFilterModel
    {
        public FilterDateModel()
        {
            this.Name = "Date";
            this.Type = EnumFilterModelType.FilterDateModel;
        }

        public void Initialize()
        {
            FirstDayOfWeek = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings.StartDayOfWeek;
        }

        #region Methods
        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            switch (DateFilterType.EnumDateRange)
            {
                case EnumDateFiterStruct.EnumDateFiter.Today:
                    return PredicateBuilder.Create<Playerstatistic>(x => x.Time >= DateTime.Now.StartOfDay());
                case EnumDateFiterStruct.EnumDateFiter.ThisWeek:
                    FirstDayOfWeek = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings.StartDayOfWeek;
                    return PredicateBuilder.Create<Playerstatistic>(x => x.Time >= DateTime.Now.FirstDayOfWeek(FirstDayOfWeek));
                case EnumDateFiterStruct.EnumDateFiter.ThisMonth:
                    return PredicateBuilder.Create<Playerstatistic>(x => x.Time >= DateTime.Now.FirstDayOfMonth());
                case EnumDateFiterStruct.EnumDateFiter.LastMonth:
                    var statisticCollection = ServiceLocator.Current.GetInstance<SingletonStorageModel>()?.StatisticCollection;
                    if (statisticCollection == null || !statisticCollection.Any())
                    {
                        break;
                    }
                    var lastAvailableDate = statisticCollection.Max(x => x.Time);
                    return PredicateBuilder.Create<Playerstatistic>(x => x.Time >= lastAvailableDate.FirstDayOfMonth());

                case EnumDateFiterStruct.EnumDateFiter.CustomDateRange:
                    if (DateFilterType.DateFrom <= DateFilterType.DateTo)
                        return PredicateBuilder.Create<Playerstatistic>(x => x.Time >= DateFilterType.DateFrom && x.Time <= DateFilterType.DateTo);
                    else
                        return PredicateBuilder.Create<Playerstatistic>(x => x.Time <= DateFilterType.DateFrom && x.Time >= DateFilterType.DateTo);

                case EnumDateFiterStruct.EnumDateFiter.ThisYear:
                    return PredicateBuilder.Create<Playerstatistic>(x => x.Time >= DateTime.Now.FirstDayOfYear());
            };

            return null;
        }

        public void ResetFilter()
        {
            DateFilterType = new EnumDateFiterStruct { EnumDateRange = EnumDateFiterStruct.EnumDateFiter.None };  
        }

        public void LoadFilter(IFilterModel filter)
        {
            if (filter is FilterDateModel)
            {
                var filterToLoad = filter as FilterDateModel;
                DateFilterType = filterToLoad.DateFilterType;
            }
        }
        #endregion

        #region Properties
        public static Action OnChanged;

        private EnumFilterModelType _type;
        private EnumDateFiterStruct _dateFilterType;

        public EnumFilterModelType Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        public EnumDateFiterStruct DateFilterType
        {
            get
            {
                return _dateFilterType;
            }

            set
            {
                if (value.EnumDateRange == _dateFilterType.EnumDateRange) return;
                _dateFilterType = value;
                OnPropertyChanged();

                if (OnChanged != null) OnChanged.Invoke();
            }
        }

        public DayOfWeek FirstDayOfWeek { get; private set; }
        #endregion
    }
}
