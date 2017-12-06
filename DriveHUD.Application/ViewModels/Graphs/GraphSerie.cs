//-----------------------------------------------------------------------
// <copyright file="GraphSerie.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace DriveHUD.Application.ViewModels.Graphs
{
    public class GraphSerie : BindableBase
    {
        private string legend;

        public string Legend
        {
            get
            {
                return legend;
            }
            set
            {
                SetProperty(ref legend, value);
            }
        }

        private ObservableCollection<GraphSerieDataPoint> dataPoints = new ObservableCollection<GraphSerieDataPoint>();

        public ObservableCollection<GraphSerieDataPoint> DataPoints
        {
            get
            {
                return dataPoints;
            }
            set
            {
                SetProperty(ref dataPoints, value);
            }
        }

    }
}