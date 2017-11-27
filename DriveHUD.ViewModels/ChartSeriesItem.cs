//-----------------------------------------------------------------------
// <copyright file="ChartSeriesItem.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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
using System.Windows.Media;

namespace DriveHUD.ViewModels
{
    [Serializable]
    public class ChartSeriesItem : ReactiveObject
    {
        #region Properties

        private decimal itemValue;

        public decimal Value
        {
            get
            {
                return itemValue;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref itemValue, value);
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

        public string ValueText
        {
            get
            {
                return !string.IsNullOrEmpty(format) ?
                    string.Format(format, Value) :
                    string.Empty;
            }
        }

        private object category;

        public object Category
        {
            get
            {
                return category;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref category, value);
            }
        }

        private DateTime date;

        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref date, value);
            }
        }

        private Color pointColor = new Color();

        public Color PointColor
        {
            get
            {
                return pointColor;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref pointColor, value);
            }
        }

        private Color trackBallColor = new Color();

        public Color TrackBallColor
        {
            get
            {
                return trackBallColor;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref trackBallColor, value);
            }
        }

        private Color tooltipColor = new Color();

        public Color TooltipColor
        {
            get
            {
                return tooltipColor;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref tooltipColor, value);
            }
        }

        #endregion       
    }
}