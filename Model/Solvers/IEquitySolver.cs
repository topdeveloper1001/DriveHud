//-----------------------------------------------------------------------
// <copyright file="IEquitySolver.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using Model.Enums;
using System.Collections.Generic;

namespace Model.Solvers
{
    public interface IEquitySolver
    {
        Dictionary<string, EquityData> CalculateEquity(HandHistory handHistory);

        decimal[] CalculateEquity(HoleCards[] playersCards, HandHistories.Objects.Cards.Card[] boardCards, HoleCards[] dead, GeneralGameTypeEnum gameType);
    }
}