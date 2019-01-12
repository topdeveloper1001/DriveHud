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

using DriveHUD.Common.Linq;
using DriveHUD.EquityCalculator.Models;
using DriveHUD.ViewModels;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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

        public void SetEquitySelectionMode(EquitySelectionMode? equitySelectionMode)
        {
            HandSuitsModelList
                .Where(p => p.IsVisible)
                .ForEach(p => p.SelectionMode = equitySelectionMode ?? DriveHUD.ViewModels.EquitySelectionMode.None);

            EquitySelectionMode = equitySelectionMode;
        }

        private EquitySelectionMode? equitySelectionMode;

        public EquitySelectionMode? EquitySelectionMode
        {
            get
            {
                return equitySelectionMode;
            }
            private set
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
            private set
            {
                SetProperty(ref combos, value);
            }
        }

        private int foldCheckCombos;

        public int FoldCheckCombos
        {
            get
            {
                return foldCheckCombos;
            }
            private set
            {
                SetProperty(ref foldCheckCombos, value);
            }
        }


        private int callCombos;

        public int CallCombos
        {
            get
            {
                return callCombos;
            }
            private set
            {
                SetProperty(ref callCombos, value);
            }
        }

        private int bluffCombos;

        public int BluffCombos
        {
            get
            {
                return bluffCombos;
            }
            private set
            {
                SetProperty(ref bluffCombos, value);
            }
        }

        private int valueBetCombos;

        public int ValueBetCombos
        {
            get
            {
                return valueBetCombos;
            }
            private set
            {
                SetProperty(ref valueBetCombos, value);
            }
        }

        private IEnumerable<CardModel> usedCards;

        public IEnumerable<CardModel> UsedCards
        {
            get
            {
                return usedCards;
            }
            set
            {
                SetProperty(ref usedCards, value);
                RefreshCombos();
            }
        }

        public override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }
            set
            {
                if (!value && value != IsSelected)
                {
                    SetEquitySelectionMode(null);
                    HandSuitsModelList.ForEach(x => x.IsVisible = false);
                    RefreshCombos();
                }

                base.IsSelected = value;
            }
        }

        #endregion

        public void RefreshCombos()
        {
            var combos = Enum.GetValues(typeof(EquitySelectionMode)).Cast<EquitySelectionMode>().ToDictionary(x => x, x => 0);

            var deadCards = usedCards?.Select(x => x.ToString()).ToArray() ?? new string[0];

            var selectedSuits = HandSuitsModelList.Where(x => x.IsSelected && x.IsVisible)
                .Select(x => new { x.SelectionMode, HandSuit = x.HandSuit.ToString() })
                .ToArray();

            var suits = new[] { RangeCardSuit.Clubs, RangeCardSuit.Diamonds, RangeCardSuit.Hearts, RangeCardSuit.Spades };

            bool checkCombos(string cardCombo, string suitCombo, out EquitySelectionMode selectionMode)
            {
                selectionMode = DriveHUD.ViewModels.EquitySelectionMode.None;

                var result = deadCards.Length == 0 || deadCards.All(x => !cardCombo.Contains(x));

                if (!result)
                {
                    return false;
                }

                var suit = selectedSuits.FirstOrDefault(x => suitCombo.Equals(x.HandSuit, StringComparison.OrdinalIgnoreCase));

                if (suit == null)
                {
                    return false;
                }

                selectionMode = suit.SelectionMode;

                return true;
            }

            if (ItemType == RangeSelectorItemType.Pair)
            {
                var rank = FisrtCard.ToRankString();

                for (var i = 0; i < suits.Length; i++)
                {
                    for (var j = i + 1; j < suits.Length; j++)
                    {
                        var suit1 = suits[i].ToSuitString();
                        var suit2 = suits[j].ToSuitString();

                        var cardCombo = $"{rank}{suit1}{rank}{suit2}";
                        var suitCombo = $"{suit1}{suit2}";

                        if (checkCombos(cardCombo, suitCombo, out EquitySelectionMode mode))
                        {
                            combos[mode]++;
                        }
                    }
                }
            }
            else if (ItemType == RangeSelectorItemType.Suited)
            {
                var rank1 = FisrtCard.ToRankString();
                var rank2 = SecondCard.ToRankString();

                for (var i = 0; i < suits.Length; i++)
                {
                    var suit = suits[i].ToSuitString();
                    var cardCombo = $"{rank1}{suit}{rank2}{suit}";
                    var suitCombo = $"{suit}{suit}";

                    if (checkCombos(cardCombo, suitCombo, out EquitySelectionMode mode))
                    {
                        combos[mode]++;
                    }
                }
            }
            else if (ItemType == RangeSelectorItemType.OffSuited)
            {
                var rank1 = FisrtCard.ToRankString();
                var rank2 = SecondCard.ToRankString();

                for (var i = 0; i < suits.Length; i++)
                {
                    for (var j = 0; j < suits.Length; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        var suit1 = suits[i].ToSuitString();
                        var suit2 = suits[j].ToSuitString();

                        var cardCombo = $"{rank1}{suit1}{rank2}{suit2}";
                        var suitCombo = $"{suit1}{suit2}";

                        if (checkCombos(cardCombo, suitCombo, out EquitySelectionMode mode))
                        {
                            combos[mode]++;
                        }
                    }
                }
            }

            FoldCheckCombos = combos[DriveHUD.ViewModels.EquitySelectionMode.FoldCheck];
            CallCombos = combos[DriveHUD.ViewModels.EquitySelectionMode.Call];
            BluffCombos = combos[DriveHUD.ViewModels.EquitySelectionMode.Bluff];
            ValueBetCombos = combos[DriveHUD.ViewModels.EquitySelectionMode.ValueBet];

            Combos = combos.Values.Sum();

            RaisePropertyChanged(nameof(Combos));
        }

        public override void HandUpdate()
        {
            base.HandUpdate();
            RefreshCombos();
        }
    }
}