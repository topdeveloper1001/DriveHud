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
    public class HeatMapValueToBackgroundConverter : HeatMapValueToColorsBaseConverter
    {
        private string[] colors = new[] { "#e1e1f2", "#5edb15", "#89d624", "#a7d624", "#c7d624", "#d6bc24", "#d69e24", "#d78924", "#d76c24", "#d64b24", "#d72424" };

        protected override string[] Colors
        {
            get
            {
                return colors;
            }
        }
    }
}