//-----------------------------------------------------------------------
// <copyright file="IThreeStateParser.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using System.Collections.Generic;

namespace HandHistories.Parser.Parsers.Base
{
    /// <summary>
    /// A ThreeStateParser splits the parsing of hand actions into 3 phases, Blinds, Actions and Showdown
    /// The interface is used for better unit testing of a 3StateParser
    /// </summary>
    internal interface IThreeStateParser
    {
        int ParseBlindActions(string[] handLines, ref List<HandAction> handActions, int firstActionIndex);

        int ParseGameActions(string[] handLines, ref List<HandAction> handActions, int firstActionIndex, out Street street);

        void ParseShowDown(string[] handLines, ref List<HandAction> handActions, int firstActionIndex, GameType gameType);
    }
}