//-----------------------------------------------------------------------
// <copyright file="Top20BiggestLosingHandsSeriesProvider.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Graphs.SeriesProviders
{
    internal class Top20BiggestLosingHandsSeriesProvider : Top20BiggestWinningHandsSeriesProvider
    {
        public override IEnumerable<GraphSerie> GetSeries()
        {
            var series = new List<GraphSerie>();

            if (dataPoints == null)
            {
                return series;
            }

            var topIndex = 1;

            foreach (var dataPoint in dataPoints.OrderBy(x => x.Value.Value).Take(20))
            {
                dataPoint.Value.Category = string.Format("#{0}: {1} ${2:#.##}", topIndex++, dataPoint.Key, Math.Abs(dataPoint.Value.Value));

                series.Add(new GraphSerie
                {
                    Legend = dataPoint.Key,
                    DataPoints = new ObservableCollection<GraphSerieDataPoint> { dataPoint.Value }
                });
            }

            return series;
        }
    }
}