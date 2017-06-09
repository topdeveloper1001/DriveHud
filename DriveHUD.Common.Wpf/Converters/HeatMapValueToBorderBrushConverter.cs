//-----------------------------------------------------------------------
// <copyright file="HeatMapValueToBorderBrushConverter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Common.Wpf.Converters
{
    public class HeatMapValueToBorderBrushConverter : HeatMapValueToColorsBaseConverter
    {
        private string[] colors = new[] { "#ffffff", "#87ff1e", "#c4ff34", "#efff34", "#ffff34", "#ffff34", "#ffe234", "#ffc434", "#ff9b34", "#ff6b34", "#ff3434" };

        protected override string[] Colors
        {
            get
            {
                return colors;
            }
        }
    }
}