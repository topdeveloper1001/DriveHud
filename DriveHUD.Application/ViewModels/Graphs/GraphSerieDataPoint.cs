//-----------------------------------------------------------------------
// <copyright file="GraphSerieDataPoint.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Application.ViewModels.Graphs
{
    internal class GraphSerieDataPoint : BindableBase
    {
        private decimal pointValue;

        public decimal Value
        {
            get
            {
                return pointValue;
            }
            set
            {
                SetProperty(ref pointValue, value);
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
    }
}