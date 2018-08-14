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

using Prism.Mvvm;
using System;
using System.Windows.Media;

namespace DriveHUD.ViewModels
{
    [Serializable]
    public class ChartSeriesItem : BindableBase
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
                SetProperty(ref itemValue, value);
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
                SetProperty(ref format, value);
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
                SetProperty(ref category, value);
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
                SetProperty(ref pointColor, value);
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
                SetProperty(ref trackBallColor, value);
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
                SetProperty(ref tooltipColor, value);
            }
        }

        private Color tooltipForegroundColor = new Color();

        public Color TooltipForegroundColor
        {
            get
            {
                return tooltipForegroundColor;
            }
            set
            {
                SetProperty(ref tooltipForegroundColor, value);
            }
        }

        #endregion       
    }
}