//-----------------------------------------------------------------------
// <copyright file="HandSuitsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.ViewModels
{
    public class HandSuitsViewModel : BaseViewModel
    {
        public HandSuitsViewModel(string resourceKey, HandSuitsEnum handSuit)
        {
            ResourceKey = resourceKey;
            HandSuit = handSuit;
        }

        #region Properties

        private string resourceKey = string.Empty;

        public string ResourceKey
        {
            get
            {
                return resourceKey;
            }
            set
            {
                SetProperty(ref resourceKey, value);
            }
        }

        private bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
            set { SetProperty(ref isVisible, value); }
        }

        private bool isSelected = true;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                SetProperty(ref isSelected, value);
            }
        }

        private HandSuitsEnum handSuit;

        public HandSuitsEnum HandSuit
        {
            get
            {
                return handSuit;
            }
            set
            {
                SetProperty(ref handSuit, value);
            }
        }

        private EquitySelectionMode selectionMode;

        public EquitySelectionMode SelectionMode
        {
            get
            {
                return selectionMode;
            }
            set
            {
                SetProperty(ref selectionMode, value);
            }
        }

        #endregion

        internal static IEnumerable<HandSuitsViewModel> GetHandSuitsList()
        {
            var list = new List<HandSuitsViewModel>()
            {
                new HandSuitsViewModel("SuitCC", HandSuitsEnum.CC),
                new HandSuitsViewModel("SuitCD", HandSuitsEnum.CD),
                new HandSuitsViewModel("SuitCH", HandSuitsEnum.CH),
                new HandSuitsViewModel("SuitCS", HandSuitsEnum.CS),
                new HandSuitsViewModel("SuitDC", HandSuitsEnum.DC),
                new HandSuitsViewModel("SuitDD", HandSuitsEnum.DD),
                new HandSuitsViewModel("SuitDH", HandSuitsEnum.DH),
                new HandSuitsViewModel("SuitDS", HandSuitsEnum.DS),
                new HandSuitsViewModel("SuitHC", HandSuitsEnum.HC),
                new HandSuitsViewModel("SuitHD", HandSuitsEnum.HD),
                new HandSuitsViewModel("SuitHH", HandSuitsEnum.HH),
                new HandSuitsViewModel("SuitHS", HandSuitsEnum.HS),
                new HandSuitsViewModel("SuitSC", HandSuitsEnum.SC),
                new HandSuitsViewModel("SuitSD", HandSuitsEnum.SD),
                new HandSuitsViewModel("SuitSH", HandSuitsEnum.SH),
                new HandSuitsViewModel("SuitSS", HandSuitsEnum.SS),
            };

            return list;
        }

        internal static void SetPairSuitsVisible(IEnumerable<HandSuitsViewModel> model)
        {
            model.ForEach(x => x.IsVisible = false);
            model.Where(x => x.HandSuit.Equals(HandSuitsEnum.CD)
                || x.HandSuit.Equals(HandSuitsEnum.CH)
                || x.HandSuit.Equals(HandSuitsEnum.CS)
                || x.HandSuit.Equals(HandSuitsEnum.DH)
                || x.HandSuit.Equals(HandSuitsEnum.DS)
                || x.HandSuit.Equals(HandSuitsEnum.HS)).ForEach(x => x.IsVisible = true);
        }

        internal static void SetOffSuitedSuitsVisible(IEnumerable<HandSuitsViewModel> model)
        {
            model.ForEach(x => x.IsVisible = false);
            model.Where(x => x.HandSuit.Equals(HandSuitsEnum.CD)
                || x.HandSuit.Equals(HandSuitsEnum.CH)
                || x.HandSuit.Equals(HandSuitsEnum.CS)
                || x.HandSuit.Equals(HandSuitsEnum.DC)
                || x.HandSuit.Equals(HandSuitsEnum.DH)
                || x.HandSuit.Equals(HandSuitsEnum.DS)
                || x.HandSuit.Equals(HandSuitsEnum.HC)
                || x.HandSuit.Equals(HandSuitsEnum.HD)
                || x.HandSuit.Equals(HandSuitsEnum.HS)
                || x.HandSuit.Equals(HandSuitsEnum.SC)
                || x.HandSuit.Equals(HandSuitsEnum.SD)
                || x.HandSuit.Equals(HandSuitsEnum.SH)).ForEach(x => x.IsVisible = true);
        }

        internal static void SetSuitedSuitsVisible(IEnumerable<HandSuitsViewModel> model)
        {
            model.ForEach(x => x.IsVisible = false);

            model.Where(x => x.HandSuit.Equals(HandSuitsEnum.CC)
                || x.HandSuit.Equals(HandSuitsEnum.DD)
                || x.HandSuit.Equals(HandSuitsEnum.HH)
                || x.HandSuit.Equals(HandSuitsEnum.SS)).ForEach(x => x.IsVisible = true);
        }

        internal static void RefreshCheckedState(IEnumerable<HandSuitsViewModel> model, bool isChecked)
        {
            model.ForEach(x => x.IsSelected = isChecked);
        }

        public static void SetAllVisible(IEnumerable<HandSuitsViewModel> model)
        {
            model.ForEach(x => x.IsVisible = true);
        }

        internal static void RefreshVisibilityCheck(IEnumerable<HandSuitsViewModel> model)
        {
            model.Where(x => !x.IsVisible).ForEach(x => x.IsSelected = true);
        }

        public override string ToString()
        {
            return $"{HandSuit}; IsSelected: {IsSelected}; IsVisible: {isVisible}; SelectionMode: {SelectionMode};";

        }
    }
}