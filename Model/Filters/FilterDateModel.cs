//-----------------------------------------------------------------------
// <copyright file="PokerBuilder.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Importer;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Model.Filters
{
    [Serializable]
    public class FilterDateModel : FilterBaseEntity, IFilterModel
    {
        public FilterDateModel()
        {
            Name = "Date";
            Type = EnumFilterModelType.FilterDateModel;
        }

        public void Initialize()
        {
            FirstDayOfWeek = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings.StartDayOfWeek;
        }

        #region Methods

        public Expression<Func<T, bool>> GetFilterPredicate<T>(Expression<Func<T, DateTime>> getDateExpression, Expression<Func<T, bool>> lastMonthExpression)
        {
            if (getDateExpression == null || lastMonthExpression == null)
            {
                return null;
            }

            var getDate = getDateExpression.Compile();

            switch (DateFilterType.EnumDateRange)
            {
                case EnumDateFiterStruct.EnumDateFiter.Today:
                    return PredicateBuilder.Create<T>(x => getDate(x) >= DateTime.Now.StartOfDay());

                case EnumDateFiterStruct.EnumDateFiter.ThisWeek:
                    FirstDayOfWeek = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings.StartDayOfWeek;
                    return PredicateBuilder.Create<T>(x => getDate(x) >= DateTime.Now.FirstDayOfWeek(FirstDayOfWeek));

                case EnumDateFiterStruct.EnumDateFiter.ThisMonth:
                    return PredicateBuilder.Create<T>(x => getDate(x) >= DateTime.Now.FirstDayOfMonth());

                case EnumDateFiterStruct.EnumDateFiter.LastMonth:
                    return lastMonthExpression;

                case EnumDateFiterStruct.EnumDateFiter.CustomDateRange:
                    if (DateFilterType.DateFrom <= DateFilterType.DateTo)
                    {
                        return PredicateBuilder.Create<T>(x => getDate(x) >= DateFilterType.DateFrom &&
                            getDate(x) <= DateFilterType.DateTo);
                    }
                    else
                    {
                        return PredicateBuilder.Create<T>(x => getDate(x) <= DateFilterType.DateFrom &&
                            getDate(x) >= DateFilterType.DateTo);
                    }

                case EnumDateFiterStruct.EnumDateFiter.ThisYear:
                    return PredicateBuilder.Create<T>(x => getDate(x) >= DateTime.Now.FirstDayOfYear());
            };

            return null;
        }

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            var storageModel = ServiceLocator.Current.GetInstance<SingletonStorageModel>();

            var lastCashAvailableDate = storageModel.StatisticCollection?.ToArray().Where(x => !x.IsTourney).MaxOrDefault(x => Converter.ToLocalizedDateTime(x.Time));
            var lastTournamentAvailableDate = storageModel.StatisticCollection?.ToArray().Where(x => x.IsTourney).MaxOrDefault(x => Converter.ToLocalizedDateTime(x.Time));

            lastCashAvailableDate = lastCashAvailableDate ?? DateTime.Now;
            lastTournamentAvailableDate = lastTournamentAvailableDate ?? DateTime.Now;

            var lastMonthPredicateExpression = PredicateBuilder.Create<Playerstatistic>(x => Converter.ToLocalizedDateTime(x.Time) >= (x.IsTourney ?
                   lastTournamentAvailableDate.Value.FirstDayOfMonth() :
                   lastCashAvailableDate.Value.FirstDayOfMonth()));

            return GetFilterPredicate(x => Converter.ToLocalizedDateTime(x.Time), lastMonthPredicateExpression);
        }

        public IEnumerable<Tournaments> FilterTournaments(IEnumerable<Tournaments> tournaments)
        {
            if (tournaments == null || !tournaments.Any())
            {
                return tournaments;
            }

            var lastTournamentAvailableDate = tournaments?.MaxOrDefault(x => x.Firsthandtimestamp);

            lastTournamentAvailableDate = lastTournamentAvailableDate ?? DateTime.Now;

            var lastMonthPredicateExpression = PredicateBuilder.Create<Tournaments>(x => x.Firsthandtimestamp >= lastTournamentAvailableDate.Value.FirstDayOfMonth());

            var filterPredicate = GetFilterPredicate(x => x.Firsthandtimestamp, lastMonthPredicateExpression);

            var dateFilterPredicate = PredicateBuilder.True<Tournaments>();

            if (filterPredicate != null)
            {
                dateFilterPredicate = dateFilterPredicate.And(filterPredicate);
            }

            return tournaments.AsQueryable().Where(dateFilterPredicate);
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
            get
            {
                return _type;
            }
            set
            {
                if (value == _type)
                {
                    return;
                }

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
                if (value.EnumDateRange == _dateFilterType.EnumDateRange)
                {
                    return;
                }

                _dateFilterType = value;

                OnPropertyChanged();
                OnChanged?.Invoke();
            }
        }

        public DayOfWeek FirstDayOfWeek { get; private set; }

        #endregion
    }
}