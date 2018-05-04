using DriveHUD.Common.Infrastructure.Base;
using System.Collections.Generic;
using System.Linq;
using System;
using DriveHUD.ViewModels;
using Model.Enums;

namespace DriveHUD.EquityCalculator.Models
{
    public abstract class CardCollectionContainer : BaseViewModel, ICardCollectionContainer
    {
        #region Fields
        private IEnumerable<CardModel> _cards = new List<CardModel>();
        private IEnumerable<RangeSelectorItemViewModel> _ranges = new List<RangeSelectorItemViewModel>();
        #endregion

        #region Properties

        public IEnumerable<CardModel> Cards
        {
            get
            {
                return _cards;
            }

            set
            {
                SetProperty(ref _cards, value);
            }
        }

        public abstract int ContainerSize
        {
            get;
        }

        public IEnumerable<RangeSelectorItemViewModel> Ranges
        {
            get
            {
                return _ranges;
            }

            set
            {
                _ranges = value;
            }
        }
        #endregion

        public CardCollectionContainer() : this(null) { }

        public CardCollectionContainer(IEnumerable<CardModel> model)
        {
            SetCollection(model);
        }

        public virtual void SetCollection(IEnumerable<CardModel> model)
        {
            if (model == null || model.Count() == 0)
            {
                this.Cards = FillCollection(new List<CardModel>());
                return;
            }

            this.Cards = FillCollection(new List<CardModel>(model));
        }

        public virtual void SetRanges(IEnumerable<RangeSelectorItemViewModel> model)
        {
            if (model != null)
            {
                this.Ranges = new List<RangeSelectorItemViewModel>(model);
            }
            else
            {
                this.Ranges = new List<RangeSelectorItemViewModel>();
            }
        }

        public virtual void Reset()
        {
            Cards = FillCollection(new List<CardModel>());
        }

        protected virtual IEnumerable<CardModel> FillCollection(IEnumerable<CardModel> cards)
        {
            List<CardModel> list = new List<CardModel>(cards);
            for (int i = list.Count; i < ContainerSize; i++)
            {
                list.Add(new CardModel(RangeCardRank.None, RangeCardSuit.None));
            }
            return list;
        }        
    }
}
