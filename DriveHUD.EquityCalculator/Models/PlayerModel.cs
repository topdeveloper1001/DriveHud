﻿using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Resources;
using DriveHUD.EquityCalculator.Analyzer;
using DriveHUD.EquityCalculator.ViewModels;
using DriveHUD.ViewModels;
using HandHistories.Objects.Cards;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Events;
using Model.Extensions;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.EquityCalculator.Models
{
    public class PlayerModel : CardCollectionContainer
    {
        #region Fields

        private readonly int containerSize = 2;
        private string playerName = "Player";
        private double equityValue = 0.0;
        private double tiePrct = 0.0;
        private double winPrct = 0.0;
        private ObservableCollection<string> playerCards = new ObservableCollection<string>();

        #endregion

        #region Properties

        public override int ContainerSize
        {
            get
            {
                return containerSize;
            }
        }

        public string PlayerName
        {
            get
            {
                return playerName;
            }
            set
            {
                SetProperty(ref playerName, value);
            }
        }

        public ObservableCollection<string> PlayerCards
        {
            get
            {
                return playerCards;
            }
            set
            {
                SetProperty(ref playerCards, value);
            }
        }

        public double EquityValue
        {
            get
            {
                return equityValue;
            }
            set
            {
                SetProperty(ref equityValue, value);
            }
        }

        public double TiePrct
        {
            get
            {
                return tiePrct;
            }
            set
            {
                SetProperty(ref tiePrct, value);
            }
        }

        public double WinPrct
        {
            get
            {
                return winPrct;
            }
            set
            {
                SetProperty(ref winPrct, value);
            }
        }

        private int totalCombos;

        public int TotalCombos
        {
            get
            {
                return totalCombos;
            }
            set
            {
                SetProperty(ref totalCombos, value);
            }
        }

        private bool isTotalCombosVisible = true;

        public bool IsTotalCombosVisible
        {
            get
            {
                return isTotalCombosVisible;
            }
            private set
            {
                SetProperty(ref isTotalCombosVisible, value);
            }
        }

        private int foldCheckCombos;

        public int FoldCheckCombos
        {
            get
            {
                return foldCheckCombos;
            }
            set
            {
                SetProperty(ref foldCheckCombos, value);
            }
        }

        private decimal foldCheckPercentage;

        public decimal FoldCheckPercentage
        {
            get
            {
                return foldCheckPercentage;
            }
            set
            {
                SetProperty(ref foldCheckPercentage, value);
            }
        }

        private int callCombos;

        public int CallCombos
        {
            get
            {
                return callCombos;
            }
            set
            {
                SetProperty(ref callCombos, value);
            }
        }

        private decimal callPercentage;

        public decimal CallPercentage
        {
            get
            {
                return callPercentage;
            }
            set
            {
                SetProperty(ref callPercentage, value);
            }
        }

        private int bluffCombos;

        public int BluffCombos
        {
            get
            {
                return bluffCombos;
            }
            set
            {
                SetProperty(ref bluffCombos, value);
            }
        }

        private decimal bluffPercentage;

        public decimal BluffPercentage
        {
            get
            {
                return bluffPercentage;
            }
            set
            {
                SetProperty(ref bluffPercentage, value);
            }
        }

        private int valueBetCombos;

        public int ValueBetCombos
        {
            get
            {
                return valueBetCombos;
            }
            set
            {
                SetProperty(ref valueBetCombos, value);
            }
        }

        private decimal valueBetPercentage;

        public decimal ValueBetPercentage
        {
            get
            {
                return valueBetPercentage;
            }
            set
            {
                SetProperty(ref valueBetPercentage, value);
            }
        }

        #endregion

        #region ICommand

        public ICommand RemoveRangeCommand { get; private set; }

        #endregion

        public PlayerModel() : base()
        {
            RemoveRangeCommand = new RelayCommand(RemoveRange);
        }

        public PlayerModel(IEnumerable<CardModel> model) : base(model)
        {
            RemoveRangeCommand = new RelayCommand(RemoveRange);
        }

        public override void Reset()
        {
            base.Reset();
            PlayerName = "Player";
            PlayerCards.Clear();
            EquityValue = 0.0;
            TiePrct = 0.0;
            WinPrct = 0.0;
        }

        public override void SetCollection(IEnumerable<CardModel> model)
        {
            base.SetRanges(null);
            base.SetCollection(model);

            UpdatePlayerCardsData();
        }

        public override void SetRanges(IEnumerable<RangeSelectorItemViewModel> model)
        {
            base.SetCollection(null);
            base.SetRanges(model);

            UpdatePlayerCardsData();
        }

        public List<String> GetPlayersHand()
        {
            return GetPlayersHand(false);
        }

        public List<String> GetPlayersHand(bool withPercent)
        {
            var rangesList = Ranges.ToList();

            if (rangesList.Count > 0)
            {
                if (!withPercent)
                {
                    /* return only selected range hands */
                    var hands = new List<String>();

                    foreach (RangeSelectorItemViewModel hand in rangesList.Where(x => x.IsSelected))
                    {
                        if (hand.HandSuitsModelList.Where(x => x.IsVisible && x.IsSelected).Count() > 0)
                        {
                            hands.Add(hand.Caption);
                        }
                    }

                    if (hands.Count() > 0)
                    {
                        return hands;
                    }
                }
                else
                {
                    /* return full information (splitted range cards, likelihood) */
                    List<String> res = new List<string>();
                    foreach (RangeSelectorItemViewModel hand in rangesList.Where(x => x.IsSelected))
                    {
                        bool suited = hand.ItemType.Equals(RangeSelectorItemType.Suited);
                        bool pocketPair = hand.ItemType.Equals(RangeSelectorItemType.Pair);

                        List<String> handSuits = new List<String>();
                        foreach (var handSuit in hand.HandSuitsModelList.Where(x => x.IsVisible && x.IsSelected))
                        {
                            handSuits.Add(handSuit.HandSuit.ToString());
                        }

                        /* Ungroup hands with disabled suits */
                        if (
                            ((suited && !pocketPair && handSuits.Count != 4)
                            || (!suited && !pocketPair && handSuits.Count != 12)
                            || (pocketPair && handSuits.Count != 6)
                            ) && handSuits.Count > 0
                            )
                        {
                            foreach (String suit in handSuits)
                            {
                                res.Add(hand.Caption[0].ToString() + suit[0].ToString() + hand.Caption[1].ToString() + suit[1].ToString() + "(" + hand.LikelihoodPercent + ")");
                            }
                        }
                        /* Do not ungroup if all suits selected*/
                        else if (handSuits.Count > 0)
                            res.Add(hand.Caption + "(" + hand.LikelihoodPercent + ")");
                    }

                    return res;
                }
            }
            else if (Cards.Where(x => x.Rank != RangeCardRank.None && x.Suit != RangeCardSuit.None).Count() > 0)
            {
                /* Exact cards are selected */
                return new List<string>() { string.Concat(this.Cards.Select(x => x.ToString())) };
            }

            return new List<string>();
        }

        public void UpdateEquityData()
        {
            var ranges = Ranges?.OfType<EquityRangeSelectorItemViewModel>().ToArray();

            var totalCombos = 0;
            var foldCheckCombos = 0;
            var callCombos = 0;
            var bluffCombos = 0;
            var valueBetCombos = 0;

            if (ranges.Length != 0)
            {
                foreach (var range in ranges)
                {
                    foldCheckCombos += range.FoldCheckCombos;
                    callCombos += range.CallCombos;
                    bluffCombos += range.BluffCombos;
                    valueBetCombos += range.ValueBetCombos;
                    totalCombos += range.Combos;
                }
            }
            // known cards are equal to 1 combo
            else if (Cards.All(x => x.Validate()))
            {
                totalCombos = 1;
            }

            TotalCombos = totalCombos;
            FoldCheckCombos = foldCheckCombos;
            CallCombos = callCombos;
            BluffCombos = bluffCombos;
            ValueBetCombos = valueBetCombos;

            IsTotalCombosVisible = FoldCheckCombos == CallCombos &&
                CallCombos == BluffCombos && BluffCombos == ValueBetCombos && ValueBetCombos == 0;

            FoldCheckPercentage = GetEquityRangePercentage(foldCheckCombos, totalCombos);
            CallPercentage = GetEquityRangePercentage(callCombos, totalCombos);
            BluffPercentage = GetEquityRangePercentage(bluffCombos, totalCombos);
            ValueBetPercentage = GetEquityRangePercentage(valueBetCombos, totalCombos);
        }

        private string bluffToValueRatioWarning;

        public string BluffToValueRatioWarning
        {
            get
            {
                return bluffToValueRatioWarning;
            }
            private set
            {
                SetProperty(ref bluffToValueRatioWarning, value);
            }
        }

        public void CheckBluffToValueBetRatio(int opponentsCount, Street street)
        {
            if (BluffToValueRatioCalculator.CheckRatio(opponentsCount, BluffCombos, ValueBetCombos, street, out int[] increaseBluffBy, out int[] increaseValueBy))
            {
                BluffToValueRatioWarning = string.Empty;
                return;
            }

            if (BluffCombos == 0)
            {
                BluffToValueRatioWarning = string.Format(CommonResourceManager.Instance.GetResourceString("Common_EquityCalculator_WarningMessageNoBluffCombos"),
                    street, BluffToValueRatioCalculator.GetRecommendedRange(opponentsCount, street));

                return;
            }

            if (ValueBetCombos == 0)
            {
                BluffToValueRatioWarning = string.Format(CommonResourceManager.Instance.GetResourceString("Common_EquityCalculator_WarningMessageNoValueCombos"),
                  street, BluffToValueRatioCalculator.GetRecommendedRange(opponentsCount, street));

                return;
            }

            var rangeText = increaseBluffBy[0] > 0 ?
                CommonResourceManager.Instance.GetResourceString("Common_EquityCalculator_Bluff") :
                CommonResourceManager.Instance.GetResourceString("Common_EquityCalculator_Value");

            var increaseBy = increaseBluffBy[0] > 0 ? increaseBluffBy : increaseValueBy;
            var increaseByText = string.Join("-", increaseBy.Distinct().OrderBy(x => x).ToArray());

            BluffToValueRatioWarning = string.Format(CommonResourceManager.Instance.GetResourceString("Common_EquityCalculator_WarningMessageWithRange"),
                street, BluffToValueRatioCalculator.GetRecommendedRange(opponentsCount, street), rangeText, increaseByText);
        }

        private decimal GetEquityRangePercentage(int value, int total)
        {
            return value == total || total == 0 ?
                (decimal)value / 1326 :
                (decimal)value / total;
        }

        private void UpdatePlayerCardsData()
        {
            PlayerCards.Clear();

            var hands = CardHelper.GetHandsFormatted(GetPlayersHand());

            foreach (var hand in hands)
            {
                PlayerCards.Add(hand);
            }

            UpdateEquityData();
        }

        #region ICommand Implementation

        private void RemoveRange(object obj)
        {
            if (obj != null)
            {
                var range = CardHelper.GetHandsUnFormatted(obj.ToString());

                if (Ranges.Count() != 0)
                {
                    var ranges = (from rangeItem in Ranges
                                  join cards in range on rangeItem.Caption equals cards into gj
                                  from grouped in gj.DefaultIfEmpty()
                                  where grouped == null
                                  select rangeItem).ToArray();

                    SetRanges(ranges);
                }
                else
                {
                    if (PlayerCards.First() == range.First())
                    {
                        SetCollection(null);
                    }
                }

                ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<EquityCalculatorRangeRemovedEvent>().Publish(new EventArgs());
            }
        }

        #endregion
    }
}