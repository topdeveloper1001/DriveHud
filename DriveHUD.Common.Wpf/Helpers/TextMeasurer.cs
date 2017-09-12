//-----------------------------------------------------------------------
// <copyright file="TextMeasurer.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Helpers
{
    public static class TextMeasurer
    {
        public static double MesureString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0d;
            }

            var tb = new TextBlock { FontSize = 18 };

            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                tb.FontSize,
                Brushes.Black);

            return formattedText.Width;
        }
    }
}