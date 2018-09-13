using DriveHUD.Common.Infrastructure.Base;
using Model.Enums;
using Model.Notifications;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace DriveHUD.ViewModels
{
    public class RangeSelectorItemViewModel : BaseViewModel
    {
        #region  Fields

        private RangeCardRank _fisrtCard = RangeCardRank.None;
        private RangeCardRank _secondCard = RangeCardRank.None;
        private RangeSelectorItemType _itemType;
        private Likelihood _itemLikelihood;
        private bool _isSelected = false;
        private bool _isMainInSequence = false;
        private int _likelihoodPercent = (int)Likelihood.None;
        private IEnumerable<HandSuitsViewModel> _handSuitsModelList = HandSuitsViewModel.GetHandSuitsList();

        #endregion

        #region Properties

        public InteractionRequest<CustomLikelihoodNotification> CustomLikelihoodRequest { get; private set; }

        public string Caption
        {
            get
            {
                return FisrtCard.ToRankString()
                    + SecondCard.ToRankString()
                    + ItemType.ToRangeSelectorItemString();
            }

        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value != _isSelected)
                {
                    if (value)
                    {
                        LikelihoodPercent = (int)Likelihood.Definitely;
                    }
                    else
                    {
                        LikelihoodPercent = (int)Likelihood.None;
                    }
                }

                SetProperty(ref _isSelected, value);
            }
        }

        public int LikelihoodPercent
        {
            get
            {
                return _likelihoodPercent;
            }
            set
            {
                SetProperty(ref _likelihoodPercent, value);
            }
        }

        public RangeSelectorItemType ItemType
        {
            get
            {
                return _itemType;
            }
            set
            {
                _itemType = value;
            }
        }

        public RangeCardRank FisrtCard
        {
            get
            {
                return _fisrtCard;
            }
            set
            {
                _fisrtCard = value;
            }
        }

        public RangeCardRank SecondCard
        {
            get
            {
                return _secondCard;
            }

            set
            {
                _secondCard = value;
            }
        }

        public Likelihood ItemLikelihood
        {
            get { return _itemLikelihood; }
            set { SetProperty(ref _itemLikelihood, value); }
        }

        public IEnumerable<HandSuitsViewModel> HandSuitsModelList
        {
            get { return _handSuitsModelList; }
            set { _handSuitsModelList = value; }
        }

        public bool IsMainInSequence
        {
            get { return _isMainInSequence; }
            set { _isMainInSequence = value; }
        }
        #endregion

        #region ICommand

        public ICommand SelectLikelihoodCommand { get; set; }

        #endregion

        public RangeSelectorItemViewModel() : this(RangeCardRank.None, RangeCardRank.None)
        {
        }

        public RangeSelectorItemViewModel(RangeCardRank firstCard, RangeCardRank secondCard)
            : this(firstCard, secondCard, RangeSelectorItemType.Default)
        {

        }

        public RangeSelectorItemViewModel(RangeCardRank firstCard, RangeCardRank secondCard, RangeSelectorItemType itemType)
        {
            FisrtCard = firstCard;
            SecondCard = secondCard;
            ItemType = itemType;

            Init();
        }

        private void Init()
        {
            SelectLikelihoodCommand = new RelayCommand(SelectLikelihood);

            CustomLikelihoodRequest = new InteractionRequest<CustomLikelihoodNotification>();
        }

        public virtual void HandUpdate()
        {
            switch (ItemType)
            {
                case RangeSelectorItemType.Suited:
                    HandSuitsViewModel.SetSuitedSuitsVisible(HandSuitsModelList);
                    break;
                case RangeSelectorItemType.OffSuited:
                    HandSuitsViewModel.SetOffSuitedSuitsVisible(HandSuitsModelList);
                    break;
                case RangeSelectorItemType.Pair:
                    HandSuitsViewModel.SetPairSuitsVisible(HandSuitsModelList);
                    break;
            }
        }

        public void HandRefreshVisibilityCheck()
        {
            HandSuitsViewModel.RefreshVisibilityCheck(HandSuitsModelList);
        }

        public void HandRefresh()
        {
            HandSuitsViewModel.RefreshCheckedState(HandSuitsModelList, true);
        }

        public virtual void HandUpdateAndRefresh()
        {
            HandUpdate();            
        }

        public static RangeSelectorItemViewModel FromString(string s)
        {
            if (!string.IsNullOrEmpty(s) && s.Length >= 2)
            {
                var model = new RangeSelectorItemViewModel(new RangeCardRank().StringToRank(s[0].ToString()), new RangeCardRank().StringToRank(s[1].ToString()));

                if (s.Length == 3)
                {
                    model.ItemType = new RangeSelectorItemType().StringToRangeItemType(s[2].ToString());
                }

                return model;
            }

            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj is RangeSelectorItemViewModel)
            {
                var o = obj as RangeSelectorItemViewModel;

                return o.FisrtCard == this.FisrtCard
                    && o.SecondCard == this.SecondCard
                    && o.ItemType == this.ItemType;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region ICommand implementation

        private void SelectLikelihood(object obj)
        {
            if (obj == null)
            {
                return;
            }

            ItemLikelihood = (Likelihood)Enum.Parse(typeof(Likelihood), obj.ToString());

            switch (ItemLikelihood)
            {
                case Likelihood.Definitely:
                case Likelihood.Likely:
                case Likelihood.NotVeryLikely:
                case Likelihood.Rarely:
                    LikelihoodPercent = (int)(ItemLikelihood);
                    break;
                case Likelihood.Custom:
                    CustomLikelihoodRequest.Raise(
                        new CustomLikelihoodNotification() { Title = "Custom Likelihood" },
                        returned =>
                        {
                            if (returned != null)
                            {
                                if (returned.Likelihood >= 0)
                                {
                                    LikelihoodPercent = returned.Likelihood;
                                }
                            }
                        });
                    break;
            }
        }

        #endregion
    }
}