//-----------------------------------------------------------------------
// <copyright file="IHandHistoryParser.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;

namespace HandHistories.Parser.Parsers.Base
{
    public interface IHandHistoryParser : IHandHistorySummaryParser
    {
        HandHistory ParseFullHandHistory(string handText, bool rethrowExceptions = false);
        
        List<HandAction> ParseHandActions(string handText);

        PlayerList ParsePlayers(string handText);
        
        BoardCards ParseCommunityCards(string handText);
    }
}