//-----------------------------------------------------------------------
// <copyright file="Top20ToughestOpponentsByLossAmountSeriesProvider.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using System;
using System.Collections.Generic;

namespace DriveHUD.Application.ViewModels.Graphs.SeriesProviders
{
    internal class Top20ToughestOpponentsByLossAmountSeriesProvider : IGraphSeriesProvider
    {
        private SingletonStorageModel StorageModel
        {
            get { return ServiceLocator.Current.TryResolve<SingletonStorageModel>(); }
        }

        public IEnumerable<GraphSerie> GetSeries()
        {
            throw new NotImplementedException();
        }

        public void Process(Playerstatistic statistic)
        {
        }
    }
}