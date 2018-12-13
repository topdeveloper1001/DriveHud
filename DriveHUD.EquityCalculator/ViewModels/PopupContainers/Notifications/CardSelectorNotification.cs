//-----------------------------------------------------------------------
// <copyright file="CardSelectorNotification.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.EquityCalculator.Models;
using Prism.Interactivity.InteractionRequest;
using System.Collections.Generic;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public enum CardSelectorType
    {
        BoardSelector,
        PlayerSelector
    }

    internal enum CardSelectorReturnType
    {
        Cards,
        Range
    }

    public class CardSelectorNotification : Confirmation
    {
        public CardSelectorNotification() : base() { }

        #region Properties

        internal object Source { get; set; }

        internal ICardCollectionContainer CardsContainer { get; set; }

        internal CardSelectorType SelectorType { get; set; } = CardSelectorType.BoardSelector;

        internal IEnumerable<CardModel> UsedCards { get; set; }

        internal IEnumerable<CardModel> BoardCards { get; set; }

        internal CardSelectorReturnType ReturnType { get; set; } = CardSelectorReturnType.Cards;

        public EquityCalculatorMode EquityCalculatorMode { get; set; }

        #endregion      
    }
}