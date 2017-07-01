//-----------------------------------------------------------------------
// <copyright file="FilterHoleCardsModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Cards = HandHistories.Objects.Cards;

namespace Model.Filters
{
    [Serializable]
    public class FilterHoleCardsModel : FilterBaseEntity, IFilterModel
    {
        public static Action OnSelectionChanged;

        #region Constructor

        public FilterHoleCardsModel()
        {
            Name = "Hole Cards";
            Type = EnumFilterModelType.FilterHoleCards;
        }

        public void Initialize()
        {
            FilterSectionHoleCardsInitialize();
        }

        #endregion

        #region Methods

        public void FilterSectionHoleCardsInitialize()
        {
            HoleCardsCollection = new ObservableCollection<HoleCardsItem>();

            var rankValues = Cards.Card.PossibleRanksHighCardFirst;

            for (int i = 0; i < rankValues.Count(); i++)
            {
                bool startS = false;

                for (int j = 0; j < rankValues.Count(); j++)
                {
                    string card1 = i < j ? rankValues.ElementAt(i) : rankValues.ElementAt(j);
                    string card2 = i < j ? rankValues.ElementAt(j) : rankValues.ElementAt(i);

                    if (startS)
                    {
                        HoleCardsCollection.Add(new HoleCardsItem
                        {
                            Name = string.Format("{0}{1}s", card1, card2),
                            ItemType = RangeSelectorItemType.Suited,
                            IsChecked = true
                        });
                    }
                    else
                    {
                        if (!card1.Equals(card2))
                        {
                            HoleCardsCollection.Add(new HoleCardsItem
                            {
                                Name = string.Format("{0}{1}o", card1, card2),
                                ItemType = RangeSelectorItemType.OffSuited,
                                IsChecked = true
                            });
                        }
                        else
                        {
                            HoleCardsCollection.Add(new HoleCardsItem
                            {
                                Name = string.Format("{0}{1}", card1, card2),
                                ItemType = RangeSelectorItemType.Pair,
                                IsChecked = true
                            });

                            startS = true;
                        }
                    }
                }
            }
        }

        public override object Clone()
        {
            FilterHoleCardsModel model = this.DeepCloneJson();

            return model;
        }

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            if (!HoleCardsCollection.Any(x => !x.IsChecked))
            {
                return null;
            }

            var checkedHoleCards = HoleCardsCollection.Where(x => x.IsChecked);
            var holeCardsPredicate = PredicateBuilder.Create<Playerstatistic>(x => FilterHelpers.CheckHoleCards(x.Cards, checkedHoleCards));

            return holeCardsPredicate;
        }

        public void ResetFilter()
        {
            ResetHoleCardsCollection();
        }

        public void LoadFilter(IFilterModel filter)
        {
            if (filter is FilterHoleCardsModel)
            {
                var filterToLoad = filter as FilterHoleCardsModel;

                ResetFilterHoleCardsTo(filterToLoad.HoleCardsCollection.ToList());
            }
        }
        #endregion

        #region ResetFilters

        public void ResetHoleCardsCollection()
        {
            HoleCardsCollection.Where(x => !x.IsChecked).ToList().ForEach(x => x.IsChecked = true);
        }

        #endregion

        #region Restore Defaults

        private void ResetFilterHoleCardsTo(IEnumerable<HoleCardsItem> holeCardsList)
        {
            foreach (var holeCard in holeCardsList)
            {
                var cur = HoleCardsCollection.FirstOrDefault(x => x.Name == holeCard.Name);

                if (cur != null)
                {
                    cur.IsChecked = holeCard.IsChecked;
                }
            }
        }

        #endregion

        #region Properties

        private ObservableCollection<HoleCardsItem> _holeCardsCollection;
        private EnumFilterModelType _type;

        public EnumFilterModelType Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<HoleCardsItem> HoleCardsCollection
        {
            get
            {
                return _holeCardsCollection;
            }
            set
            {
                if (value == _holeCardsCollection) return;
                _holeCardsCollection = value;
            }
        }

        #endregion
    }

    [Serializable]
    public class HoleCardsItem : FilterBaseEntity
    {
        public static Action OnIsChecked;

        private bool _isChecked;
        private RangeSelectorItemType _itemType;

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();

                if (OnIsChecked != null) OnIsChecked.Invoke();
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

        public int GetFirstCardRangeIndex()
        {
            if (Name.Length >= 1)
            {
                return Cards.Card.PossibleRanksHighCardFirst.Reverse().ToList().IndexOf(Name[0].ToString());
            }
            return -1;
        }

        public int GetSecondCardRangeIndex()
        {
            if (Name.Length >= 2)
            {
                return Cards.Card.PossibleRanksHighCardFirst.Reverse().ToList().IndexOf(Name[1].ToString());
            }
            return -1;
        }

        public int GetSuitsCount()
        {
            switch (ItemType)
            {
                case RangeSelectorItemType.Pair:
                    return 6;
                case RangeSelectorItemType.OffSuited:
                    return 12;
                case RangeSelectorItemType.Suited:
                    return 4;
            }

            return 1;
        }

        /// <summary>
        /// Determines if specific hand is part of current hole cards range 
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public bool Contains(IEnumerable<string> hand)
        {
            if (hand.Count() != 2 || Name.Length < 2)
            {
                return false;
            }

            switch (ItemType)
            {
                case RangeSelectorItemType.Pair:
                    return hand.ElementAt(0)[0] == hand.ElementAt(1)[0] && this.Name.Contains(hand.ElementAt(0)[0]);
                case RangeSelectorItemType.OffSuited:
                    return hand.ElementAt(0)[0] != hand.ElementAt(1)[0] && hand.ElementAt(0)[1] != hand.ElementAt(1)[1] && hand.All(x => this.Name.Contains(x[0]));
                case RangeSelectorItemType.Suited:
                    return hand.ElementAt(0)[0] != hand.ElementAt(1)[0] && hand.ElementAt(0)[1] == hand.ElementAt(1)[1] && hand.All(x => this.Name.Contains(x[0]));
            }
            return false;
        }
    }
}