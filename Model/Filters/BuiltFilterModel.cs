//-----------------------------------------------------------------------
// <copyright file="BuiltFilterModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using HandHistories.Objects.Cards;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Extensions;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.Filters
{
    public class BuiltFilterModel : BindableBase
    {
        #region  Properties

        private IFilterModelManagerService FilterModelManager { get; set; }

        private FilterStandardModel StandardModel
        {
            get { return FilterModelManager.FilterModelCollection.OfType<FilterStandardModel>().FirstOrDefault(); }
        }

        private FilterHoleCardsModel HoleCardsModel
        {
            get { return FilterModelManager.FilterModelCollection.OfType<FilterHoleCardsModel>().FirstOrDefault(); }
        }

        private FilterOmahaHandGridModel OmahaHandGridModel
        {
            get { return FilterModelManager.FilterModelCollection.OfType<FilterOmahaHandGridModel>().FirstOrDefault(); }
        }

        private FilterHandValueModel HandValueModel
        {
            get { return FilterModelManager.FilterModelCollection.OfType<FilterHandValueModel>().FirstOrDefault(); }
        }

        private FilterBoardTextureModel BoardTextureModel
        {
            get { return FilterModelManager.FilterModelCollection.OfType<FilterBoardTextureModel>().FirstOrDefault(); }
        }

        private FilterHandActionModel HandActionModel
        {
            get { return FilterModelManager.FilterModelCollection.OfType<FilterHandActionModel>().FirstOrDefault(); }
        }

        private FilterDateModel DateModel
        {
            get { return FilterModelManager.FilterModelCollection.OfType<FilterDateModel>().FirstOrDefault(); }
        }

        private FilterQuickModel QuickFilterModel
        {
            get { return FilterModelManager.FilterModelCollection.OfType<FilterQuickModel>().FirstOrDefault(); }
        }

        private FilterAdvancedModel AdvancedFilterModel
        {
            get { return FilterModelManager.FilterModelCollection.OfType<FilterAdvancedModel>().FirstOrDefault(); }
        }

        private ObservableCollection<FilterSectionItem> _filterSectionCollection;

        public ObservableCollection<FilterSectionItem> FilterSectionCollection
        {
            get
            {
                return _filterSectionCollection;
            }
            set
            {
                SetProperty(ref _filterSectionCollection, value);
            }
        }

        #endregion

        public BuiltFilterModel() { }

        public BuiltFilterModel(FilterServices service)
        {
            FilterModelManager = ServiceLocator.Current.GetInstance<IFilterModelManagerService>(service.ToString());
            Initialize();
        }

        private void Initialize()
        {
            FilterSectionCollection = new ObservableCollection<FilterSectionItem>
            {
                new FilterSectionItem() { Name = EnumFilterSectionItemType.StakeLevel.ToString(), ItemType = EnumFilterSectionItemType.StakeLevel },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.Buyin.ToString(), ItemType = EnumFilterSectionItemType.Buyin },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.PreFlopAction.ToString(), ItemType = EnumFilterSectionItemType.PreFlopAction },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.Currency.ToString(), ItemType = EnumFilterSectionItemType.Currency },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.TableSixRing.ToString(), ItemType = EnumFilterSectionItemType.TableSixRing },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.TableNineRing.ToString(), ItemType = EnumFilterSectionItemType.TableNineRing },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.RaiserSixRing.ToString(), ItemType = EnumFilterSectionItemType.RaiserSixRing },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.RaiserNineRing.ToString(), ItemType = EnumFilterSectionItemType.RaiserNineRing },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.ThreeBettorSixRing.ToString(), ItemType = EnumFilterSectionItemType.ThreeBettorSixRing },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.ThreeBettorNineRing.ToString(), ItemType = EnumFilterSectionItemType.ThreeBettorNineRing },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.FourBettorSixRing.ToString(), ItemType = EnumFilterSectionItemType.FourBettorSixRing },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.FourBettorNineRing.ToString(), ItemType = EnumFilterSectionItemType.FourBettorNineRing },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.PlayersBetween.ToString(), ItemType = EnumFilterSectionItemType.PlayersBetween },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.HoleCards.ToString(), ItemType = EnumFilterSectionItemType.HoleCards },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.FlopHandValue.ToString(), ItemType = EnumFilterSectionItemType.FlopHandValue },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.TurnHandValue.ToString(), ItemType = EnumFilterSectionItemType.TurnHandValue },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.RiverHandValue.ToString(), ItemType = EnumFilterSectionItemType.RiverHandValue },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.Date.ToString(), ItemType = EnumFilterSectionItemType.Date },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.FlopBoardCardItem.ToString(), ItemType = EnumFilterSectionItemType.FlopBoardCardItem },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.TurnBoardCardItem.ToString(), ItemType = EnumFilterSectionItemType.TurnBoardCardItem },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.RiverBoardCardItem.ToString(), ItemType = EnumFilterSectionItemType.RiverBoardCardItem },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.FlopBoardTextureItem.ToString(), ItemType = EnumFilterSectionItemType.FlopBoardTextureItem },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.TurnBoardTextureItem.ToString(), ItemType = EnumFilterSectionItemType.TurnBoardTextureItem },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.RiverBoardTextureItem.ToString(), ItemType = EnumFilterSectionItemType.RiverBoardTextureItem },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.PreflopHandActionItem.ToString(), ItemType = EnumFilterSectionItemType.PreflopHandActionItem },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.FlopHandActionItem.ToString(), ItemType = EnumFilterSectionItemType.FlopHandActionItem },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.TurnHandActionItem.ToString(), ItemType = EnumFilterSectionItemType.TurnHandActionItem },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.RiverHandActionItem.ToString(), ItemType = EnumFilterSectionItemType.RiverHandActionItem },
                new FilterSectionItem() { Name = EnumFilterSectionItemType.OmahaHandGridItem.ToString(), ItemType = EnumFilterSectionItemType.OmahaHandGridItem },
            };
        }

        #region Methods   

        public void BindFilterSectionCollection()
        {
            #region Standard Filter actions

            // Make StatItemSection_IsActive depending on any FilterSectionItem is/not Checked
            StatItem.OnTriState = () =>
            {
                SetStatItems();
            };

            StatItem.OnTriState.Invoke();

            // Make StakeLevelSection_IsActive depending on any FilterSectionItem is/not Checked
            StakeLevelItem.OnIsChecked = () =>
            {
                SetStakeLevelItems();
            };

            StakeLevelItem.OnIsChecked.Invoke();

            BuyinItem.OnIsChecked = () => SetBuyinItems();
            BuyinItem.OnIsChecked.Invoke();

            // Make PreFlopActionSection_IsActive depending on any FilterSectionItem is/not Checked
            PreFlopActionItem.OnIsChecked = () =>
            {
                SetPreFlopActionItems();
            };

            PreFlopActionItem.OnIsChecked.Invoke();

            // Make CurrencySection_IsActive depending on any FilterSectionItem is/not Checked
            CurrencyItem.OnIsChecked = () =>
            {
                SetCurrencyItems();
            };

            CurrencyItem.OnIsChecked.Invoke();

            // Make TableRingSection_IsActive depending on any FilterSectionItem is/not Checked
            TableRingItem.OnIsChecked = () =>
            {
                SetTableRingItems();
            };
            TableRingItem.OnIsChecked.Invoke();

            FilterStandardModel.OnPlayersBetweenChanged = () =>
            {
                SetPlayersBetweenItems();
            };

            FilterStandardModel.OnPlayersBetweenChanged.Invoke();

            #endregion

            #region Hole Cards actions

            HoleCardsItem.OnIsChecked = () =>
            {
                SetHoleCardsItems();
            };
            HoleCardsItem.OnIsChecked.Invoke();

            #endregion

            #region Date Filter Actions
            FilterDateModel.OnChanged = () =>
            {
                SetDateItems();
            };
            FilterDateModel.OnChanged.Invoke();
            #endregion

            #region Hand Values actions
            FastFilterItem.OnTriState = () =>
            {
                SetHandValueFastFilterItems();
            };
            FastFilterItem.OnTriState.Invoke();

            HandValueItem.OnIsChecked = (street) =>
            {
                SetHandValueItems(street);
            };
            HandValueItem.OnIsChecked.Invoke(Street.Flop);
            HandValueItem.OnIsChecked.Invoke(Street.Turn);
            HandValueItem.OnIsChecked.Invoke(Street.River);

            #endregion

            #region Board Texture actions
            BoardCardItem.OnChanged = (street) =>
            {
                SetBoardCardItems(street);
            };

            BoardTextureItem.OnChanged = (street) =>
            {
                SetBoardTextureItems(street);
            };

            BoardCardItem.OnChanged.Invoke(Street.Flop);
            BoardCardItem.OnChanged.Invoke(Street.Turn);
            BoardCardItem.OnChanged.Invoke(Street.River);

            BoardTextureItem.OnChanged.Invoke(Street.Flop);
            BoardTextureItem.OnChanged.Invoke(Street.Turn);
            BoardTextureItem.OnChanged.Invoke(Street.River);
            #endregion

            #region HandAction Filter actions
            HandActionFilterItem.OnChanged = (street) =>
            {
                SetHandActionItems(street);
            };

            HandActionFilterItem.OnChanged.Invoke(Street.Preflop);
            HandActionFilterItem.OnChanged.Invoke(Street.Flop);
            HandActionFilterItem.OnChanged.Invoke(Street.Turn);
            HandActionFilterItem.OnChanged.Invoke(Street.River);
            #endregion

            #region Omaha Hand Grid  Actions
            OmahaHandGridItem.OnChanged = () =>
            {
                SetOmahaHandGridItems();
            };

            OmahaHandGridItem.OnChanged.Invoke();
            #endregion

            #region Quick Filter actions

            QuickFilterItem.OnTriState = () =>
            {
                SetQuickFilterItems();
            };

            QuickFilterItem.OnTriState.Invoke();

            #endregion

            #region Advanced Filter 

            AdvancedFilterModel?.SetBuiltFilterModel(this);

            #endregion
        }

        public void RemoveBuiltFilterItem(FilterSectionItem param)
        {
            switch (param.ItemType)
            {
                case EnumFilterSectionItemType.StakeLevel:
                    RemoveStakeLevelItem();
                    break;
                case EnumFilterSectionItemType.Buyin:
                    RemoveBuyinItem();
                    break;
                case EnumFilterSectionItemType.PreFlopAction:
                    RemovePreFlopActionItem();
                    break;
                case EnumFilterSectionItemType.Currency:
                    RemoveCurrencyItem();
                    break;
                case EnumFilterSectionItemType.Stat:
                    RemoveStatItem(param);
                    break;
                case EnumFilterSectionItemType.TableSixRing:
                case EnumFilterSectionItemType.TableNineRing:
                case EnumFilterSectionItemType.RaiserSixRing:
                case EnumFilterSectionItemType.RaiserNineRing:
                case EnumFilterSectionItemType.ThreeBettorSixRing:
                case EnumFilterSectionItemType.ThreeBettorNineRing:
                case EnumFilterSectionItemType.FourBettorSixRing:
                case EnumFilterSectionItemType.FourBettorNineRing:
                    RemoveTableRingItem(param);
                    break;
                case EnumFilterSectionItemType.PlayersBetween:
                    RemovePlayersBetweenItem();
                    break;
                case EnumFilterSectionItemType.HoleCards:
                    RemoveHoleCardsItem();
                    break;
                case EnumFilterSectionItemType.HandValueFastFilter:
                    RemoveHandValueFastFilterItem(param);
                    break;
                case EnumFilterSectionItemType.FlopHandValue:
                case EnumFilterSectionItemType.TurnHandValue:
                case EnumFilterSectionItemType.RiverHandValue:
                    RemoveHandValueItem(param);
                    break;
                case EnumFilterSectionItemType.Date:
                    RemoveDateItem();
                    break;
                case EnumFilterSectionItemType.FlopBoardCardItem:
                case EnumFilterSectionItemType.TurnBoardCardItem:
                case EnumFilterSectionItemType.RiverBoardCardItem:
                    RemoveBoardCardItem(param);
                    break;
                case EnumFilterSectionItemType.FlopBoardTextureItem:
                case EnumFilterSectionItemType.TurnBoardTextureItem:
                case EnumFilterSectionItemType.RiverBoardTextureItem:
                    RemoveBoardTextureItem(param);
                    break;
                case EnumFilterSectionItemType.PreflopHandActionItem:
                case EnumFilterSectionItemType.FlopHandActionItem:
                case EnumFilterSectionItemType.TurnHandActionItem:
                case EnumFilterSectionItemType.RiverHandActionItem:
                    RemoveHandActionItem(param);
                    break;
                case EnumFilterSectionItemType.OmahaHandGridItem:
                    RemoveOmahaHandGridItem();
                    break;
                case EnumFilterSectionItemType.QuickFilterItem:
                    RemoveQuickFilterItem(param);
                    break;
                case EnumFilterSectionItemType.AdvancedFilterItem:
                    RemoveAdvancedFilterItem(param);
                    break;
            }
        }

        public BuiltFilterModel Clone()
        {
            var model = new BuiltFilterModel
            {
                FilterSectionCollection = new ObservableCollection<FilterSectionItem>(this.FilterSectionCollection.Select(x => (FilterSectionItem)x.Clone()))
            };

            return model;
        }

        #endregion

        #region Built Filter Collection Setters

        #region Stat Filters (Standard Filter)
        private void SetStatItems()
        {
            if (StandardModel == null)
            {
                return;
            }

            foreach (var triStateItem in StandardModel.StatCollection)
            {
                var filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => !string.IsNullOrEmpty(x.Value) && x.Value == triStateItem.PropertyName);
                var state = triStateItem.CurrentTriState;
                if (state == EnumTriState.Any && filterSectionItem != null)
                {
                    /* Make element not active instead of removing it because tooltip is losing its style after element is being removed */
                    filterSectionItem.IsActive = false;
                    continue;
                }

                if (state != EnumTriState.Any)
                {
                    FilterSectionItem newFilterSectionItem;
                    if (filterSectionItem != null)
                    {
                        newFilterSectionItem = filterSectionItem;
                    }
                    else
                    {
                        newFilterSectionItem = new FilterSectionItem() { ItemType = EnumFilterSectionItemType.Stat, Value = triStateItem.PropertyName };
                        this.FilterSectionCollection.Add(newFilterSectionItem);
                    }

                    newFilterSectionItem.Name = String.Format("{0}={1}", triStateItem.Name, state == EnumTriState.On ? "Yes" : "No");
                    newFilterSectionItem.IsActive = true;
                }
            }
        }

        private void RemoveStatItem(FilterSectionItem param)
        {
            if (param.ItemType != EnumFilterSectionItemType.Stat || string.IsNullOrEmpty(param.Value))
            {
                return;
            }

            var filterItemValue = param.Value;
            var statItem = StandardModel?.StatCollection.FirstOrDefault(x => x.PropertyName == filterItemValue);

            if (statItem != null)
            {
                statItem.CurrentTriState = EnumTriState.Any;
            }

        }
        #endregion

        #region Stake Level (Standard Filter)

        private void SetStakeLevelItems()
        {
            FilterSectionItem filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.StakeLevel);

            var selectedStakeLevels = StandardModel?.StakeLevelCollection.Where(x => x.IsChecked);

            if (selectedStakeLevels == null || !StandardModel.StakeLevelCollection.Any(x => !x.IsChecked))
            {
                filterSectionItem.IsActive = false;
                return;
            }

            var stakesString = String.Join(",", selectedStakeLevels.Select(x => x.Name.Trim()));
            filterSectionItem.Name = String.Format("Stakes={0}", stakesString);
            filterSectionItem.IsActive = true;
        }

        private void RemoveStakeLevelItem()
        {
            StandardModel?.ResetStakeLevelCollection();
        }

        private void SetBuyinItems()
        {
            var filterSectionItem = FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.Buyin);

            var buyinItems = StandardModel?.BuyinCollection.Where(x => x.IsChecked);

            if (buyinItems == null || !StandardModel.BuyinCollection.Any(x => !x.IsChecked))
            {
                filterSectionItem.IsActive = false;
                return;
            }

            var buyinString = string.Join(",", buyinItems.Select(x => x.Name.Trim()));
            filterSectionItem.Name = $"Buyins={buyinString}";
            filterSectionItem.IsActive = true;
        }

        private void RemoveBuyinItem()
        {
            StandardModel?.ResetBuyinCollection();
        }

        #endregion

        #region Pre Flop Action Item (Standard Filter)

        private void SetPreFlopActionItems()
        {
            FilterSectionItem filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.PreFlopAction);
            var selectedPreFlopItems = StandardModel?.PreFlopActionCollection.Where(x => x.IsChecked);

            if (selectedPreFlopItems == null || StandardModel == null || !StandardModel.PreFlopActionCollection.Any(x => !x.IsChecked))
            {
                filterSectionItem.IsActive = false;
                return;
            }

            var preFlopItemsString = String.Join(",", selectedPreFlopItems.Select(x => x.Name.Trim()));
            filterSectionItem.Name = String.Format("Pre-flop Action Facing Hero={0}", preFlopItemsString);
            filterSectionItem.IsActive = true;
        }

        private void RemovePreFlopActionItem()
        {
            StandardModel?.ResetPreFlopActionCollection();
        }
        #endregion

        #region Currency Item (Standard Filter)
        private void SetCurrencyItems()
        {
            FilterSectionItem filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.Currency);
            var selectedCurrencyItems = StandardModel?.CurrencyCollection.Where(x => x.IsChecked);
            if (selectedCurrencyItems == null || !StandardModel.CurrencyCollection.Any(x => !x.IsChecked))
            {
                filterSectionItem.IsActive = false;
                return;
            }

            var currencyItemsString = String.Join("+", selectedCurrencyItems.Select(x => x.Name));
            filterSectionItem.Name = String.Format("Currency={0}", currencyItemsString);
            filterSectionItem.IsActive = true;
        }

        private void RemoveCurrencyItem()
        {
            StandardModel?.ResetCurrencyCollection();
        }
        #endregion

        #region Table Ring (Standard Filter)

        private void SetTableRingItems()
        {
            if (StandardModel != null)
            {
                SetTableItem(FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.TableSixRing), StandardModel.Table6MaxCollection, "Position(6max)={0}");
                SetTableItem(FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.TableNineRing), StandardModel.TableFullRingCollection, "Position(Full Ring)={0}");
                SetTableItem(FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.RaiserSixRing), StandardModel.Raiser6maxPositionsCollection, "Opponent PFR position(6max)={0}");
                SetTableItem(FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.RaiserNineRing), StandardModel.RaiserFullRingPositionsCollection, "Opponent PFR position (Full Ring)={0}");
                SetTableItem(FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.ThreeBettorSixRing), StandardModel.ThreeBettor6maxPositionsCollection, "Opponent 3-Bet position(6max)={0}");
                SetTableItem(FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.ThreeBettorNineRing), StandardModel.ThreeBettorFullRingPositionsCollection, "Opponent 3-Bet position(Full Ring)={0}");
                SetTableItem(FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.FourBettorSixRing), StandardModel.FourBettor6maxPositionsCollection, "Opponent 4-Bet position(6max)={0}");
                SetTableItem(FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.FourBettorNineRing), StandardModel.FourBettorFullRingPositionsCollection, "Opponent 4-Bet position(Full Ring)={0}");
            }
        }

        private void SetTableItem(FilterSectionItem filterSectionItem, ObservableCollection<TableRingItem> collection, string formatString)
        {
            var selectedTableItems = collection.Where(x => x.IsChecked);

            if (selectedTableItems == null || collection.All(x => x.IsChecked) || collection.All(x => !x.IsChecked))
            {
                filterSectionItem.IsActive = false;
                return;
            }

            var tableItemsString = string.Join(",", selectedTableItems.Select(x => x.Name));
            filterSectionItem.Name = string.Format(formatString, tableItemsString);
            filterSectionItem.IsActive = true;
        }

        private void RemoveTableRingItem(FilterSectionItem param)
        {
            switch (param.ItemType)
            {
                case EnumFilterSectionItemType.TableSixRing:
                    StandardModel?.ResetTableRingCollection(EnumTableType.Six, PreflopActorPosition.Hero);
                    break;
                case EnumFilterSectionItemType.TableNineRing:
                    StandardModel?.ResetTableRingCollection(EnumTableType.Nine, PreflopActorPosition.Hero);
                    break;
                case EnumFilterSectionItemType.RaiserSixRing:
                    StandardModel?.ResetTableRingCollection(EnumTableType.Six, PreflopActorPosition.Raiser);
                    break;
                case EnumFilterSectionItemType.RaiserNineRing:
                    StandardModel?.ResetTableRingCollection(EnumTableType.Nine, PreflopActorPosition.Raiser);
                    break;
                case EnumFilterSectionItemType.ThreeBettorSixRing:
                    StandardModel?.ResetTableRingCollection(EnumTableType.Six, PreflopActorPosition.ThreeBettor);
                    break;
                case EnumFilterSectionItemType.ThreeBettorNineRing:
                    StandardModel?.ResetTableRingCollection(EnumTableType.Nine, PreflopActorPosition.ThreeBettor);
                    break;
                case EnumFilterSectionItemType.FourBettorSixRing:
                    StandardModel?.ResetTableRingCollection(EnumTableType.Six, PreflopActorPosition.FourBettor);
                    break;
                case EnumFilterSectionItemType.FourBettorNineRing:
                    StandardModel?.ResetTableRingCollection(EnumTableType.Nine, PreflopActorPosition.FourBettor);
                    break;
            }
        }

        #endregion

        #region Players Between Item (Standard Filter)
        private void SetPlayersBetweenItems()
        {
            if (StandardModel == null)
            {
                return;
            }

            FilterSectionItem filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.PlayersBetween);

            filterSectionItem.Name = String.Format("Players={0}-{1}", StandardModel.PlayerCountMinSelectedItem, StandardModel.PlayerCountMaxSelectedItem);
            filterSectionItem.IsActive = (StandardModel.PlayerCountMaxSelectedItem != StandardModel.PlayerCountMaxAvailable) || (StandardModel.PlayerCountMinSelectedItem != StandardModel.PlayerCountMinAvailable);
        }

        private void RemovePlayersBetweenItem()
        {
            StandardModel?.FilterSectionPlayersBetweenCollectionInitialize();
        }
        #endregion

        #region Hole Cards (HoleCards Filter)
        private void SetHoleCardsItems()
        {
            FilterSectionItem filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.HoleCards);
            var selectedHoleCards = HoleCardsModel.HoleCardsCollection.Where(x => x.IsChecked);
            if (selectedHoleCards == null || !HoleCardsModel.HoleCardsCollection.Any(x => !x.IsChecked))
            {
                filterSectionItem.IsActive = false;
                return;
            }

            string selectedHoleCardsString = String.Join(",", CardHelper.GetHandsFormatted(selectedHoleCards.Select(x => x.Name).ToList()));
            filterSectionItem.Name = String.Format("Hole Cards={0}", selectedHoleCardsString);
            filterSectionItem.IsActive = true;
        }

        private void RemoveHoleCardsItem()
        {
            HoleCardsModel.ResetHoleCardsCollection();
        }
        #endregion

        #region Fast Filter (HandValue Filter)

        private void SetHandValueFastFilterItems()
        {
            foreach (var triStateItem in HandValueModel.FastFilterCollection)
            {
                var filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => !string.IsNullOrEmpty(x.Value) && x.Value == triStateItem.Name);

                var state = triStateItem.CurrentTriState;

                if (state == EnumTriState.Any && filterSectionItem != null)
                {
                    /* Make element not active instead of removing it because tooltip is losing its style after element is being removed */
                    filterSectionItem.IsActive = false;
                    continue;
                }

                if (state != EnumTriState.Any)
                {
                    FilterSectionItem newFilterSectionItem;
                    if (filterSectionItem != null)
                    {
                        newFilterSectionItem = filterSectionItem;
                    }
                    else
                    {
                        newFilterSectionItem = new FilterSectionItem() { ItemType = EnumFilterSectionItemType.HandValueFastFilter, Value = triStateItem.Name };
                        this.FilterSectionCollection.Add(newFilterSectionItem);
                    }

                    newFilterSectionItem.Name = String.Format("{0}={1}", triStateItem.Name, state == EnumTriState.On ? "Yes" : "No");
                    newFilterSectionItem.IsActive = true;
                }
            }
        }

        private void RemoveHandValueFastFilterItem(FilterSectionItem param)
        {
            if (param.ItemType != EnumFilterSectionItemType.HandValueFastFilter || string.IsNullOrEmpty(param.Value))
            {
                return;
            }

            var filterItemValue = param.Value;

            var fastFilterItem = HandValueModel.FastFilterCollection.FirstOrDefault(x => x.Name == filterItemValue);

            if (fastFilterItem != null)
            {
                fastFilterItem.CurrentTriState = EnumTriState.Any;
            }
        }

        #endregion

        #region Hand Values (HandValue Filter)

        private void SetHandValueItems(Street street)
        {
            FilterSectionItem filterSectionItem = null;
            IEnumerable<HandValueItem> collection;
            string filterSectionItemString = string.Empty;

            switch (street)
            {
                case Street.Flop:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.FlopHandValue);
                    filterSectionItemString = "Flop Hands";
                    collection = HandValueModel.FlopHandValuesCollection;
                    break;
                case Street.Turn:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.TurnHandValue);
                    filterSectionItemString = "Turn Hands";
                    collection = HandValueModel.TurnHandValuesCollection;
                    break;
                case Street.River:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.RiverHandValue);
                    filterSectionItemString = "River Hands";
                    collection = HandValueModel.RiverHandValuesCollection;
                    break;
                default:
                    return;
            }

            var selectedHandValues = collection.Where(x => x.IsChecked);

            if (selectedHandValues == null || selectedHandValues.Count() == 0)
            {
                filterSectionItem.IsActive = false;
                return;
            }


            var handValueItemsString = String.Join("+", selectedHandValues.Select(x => x.Name.Trim()));
            filterSectionItem.Name = String.Format("{0}={1}", filterSectionItemString, handValueItemsString);
            filterSectionItem.IsActive = true;
        }

        private void RemoveHandValueItem(FilterSectionItem param)
        {
            switch (param.ItemType)
            {
                case EnumFilterSectionItemType.FlopHandValue:
                    HandValueModel.ResetFlopHandValuesCollection();
                    break;
                case EnumFilterSectionItemType.TurnHandValue:
                    HandValueModel.ResetTurnHandValuesCollection();
                    break;
                case EnumFilterSectionItemType.RiverHandValue:
                    HandValueModel.ResetRiverHandValuesCollection();
                    break;
            }
        }

        #endregion

        #region Date Filter

        private void SetDateItems()
        {
            if (DateModel == null)
            {
                return;
            }

            FilterSectionItem filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.Date);
            switch (DateModel.DateFilterType.EnumDateRange)
            {
                case EnumDateFiterStruct.EnumDateFiter.Today:
                    filterSectionItem.Name = "Today";
                    filterSectionItem.IsActive = true;
                    break;
                case EnumDateFiterStruct.EnumDateFiter.ThisWeek:
                    filterSectionItem.Name = "This Week";
                    filterSectionItem.IsActive = true;
                    break;
                case EnumDateFiterStruct.EnumDateFiter.ThisMonth:
                    filterSectionItem.Name = "This Month";
                    filterSectionItem.IsActive = true;
                    break;
                case EnumDateFiterStruct.EnumDateFiter.LastMonth:
                    filterSectionItem.Name = "Last Month";
                    filterSectionItem.IsActive = true;
                    break;
                case EnumDateFiterStruct.EnumDateFiter.ThisYear:
                    filterSectionItem.Name = "This Year";
                    filterSectionItem.IsActive = true;
                    break;
                case EnumDateFiterStruct.EnumDateFiter.CustomDateRange:
                    if (DateModel.DateFilterType.DateFrom <= DateModel.DateFilterType.DateTo)
                        filterSectionItem.Name = DateModel.DateFilterType.DateFrom.Date.ToShortDateString()
                                               + " - "
                                               + DateModel.DateFilterType.DateTo.Date.ToShortDateString();
                    else
                        filterSectionItem.Name = DateModel.DateFilterType.DateTo.Date.ToShortDateString()
                                               + " - "
                                               + DateModel.DateFilterType.DateFrom.Date.ToShortDateString();


                    filterSectionItem.IsActive = true;
                    break;
                default:
                    filterSectionItem.IsActive = false;
                    break;
            }
        }

        private void RemoveDateItem()
        {
            DateModel?.ResetFilter();
        }

        #endregion

        #region Board Card Item (BoardTexture Filter)
        private void SetBoardCardItems(Street street)
        {
            FilterSectionItem filterSectionItem = null;
            IEnumerable<BoardCardItem> collection;
            string filterSectionItemString = string.Empty;

            switch (street)
            {
                case Street.Flop:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.FlopBoardCardItem);
                    filterSectionItemString = "Specific Flop";
                    collection = BoardTextureModel.FlopCardItemsCollection;
                    break;
                case Street.Turn:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.TurnBoardCardItem);
                    filterSectionItemString = "Specific Flop+Turn";
                    collection = BoardTextureModel.TurnCardItemsCollection;
                    break;
                case Street.River:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.RiverBoardCardItem);
                    filterSectionItemString = "Specific Full Board";
                    collection = BoardTextureModel.RiverCardItemsCollection;
                    break;
                default:
                    return;
            }

            var selectedHandValues = collection.Where(x => x.Suit != RangeCardSuit.None);

            if (selectedHandValues == null || selectedHandValues.Count() == 0)
            {
                filterSectionItem.IsActive = false;
                return;
            }


            var handValueItemsString = String.Join("+", selectedHandValues.Select(x => x.ToString().Trim()));
            filterSectionItem.Name = String.Format("{0}={1}", filterSectionItemString, handValueItemsString);
            filterSectionItem.IsActive = true;
        }

        private void RemoveBoardCardItem(FilterSectionItem param)
        {
            switch (param.ItemType)
            {
                case EnumFilterSectionItemType.FlopBoardCardItem:
                    BoardTextureModel.ResetFlopCardItemsCollection();
                    break;
                case EnumFilterSectionItemType.TurnBoardCardItem:
                    BoardTextureModel.ResetTurnCardItemsCollection();
                    break;
                case EnumFilterSectionItemType.RiverBoardCardItem:
                    BoardTextureModel.ResetRiverCardItemsCollection();
                    break;
            }
        }
        #endregion

        #region Board Texture  Item (BoardTexture Filter)
        private void SetBoardTextureItems(Street street)
        {
            FilterSectionItem filterSectionItem = null;
            IEnumerable<BoardTextureItem> collection;
            string filterSectionItemString = string.Empty;

            switch (street)
            {
                case Street.Flop:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.FlopBoardTextureItem);
                    filterSectionItemString = "Flop Texture";
                    collection = BoardTextureModel.FlopBoardTextureCollection;
                    break;
                case Street.Turn:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.TurnBoardTextureItem);
                    filterSectionItemString = "Turn Texture";
                    collection = BoardTextureModel.TurnBoardTextureCollection;
                    break;
                case Street.River:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.RiverBoardTextureItem);
                    filterSectionItemString = "River Texture";
                    collection = BoardTextureModel.RiverBoardTextureCollection;
                    break;
                default:
                    return;
            }

            var selectedHandValues = collection.Where(x => x.IsChecked);

            if (selectedHandValues == null || selectedHandValues.Count() == 0)
            {
                filterSectionItem.IsActive = false;
                return;
            }


            var handValueItemsString = String.Join("+", selectedHandValues.Select(x => x.ToString().Trim()));
            filterSectionItem.Name = String.Format("{0}={1}", filterSectionItemString, handValueItemsString);
            filterSectionItem.IsActive = true;
        }

        private void RemoveBoardTextureItem(FilterSectionItem param)
        {
            switch (param.ItemType)
            {
                case EnumFilterSectionItemType.FlopBoardTextureItem:
                    BoardTextureModel.ResetFlopBoardTextureCollection();
                    break;
                case EnumFilterSectionItemType.TurnBoardTextureItem:
                    BoardTextureModel.ResetTurnBoardTextureCollection();
                    break;
                case EnumFilterSectionItemType.RiverBoardTextureItem:
                    BoardTextureModel.ResetRiverBoardTextureCollection();
                    break;
            }
        }
        #endregion

        #region Hand Action Item (HandAction Filter)
        private void SetHandActionItems(Street street)
        {
            FilterSectionItem filterSectionItem = null;
            var button = HandActionModel.GetButonsCollectionForStreet(street).FirstOrDefault(x => x.IsChecked);
            IEnumerable<HandActionFilterItem> collection = null;

            string filterSectionItemString = string.Empty;

            switch (street)
            {
                case Street.Preflop:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.PreflopHandActionItem);
                    filterSectionItemString = "Preflop Action";
                    break;
                case Street.Flop:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.FlopHandActionItem);
                    filterSectionItemString = "Flop Action";
                    break;
                case Street.Turn:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.TurnHandActionItem);
                    filterSectionItemString = "Turn Action";
                    break;
                case Street.River:
                    filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.RiverHandActionItem);
                    filterSectionItemString = "River Action";
                    break;
                default:
                    return;
            }

            if (button == null)
            {
                filterSectionItem.IsActive = false;
                return;
            }

            collection = HandActionModel.GetItemsCollectionForStreet(street).Where(x => x.BeginningActionType == button.HandActionType);
            var selectedActions = collection.Where(x => x.IsChecked);
            if (selectedActions == null || selectedActions.Count() == 0)
            {
                filterSectionItem.IsActive = false;
                return;
            }

            var handActionItemsString = String.Join("+", selectedActions.Select(x => x.ToString().Trim()));
            filterSectionItem.Name = String.Format("{0}={1}", filterSectionItemString, handActionItemsString);
            filterSectionItem.IsActive = true;
        }

        private void RemoveHandActionItem(FilterSectionItem param)
        {
            switch (param.ItemType)
            {
                case EnumFilterSectionItemType.PreflopHandActionItem:
                    HandActionModel.ResetPreflopFilter();
                    break;
                case EnumFilterSectionItemType.FlopHandActionItem:
                    HandActionModel.ResetFlopFilter();
                    break;
                case EnumFilterSectionItemType.TurnHandActionItem:
                    HandActionModel.ResetTurnFilter();
                    break;
                case EnumFilterSectionItemType.RiverHandActionItem:
                    HandActionModel.ResetRiverFilter();
                    break;
            }
        }
        #endregion

        #region Omaha Hand Grid Item (OmahaHandGrid Filter)
        private void SetOmahaHandGridItems()
        {
            FilterSectionItem filterSectionItem = this.FilterSectionCollection.FirstOrDefault(x => x.ItemType == EnumFilterSectionItemType.OmahaHandGridItem);

            var selectedHandItems = OmahaHandGridModel.HandGridCollection.Where(x => x.IsChecked);
            if (selectedHandItems == null || selectedHandItems.Count() == 0)
            {
                filterSectionItem.IsActive = false;
                return;
            }

            var handItemsString = String.Join("+", selectedHandItems.Select(x => x.ToString().Trim()));
            filterSectionItem.Name = String.Format("Omaha Hole Cards={0}", handItemsString);
            filterSectionItem.IsActive = true;
        }

        private void RemoveOmahaHandGridItem()
        {
            OmahaHandGridModel.ResetHandGridCollection();
        }
        #endregion

        #region Quick Filter Item

        private void SetQuickFilterItems()
        {
            foreach (var triStateItem in QuickFilterModel.QuickFilterCollection)
            {
                var filterSectionItem = FilterSectionCollection.FirstOrDefault(x => !string.IsNullOrEmpty(x.Value) && x.Value == triStateItem.Name);

                var state = triStateItem.CurrentTriState;

                if (state == EnumTriState.Any && filterSectionItem != null)
                {
                    /* Make element not active instead of removing it because tooltip is losing its style after element is being removed */
                    filterSectionItem.IsActive = false;
                    continue;
                }

                if (state != EnumTriState.Any)
                {
                    FilterSectionItem newFilterSectionItem;
                    if (filterSectionItem != null)
                    {
                        newFilterSectionItem = filterSectionItem;
                    }
                    else
                    {
                        newFilterSectionItem = new FilterSectionItem() { ItemType = EnumFilterSectionItemType.QuickFilterItem, Value = triStateItem.Name };
                        this.FilterSectionCollection.Add(newFilterSectionItem);
                    }

                    newFilterSectionItem.Name = String.Format("{0}={1}", triStateItem.Name, state == EnumTriState.On ? "Yes" : "No");
                    newFilterSectionItem.IsActive = true;
                }
            }
        }

        private void RemoveQuickFilterItem(FilterSectionItem param)
        {
            if (param.ItemType != EnumFilterSectionItemType.QuickFilterItem || string.IsNullOrEmpty(param.Value))
            {
                return;
            }

            var filterItemValue = param.Value;

            var fastFilterItem = QuickFilterModel.QuickFilterCollection.FirstOrDefault(x => x.Name == filterItemValue);

            if (fastFilterItem != null)
            {
                fastFilterItem.CurrentTriState = EnumTriState.Any;
            }
        }

        #endregion

        #region Advanced Filter Item

        private void RemoveAdvancedFilterItem(FilterSectionItem param)
        {
            if (param.ItemType != EnumFilterSectionItemType.AdvancedFilterItem ||
                string.IsNullOrEmpty(param.Name))
            {
                return;
            }

            AdvancedFilterModel?.SelectedFilters.RemoveByCondition(x => x.ToolTip == param.Name);
        }

        #endregion

        #endregion
    }
}