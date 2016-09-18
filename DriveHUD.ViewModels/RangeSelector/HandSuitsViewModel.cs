using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.ViewModels
{
    public enum HandSuitsEnum
    {
        CC, CD, CH, CS, DC, DD, DH, DS, HC, HD, HH, HS, SC, SD, SH, SS, None
    }

    public class HandSuitsViewModel : BaseViewModel
    {
        #region Fields
        private string _resourceKey = string.Empty;
        private bool _isVisible = false;
        private bool _isSelected = true;
        private HandSuitsEnum _handSuit;
        #endregion

        #region Properties
        public string ResourceKey
        {
            get { return _resourceKey; }
            set { SetProperty(ref _resourceKey, value); }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public HandSuitsEnum HandSuit
        {
            get { return _handSuit; }
            set { SetProperty(ref _handSuit, value); }
        }

        #endregion

        public HandSuitsViewModel(string resourceKey, HandSuitsEnum handSuit)
        {
            this.ResourceKey = resourceKey;
            this.HandSuit = handSuit;
        }

        internal static IEnumerable<HandSuitsViewModel> GetHandSuitsList()
        {
            List<HandSuitsViewModel> list = new List<HandSuitsViewModel>()
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
    }
}
