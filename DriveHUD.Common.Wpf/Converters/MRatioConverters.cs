//-----------------------------------------------------------------------
// <copyright file="MRatioToColorConverter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Enums;
using System;
using System.Globalization;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Converters
{
    public class MRatioToColorConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is EnumMRatio)
            {
                switch ((EnumMRatio)value)
                {
                    case EnumMRatio.RedZone:
                        return new SolidColorBrush(Colors.Red);
                    case EnumMRatio.OrangeZone:
                        return new SolidColorBrush(Colors.Orange);
                    case EnumMRatio.YellowZone:
                        return new SolidColorBrush(Colors.Yellow);
                    case EnumMRatio.GreenZone:
                        return new SolidColorBrush(Colors.Green);
                    case EnumMRatio.BlueZone:
                        return new SolidColorBrush(Colors.Blue);
                    case EnumMRatio.PurpleZone:
                        return new SolidColorBrush(Colors.Purple);
                }
            }
            return new SolidColorBrush();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MRatioToTextConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EnumMRatio)
            {
                switch ((EnumMRatio)value)
                {
                    case EnumMRatio.RedZone:
                        return "\u2264 5";
                    case EnumMRatio.OrangeZone:
                        return "6-10";
                    case EnumMRatio.YellowZone:
                        return "10-20";
                    case EnumMRatio.GreenZone:
                        return "20-40";
                    case EnumMRatio.BlueZone:
                        return "40-60";
                    case EnumMRatio.PurpleZone:
                        return "\u2265 60";
                }
            }
            return string.Empty;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
