//-----------------------------------------------------------------------
// <copyright file="BaseChartSeries.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace DriveHUD.ViewModels
{
    [Serializable]
    public class BaseChartSeries : ReactiveObject
    {
        #region Properties

        private string caption;

        public string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref caption, value);
            }
        }

        private string format;

        public string Format
        {
            get
            {
                return format;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref format, value);
            }
        }     

        private ObservableCollection<ChartSeriesItem> itemsCollection = new ObservableCollection<ChartSeriesItem>();

        public ObservableCollection<ChartSeriesItem> ItemsCollection
        {
            get
            {
                return itemsCollection;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref itemsCollection, value);
            }
        }

        private bool isVisible = false;

        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isVisible, value);
            }
        }

        private ChartSerieResourceHelper colorsPalette;

        public ChartSerieResourceHelper ColorsPalette
        {
            get
            {
                return colorsPalette;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref colorsPalette, value);
            }
        }

        #endregion
    }
}