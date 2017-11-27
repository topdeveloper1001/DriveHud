//-----------------------------------------------------------------------
// <copyright file="ChartSeries.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model.Enums;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace DriveHUD.ViewModels
{
    [Serializable]
    public class ChartSeries : ReactiveObject
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

        private ChartSeriesType chartSeriesType;

        public ChartSeriesType ChartSeriesType
        {
            get
            {
                return chartSeriesType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref chartSeriesType, value);
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

        private bool isVisible = true;

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

        private Color lineColor = new Color();

        public Color LineColor
        {
            get
            {
                return lineColor;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref lineColor, value);
            }
        }

        private VisualBrush areaStyle;

        public VisualBrush AreaStyle
        {
            get
            {
                return areaStyle;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref areaStyle, value);
            }
        }

        public Action<ChartSeriesItem, ChartSeriesItem, Playerstatistic> UpdateChartSeriesItem
        {
            get;
            set;
        }

        #region Obsolete

        [Obsolete]
        private EnumTelerikRadChartFunctionType functionName;

        [Obsolete]
        public EnumTelerikRadChartFunctionType FunctionName
        {
            get
            {
                return functionName;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref functionName, value);
            }
        }

        [Obsolete]
        private EnumTelerikRadChartSeriesType type = EnumTelerikRadChartSeriesType.Area;

        [Obsolete]
        public EnumTelerikRadChartSeriesType Type
        {
            get
            {
                return type;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref type, value);
            }
        }

        #endregion

        #endregion
    }
}