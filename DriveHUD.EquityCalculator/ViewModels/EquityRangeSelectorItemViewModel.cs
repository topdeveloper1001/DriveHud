//-----------------------------------------------------------------------
// <copyright file="PreflopSelectorViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.ViewModels;
using Model.Enums;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public class EquityRangeSelectorItemViewModel : RangeSelectorItemViewModel
    {
        public EquityRangeSelectorItemViewModel()
        {
        }

        public EquityRangeSelectorItemViewModel(RangeCardRank firstCard, RangeCardRank secondCard)
            : base(firstCard, secondCard)
        {
        }

        public EquityRangeSelectorItemViewModel(RangeCardRank firstCard, RangeCardRank secondCard, RangeSelectorItemType itemType)
            : base(firstCard, secondCard, itemType)
        {
        }

        #region Properties

        private EquitySelectionMode? equitySelectionMode;

        public EquitySelectionMode? EquitySelectionMode
        {
            get
            {
                return equitySelectionMode;
            }
            set
            {
                SetProperty(ref equitySelectionMode, value);
            }
        }

        private int combos;

        public int Combos
        {
            get
            {
                return combos;
            }
            set
            {
                SetProperty(ref combos, value);
            }
        }

        #endregion     
    }
}