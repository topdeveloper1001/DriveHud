//-----------------------------------------------------------------------
// <copyright file="BaseGraphViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DriveHUD.Application.ViewModels.Graphs
{
    /// <summary>
    /// Base class for DH graphs
    /// </summary>
    public abstract class BaseGraphViewModel<T> : BaseViewModel, IBaseGraphViewModel
        where T : BaseChartSeries
    {
        public BaseGraphViewModel(IEnumerable<T> chartSeries)
        {
            if (chartSeries == null)
            {
                throw new ArgumentNullException(nameof(chartSeries));
            }

            chartCollection = new ObservableCollection<T>(chartSeries);
        }

        private ObservableCollection<T> chartCollection;

        public virtual ObservableCollection<T> ChartCollection
        {
            get
            {
                return chartCollection;
            }
            set
            {
                SetProperty(ref chartCollection, value);
            }
        }

        public abstract void Update();
    }
}