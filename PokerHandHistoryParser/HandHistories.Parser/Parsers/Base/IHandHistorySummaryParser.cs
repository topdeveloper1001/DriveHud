//-----------------------------------------------------------------------
// <copyright file="IHandHistorySummaryParser.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using System;
using System.Collections.Generic;

namespace HandHistories.Parser.Parsers.Base
{
    public interface IHandHistorySummaryParser
    {
        EnumPokerSites SiteName { get; }
        
        IEnumerable<string> SplitUpMultipleHands(string rawHandHistories);

        HandHistorySummary ParseFullHandSummary(string handText, bool rethrowExceptions = false);

        int ParseDealerPosition(string handText);
        
        DateTime ParseDateUtc(string handText);

        long ParseHandId(string handText);

        string ParseTableName(string handText);

        GameDescriptor ParseGameDescriptor(string handText);

        SeatType ParseSeatType(string handText);

        GameType ParseGameType(string handText);

        TableType ParseTableType(string handText);

        Limit ParseLimit(string handText);

        int ParseNumPlayers(string handText);        

        /// <summary>
        /// An initial bit of verification to check if the hand text
        /// is valid. For instance Party hands must contain ' wins '.
        /// </summary>
        /// <param name="handText">The entire hand text.</param>
        /// <returns>True if the hand is valid. False if not.</returns>
        bool IsValidHand(string handText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handText"></param>
        /// <param name="isCancelled"></param>
        /// <returns>True if the hand is valid, false if not. Outs the canceled state.</returns>
        bool IsValidOrCanceledHand(string handText, out bool isCancelled);
    }
}