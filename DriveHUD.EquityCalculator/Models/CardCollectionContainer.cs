//-----------------------------------------------------------------------
// <copyright file="CardCollectionContainer.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.ViewModels;
using Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.EquityCalculator.Models
{
    public abstract class CardCollectionContainer : BaseViewModel, ICardCollectionContainer
    {
        #region Fields

        private IEnumerable<CardModel> cards = new List<CardModel>();
        private IEnumerable<RangeSelectorItemViewModel> ranges = new List<RangeSelectorItemViewModel>();

        #endregion
        
        public CardCollectionContainer() : this(null)
        {
        }

        public CardCollectionContainer(IEnumerable<CardModel> model)
        {
            SetCollection(model);
        }

        #region Properties

        public IEnumerable<CardModel> Cards
        {
            get
            {
                return cards;
            }

            set
            {
                SetProperty(ref cards, value);
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
                return ranges;
            }
            set
            {
                SetProperty(ref ranges, value);
            }
        }

        #endregion

        public virtual void SetCollection(IEnumerable<CardModel> model)
        {
            Cards = FillCollection(model);
        }

        public virtual void SetRanges(IEnumerable<RangeSelectorItemViewModel> model)
        {
            Ranges = model?.ToList() ?? new List<RangeSelectorItemViewModel>();
        }

        public virtual void Reset()
        {
            SetCollection(null);
        }

        protected virtual IEnumerable<CardModel> FillCollection(IEnumerable<CardModel> cards)
        {
            var list = cards?.ToList() ?? new List<CardModel>();

            for (int i = list.Count; i < ContainerSize; i++)
            {
                list.Add(new CardModel(RangeCardRank.None, RangeCardSuit.None));
            }

            return list;
        }
    }
}