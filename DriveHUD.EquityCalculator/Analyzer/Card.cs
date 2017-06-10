//-----------------------------------------------------------------------
// <copyright file="Card.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class Card
    {
        internal static string[] AllCards = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A" };
        internal static List<string> AllCardsList = new List<string>(AllCards);
        internal static Hashtable CardValues = new Hashtable();

        internal static char[] CardSuit = new char[]
        {
            'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h',
            'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd',
            'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c', 'c',
            's', 's', 's', 's', 's', 's', 's', 's', 's', 's', 's', 's', 's'
        };

        internal static char[] CardName = new char[]
        {
            '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A',
            '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A',
            '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A',
            '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'
        };

        internal static void Init()
        {
            for (int i = 0; i < 13; i++)
            {
                CardValues.Add(AllCards[i], i);
            }
        }
    }
}